#if CRUFRAMEWORK_URP_SUPPORT

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CruFramework.Rendering.PostProcessing
{
    public sealed class FastBlurRendererFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private Shader shader = null;

        private FastBlurRenderPass pass = null;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            pass.Setup(renderer.cameraColorTargetHandle);
        }

        public override void Create()
        {
            pass = new FastBlurRenderPass(shader, RenderPassEvent.BeforeRenderingPostProcessing);
        }

        protected override void Dispose(bool disposing)
        {
            pass.Dispose();
            base.Dispose(disposing);
        }
    }
}

#endif