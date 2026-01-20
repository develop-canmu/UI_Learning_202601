using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Storage;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class TrainerCardDisplayCharacterSelectModal : ModalWindow
    {
        public class ModalData
        {
            public long CharaParentId { get; }
            
            public long CurrentDisplayCharacterId { get; }
            
            public Action<long> OnSelected { get; }
            
            public ModalData(long charaParentId, long currentDisplayCharacterId, Action<long> onSelected)
            {
                CharaParentId = charaParentId;
                CurrentDisplayCharacterId = currentDisplayCharacterId;
                OnSelected = onSelected;
            }
        }
        
        [SerializeField]
        private TextMeshProUGUI nameText;
        
        [SerializeField]
        private DisplayCharacterImage characterCardImage;
        
        [SerializeField]
        private ScrollGrid scrollGrid;
        
        [SerializeField]
        private TextMeshProUGUI haveCountText;
        
        [SerializeField]
        private UIButton applyButton;
        
        private int previewIndex = 0;
        
        private ModalData modalData;
        
        private List<TrainerCardDisplayCharacterIconScrollItem.ScrollData> scrollData = new ();

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (ModalData) args;
            scrollGrid.OnItemEvent += OnSelected;
            scrollData.Clear();
            
            // 名前設定
            CharaParentMasterObject charaParentMaster = MasterManager.Instance.charaParentMaster.FindData(modalData.CharaParentId);
            nameText.text = charaParentMaster.name;
            
            // マスタ取得
            List<ProfileCharaMasterObject> masterData = MasterManager.Instance.profileCharaMaster.GetListByCharaParentId(modalData.CharaParentId).OrderByDescending(master => master.priority).ThenBy(master => master.id).ToList();
            
            // スクロールビューのデータ作成
            for (int index = 0; index < masterData.Count; index++)
            {
                scrollData.Add(new TrainerCardDisplayCharacterIconScrollItem.ScrollData(index, masterData[index].id, masterData[index].IsHave, 
                    masterData[index].IsHave && !LocalSaveManager.saveData.viewedDisplayCharaIds.Contains(masterData[index].id), index == previewIndex, 
                    masterData[index].id == modalData.CurrentDisplayCharacterId));
            }

            // スクロールビューにデータをセット
            scrollGrid.SetItems(scrollData);
            
            // 初期表示
            scrollGrid.SelectItem(scrollData.First(data => data.IsHave).Index);

            haveCountText.text = string.Format(StringValueAssetLoader.Instance["common.ratio_value"], masterData.Count(data => data.IsHave), masterData.Count);
            
            // プレビュー表示
            await DisplayCharacter();
            
            // ボタンの有効無効
            applyButton.interactable = modalData.CurrentDisplayCharacterId != scrollData[previewIndex].DisplayCharacterId && scrollData[previewIndex].IsHave;
            
            await base.OnPreOpen(args, token);
        }
        
        protected override UniTask OnPreClose(CancellationToken token)
        {
            scrollGrid.OnItemEvent -= OnSelected;
            
            // 今回閲覧したキャラクターを保存
            LocalSaveManager.saveData.viewedDisplayCharaIds.AddRange(scrollData.Where(data => data.IsHave && data.IsNew).Select(data => data.DisplayCharacterId));
            LocalSaveManager.Instance.SaveData();
            
            return base.OnPreClose(token);
        }
        
        /// <summary>選択更新</summary>
        private async UniTask UpdateSelect(int index)
        {
            // 現在の選択を解除
            scrollData[previewIndex].IsSelect = false;
            // 選択更新
            previewIndex = index;
            // 選択を設定
            scrollData[previewIndex].IsSelect = true;
            
            // ボタンの有効無効
            applyButton.interactable = modalData.CurrentDisplayCharacterId != scrollData[previewIndex].DisplayCharacterId;
            
            // プレビュー表示
            await DisplayCharacter();
        }
        
        /// <summary>UGUI</summary>
        public void Prev()
        {
            int index = GetPrevIndex();
            if (index == previewIndex)
            {
                return;
            }
            
            scrollGrid.ScrollToItemIndex(index);
            scrollGrid.SelectItem(index);
        }
        
        /// <summary>UGUI</summary>
        public void Next()
        {
            int index = GetNextIndex();
            if (index == previewIndex)
            {
                return;
            }
            scrollGrid.ScrollToItemIndex(index);
            scrollGrid.SelectItem(index);
        }
        
        /// <summary>前のインデックスを取得</summary>
        private int GetPrevIndex()
        {
            // 一つ前のインデックスを取得
            int i = previewIndex - 1;
            
            // ループしている場合は最後のインデックスに戻る
            while (i != previewIndex)
            {
                // 一番最初のインデックスの場合は最後のインデックスに戻る
                if (i < 0)
                {
                    i = scrollData.Count - 1;
                }
                
                // 所持しているキャラクターがあればそのインデックスを返す
                if (scrollData[i].IsHave)
                {
                    return i;
                }
                
                i--;
            }
            
            return previewIndex;
        }
        
        /// <summary>次のインデックスを取得</summary>
        private int GetNextIndex()
        {
            // 一つ前のインデックスを取得
            int i = previewIndex + 1;
            
            // ループしている場合は最初のインデックスに戻る
            while (i != previewIndex)
            {
                // 一番最後のインデックスの場合は最初のインデックスに戻る
                if (i == scrollData.Count)
                {
                    i = 0;
                }
                
                // 所持しているキャラクターがあればそのインデックスを返す
                if (scrollData[i].IsHave)
                {
                    return i;
                }

                i++;
            }
            
            return previewIndex;
        }

        /// <summary>PreView表示</summary>
        private async UniTask DisplayCharacter()
        {
            TrainerCardDisplayCharacterIconScrollItem.ScrollData data = scrollData[previewIndex];
            await characterCardImage.SetTextureAsync(data.DisplayCharacterId);
        }
        
        /// <summary>コールバック</summary>
        private void OnSelected(ScrollGridItem item, object index)
        {
            UpdateSelect((int)index).Forget();
        }
        
        /// <summary>UGUI</summary>
        public void OnApply()
        {
            modalData.OnSelected?.Invoke(scrollData[previewIndex].DisplayCharacterId);
            // リスト表示モーダルだけ閉じる
            AppManager.Instance.UIManager.ModalManager.RemoveModals(window => window is TrainerCardDisplayCharacterListModal);
            Close();
        }
    }
}