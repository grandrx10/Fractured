using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class URPRendererFeatureUtility
{
    public static T GetRendererFeature<T>(Camera cam) where T : ScriptableRendererFeature
    {
        if (!cam.TryGetComponent<UniversalAdditionalCameraData>(out var camData))
        {
            Debug.LogError("Camera missing UniversalAdditionalCameraData.");
            return null;
        }

        var renderer = camData.scriptableRenderer;
        if (renderer == null)
        {
            Debug.LogError("Camera has no ScriptableRenderer.");
            return null;
        }

        // Get the protected field "m_RendererFeatures"
        var flags = BindingFlags.NonPublic | BindingFlags.Instance;
        FieldInfo featureListField = typeof(ScriptableRenderer).GetField("m_RendererFeatures", flags);

        if (featureListField == null)
        {
            Debug.LogError("Cannot locate m_RendererFeatures via reflection. URP version changed?");
            return null;
        }

        var featureList = featureListField.GetValue(renderer) as System.Collections.Generic.List<ScriptableRendererFeature>;
        if (featureList == null)
        {
            Debug.LogError("Renderer feature list is null.");
            return null;
        }

        foreach (var f in featureList)
        {
            if (f is T typed)
                return typed;
        }

        return null;
    }

    public static bool SetFeatureEnabled<T>(Camera cam, bool enabled) where T : ScriptableRendererFeature
    {
        var feature = GetRendererFeature<T>(cam);
        if (feature == null)
            return false;

        feature.SetActive(enabled);
        return true;
    }
}