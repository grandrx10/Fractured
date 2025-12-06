using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string speakerName;

    [TextArea]
    public string line;
    public AudioClip voiceLine;
    public DialogueEvent[] events;
}
