using UnityEngine;
using System.Collections.Generic;
using Characters.Interactables;

public class SymbolChanger : ButtonInteractable
{
    [Header("Symbol Settings")]
    [Tooltip("SpriteRenderer that displays the symbol")]
    public SpriteRenderer symbolRenderer;

    [Tooltip("List of symbols to cycle through")]
    public List<Sprite> symbols = new List<Sprite>();

    [Tooltip("Index to start at")]
    public int startingIndex = 0;

    private int currentIndex = 0;
    public int CurrentIndex => currentIndex;


    protected virtual void Start()
    {
        if (symbolRenderer == null)
        {
            Debug.LogWarning($"{name}: SymbolRenderer not assigned.");
            return;
        }

        if (symbols.Count == 0)
        {
            Debug.LogWarning($"{name}: No symbols assigned.");
            return;
        }

        currentIndex = Mathf.Clamp(startingIndex, 0, symbols.Count - 1);
        symbolRenderer.sprite = symbols[currentIndex];
    }

    public override void Action()
    {
        if (symbolRenderer == null || symbols.Count == 0)
            return;

        currentIndex++;

        if (currentIndex >= symbols.Count)
            currentIndex = 0;

        symbolRenderer.sprite = symbols[currentIndex];
    }
}
