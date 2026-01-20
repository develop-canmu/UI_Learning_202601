#if CRUFRAMEWORK_URP_SUPPORT

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CruFramework.Rendering.PostProcessing
{
    public sealed class GrayscaleRenderPass : ScriptableRenderPass, IDisposable
    {
        private static readonly int tempId = Shader.PropertyToID("GrayscaleRT");
        private Material material = null;
        private RenderTargetIdentifier target;

        public GrayscaleRenderPass(Shader shader, RenderPassEvent evt)
        {
            profilingSampler = new ProfilingSampler("Grayscale Render Pass");
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
            Grayscale grayscale = stack.GetComponent<Grayscale>();
            
            if (!grayscale.active) return;
            if (!grayscale.IsActive()) return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilingSampler))
            {
                // TODO SwapBufferへの書き換え

                ref CameraData cameraData = ref renderingData.cameraData;

                material.SetFloat("_Weight", grayscale.Weight.value);

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