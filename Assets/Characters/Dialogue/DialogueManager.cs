using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Characters.Interactables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cards.Environments;
using Game;

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
        public string eventName;        // For <EventName> format
        public string conversationName; // For {ConvoName} format
        public string requiredFlag;     // For (FlagName) format - flag must be true to show
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
        public System.Action OnConversationEnded;

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
        public bool inConversation = false;
        private int dialogueSessionDepth = 0;
        public bool IsDialogueSessionActive => dialogueSessionDepth > 0;
        private bool waitingForChoice = false;

        private List<Speaker> disabledSpeakers = new List<Speaker>();
        private Dictionary<string, DialogueEvent> eventCache = new Dictionary<string, DialogueEvent>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadAllDialogueFiles();
                CacheDialogueEvents();
            }
            else if (Instance != this)
            {
                // Merge character mappings
                foreach (var mapping in characterMappings)
                {
                    if (!Instance.characterMappings.Exists(c => c.characterName == mapping.characterName))
                        Instance.characterMappings.Add(mapping);
                }

                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            GlobalWorldManager.OnLoadNewScene += OnNewSceneLoaded;
        }

        private void OnDisable()
        {
            GlobalWorldManager.OnLoadNewScene -= OnNewSceneLoaded;
        }

        private void OnNewSceneLoaded(CardEnv env)
        {
            Scene newScene = GlobalWorldManager.Instance._newScene;
            if (!newScene.IsValid())
            {
                Debug.LogWarning("DialogueManager: GlobalWorldManager _newScene is invalid!");
                return;
            }

            // ----------------------------
            // 1. Rebuild character mappings from the new scene
            // ----------------------------
            RebuildCharacterMappingsFromScene(newScene);

            // ----------------------------
            // 2. Find DialogueEvents object
            // ----------------------------
            GameObject newEventHandler = null;
            foreach (GameObject root in newScene.GetRootGameObjects())
            {
                if (root.name == "DialogueEvents")
                {
                    newEventHandler = root;
                    break;
                }
            }

            if (newEventHandler != null)
            {
                eventHandlerObject = newEventHandler;
                CacheDialogueEvents();
                Debug.Log("DialogueManager: Found new DialogueEvents object in scene " + newScene.name);
            }
            else
            {
                eventCache.Clear();
                Debug.LogWarning("DialogueManager: No DialogueEvents object found in scene " + newScene.name);
            }
        }

        private void RebuildCharacterMappingsFromScene(Scene scene)
        {
            characterMappings.Clear();
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Speaker sp in root.GetComponentsInChildren<Speaker>(true))
                {
                    characterMappings.Add(new CharacterMapping
                    {
                        characterName = sp.name,
                        characterObject = sp.gameObject
                    });
                }
            }
            Debug.Log($"DialogueManager: Rebuilt {characterMappings.Count} character mappings from scene {scene.name}");
        }

        private void CacheDialogueEvents()
        {
            if (eventHandlerObject == null)
            {
                Debug.LogWarning("No event handler object assigned to DialogueManager");
                return;
            }

            DialogueEvent[] events = eventHandlerObject.GetComponentsInChildren<DialogueEvent>();
            eventCache.Clear();
            foreach (DialogueEvent evt in events)
                eventCache[evt.gameObject.name] = evt;
        }

        private void LoadAllDialogueFiles()
        {
            foreach (string path in dialogueFilePaths)
                LoadDialogueFileFromResources(path);
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
            string[] convoBlocks = text.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string block in convoBlocks)
                ParseConversationBlock(block.Trim());
        }

        private void ParseConversationBlock(string block)
        {
            string[] sections = block.Split(new string[] { "===" }, StringSplitOptions.None);
            if (sections.Length < 2) return;

            string convoName = sections[0].Trim();
            if (string.IsNullOrEmpty(convoName)) return;

            string dialogueSection = sections[1].Trim();
            string postDialogueSection = sections.Length > 2 ? string.Join("\n", sections, 2, sections.Length - 2).Trim() : "";

            Conversation convo = new Conversation { name = convoName };

            // Parse choices
            string[] dialogueSplit = dialogueSection.Split(new string[] { "???" }, StringSplitOptions.None);
            string actualDialogue = dialogueSplit[0].Trim();
            string choicesSection = dialogueSplit.Length > 1 ? dialogueSplit[1].Trim() : "";

            if (!string.IsNullOrEmpty(choicesSection))
            {
                string[] choiceLines = choicesSection.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string choiceLine in choiceLines)
                {
                    string trimmed = choiceLine.Trim();
                    if (string.IsNullOrEmpty(trimmed)) continue;

                    DialogueChoice choice = new DialogueChoice();
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

            // Parse post-dialogue instructions
            if (!string.IsNullOrEmpty(postDialogueSection))
            {
                string[] postLines = postDialogueSection.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in postLines)
                {
                    string trimmedLine = line.Trim();
                    if (string.IsNullOrEmpty(trimmedLine)) continue;

                    Match swapMatch = Regex.Match(trimmedLine, @"^\[(.*?)\]\{(.*?)\}$");
                    if (swapMatch.Success)
                    {
                        convo.swapInstructions[swapMatch.Groups[1].Value.Trim()] = swapMatch.Groups[2].Value.Trim();
                        continue;
                    }

                    Match nextConvoMatch = Regex.Match(trimmedLine, @"^>>\s*(.+)$");
                    if (nextConvoMatch.Success)
                        convo.nextConversation = nextConvoMatch.Groups[1].Value.Trim();
                }
            }

            // Parse dialogue lines
            string[] dialogueBlocks = actualDialogue.Split(new string[] { "---" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string dlgBlock in dialogueBlocks)
            {
                string[] dlgLines = dlgBlock.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (dlgLines.Length == 0) continue;

                DialogueLine dialogueLine = new DialogueLine();
                int lineIndex = 0;

                while (lineIndex < dlgLines.Length)
                {
                    string line = dlgLines[lineIndex].Trim();
                    MatchCollection flagMatches = Regex.Matches(line, @">>!F\s+(\S+)");
                    string cleanLine = Regex.Replace(line, @"<([^>]+)>", "").Trim();
                    cleanLine = Regex.Replace(cleanLine, @">>!F\s+\S+", "").Trim();

                    if (string.IsNullOrEmpty(cleanLine))
                    {
                        MatchCollection eventMatches = Regex.Matches(line, @"<([^>]+)>");
                        foreach (Match match in eventMatches)
                            dialogueLine.events.Add(match.Groups[1].Value);

                        foreach (Match match in flagMatches)
                            dialogueLine.flagsToSet.Add(match.Groups[1].Value.Trim());

                        lineIndex++;
                    }
                    else break;
                }

                if (lineIndex >= dlgLines.Length)
                {
                    dialogueLine.speaker = "";
                    dialogueLine.text = "";
                    convo.lines.Add(dialogueLine);
                    continue;
                }

                string firstLine = dlgLines[lineIndex];
                MatchCollection remainingEvents = Regex.Matches(firstLine, @"<([^>]+)>");
                foreach (Match match in remainingEvents)
                    dialogueLine.events.Add(match.Groups[1].Value);

                MatchCollection remainingFlags = Regex.Matches(firstLine, @">>!F\s+(\S+)");
                foreach (Match match in remainingFlags)
                    dialogueLine.flagsToSet.Add(match.Groups[1].Value.Trim());

                firstLine = Regex.Replace(firstLine, @"<([^>]+)>", "").Trim();
                firstLine = Regex.Replace(firstLine, @">>!F\s+\S+", "").Trim();

                Match speakerMatch = Regex.Match(firstLine, @"\[(.*?)\]");
                dialogueLine.speaker = speakerMatch.Success ? speakerMatch.Groups[1].Value.Trim() : "";

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
            SetCombatInvincible(true);
            if (!inConversation)
                dialogueSessionDepth++;

            currentConversation = conversations.Find(c => c.name == conversationName);
            if (currentConversation == null)
            {
                Debug.LogError("Conversation not found: " + conversationName);
                dialogueSessionDepth--;
                return;
            }

            PlayerInteractController.PlayerInputs.AddBlocker("Conversation", InputBlockPrio.Dialogue);

            inConversation = true;
            waitingForChoice = false;
            disabledSpeakers.Clear();

            dialogueIndex = 0;
            dialogueBox.SetActive(true);
            ShowNextLine();
        }


        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && inConversation && !waitingForChoice &&
                PlayerInteractController.PlayerInputs.IsInputAllowed(InputBlockPrio.Dialogue))
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
                    ShowChoices();
                else
                    EndConversation();
                return;
            }

            DialogueLine line = currentConversation.lines[dialogueIndex];

            foreach (string eventName in line.events)
                ExecuteEvent(eventName);

            if (GlobalState.instance != null)
            {
                foreach (string flagName in line.flagsToSet)
                {
                    GlobalState.instance.AddEvent(flagName);
                    Debug.Log($"GlobalState: Added event '{flagName}'");
                }
            }

            dialogueIndex++;
            if (string.IsNullOrEmpty(line.text))
            {
                ShowNextLine();
                return;
            }

            characterNameText.text = line.speaker;
            dialogueBox.SetActive(true);

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(line.text));
        }

        private void ShowChoices()
        {
            waitingForChoice = true;
            PlayerCamera.Instance.CursorUnlock += "Dialogue";

            foreach (DialogueChoice choice in currentConversation.choices)
            {
                if (!string.IsNullOrEmpty(choice.requiredFlag) && GlobalState.instance != null)
                {
                    bool showChoice = true;
                    string flagName = choice.requiredFlag;

                    if (flagName.StartsWith("!"))
                    {
                        // Show only if the flag is NOT set
                        flagName = flagName.Substring(1);
                        showChoice = !GlobalState.instance.HasEvent(flagName);
                    }
                    else
                    {
                        // Show only if the flag IS set
                        showChoice = GlobalState.instance.HasEvent(flagName);
                    }

                    if (!showChoice)
                        continue;
                }

                GameObject choiceObj = Instantiate(choicePrefab, choiceParent);
                TMP_Text choiceText = choiceObj.GetComponentInChildren<TMP_Text>();
                if (choiceText != null) choiceText.text = choice.choiceText;

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

            // Execute event immediately if present
            if (!string.IsNullOrEmpty(choice.eventName))
                ExecuteEvent(choice.eventName);

            if (!string.IsNullOrEmpty(choice.conversationName))
                currentConversation.nextConversation = choice.conversationName;

            PlayerCamera.Instance.CursorUnlock -= "Dialogue";

            // EndConversation ONCE
            EndConversation();
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
        private void SetCombatInvincible(bool value)
        {
            if (OpenWorldEnv.Current is RTCombatEnv combatEnv)
            {
                combatEnv.invincible = value;
            }
        }


        private void EndConversation()
        {
            dialogueBox.SetActive(false);
            SetCombatInvincible(false);

            // Apply swaps BEFORE clearing currentConversation
            if (currentConversation != null && currentConversation.swapInstructions.Count > 0)
            {
                foreach (var swap in currentConversation.swapInstructions)
                {
                    CharacterMapping mapping = characterMappings.Find(c => c.characterName == swap.Key);
                    Debug.Log("SWAP DETECTED: " + swap.Key + " -> " + swap.Value);
                    if (mapping != null)
                    {
                        Speaker sp = mapping.characterObject.GetComponent<Speaker>();
                        if (sp != null)
                        {
                            sp.conversationName = swap.Value;
                            sp.canInteract = true; // re-enable interaction after conversation
                        }
                    }
                }
            }

            // Re-enable any previously disabled speakers
            foreach (Speaker sp in disabledSpeakers)
                if (sp != null) sp.canInteract = true;

            disabledSpeakers.Clear();
            inConversation = false;
            PlayerInteractController.PlayerInputs.RemoveBlocker("Conversation");

            string nextConvo = currentConversation?.nextConversation;
            currentConversation = null; // null AFTER swaps

            dialogueIndex = 0;
            waitingForChoice = false;

            foreach (Transform child in choiceParent)
                Destroy(child.gameObject);

            if (!string.IsNullOrEmpty(nextConvo))
            {
                StartConversation(nextConvo);
            }
            else
            {
                dialogueSessionDepth--;
                OnConversationEnded?.Invoke();
            }

        }

    }
}
