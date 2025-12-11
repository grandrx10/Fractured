using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Characters
{
    public class ThirdPersonCam : MonoBehaviour
    {
        // ------------------------------
        // Singleton
        // ------------------------------
        public static ThirdPersonCam Instance { get; private set; }
        public CinemachineInputAxisController axisController;
        private void Awake()
        {
            // If there is already an instance and it's not this → destroy this
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }

        // ------------------------------
        // Camera Settings
        // ------------------------------
        public Transform orientation;
        public Transform player;
        public Transform playerObj;
        public Rigidbody rb;

        public float rotationSpeed = 10f;
    
        public LockKeySet CursorUnlock = new LockKeySet();

        public bool CameraLocked => CursorUnlock.Count == 0;

        void Start()
        {
            ApplyCursorState();
        }

        void Update()
        {
            ApplyCursorState();
            axisController.enabled = CameraLocked;

            // If camera is NOT locked → do nothing
            if (!CameraLocked)
                return;

            // --- Existing camera movement ---
            Vector3 viewDir = player.position - new Vector3(
                transform.position.x,
                player.position.y,
                transform.position.z
            );
            orientation.forward = viewDir.normalized;

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(
                    playerObj.forward,
                    inputDir.normalized,
                    Time.deltaTime * rotationSpeed
                );
            }
        }

        void ApplyCursorState()
        {
            if (CameraLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public class LockKeySet
    {
        private readonly HashSet<string> keys = new();

        public void Add(string key) => keys.Add(key);
        public void Remove(string key) => keys.Remove(key);

        // operator overloads to enable += and -=
        public static LockKeySet operator +(LockKeySet set, string key)
        {
            set.Add(key);
            return set;
        }

        public static LockKeySet operator -(LockKeySet set, string key)
        {
            set.Remove(key);
            return set;
        }

        public int Count => keys.Count;
    }
}