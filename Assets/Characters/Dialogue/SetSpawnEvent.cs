using UnityEngine;
using Game;
using Characters.Dialogue;

public class SetSpawnEvent : DialogueEvent
{
    [Header("Target Spawn Object")]
    [Tooltip("The SpawnAt object whose spawn location will be set")]
    public SpawnAt targetSpawnAt;

    [Header("Spawn Point Name")]
    [Tooltip("The name of the spawn point to assign")]
    private string spawnPointName;

    void Start()
    {
        spawnPointName = gameObject.name;
    }

    public override void Execute()
    {
        if (targetSpawnAt == null)
        {
            Debug.LogWarning("SetSpawnEvent: No target SpawnAt assigned.");
            return;
        }

        if (GlobalState.instance == null)
        {
            Debug.LogWarning("SetSpawnEvent: No GlobalState instance found.");
            return;
        }

        // Build the key using the prefix + target SpawnAt's GameObject name
        string spawnKey = "spawn_" + targetSpawnAt.gameObject.name;

        // Set the spawn location in GlobalState
        GlobalState.instance.SetStr(spawnKey, spawnPointName);
        Debug.Log($"SetSpawnEvent: Set spawn of {targetSpawnAt.gameObject.name} to '{spawnPointName}'");
    }
}
