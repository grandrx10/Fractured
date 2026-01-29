using System.Collections.Generic;
using Characters.Interactables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Characters
{
    public class PlayerInteractController : MonoBehaviour
    {
        public Transform head;
        public float headTurnSpeed, headTurnCap;
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

        public static InputBlockerManager PlayerInputs = new InputBlockerManager();
        public GameObject currentLookTarget;
        
        // Interaction progress
        private float holdTimer = 0f;
        private bool isHolding = false;
        
        public Interactable LookingAtInteractable
        {
            get
            {
                if (currentLookTarget && currentLookTarget.TryGetComponent(out Interactable i) &&
                    i.canInteract) return i;
                return null;
            }
        }
        
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

            if (currentLookTarget != null && currentLookTarget.TryGetComponent(out Interactable currentInteractable) && currentInteractable.canInteract)
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

            HeadAnimation();
        }

        private void HeadAnimation()
        {
            
            Vector3 worldDir = raycastOrigin.forward;
            Vector3 localDir = head.parent.InverseTransformDirection(worldDir);

            // Flatten to local Y plane
            localDir.y = 0f;
            if (localDir.sqrMagnitude < 0.0001f)
                return;

            // Desired yaw (local space)
            float targetYaw = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

            // Clamp yaw
            float clampedYaw = Mathf.Clamp(targetYaw, -headTurnCap, headTurnCap);

            // Smooth rotate
            float currentYaw = head.localEulerAngles.y;
            if (currentYaw > 180f) currentYaw -= 360f;

            float newYaw = Mathf.MoveTowards(
                currentYaw,
                clampedYaw,
                headTurnSpeed * Time.deltaTime
            );

            head.localRotation = Quaternion.Euler(0f, newYaw, 0f);
            
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

        public RaycastHit GetPlayerLookTarget(LayerMask layers)
        {
            bool hitSomething;
            RaycastHit hit;
            Vector3 pos = raycastOrigin.position + raycastOrigin.forward * 0.3f;
            if (useSphereCast)
            {
                hitSomething = Physics.SphereCast(
                    pos,
                    sphereCastRadius,
                    raycastOrigin.forward,
                    out hit,
                    interactRange,
                    layers,
                    QueryTriggerInteraction.Ignore
                );
            }
            else
            {
                hitSomething = Physics.Raycast(
                    pos,
                    raycastOrigin.forward,
                    out hit,
                    interactRange,
                    layers,
                    QueryTriggerInteraction.Ignore
                );
            }
            if (hitSomething) return hit;
            return new RaycastHit();
        }

        private void CheckForInteractable()
        {
            GameObject go = PhysicsHelper.MainObj(GetPlayerLookTarget(interactableLayer).collider);
            // Debug.Log(go);
            if (showDebugRay)
                Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * interactRange, Color.red);
            if (go)
            {
                currentLookTarget = go;
                return;
            }

            currentLookTarget = null;
            
        }

        private void UpdateUi()
        {
            if (interactText != null)
            {
                var i = LookingAtInteractable;
                if (i)
                {
                    interactText.gameObject.SetActive(true);
                    interactText.text = i.interactText;
                }
                else
                {
                    interactText.gameObject.SetActive(false);
                }
            }
        }

        public Vector3 GetCameraRaycastTarget()
        {
            Vector3 pos = raycastOrigin.position + raycastOrigin.forward * 0.3f;
            
            Ray ray = new Ray(pos, raycastOrigin.forward);
            RaycastHit hit;

            
            if (Physics.Raycast(ray, out hit, 500f, interactableLayer))
            {
                Debug.Log(hit.collider.name);
                return hit.point;
            }

            return raycastOrigin.position + raycastOrigin.forward * 500f;
        }

    }
}
