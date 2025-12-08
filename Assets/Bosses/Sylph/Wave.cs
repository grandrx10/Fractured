using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "BossAttacks/Sylph/WaveAttack")]
public class WaveAttack : BossAttack
{
    [Header("Wave Settings")]
    public GameObject wavePrefab;
    public float waveHeight = 3f;
    public Vector2 waveDistanceRange = new Vector2(8f, 12f); // min and max distance
    
    [Header("Attack Timing")]
    public float waveDuration = 3f;       // How long the wave attack lasts
    public float delayAfterAttack = 1f;   // Delay between attacks
    public float rotationDuration = 0.5f; // Time to rotate boss gradually

    private Coroutine attackCoroutine;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);

        if (wavePrefab == null)
        {
            Debug.LogError("WaveAttack: Wave prefab not assigned!");
            return;
        }

        MonoBehaviour mono = boss.GetComponent<MonoBehaviour>();
        if (attackCoroutine != null)
        {
            mono.StopCoroutine(attackCoroutine);
        }
        attackCoroutine = mono.StartCoroutine(RepeatingWaveAttack(boss));
    }

    public override void EndAttack(GameObject boss)
    {
        base.EndAttack(boss);
        
        MonoBehaviour mono = boss.GetComponent<MonoBehaviour>();
        if (attackCoroutine != null)
        {
            mono.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private IEnumerator RepeatingWaveAttack(GameObject boss)
    {
        while (isActive)
        {
            yield return SpawnAndMoveWave(boss);
            
            if (isActive)
                yield return new WaitForSeconds(delayAfterAttack);
        }
    }

    private IEnumerator SpawnAndMoveWave(GameObject boss)
    {
        Vector3 bossStartPos = boss.transform.position;

        // Instantiate wave at boss position
        GameObject wave = Instantiate(wavePrefab, bossStartPos, Quaternion.identity);

        // Find the "Standpoint" child
        Transform standpoint = wave.transform.Find("Standpoint");
        if (standpoint == null)
        {
            Debug.LogError("WaveAttack: Standpoint not found in wave prefab!");
            Destroy(wave);
            yield break;
        }

        // Position wave so standpoint is exactly at boss's current position
        Vector3 offset = bossStartPos - standpoint.position;
        wave.transform.position += offset;

        // Get player's position to determine direction
        Vector3 playerPos = PlayerSingleton.Instance != null
            ? PlayerSingleton.Instance.GetPositionBelow()
            : boss.transform.position + boss.transform.forward * 5f;

        // Calculate direction
        Vector3 direction = (playerPos - bossStartPos).normalized;
        direction.y = 0; // horizontal only

        // Rotate wave to face player immediately
        if (direction != Vector3.zero)
        {
            wave.transform.rotation = Quaternion.LookRotation(direction);
        }

        // Randomize wave distance
        float waveDistance = Random.Range(waveDistanceRange.x, waveDistanceRange.y);
        Vector3 startPos = wave.transform.position;
        Vector3 endPos = startPos + direction * waveDistance;

        // Gradual rotation of boss toward wave direction
        if (direction != Vector3.zero)
        {
            Quaternion initialRotation = boss.transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            float rotTime = 0f;
            while (rotTime < rotationDuration)
            {
                float t = rotTime / rotationDuration;
                boss.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
                rotTime += Time.deltaTime;
                yield return null;
            }
            boss.transform.rotation = targetRotation;
        }

        // Move wave and boss
        float time = 0f;
        while (time < waveDuration && isActive)
        {
            float t = time / waveDuration;

            // Linear horizontal movement
            Vector3 wavePos = Vector3.Lerp(startPos, endPos, t);

            // Vertical sine wave
            float verticalOffset = Mathf.Sin(t * Mathf.PI) * waveHeight;
            wave.transform.position = wavePos + Vector3.up * verticalOffset;

            // Boss rides the wave
            boss.transform.position = standpoint.position;

            time += Time.deltaTime;
            yield return null;
        }

        // Ensure wave finishes exactly at end position
        wave.transform.position = endPos;
        boss.transform.position = standpoint.position;

        // Destroy wave
        Destroy(wave);
    }
}
