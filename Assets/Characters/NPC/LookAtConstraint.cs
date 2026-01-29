using Unity.Cinemachine;
using UnityEngine;

namespace Characters
{
    public class CinemachineLookAtAdjuster : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private Transform player;
        [SerializeField] private Transform originalLookAtPoint; // Your shoulder offset point

        [Header("Settings")]
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private float pullSpeed = 10f; // How fast to adjust the look point
        [SerializeField] private float wallOffset = 0.1f; // Small distance to stay off walls

        private Transform dynamicLookAtPoint;
        private Camera mainCam;

        void Start()
        {
            mainCam = Camera.main;

            // Create a dynamic look-at point that we'll move
            GameObject lookAtObj = new GameObject("Dynamic LookAt Point");
            dynamicLookAtPoint = lookAtObj.transform;
            dynamicLookAtPoint.position = originalLookAtPoint.position;
            DontDestroyOnLoad(dynamicLookAtPoint);
            virtualCamera.Follow = dynamicLookAtPoint;
            // Set the vcam to look at our dynamic point
            virtualCamera.LookAt = dynamicLookAtPoint;
        }

        void LateUpdate()
        {
            if (mainCam == null || player == null || originalLookAtPoint == null)
                return;

            // Step 1: Compute desired look-at position (shoulder offset)
            Vector3 desiredPosition = player.position + (originalLookAtPoint.position - player.position);

            // Step 2: Prevent the look-at point from going inside walls
            if (Physics.Linecast(player.position, desiredPosition, out RaycastHit hit, collisionLayers))
            {
                // Move it just in front of the wall
                desiredPosition = hit.point - (desiredPosition - player.position).normalized * wallOffset;
            }

            // Step 3: Smoothly move the dynamic look-at point
            dynamicLookAtPoint.position = Vector3.Lerp(
                dynamicLookAtPoint.position,
                desiredPosition,
                Time.deltaTime * pullSpeed
            );
        }
    }
}
