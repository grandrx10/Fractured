using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscretePuzzleDoor : Door
{
    [Header("Puzzle Settings")]
    [Tooltip("All detectors that must be active to open this door")]
    public List<DiscreteBoxDetector> detectors = new List<DiscreteBoxDetector>();

    protected override void Update()
    {
        if (isMoving)
            return;

        // Only open if all detectors are active
        bool allActive = detectors.Count > 0 && detectors.TrueForAll(d => d.isActive);

        if (allActive && !isOpen)
        {
            Open();
        }
        else if (!allActive && isOpen)
        {
            Close();
        }
    }
}
