using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using Pjfb.Master;

namespace Pjfb
{
    public class CommonExecuteConfirmModal : ModalWindow
    {
        
        public class Data
        {
            public long pointId;
            public long value;
            public string title;
            public string message;
            public string positiveButtonText;
            public Action executeAction;

            public Data(long _pointId, long _value, string _title, string _message, string _positiveButtonText = null, Action _action = null)
            {
                pointId = _pointId;
                value = _value;
                title = _title;
                message = _message;
                positiveButtonText = _positiveButtonText;
                executeAction = _action;
            }
        }
        
        [SerializeField] private IconImage iconImage = null;
        [SerializeField] private TextMeshProUGUI beforeCountText = null;
        [SerializeField] private TextMeshProUGUI afterCountText = null;
        [SerializeField] private TMP_Text titleText = null;
        [SerializeField] private TMP_Text messageText = null;
        [SerializeField] private GameObject termsTransactionRoot;
        [SerializeField] private TextMeshProUGUI positiveButtonText;
        private Data modalParam;
            
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalParam = (Data)args;
            // ポイント画像セット
            iconImage.SetTexture(modalParam.pointId);
            // 実行前のポイント数
            long beforeCount = 0;
            if(UserDataManager.Instance.point.data.ContainsKey(modalParam.pointId))
            {
                beforeCount = UserDataManager.Instance.point.Find(modalParam.pointId).value;
            }
            beforeCountText.text = beforeCount.ToString();
            // 実行後のポイント数
            afterCountText.text = (beforeCount - modalParam.value).ToString();
            
            titleText.text = modalParam.title;
            messageText.text = modalParam.message;
            termsTransactionRoot.SetActive(modalParam.pointId == ConfigManager.Instance.mPointIdGem);
            positiveButtonText.text = string.IsNullOrEmpty(modalParam.positiveButtonText)
                ? StringValueAssetLoader.Instance["common.ok"]
                : modalParam.positiveButtonText;
            await base.OnPreOpen(args, token);
        }
        
        public void OnClickPositiveButton()
        {
            modalParam?.executeAction();
            Close();
        }
        
        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }
        
    }
}
