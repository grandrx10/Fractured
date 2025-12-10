using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Reflection;

[InitializeOnLoad]
public static class DisableURPFeatureOnExit
{
    static DisableURPFeatureOnExit()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            DisableFeature(false);
        }
        else if (state == PlayModeStateChange.EnteredPlayMode)
        {
            DisableFeature(true);
        }
            
    }

    private static void DisableFeature(bool active)
    {
        var pipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (!pipeline) return;

        // Access the internal renderer list
        var listField = typeof(UniversalRenderPipelineAsset)
            .GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);

        var rendererDatas = listField?.GetValue(pipeline) as ScriptableRendererData[];
        if (rendererDatas == null) return;

        foreach (var data in rendererDatas)
        {
            if (!data) continue;

            var featuresField = typeof(ScriptableRendererData)
                .GetField("m_RendererFeatures", BindingFlags.NonPublic | BindingFlags.Instance);

            var features = featuresField.GetValue(data) as 
                System.Collections.Generic.List<ScriptableRendererFeature>;

            foreach (var f in features)
            {
                if (f is FullScreenPassRendererFeature)
                {
                    f.SetActive(active);
                    EditorUtility.SetDirty(data);
                }
            }
        }
    }
}