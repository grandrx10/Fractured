using UnityEngine;

namespace Characters.Interactables
{
    public class FishingInteractable : Interactable
    {
        [Header("Fishing Minigame")]
        public RhythmFishingBars fishingMinigame; // Assign the RhythmFishingBars component in Inspector
        public int numberOfBeats = 8; // Default number of beats
        public RhythmFishingBars.NoteType[] beatPattern; // Optional custom pattern
        public float bpm = 120f; // Tempo
        public RhythmFishingBars.Difficulty difficulty = RhythmFishingBars.Difficulty.Medium; // Selectable difficulty

        public override void Interact(GameObject player)
        {
            if (!canInteract) return;

            base.Interact(player);

            if (fishingMinigame != null)
            {
                fishingMinigame.bpm = bpm;

                // Fill default pattern if none provided
                if (beatPattern == null || beatPattern.Length == 0)
                {
                    beatPattern = new RhythmFishingBars.NoteType[numberOfBeats];
                    for (int i = 0; i < numberOfBeats; i++)
                        beatPattern[i] = RhythmFishingBars.NoteType.Quarter;
                }

                // Start the rhythm minigame with selected difficulty
                fishingMinigame.StartRhythm(numberOfBeats, beatPattern, difficulty);
            }
            else
            {
                Debug.LogWarning("FishingInteractable: No RhythmFishingBars assigned!");
            }
        }
    }
}
