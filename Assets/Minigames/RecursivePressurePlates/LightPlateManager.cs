// LightPlateManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LightPlateManager : PuzzleManager
{
    [SerializeField] private List<LightPlate> plates = new List<LightPlate>();
    [ContextMenu("Solve")]
    public override void OnPuzzleSolved()
    {
        base.OnPuzzleSolved();
    }

    public override void ResetPuzzle()
    {
        foreach (LightPlate plate in plates)
        {
            if (plate.IsPressed) plate.TogglePlate(true);
        }
        base.ResetPuzzle();
    }

    private void Update()
    {
        // Check every frame if all plates are pressed
        if (plates.Count > 0 && plates.All(plate => plate.IsPressed))
        {
            OnPuzzleSolved();
        }
    }
}