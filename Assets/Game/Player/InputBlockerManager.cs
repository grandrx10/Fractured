using System.Collections.Generic;

public class InputBlockerManager
{
    // name → priority
    private readonly Dictionary<string, int> blockers = new();

    private int cachedMaxBlocker = int.MinValue;
    private bool dirty = false;
    public bool InCombat;
    /// <summary>
    /// Add or update an input blocker.
    /// Higher priority blocks lower priority inputs.
    /// </summary>
    public void AddBlocker(string name, InputBlockPrio priority)
    {
        AddBlocker(name, (int)priority);
    }
    public void AddBlocker(string name, int priority)
    {
        blockers[name] = priority;
        dirty = true;
    }

    /// <summary>
    /// Remove an input blocker by name.
    /// </summary>
    public void RemoveBlocker(string name)
    {
        if (blockers.Remove(name))
            dirty = true;
    }
    
    public bool IsInputAllowed(InputBlockPrio inputPriority)
    {
        return IsInputAllowed((int) inputPriority);
    }
    public bool IsInputAllowed(int inputPriority)
    {
        if (blockers.Count == 0)
            return true;

        if (dirty)
            RecalculateMax();

        return inputPriority >= cachedMaxBlocker;
    }

    /// <summary>
    /// Clears all blockers.
    /// </summary>
    public void Clear()
    {
        blockers.Clear();
        cachedMaxBlocker = int.MinValue;
        dirty = false;
    }

    /// <summary>
    /// Returns the current highest blocker priority.
    /// Useful for debugging.
    /// </summary>
    public int CurrentBlockerPriority
    {
        get
        {
            if (dirty)
                RecalculateMax();
            return cachedMaxBlocker;
        }
    }

    private void RecalculateMax()
    {
        cachedMaxBlocker = int.MinValue;

        foreach (var p in blockers.Values)
            if (p > cachedMaxBlocker)
                cachedMaxBlocker = p;

        dirty = false;
    }
}

public enum InputBlockPrio
{
    StandardInput = 0,
    Dialogue = 3,
    Inventory = 5,
    Menu = 10
}