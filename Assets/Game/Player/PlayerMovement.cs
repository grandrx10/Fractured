using System;
using Cards;
using Cards.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed;
        
        public float walkSpeed;
        public float groundDrag, airDrag;
        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier;
        public float rotationSpeed = 5;
        bool readyToJump;
        public AnimationCurve dotDirCurve, speedForceCurve;
        public AnimationCurve speedCurve, airCurve;
        
        [Header("Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;

        [Header("Ground Check")]
        public LayerMask whatIsGround;
        public Collider groundCheckCollider;
        bool grounded;
        private bool wasGrounded; // Track previous grounded state
        public Animator animator;
        public PlayerAgent agent;
        public Transform orientation;
        public bool MovementOverride { get; set; }
        public bool LookForward { get; set; }

        [Header("Audio")]
        public AudioClip footstepClip;
        public AudioClip jumpClip;
        public AudioClip landClip; // Optional landing sound
        public float footstepInterval = 0.45f;
        public float minFootstepSpeed = 1.2f;
        private float footstepTimer;
        private bool isMoving; // Track if player is actually moving
        private float landCooldown = 0f; // Prevent multiple land sounds

        float horizontalInput;
        float verticalInput;

        Vector3 moveDirection;

        Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            readyToJump = true;
            agent.OnUseCard += UseCard;
            Application.targetFrameRate = 60;
        }

        private void UseCard(Card c)
        {
            animator.SetTrigger("Throw");
        }
        
        public Vector3 GetPositionBelow(float distance)
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, distance, whatIsGround))
                return hit.point;

            return transform.position;
        }

        private void Update()
        {
            MyInput();
            
            // Handle jump input immediately in Update for instant response
            if (PlayerInteractController.PlayerInputs.IsInputAllowed(InputBlockPrio.StandardInput) && 
                !MovementOverride &&
                Input.GetKeyDown(jumpKey) && 
                readyToJump && 
                grounded)
            {
                readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }
            
            rb.linearDamping = grounded ? groundDrag : 0;
            HandleFootsteps();
            
            // Decrease land cooldown
            if (landCooldown > 0f)
                landCooldown -= Time.deltaTime;
            
            // Detect landing - play when transitioning from air to ground
            if (grounded && !wasGrounded && landClip != null && landCooldown <= 0f)
            {
                AudioManager.Instance.PlayOneShot(
                    landClip,
                    transform.position,
                    0.8f
                );
                landCooldown = 0.3f; // Prevent retriggering
            }
            
            wasGrounded = grounded;
        }

        private void HandleFootsteps()
        {
            // Get horizontal velocity (ignore vertical movement)
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            float speed = flatVel.magnitude;

            // Check if player is actually moving and on ground
            bool shouldPlayFootsteps = grounded && 
                                      speed >= minFootstepSpeed && 
                                      moveDirection.magnitude > 0.1f; // Check input as well

            if (!shouldPlayFootsteps)
            {
                footstepTimer = 0f;
                isMoving = false;
                return;
            }

            isMoving = true;

            // Countdown timer for step interval
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                // Play the footstep sound at the player's position
                AudioManager.Instance.PlayOneShot(
                    footstepClip,
                    transform.position,
                    0.7f
                );

                // Adjust interval based on movement speed
                float speedNormalized = Mathf.InverseLerp(minFootstepSpeed, moveSpeed, speed);
                footstepTimer = Mathf.Lerp(0.6f, 0.3f, speedNormalized);
            }
            rb.linearDamping = grounded ? groundDrag : airDrag;
        }

        private int _airFrames = 0;
        private void FixedUpdate()
        {
            MovePlayer();
            
            animator.SetBool("MovingUp", rb.linearVelocity.y > 0);
            if (!grounded) _airFrames++;
            else _airFrames = 0;
            animator.SetFloat("Fall", airCurve.Evaluate(_airFrames));
            animator.SetBool("Grounded", _airFrames <= 20);
        }

        private void MyInput()
        {
            if (!PlayerInteractController.PlayerInputs.IsInputAllowed(InputBlockPrio.StandardInput) || MovementOverride)
            {
                horizontalInput = 0;
                verticalInput = 0;
                return;
            }
            
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }
        
        float GetSpeed()
        {
            float walkspeed = Mathf.Min(moveSpeed, walkSpeed);
            float sprintSpeed = Mathf.Max(moveSpeed, walkSpeed);
            float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkspeed;
            return speed;
        }

        private void MovePlayer()
        {
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
            moveDirection = inputDir.normalized;
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (LookForward)
            {
                var flatLook = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flatLook), Time.fixedDeltaTime * rotationSpeed));
            } else if (inputDir != Vector3.zero && !LookForward)
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputDir), Time.fixedDeltaTime * rotationSpeed));
            }

            
            animator.SetBool("Moving", moveDirection.magnitude >= 0.1f);
            var dot = Vector3.Dot(moveDirection, flatVel.normalized);
            var s = dotDirCurve.Evaluate(dot) 
                    * speedForceCurve.Evaluate(flatVel.magnitude/moveSpeed * dot);
            var speed = GetSpeed() * s;
            var airFactor = grounded? 1 : airMultiplier;
            rb.AddForce(moveDirection * speed * 10f * airFactor, ForceMode.Force);
            SpeedControl();
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            animator.SetFloat("Speed", speedCurve.Evaluate(flatVel.magnitude) * ( _airFrames <= 20 ? 1 : 0.1f));
            
            var speed = GetSpeed();
            if (!MovementOverride && flatVel.magnitude > speed && grounded)
            {
                Vector3 limitedVel = flatVel.normalized * speed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

        private void Jump()
        {
            // Play jump sound IMMEDIATELY when jump is called
            if (jumpClip != null)
            {
                AudioManager.Instance.PlayOneShot(
                    jumpClip,
                    transform.position,
                    1f
                );
            }
            
            // Reset footstep timer to prevent immediate footstep after landing
            footstepTimer = 0f;
            
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }

        private void ResetJump()
        {
            readyToJump = true;
        }

        public float maxSlopeAngle = 45f;
        [FormerlySerializedAs("slideAcceleration")] public float slideAccelerationFactor = 30f;
        public float sideDrag = 6f;

        private void OnCollisionStay(Collision other)
        {
            if (((1 << other.gameObject.layer) & whatIsGround) == 0 || !readyToJump)
                return;

            Vector3 n = other.contacts[0].normal.normalized;

            float slopeCos = Vector3.Dot(n, Vector3.up);
            float slopeAngle = Mathf.Acos(Mathf.Clamp(slopeCos, -1f, 1f)) * Mathf.Rad2Deg;

            if (slopeAngle <= maxSlopeAngle)
                return;

            Vector3 v = rb.linearVelocity;

            // Surface directions
            Vector3 downhill = Vector3.ProjectOnPlane(Vector3.down, n).normalized;
            Vector3 uphill = -downhill;
            Vector3 sideways = Vector3.Cross(n, downhill).normalized;

            // 1️⃣ Block uphill motion
            float uphillSpeed = Vector3.Dot(v, uphill);
            if (uphillSpeed > 0f)
                v -= uphill * uphillSpeed;

            // 2️⃣ Apply sideways drag
            float sideSpeed = Vector3.Dot(v, sideways);
            v -= sideways * sideSpeed * sideDrag * Time.fixedDeltaTime;

            // 3️⃣ Apply downhill gravity
            Vector3 slideAccel = Vector3.ProjectOnPlane(Physics.gravity, n);
            v += slideAccel * Time.fixedDeltaTime * moveSpeed * slideAccelerationFactor;

            rb.linearVelocity = v;
        }

        
        // Trigger-based ground detection
        private void OnTriggerStay(Collider other)
        {
            if (((1 << other.gameObject.layer) & whatIsGround) != 0 && !other.isTrigger)
            {
                grounded = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & whatIsGround) != 0 && !other.isTrigger)
            {
                grounded = false;
            }
        }

    }
}