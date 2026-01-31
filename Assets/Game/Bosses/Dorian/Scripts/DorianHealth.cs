using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Health;

namespace Game.Bosses.Dorian
{
    public class DorianHealth : BossHealth
    {
        [Header("Invincible Dash Settings")]
        public float invincibleDashRadius = 8f;
        public float invincibleDashDuration = 0.15f;

        [Header("Dash Line Settings")]
        public GameObject dashLinePrefab;
        public float dashLineLifetime = 0.3f;

        [Header("Shields")]
        public List<Shield> shields = new List<Shield>();

        private bool isInvincibleDashing = false;
        private Coroutine dashRoutine;
        private LineRenderer dashLine;

        private void Update()
        {
            // Update invincibility state based on shields
            isInvincible = AreAnyShieldsActive();
        }

        private bool AreAnyShieldsActive()
        {
            foreach (var shield in shields)
            {
                if (shield != null && shield.isActive)
                    return true;
            }
            return false;
        }

        protected override void InvincibleReaction(GameObject source)
        {
            // Only trigger dash if currently invincible
            if (!isInvincible)
                return;

            if (isInvincibleDashing)
                return;

            dashRoutine = StartCoroutine(InvincibleDash());
        }

        private IEnumerator InvincibleDash()
        {
            isInvincibleDashing = true;

            Vector3 startPos = transform.position;

            // Pick random dash target within radius (XZ plane)
            Vector2 offset2D = Random.insideUnitCircle * invincibleDashRadius;
            Vector3 targetPos = startPos + new Vector3(offset2D.x, 0f, offset2D.y);

            // Create dash line
            if (dashLinePrefab != null)
            {
                GameObject lineObj = Instantiate(dashLinePrefab);
                dashLine = lineObj.GetComponent<LineRenderer>();

                if (dashLine != null)
                {
                    dashLine.useWorldSpace = true;
                    dashLine.positionCount = 2;
                    dashLine.SetPosition(0, startPos);
                    dashLine.SetPosition(1, startPos);
                }
            }

            float elapsed = 0f;

            while (elapsed < invincibleDashDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / invincibleDashDuration);

                Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
                transform.position = currentPos;

                // Update line end point
                if (dashLine != null)
                    dashLine.SetPosition(1, currentPos);

                yield return null;
            }

            transform.position = targetPos;

            if (dashLine != null)
            {
                dashLine.SetPosition(1, targetPos);
                Destroy(dashLine.gameObject, dashLineLifetime);
            }

            isInvincibleDashing = false;
        }
    }
}
