using UnityEngine;
using Characters.Interactables;

public class PuzzleResetButton : Interactable
{
    [Header("Reset Button Settings")]
    [SerializeField] private GridPuzzleManager puzzleManager;
    
    [Header("Visual Feedback")]
    [SerializeField] private Renderer buttonRenderer;
    [SerializeField] private Color normalColor = Color.red;
    [SerializeField] private Color pressedColor = Color.yellow;
    [SerializeField] private float pressAnimationDuration = 0.2f;
    
    private Material buttonMaterial;
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

        // Set up button material
        if (buttonRenderer != null)
        {
            buttonMaterial = new Material(buttonRenderer.material);
            buttonRenderer.material = buttonMaterial;
            buttonMaterial.color = normalColor;
        }
    }

    public override void Interact(GameObject player)
    {
        if (!canInteract || isAnimating)
            return;

        Debug.Log("Resetting puzzle...");
        
        // Visual feedback
        if (buttonRenderer != null)
            StartCoroutine(PressAnimation());
        
        // Reset the puzzle
        puzzleManager.ResetPuzzle();
    }

    private System.Collections.IEnumerator PressAnimation()
    {
        isAnimating = true;
        
        // Press down
        buttonMaterial.color = pressedColor;
        yield return new WaitForSeconds(pressAnimationDuration);
        
        // Return to normal
        buttonMaterial.color = normalColor;
        
        isAnimating = false;
    }

    private void OnDestroy()
    {
        if (buttonMaterial != null)
            Destroy(buttonMaterial);
    }
}