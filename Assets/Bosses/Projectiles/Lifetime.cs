using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f; // Time in seconds before destruction

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
