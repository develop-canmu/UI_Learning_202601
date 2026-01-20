using System;
using System.Collections.Generic;
using CruFramework.Page;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    // 1. Initialize ui and set id
    // 2. Initialize dictionary
    
    public abstract class GrowthLiberationTabSheetBase : Sheet
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
        

        protected long userCharacterId = -1;
        protected UserDataChara uChara;
        protected CharaMasterObject MChara => uChara.MChara;
        protected virtual long currentLv { get; }
        protected long afterLv = 0;
        protected long mElementId = 0;
        protected bool canLevelUp = false;
        protected bool IsMaxLevel => currentLv >= maxLevel;
        protected long maxLevel;
        private long maxAvailableLevel;
        public Action<long> OnModifyLevel { get; set; }
        public Action<GrowthLiberationTabSheetType, long, long, long, Networking.App.Request.NativeApiAutoSell, List<CombinationManager.CollectionProgressData>> OnPlayEffect { get; set; }
        public Func<UniTask> OnLevelUp;
        
        protected abstract void SetDictionary();
        protected abstract bool CanLevelUp(long lv);
        public abstract void OnClickConfirmButton();
        
        public long CurrentLv { get { return currentLv; } }
        
        public long AfterLv { get { return afterLv; } }

        public virtual void Initialize(long id)
        {
            userCharacterId = id;
            uChara = UserDataManager.Instance.chara.Find(userCharacterId);
            SetDictionary();
            InitializeView();
        }

        public virtual void InitializeView(long id)
        {
            uChara = UserDataManager.Instance.chara.Find(userCharacterId);
            InitializeView();
        }
        
        public virtual void InitializeView()
        {
            SetMaxLevel();
            InitializeAfterLevel();
            SetMaxAvailableLevel();
            SetUI();
        }

        private void SetMaxAvailableLevel()
        {
            maxAvailableLevel = currentLv;
            while (maxAvailableLevel < maxLevel && CanLevelUp(maxAvailableLevel + 1)) 
            {
                maxAvailableLevel++;
            }
        }

        private void InitializeAfterLevel()
        {
            if (IsMaxLevel)
            {
                afterLv = currentLv;
                return;
            }
            afterLv = currentLv + 1;
        }

        protected abstract void SetMaxLevel();

        protected abstract void SetRequiredItem();

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
            applyButton.interactable = canLevelUp && !IsMaxLevel;
        }
        
        public void OnClickLvUpButton()
        {
            // キャラの最大レベルを超える場合はreturnする
            if(afterLv >= maxLevel) return;
            afterLv++;
            SetUI();
        }
        
        public void OnClickLvDownButton()
        {
            if(afterLv - 1 <= currentLv) return;
            afterLv--;
            SetUI();
        }

        public void ResetUI()
        {
            InitializeAfterLevel();
            SetUI();
        }
        
        private void SetUI()
        {
            currentLvText.text = string.Format(StringValueAssetLoader.Instance[lvStringValueKey], currentLv);
            afterLvText.text = !IsMaxLevel ? string.Format(StringValueAssetLoader.Instance[lvStringValueKey], afterLv) : StringValueAssetLoader.Instance[MaxLvStringValueKey];
            SetRequiredItem();
            SetButtonInteractable();
            OnModifyLevel?.Invoke(afterLv);
        }
        
        public void OnClickMaxLvButton()
        {
            // afterLvが現在所持しているアイテムで強化できる最大レベル以上か
            var isMaxAvailableLevel = afterLv >= maxAvailableLevel;
            // afterLvが現在所持しているアイテムで強化できる最大レベル以上の場合キャラの最大レベルに設定する
            // afterLvが現在所持しているアイテムで強化できる最大レベル以上ではない場合は現在所持しているアイテムで強化できる最大レベルに設定する
            afterLv = isMaxAvailableLevel ? maxLevel : maxAvailableLevel;
            SetUI();
        }
        
        public void OnClickMinLvButton()
        {
            afterLv = currentLv + 1;
            SetUI();
        }

        public void ResetModifiedLevel()
        {
            OnModifyLevel?.Invoke(currentLv);
        }
    }
}