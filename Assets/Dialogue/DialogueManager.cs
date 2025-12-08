using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public List<string> events = new List<string>();
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public string eventName;      // For <EventName> format
    public string conversationName; // For {ConvoName} format
}

[System.Serializable]
public class Conversation
{
    public string name;
    public List<DialogueLine> lines = new List<DialogueLine>();
    public List<DialogueChoice> choices = new List<DialogueChoice>();
    public Dictionary<string, string> swapInstructions = new Dictionary<string, string>();

    public string nextConversation = null; // auto-next conversation
}

[System.Serializable]
public class CharacterMapping
{
    public string characterName;
    public GameObject characterObject;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject dialogueBox;
    public TMP_Text characterNameText;
    public TMP_Text characterSpeechText;

    [Header("Choice UI")]
    public Transform choiceParent;
    public GameObject choicePrefab;

    [Header("Settings")]
    public float lettersPerSecond = 30f;

    [Header("Dialogue Files (Resources paths)")]
    public List<string> dialogueFilePaths = new List<string>();

    [Header("Character Mappings")]
    public List<CharacterMapping> characterMappings = new List<CharacterMapping>();

    [Header("Event Handler")]
    public GameObject eventHandlerObject;

    private List<Conversation> conversations = new List<Conversation>();
    private Conversation currentConversation;
    private int dialogueIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool inConversation = false;
    private bool waitingForChoice = false;

    // Track which speakers were disabled during this conversation
    private List<Speaker> disabledSpeakers = new List<Speaker>();

