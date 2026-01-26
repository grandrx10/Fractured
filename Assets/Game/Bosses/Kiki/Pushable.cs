using UnityEngine;
using Characters.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class Pushable : Interactable
{
    [Header("Push Settings")]
    public float pushForce = 6f;
    public bool ignoreY = true; // keeps push horizontal

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Interact(GameObject player)
    {
        if (!canInteract || rb == null) return;

        Vector3 direction = transform.position - player.transform.position;

        if (ignoreY)
            direction.y = 0f;

        direction.Normalize();

        rb.AddForce(direction * pushForce, ForceMode.Impulse);
    }
}
