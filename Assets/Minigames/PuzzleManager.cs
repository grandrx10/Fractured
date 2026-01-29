using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class PuzzleManager : MonoBehaviour
{
    protected bool isSolved = false;
    public UnityEvent onSolve;
    /// <summary>
    /// Called when the puzzle is successfully solved.
    /// </summary>
    public virtual void OnPuzzleSolved()
    {
        if (isSolved)
            return;
        onSolve.Invoke();
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
