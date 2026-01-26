using UnityEngine;

public class StickOnContact : MonoBehaviour
{
    private Rigidbody rb;
    private bool isMovingAfterContact = false;
    private Vector3 moveDirection;
    private float distanceRemaining;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float travelDistance = 10f; // distance to move after contact
    [SerializeField] private float travelSpeed = 5f;      // speed while moving upward after contact

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("StickOnContact requires a Rigidbody on the same GameObject.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb == null) return;

        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            Rigidbody otherRb = collision.rigidbody;

            if (otherRb == null || otherRb.isKinematic)
            {
                // Stop current physics movement
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // Set kinematic so it won't be affected by physics anymore
                rb.isKinematic = true;

                // Start moving upward manually
                isMovingAfterContact = true;
                moveDirection = transform.up;
                distanceRemaining = travelDistance;
            }
        }
    }

    void Update()
    {
        if (isMovingAfterContact)
        {
            float moveStep = travelSpeed * Time.deltaTime;

            if (moveStep >= distanceRemaining)
            {
                // Finish moving
                transform.position += moveDirection * distanceRemaining;
                isMovingAfterContact = false;

                // Set layer to Ground
                gameObject.layer = Mathf.RoundToInt(Mathf.Log(groundLayer.value, 2));
            }
            else
            {
                transform.position += moveDirection * moveStep;
                distanceRemaining -= moveStep;
            }
        }
    }
}
