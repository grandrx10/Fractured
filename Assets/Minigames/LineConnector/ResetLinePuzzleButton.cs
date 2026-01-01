using UnityEngine;
using Characters.Interactables;

public class PuzzleResetButton : Interactable
{
    [Header("Reset Button Settings")]
    [SerializeField] private GridPuzzleManager puzzleManager;
    
    [Header("Press Animation")]
    [SerializeField] private Transform buttonTransform;
    [SerializeField] private float pressDepth = 0.1f;
    [SerializeField] private float pressSpeed = 10f;
    
    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;
    private bool isAnimating = false;

    private void Start()
    {
        // Auto-find puzzle manager if not assigned
        if (puzzleManager == null)
            puzzleManager = FindObjectOfType<GridPuzzleManager>();

        if (puzzleManager == null)
        {
            Debug.LogError("PuzzleResetButton: No GridPuzzleManager found in scene!");
            canInteract = false;
            return;
        }

        // Set up button transform
        if (buttonTransform == null)
            buttonTransform = transform;

        originalPosition = buttonTransform.localPosition;
        pressedPosition = originalPosition + Vector3.down * pressDepth;
    }

    public override void Interact(GameObject player)
    {
        if (!canInteract || isAnimating)
            return;

        // Check if puzzle is already completed
        if (puzzleManager.IsPuzzleCompleted())
        {
            Debug.Log("Puzzle is already completed - reset button disabled");
            return;
        }

        Debug.Log("Resetting puzzle...");
        
        // Press animation
        StartCoroutine(PressAnimation());
        
        // Reset the puzzle
        puzzleManager.ResetPuzzle();
    }

    private System.Collections.IEnumerator PressAnimation()
    {
        isAnimating = true;
        
        // Press down
        isPressed = true;
        while (Vector3.Distance(buttonTransform.localPosition, pressedPosition) > 0.001f)
        {
            buttonTransform.localPosition = Vector3.Lerp(
                buttonTransform.localPosition, 
                pressedPosition, 
                Time.deltaTime * pressSpeed
            );
            yield return null;
        }
        buttonTransform.localPosition = pressedPosition;
        
        yield return new WaitForSeconds(0.1f);
        
        // Return to normal
        isPressed = false;
        while (Vector3.Distance(buttonTransform.localPosition, originalPosition) > 0.001f)
        {
            buttonTransform.localPosition = Vector3.Lerp(
                buttonTransform.localPosition, 
                originalPosition, 
                Time.deltaTime * pressSpeed
            );
            yield return null;
        }
        buttonTransform.localPosition = originalPosition;
        
        isAnimating = false;
    }
}