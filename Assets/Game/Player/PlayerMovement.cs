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
