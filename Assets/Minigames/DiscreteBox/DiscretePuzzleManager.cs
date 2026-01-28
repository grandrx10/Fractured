using System.Collections.Generic;
using Cards.Environments;
using Minigames;
using UnityEngine;

public class DiscretePuzzleManager : PuzzleManager
{
    [Header("Win Detectors")]
    public List<DiscreteBoxDetector> detectors = new();

    [Header("Resettable Objects")]
    public List<GameObject> resettableObjects = new();

    private Dictionary<GameObject, Pose> initialPoses = new();
    private bool initialized = false;
    
    private void Awake()
    {
        GlobalWorldManager.OnPreLoadNewScene += Init;
    }
    
    private void Init(CardEnv env)
    {
        GlobalWorldManager.OnPreLoadNewScene -= Init;
        // Cache initial poses
        foreach (GameObject obj in resettableObjects)
        {
            if (obj != null && !initialPoses.ContainsKey(obj))
            {
                initialPoses[obj] = new Pose(
                    obj.transform.position,
                    obj.transform.rotation
                );
            }
        }

        initialized = true;
    }

    private void Update()
    {
        if (isSolved || detectors.Count == 0)
            return;

        for (int i = 0; i < detectors.Count; i++)
        {
            if (!detectors[i].isActive)
                return;
        }

        OnPuzzleSolved();
    }
    
    
[ContextMenu("Solve")]
    public override void OnPuzzleSolved()
    {
        base.OnPuzzleSolved();

        Debug.Log($"{gameObject.name} discrete puzzle completed.");

        // Puzzle-specific success logic here
        // e.g. open door, play sound, trigger dialogue
    }

    public override void ResetPuzzle()
    {
        base.ResetPuzzle();

        if (!initialized)
            return;

        foreach (var pair in initialPoses)
        {
            if (pair.Key == null)
            {
                Debug.Log("???");
                continue;
            }
            Debug.Log(pair.Key);
            Debug.Log(pair.Value.position);
            
            if (pair.Key.TryGetComponent<IPuzzleResettable>(out IPuzzleResettable pr))
            {
                pr.Reset(pair.Value);
            }
            if (pair.Key.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.position = pair.Value.position;
                rb.rotation = pair.Value.rotation;
            }
            else
            {
                pair.Key.transform.SetPositionAndRotation(
                    pair.Value.position,
                    pair.Value.rotation
                );
            }
        }
    }
}
