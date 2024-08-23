using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostRTRenderFeature: ScriptableRendererFeature
{
    [Serializable]
    public class PostSetting
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public string grabbedTextureName = "_GrabbedTexture";
    }

    class PostRTRenderPass : ScriptableRenderPass
    {
        private PostSetting postSetting;
        private RTHandle _grabbedTextureHandle;
        private int _grabbedTexturePropertyId;

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
        }

        public PostRTRenderPass(PostSetting postSetting)
        {
            renderPassEvent = postSetting.renderPassEvent;
            this.postSetting = postSetting;
            _grabbedTexturePropertyId = Shader.PropertyToID(postSetting.grabbedTextureName);
            _grabbedTextureHandle = RTHandles.Alloc(postSetting.grabbedTextureName, postSetting.grabbedTextureName);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(_grabbedTexturePropertyId, cameraTextureDescriptor);
            cmd.SetGlobalTexture(postSetting.grabbedTextureName, _grabbedTextureHandle.nameID);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get(nameof(PostRTRenderPass));
            cmd.Clear();
            var renderer = renderingData.cameraData.renderer;
            if (renderer.cameraColorTargetHandle.rt != null)
            {
                Blit(cmd, renderer.cameraColorTargetHandle, _grabbedTextureHandle);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_grabbedTexturePropertyId);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

    }

    PostRTRenderPass m_ScriptablePass;
    public PostSetting postSetting = new PostSetting();

    public override void Create()
    {
        m_ScriptablePass = new PostRTRenderPass(postSetting);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}