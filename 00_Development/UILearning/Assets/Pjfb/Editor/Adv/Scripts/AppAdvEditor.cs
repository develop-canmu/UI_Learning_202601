using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;
using UnityEditor;
using CruFramework.Editor.Adv;
using Pjfb.Adv;

namespace Pjfb.Editor.Adv
{
    public class AppAdvEditor : AdvEditor<AppAdvManager>
    {
        [MenuItem("CruFramework/Adv/AdvEditor")]
        public static void Open()
        {
            GetWindow<AppAdvEditor>("AdvEditor");
        }

        protected override void OnStartTestRun(AdvManager maanger)
        {
            maanger.AddMessageReplaceString(AppAdvConstants.ReplaceCharacterName, "潔");
        }

        private AdvConfig config = null;
        
        /// <summary>エディタ用ファイルの保存先</summary>
        protected override string EditorFileDirectory { get{return "Assets/Pjfb/Editor/Adv/GraphViewFiles";}}
        /// <summary>Advアセットの保存先</summary>
        protected override string AdvFileDirectory { get{return "Assets/Pjfb/Runtime/AssetBundles/Remote/Adv/Command";} }
        /// <summary>Configファイル</summary>
        public override AdvConfig AdvConfigAsset
        {
            get
            {
                if(config == null)
                {
                    config = AssetDatabase.LoadAssetAtPath<AdvConfig>("Assets/Pjfb/Runtime/AssetBundles/Remote/Adv/AdvConfig.asset");
                }
                return config;
            }
        }
    }
}
