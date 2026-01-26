using System;
using UnityEngine;
using System.Collections.Generic;
using Game;

public class SymbolDoor : Door
{
    [Header("Symbol Puzzle")]
    [Tooltip("Buttons controlling the symbols")]
    public List<SymbolChanger> symbolChangers = new List<SymbolChanger>();

    [Tooltip("Correct index for each symbol (must match order above)")]
    public List<int> correctIndices = new List<int>();
    
    public override bool CheckCondition()
    {
        if (symbolChangers.Count == 0 || correctIndices.Count == 0)
            return false;

        if (symbolChangers.Count != correctIndices.Count)
        {
            Debug.LogWarning($"{name}: Symbol list and index list size mismatch.");
            return false;
        }

        for (int i = 0; i < symbolChangers.Count; i++)
        {
            if (symbolChangers[i] == null)
                return false;

            if (symbolChangers[i].CurrentIndex != correctIndices[i])
                return false;
        }

        return true;
    }
}
