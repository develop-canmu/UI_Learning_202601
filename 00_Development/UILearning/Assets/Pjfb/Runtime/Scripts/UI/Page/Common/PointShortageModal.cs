using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;

namespace Pjfb
{
    public class PointShortageModal : ModalWindow
    {
        
        
        public class Arguments
        {
            private long pointId = 0;
            /// <summary>アイテムId</summary>
            public long PointId{get{return pointId;}set{pointId = value;}}
            
            private long needCount = 0;
            /// <summary>必要数</summary>
            public long NeedCount{get{return needCount;}set{needCount = value;}}
            
            private long currentCount = 0;
            /// <summary>現在の所持数</summary>
            public long CurrentCount{get{return currentCount;}set{currentCount = value;}}
            
            private string title = string.Empty;
            /// <summary>タイトル</summary>
            public string Title{get{return title;}set{title = value;}}
            
            private string message = string.Empty;
            /// <summary>メッセージ</summary>
            public string Message{get{return message;}set{message = value;}}
        }

        [SerializeField]
        private TMP_Text titleText = null;
        
        [SerializeField]
        private TMP_Text messageText = null;

        [SerializeField]
        private TMP_Text needCountText = null;
        [SerializeField]
        private TMP_Text currentCountText = null;
        
        [SerializeField]
        private IconImage[] iconImages = null;


        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments arguments = (Arguments)args;
            
            // タイトル
            titleText.text = arguments.Title;
            // メッセージ
            messageText.text = arguments.Message;
            
            // 必要数
            needCountText.text = arguments.NeedCount.ToString();
            // 現在の数
            currentCountText.text = arguments.CurrentCount.ToString();
            
            // アイコン
            foreach(IconImage icon in iconImages)
            {
                icon.SetTexture(arguments.PointId);
            }
            
            return base.OnPreOpen(args, token);
        }

        public void OnGotoShop()
        {
            AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
            // ショップページへ
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Shop, true, null);
        }
        
        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }
    }
}