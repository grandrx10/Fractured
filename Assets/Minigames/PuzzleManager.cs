using UnityEngine;

public abstract class PuzzleManager : MonoBehaviour
{
    protected bool isSolved = false;

    /// <summary>
    /// Called when the puzzle is successfully solved.
    /// </summary>
    public virtual void OnPuzzleSolved()
    {
        if (isSolved)
            return;

        isSolved = true;
        Debug.Log($"{gameObject.name} puzzle solved.");
    }

    /// <summary>
    /// Resets the puzzle to its initial state.
    /// </summary>
    public virtual void ResetPuzzle()
    {
        isSolved = false;
        Debug.Log($"{gameObject.name} puzzle reset.");
    }
}
