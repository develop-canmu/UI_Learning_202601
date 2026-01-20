using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{

    public class SwipeableParams<T>
    {
        public SwipeableParams()
        {
            DetailOrderList = new List<T>();
            StartIndex = 0;
            OnIndexChanged = null;
        }
        public SwipeableParams(List<T> detailOrderList, int startIndex, Action<int> onIndexChanged = null)
        {
            this.DetailOrderList = detailOrderList;
            this.StartIndex = startIndex;
            this.OnIndexChanged = onIndexChanged;
        }
        public List<T> DetailOrderList;
        public int StartIndex;
        public Action<int> OnIndexChanged;
    }
    public class SwipeableDetailModalParams<T>
    {
        public SwipeableDetailModalParams(SwipeableParams<T> swipeableParams, string titleStringKey = null)
        {
            this.SwipeableParams = swipeableParams;
            this.TitleStringKey = titleStringKey;
        }

        public SwipeableParams<T> SwipeableParams;
        public string TitleStringKey;
    }
    
    public abstract class SwipeableDetailModalWindow<T, TParams> : ModalWindow where TParams : SwipeableDetailModalParams<T>
    {
        [SerializeField] private SwipeUi swipeUi;
        [SerializeField] private TextMeshProUGUI titleText;
        protected int currentIndex = 0;
        protected TParams modalParams;
        private int OrderListCount => detailOrderList.Count;
        protected T objectDetail => detailOrderList[currentIndex];
        private List<T> detailOrderList;
        protected virtual string defaultTitleStringKey => "";

        public static void Open(ModalType modalType, TParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(modalType, data);
        }
        
        public static async UniTask<CruFramework.Page.ModalWindow> OpenModalAsync(ModalType modalType, TParams data)
        {
            return await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(modalType, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalParams = (TParams) args;
            detailOrderList = modalParams.SwipeableParams.DetailOrderList.ToList();
            
            currentIndex = Mathf.Clamp(modalParams.SwipeableParams.StartIndex, 0, OrderListCount - 1);
            swipeUi.EnableSwipe = detailOrderList.Count > 1;
            swipeUi.OnNextAction = NextDetail;
            swipeUi.OnPrevAction = PrevDetail;
            titleText.text = StringValueAssetLoader.Instance[modalParams.TitleStringKey ?? defaultTitleStringKey];
            Init();
            modalParams.SwipeableParams.OnIndexChanged?.Invoke(currentIndex);
            return base.OnPreOpen(args, token);
        }

        
        
        public virtual void NextDetail()
        {
            currentIndex++;
            currentIndex %= OrderListCount;
            Init();
            modalParams.SwipeableParams.OnIndexChanged?.Invoke(currentIndex);
        }

        public virtual void PrevDetail()
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = OrderListCount - 1;
            Init();
            modalParams.SwipeableParams.OnIndexChanged?.Invoke(currentIndex);
        }

        protected abstract void Init();
    }
}
