using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters.Interactables;

public enum PipeDirection
{
    Up,
    Right,
    Down,
    Left
}

public enum PipeShape
{
    I,
    L,
    T,
    Plus
}

public class PipePiece : Interactable
{
    public PipePuzzleManager puzzleManager;
    [Header("Pipe Settings")]
    public PipeShape shape;
    public bool isSource;
    public bool isSink;

    [Header("Visual Root")]
    [Tooltip("Root object containing all mesh renderers for this pipe")]
    public GameObject visualRoot;

    [Header("Rotation")]
    [Tooltip("Seconds it takes to rotate 90 degrees")]
    public float rotateDuration = 0.25f;

    private int rotationIndex; // tracks current rotation (0-3)
    private bool isRotating;
    private Coroutine rotateRoutine;

    // ----------------------------------
    // PUBLIC ACCESS
    // ----------------------------------
    /// <summary>
    /// Current rotation index (0 = 0°, 1 = 90°, 2 = 180°, 3 = 270°)
    /// </summary>
    public int CurrentRotation => rotationIndex;

    // ---------------- INITIALIZATION ----------------

    public void SetInitialRotation(int rot)
    {
        rotationIndex = rot % 4;

        if (visualRoot != null)
        {
            visualRoot.transform.localRotation = Quaternion.Euler(0f, rotationIndex * 90f, 0f);
        }
    }

    // ---------------- INTERACTION ----------------

    public override void Interact(GameObject player)
    {
        if (!canInteract || isRotating)
            return;
        RotatePipe();
        puzzleManager.RecalculateFlow();
    }

    private void RotatePipe()
    {
        int oldIndex = rotationIndex;
        rotationIndex = (rotationIndex + 1) % 4;

        if (rotateRoutine != null)
            StopCoroutine(rotateRoutine);

        rotateRoutine = StartCoroutine(
            RotateVisualCoroutine(oldIndex, rotationIndex)
        );
    }

    // ---------------- VISUAL ROTATION ----------------

    private IEnumerator RotateVisualCoroutine(int fromIndex, int toIndex)
    {
        if (visualRoot == null)
            yield break;

        isRotating = true;

        Quaternion startRot = Quaternion.Euler(0f, fromIndex * 90f, 0f);
        Quaternion endRot = Quaternion.Euler(0f, toIndex * 90f, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / rotateDuration;
            visualRoot.transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        visualRoot.transform.localRotation = endRot;
        isRotating = false;
    }

    // ---------------- LOGIC ----------------

    /// <summary>
    /// Returns the open directions of this pipe after rotation
    /// </summary>
    public HashSet<PipeDirection> GetOpenDirections()
    {
        HashSet<PipeDirection> dirs = new();

        switch (shape)
        {
            case PipeShape.I:
                dirs.Add(PipeDirection.Up);
                dirs.Add(PipeDirection.Down);
                break;

            case PipeShape.L:
                dirs.Add(PipeDirection.Up);
                dirs.Add(PipeDirection.Right);
                break;

            case PipeShape.T:
                dirs.Add(PipeDirection.Left);
                dirs.Add(PipeDirection.Up);
                dirs.Add(PipeDirection.Right);
                break;

            case PipeShape.Plus:
                dirs.Add(PipeDirection.Up);
                dirs.Add(PipeDirection.Right);
                dirs.Add(PipeDirection.Down);
                dirs.Add(PipeDirection.Left);
                break;
        }

        return RotateDirections(dirs, rotationIndex);
    }

    private HashSet<PipeDirection> RotateDirections(HashSet<PipeDirection> original, int rot)
    {
        HashSet<PipeDirection> rotated = new();
        foreach (var d in original)
            rotated.Add((PipeDirection)(((int)d + rot) % 4));
        return rotated;
    }

    
}

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
