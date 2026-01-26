using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Building : MonoBehaviour
{
    [Header("Ground Detection")]
    public LayerMask groundLayer;

    [Header("Slide Settings")]
    public float slideDistance = 10f;
    public float slideSpeed = 20f;

    private Rigidbody rb;
    private bool hasHitGround = false;

    private float slidSoFar = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHitGround)
            return;

        if (((1 << collision.gameObject.layer) & groundLayer) == 0)
            return;

        hasHitGround = true;

        // Cache original world scale
        Vector3 originalScale = transform.localScale;

        // Parent to what we hit
        transform.SetParent(collision.transform, true);

        // Reset local scale to ignore parent's scale
        transform.localScale = originalScale;

        // Destroy Rigidbody; physics no longer needed
        Destroy(rb);
    }

    private void Update()
    {
        if (!hasHitGround)
            return;

        if (slidSoFar >= slideDistance)
            return;

        float step = slideSpeed * Time.deltaTime;
        transform.position += Vector3.down * step;
        slidSoFar += step;
    }
}
