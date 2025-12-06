using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conversation")]
public class Conversation : ScriptableObject
{
    public bool inChat = true;
    public Dialogue[] dialogues;
}
