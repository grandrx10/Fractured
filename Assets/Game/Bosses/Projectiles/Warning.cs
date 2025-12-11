using System.Collections;
using UnityEngine;

namespace Game.Bosses.Projectiles
{
    public class Warning : MonoBehaviour
    {
        public enum WarningType
        {
            Grounded,
            Floating
        }

        [Header("Settings")]
        [SerializeField] private LayerMask groundMask;

        private Renderer[] renderers;
        private float radius = 1f;
        private float duration = 1f;
        private WarningType type = WarningType.Grounded;
        public float fadeTime = 0.3f; // duration of fade in/out

        private void Awake()
        {
            // Get all renderers and hide initially
            renderers = GetComponentsInChildren<Renderer>();
            SetRenderersAlpha(0f);
        }

        /// <summary>
        /// Initialize the warning
        /// </summary>
        public void Initialize(float radius, float duration, WarningType type, float fadeTime)
        {
            this.radius = radius;
            this.duration = duration;
            this.type = type;
            this.fadeTime = fadeTime;

            ApplySettings();
            StartCoroutine(FadeRoutine());
        }

        private void ApplySettings()
        {
            // Scale
            transform.localScale = new Vector3(radius, 1f, radius);

            // Ground placement
            if (type == WarningType.Grounded)
            {
                Ray ray = new Ray(transform.position + Vector3.up * 5f, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, 50f, groundMask))
                {
                    transform.position = hit.point;
                }
            }
        }

        private IEnumerator FadeRoutine()
        {
            // Fade in
            yield return Fade(0f, 1f, fadeTime);

            // Wait for duration minus fade times
            yield return new WaitForSeconds(Mathf.Max(0f, duration - fadeTime * 2f));

            // Fade out
            yield return Fade(1f, 0f, fadeTime);

            // Destroy self
            Destroy(gameObject);
        }

        private IEnumerator Fade(float from, float to, float time)
        {
            float elapsed = 0f;
            while (elapsed < time)
            {
                float alpha = Mathf.Lerp(from, to, elapsed / time);
                SetRenderersAlpha(alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
            SetRenderersAlpha(to);
        }

        private void SetRenderersAlpha(float alpha)
        {
            if (renderers == null) return;

            foreach (var r in renderers)
            {
                foreach (var mat in r.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        Color c = mat.color;
                        c.a = alpha;
                        mat.color = c;
                    }
                }
            }
        }
    }
}
