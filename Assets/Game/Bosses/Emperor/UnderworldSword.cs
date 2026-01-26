using UnityEngine;

namespace Game.Bosses
{
    public class UnderworldSword : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float minWaveHeight = 0.5f;
        public float maxWaveHeight = 2f;
        public float minWaveSpeed = 1f;
        public float maxWaveSpeed = 3f;
        public float forwardSpeed = 5f;

        [Header("Sword Spawning")]
        public GameObject swordPrefab;
        public float swordSpawnInterval = 0.5f;
        public float swordRiseHeight = 3f;
        public float swordRiseSpeed = 2f;
        public float swordLifetime = 2f;

        private float waveHeight;
        private float waveSpeed;
        private float timePassed = 0f;
        private float swordSpawnTimer = 0f;
        private Vector3 startPosition;
        private bool isReversing = false;
        private float maxTimeReached = 0f;

        private Vector3 startForward;
        private Vector3 startRight;

        private void Start()
        {
            startPosition = transform.position;
            startForward = transform.forward;
            startRight = transform.right;
            waveHeight = Random.Range(minWaveHeight, maxWaveHeight);
            waveSpeed = Random.Range(minWaveSpeed, maxWaveSpeed);
        }

        private void Update()
        {
            if (!isReversing)
            {
                timePassed += Time.deltaTime;
                maxTimeReached = timePassed;
            }
            else
            {
                timePassed -= Time.deltaTime;
                if (timePassed <= 0f)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            // Calculate position using sin wave on XZ plane
            float distance = timePassed * forwardSpeed;
            float sidewaysOffset = Mathf.Sin(timePassed * waveSpeed) * waveHeight;

            transform.position = startPosition + startForward * distance + startRight * sidewaysOffset;

            // Spawn swords at intervals (both forward and reverse)
            swordSpawnTimer += Time.deltaTime;
            if (swordSpawnTimer >= swordSpawnInterval)
            {
                SpawnSword();
                swordSpawnTimer = 0f;
            }
        }

        private void SpawnSword()
        {
            if (swordPrefab == null) return;

            // Spawn at current position but at ground level (following the sin wave curve)
            Vector3 groundPos = new Vector3(transform.position.x, startPosition.y, transform.position.z);
            
            // Calculate tangent direction using derivative of sin wave
            // Position = forward * (t * forwardSpeed) + right * (sin(t * waveSpeed) * waveHeight)
            // Derivative = forward * forwardSpeed + right * (cos(t * waveSpeed) * waveSpeed * waveHeight)
            float cosComponent = Mathf.Cos(timePassed * waveSpeed) * waveSpeed * waveHeight;
            Vector3 tangent = startForward * forwardSpeed + startRight * cosComponent;
            tangent.y = 0; // Keep on XZ plane
            tangent.Normalize();
            
            Quaternion swordRotation = Quaternion.LookRotation(tangent);
            GameObject sword = Instantiate(swordPrefab, groundPos, swordRotation);
            
            SwordInstance swordInstance = sword.AddComponent<SwordInstance>();
            swordInstance.Initialize(swordRiseHeight, swordRiseSpeed, swordLifetime);
        }

        public void Reverse()
        {
            isReversing = true;
            timePassed = maxTimeReached;
        }

        private class SwordInstance : MonoBehaviour
        {
            private float riseHeight;
            private float riseSpeed;
            private float lifetime;
            private Vector3 startPos;
            private float elapsedTime = 0f;
            private bool rising = true;

            public void Initialize(float height, float speed, float life)
            {
                riseHeight = height;
                riseSpeed = speed;
                lifetime = life;
                startPos = transform.position;
            }

            private void Update()
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= lifetime)
                {
                    Destroy(gameObject);
                    return;
                }

                // Rise up and down animation
                float progress = (elapsedTime * riseSpeed) % (Mathf.PI * 2);
                float yOffset = Mathf.Sin(progress) * riseHeight;
                if (yOffset < 0) yOffset = 0;

                transform.position = startPos + Vector3.up * yOffset;
            }
        }
    }
}