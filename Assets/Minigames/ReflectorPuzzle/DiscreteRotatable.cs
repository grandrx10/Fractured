using UnityEngine;
using Characters.Interactables;
using System.Collections;

public class DiscreteRotatable : Interactable
{
    [Header("Rotation")]
    public Vector3 rotationAxis = Vector3.up;
    public float rotationStepDegrees = 15f;
    public float rotationDuration = 0.25f;

    private bool isRotating = false;

    public override void Interact(GameObject player)
    {
        if (!canInteract || isRotating)
            return;

        StartCoroutine(RotateSmooth());
    }

    private IEnumerator RotateSmooth()
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation =
            Quaternion.AngleAxis(rotationStepDegrees, rotationAxis.normalized) * startRotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / rotationDuration;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }
}
