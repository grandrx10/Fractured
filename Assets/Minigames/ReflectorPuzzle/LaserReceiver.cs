using UnityEngine;
using Characters.Dialogue;

public class LaserReceiver : MonoBehaviour
{
    [Header("Timing")]
    public float requiredHoldTime = 2f;

    [Header("Dialogue")]
    public TriggerDialogueEvent dialogueEvent;

    [Header("Puzzle")]
    public LaserPuzzleManager puzzleManager;

    private float currentHoldTime = 0f;
    private bool completed = false;
    private bool hitThisFrame = false;

    void Update()
    {
        if (completed)
            return;

        if (hitThisFrame)
        {
            currentHoldTime += Time.deltaTime;

            if (currentHoldTime >= requiredHoldTime)
            {
                CompletePuzzle();
            }
        }
        else
        {
            currentHoldTime = 0f;
        }

        hitThisFrame = false;
    }

    public void RegisterLaserHit()
    {
        hitThisFrame = true;
    }

    private void CompletePuzzle()
    {
        completed = true;

        if (dialogueEvent != null)
            dialogueEvent.TryTrigger();
        else
            Debug.LogWarning("LaserReceiver missing dialogueEvent.");

        if (puzzleManager != null)
            puzzleManager.SendMessage("OnPuzzleSolved", SendMessageOptions.DontRequireReceiver);
        else
            Debug.LogWarning("LaserReceiver missing PuzzleManager reference.");
    }

    public void ResetReceiver()
    {
        completed = false;
        currentHoldTime = 0f;
        hitThisFrame = false;
    }
}
