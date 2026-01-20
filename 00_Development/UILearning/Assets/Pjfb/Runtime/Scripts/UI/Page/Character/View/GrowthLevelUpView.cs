using System;
using System.Collections.Generic;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    public class GrowthLevelUpView : MonoBehaviour
    {
        
        private static readonly string MaxLvStringValueKey = "character.base_chara_growth_liberation.growth_liberation_level_max";
        
        [SerializeField][StringValue] private string lvStringValueKey;
        [SerializeField] protected TMPro.TMP_Text currentLvText = null;
        [SerializeField] protected TMPro.TMP_Text afterLvText = null;
        [SerializeField] private GameObject lvMaxItemText = null;
        [SerializeField] private UIButton maxLvButton;
        [SerializeField] private UIButton lvUpButton;
        [SerializeField] private UIButton minLvButton;
        [SerializeField] private UIButton lvDownButton;
        [SerializeField] private UIButton applyButton;
        [SerializeField] private ScrollGrid itemScrollGrid;
        
        // 現在のレベル
        private long currentLv = 0;
        // 強化後のレベル
        private long afterLv = 0;
        // 最大レベル
        private long maxLevel = 0;
        // 現在のアイテムであげられる最大レベル
        private long maxAvailableLevel = 0;

        // 強化に必要なポイント情報
        private EnhancePointInfo enhancePointInfo;
        
        public bool IsMaxLevel => currentLv >= maxLevel;
        
        public long AfterLv{get => afterLv;}
        
        // レベルがあげられるか計算するコールバック
        private Func<long, long, bool> onCanLevelUp { get; set; }
        
        // レベルが適用された際のコールバック
        private Action<long> onModifyLevel { get; set; }
        
        // 初期化
        public void Initialize(long currentLv, long maxLevel, EnhancePointInfo enhancePointInfo, bool isModifyLevel)
        {
            this.currentLv = currentLv;
            this.maxLevel = maxLevel;
            this.enhancePointInfo = enhancePointInfo;
            InitializeMaxAvailableLevel();
            InitializeAfterLevel();
            UpdateView(isModifyLevel);
        }

        //// <summary> 現在のレベルが上がった際の更新 </summary>
        public void UpdateCurrentLv(long currentLv)
        {
            this.currentLv = currentLv;
            InitializeMaxAvailableLevel();
            InitializeAfterLevel();
            UpdateView();
        }

        // コールバックの登録
        public void SetCanLevelUp(Func<long, long, bool> OnCanLevelUp)
        {
            this.onCanLevelUp = OnCanLevelUp;
        }

        // レベル適用時のコールバック登録
        public void SetModifyLevel(Action<long> OnModifyLevel)
        {
            this.onModifyLevel = OnModifyLevel;
        }

        // 強化後のレベルの初期化
        private void InitializeAfterLevel()
        {
            // 最大レベルなら今のレベル、それ以外なら今のレベルに+1した値に
            afterLv = IsMaxLevel ? currentLv : currentLv + 1;
        }
        
        // 現在の素材であげることのできる最大レベル
        private void InitializeMaxAvailableLevel()
        {
            maxAvailableLevel = currentLv;
            while (maxAvailableLevel < maxLevel && onCanLevelUp(currentLv, maxAvailableLevel + 1)) 
            {
                maxAvailableLevel++;
            }
        }
        
        // Viewの更新
        public void UpdateView(bool isModifyLevel = true)
        {
            currentLvText.text = string.Format(StringValueAssetLoader.Instance[lvStringValueKey], currentLv);
            afterLvText.text = !IsMaxLevel ? string.Format(StringValueAssetLoader.Instance[lvStringValueKey], afterLv) : StringValueAssetLoader.Instance[MaxLvStringValueKey];
            SetButtonInteractable();
            SetRequiredItems();
            
            if (isModifyLevel)
            {
                onModifyLevel?.Invoke(afterLv);
            }
        }
        
        private void SetButtonInteractable()
        {
            //　afterLvが現在のレベルの一つ上のレベル以下か
            var isMinAvailableLevel = afterLv <= currentLv + 1;
            var isAfterLvMax = afterLv >= maxLevel;
            lvMaxItemText.SetActive(IsMaxLevel);
            maxLvButton.interactable = !IsMaxLevel && !isAfterLvMax;
            lvUpButton.interactable = !IsMaxLevel && !isAfterLvMax;
            lvDownButton.interactable = !IsMaxLevel && !isMinAvailableLevel;
            minLvButton.interactable = !IsMaxLevel && !isMinAvailableLevel;
            applyButton.interactable = onCanLevelUp(currentLv, afterLv) && !IsMaxLevel;
        }

        // 必要なアイテムを表示する
        private void SetRequiredItems()
        {
            var itemIconGridItemDataList = new List<ItemIconGridItem.Data>();

            // 最大レベルでないときのみコストを計算する
            if (IsMaxLevel == false)
            {
                Dictionary<long, long> requiredCostList = enhancePointInfo.GetTotalRequiredCost(currentLv, afterLv);

                // コストデータをアイテムアイコンデータに入れる
                foreach (KeyValuePair<long, long> itemCost in requiredCostList)
                {
                    long mPointId = itemCost.Key;
                    long requiredValue = itemCost.Value;
                    long possessionValue = UserDataManager.Instance.point.Find(mPointId)?.value ?? 0;
                    itemIconGridItemDataList.Add(new ItemIconGridItem.Data(mPointId, requiredValue, possessionValue, true));
                }
            }

            itemScrollGrid.SetItems(itemIconGridItemDataList);
        }

        // 強化後のレベルを初期状態に戻す
        public void ResetView()
        {
            InitializeAfterLevel();
            UpdateView();
        }
        
        // レベルを現在のレベルに
        public void ResetModifiedLevel()
        {
            onModifyLevel?.Invoke(currentLv);
        }

        #region OnGUI
        
        public void OnClickLvUpButton()
        {
            // キャラの最大レベルを超える場合はreturnする
            if(afterLv >= maxLevel) return;
            afterLv++;
            UpdateView();
        }
        
        public void OnClickLvDownButton()
        {
            if(afterLv - 1 <= currentLv) return;
            afterLv--;
            UpdateView();
        }
        
        public void OnClickMaxLvButton()
        {
            // afterLvが現在所持しているアイテムで強化できる最大レベル以上か
            var isMaxAvailableLevel = afterLv >= maxAvailableLevel;
            // afterLvが現在所持しているアイテムで強化できる最大レベル以上の場合キャラの最大レベルに設定する
            // afterLvが現在所持しているアイテムで強化できる最大レベル以上ではない場合は現在所持しているアイテムで強化できる最大レベルに設定する
            afterLv = isMaxAvailableLevel ? maxLevel : maxAvailableLevel;
            UpdateView();
        }
        
        public void OnClickMinLvButton()
        {
            afterLv = currentLv + 1;
            UpdateView();
        }
        
        #endregion
    }

}
