using System.Collections.Generic;
using UnityEngine;

public class LaserPuzzleManager : PuzzleManager
{
    [Header("Puzzle Components")]
    public LaserReceiver receiver;

    [Header("Resettable Objects")]
    public List<GameObject> resettableObjects = new List<GameObject>();

    private Dictionary<GameObject, Quaternion> initialRotations = new();

    private void Awake()
    {
        // Cache initial rotations
        foreach (GameObject obj in resettableObjects)
        {
            if (obj != null && !initialRotations.ContainsKey(obj))
            {
                initialRotations[obj] = obj.transform.rotation;
            }
        }
    }

    public override void OnPuzzleSolved()
    {
        base.OnPuzzleSolved();
        Debug.Log("Laser puzzle completed!");
    }

    public override void ResetPuzzle()
    {
        base.ResetPuzzle();

        if (receiver != null)
            receiver.ResetReceiver();

        foreach (var pair in initialRotations)
        {
            if (pair.Key != null)
            {
                pair.Key.transform.rotation = pair.Value;
            }
        }
    }
}
