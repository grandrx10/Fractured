using UnityEngine;
using Game;
using Characters.Dialogue;

public class SetPrefabToEvent : DialogueEvent
{
    [Header("Target Object")]
    [Tooltip("The object whose prefab state will be set")]
    public string targetObjectName;

    [Header("Prefab Name")]
    [Tooltip("The name of the prefab this object should become")]
    public string prefabName;

    public override void Execute()
    {
        if (targetObjectName == null)
        {
            Debug.LogWarning("SetPrefabToEvent: No target object assigned.");
            return;
        }

        if (GlobalState.instance == null)
        {
            Debug.LogWarning("SetPrefabToEvent: No GlobalState instance found.");
            return;
        }

        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogWarning("SetPrefabToEvent: Prefab name is empty.");
            return;
        }

        string key = $"PREFAB_{targetObjectName}";
        GlobalState.instance.SetStr(key, prefabName);

        Debug.Log($"SetPrefabToEvent: Set {targetObjectName} to prefab '{prefabName}'");
    }
}
