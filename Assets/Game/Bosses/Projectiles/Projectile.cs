using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{gameObject.name} hit the Player!");
        }
    }
}
