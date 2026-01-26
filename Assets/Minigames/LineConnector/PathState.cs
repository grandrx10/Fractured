using System.Collections.Generic;

public class PathState
{
    public int pairId;
    public LinePressurePlate startPlate;  // Add this line
    public List<LinePressurePlate> plates = new();
    public bool completed;

    public void Reset()
    {
        foreach (var plate in plates)
            plate.Clear();

        plates.Clear();
        completed = false;
        startPlate = null;  // Add this line
    }
}