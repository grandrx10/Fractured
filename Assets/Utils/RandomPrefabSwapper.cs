using System;
using System.Collections.Generic;
using UnityEngine;

namespace World.Objects
{
    [RequireComponent(typeof(PersistentID))]
    public class RandomPrefabSwapper : MonoBehaviour
    {
        public List<GameObject> prefabs;
        public List<Transform> spawnPoints;

        private void Start()
        {
            var pid = GetComponent<PersistentID>();
            int baseSeed = StableHash(pid.ID);

            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (prefabs.Count == 0 || spawnPoints[i] == null)
                    continue;

                // Different deterministic stream per spawn point
                var rng = new System.Random(baseSeed + i);

                int prefabIndex = rng.Next(prefabs.Count);
                Instantiate(
                    prefabs[prefabIndex],
                    spawnPoints[i].position,
                    spawnPoints[i].rotation,
                    spawnPoints[i]
                );
            }
        }

        private static int StableHash(string s)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in s)
                    hash = hash * 31 + c;
                return hash;
            }
        }
    }
}