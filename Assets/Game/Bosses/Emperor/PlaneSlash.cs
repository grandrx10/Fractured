using UnityEngine;

namespace Game.Bosses.Projectiles
{
    public class PlaneSlash : MonoBehaviour
    {
        [Header("Optional Settings")]
        public float lifetime = 2f;      // auto destroy after this time
        public Vector3 moveDirection = Vector3.zero; // if you want it to move

        private void Start()
        {
            if (lifetime > 0f)
                Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (moveDirection != Vector3.zero)
            {
                transform.position += moveDirection * Time.deltaTime;
            }
        }

        // Could add collision detection for player damage
        private void OnTriggerEnter(Collider other)
        {
            // Example: if hit player, deal damage
            if (other.CompareTag("Player"))
            {
                // Implement damage logic here
                Debug.Log("Player hit by plane slash!");
            }
        }
    }
}
