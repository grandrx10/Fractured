using System.Collections;
using UnityEngine;
using Cards.Environments;

public class HeavenBlade : MonoBehaviour
{
    public float aimDuration = 1f;          // Time to rotate towards target
    public float launchVelocity = 100f;     // Speed of blade
    public float targetRadius = 5f;         // Random offset radius around player
    public float launchDelay = 0.5f;        // Delay after aiming before launch
    public GameObject partToDelete;         // Part of blade to remove on launch

    private Transform player;

    void Start()
    {
        // Find the player in the open world environment
        player = OpenWorldEnv.Current.PlayerTransform;

        // Start aiming coroutine
        StartCoroutine(AimAndLaunch());
    }

    private IEnumerator AimAndLaunch()
    {
        // Pick random point near the player
        Vector2 randomOffset = Random.insideUnitCircle * targetRadius;
        Vector3 targetPos = player.position + new Vector3(randomOffset.x, 0, randomOffset.y);

        // Store initial rotation
        Quaternion initialRot = transform.rotation;

        // Calculate target rotation to face the targetPos
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(direction);

        // Rotate over aimDuration
        float timer = 0f;
        while (timer < aimDuration)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(initialRot, targetRot, timer / aimDuration);
            yield return null;
        }

        transform.rotation = targetRot;

        // Wait before launching
        yield return new WaitForSeconds(launchDelay);

        // Delete specified part
        if (partToDelete != null)
        {
            Destroy(partToDelete);
        }

        // Launch blade
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
        rb.isKinematic = false;
        rb.linearVelocity = direction * launchVelocity;
    }
}
