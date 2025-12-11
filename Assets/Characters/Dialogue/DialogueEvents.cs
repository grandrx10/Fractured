using UnityEngine;

namespace Characters.Dialogue
{
    public class DialogueEvent : MonoBehaviour
    {
        public virtual void Execute()
        {
        
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