    // Cache for dialogue events
    private Dictionary<string, DialogueEvent> eventCache = new Dictionary<string, DialogueEvent>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadAllDialogueFiles();
        CacheDialogueEvents();
    }

    private void CacheDialogueEvents()
    {
        if (eventHandlerObject == null)
        {
            Debug.LogWarning("No event handler object assigned to DialogueManager");
            return;
        }

        DialogueEvent[] events = eventHandlerObject.GetComponentsInChildren<DialogueEvent>();
        foreach (DialogueEvent evt in events)
        {
            eventCache[evt.gameObject.name] = evt;
        }
    }

    private void LoadAllDialogueFiles()
    {
        foreach (string path in dialogueFilePaths)
        {
            LoadDialogueFileFromResources(path);
        }
    }

    private void LoadDialogueFileFromResources(string resourcePath)
    {
        string cleanPath = resourcePath.Replace(".txt", "");
        TextAsset dialogueFile = Resources.Load<TextAsset>(cleanPath);

        if (dialogueFile == null)
        {
            Debug.LogError("Dialogue file not found in Resources at: " + resourcePath);
            return;
        }

        string text = dialogueFile.text;
        string[] convoBlocks = text.Split(new string[] { "###" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string block in convoBlocks)
        {
            ParseConversationBlock(block.Trim());
        }
    }

    private void ParseConversationBlock(string block)
{
    // Split by === to get conversation name and content
    string[] sections = block.Split(new string[] { "===" }, System.StringSplitOptions.None);
    if (sections.Length < 2) return;

    string convoName = sections[0].Trim();
    if (string.IsNullOrEmpty(convoName)) return;

    // Dialogue section (everything between first === and before last ===)
    string dialogueSection = sections[1].Trim();

    // Post-dialogue section (everything after the first === except dialogueSection)
    string postDialogueSection = "";
    if (sections.Length > 2)
    {
        postDialogueSection = string.Join("\n", sections, 2, sections.Length - 2).Trim();
    }

    Conversation convo = new Conversation();
    convo.name = convoName;

    // Parse choices if present (??? separator)
    string[] dialogueSplit = dialogueSection.Split(new string[] { "???" }, System.StringSplitOptions.None);
    string actualDialogue = dialogueSplit[0].Trim();
    string choicesSection = dialogueSplit.Length > 1 ? dialogueSplit[1].Trim() : "";

    if (!string.IsNullOrEmpty(choicesSection))
    {
        string[] choiceLines = choicesSection.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string choiceLine in choiceLines)
        {
            string trimmed = choiceLine.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            DialogueChoice choice = new DialogueChoice();

            // Match [ChoiceText]<EventName> or [ChoiceText]{ConvoName}
            Match eventMatch = Regex.Match(trimmed, @"^\[(.*?)\]<([^>]+)>$");
            Match convoMatch = Regex.Match(trimmed, @"^\[(.*?)\]\{(.*?)\}$");

            if (eventMatch.Success)
            {
                choice.choiceText = eventMatch.Groups[1].Value.Trim();
                choice.eventName = eventMatch.Groups[2].Value.Trim();
                convo.choices.Add(choice);
            }
            else if (convoMatch.Success)
            {
                choice.choiceText = convoMatch.Groups[1].Value.Trim();
                choice.conversationName = convoMatch.Groups[2].Value.Trim();
                convo.choices.Add(choice);
            }
        }
    }

    // Parse post-dialogue section for swap instructions and next conversation
    if (!string.IsNullOrEmpty(postDialogueSection))
    {
        string[] postLines = postDialogueSection.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in postLines)
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine)) continue;

            // Swap instruction [Character]{ConvoName}
            Match swapMatch = Regex.Match(trimmedLine, @"^\[(.*?)\]\{(.*?)\}$");
            if (swapMatch.Success)
            {
                string characterName = swapMatch.Groups[1].Value.Trim();
                string newConvoName = swapMatch.Groups[2].Value.Trim();
                convo.swapInstructions[characterName] = newConvoName;
                continue;
            }

            // Next conversation >>ConvoName
            Match nextConvoMatch = Regex.Match(trimmedLine, @"^>>\s*(.+)$");
            if (nextConvoMatch.Success)
            {
                convo.nextConversation = nextConvoMatch.Groups[1].Value.Trim();
            }
        }
    }

    // Now parse dialogue lines separated by ---
    string[] dialogueBlocks = actualDialogue.Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);

    foreach (string dlgBlock in dialogueBlocks)
    {
        string[] dlgLines = dlgBlock.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        if (dlgLines.Length == 0) continue;

        DialogueLine dialogueLine = new DialogueLine();
        int lineIndex = 0;

        // Extract events at the start
        while (lineIndex < dlgLines.Length)
        {
            string line = dlgLines[lineIndex].Trim();
            string withoutEvents = Regex.Replace(line, @"<([^>]+)>", "").Trim();

            if (string.IsNullOrEmpty(withoutEvents))
            {
                MatchCollection eventMatches = Regex.Matches(line, @"<([^>]+)>");
                foreach (Match match in eventMatches)
                    dialogueLine.events.Add(match.Groups[1].Value);

                lineIndex++;
            }
            else
            {
                break;
            }
        }

        if (lineIndex >= dlgLines.Length) continue;

        string firstLine = dlgLines[lineIndex];

        // Remaining events in the first line
        MatchCollection remainingEvents = Regex.Matches(firstLine, @"<([^>]+)>");
        foreach (Match match in remainingEvents)
            dialogueLine.events.Add(match.Groups[1].Value);

        // Extract speaker
        firstLine = Regex.Replace(firstLine, @"<([^>]+)>", "").Trim();
        Match speakerMatch = Regex.Match(firstLine, @"\[(.*?)\]");
        dialogueLine.speaker = speakerMatch.Success ? speakerMatch.Groups[1].Value.Trim() : "";

        // Extract text
        if (dlgLines.Length > lineIndex + 1)
            dialogueLine.text = string.Join("\n", dlgLines, lineIndex + 1, dlgLines.Length - lineIndex - 1).Trim();
        else
            dialogueLine.text = firstLine.Replace("[" + dialogueLine.speaker + "]", "").Trim();

        convo.lines.Add(dialogueLine);
    }

    conversations.Add(convo);
}



    public void StartConversation(string conversationName)
    {
        currentConversation = conversations.Find(c => c.name == conversationName);
        if (currentConversation == null)
        {
            Debug.LogError("Conversation not found: " + conversationName);
            return;
        }

        inConversation = true;
        waitingForChoice = false;
        disabledSpeakers.Clear();

        foreach (var swap in currentConversation.swapInstructions)
        {
            CharacterMapping mapping = characterMappings.Find(c => c.characterName == swap.Key);
            if (mapping != null)
            {
                Speaker sp = mapping.characterObject.GetComponent<Speaker>();
                if (sp != null)
                {
                    sp.conversationName = swap.Value;
                    sp.canInteract = false;
                    if (!string.IsNullOrEmpty(swap.Value))
                        disabledSpeakers.Add(sp);
                }
            }
        }

        dialogueIndex = 0;
        dialogueBox.SetActive(true);
        ShowNextLine();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && inConversation && !waitingForChoice)
        {
            if (isTyping)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                DialogueLine currentLine = currentConversation.lines[dialogueIndex - 1];
                characterSpeechText.text = currentLine.text;
                isTyping = false;
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    private void ShowNextLine()
    {
        if (currentConversation == null || dialogueIndex >= currentConversation.lines.Count)
        {
            if (currentConversation != null && currentConversation.choices.Count > 0)
            {
                ShowChoices();
            }
            else
            {
                EndConversation();
            }
            return;
        }

        DialogueLine line = currentConversation.lines[dialogueIndex];

        foreach (string eventName in line.events)
            ExecuteEvent(eventName);

        characterNameText.text = line.speaker;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line.text));

        dialogueIndex++;
    }

    private void ShowChoices()
    {
        waitingForChoice = true;
        // Keep previous dialogue visible
        ThirdPersonCam.Instance.cameraLocked = false;

        foreach (DialogueChoice choice in currentConversation.choices)
        {
            GameObject choiceObj = Instantiate(choicePrefab, choiceParent);
            TMP_Text choiceText = choiceObj.GetComponentInChildren<TMP_Text>();
            if (choiceText != null)
                choiceText.text = choice.choiceText;

            Button button = choiceObj.GetComponent<Button>();
            if (button != null)
            {
                DialogueChoice capturedChoice = choice;
                button.onClick.AddListener(() => OnChoiceSelected(capturedChoice));
            }
        }
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        foreach (Transform child in choiceParent)
            Destroy(child.gameObject);

        waitingForChoice = false;

        if (!string.IsNullOrEmpty(choice.eventName))
        {
            ExecuteEvent(choice.eventName);
            EndConversation();
        }
        else if (!string.IsNullOrEmpty(choice.conversationName))
        {
            EndConversation();
            StartConversation(choice.conversationName);
        }
        else
        {
            EndConversation();
        }

        ThirdPersonCam.Instance.cameraLocked = true;
    }

    private void ExecuteEvent(string eventName)
    {
        if (eventCache.TryGetValue(eventName, out DialogueEvent evt))
            evt.Execute();
        else
            Debug.LogWarning($"Event '{eventName}' not found in event handler object");
    }

    private IEnumerator TypeText(string text)
    {
        characterSpeechText.text = "";
        isTyping = true;
        foreach (char c in text)
        {
            characterSpeechText.text += c;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }

    private void EndConversation()
    {
        dialogueBox.SetActive(false);

        // Re-enable disabled speakers
        foreach (Speaker sp in disabledSpeakers)
            if (sp != null) sp.canInteract = true;

        disabledSpeakers.Clear();
        inConversation = false;

        string nextConvo = currentConversation?.nextConversation;
        currentConversation = null;
        dialogueIndex = 0;
        waitingForChoice = false;

        foreach (Transform child in choiceParent)
            Destroy(child.gameObject);

        // Auto-start next conversation if specified
        if (!string.IsNullOrEmpty(nextConvo))
        {
            StartConversation(nextConvo);
        }
    }
}
