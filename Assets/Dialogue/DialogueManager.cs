using UnityEngine;
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
public class Conversation
{
    public string name;
    public List<DialogueLine> lines = new List<DialogueLine>();
    public Dictionary<string, string> swapInstructions = new Dictionary<string, string>();
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

    [Header("Settings")]
    public float lettersPerSecond = 30f;

    [Header("Dialogue Files (Resources paths)")]
    public List<string> dialogueFilePaths = new List<string>();

    [Header("Character Mappings")]
    public List<CharacterMapping> characterMappings = new List<CharacterMapping>();

    private List<Conversation> conversations = new List<Conversation>();
    private Conversation currentConversation;
    private int dialogueIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool inConversation = false;

    // Track which speakers were disabled during this conversation
    private List<Speaker> disabledSpeakers = new List<Speaker>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadAllDialogueFiles();
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
        Debug.Log($"Loading file: {resourcePath} - Found {convoBlocks.Length} conversation blocks");
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
    if (string.IsNullOrEmpty(convoName)) return; // Skip empty blocks
    
    // Get dialogue section (between first and last ===)
    string dialogueSection = sections[1].Trim();
    
    // Get swap instruction section (after last ===, if exists)
    string swapSection = sections.Length >= 3 ? sections[2].Trim() : "";

    Conversation convo = new Conversation();
    convo.name = convoName;

    // Check swap section for swap instructions (only if not empty)
    if (!string.IsNullOrEmpty(swapSection))
    {
        string[] swapLines = swapSection.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in swapLines)
        {
            string trimmedLine = line.Trim();
            // Skip lines that are just separators or empty
            if (string.IsNullOrEmpty(trimmedLine)) continue;
            
            Match swapMatch = Regex.Match(trimmedLine, @"^\[(.*?)\]\{(.*?)\}$");
            if (swapMatch.Success)
            {
                string characterName = swapMatch.Groups[1].Value.Trim();
                string newConvoName = swapMatch.Groups[2].Value.Trim();
                convo.swapInstructions[characterName] = newConvoName;
                Debug.Log($"Found swap instruction: {characterName} -> {newConvoName}");
            }
        }
    }

    // Now parse dialogue blocks separated by ---
    string[] dialogueBlocks = dialogueSection.Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);

    foreach (string dlgBlock in dialogueBlocks)
    {
        string[] dlgLines = dlgBlock.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        if (dlgLines.Length == 0) continue;

        DialogueLine dialogueLine = new DialogueLine();

        // Extract events from first line (e.g., <EventName>)
        MatchCollection eventMatches = Regex.Matches(dlgLines[0], @"<([^>]+)>");
        foreach (Match match in eventMatches)
            dialogueLine.events.Add(match.Groups[1].Value);

        // Remove events from first line and extract speaker
        string firstLine = Regex.Replace(dlgLines[0], @"<([^>]+)>", "").Trim();
        Match speakerMatch = Regex.Match(firstLine, @"\[(.*?)\]");
        dialogueLine.speaker = speakerMatch.Success ? speakerMatch.Groups[1].Value.Trim() : "";

        // Extract dialogue text
        if (dlgLines.Length > 1)
        {
            // Multi-line dialogue
            dialogueLine.text = string.Join("\n", dlgLines, 1, dlgLines.Length - 1).Trim();
        }
        else
        {
            // Single-line dialogue (remove speaker tag)
            dialogueLine.text = firstLine.Replace("[" + dialogueLine.speaker + "]", "").Trim();
        }

        convo.lines.Add(dialogueLine);
    }

    conversations.Add(convo);
    Debug.Log($"Loaded conversation: {convo.name} with {convo.lines.Count} lines and {convo.swapInstructions.Count} swap instructions");
    
    // Debug: List all loaded conversation names
    if (convo.swapInstructions.Count > 0)
    {
        foreach (var swap in convo.swapInstructions)
        {
            Debug.Log($"  - Swap: {swap.Key} will trigger conversation '{swap.Value}'");
        }
    }
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
        // Apply swap instructions and disable interaction
disabledSpeakers.Clear();
foreach (var swap in currentConversation.swapInstructions)
{
    CharacterMapping mapping = characterMappings.Find(c => c.characterName == swap.Key);
    if (mapping != null)
    {
        Speaker sp = mapping.characterObject.GetComponent<Speaker>();
        if (sp != null)
        {
            sp.conversationName = swap.Value;   // swap conversation
            sp.canInteract = false;             // disable interaction immediately

            // Only track for re-enable if swap target is NOT empty
            if (!string.IsNullOrEmpty(swap.Value))
                disabledSpeakers.Add(sp);

            Debug.Log($"Assigned conversation '{swap.Value}' and disabled interaction for {swap.Key}");
        }
    }
}



        dialogueIndex = 0;
        dialogueBox.SetActive(true);
        ShowNextLine();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && inConversation)
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
            EndConversation();
            return;
        }

        DialogueLine line = currentConversation.lines[dialogueIndex];

        foreach (string evt in line.events)
            Debug.Log("Trigger Event: " + evt);

        characterNameText.text = line.speaker;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line.text));

        dialogueIndex++;
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
        currentConversation = null;
        dialogueIndex = 0;

        // Re-enable interaction for all disabled speakers
        foreach (Speaker sp in disabledSpeakers)
            if (sp != null) sp.canInteract = true;

        disabledSpeakers.Clear();
        inConversation = false;

        Debug.Log("Conversation Ended");
    }
}
