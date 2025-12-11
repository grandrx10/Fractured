using Characters.Interactables;
using TMPro;
using UnityEngine;

namespace Characters
{
    public class PlayerInteractController : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        [Header("Raycast Settings")]
        [SerializeField] private Transform raycastOrigin;
        [SerializeField] private bool useSphereCast = false;
        [SerializeField] private float sphereCastRadius = 0.5f;

        [Header("UI Settings")]
        [SerializeField] private TextMeshProUGUI interactText; // assign in inspector

        [Header("Debug")]
        [SerializeField] private bool showDebugRay = true;

        public Interactable currentInteractable;

        private void Start()
        {
            if (raycastOrigin == null)
                raycastOrigin = Camera.main.transform;

            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }

        private void Update()
        {
            CheckForInteractable();

            if (Input.GetKeyDown(interactKey))
                TryInteract();

            UpdateUi();
        }

        private void CheckForInteractable()
        {
            RaycastHit hit;
            bool hitSomething;

            if (useSphereCast)
            {
                hitSomething = Physics.SphereCast(
                    raycastOrigin.position,
                    sphereCastRadius,
                    raycastOrigin.forward,
                    out hit,
                    interactRange,
                    interactableLayer
                );
            }
            else
            {
                hitSomething = Physics.Raycast(
                    raycastOrigin.position,
                    raycastOrigin.forward,
                    out hit,
                    interactRange,
                    interactableLayer
                );
            }

            if (hitSomething)
            {
                Interactable found =
                    hit.collider.GetComponentInParent<Interactable>() ??
                    hit.collider.GetComponent<Interactable>() ??
                    hit.collider.GetComponentInChildren<Interactable>();

                if (found != null && found.canInteract)
                {
                    currentInteractable = found;

                    if (showDebugRay)
                        Debug.DrawLine(raycastOrigin.position, hit.point, Color.green);

                    return;
                }
            }

            currentInteractable = null;

            if (showDebugRay)
                Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * interactRange, Color.red);
        }

        private void TryInteract()
        {
            if (currentInteractable != null && currentInteractable.canInteract)
                currentInteractable.Interact(gameObject);
        }

        private void UpdateUi()
        {
            if (interactText == null) return;

            bool visible = currentInteractable != null;
            interactText.gameObject.SetActive(visible);
        }

        public Vector3 GetCameraRaycastTarget()
        {
            Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
            RaycastHit hit;

            // Build mask: ignore Projectile layer
            int ignoreProjectileMask = ~(1 << LayerMask.NameToLayer("Projectile"));

            if (Physics.Raycast(ray, out hit, 500f, ignoreProjectileMask))
            {
                return hit.point;
            }

            return raycastOrigin.position + raycastOrigin.forward * 500f;
        }

    }
}
