using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace CruFramework.Editor.H2MD
{
	public class H2MDPostProcessBuild
	{
		[PostProcessBuild]
		public static void OnPostProcessBuild(BuildTarget target, string path)
		{
			// IOSのみ
	#if UNITY_IOS
			if (target != BuildTarget.iOS)return;
		
			string projectFilePath = PBXProject.GetPBXProjectPath(path);
			PBXProject project = new PBXProject();
			project.ReadFromFile(projectFilePath);

			string targetGuid = project.GetUnityFrameworkTargetGuid();
			project.AddFrameworkToProject(targetGuid, "libz.tbd", false);
			project.WriteToFile(projectFilePath);
	#endif
		}
	}
}
