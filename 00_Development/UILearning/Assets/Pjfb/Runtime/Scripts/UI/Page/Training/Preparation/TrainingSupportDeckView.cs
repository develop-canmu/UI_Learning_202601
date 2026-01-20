using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;
using Pjfb.Networking.App.Request;

using System;
using Pjfb.Character;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Training
{
    public class TrainingSupportDeckView : MonoBehaviour
    {
        public struct SelectData
        {
            public int DeckIndex;
            public int Order;
            public TrainingDeckMemberType Type;
            public bool IsExtraCharacter;
            
            public SelectData(int deckIndex, int order, TrainingDeckMemberType type, bool isExtraCharacter)
            {
                DeckIndex = deckIndex;
                Order = order;
                Type = type;
                IsExtraCharacter = isExtraCharacter;
            }
        }
        
        
        
        private DeckData deckData = null;
        /// <summary>デッキ</summary>
        public DeckData DeckData{get{return deckData;}}
        
        protected TrainingPreparationArgs arguments = null;
        
        protected TrainingCharacterData[] characterDatas = null;
        protected Dictionary<long, long> characterTupleDictionary = new Dictionary<long, long>();

        [SerializeField]
        protected TrainingSupportDeckTypeCountView[] typeCountViews = null;
        
        [SerializeField]
        protected DeckNameView deckNameView = null;

        [SerializeField] 
        protected GameObject combinationLockRoot = null;
        [SerializeField] 
        protected GameObject combinationSkillCountRoot = null;
        [SerializeField]
        protected TMP_Text combinationSkillCountText = null;
        
        public event Action<SelectData> OnSelected = null;
        public event Action<TrainingSupportDeckView> OnReset = null;
        public event Action<TrainingSupportDeckView> OnRecommend = null;
        
        
        /// <summary>UGUI</summary>
        public void OnDeckResetButton()
        {
            if(OnReset != null)
            {
                OnReset(this);
            }
        }
        
        /// <summary>UGUI</summary>
        public void OnDeckRecommendEditButton()
        {
            if(OnRecommend != null)
            {
                OnRecommend(this);
            }
        }
        
        protected void CallOnSelect(int order, TrainingDeckMemberType type, bool isExtraCharacter)
        {
            if(OnSelected != null)
            {
                OnSelected( new SelectData(DeckData.Index, order, type, isExtraCharacter) );
            }
        }
        
        /// <summary>タイプ数表示更新</summary>
        public void UpdateTypeCount()
        {
            Dictionary<CharacterType, int> typeCount = new Dictionary<CharacterType, int>();

            foreach(CharacterType type in System.Enum.GetValues(typeof(CharacterType)))
            {
                typeCount.Add(type, 0);
            }
            
            OnUpdateTypeCount(typeCount);
            
            // 各タイプの数を表示
            foreach(TrainingSupportDeckTypeCountView view in typeCountViews)
            {
                view.SetCount(typeCount[view.Type]);
            }
        }
        
        protected virtual void OnUpdateTypeCount(Dictionary<CharacterType, int> typeCount)
        {
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnCombinationSkillButton()
        {
            if (!CombinationManager.IsUnLockCombination())
            {
                var systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(CombinationManager.CombinationLockId);            
                if(systemLock != null && !string.IsNullOrEmpty(systemLock.description))
                {
                    string description = systemLock.description;
                    ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                    ConfirmModalData data = new ConfirmModalData(StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                    return;
                }
            }

            TrainingCombinationListModal.Arguments args =
                new TrainingCombinationListModal.Arguments(characterDatas, true, true, string.Empty, UpdateCombinationView);
            AppManager.Instance.UIManager.ModalManager.OpenModal( ModalType.TrainingCombinationList,  args);
        }
        
        public void SetView(TrainingPreparationArgs args, DeckData deckData)
        {
            arguments = args;
            this.deckData = deckData;
            UpdateView();
        }
        
        protected virtual void OnUpdateView()
        {
        }
        
        public void UpdateView()
        {
            OnUpdateView();
            // コネクトスキル
            UpdateCombinationView();
            // デッキ名を表示
            deckNameView.SetDeckData(DeckData);
            // タイプ数の表示
            UpdateTypeCount();
        }
        
        private void UpdateCombinationView()
        {
            // コネクトスキル
            bool unlockCombination = CombinationManager.IsUnLockCombination();
            combinationLockRoot.SetActive(!unlockCombination);
            int trainingCombinationSkillCount = CombinationManager.ActivatingCombinationTrainingCount(characterTupleDictionary);
            combinationSkillCountRoot.SetActive(unlockCombination && trainingCombinationSkillCount > 0);
            // テキスト
            combinationSkillCountText.text = string.Format( StringValueAssetLoader.Instance["common.combination.current_active"], trainingCombinationSkillCount );
        }
    }
}