using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Characters.Interactables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.Dialogue
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        public string text;
        public List<string> events = new List<string>();
        public List<string> flagsToSet = new List<string>(); // Flags to set when this line is shown
    }

    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public string eventName;      // For <EventName> format
        public string conversationName; // For {ConvoName} format
        public string requiredFlag;   // For (FlagName) format - flag must be true to show
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

                    // Match patterns with optional (FlagName):
                    // [ChoiceText](FlagName)<EventName> or [ChoiceText](FlagName){ConvoName}
                    // [ChoiceText]<EventName> or [ChoiceText]{ConvoName}
                    
                    Match eventMatch = Regex.Match(trimmed, @"^\[(.*?)\](?:\(([^)]+)\))?<([^>]+)>$");
                    Match convoMatch = Regex.Match(trimmed, @"^\[(.*?)\](?:\(([^)]+)\))?\{(.*?)\}$");

                    if (eventMatch.Success)
                    {
                        choice.choiceText = eventMatch.Groups[1].Value.Trim();
                        choice.requiredFlag = eventMatch.Groups[2].Success ? eventMatch.Groups[2].Value.Trim() : null;
                        choice.eventName = eventMatch.Groups[3].Value.Trim();
                        convo.choices.Add(choice);
                    }
                    else if (convoMatch.Success)
                    {
                        choice.choiceText = convoMatch.Groups[1].Value.Trim();
                        choice.requiredFlag = convoMatch.Groups[2].Success ? convoMatch.Groups[2].Value.Trim() : null;
                        choice.conversationName = convoMatch.Groups[3].Value.Trim();
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

                // Extract events and flags at the start
                while (lineIndex < dlgLines.Length)
                {
                    string line = dlgLines[lineIndex].Trim();
                    
                    // Check for flags first
                    MatchCollection flagMatches = Regex.Matches(line, @">>!F\s+(\S+)");
                    bool hasFlags = flagMatches.Count > 0;
                    
                    string withoutEventsAndFlags = Regex.Replace(line, @"<([^>]+)>", "").Trim();
                    withoutEventsAndFlags = Regex.Replace(withoutEventsAndFlags, @">>!F\s+\S+", "").Trim();

                    if (string.IsNullOrEmpty(withoutEventsAndFlags))
                    {
                        // Extract events
                        MatchCollection eventMatches = Regex.Matches(line, @"<([^>]+)>");
                        foreach (Match match in eventMatches)
                            dialogueLine.events.Add(match.Groups[1].Value);

                        // Extract flags
                        foreach (Match match in flagMatches)
                        {
                            string flagName = match.Groups[1].Value.Trim();
                            dialogueLine.flagsToSet.Add(flagName);
                            Debug.Log($"Parsed flag to set: {flagName}");
                        }

                        lineIndex++;
                    }
                    else
                    {
                        break;
                    }
                }

                // If we only have events/flags and no more lines, still add the dialogue line
                if (lineIndex >= dlgLines.Length)
                {
                    // This dialogue line only has events/flags, no speaker or text
                    dialogueLine.speaker = "";
                    dialogueLine.text = "";
                    convo.lines.Add(dialogueLine);
                    continue;
                }

                string firstLine = dlgLines[lineIndex];

                // Remaining events and flags in the first line
                MatchCollection remainingEvents = Regex.Matches(firstLine, @"<([^>]+)>");
                foreach (Match match in remainingEvents)
                    dialogueLine.events.Add(match.Groups[1].Value);

                MatchCollection remainingFlags = Regex.Matches(firstLine, @">>!F\s+(\S+)");
                foreach (Match match in remainingFlags)
                    dialogueLine.flagsToSet.Add(match.Groups[1].Value.Trim());

                // Extract speaker (remove events and flags first)
                firstLine = Regex.Replace(firstLine, @"<([^>]+)>", "").Trim();
                firstLine = Regex.Replace(firstLine, @">>!F\s+\S+", "").Trim();
                Match speakerMatch = Regex.Match(firstLine, @"\[(.*?)\]");
                dialogueLine.speaker = speakerMatch.Success ? speakerMatch.Groups[1].Value.Trim() : "";

                // Extract text
                if (dlgLines.Length > lineIndex + 1)
                {
                    dialogueLine.text = string.Join("\n", dlgLines, lineIndex + 1, dlgLines.Length - lineIndex - 1).Trim();
                }
                else
                {
                    // Remove speaker tag and any remaining content
                    string remainingText = firstLine.Replace("[" + dialogueLine.speaker + "]", "").Trim();
                    dialogueLine.text = remainingText;
                }

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

            // Execute events for this line
            foreach (string eventName in line.events)
            {
                ExecuteEvent(eventName);   
            }

            // Set flags for this line
            if (UniversalFlags.Instance != null)
            {
                foreach (string flagName in line.flagsToSet)
                {
                    UniversalFlags.Instance.SetFlag(flagName, true);
                    Debug.Log($"Set flag '{flagName}' to true");
                }
            }

            dialogueIndex++; // Increment index early to handle recursion correctly

            // If the line has no text, skip showing the dialogue box
            if (string.IsNullOrEmpty(line.text))
            {
                // Immediately show the next line
                ShowNextLine();
                return;
            }

            // Show dialogue normally
            characterNameText.text = line.speaker;
            dialogueBox.SetActive(true);

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(line.text));
        }

        private void ShowChoices()
        {
            waitingForChoice = true;
            // Keep previous dialogue visible
            ThirdPersonCam.Instance.CursorUnlock += "Dialogue";

            foreach (DialogueChoice choice in currentConversation.choices)
            {
                // Check if choice requires a flag
                if (!string.IsNullOrEmpty(choice.requiredFlag))
                {
                    // Only show this choice if the flag is true
                    if (UniversalFlags.Instance == null || !UniversalFlags.Instance.GetFlag(choice.requiredFlag))
                    {
                        continue; // Skip this choice
                    }
                }

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

            ThirdPersonCam.Instance.CursorUnlock -= "Dialogue";
        }

        private void ExecuteEvent(string eventName)
        {
            Debug.Log("executing " + eventName);
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
}