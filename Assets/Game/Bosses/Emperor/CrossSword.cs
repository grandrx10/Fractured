using System.Collections;
using UnityEngine;

public class CrossSword : MonoBehaviour
{
    public float moveDuration = 1f;     // Lerp duration
    public float launchSpeed = 40f;     // Constant forward speed
    public float lifetime = 12f;         // Time after launch before breaking apart
    public float breakForce = 15f;      // Force applied to child rigidbodies

    private Vector3 startPos;
    private Vector3 endPos;
    private Rigidbody selfRb;

    public void Initialize(Vector3 start, Vector3 end)
    {
        startPos = start;
        endPos = end;

        transform.position = startPos;

        StartCoroutine(MoveThenLaunch());
    }

    private IEnumerator MoveThenLaunch()
    {
        Quaternion startRot = transform.rotation;

        // Random rotation on Z axis
        float randomZ = Random.Range(0f, 360f);
        Quaternion targetRot = Quaternion.Euler(0f, 0f, randomZ);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;

            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = targetRot;

        // Launch forward
        selfRb = GetComponent<Rigidbody>();
        if (selfRb == null)
        {
            selfRb = gameObject.AddComponent<Rigidbody>();
            selfRb.useGravity = false;
        }

        selfRb.linearVelocity = transform.forward * launchSpeed;

        // Wait for lifetime, then break apart
        yield return new WaitForSeconds(lifetime);
        BreakApart();
    }

    private void BreakApart()
    {
        // Stop this object's movement
        if (selfRb != null)
        {
            selfRb.linearVelocity = Vector3.zero;
            selfRb.isKinematic = true;
        }

        Rigidbody[] childRigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in childRigidbodies)
        {
            if (rb == selfRb)
                continue;

            rb.isKinematic = false;

            Vector3 dir = (rb.worldCenterOfMass - transform.position).normalized;
            rb.AddForce(dir * breakForce, ForceMode.Impulse);
        }
    }
}
