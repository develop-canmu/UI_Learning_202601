using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Storage;
using Pjfb.Utility;
using UnityEngine;

namespace Pjfb
{
    public class TrainerCardCustomizeModal : ModalWindow
    {
        public class ModalData
        {
            /// <summary>現在の着せ替え情報</summary>
            public long CurrentProfileFrameId { get; }
            
            public ModalData(long currentProfileFrameId)
            {
                CurrentProfileFrameId = currentProfileFrameId;
            }
        }
        
        [SerializeField]
        private ScrollGrid scrollGrid;
        
        [SerializeField]
        private UIButton applyButton;

        private ModalData modalData;
        private List<TrainerCardCustomizeImageScrollItem.ScrollData> scrollDataList = new ();

        private int currentIndex = 0;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (ModalData) args;
            currentIndex = 0;
            
            scrollGrid.OnItemEvent += OnSelect;
            
            scrollDataList.Clear();

            ProfileFrameMasterObject[] masterData = MasterManager.Instance.profileFrameMaster.values.Where(master => master.IsHave).OrderByDescending(master => master.id == modalData.CurrentProfileFrameId)
                .ThenBy(master => master.id).ToArray();

            for (int i = 0; i < masterData.Length; i++)
            {
                scrollDataList.Add(new TrainerCardCustomizeImageScrollItem.ScrollData(masterData[i].id, !LocalSaveManager.saveData.viewedCustomizeFrameIds.Contains(masterData[i].id),
                    currentIndex == i, 
                    masterData[i].id == modalData.CurrentProfileFrameId, i));
            }
            
            scrollGrid.SetItems(scrollDataList);
            
            // 設定中のデータを選択状態に
            scrollGrid.SelectItem(currentIndex);
            
            applyButton.interactable = false;
            
            return base.OnPreOpen(args, token);
        }

        protected override UniTask OnPreClose(CancellationToken token)
        {
            scrollGrid.OnItemEvent -= OnSelect;
            
            // 閲覧済のデータを保存
            LocalSaveManager.saveData.viewedCustomizeFrameIds = scrollDataList.Select(scrollData => scrollData.Id).ToList();
            LocalSaveManager.Instance.SaveData();
            return base.OnPreClose(token);
        }
        
        private async UniTask OnApplyAsync()
        {
            SetCloseParameter(await TrainerCardUtility.UpdateProfileFrame(scrollDataList[currentIndex].Id));
            await CloseAsync();
        }
        
        /// <summary>見た目更新</summary>
        private void OnSelect(ScrollGridItem item, object value)
        {
            int index = (int) value;
            
            scrollDataList[currentIndex].IsSelect = false;
            
            // 選択中のIdを更新
            currentIndex = index;
            
            // 選択中のデータを更新
            scrollDataList[currentIndex].IsSelect = true;
            
            // ボタンの有効無効
            applyButton.interactable = modalData.CurrentProfileFrameId != scrollDataList[currentIndex].Id;
        }

        /// <summary>UGUI</summary>
        public void OnApply()
        {
            OnApplyAsync().Forget();
        }
    }
}