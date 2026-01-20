#if CRUFRAMEWORK_URP_SUPPORT

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CruFramework.Rendering.PostProcessing
{
    public sealed class FastBlurRenderPass : ScriptableRenderPass, IDisposable
    {
        private static readonly int tempId = Shader.PropertyToID("FastBlurRT");
        private Material material = null;
        private RenderTargetIdentifier target;

        public FastBlurRenderPass(Shader shader, RenderPassEvent evt)
        {
            profilingSampler = new ProfilingSampler("FastBlur Render Pass");
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
            FastBlur fastBlur = stack.GetComponent<FastBlur>();

            if (!fastBlur.active) return;
            if (!fastBlur.IsActive()) return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilingSampler))
            {
                // TODO SwapBufferへの書き換え

                ref CameraData cameraData = ref renderingData.cameraData;

                material.SetFloat("_Strength", fastBlur.Strength.value);

                int width = cameraData.cameraTargetDescriptor.width >> fastBlur.Downsample.value;
                int height = cameraData.cameraTargetDescriptor.height >> fastBlur.Downsample.value;

                cmd.GetTemporaryRT(tempId, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
                cmd.SetGlobalTexture("_MainTex", tempId);
                cmd.Blit(target, tempId, material, 0);
                cmd.Blit(tempId, target, material, 0);
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