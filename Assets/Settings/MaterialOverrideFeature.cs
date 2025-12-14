using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MaterialOverrideFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material overrideMaterial;
        public LayerMask layerMask = -1;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public string cameraTag = "MaterialOverrideCamera";
    }

    public Settings settings = new();
    MaterialOverridePass pass;

    public override void Create()
    {
        pass = new MaterialOverridePass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Only run on a specific camera
        if (!renderingData.cameraData.camera.CompareTag(settings.cameraTag))
            return;
        renderer.EnqueuePass(pass);
    }

    class MaterialOverridePass : ScriptableRenderPass
    {
        Settings settings;
        FilteringSettings filtering;
        ShaderTagId shaderTag = new ShaderTagId("UniversalForward");

        public MaterialOverridePass(Settings settings)
        {
            this.settings = settings;
            filtering = new FilteringSettings(RenderQueueRange.all, settings.layerMask);
            renderPassEvent = settings.renderPassEvent;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (settings.overrideMaterial == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Material Override Pass");

            using (new ProfilingScope(cmd, new ProfilingSampler("Material Override")))
            {
                // CLEAR FIRST
                cmd.ClearRenderTarget(true, true, Color.clear);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var drawingSettings = CreateDrawingSettings(
                    shaderTag,
                    ref renderingData,
                    SortingCriteria.CommonOpaque
                );

                drawingSettings.overrideMaterial = settings.overrideMaterial;

                context.DrawRenderers(
                    renderingData.cullResults,
                    ref drawingSettings,
                    ref filtering
                );
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
