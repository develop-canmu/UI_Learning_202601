using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Pjfb.UserData;
using Pjfb.Master;

namespace Pjfb
{
    public class CommonExecuteConfirmPointShortageModal : ModalWindow
    {

        public class Data
        {
            public long pointId;
            public long value;
            public string title;
        }
        
        [SerializeField] private IconImage[] iconImages;
        [SerializeField] private TextMeshProUGUI necessaryCountText;
        [SerializeField] private TextMeshProUGUI possessionCountText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] protected TextMeshProUGUI messageText;
        [SerializeField] private GameObject termsTransactionRoot;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var data = (Data)args;
            var pointId = data.pointId;
            var pointValue = data.value;
            var pointMaster = MasterManager.Instance.pointMaster.FindData(pointId);

            titleText.text = string.IsNullOrEmpty(data.title)
                ? string.Format(StringValueAssetLoader.Instance["common.not_enough_point.title"], pointMaster.name)
                : data.title;
     
            foreach (IconImage iconImage in iconImages)
            {
                iconImage.SetTexture(pointId);
            }
            
            necessaryCountText.text = pointValue.ToString();
            
            long possessionCount = 0;
            if(UserDataManager.Instance.point.data.ContainsKey(pointId))
            {
                possessionCount = UserDataManager.Instance.point.Find(pointId).value;
            }
            
            possessionCountText.text = possessionCount.ToString();
            messageText.text = string.Format(StringValueAssetLoader.Instance["common.not_enough_point"], pointMaster.name);
            
            termsTransactionRoot.SetActive(pointMaster.id == ConfigManager.Instance.mPointIdGem);

            return base.OnPreOpen(null, token);
        }
        
        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }

        public void OnClickPositiveButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Shop, true, null);
            Close();
        }
    }
}