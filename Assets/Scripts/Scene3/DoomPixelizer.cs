using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class DoomPixelizer : ScriptableRendererFeature
{
    private class DoomPixelizerPass : ScriptableRenderPass
    {
        private class PassData
        {
            public TextureHandle src;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            if (cameraData.cameraType == CameraType.SceneView) return; // No pixelar el Scene View

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            if (!resourceData.activeColorTexture.IsValid()) return;

            TextureHandle cameraColor = resourceData.activeColorTexture;

            // 1. Create 320x240 Low-Res Texture Description
            TextureDesc lowResDesc = renderGraph.GetTextureDesc(cameraColor);
            lowResDesc.width = 320;
            lowResDesc.height = 240;
            lowResDesc.msaaSamples = MSAASamples.None; // No MSAA for the low-res buffer
            lowResDesc.filterMode = FilterMode.Point;
            lowResDesc.name = "DoomLowResBuffer";
            lowResDesc.clearBuffer = false;

            TextureHandle lowResRT = renderGraph.CreateTexture(lowResDesc);

            // 2. Downsample Pass (CameraColor -> LowRes)
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("DoomDownsample", out var passData))
            {
                passData.src = cameraColor;
                builder.UseTexture(passData.src, AccessFlags.Read);
                builder.SetRenderAttachment(lowResRT, 0, AccessFlags.Write);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    // Blit Native to 320x240
                    Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), 0, false);
                });
            }

            // 3. Upsample Pass (LowRes -> CameraColor)
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("DoomUpsample", out var passData))
            {
                passData.src = lowResRT;
                builder.UseTexture(passData.src, AccessFlags.Read);
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    // Blit 320x240 back to Native. 
                    // Point filtering is inherited from lowResRT descriptor.
                    Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), 0, false);
                });
            }
        }
    }

    private DoomPixelizerPass m_Pass;

    public override void Create()
    {
        m_Pass = new DoomPixelizerPass();
        m_Pass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_Pass);
    }
}
