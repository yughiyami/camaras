using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class DoomColorLimiterFeature : ScriptableRendererFeature
{
    private class DoomColorLimiterPass : ScriptableRenderPass
    {
        public Material material;

        private class PassData
        {
            public Material material;
            public TextureHandle src;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (material == null) return;

            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            if (cameraData.cameraType == CameraType.SceneView) return; // No aplicar al Scene View

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            if (!resourceData.activeColorTexture.IsValid()) return;

            TextureHandle cameraColor = resourceData.activeColorTexture;
            
            // 1. Crear textura temporal
            TextureDesc desc = renderGraph.GetTextureDesc(cameraColor);
            desc.name = "DoomColorLimiterTemp";
            desc.clearBuffer = false;
            TextureHandle tempTexture = renderGraph.CreateTexture(desc);

            // 2. Primer pase: SceneColor -> Temp (Procesando con el Material)
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("DoomColorLimiter_Step1", out var passData))
            {
                passData.material = material;
                passData.src = cameraColor;

                builder.UseTexture(passData.src, AccessFlags.Read);
                builder.SetRenderAttachment(tempTexture, 0, AccessFlags.Write);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }

            // 3. Segundo pase: Temp -> SceneColor (Blit de regreso)
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("DoomColorLimiter_Step2", out var passData))
            {
                passData.src = tempTexture;

                builder.UseTexture(passData.src, AccessFlags.Read);
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), 0, false);
                });
            }
        }
    }

    public Material material;
    private DoomColorLimiterPass m_Pass;

    public override void Create()
    {
        m_Pass = new DoomColorLimiterPass();
        m_Pass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_Pass.material = material;
        renderer.EnqueuePass(m_Pass);
    }
}
