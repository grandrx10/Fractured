using UnityEngine;
using Game;
using Characters.Dialogue;

public class SetPrefabToEvent : DialogueEvent
{
    [Header("Target Object")]
    [Tooltip("The object whose prefab state will be set")]
    public GameObject targetObject;

    [Header("Prefab Name")]
    [Tooltip("The name of the prefab this object should become")]
    public string prefabName;

    public override void Execute()
    {
        if (targetObject == null)
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

        string key = $"PREFAB_{targetObject.name}";
        GlobalState.instance.SetStr(key, prefabName);

        Debug.Log($"SetPrefabToEvent: Set {targetObject.name} to prefab '{prefabName}'");
    }
}
