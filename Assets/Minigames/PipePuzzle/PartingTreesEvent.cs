using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Dialogue
{
    public class TiltObjectsEvent : DialogueEvent
    {
        [Header("Objects to Tilt")]
        [Tooltip("List of transforms to tilt 90° backwards")]
        public List<Transform> objectsToTilt = new();

        [Header("Barrier")]
        [Tooltip("GameObject to delete when event executes")]
        public GameObject barrierToDelete;

        [Header("Tilt Settings")]
        [Tooltip("Degrees to tilt (default 90)")]
        public float tiltAngle = 90f;

        [Tooltip("Speed of the tilt in degrees per second")]
        public float tiltSpeed = 90f;

        public override void Execute()
        {
            if (objectsToTilt.Count > 0)
            {
                foreach (var t in objectsToTilt)
                {
                    if (t != null)
                        StartCoroutine(TiltCoroutine(t));
                }
            }

            if (barrierToDelete != null)
            {
                Destroy(barrierToDelete);
            }
        }

        private IEnumerator TiltCoroutine(Transform t)
        {
            Quaternion startRot = t.rotation;
            Quaternion endRot = startRot * Quaternion.Euler(-tiltAngle, 0f, 0f); // Tilt backwards

            float elapsed = 0f;
            float duration = tiltAngle / tiltSpeed; // Time = angle / speed

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float tLerp = Mathf.Clamp01(elapsed / duration);
                t.rotation = Quaternion.Slerp(startRot, endRot, tLerp);
                yield return null;
            }

            t.rotation = endRot; // Ensure exact final rotation
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw a cyan sphere at the event's position
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.25f);

            // Draw the event's name above the sphere
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, gameObject.name);

            // Draw magenta lines to the target objects
            if (objectsToTilt != null)
            {
                Gizmos.color = Color.magenta;
                foreach (var t in objectsToTilt)
                {
                    if (t != null)
                    {
                        Gizmos.DrawLine(transform.position, t.position);
                    }
                }
            }
        }
#endif
    }
}
