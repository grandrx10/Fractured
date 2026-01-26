using System.Collections;
using UnityEngine;
using Cards.Environments;

public class DelayedTriggerHeavenBlade : MonoBehaviour
{
    public float aimDuration = 1f;
    public float launchVelocity = 100f;
    public float targetRadius = 5f;
    public GameObject partToDelete;

    private Transform player;
    private Rigidbody rb;

    private Vector3 launchDirection;
    private bool aimed = false;
    private bool activated = false;

    void Start()
    {
        player = OpenWorldEnv.Current.PlayerTransform;
        StartCoroutine(AimRoutine());
    }

    private IEnumerator AimRoutine()
    {
        Vector2 randomOffset = Random.insideUnitCircle * targetRadius;
        Vector3 targetPos = player.position + new Vector3(randomOffset.x, 0, randomOffset.y);

        Quaternion initialRot = transform.rotation;
        launchDirection = (targetPos - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(launchDirection);

        float timer = 0f;
        while (timer < aimDuration)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(initialRot, targetRot, timer / aimDuration);
            yield return null;
        }

        transform.rotation = targetRot;
        aimed = true;

        // If Activate() was already called, launch immediately
        if (activated)
            Launch();
    }

    public void Activate()
    {
        activated = true;

        if (aimed)
            Launch();
    }

    private void Launch()
    {
        if (rb != null) return; // prevent double-launch

        if (partToDelete != null)
            Destroy(partToDelete);

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
        rb.isKinematic = false;
        rb.linearVelocity = launchDirection * launchVelocity;
    }
}
