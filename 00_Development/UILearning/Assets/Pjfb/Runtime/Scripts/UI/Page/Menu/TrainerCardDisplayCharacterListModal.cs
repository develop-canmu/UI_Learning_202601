using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Storage;
using UnityEngine;

namespace Pjfb
{
    public class TrainerCardDisplayCharacterListModal : ModalWindow
    {
        public class ModalData
        {
            public long DisplayCharacterId { get; }
            
            public ModalData(long displayCharacterId)
            {
                DisplayCharacterId = displayCharacterId;
            }
        }
        [SerializeField]
        private ScrollGrid scrollGrid;
        
        /// <summary>親キャラidをキーにしたマスターのリスト</summary>
        private Dictionary<long, List<ProfileCharaMasterObject>> charaDictionary = new Dictionary<long, List<ProfileCharaMasterObject>>();
        
        private List<TrainerCardDisplayCharacterListScrollData> scrollData = new List<TrainerCardDisplayCharacterListScrollData>();

        private ModalData modalData;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            SetCloseParameter((long)0);
            scrollData.Clear();
            charaDictionary.Clear();
            
            modalData = (ModalData)args;
            scrollGrid.OnItemEvent += OnSelectItem;
            
            // マスターから親キャラを取得
            foreach (CharaParentMasterObject master in MasterManager.Instance.charaParentMaster.values)
            {
                List<ProfileCharaMasterObject> profileCharaList = MasterManager.Instance.profileCharaMaster.GetListByCharaParentId(master.id);
                if(profileCharaList == null || profileCharaList.Count == 0)
                {
                    if(master.mCharaIdList == null || master.mCharaIdList.Length == 0)
                    {
                        // 親キャラに紐づくキャラがいない場合はスキップ
                        continue;
                    }
                    if (master.mCharaIdList.Any(x => MasterManager.Instance.charaMaster.FindData(x).cardType == CardType.Character) == false)
                    {
                        // 選手でない場合はスキップ
                        continue;
                    }
                }
                
                charaDictionary[master.id] = profileCharaList;
                int haveCount = charaDictionary[master.id].Count(profileCharaMasterObject => profileCharaMasterObject.IsHave);
                bool haveNew = charaDictionary[master.id].Any(profileCharaMasterObject => profileCharaMasterObject.IsHave && !LocalSaveManager.saveData.viewedDisplayCharaIds.Contains(profileCharaMasterObject.id));
                scrollData.Add(new TrainerCardDisplayCharacterListScrollData(master.id, haveNew, haveCount, charaDictionary[master.id].Count));
            }
            // 所持数が0のものを下に持っていき、親キャラIDでソート
            scrollData = scrollData.OrderByDescending(data => data.HaveCount != 0).ThenBy(data => MasterManager.Instance.charaParentMaster.FindData(data.CharaParentId).sortNumber).ToList();
            scrollGrid.SetItems(scrollData);
            
            return base.OnPreOpen(args, token);
        }
        
        /// <summary>バッチの更新</summary>
        private void UpdateBadge()
        {
            // 新規表示かの更新
            foreach (TrainerCardDisplayCharacterListScrollData data in scrollData)
            {
                data.IsNew = charaDictionary[data.CharaParentId].Any(profileCharaMasterObject => profileCharaMasterObject.IsHave && !LocalSaveManager.saveData.viewedDisplayCharaIds.Contains(profileCharaMasterObject.id));
            }
            
            // バッチの更新
            scrollGrid.RefreshItemView();
        }

        /// <summary>キャラ選択画面表示</summary>
        private async UniTask OpenCharacterSelect(long charaParentId)
        {
            TrainerCardDisplayCharacterSelectModal.ModalData data = new TrainerCardDisplayCharacterSelectModal.ModalData(charaParentId, this.modalData.DisplayCharacterId, SetCloseParam);
            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainerCardDisplayCharacterSelect, data, destroyCancellationToken);
            
            // モーダルが閉じるまで待機(そのままこのモーダルが閉じられたらキャンセル)
            await modal.WaitCloseAsync(destroyCancellationToken);
            // バッチの更新
            UpdateBadge();
        }
        
        /// <summary>閉じる際のパラメータ設定</summary>
        private void SetCloseParam(long displayCharacterId)
        {
            SetCloseParameter(displayCharacterId);
        }

        /// <summary>コールバック</summary>
        private void OnSelectItem(ScrollGridItem item, object charaParentId)
        {
            OpenCharacterSelect((long)charaParentId).Forget();
        }
    }
}