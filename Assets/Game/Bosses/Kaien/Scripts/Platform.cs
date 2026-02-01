using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Materials")]
    public Material activatedMaterial;

    [Header("Fall Settings")]
    public float initialFallVelocity = 8f;

    [Header("Return Settings")]
    public float returnDuration = 1.5f;

    [Header("Activation")]
    public LayerMask groundLayer;

    [Header("Blast")]
    public float blastForce = 12f;
    public float blastUpwardForce = 4f;

    [Header("Overlap Check")]
    public float overlapCheckRadius = 0.5f;

    private Rigidbody rb;
    private Renderer platformRenderer;
    private Material originalMaterial;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isActivated = false;
    private bool isInCycle = false;
    [Header("Permanent Fall")]
    public bool noRecovery = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        platformRenderer = GetComponent<Renderer>();

        originalMaterial = platformRenderer.sharedMaterial;

        rb.isKinematic = true;
    }

    public void Activate()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        rb.isKinematic = true;
        

        platformRenderer.material = originalMaterial;

        isActivated = true;
        isInCycle = false;
    }

    /* =======================
     * PLAYER — collision
     * ======================= */
    private void OnCollisionEnter(Collision collision)
    {
        if (!isActivated || isInCycle)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        isInCycle = true;
        StartCoroutine(FallRoutineDelayed());
    }

    /* =======================
     * BUILDINGS — trigger
     * ======================= */
    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated || isInCycle)
            return;

        if ((groundLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        isInCycle = true;
        StartCoroutine(FallRoutineImmediate(other.transform));
    }

    /* =======================
     * PLAYER FALL (delayed)
     * ======================= */
    private IEnumerator FallRoutineDelayed()
    {
        if (activatedMaterial != null)
            platformRenderer.material = activatedMaterial;

        yield return new WaitForSeconds(3f);

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.down * initialFallVelocity;

        yield return new WaitForSeconds(7f);

        if (noRecovery)
            yield break;

        ResetPhysics();
        yield return StartCoroutine(ReturnToStart());

        platformRenderer.material = originalMaterial;
        isInCycle = false;

    }

    /* =======================
     * BUILDING FALL (instant + blast)
     * ======================= */
    private IEnumerator FallRoutineImmediate(Transform impactSource)
    {
        if (activatedMaterial != null)
            platformRenderer.material = activatedMaterial;

        rb.isKinematic = false;

        Vector3 blastDir = (transform.position - impactSource.position).normalized;
        Vector3 velocity =
            blastDir * blastForce +
            Vector3.down * initialFallVelocity +
            Vector3.up * blastUpwardForce;

        rb.linearVelocity = velocity;

        yield return new WaitForSeconds(7f);

        if (noRecovery)
            yield break;

        ResetPhysics();
        yield return StartCoroutine(ReturnToStart());

        platformRenderer.material = originalMaterial;
        isInCycle = false;

    }

    /* =======================
     * RETURN + OVERLAP CHECK
     * ======================= */
    private IEnumerator ReturnToStart()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / returnDuration;

            transform.position = Vector3.Lerp(startPos, originalPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, originalRotation, t);

            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        // IMMEDIATE overlap check after regeneration
        CheckImmediateOverlap();
    }

    private void CheckImmediateOverlap()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            overlapCheckRadius,
            groundLayer,
            QueryTriggerInteraction.Collide
        );

        if (hits.Length == 0)
            return;

        // Immediately knock it down again using first overlap
        isInCycle = true;
        StartCoroutine(FallRoutineImmediate(hits[0].transform));
    }

    public void ForcePermanentFall()
    {
        if (isInCycle)
            return;

        noRecovery = true;
        isActivated = true;
        isInCycle = true;

        StopAllCoroutines();
        StartCoroutine(FallRoutineDelayed());
    }


    private void ResetPhysics()
    {
        rb.isKinematic = true;
        // rb.linearVelocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;
    }
}
