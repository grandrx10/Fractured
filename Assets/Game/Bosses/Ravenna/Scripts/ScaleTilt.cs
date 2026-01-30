using UnityEngine;

public class ScaleTilt : MonoBehaviour
{
    [Header("Direction Targets")]
    public Transform from;
    public Transform to;

    [Header("Tilt Settings")]
    public float smoothSpeed = 5f;

    private Quaternion initialRotation;

    void Start()
    {
        if (!from || !to)
        {
            Debug.LogError("ScaleTilt: Missing from/to transform.");
            enabled = false;
            return;
        }

        initialRotation = transform.rotation;
    }

    void Update()
    {
        Vector3 dir = (to.position - from.position).normalized;
        if (dir.sqrMagnitude < 0.0001f)
            return;

        Quaternion look = Quaternion.LookRotation(dir, Vector3.up);


        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            look,
            Time.deltaTime * smoothSpeed
        );
    }
}