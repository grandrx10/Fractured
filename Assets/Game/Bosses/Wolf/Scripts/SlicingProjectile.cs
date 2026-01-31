using UnityEngine;
using Cards.Card_Assets.RPS.Behaviors;

public class SlicingProjectile : MonoBehaviour
{
    public float speed = 20f;

    [Header("Spin Settings")]
    public float minSpinSpeed = 180f; // degrees per second
    public float maxSpinSpeed = 360f;
    public float rotateSpeed = 360f;
    private Vector3 moveDirection;
    private float spinSpeed;

    void Start()
    {
        // Movement direction
        moveDirection = transform.forward;

        // Random spin speed
        spinSpeed = Random.Range(minSpinSpeed, maxSpinSpeed);
    }

    void Update()
    {

        // Move straight
        transform.position += moveDirection * speed * Time.deltaTime;

        // Spin around forward axis
        transform.Rotate(moveDirection, spinSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self);
    }
}
