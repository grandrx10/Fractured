using System.Collections;
using Characters;
using Game.Bosses;
using UnityEngine;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Wolf
{
    [CreateAssetMenu(menuName = "BossAttacks/Wolf/FloorCutawayAttack")]
    public class FloorCutawayAttack : BossAttack
    {
        [Header("Floor Settings")]
        public string floorTransformName;         // Transform on the boss that has the FloorCutaway
        public GameObject warningPrefab;          // Large visual warning prefab
        public float warningHeightOffset = 0.5f;  // How far above the floor to display the warning
        public float warningDuration = 1f;        // How long the warning lasts
        public float warningRadius = 5f;          // Radius of the warning effect

        private Transform floorTransform;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            // Get the floor transform from the boss
            floorTransform = boss.GetComponent<Boss>().GetPointTransform(floorTransformName);
            if (floorTransform == null)
            {
                Debug.LogError("FloorCutawayAttack: Floor transform not found: " + floorTransformName);
                return;
            }

            // Start the attack routine
            boss.GetComponent<MonoBehaviour>().StartCoroutine(FloorCutawayRoutine());
        }

        private IEnumerator FloorCutawayRoutine()
        {
            // Spawn warning above the floor
            if (warningPrefab != null && floorTransform != null)
            {
                Vector3 warningPos = floorTransform.position + Vector3.up * warningHeightOffset;
                GameObject warningObj = Instantiate(warningPrefab, warningPos, Quaternion.identity);

                // Initialize the warning using the Warning component like in your example
                Warning w = warningObj.GetComponent<Warning>();
                if (w != null)
                {
                    w.Initialize(warningRadius, 0f, Warning.WarningType.Grounded, warningDuration);
                }

                // Wait for the warning duration
                yield return new WaitForSeconds(warningDuration);

                // Destroy the warning object
                if (warningObj != null)
                    Destroy(warningObj);
            }

            // Trigger the floor cutaway
            FloorCutaway cutaway = floorTransform.GetComponent<FloorCutaway>();
            if (cutaway != null)
            {
                cutaway.TriggerCutaway();
            }
        }
    }
}
