using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaExecuteConfirmModalBase : ModalWindow
    {
        [SerializeField]
        private TextMeshProUGUI titleText = null;
        
        [SerializeField]
        protected TextMeshProUGUI messageText = null;
        
        private GachaCategoryData categoryData = null;
        /// <summary>実行に必要なデータ</summary>
        public GachaCategoryData CategoryData { get { return categoryData; } }
        
        private PointMasterObject pointMaster = null;
        /// <summary>ポイントマスタ</summary>
        protected PointMasterObject PointMaster { get { return pointMaster; } }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            categoryData = (GachaCategoryData)args;
            pointMaster = MasterManager.Instance.pointMaster.FindData(categoryData.PointId);
            
            // タイトル
            var prizeCount = CategoryData.PrizeCount * CategoryData.GachaCount;
            titleText.text = string.Format(StringValueAssetLoader.Instance[GachaUtility.PrizeCountStringKey], prizeCount);
            // メッセージ
            messageText.text = string.Format(StringValueAssetLoader.Instance[GachaUtility.DrawGachaConfirmStringKey], PointMaster.name, CategoryData.Price, PointMaster.unitName, prizeCount);
            
            return base.OnPreOpen(args, token);
        }
        
        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }
    }
}
