using UnityEngine;

public class GlobalLightLerp : MonoBehaviour
{
    public static GlobalLightLerp Instance;

    [Header("This light's properties")]
    public Light targetLight;

    private LightProperties? incomingProperties;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Let the existing instance handle the new light
            Instance.StartLerpToNewLight(this.targetLight);

            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    private void Update()
    {
        if (incomingProperties.HasValue)
        {
            float t = GlobalWorldManager.Instance.TransitionTime;

            if (t == -1f)
            {
                // Instantly copy properties
                CopyProperties(incomingProperties.Value, targetLight);
                incomingProperties = null;
            }
            else
            {
                // Lerp properties smoothly
                LerpProperties(incomingProperties.Value, targetLight, t);
            }
        }
    }

    public void StartLerpToNewLight(Light newLight)
    {
        if (newLight == null) return;

        // Save the light's properties
        incomingProperties = new LightProperties
        {
            color = newLight.color,
            intensity = newLight.intensity,
            range = newLight.range,
            spotAngle = newLight.spotAngle
        };
    }

    private void LerpProperties(LightProperties from, Light to, float t)
    {
        to.color = Color.Lerp(to.color, from.color, t);
        to.intensity = Mathf.Lerp(to.intensity, from.intensity, t);
        to.range = Mathf.Lerp(to.range, from.range, t);
        to.spotAngle = Mathf.Lerp(to.spotAngle, from.spotAngle, t);
    }

    private void CopyProperties(LightProperties from, Light to)
    {
        to.color = from.color;
        to.intensity = from.intensity;
        to.range = from.range;
        to.spotAngle = from.spotAngle;
    }

    // Struct to store light properties
    private struct LightProperties
    {
        public Color color;
        public float intensity;
        public float range;
        public float spotAngle;
    }
}
