using Unity.Cinemachine;
using UnityEngine;

namespace Characters
{
    public class CinemachineLookAtAdjuster : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private Transform player;
        [SerializeField] private Transform originalLookAtPoint; // Your offset point
    
        [Header("Settings")]
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private float pullSpeed = 10f; // How fast to adjust the look point
    
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
            // Set the vcam to look at our dynamic point
            virtualCamera.LookAt = dynamicLookAtPoint;
        }
    
        void LateUpdate()
        {
            if (mainCam == null || player == null || originalLookAtPoint == null)
                return;
        
            Vector3 cameraPos = mainCam.transform.position;
            Vector3 targetPos = originalLookAtPoint.position;
            Vector3 directionToTarget = targetPos - cameraPos;
            float distanceToTarget = directionToTarget.magnitude;
        
            // Raycast from camera to the offset look-at point
            if (Physics.Raycast(cameraPos, directionToTarget.normalized, out RaycastHit hit, distanceToTarget, collisionLayers))
            {
                // Something is blocking the view, pull the look-at point towards the player
                Vector3 pullDirection = (player.position - originalLookAtPoint.position).normalized;
                float pullDistance = Vector3.Distance(hit.point, originalLookAtPoint.position);
                Vector3 adjustedPosition = originalLookAtPoint.position + pullDirection * pullDistance;
            
                // Smoothly move the dynamic point
                dynamicLookAtPoint.position = Vector3.Lerp(
                    dynamicLookAtPoint.position, 
                    adjustedPosition, 
                    Time.deltaTime * pullSpeed
                );
            }
            else
            {
                // No obstruction, return to original offset position
                dynamicLookAtPoint.position = Vector3.Lerp(
                    dynamicLookAtPoint.position, 
                    originalLookAtPoint.position, 
                    Time.deltaTime * pullSpeed
                );
            }
        }
    }
}