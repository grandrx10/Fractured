using UnityEngine;

namespace Characters.Dialogue
{
    public class DialogueEvent : MonoBehaviour
    {
        [Header("Audio")]
        public AudioClip eventSound;   // optional audio clip
        [Range(0f, 1f)]
        public float eventVolume = 1f; // optional volume control

        public virtual void Execute()
        {
            // Play sound if assigned
            if (eventSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayOneShot(
                    eventSound,
                    transform.position, // play at this object's position
                    eventVolume
                );
            }
        }

        private void OnDrawGizmos()
        {
            // Set the color of the gizmo
            Gizmos.color = Color.cyan;

            // Draw a small sphere at the object's position
            Gizmos.DrawSphere(transform.position, 0.25f);

#if UNITY_EDITOR
            // Optional: Draw the GameObject name above the sphere
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, gameObject.name);
#endif
        }
    }
}
