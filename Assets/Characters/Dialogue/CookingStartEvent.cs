using UnityEngine;
using Characters.Dialogue;

public class CookingStartEvent : DialogueEvent
{
    [Header("Cooking")]
    [Tooltip("Pot station to start cooking on")]
    public PotStation pot;

    public override void Execute()
    {
        if (pot == null)
        {
            Debug.LogWarning("CookingStartEvent: No PotStation assigned.");
            return;
        }

        pot.StartPot();
        Debug.Log($"Cooking started on pot: {pot.name}");
    }
}
