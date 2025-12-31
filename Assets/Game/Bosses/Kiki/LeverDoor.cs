using UnityEngine;

public class LeverDoor : Door
{
    protected override void Update()
    {
        // Disable automatic condition-based opening
    }

    public void Toggle()
    {
        // Do nothing if this door has been disabled (eg. power out)
        if (!enabled)
            return;

        if (IsMoving())
            return;

        if (IsOpen())
            Close();
        else
            Open();
    }
}
