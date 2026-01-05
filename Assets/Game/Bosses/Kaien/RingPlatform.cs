using UnityEngine;

public class PlatformRingFilter : MonoBehaviour
{
    [Header("Ring Settings")]
    public float outerRadius = 8f;
    public float innerHoleRadius = 3f;

    public void ApplyRingMask()
    {
        Vector3 center = transform.position;

        foreach (Transform child in transform)
        {
            Platform platform = child.GetComponent<Platform>();
            if (platform == null)
                continue;

            Vector3 offset = child.position - center;
            offset.y = 0f;

            float distance = offset.magnitude;

            if (distance <= outerRadius && distance >= innerHoleRadius)
                continue;

            platform.ForcePermanentFall();
        }
    }
}
