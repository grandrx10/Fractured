using UnityEngine;
using UnityEngine.Events;

public class PressureDoor : Door
{
    [Header("Pressure Plate Settings")]
    [Tooltip("List of all pressure plates that must be active to open this door")]
    public PressurePlate[] pressurePlates;

    protected override void Update()
    {
        if (isMoving)
            return;

        // Check if all pressure plates are pressed
        bool allPressed = AreAllPlatesPressed();

        if (allPressed && !isOpen)
        {
            Open();
        }
        else if (!allPressed && isOpen)
        {
            Close();
        }
    }

    private bool AreAllPlatesPressed()
    {
        if (pressurePlates == null || pressurePlates.Length == 0)
        {
            Debug.LogWarning($"{name}: No pressure plates assigned!");
            return false;
        }

        foreach (PressurePlate plate in pressurePlates)
        {
            if (plate == null)
            {
                Debug.LogWarning($"{name}: A pressure plate reference is null!");
                continue;
            }

            if (!plate.IsPressed())
            {
                return false;
            }
        }

        return true;
    }

    public override bool CheckCondition()
    {
        return AreAllPlatesPressed();
    }
}