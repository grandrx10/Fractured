using UnityEngine;
using System.Collections;

public class SunDance : MonoBehaviour
{
    [Header("Scaling")]
    public float targetScale = 5f;
    public float scaleDuration = 2f;
    public float shrinkDuration = 2f; // time to shrink at the end
    public float lifetime = 20f; // total time before shrinking

    [Header("Laser Spawning")]
    public GameObject laserPrefab;
    public int numberOfLasers = 10;
    public float laserSpawnDelay = 2f;

    [Header("Rotation")]
    public float rotationAccelerationTime = 2f; // time to reach max speed
    public float rotationDecelerationTime = 2f; // time to slow down before changing direction
    public float maxRotationSpeed = 45f; // degrees per second
    public float directionChangeInterval = 10f; // seconds
    private Vector3 currentRotationAxis;
    private float currentRotationSpeed = 0f;

    private bool isRotating = false;

    void Start()
    {
        transform.localScale = Vector3.one;
        StartCoroutine(ScaleUp());
        StartCoroutine(LifetimeRoutine());
    }

    IEnumerator ScaleUp()
    {
        Vector3 initialScale = transform.localScale;
        Vector3 finalScale = Vector3.one * targetScale;

        float elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsed / scaleDuration);
            yield return null;
        }

        transform.localScale = finalScale;

        // After scaling, spawn lasers
        yield return new WaitForSeconds(laserSpawnDelay);
        SpawnLasers();

        // Start rotation after scaling + laser spawn
        StartCoroutine(RotationRoutine());
    }

    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(currentRotationAxis, currentRotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void SpawnLasers()
    {
        if (laserPrefab == null)
        {
            Debug.LogWarning("Laser prefab not assigned!");
            return;
        }

        for (int i = 0; i < numberOfLasers; i++)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Random.rotation, transform);
        }
    }

    private IEnumerator RotationRoutine()
    {
        isRotating = true;

        while (true)
        {
            // Choose a new random direction
            currentRotationAxis = Random.onUnitSphere;

            // Accelerate to max speed
            float elapsed = 0f;
            while (elapsed < rotationAccelerationTime)
            {
                elapsed += Time.deltaTime;
                currentRotationSpeed = Mathf.Lerp(0f, maxRotationSpeed, elapsed / rotationAccelerationTime);
                yield return null;
            }

            currentRotationSpeed = maxRotationSpeed;

            // Rotate for directionChangeInterval minus acceleration and deceleration
            float rotationTime = directionChangeInterval - rotationAccelerationTime - rotationDecelerationTime;
            yield return new WaitForSeconds(rotationTime);

            // Decelerate to 0 before changing direction
            elapsed = 0f;
            float startSpeed = currentRotationSpeed;
            while (elapsed < rotationDecelerationTime)
            {
                elapsed += Time.deltaTime;
                currentRotationSpeed = Mathf.Lerp(startSpeed, 0f, elapsed / rotationDecelerationTime);
                yield return null;
            }

            currentRotationSpeed = 0f;
        }
    }

    private IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(lifetime);

        // Shrink to 0.1 scale
        Vector3 initialScale = transform.localScale;
        Vector3 finalScale = Vector3.one * 0.1f;
        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsed / shrinkDuration);
            yield return null;
        }

        transform.localScale = finalScale;

        // Destroy self
        Destroy(gameObject);
    }
}
