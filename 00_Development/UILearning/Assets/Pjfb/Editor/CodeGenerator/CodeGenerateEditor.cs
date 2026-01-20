using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pjfb.Editor.API;
using Pjfb.Editor.Master;

namespace Pjfb.Editor {
    public class CodeGenerateEditor {
        [MenuItem("Pjfb/API/Code Generate")]
        static public void APICodeGenerate() {
            var generator = new APICodeGenerator();
            generator.Generate();
        }

        [MenuItem("Pjfb/API/Lambda Code Generate")]
        static public void LambdaAPICodeGenerate() {
            var generator = new APICodeGenerator();
            generator.GenerateLambda();
        }

        [MenuItem("Pjfb/Master/Code Generate")]
        static public void MasterCodeGenerate() {
            var generator = new MasterCodeGenerator();
            generator.Generate();
        }

    }
}