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

        public Vector3 GetPositionBelow()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, groundLayer))
                return hit.point;

            return transform.position;
        }

        /// <summary>
        /// Gets the PlayerInteractController component and returns the camera raycast target
        /// </summary>
        public Vector3 GetCameraForwardPositionFromController()
        {
            PlayerInteractController controller = GetComponent<PlayerInteractController>();
            if (controller != null)
            {
                return controller.GetCameraRaycastTarget();
            }

            // Fallback: just forward from player if no controller exists
            return transform.position + transform.forward * 10f;
        }
    }
}
