using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using CruFramework;
using Pjfb.Master;
using System;
using System.Text.RegularExpressions;

namespace Pjfb.Training
{
    public class TrainingMenuDetailModal : ModalWindow
    {

        [SerializeField]
        private CancellableRawImage bannerImage = null;
        [SerializeField]
        private UIButton specialAttackButton = null;
        
        private long id = 0;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            id = (long)args;
            
            // 特攻ボタン
            Dictionary<long, CharaTrainingStatusMasterObject> mStatus = MasterManager.Instance.charaTrainingStatusMaster.FindLvMaxData(id);
            specialAttackButton.interactable = mStatus.Count > 0;
            // バナーの読み込み
            await bannerImage.SetTextureAsync(ResourcePathManager.GetPath("TrainingMenuBanner", id));
            await base.OnPreOpen(args, token);
        }

        /// <summary>
        /// UGUI
        /// </summary>
        public void OnHowToPlayButton()
        {
            HowToPlayModal.HowToData howtoData = HowToPlayModal.CreateHowToDataByScenarioId(id);
            // 遊び方モーダルを開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.HowToPlay, howtoData);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnTargetButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingMenuDetailTarget, id);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnSnarioSpecialAttackButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ScenarioSpecialAttack, id);
        }
    }
}