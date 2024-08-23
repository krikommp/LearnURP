using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AlphaMaskRenderFeature : ScriptableRendererFeature
{
    class AlphaMaskRenderPass : ScriptableRenderPass
    {
        private RTHandle m_temporaryAlphaMaskTexture;
        private ProfilingSampler m_profilingSample = new ProfilingSampler("AlphaMask");
        private FilteringSettings m_filteringSettings;
        private RenderStateBlock m_renderStateBlock;
        private RenderQueueType m_renderQueueType;
        private static readonly string kAlphaMaskTextureName = "_AlphaMask";
        private static readonly ShaderTagId kAlphaMaskShaderTagId = new ShaderTagId("AlphaMask");

        public AlphaMaskRenderPass(RenderQueueType renderQueueType, int layerMask)
        {
            RenderQueueRange renderQueueRange = (renderQueueType == RenderQueueType.Transparent)
                ? RenderQueueRange.transparent
                : RenderQueueRange.opaque;
            m_filteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            m_renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            m_renderQueueType = renderQueueType;
        }

        public void Setup(CameraData cameraData)
        {
            RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
            desc.graphicsFormat = GraphicsFormat.R32G32_SFloat;
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;
            RenderingUtils.ReAllocateIfNeeded(ref m_temporaryAlphaMaskTexture, desc, name: kAlphaMaskTextureName);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(m_temporaryAlphaMaskTexture);
            ConfigureClear(ClearFlag.Color, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SortingCriteria sortingCriteria = (m_renderQueueType == RenderQueueType.Transparent)
                ? SortingCriteria.CommonTransparent
                : renderingData.cameraData.defaultOpaqueSortFlags;

            DrawingSettings drawingSettings =
                CreateDrawingSettings(kAlphaMaskShaderTagId, ref renderingData, sortingCriteria);


            var cmd = CommandBufferPool.Get();
            
            using (new ProfilingScope(cmd, m_profilingSample))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();   
             
                cmd.SetGlobalTexture(m_temporaryAlphaMaskTexture.name, m_temporaryAlphaMaskTexture.nameID);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();   
                
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_filteringSettings, ref m_renderStateBlock);
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public void Dispose()
        {
            m_temporaryAlphaMaskTexture?.Release();
        }
    }
    
    /// <summary>
    /// The filter settings used.
    /// </summary>
    [System.Serializable]
    public class FilterSettings
    {
        // TODO: expose opaque, transparent, all ranges as drop down

        /// <summary>
        /// The queue type for the objects to render.
        /// </summary>
        public RenderQueueType RenderQueueType;

        /// <summary>
        /// The layer mask to use.
        /// </summary>
        public LayerMask LayerMask;

        /// <summary>
        /// The constructor for the filter settings.
        /// </summary>
        public FilterSettings()
        {
            RenderQueueType = RenderQueueType.Opaque;
            LayerMask = 0;
        }
    }

    AlphaMaskRenderPass m_ScriptablePass;
    public FilterSettings m_filterSettings = new();

    public override void Create()
    {
        FilterSettings filter = m_filterSettings;
        
        m_ScriptablePass = new AlphaMaskRenderPass(filter.RenderQueueType, filter.LayerMask);
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        m_ScriptablePass.Setup(renderingData.cameraData);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing)
    {
        m_ScriptablePass?.Dispose();
    }
}


