using UnityEngine;

namespace Characters
{
    public class PlayerSingleton : MonoBehaviour
    {
        public static PlayerSingleton Instance { get; private set; }
        public LayerMask groundLayer;      // Set this to your ground layer(s)
        public float maxRayDistance = 10f; // How far down to check for ground
        public GameObject playerObj;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Returns the world position directly under the player's feet by raycasting down.
        /// If no ground is found within maxRayDistance, returns the player's current position.
        /// </summary>
        public Vector3 GetPositionBelow()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, groundLayer))
            {
                return hit.point;
            }

            // Fallback: return current position if no ground detected
            return transform.position;
        }
    }
}
