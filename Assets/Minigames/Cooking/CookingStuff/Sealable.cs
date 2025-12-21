using UnityEngine;

namespace Minigames.Cooking.CookingStuff
{
    public class Sealable : MonoBehaviour
    {
        [Header("Sealing Settings")]
        public float timeToSeal = 5f;  // Time after spawn to seal before exploding
        [HideInInspector] public bool isSealed = false;

        private float timer = 0f;

        private void Awake()
        {
            timer = 0f;
        }

        private void Update()
        {
            if (isSealed) return;

            timer += Time.deltaTime;
            if (timer >= timeToSeal)
            {
                Explode();
            }
        }

        private void Explode()
        {
            if (ExplodeManager.Instance != null)
            {
                ExplodeManager.Instance.Explode(transform);
            }
            Destroy(gameObject);
        }
    }
}
