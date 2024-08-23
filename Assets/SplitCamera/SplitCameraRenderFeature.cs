using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SplitCameraRenderFeature : ScriptableRendererFeature
{
    class SplitCameraRenderPass : ScriptableRenderPass
    {
        private ProfilingSampler m_profilingSampler = new ProfilingSampler("SplitCamera");
        public int m_stencilRef = 1;
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, m_profilingSampler))
            {
                cmd.SetGlobalInt("_StencilRef", m_stencilRef);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();   
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            CommandBufferPool.Release(cmd);
        }
    }
    
    public RenderPassEvent renderPassEvent;
    public int m_stencilRef = 1;
    private SplitCameraRenderPass m_SplitCameraRenderPass;

    public override void Create()
    {
        m_SplitCameraRenderPass = new SplitCameraRenderPass();
        m_SplitCameraRenderPass.renderPassEvent = renderPassEvent;
        m_SplitCameraRenderPass.m_stencilRef = m_stencilRef;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_SplitCameraRenderPass);
    }
}
