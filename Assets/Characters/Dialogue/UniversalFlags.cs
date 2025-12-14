using System.Collections.Generic;
using UnityEngine;

public class UniversalFlags : MonoBehaviour
{
    public static UniversalFlags Instance { get; private set; }

    // Internal storage for flags
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Remove if you DON'T want persistence
    }

    /// <summary>
    /// Returns the value of the flag.
    /// If the flag does not exist, returns false.
    /// </summary>
    public bool GetFlag(string key)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        return flags.TryGetValue(key, out bool value) && value;
    }

    /// <summary>
    /// Adds a new flag or updates an existing one.
    /// </summary>
    public void SetFlag(string key, bool value)
    {
        if (string.IsNullOrEmpty(key))
            return;

        flags[key] = value;
    }

    /// <summary>
    /// Removes a flag entirely
    /// </summary>
    public void RemoveFlag(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        flags.Remove(key);
    }

    /// <summary>
    /// Clears all flags
    /// </summary>
    public void ClearFlags()
    {
        flags.Clear();
    }
}
