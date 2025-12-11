using System.Collections;
using UnityEngine;

namespace Game.Bosses
{
    public class NpcCommands : MonoBehaviour
    {
        [SerializeField] private float rotationDuration = 0.5f;
        private Transform lookingAt;
        private Coroutine currentRotation;

        /// <summary>
        /// Continuously look at this target while set
        /// </summary>
        public Transform LookingAt
        {
            get => lookingAt;
            set
            {
                if (lookingAt != value)
                {
                    lookingAt = value;
                    if (lookingAt != null)
                    {
                        StartContinuousRotation();
                    }
                    else
                    {
                        StopContinuousRotation();
                    }
                }
            }
        }

        public void SetLookingAt(Transform target)
        {
            LookingAt = target;
        }

        private void StartContinuousRotation()
        {
            if (currentRotation != null)
                StopCoroutine(currentRotation);

            currentRotation = StartCoroutine(RotateContinuously());
        }

        private void StopContinuousRotation()
        {
            if (currentRotation != null)
            {
                StopCoroutine(currentRotation);
                currentRotation = null;
            }
        }

        private IEnumerator RotateContinuously()
        {
            while (lookingAt != null)
            {
                Vector3 lookDir = lookingAt.position - transform.position;
                lookDir.y = 0; // XZ plane only
                if (lookDir.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime / rotationDuration);
                }
                yield return null;
            }
        }

        /// <summary>
        /// Rotate once toward a target over rotationDuration
        /// </summary>
        public void RotateOnceTowards(Transform target)
        {
            if (target == null) return;

            StopContinuousRotation(); // stop any continuous rotation
            StartCoroutine(RotateOnceCoroutine(target));
        }

        private IEnumerator RotateOnceCoroutine(Transform target)
        {
            Quaternion startRot = transform.rotation;
            Vector3 lookDir = target.position - transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude < 0.001f) yield break;

            Quaternion targetRot = Quaternion.LookRotation(lookDir);

            float elapsed = 0f;
            while (elapsed < rotationDuration)
            {
                transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.rotation = targetRot;
        }
    }
}
