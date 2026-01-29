using System;
using System.Collections.Generic;
using UnityEngine;

namespace World.Objects
{
    [RequireComponent(typeof(PersistentID))]
    public class RandomMaterialwapper : MonoBehaviour
    {
        public List<Material> materials;
        public List<Renderer> spawnPoints;

        private void Start()
        {
            var pid = GetComponent<PersistentID>();
            int baseSeed = StableHash(pid.ID);

            for (int i = 0; i < spawnPoints.Count; i++)
            {
                if (materials.Count == 0 || spawnPoints[i] == null)
                    continue;

                // Different deterministic stream per spawn point
                var rng = new System.Random(baseSeed + i);

                int prefabIndex = rng.Next(materials.Count);
                spawnPoints[i].material = materials[prefabIndex];
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