using UnityEngine;

public class MirroredObj : MonoBehaviour
{
    [Header("Mirror Setup")]
    [Tooltip("Transform representing the mirror plane (its forward is the mirror normal)")]
    public Transform mirrorPlane;

    [Tooltip("Object that will mirror this object")]
    public Transform mirrorObject;

    private void Update()
    {
        if (mirrorPlane == null || mirrorObject == null)
            return;

        MirrorTransform();
    }

    private void MirrorTransform()
    {
        // --- Position ---
        Vector3 localPos = mirrorPlane.InverseTransformPoint(transform.position);
        localPos.z *= -1f;
        mirrorObject.position = mirrorPlane.TransformPoint(localPos);

        // --- Rotation ---
        Vector3 localForward = mirrorPlane.InverseTransformDirection(transform.forward);
        Vector3 localUp = mirrorPlane.InverseTransformDirection(transform.up);

        localForward.z *= -1f;
        localUp.z *= -1f;

        mirrorObject.rotation = Quaternion.LookRotation(
            mirrorPlane.TransformDirection(localForward),
            mirrorPlane.TransformDirection(localUp)
        );
    }
}