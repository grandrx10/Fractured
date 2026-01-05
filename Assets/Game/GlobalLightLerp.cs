using UnityEngine;

public class GlobalLightLerp : MonoBehaviour
{
    public static GlobalLightLerp Instance;

    private Light targetLight;

    private LightProperties? fromProperties;
    private LightProperties? toProperties;

    private void Awake()
    {
        targetLight = GetComponent<Light>();
        if (Instance != null && Instance != this)
        {
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
        if (!fromProperties.HasValue || !toProperties.HasValue)
            return;

        float t = GlobalWorldManager.Instance.CurvedTransitionTime;

        if (t == -1f)
        {
            ApplyProperties(toProperties.Value, targetLight);
            fromProperties = null;
            toProperties = null;
        }
        else
        {
            LerpProperties(fromProperties.Value, toProperties.Value, targetLight, t);
        }
    }

    public void StartLerpToNewLight(Light newLight)
    {
        if (newLight == null || targetLight == null)
            return;

        // Snapshot CURRENT light (past)
        fromProperties = LightProperties.FromLight(targetLight);

        // Snapshot TARGET light
        toProperties = LightProperties.FromLight(newLight);
    }

    private void LerpProperties(
        LightProperties from,
        LightProperties to,
        Light light,
        float t)
    {
        light.color = Color.Lerp(from.color, to.color, t);
        light.intensity = Mathf.Lerp(from.intensity, to.intensity, t);
        light.range = Mathf.Lerp(from.range, to.range, t);
        light.spotAngle = Mathf.Lerp(from.spotAngle, to.spotAngle, t);
        light.transform.rotation =
            Quaternion.Slerp(from.rotation, to.rotation, t);
    }

    private void ApplyProperties(LightProperties props, Light light)
    {
        light.color = props.color;
        light.intensity = props.intensity;
        light.range = props.range;
        light.spotAngle = props.spotAngle;
        light.transform.rotation = props.rotation;
    }

    private struct LightProperties
    {
        public Color color;
        public float intensity;
        public float range;
        public float spotAngle;
        public Quaternion rotation;

        public static LightProperties FromLight(Light light)
        {
            return new LightProperties
            {
                color = light.color,
                intensity = light.intensity,
                range = light.range,
                spotAngle = light.spotAngle,
                rotation = light.transform.rotation
            };
        }
    }
}
