using System;

namespace Pjfb
{
    public class ConfirmModalData
    {
        private string title = string.Empty;
        /// <summary>ウィドウタイトル</summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string message = string.Empty;
        /// <summary>ウィンドウメッセージ</summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        
        private string caution = string.Empty;
        /// <summary>注意</summary>
        public string Caution
        {
            get { return caution; }
            set { caution = value; }
        }

        private ConfirmModalButtonParams positiveButtonParams = null;
        /// <summary>オレンジのボタン</summary>
        public ConfirmModalButtonParams PositiveButtonParams
        {
            get { return positiveButtonParams; }
            set { positiveButtonParams = value; }
        }
        
        private ConfirmModalButtonParams negativeButtonParams = null;
        /// <summary>白いボタン</summary>
        public ConfirmModalButtonParams NegativeButtonParams
        {
            get { return negativeButtonParams; }
            set { negativeButtonParams = value; }
        }
        
        private bool disableBackKey = false;
        /// <summary>注意</summary>
        public bool DisableBackKey
        {
            get { return disableBackKey; }
            set { disableBackKey = value; }
        }
        
        public ConfirmModalData()
        {
        }

        public ConfirmModalData(string title, string message, string caution, ConfirmModalButtonParams buttonParams, bool disableBackKey = false)
        {
            this.title = title;
            this.message = message;
            this.caution = caution;
            this.positiveButtonParams = null;
            this.negativeButtonParams = buttonParams;
            this.disableBackKey = disableBackKey;
        }

        public ConfirmModalData(string title, string message, string caution, ConfirmModalButtonParams positiveButtonParams, ConfirmModalButtonParams negativeButtonParams, bool disableBackKey = false)
        {
            this.title = title;
            this.message = message;
            this.caution = caution;
            this.positiveButtonParams = positiveButtonParams;
            this.negativeButtonParams = negativeButtonParams;
            this.disableBackKey = disableBackKey;
        }
    }

    public class ConfirmModalButtonParams
    {
        public string text;
        public Action<ModalWindow> onClick;
        public bool isRed = false;

        public ConfirmModalButtonParams(string text, Action<ModalWindow> onClick,bool isRed = false)
        {
            this.text = text;
            this.onClick = onClick;
            this.isRed = isRed;
        }
    }
}
