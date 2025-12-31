using Characters.Interactables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private Slider interactSlider;        // assign in inspector

        [Header("Debug")]
        [SerializeField] private bool showDebugRay = true;

        public Interactable currentInteractable;

        // Interaction progress
        private float holdTimer = 0f;
        private bool isHolding = false;

        private void Start()
        {
            if (raycastOrigin == null)
                raycastOrigin = Camera.main.transform;

            if (interactText != null)
                interactText.gameObject.SetActive(false);

            if (interactSlider != null)
            {
                interactSlider.gameObject.SetActive(false);
                interactSlider.minValue = 0f;
                interactSlider.maxValue = 1f;
                interactSlider.value = 0f;
                interactSlider.wholeNumbers = false;
            }
        }

        private void Update()
        {
            CheckForInteractable();
            UpdateUi();

            if (currentInteractable != null && currentInteractable.canInteract)
            {
                if (currentInteractable.interactTime <= 0f)
                {
                    // Instant interaction
                    if (Input.GetKeyDown(interactKey))
                        currentInteractable.Interact(gameObject);
                }
                else
                {
                    // Hold to interact
                    if (Input.GetKey(interactKey))
                    {
                        isHolding = true;
                        holdTimer += Time.deltaTime;

                        if (interactSlider != null)
                        {
                            interactSlider.gameObject.SetActive(true);
                            interactSlider.value = Mathf.Clamp01(holdTimer / currentInteractable.interactTime);
                        }

                        if (holdTimer >= currentInteractable.interactTime)
                        {
                            currentInteractable.Interact(gameObject);
                            ResetHold();
                        }
                    }
                    else
                    {
                        if (isHolding)
                            ResetHold();
                    }
                }
            }
            else
            {
                ResetHold();
            }
        }

        private void ResetHold()
        {
            isHolding = false;
            holdTimer = 0f;

            if (interactSlider != null)
            {
                interactSlider.value = 0f;
                interactSlider.gameObject.SetActive(false);
            }
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

        private void UpdateUi()
        {
            if (interactText != null) {}
                interactText.gameObject.SetActive(currentInteractable != null);
        }

        public Vector3 GetCameraRaycastTarget()
        {
            Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
            RaycastHit hit;

            // Use the same layer mask as the interactables
            if (Physics.Raycast(ray, out hit, 500f, interactableLayer))
                return hit.point;

            return raycastOrigin.position + raycastOrigin.forward * 500f;
        }

    }
}
