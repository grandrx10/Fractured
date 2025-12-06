using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject dialogueBox;
    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    [Header("Settings")]
    public float lettersPerSecond = 30f;

    [Header("Characters")]
    public List<Character> characters = new List<Character>();

    private Conversation currentConversation;
    private int index = 0;
    private bool isRunning = false;
    private Coroutine typingCoroutine;

    private AudioSource currentAudioSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (dialogueBox != null)
            dialogueBox.SetActive(false);
    }

    private void Update()
    {
        if (!isRunning)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // If text is still typing, finish instantly
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentConversation.dialogues[index - 1].line;
                typingCoroutine = null;

                // Stop any currently playing audio immediately
                if (currentAudioSource != null && currentAudioSource.isPlaying)
                    currentAudioSource.Stop();
            }
            else
            {
                PlayNextDialogue();
            }
        }
    }

    public void StartConversation(Conversation convo)
    {
        if (isRunning)
            return;

        currentConversation = convo;
        index = 0;
        isRunning = true;

        if (dialogueBox != null)
            dialogueBox.SetActive(true);

        PlayNextDialogue();
    }

    private void PlayNextDialogue()
    {
        if (index >= currentConversation.dialogues.Length)
        {
            EndConversation();
            return;
        }

        Dialogue dialogue = currentConversation.dialogues[index];
        index++;

        // Execute dialogue events
        foreach (DialogueEvent e in dialogue.events)
            e?.Execute();

        // Update speaker name
        if (speakerText != null)
            speakerText.text = dialogue.speakerName;

        // Stop previous audio
        if (currentAudioSource != null && currentAudioSource.isPlaying)
            currentAudioSource.Stop();

        // Play voice line
        if (dialogue.voiceLine != null)
        {
            Character speakerCharacter = characters.Find(c => c.characterName == dialogue.speakerName);
            if (speakerCharacter != null)
            {
                AudioSource source = speakerCharacter.characterTransform.GetComponent<AudioSource>();
                if (source == null)
                    source = speakerCharacter.characterTransform.gameObject.AddComponent<AudioSource>();

                source.clip = dialogue.voiceLine;
                source.Play();
                currentAudioSource = source;
            }
        }

        // Start typing the dialogue
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(dialogue.line));
    }

    private IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        typingCoroutine = null;
    }

    private void EndConversation()
    {
        isRunning = false;
        currentConversation = null;

        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        // Stop any remaining audio
        if (currentAudioSource != null && currentAudioSource.isPlaying)
            currentAudioSource.Stop();

        currentAudioSource = null;

        Debug.Log("Conversation finished.");
    }

    /// <summary>
    /// Helper method to get a character by name
    /// </summary>
    public Character GetCharacterByName(string name)
    {
        return characters.Find(c => c.characterName == name);
    }
}
