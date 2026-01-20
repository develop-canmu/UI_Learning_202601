#if !POLYQA_DISABLE

using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace PolyQA.Editor
{
    public class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string AlwaysIncludeShaderName = "Hidden/PolyQA/ReverseResolve";
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var shader = Shader.Find(AlwaysIncludeShaderName);
            if (shader == null)
            {
                return;
            }
            
            var alwaysIncludedShaderList = new AlwaysIncludedShaderList();
            alwaysIncludedShaderList.Add(shader);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            var shader = Shader.Find(AlwaysIncludeShaderName);
            if (shader == null)
            {
                return;
            }
            
            var alwaysIncludedShaderList = new AlwaysIncludedShaderList();
            alwaysIncludedShaderList.Remove(shader);
        }
    }
}

#endif
