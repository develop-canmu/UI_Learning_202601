using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
    public class CombinationCollectionView : CombinationViewBase<CombinationCollectionPracticeSkillView, CombinationManager.CollectionMultipleSkillData>
    {
        public enum ActivatedLabelType
        {
            Total,
            UnLock
        }
        
        [SerializeField] private GameObject badgeObject;
        [SerializeField] private GameObject detailButton;
        [SerializeField] private TMPro.TMP_Text activatedLabelText;
        [SerializeField] private Transform canActiveSkillRoot;
        [SerializeField] private CombinationCollectionPracticeSkillView canActiveSkillPrefab;

        protected List<CombinationCollectionPracticeSkillView> canActiveSkillPrefabList = new();
        
        private long mCombinationId;
        private bool showDetailActiveButton;
        private Action<long> onProgressCombinationCollectionAction;
        
        public void Initialize(CombinationCollectionScrollData data)
        {
            mCombinationId = data.MCombinationId;
            detailButton.SetActive(data.ShowDetailButton);
            badgeObject.SetActive(data.ShowBadge);
            // キャラアイコンの設定
            InitializeCharaUI(data.CharacterDetailDataList, data.MCharaIdPossessionDictionary, data.ShowCharaLevel);
            // 解放済みと未解放のスキル表示設定
            switch (data.SkillDataLists.ActivatedLabelType)
            {
                case ActivatedLabelType.Total:
                    activatedLabelText.text = StringValueAssetLoader.Instance["combination.cell.skill.label.activated_total"];
                    break;
                case ActivatedLabelType.UnLock:
                    activatedLabelText.text = StringValueAssetLoader.Instance["combination.cell.skill.label.activated_unlock"];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(data.SkillDataLists.ActivatedLabelType), data.SkillDataLists.ActivatedLabelType, null);
            }
            onProgressCombinationCollectionAction = data.OnProgressCombinationCollectionAction;
            InitializeSkillUi(data.SkillDataLists.ActivatedSkillDataList, data.SkillDataLists.LockSkillDataList, data.SkillDataLists.NotActiveLabelType);
            InitializeSkillPrefabListUnique(ActivatedSkillPrefabList, data.SkillDataLists.ActivatedSkillDataList, false);
            InitializeSkillPrefabListUnique(notActiveSkillPrefabList, data.SkillDataLists.LockSkillDataList, false);
            // 解放可能スキル表示設定(解放可能はコレクションだけなのでCombinationCollectionViewで対応)
            InitializeCollectionMultipleSkillPrefab(canActiveSkillPrefabList, data.SkillDataLists.CanActiveSkillDataList, canActiveSkillPrefab, canActiveSkillRoot);
            InitializeSkillPrefabListUnique(canActiveSkillPrefabList, data.SkillDataLists.CanActiveSkillDataList, true);
        }
        
        protected override void InitializeSkillPrefab(CombinationCollectionPracticeSkillView tPrefab,  CombinationManager.CollectionMultipleSkillData tSkill)
        {
            if (tSkill == null) return;
            var hasSkill = (tSkill.PracticeSkillDataList?.Count ?? 0) > 0;
            if(!hasSkill) return;

            tPrefab.InitializeUI(tSkill.PracticeSkillDataList, !string.IsNullOrEmpty(tSkill.LockString),
                tSkill.LockString, tSkill.ShowSkillHighlight);
        }

        private void InitializeSkillPrefabListUnique(List<CombinationCollectionPracticeSkillView> prefabList ,List<CombinationManager.CollectionMultipleSkillData> collectionMultipleSkillDataList,  bool isActiveButtonInteractable)
        {
            for (int i = 0; i < prefabList.Count; i++)
            {
                if (i >= collectionMultipleSkillDataList.Count) continue;
                var prefab = prefabList[i];
                var collectionMultipleSkillData = collectionMultipleSkillDataList[i];
                prefab.InitializeUniqueUi(mCombinationId, collectionMultipleSkillData.ProgressId,
                    collectionMultipleSkillData.ShowActiveButton,
                    isActiveButtonInteractable, onProgressCombinationCollectionAction);
            }
        }

        public void OnClickDetailButton()
        {
            var combinationCollection = CombinationManager.GetAllCombinationCollectionList().FirstOrDefault(data => data.MCombinationId == mCombinationId);
            if (combinationCollection == null)
            {
                CruFramework.Logger.LogError($"combinationCollectionがnullです。クライアントに確認をお願いします。　mCombinationId：{mCombinationId}");
                return;
            }

            CombinationCollectionSkillSetDetailModal.Open(
                new CombinationCollectionSkillSetDetailModal.Data(combinationCollection, onProgressCombinationCollectionAction));
        }
    }
}
