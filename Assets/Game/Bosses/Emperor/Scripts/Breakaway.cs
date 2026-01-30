using System.Collections;
using UnityEngine;

public class Breakaway : MonoBehaviour
{
    public float returnDuration = 3f;
    public float breakKinematicDelay = 3f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Rigidbody rb;
    private Coroutine returnCoroutine;
    private Coroutine breakCoroutine;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Breakaway: No Rigidbody found.");
            return;
        }

        rb.isKinematic = true;
    }

    public void Break()
    {
        if (rb == null) return;

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        if (breakCoroutine != null)
        {
            StopCoroutine(breakCoroutine);
        }

        rb.isKinematic = false;
        breakCoroutine = StartCoroutine(ReKinematicAfterDelay());
    }

    public void Return()
    {
        if (rb == null) return;

        if (breakCoroutine != null)
        {
            StopCoroutine(breakCoroutine);
            breakCoroutine = null;
        }

        rb.isKinematic = true;

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }

        returnCoroutine = StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReKinematicAfterDelay()
    {
        yield return new WaitForSeconds(breakKinematicDelay);
        rb.isKinematic = true;
        breakCoroutine = null;
    }

    private IEnumerator ReturnRoutine()
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

        returnCoroutine = null;
    }
}
