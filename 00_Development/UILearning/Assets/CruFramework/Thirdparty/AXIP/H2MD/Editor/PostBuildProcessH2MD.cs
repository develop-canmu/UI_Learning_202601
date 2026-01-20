//Unity Cloud BuildでiOS Buildにlibzを追加する

/*

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

public class PostBuildProcess {


    [PostProcessBuild]
    public static void OnPostProcessBuild (BuildTarget buildTarget, string path) {
#if UNITY_IOS
        string projPath = Path.Combine (path, "Unity-iPhone.xcodeproj/project.pbxproj");

        PBXProject proj = new PBXProject ();
        proj.ReadFromString (File.ReadAllText (projPath));

        string target = proj.TargetGuidByName ("Unity-iPhone");
        proj.AddBuildProperty (target, "OTHER_LDFLAGS", "-lz");

        File.WriteAllText (projPath, proj.WriteToString ());
#endif
    }
}

*/