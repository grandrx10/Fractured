using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusHolder : MonoBehaviour
{
    private Dictionary<string, float> statuses = new Dictionary<string, float>();

    /// <summary>
    /// Adds a status or updates it if it already exists.
    /// </summary>
    public void AddStatus(string statusName, float value)
    {
        if (statuses.ContainsKey(statusName))
            statuses[statusName] += value;
        else
            statuses[statusName] = value;
    }

    /// <summary>
    /// Gets the value of a status. Returns 0 if it doesn't exist.
    /// </summary>
    public float GetStatus(string statusName)
    {
        if (statuses.TryGetValue(statusName, out float value))
            return value;

        return 0f;
    }
}
