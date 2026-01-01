using Cards.Environments;
using UnityEngine;
using Characters;

namespace Characters.Interactables
{
    public class FishingPool : MonoBehaviour
    {
        [Header("Fishing Minigame")]
        public RhythmFishingBars fishingMinigame;
        public int numberOfBeats = 8;
        public RhythmFishingBars.NoteType[] beatPattern;
        public float bpm = 120f;
        public RhythmFishingBars.Difficulty difficulty = RhythmFishingBars.Difficulty.Medium;

        [Header("Reward Settings")]
        public GameObject rewardPrefab;
        public float rewardLaunchSpeed = 10f;
        public int rewardsToSpawn = 1;
        public float rewardArcAngle = 30f;

        private bool isActive;

        public void Activate()
        {
            if (isActive)
            {
                Debug.Log("FishingPool: Already active, ignoring activation.");
                return;
            }
            
            isActive = true;
            Debug.Log("FishingPool: Activating minigame...");

            if (fishingMinigame == null)
            {
                Debug.LogWarning("FishingPool: No RhythmFishingBars assigned!");
                return;
            }

            fishingMinigame.bpm = bpm;

            if (beatPattern == null || beatPattern.Length == 0)
            {
                beatPattern = new RhythmFishingBars.NoteType[numberOfBeats];
                for (int i = 0; i < numberOfBeats; i++)
                    beatPattern[i] = RhythmFishingBars.NoteType.Quarter;
            }

            fishingMinigame.OnMinigameComplete += OnMinigameCompleteHandler;
            Debug.Log($"FishingPool: Subscribed to OnMinigameComplete. FishingPool instance: {GetInstanceID()}");
            fishingMinigame.StartRhythm(numberOfBeats, beatPattern, difficulty);
        }

        private void OnMinigameCompleteHandler(bool success)
        {
            Debug.Log($"===== FISHINGPOOL HANDLER ENTERED ===== Instance: {GetInstanceID()}, GameObject active: {gameObject.activeInHierarchy}, Component enabled: {enabled}");
            Debug.Log($"FishingPool: OnMinigameCompleteHandler called on instance {GetInstanceID()}! Success: {success}");

            if (success)
            {
                Debug.Log("FishingPool: Attempting to launch rewards...");
                LaunchRewardsAtPlayer();
            }
            else
            {
                Debug.Log("FishingPool: Minigame failed, no rewards.");
            }

            isActive = false;
            if (fishingMinigame != null)
                fishingMinigame.OnMinigameComplete -= OnMinigameCompleteHandler;
        }

        private void LaunchRewardsAtPlayer()
        {
            Debug.Log("FishingPool: LaunchRewardsAtPlayer called");

            if (rewardPrefab == null)
            {
                Debug.LogError("FishingPool: rewardPrefab is NULL! Assign it in the inspector.");
                return;
            }

            Transform playerTransform = OpenWorldEnv.Current.PlayerTransform;

            if (playerTransform == null)
            {
                Debug.LogError("FishingPool: Could not get player transform!");
                return;
            }

            Vector3 spawnPos = transform.position + Vector3.up * 1f;
            Vector3 baseDirection = (playerTransform.position - spawnPos).normalized;

            Debug.Log($"FishingPool: Spawning {rewardsToSpawn} rewards at {spawnPos}, aiming at player at {playerTransform.position}");

            for (int i = 0; i < rewardsToSpawn; i++)
            {
                // Fix potential division by zero when rewardsToSpawn == 1
                float t = rewardsToSpawn > 1 ? i / (float)(rewardsToSpawn - 1) : 0.5f;
                float angleOffset = Mathf.Lerp(-rewardArcAngle / 2, rewardArcAngle / 2, t);
                Vector3 direction = Quaternion.Euler(0f, angleOffset, 0f) * baseDirection;

                GameObject reward = Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
                
                if (reward == null)
                {
                    Debug.LogError($"FishingPool: Failed to instantiate reward {i}!");
                    continue;
                }

                Debug.Log($"FishingPool: Spawned reward {i} at {spawnPos} with direction {direction}");

                Rigidbody rb = reward.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * rewardLaunchSpeed;
                    Debug.Log($"FishingPool: Applied velocity {direction * rewardLaunchSpeed} to reward {i}");
                }
                else
                {
                    Debug.LogWarning($"FishingPool: Reward {i} has no Rigidbody component!");
                }
            }

            Debug.Log("FishingPool: All rewards spawned successfully!");
        }

        public void Deactivate()
        {
            Debug.Log("FishingPool: Deactivating...");
            isActive = false;
        }
    }
}