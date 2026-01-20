using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEditor;

namespace Pjfb.InGame.ClubRoyal
{
    [CustomEditor(typeof(DebugClubRoyalInGameCharacterAnimation))]
    public class DebugClubRoyalInGameCharacterAnimationEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as DebugClubRoyalInGameCharacterAnimation;
            if (GUILayout.Button("ResetPosition"))
            {
                script.ResetPosition();
            }
            
            if (GUILayout.Button("PlayMatchUp"))
            {
                script.PlayMatchUp().Forget();
            }
            
            if (GUILayout.Button("PlayLongMatchUp"))
            {
                script.PlayLongMatchUp().Forget();
            }
            
            if (GUILayout.Button("PlayGoal"))
            {
                script.PlayGoalMatchUp().Forget();
            }

            if (GUILayout.Button("PlayLastGoal"))
            {
                script.PlayLastGoalMatchUp().Forget();
            }

        }
    }
}