using System;
using Cards;
using Cards.Core;
using UnityEngine;

namespace Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed;
        public float groundDrag;
        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier;
        public float rotationSpeed = 5;
        bool readyToJump;

        [Header("Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;

        [Header("Ground Check")]
        public LayerMask whatIsGround;
        public Collider groundCheckCollider; // reference to your trigger collider
        bool grounded;
        public Animator animator;
        public PlayerAgent agent;
        public Transform orientation;
        
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
            SpeedControl();
            rb.linearDamping = grounded ? groundDrag : 0;
        }

        private int _airFrames = 0;
        private void FixedUpdate()
        {
            MovePlayer();
            
            animator.SetBool("MovingUp", rb.linearVelocity.y > 0);
            if (!grounded) _airFrames++;
            else _airFrames = 0;
            animator.SetBool("Grounded", _airFrames <= 20);
        }

        private void MyInput()
        {
            if (!PlayerInteractController.PlayerInputs.IsInputAllowed(InputBlockPrio.StandardInput))
            {
                horizontalInput = 0;
                verticalInput = 0;
                return;
            }
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
            
            if (Input.GetKey(jumpKey) && readyToJump && grounded)
            {
                readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        private void MovePlayer()
        {
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
            moveDirection = inputDir;
            
            if (inputDir != Vector3.zero)
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputDir), Time.fixedDeltaTime * rotationSpeed));
            }
            animator.SetBool("Moving", moveDirection.magnitude >= 0.1f);
            
            if (grounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            else
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

        private void Jump()
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }

        private void ResetJump()
        {
            readyToJump = true;
        }

        public float maxSlopeAngle = 45f;
        public float slideAcceleration = 30f;
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

            // 3️⃣ Apply downhill gravity (THIS fixes hovering)
            Vector3 slideAccel = Vector3.ProjectOnPlane(Physics.gravity, n);
            v += slideAccel * Time.fixedDeltaTime * slideAcceleration;

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
