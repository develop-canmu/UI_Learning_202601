using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.Training
{
    public class TrainingActionBoostPointView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text currentPointText = null;
        [SerializeField]
        private TMP_Text levelUpPointText = null;
        [SerializeField]
        private TMP_Text reloadPointText = null;
        
        [SerializeField]
        private UIButton boostLevelUpButton = null;
        [SerializeField]
        private UIButton reloadButton = null;
        
        [SerializeField]
        private GameObject boostCooltimeRoot = null;
        [SerializeField]
        private TMP_Text boostCooltimeText = null;
        
        [SerializeField]
        private GameObject reloadCooltimeRoot = null;
        [SerializeField]
        private TMP_Text reloadCooltimeText = null;
        
        private long currentPoint = 0;

        // 引き直しが可能か
        private bool isActiveHandReset = false;
        
        /// <summary>現在のポイントを表示</summary>
        public void SetView(TrainingMainArguments args)
        {
            // 現在のトレーニングで引き直しが可能かを取得
            isActiveHandReset = TrainingUtility.IsEnableHandReload(args.Pending.mTrainingScenarioId);
            SetView(args.PointStatus.value, args.PointStatus.enableLevelUp, args.PointStatus.enableHandReset);
            SetBoostLevelUpCost(args.PointStatus.levelUpCostValue);
            SetReloadCost(args.PointStatus.handResetCostValue);
            
            SetBoostCooltime( args.PointStatus.remainLevelUpEnableTurn );
            SetReloadCooltime( args.PointStatus.remainHandResetEnableTurn );
        }
        
        /// <summary>現在のポイントを表示</summary>
        public void SetView(long point, bool canLevelUp, bool canReload)
        {
            currentPoint = point;
            // ポイント表示
            currentPointText.text = point.GetStringNumberWithComma();
            // レベルアップボタン
            boostLevelUpButton.interactable = canLevelUp;
            // 引き直し(ポイントが足りない場合は非活性化)
            reloadButton.interactable = canReload;
            // 引き直しボタンを表示するか
            reloadButton.gameObject.SetActive(isActiveHandReset);
        }
        
        public void SetBoostCooltime(long cooltime)
        {
            if(cooltime <= 0)
            {
                boostCooltimeRoot.SetActive(false);
                return;
            }
            
            boostCooltimeRoot.SetActive(true);
            boostCooltimeText.text = string.Format( StringValueAssetLoader.Instance["training.boost.cooltime"], cooltime); 
        }
        
        public void SetReloadCooltime(long cooltime)
        {
            if(cooltime <= 0)
            {
                reloadCooltimeRoot.SetActive(false);
                return;
            }
            
            
            reloadCooltimeRoot.SetActive(true);
            reloadCooltimeText.text = string.Format( StringValueAssetLoader.Instance["training.boost.cooltime"], cooltime);     
        }
        
        public void SetBoostLevelUpCost(long costValue)
        {
            levelUpPointText.text = costValue.GetStringNumberWithComma();
            // 足りない場合は赤色にする
            levelUpPointText.color = GetCostColor(costValue <= currentPoint);
        }
        
 
        
        /// <summary>現在のポイントを表示</summary>
        public void SetReloadCost(long costValue)
        {
            reloadPointText.text = costValue.GetStringNumberWithComma();
            // 足りない場合は赤色にする
            reloadPointText.color = GetCostColor(costValue <= currentPoint);
        }
        
        /// <summary>コストの色を取得</summary>
        private Color GetCostColor(bool enable)
        {
            return enable ? ColorValueAssetLoader.Instance["white"] : ColorValueAssetLoader.Instance["red"];
        }
    }
}