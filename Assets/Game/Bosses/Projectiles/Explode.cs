using UnityEngine;

namespace Game.Bosses.Projectiles
{
    public class Explode : MonoBehaviour
    {
        [Header("Explosion Prefab")]
        public GameObject explosionPrefab;

        [Header("Explosion Settings")]
        public bool explodeOnTouch = false; // If true → explode on trigger OR collision

        [Header("Audio")]
        public AudioClip explosionSound;   // assign in inspector or leave empty
        [Range(0f, 10f)]
        public float explosionVolume = 1f;

        // Public call to explode manually
        public void ExplodeNow()
        {

            // Instantiate the explosion prefab
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // Play the explosion sound
            if (explosionSound != null && AudioManager.Instance != null)
            {
                Debug.Log("Here.");
                AudioManager.Instance.PlayOneShot(
                    explosionSound,
                    transform.position,
                    explosionVolume
                );
            }

            // Destroy this projectile
            Destroy(gameObject);
        }

        private bool IsValidLayer(GameObject obj)
        {
            int layer = obj.layer;
            return layer == LayerMask.NameToLayer("Player") ||
                   layer == LayerMask.NameToLayer("Ground");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!explodeOnTouch)
                return;

            if (!IsValidLayer(other.gameObject))
                return;

            explodeOnTouch = false;
            ExplodeNow();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!explodeOnTouch)
                return;

            if (!IsValidLayer(collision.gameObject))
                return;

            explodeOnTouch = false;
            ExplodeNow();
        }
    }
}
