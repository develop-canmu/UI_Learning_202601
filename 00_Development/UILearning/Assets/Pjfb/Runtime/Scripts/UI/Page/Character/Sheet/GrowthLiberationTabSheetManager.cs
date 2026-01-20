using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    public enum GrowthLiberationTabSheetType
    {
        Growth,
        Liberation
    }
    
    public class GrowthLiberationTabSheetManager : SheetManager<GrowthLiberationTabSheetType>
    {
        [Serializable]
        public class TabBadgeUi
        {
            public GrowthLiberationTabSheetType type;
            public GameObject badge;
        }
        
        [SerializeField] private GrowthLiberationTabSheetGrowth growthLiberationTabSheetGrowth;
        [SerializeField] private GrowthLiberationTabSheetLiberation growthLiberationTabSheetLiberation;
        [SerializeField] private List<TabBadgeUi> tabBadgeUiList = new List<TabBadgeUi>();

        // 強化後レベル(開いている強化シートがレベル強化でないなら現在のレベルを返す)
        public long GrowthAfterLevel => CurrentSheetType == GrowthLiberationTabSheetType.Growth ? growthLiberationTabSheetGrowth.AfterLv : growthLiberationTabSheetGrowth.CurrentLv;
        // 強化後解放レベル(開いている強化シートが能力解放レベル強化でないなら現在の解放レベルを返す)
        public long LiberationAfterLevel => CurrentSheetType == GrowthLiberationTabSheetType.Liberation ? growthLiberationTabSheetLiberation.AfterLv : growthLiberationTabSheetLiberation.CurrentLv;

        public void InitializeUI(long uCharaId, Action<long> onModifyGrowthLevel, Action<long> onModifyLiberationLevel,
            Func<UniTask> onGrowthLevelUp, Func<UniTask> onLiberationLevelUp,
            Action<GrowthLiberationTabSheetType, long, long, long, NativeApiAutoSell,
                List<CombinationManager.CollectionProgressData>> playEffect)
        {
            UpdateTabBadge(GrowthLiberationTabSheetType.Growth, uCharaId);
            UpdateTabBadge(GrowthLiberationTabSheetType.Liberation, uCharaId);
            growthLiberationTabSheetGrowth.OnModifyLevel = onModifyGrowthLevel;
            growthLiberationTabSheetLiberation.OnModifyLevel = onModifyLiberationLevel;
            growthLiberationTabSheetGrowth.Initialize(uCharaId);
            growthLiberationTabSheetLiberation.Initialize(uCharaId);
            growthLiberationTabSheetGrowth.OnPlayEffect = playEffect;
            growthLiberationTabSheetLiberation.OnPlayEffect = playEffect;

            growthLiberationTabSheetGrowth.OnLevelUp = async () =>
            {
                await onGrowthLevelUp();
                UpdateTabBadge(GrowthLiberationTabSheetType.Growth, uCharaId);
                growthLiberationTabSheetGrowth.InitializeView(uCharaId);
                
            };
                
            growthLiberationTabSheetLiberation.OnLevelUp = async () =>
            {
                await onLiberationLevelUp();
                UpdateTabBadge(GrowthLiberationTabSheetType.Liberation, uCharaId);
                growthLiberationTabSheetGrowth.InitializeView(uCharaId);
                growthLiberationTabSheetGrowth.ResetModifiedLevel();
                growthLiberationTabSheetLiberation.InitializeView(uCharaId);
            };
        }
        
        public void OnClickGrowthTab()
        {
            growthLiberationTabSheetLiberation.ResetModifiedLevel();
            OpenSheet(GrowthLiberationTabSheetType.Growth, null);
            growthLiberationTabSheetGrowth.ResetUI();
        }

        public void OnClickLiberationTab()
        {
            growthLiberationTabSheetGrowth.ResetModifiedLevel();
            OpenSheet(GrowthLiberationTabSheetType.Liberation, null);
            growthLiberationTabSheetLiberation.ResetUI();
        }
        
        private void UpdateTabBadge(GrowthLiberationTabSheetType type, long uCharaId)
        {
            var uChara = UserDataManager.Instance.chara.Find(uCharaId);
            bool isActive;
            switch (type)
            {
                case GrowthLiberationTabSheetType.Growth:
                    isActive = uChara?.IsPossibleGrowth() ?? false;
                    break;
                case GrowthLiberationTabSheetType.Liberation:
                    isActive = uChara?.IsPossibleLiberation() ?? false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            foreach (var tabUi in tabBadgeUiList)
            {
                if(tabUi.type != type || tabUi.badge == null) continue;
                tabUi.badge.SetActive(isActive);
            }
        }
    }
    
    
}


