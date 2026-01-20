using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

using Pjfb.Master;
using TMPro;
using System;
using Pjfb.UI;
using System.Linq;
using CruFramework.UI;
using Pjfb.Rivalry;
using Pjfb.UserData;

namespace Pjfb
{
    [Serializable]
    public class BoostEffectSheet
    {
        public BoostEffectTabSheetType type;
        public ScrollGrid scrollGrid;
        public GameObject totalBoostObject;
        public TMP_Text totalBoostText;
        [HideInInspector] public bool isInitialized;
    }

    public class BoostEffectModal : ModalWindow
    {
        public class Data
        {
            public HuntSpecificCharaMasterObject huntSpecificCharaMasterObject;
            public HuntTimetableMasterObject huntTimetableMasterObject;
            public DeckPanelScrollGridItem.Parameters deckParameters;
            public bool showCurrentEffect = true;
            public bool showCharaIconActivation;
        }

        [SerializeField] private BoostEffectTabSheetManager tabSheetManager;
        [SerializeField] private BoostEffectSheet[] sheetList;


        private Data data;
        private bool isScrollerInitialized;

        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.BoostEffect, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (Data)args;
            tabSheetManager.OnPreOpenSheet -= InitializeScroller;
            tabSheetManager.OnPreOpenSheet += InitializeScroller;
            if (!isScrollerInitialized)
            {
                InitializeScroller(tabSheetManager.CurrentSheetType);
            }
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
        }

        private void InitializeScroller(BoostEffectTabSheetType type)
        {
            // Sheet管理
            var sheet = sheetList.FirstOrDefault(s => s.type == type);
            if (sheet == null || sheet.isInitialized)
            {
                return;
            }
            sheet.totalBoostObject.SetActive(data.showCurrentEffect);
            if (data.showCurrentEffect && data.deckParameters != null)
            {
                sheet.totalBoostText.text = string.Format(StringValueAssetLoader.Instance["rivalry.rewardboost.totalvalue"], RivalryManager.GetRewardBoostValue(data.huntTimetableMasterObject.id, data.deckParameters));
            }

            // 表示
            switch (type)
            {
                case BoostEffectTabSheetType.Player:
                    InitPlayerScroller(sheet);
                break;
                case BoostEffectTabSheetType.Reward:
                    InitRewardScroller(sheet);
                break;
            }
            sheet.isInitialized = true;

            isScrollerInitialized = true;
        }

        private void InitPlayerScroller(BoostEffectSheet sheet)
        {
            var charaList = new List<BoostCharaScrollData>();
            List<object> mCharaIdList = (List<object>)MiniJSON.Json.Deserialize(data.huntSpecificCharaMasterObject.mCharaIdList);
            foreach (var charaId in mCharaIdList)
            {
                charaList.Add(
                    new BoostCharaScrollData(
                        (long)charaId, 
                        UserDataManager.Instance.chara.data.Values.Any(chara => chara.charaId == (long)charaId), 
                        data.huntSpecificCharaMasterObject.rate, 
                        null, 
                        data.deckParameters?.viewParams.iconParams.Any(chara => chara.nullableCharacterData?.MCharaId == (long)charaId) ?? true,
                        data.showCharaIconActivation
                    )
                );
            }
            sheet.scrollGrid.SetItems(charaList);
        }

        private void InitRewardScroller(BoostEffectSheet sheet)
        {
            var rewardList = new List<BoostEffectRewardGridItem.ItemParams>();
            List<object> mPointIdList = (List<object>)MiniJSON.Json.Deserialize(data.huntSpecificCharaMasterObject.mPointIdList);
            foreach (var pointId in mPointIdList)
            {
                var reward = new BoostEffectRewardGridItem.ItemParams();
                reward.prizeJsonViewData = new PrizeJsonViewData((long)pointId);
                rewardList.Add(reward);
            }
            sheet.scrollGrid.SetItems(rewardList);
        }
        
    }
}