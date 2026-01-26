using Cards.Environments;
using UnityEngine;
using Characters;

public class MirrorPuzzle : MonoBehaviour
{
    [Header("Mirror Setup")]
    [Tooltip("Transform representing the mirror plane (its forward is the mirror normal)")]
    public Transform mirrorPlane;

    [Tooltip("Object that mirrors the player")]
    public Transform mirrorObject;

    private Transform player;
    private Rigidbody mirrorRigidbody;
    private bool playerInside = false;

    private void Start()
    {
        player = OpenWorldEnv.Current.PlayerTransform;

        if (mirrorObject != null)
        {
            mirrorRigidbody = mirrorObject.GetComponent<Rigidbody>();
            if (mirrorRigidbody == null)
            {
                Debug.LogWarning("MirrorPuzzle: mirrorObject should have a Rigidbody component!");
            }
        }
    }

    private void FixedUpdate()
    {
        if (!playerInside || player == null)
            return;

        MirrorTransform();
    }

    private void MirrorTransform()
    {
        // --- Position ---
        Vector3 localPos = mirrorPlane.InverseTransformPoint(player.position);
        localPos.z *= -1f;
        Vector3 targetPosition = mirrorPlane.TransformPoint(localPos);

        // --- Rotation ---
        Vector3 localForward = mirrorPlane.InverseTransformDirection(player.forward);
        Vector3 localUp = mirrorPlane.InverseTransformDirection(player.up);

        localForward.z *= -1f;
        localUp.z *= -1f;

        Quaternion targetRotation = Quaternion.LookRotation(
            mirrorPlane.TransformDirection(localForward),
            mirrorPlane.TransformDirection(localUp)
        );

        // Use physics-based movement if Rigidbody exists
        if (mirrorRigidbody != null)
        {
            mirrorRigidbody.MovePosition(targetPosition);
            mirrorRigidbody.MoveRotation(targetRotation);
        }
        else
        {
            mirrorObject.position = targetPosition;
            mirrorObject.rotation = targetRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player != null && (other.transform == player || other.transform.IsChildOf(player)))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (player != null && (other.transform == player || other.transform.IsChildOf(player)))
        {
            playerInside = false;
        }
    }
}