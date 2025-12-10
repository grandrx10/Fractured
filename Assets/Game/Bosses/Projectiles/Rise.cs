using UnityEngine;

public class Rise : MonoBehaviour
{
    [Header("Rise Settings")]
    public float riseHeight = 2f;       // How far to rise upward
    public float riseDuration = 0.5f;   // How long the rise takes
    public float delayBeforeRise = 0f;  // How long to wait before rising

    private Vector3 startPos;
    private Vector3 endPos;
    private float timer = 0f;
    private float delayTimer = 0f;
    private bool rising = false;

    void Update()
    {
        // Wait for delay
        if (!rising)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer >= delayBeforeRise)
            {
                rising = true;

                // Capture the ACTUAL position when the rise starts
                startPos = transform.position;
                endPos = startPos + Vector3.up * riseHeight;

                timer = 0f; // reset just in case
            }

            return;
        }

        // Rising motion
        if (timer < riseDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / riseDuration);
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, endPos, smooth);
        }
    }
}
