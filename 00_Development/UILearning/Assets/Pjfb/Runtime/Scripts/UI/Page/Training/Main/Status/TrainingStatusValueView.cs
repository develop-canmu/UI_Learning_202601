using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb.Training
{
    public class TrainingStatusValueView : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text valueText;
        [SerializeField]
        private OmissionTextSetter omissionTextSetter;
        [SerializeField]
        private CharacterStatusRankImage rankImage;
        
        [SerializeField]
        private TrainingStatusUpView upValueView = null;
        /// <summary>ランクアップ</summary>
        public TrainingStatusUpView UpValueView{get{return upValueView;}}
        
        [SerializeField]
        private Animator rankUpAnimator = null;
        /// <summary>ランクアップ</summary>
        public Animator RankUpAnimator{get{return rankUpAnimator;}}
        
        [SerializeField]
        private Animator valueUpAnimator = null;
        /// <summary>ステータスアップ</summary>
        public Animator ValueUpAnimator{get{return valueUpAnimator;}}
        
        [SerializeField]
        private TrainingStatusUpView animationUpValueView = null;
        /// <summary>ステータスアップ</summary>
        public TrainingStatusUpView AnimationUpValueView{get{return animationUpValueView;}}
        
        private BigValue value = BigValue.Zero;
        
        public void OnRankupAnimation()
        {
            long rank = StatusUtility.GetRank(CharaRankMasterStatusType.Status, value);
            rankImage.SetTexture(rank);
        }
        
        public void SetStatusWithoutRank(BigValue value)
        {
            this.value = value;
            valueText.text = value.ToDisplayString(omissionTextSetter.GetOmissionData());
        }
        
        public void SetStatus(BigValue value)
        {
            SetStatusWithoutRank(value);
            long rank = StatusUtility.GetRank(CharaRankMasterStatusType.Status, value);
            rankImage.SetTexture(rank);
        }
        
    }
}