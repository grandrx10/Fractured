using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmFishingBars : MonoBehaviour
{
    [Header("Settings")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;
    public Transform middlePoint;
    public GameObject barPrefab;
    public GameObject playerBarPrefab;

    [Header("BPM Settings")]
    public float bpm = 120f;

    [Header("Audio")]
    public AudioSource hitAudioSource;
    public AudioClip demoClip;
    public AudioClip playerHitClip;

    public enum NoteType { Eighth, Quarter, Half, Whole }
    public enum BeatState { Ongoing, Completed, Failed }
    public enum Difficulty { Easy, Medium, Hard }

    [Header("State")]
    public BeatState currentState = BeatState.Ongoing;

    public System.Action<bool> OnMinigameComplete;

    private List<BarData> activeBars = new List<BarData>();

    private class BarData
    {
        public GameObject obj;
        public bool requiresInput;
        public bool hit;
        public float travelTime;
        public float elapsed;
    }

    private float hitWindow = 0.2f;

    public void StartRhythm(int n, NoteType[] pattern, Difficulty difficulty = Difficulty.Medium)
    {
        currentState = BeatState.Ongoing;

        switch (difficulty)
        {
            case Difficulty.Easy: hitWindow = 0.35f; break;
            case Difficulty.Medium: hitWindow = 0.2f; break;
            case Difficulty.Hard: hitWindow = 0.1f; break;
        }

        Debug.Log($"RhythmFishingBars: StartRhythm called. OnMinigameComplete subscribers: {(OnMinigameComplete != null ? OnMinigameComplete.GetInvocationList().Length : 0)}");
        StartCoroutine(RhythmCoroutine(n, pattern));
    }

    private IEnumerator RhythmCoroutine(int n, NoteType[] pattern)
    {
        // 1. Demo phase
        yield return SpawnAndMoveBars(n, pattern, false);

        yield return new WaitForSeconds(0.5f);

        // 2. Player phase
        List<BarData> playerBars = new List<BarData>();
        yield return SpawnAndMoveBars(n, pattern, true, playerBars);

        // ✅ FIXED: Wait a frame to ensure all hit checks are complete
        yield return new WaitForEndOfFrame();

        currentState = BeatState.Completed;

        // Invoke callback
        bool success = true;
        foreach (var bar in playerBars)
        {
            if (!bar.hit)
            {
                success = false;
                break;
            }
        }

        Debug.Log($"Minigame completed, success: {success}. Hits: {playerBars.FindAll(b => b.hit).Count}/{playerBars.Count}");
        Debug.Log($"RhythmFishingBars: About to invoke callback. OnMinigameComplete is null: {OnMinigameComplete == null}");
        
        if (OnMinigameComplete != null)
        {
            Debug.Log($"RhythmFishingBars: Invoking OnMinigameComplete with {OnMinigameComplete.GetInvocationList().Length} subscribers");
            
            // Let's see who's subscribed
            foreach (var subscriber in OnMinigameComplete.GetInvocationList())
            {
                Debug.Log($"Subscriber: {subscriber.Target} | Method: {subscriber.Method.Name}");
            }
            
            try
            {
                OnMinigameComplete.Invoke(success);
                Debug.Log("RhythmFishingBars: Callback invoked successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"RhythmFishingBars: Exception during callback invocation: {e.Message}\n{e.StackTrace}");
            }
        }
        else
        {
            Debug.LogWarning("RhythmFishingBars: OnMinigameComplete is NULL! No subscribers registered.");
        }
    }

    private IEnumerator SpawnAndMoveBars(int n, NoteType[] pattern, bool playerPhase, List<BarData> playerBars = null)
    {
        activeBars.Clear();

        for (int i = 0; i < n; i++)
        {
            NoteType note = pattern[i % pattern.Length];
            GameObject prefab = playerPhase ? playerBarPrefab : barPrefab;

            GameObject leftBar = Instantiate(prefab, leftSpawnPoint.position, Quaternion.identity, transform);
            GameObject rightBar = Instantiate(prefab, rightSpawnPoint.position, Quaternion.identity, transform);

            float travelTime = 1f;

            BarData leftData = new BarData { obj = leftBar, requiresInput = playerPhase, hit = false, travelTime = travelTime, elapsed = 0f };
            BarData rightData = new BarData { obj = rightBar, requiresInput = playerPhase, hit = false, travelTime = travelTime, elapsed = 0f };

            activeBars.Add(leftData);
            activeBars.Add(rightData);

            if (playerPhase && playerBars != null)
            {
                playerBars.Add(leftData);
                playerBars.Add(rightData);
            }

            StartCoroutine(MoveBar(leftData, playerPhase));
            StartCoroutine(MoveBar(rightData, playerPhase));

            float beatDuration = 60f / bpm;
            switch (note)
            {
                case NoteType.Eighth: beatDuration /= 2f; break;
                case NoteType.Quarter: break;
                case NoteType.Half: beatDuration *= 2f; break;
                case NoteType.Whole: beatDuration *= 4f; break;
            }

            yield return new WaitForSeconds(beatDuration);
        }

        while (activeBars.Count > 0)
        {
            activeBars.RemoveAll(bar => bar.obj == null);
            yield return null;
        }
    }

    private IEnumerator MoveBar(BarData barData, bool playerPhase)
    {
        GameObject bar = barData.obj;
        if (bar == null) yield break;

        Vector3 startPos = bar.transform.position;
        Vector3 endPos = middlePoint.position;

        while (barData.elapsed < barData.travelTime)
        {
            if (bar == null) yield break;

            barData.elapsed += Time.deltaTime;
            float t = barData.elapsed / barData.travelTime;
            bar.transform.position = Vector3.Lerp(startPos, endPos, t);

            // ✅ FIXED: Check if we're near the END of travel (close to middle point)
            if (playerPhase && barData.requiresInput && !barData.hit)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // Calculate how close we are to the middle (1.0 = at middle)
                    float normalizedProgress = barData.elapsed / barData.travelTime;
                    float timeToMiddle = Mathf.Abs(1.0f - normalizedProgress) * barData.travelTime;
                    
                    if (timeToMiddle <= hitWindow)
                    {
                        barData.hit = true;
                        Debug.Log($"Bar hit! Progress: {normalizedProgress:F2}, Time to middle: {timeToMiddle:F3}");
                        if (hitAudioSource != null && playerHitClip != null)
                            hitAudioSource.PlayOneShot(playerHitClip);
                    }
                    else
                    {
                        Debug.Log($"Missed! Progress: {normalizedProgress:F2}, Time to middle: {timeToMiddle:F3}, Hit window: {hitWindow}");
                    }
                }
            }

            yield return null;
        }

        if (bar != null)
        {
            bar.transform.position = endPos;
            if (!playerPhase && hitAudioSource != null && demoClip != null)
                hitAudioSource.PlayOneShot(demoClip);

            SpriteRenderer sr = bar.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            Destroy(bar, 0.1f);
        }
    }
}