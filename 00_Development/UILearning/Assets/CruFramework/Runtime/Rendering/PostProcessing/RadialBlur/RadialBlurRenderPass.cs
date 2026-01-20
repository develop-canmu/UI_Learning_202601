#if CRUFRAMEWORK_URP_SUPPORT

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CruFramework.Rendering.PostProcessing
{
    public sealed class RadialBlurRenderPass : ScriptableRenderPass, IDisposable
    {
        private static readonly int tempId = Shader.PropertyToID("RadialBlurRT");
        private Material material = null;
        private RenderTargetIdentifier target;

        public RadialBlurRenderPass(Shader shader, RenderPassEvent evt)
        {
            profilingSampler = new ProfilingSampler("RadialBlur Render Pass");
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
            RadialBlur radialBlur = stack.GetComponent<RadialBlur>();

            if (!radialBlur.active) return;
            if (!radialBlur.IsActive()) return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilingSampler))
            {
                ref CameraData cameraData = ref renderingData.cameraData;

                material.SetVector("_Center", radialBlur.Center.value);
                material.SetInt("_Strength", radialBlur.Strength.value);
                material.SetInt("_SampleCount", radialBlur.SampleCount.value);

                int width = cameraData.cameraTargetDescriptor.width;
                int height = cameraData.cameraTargetDescriptor.height;

                cmd.GetTemporaryRT(tempId, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
                cmd.SetGlobalTexture("_MainTex", tempId);
                cmd.Blit(target, tempId, material, 0);
                cmd.Blit(tempId, target);
                cmd.ReleaseTemporaryRT(tempId);

                // TODO SwapBufferへの書き換え
                //material.SetFloat("_Weight", grayscale.weight.value);
                //Blit(cmd, ref renderingData, material);
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