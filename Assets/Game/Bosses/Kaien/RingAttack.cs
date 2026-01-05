using UnityEngine;

namespace Game.Bosses
{
    [CreateAssetMenu(menuName = "BossAttacks/Kaien/RingAttack")]
    public class RingAttack : BossAttack
    {
        [Header("Movement")]
        public string movePointName;      // NamedPoint on Boss
        public float moveDuration = 1.5f;

        [Header("Ring")]
        public string ringPointName;      // NamedPoint holding PlatformRingFilter
        public bool applyOnArrival = true;

        private Transform bossTransform;
        private Transform targetTransform;
        private PlatformRingFilter ringFilter;

        private Vector3 startPos;
        private float moveTimer;
        private bool ringApplied;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            Boss bossScript = boss.GetComponent<Boss>();
            bossTransform = boss.transform;

            targetTransform = bossScript.GetPointTransform(movePointName);

            Transform ringTransform = bossScript.GetPointTransform(ringPointName);
            if (ringTransform != null)
                ringFilter = ringTransform.GetComponent<PlatformRingFilter>();

            startPos = bossTransform.position;
            moveTimer = 0f;
            ringApplied = false;

            if (targetTransform == null)
                Debug.LogError($"RingAttack: Move point '{movePointName}' not found.");

            if (ringFilter == null)
                Debug.LogError($"RingAttack: PlatformRingFilter not found at '{ringPointName}'.");
        }

        public override void Tick(GameObject boss)
        {
            if (!isActive || targetTransform == null)
                return;

            moveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(moveTimer / moveDuration);

            bossTransform.position = Vector3.Lerp(
                startPos,
                targetTransform.position,
                t
            );

            // Apply ring mask once arrival is reached
            if (applyOnArrival && !ringApplied && t >= 1f)
            {
                ApplyRing();
            }
        }

        private void ApplyRing()
        {
            if (ringFilter == null)
                return;

            ringApplied = true;
            ringFilter.ApplyRingMask();
            Debug.Log("APPLYING RING");
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);
        }
    }
}
