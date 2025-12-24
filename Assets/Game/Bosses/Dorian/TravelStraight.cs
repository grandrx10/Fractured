using UnityEngine;

public class TravelStraight : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f; // units per second

    void Update()
    {
        // Move forward in local Z direction
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
