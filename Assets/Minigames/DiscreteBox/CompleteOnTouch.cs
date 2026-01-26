using UnityEngine;

public class CompleteOnTouch : MonoBehaviour
{
    [Header("Completion")]
    [Tooltip("Tag used to identify the player")]
    public string playerTag = "Player";

    [Tooltip("Puzzle to complete when touched")]
    public PuzzleManager puzzleManager;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag(playerTag))
            return;

        if (puzzleManager == null)
        {
            Debug.LogWarning("CompleteOnTouch missing PuzzleManager reference.");
            return;
        }

        triggered = true;

        // Complete the puzzle via the base class
        puzzleManager.OnPuzzleSolved();
    }
}
