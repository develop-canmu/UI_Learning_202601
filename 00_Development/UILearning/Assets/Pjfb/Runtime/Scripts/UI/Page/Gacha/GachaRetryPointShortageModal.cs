using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Pjfb.UserData;
using Pjfb.Master;

namespace Pjfb.Gacha
{
    public class GachaRetryPointShortageModal : ModalWindow
    {
        public class Param {
            public GachaPendingFrameData pendingData = null;
            public long gachaCategoryId = 0;
        }

        [SerializeField]
        private IconImage[] iconImages = null;
        
        [SerializeField]
        private TextMeshProUGUI necessaryCountText = null;
        
        [SerializeField]
        private TextMeshProUGUI possessionCountText = null;
        [SerializeField]
        protected TextMeshProUGUI messageText = null;
        
        [SerializeField]
        private PossessionItemUi alternativePointUi = null;
        [SerializeField]
        private TextMeshProUGUI periodText = null;

        Param _param = null;
        
        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);

            _param = (Param)args;
            var pointMaster = MasterManager.Instance.pointMaster.FindData(_param.pendingData.RetryPointId);
            
            
            // ポイント画像セット
            foreach (IconImage iconImage in iconImages)
            {
                iconImage.SetTexture(pointMaster.id);
            }
            // 必要数
            necessaryCountText.text = _param.pendingData.RetryPrice.ToString();
            // 所持数
            long possessionCount = 0;
            if(UserDataManager.Instance.point.data.ContainsKey(_param.pendingData.RetryPointId))
            {
                possessionCount = UserDataManager.Instance.point.Find(_param.pendingData.RetryPointId).value;
            }
            possessionCountText.text = possessionCount.ToString();

            // メッセージ
            messageText.text = string.Format(StringValueAssetLoader.Instance["gacha.not_enough_point"], pointMaster.name);

            // 仮想ポイントを使用可能か
            var uPointAlternative = GachaUtility.GetPointAlternative(_param.pendingData.RetryPointId, _param.gachaCategoryId);
            var isUsableAlternativePoint = uPointAlternative != null && uPointAlternative.UserData.value > 0;
            
            // 仮想ポイントを所持・使用できる場合は状態を表示
            alternativePointUi.gameObject.SetActive(isUsableAlternativePoint);
            if (isUsableAlternativePoint)
            {
                alternativePointUi.SetCount(uPointAlternative.UserData.pointId, uPointAlternative.UserData.value);
                periodText.text = uPointAlternative.AlternativeMasterObject.endAt.TryConvertToDateTime().ToString(StringValueAssetLoader.Instance["point.expire_date_format"]);
            }
            
        }
        
        /// <summary>
        /// uGUI
        /// </summary>
        public void OnClickPositiveButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Shop, true, null);
            Close();
        }
        
        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }
    }
}
