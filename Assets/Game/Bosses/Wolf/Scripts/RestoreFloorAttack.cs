using System.Collections;
using Game.Bosses;
using UnityEngine;

namespace Game.Bosses.Wolf
{
    [CreateAssetMenu(menuName = "BossAttacks/Wolf/RestoreFloorAttack")]
    public class RestoreFloorAttack : BossAttack
    {
        [Header("Floor Settings")]
        public string floorPointName;      // Named point on the boss for the floor
        public GameObject floorPrefab;     // Floor prefab to spawn
        public float riseHeight = 5f;      // How far below the original floor to spawn initially
        public float riseDuration = 1f;    // Time it takes for the floor to rise

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            Transform oldFloorTransform = boss.GetComponent<Boss>().GetPointTransform(floorPointName);
            if (oldFloorTransform == null)
            {
                Debug.LogError("RestoreFloorAttack: Floor transform not found for point " + floorPointName);
                return;
            }

            // Spawn new floor underneath the old floor (world space, no parent)
            Vector3 spawnPos = oldFloorTransform.position - Vector3.up * riseHeight;
            GameObject newFloor = Instantiate(floorPrefab, spawnPos, oldFloorTransform.rotation);

            // Start rising coroutine
            boss.GetComponent<MonoBehaviour>().StartCoroutine(RiseFloor(newFloor, oldFloorTransform, boss));
        }

        private IEnumerator RiseFloor(GameObject newFloor, Transform oldFloorTransform, GameObject boss)
        {
            Vector3 startPos = newFloor.transform.position;
            Vector3 endPos = oldFloorTransform.position;
            float elapsed = 0f;

            while (elapsed < riseDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / riseDuration);
                newFloor.transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            // Ensure final position
            newFloor.transform.position = endPos;

            // Delete old floor
            Destroy(oldFloorTransform.gameObject);

            // Update the boss's named point to the new floor
            Boss bossComponent = boss.GetComponent<Boss>();
            if (bossComponent != null)
            {
                bossComponent.SetPointTransform(floorPointName, newFloor.transform);
            }
        }
    }
}
