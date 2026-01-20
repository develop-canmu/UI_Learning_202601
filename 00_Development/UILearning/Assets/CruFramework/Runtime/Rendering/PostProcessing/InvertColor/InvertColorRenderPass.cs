#if CRUFRAMEWORK_URP_SUPPORT

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CruFramework.Rendering.PostProcessing
{
    public sealed class InvertColorRenderPass : ScriptableRenderPass, IDisposable
    {
        private static readonly int tempId = Shader.PropertyToID("InvertColorRT");
        private Material material = null;
        private RenderTargetIdentifier target;

        public InvertColorRenderPass(Shader shader, RenderPassEvent evt)
        {
            profilingSampler = new ProfilingSampler("InvertColor Render Pass");
            renderPassEvent = evt;
            material = CoreUtils.CreateEngineMaterial(shader);
        }

        public void Setup(RenderTargetIdentifier target)
        {
            this.target = target;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!renderingData.cameraData.postProcessEnabled) return;
            if (material == null) return;

            VolumeStack stack = VolumeManager.instance.stack;
            InvertColor invertColor = stack.GetComponent<InvertColor>();

            if (!invertColor.active) return;
            if (!invertColor.IsActive()) return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilingSampler))
            {
                // TODO SwapBufferへの書き換え

                ref CameraData cameraData = ref renderingData.cameraData;

                material.SetFloat("_Weight", invertColor.Weight.value);

                int width = cameraData.cameraTargetDescriptor.width;
                int height = cameraData.cameraTargetDescriptor.height;

                cmd.GetTemporaryRT(tempId, width, height, 0, FilterMode.Point, RenderTextureFormat.Default);
                cmd.SetGlobalTexture("_MainTex", tempId);
                cmd.Blit(target, tempId, material, 0);
                cmd.Blit(tempId, target);
                cmd.ReleaseTemporaryRT(tempId);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            CoreUtils.Destroy(material);
        }
    }
}

#endif