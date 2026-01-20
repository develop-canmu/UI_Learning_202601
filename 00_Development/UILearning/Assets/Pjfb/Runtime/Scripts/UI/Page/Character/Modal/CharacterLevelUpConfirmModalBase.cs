using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Pjfb.Character
{
    public abstract class CharacterLevelUpConfirmModalBase<T> : ModalWindow where T : class
    {
        public class Data
        {
            public readonly long UserCharacterId;
            public readonly long CurrentLv;
            public readonly long AfterLv;

            protected Data(long userCharacterId, long currentLv, long afterLv)
            {
                UserCharacterId = userCharacterId;
                CurrentLv = currentLv;
                AfterLv = afterLv;
            }
        }

        // モーダルタイトル
        [SerializeField] protected TMPro.TMP_Text modalTitle = null;
        
        [SerializeField] protected TMPro.TMP_Text currentLvText;
        [SerializeField] protected TMPro.TMP_Text afterLvText;
        protected T modalData;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (T) args;
            InitializeUi();
            return base.OnPreOpen(args, token);
        }
        public static async UniTask<CruFramework.Page.ModalWindow> OpenAsync(ModalType modalType,T data, CancellationToken token)
        {
            return await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(modalType, data as T, token);
        }

        protected abstract void InitializeUi();
        protected abstract UniTask CallApi();
        
        public void OnClickApplyButton()
        {
            CallApi().Forget();
        }

        public void OnClickCancelButton()
        {
            SetCloseParameter(null);
            Close();
        }

    }
}