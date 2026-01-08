using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class SpawnAt : MonoBehaviour
    {
        [Header("Spawn Points")]
        [Tooltip("List of possible spawn points")]
        public List<Transform> spawnPoints = new List<Transform>();

        [Header("GlobalState Key Prefix")]
        [Tooltip("Prefix for the GlobalState key; the final key will be prefix + GameObject name")]
        public string spawnKeyPrefix = "spawn_";

        private void Start()
        {
            if (GlobalState.instance == null)
            {
                Debug.LogWarning("SpawnAt: No GlobalState instance found.");
                return;
            }

            string fullSpawnKey = spawnKeyPrefix + gameObject.name;

            if (GlobalState.instance.TryGetStr(fullSpawnKey, out string targetSpawnName))
            {
                Debug.Log($"SpawnAt: Available spawn points ({spawnPoints.Count}):");
                for (int i = 0; i < spawnPoints.Count; i++)
                {
                    Transform t = spawnPoints[i];
                    if (t == null)
                    {
                        Debug.Log($"  [{i}] <null>");
                    }
                    else
                    {
                        Debug.Log($"  [{i}] '{t.name}'");
                    }
                }
                string normalizedTarget = targetSpawnName.Trim();
                Transform target = spawnPoints.Find(t =>
                    t != null && t.name.Trim() == normalizedTarget
                );

                if (target != null)
                {
                    transform.position = target.position;
                    transform.rotation = target.rotation;
                    Debug.Log($"SpawnAt: Spawned {gameObject.name} at {targetSpawnName}");
                }
                else
                {
                    Debug.LogWarning($"SpawnAt: No spawn point named '{targetSpawnName}' found in the list.");
                }
            }
            else
            {
                Debug.Log($"SpawnAt: GlobalState key '{fullSpawnKey}' not set, {gameObject.name} will not move.");
            }
        }

    }
}
