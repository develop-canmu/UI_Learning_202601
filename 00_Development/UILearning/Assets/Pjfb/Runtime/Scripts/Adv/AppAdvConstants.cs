using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public static class AppAdvConstants
    {
        /// <summary>ユーザー名置換文字列</summary>
        public static readonly string ReplaceUserName = "{UserName}";	    
        /// <summary>一人称置換文字列</summary>
        public static readonly string ReplaceFirstPersonString = "{me}";
        /// <summary>トレーニング中キャラ名文字列</summary>
        public static readonly string ReplaceCharacterName = "{CharacterName}";	
        
        /// <summary>トレーニングのチップ所持数</summary>
        public static readonly string ReplaceTrainingTips = "{TrainingTips}";
    }
}