using System.Collections;
using UnityEngine;
using Cards.Environments;

public class HeavenBlade : MonoBehaviour
{
    public float aimDuration = 1f;
    public float launchVelocity = 100f;
    public float targetRadius = 5f;
    public float launchDelay = 0.5f;
    public GameObject partToDelete;

    [Header("Audio")]
    public AudioClip launchSound;
    public float launchVolume = 1f;

    private Transform player;

    void Start()
    {
        player = OpenWorldEnv.Current.PlayerTransform;
        StartCoroutine(AimAndLaunch());
    }

    private IEnumerator AimAndLaunch()
    {
        Vector2 randomOffset = Random.insideUnitCircle * targetRadius;
        Vector3 targetPos = player.position + new Vector3(randomOffset.x, 0, randomOffset.y);

        Quaternion initialRot = transform.rotation;
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(direction);

        float timer = 0f;
        while (timer < aimDuration)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(initialRot, targetRot, timer / aimDuration);
            yield return null;
        }

        transform.rotation = targetRot;

        yield return new WaitForSeconds(launchDelay);

        if (partToDelete != null)
            Destroy(partToDelete);

        // 🔊 Play launch sound
        if (launchSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(
                launchSound,
                transform.position,
                launchVolume
            );
        }

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
