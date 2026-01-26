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

        // NEW (optional)
        public enum WarningShape
        {
            Circle,
            Rectangle
        }

        [Header("Settings")]
        [SerializeField] private LayerMask groundMask;

        // NEW (defaults preserve old behavior)
        [Header("Shape")]
        [SerializeField] private WarningShape shape = WarningShape.Circle;

        // OLD (still used for circles)
        [SerializeField] private float radius = 1f;

        // NEW (only used if Rectangle)
        [SerializeField] private Vector2 rectangleSize = Vector2.one;

        private Renderer[] renderers;
        private float duration = 1f;
        private WarningType type = WarningType.Grounded;
        public float fadeTime = 0.3f;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            SetRenderersAlpha(0f);
        }

        /// <summary>
        /// ORIGINAL INITIALIZER (unchanged)
        /// </summary>
        public void Initialize(float radius, float duration, WarningType type, float fadeTime)
        {
            this.radius = radius;
            this.duration = duration;
            this.type = type;
            this.fadeTime = fadeTime;

            shape = WarningShape.Circle; // ensure legacy calls behave correctly

            ApplySettings();
            StartCoroutine(FadeRoutine());
        }

        /// <summary>
        /// NEW RECTANGLE INITIALIZER (optional)
        /// </summary>
        public void InitializeRectangle(
            Vector2 size,
            float duration,
            WarningType type,
            float fadeTime)
        {
            shape = WarningShape.Rectangle;
            rectangleSize = size;
            this.duration = duration;
            this.type = type;
            this.fadeTime = fadeTime;

            ApplySettings();
            StartCoroutine(FadeRoutine());
        }

        private void ApplySettings()
        {
            // =========================
            // SCALE
            // =========================
            if (shape == WarningShape.Rectangle)
            {
                transform.localScale = new Vector3(rectangleSize.x, 1f, rectangleSize.y);
            }
            else
            {
                // DEFAULT / LEGACY CIRCLE
                transform.localScale = new Vector3(radius, 1f, radius);
            }

            // =========================
            // GROUND PLACEMENT
            // =========================
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

            // Hold
            yield return new WaitForSeconds(Mathf.Max(0f, duration - fadeTime * 2f));

            // Fade out
            yield return Fade(1f, 0f, fadeTime);

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
            if (renderers == null)
                return;

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
