using System;
using System.Collections.Generic;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingPracticeSkillDetailView : ScrollDynamicItem
    {
        public class IndividualCharacterData
        {
            // 個別のスキル情報
            private PracticeSkillInfo practiceSkillInfo;
            public PracticeSkillInfo PracticeSkillInfo => practiceSkillInfo;
            
            // キャラID
            private long mCharaId;
            public long CharaId => mCharaId;
            
            // ユーザーキャラID
            private long uCharaId;
            public long UCharaId => uCharaId;
            
            // キャラレベル
            private long characterLevel;
            public long CharacterLevel => characterLevel;
            
            // 解放レベル
            private long liberationLevel;
            public long LiberationLevel => liberationLevel;
            
            // ステータスIDリスト
            private long[] statusIdList;
            public long[] StatusIdList => statusIdList;
            
            // 特攻スキルか
            private bool isSpecialAttack;
            public bool IsSpecialAttack => isSpecialAttack;
            
            // キャラの表示順
            private int index;
            public int Index => index;
            
            // レベル上げ出来るか
            private bool canGrowth;
            public bool CanGrowth => canGrowth;
            
            public IndividualCharacterData(PracticeSkillInfo practiceSkillInfo, long mCharaId, long uCharaId, long characterLevel, long liberationLevel, long[] statusIdList, bool isSpecialAttack, int index, bool canGrowth)
            {
                this.practiceSkillInfo = practiceSkillInfo;
                this.mCharaId = mCharaId;
                this.uCharaId = uCharaId;
                this.characterLevel = characterLevel;
                this.liberationLevel = liberationLevel;
                this.statusIdList = statusIdList;
                this.isSpecialAttack = isSpecialAttack;
                this.index = index;
                this.canGrowth = canGrowth;
            }
            
        }
        
        
        public class ViewData
        {
            // トータル表示用のトレーニングスキル情報
            private PracticeSkillInfo parentInfo;
            public PracticeSkillInfo ParentInfo => parentInfo;
            
            // 個別のトレーニングスキル情報リスト
            private List<IndividualCharacterData> individualPracticeInfoList;
            public List<IndividualCharacterData> IndividualPracticeInfoList => individualPracticeInfoList;
            
            // 選択中か
            private bool isSelected;
            public bool IsSelected => isSelected;
            
            // 詳細表示中か
            private bool isOpenPullDown = false;
            public bool IsOpenPullDown => isOpenPullDown;
            
            public ViewData(PracticeSkillInfo parentInfo, List<IndividualCharacterData> individualPracticeInfoList, bool isSelected)
            {
                this.parentInfo = parentInfo;
                this.individualPracticeInfoList = individualPracticeInfoList;
                this.isSelected = isSelected;
            }
            
            public void SetOpenPullDown(bool isOpen)
            {
                isOpenPullDown = isOpen;
            }
        }
        
        
        // トレーニングスキルアイコン
        [SerializeField]
        private PracticeSkillImage practiceSkillImage;
        
        // トレーニングスキル名
        [SerializeField]
        private TextMeshProUGUI practiceSkillNameText;
        [SerializeField]
        private TextMeshProUGUI practiceSkillValueText;
        [SerializeField]
        private OmissionTextSetter practiceSkillValueOmissionData;
        
        // 選択中の練習メニューカードで発動するかのバッジ
        [SerializeField]
        private GameObject selectedBadge;
        
        // 個別表示用のView
        [SerializeField]
        private PracticeSkillView practiceSkillViewBase;
        
        // 個別表示のRoot
        [SerializeField]
        private GameObject skillListRoot;
        
        [SerializeField]
        private Image pullDownOpenObject;
        
        [SerializeField]
        private Image pullDownCloseObject;
        
        // プルダウンを開いたことがあるか
        private bool isOpenedOnce = false;
        
        private bool isPullDownOpen = false;
        private ViewData viewData;
        private List<PracticeSkillView> individualList = new List<PracticeSkillView>();
        
        protected override void OnSetView(object value)
        {
            // ベースのViewは非表示にしておく
            practiceSkillViewBase.gameObject.SetActive(false);
            viewData = (ViewData)value;
            // スキルアイコン
            practiceSkillImage.SetTexture(viewData.ParentInfo.GetIconId());
            // スキル名 + 効果値
            practiceSkillNameText.text = viewData.ParentInfo.GetName();
            practiceSkillValueText.text = viewData.ParentInfo.ToValueName(practiceSkillValueOmissionData.GetOmissionData());
            // 選択中のカードで発動する場合は色を変える
            Color baseColor = viewData.IsSelected ? ColorValueAssetLoader.Instance["training.practice_skill.selected"] : Color.white;
            pullDownOpenObject.color = baseColor;
            pullDownCloseObject.color = baseColor;
            // 選択中バッジ
            selectedBadge.SetActive(viewData.IsSelected);
            
            // 開いたかどうかのフラグをリセット
            isOpenedOnce = false;
            // 個別表示の更新
            UpdateDetailSize(viewData.IsOpenPullDown);
        }

        public void OnClickDetail()
        {
            UpdatePullDownAsync().Forget();
        }

        private async UniTask UpdatePullDownAsync()
        {
            UpdateDetailSize(isPullDownOpen == false);
            // Unityレイアウトの再計算
            // LayoutRebuilderだとUnity側のDestroy処理が上手く走らず正しくサイズが反映されないため、次フレームまで待機する
            await UniTask.NextFrame();
            // サイズ変更を通知
            RecalculateSize();
        }

        private void UpdateDetailSize(bool isOpen)
        {
            if (isOpen == true)
            {
                // プルダウンを開いたことがない場合個別表示のゲームオブジェクトを削除して再生成
                if (isOpenedOnce == false)
                {
                    foreach (var instance in individualList)
                    {
                        GameObject.Destroy(instance.gameObject);
                    }
                    individualList.Clear();

                    foreach (var individualData in viewData.IndividualPracticeInfoList)
                    {
                        var instance = GameObject.Instantiate(practiceSkillViewBase, skillListRoot.transform);
                        instance.SetSkillData(individualData.PracticeSkillInfo, individualData.CharaId, individualData.UCharaId, individualData.CharacterLevel, individualData.LiberationLevel, individualData.StatusIdList, individualData.CanGrowth);
                        instance.SetSpecialAttackEnable(individualData.IsSpecialAttack);
                        instance.gameObject.SetActive(true);
                        individualList.Add(instance);
                    }
                }

                isOpenedOnce = true;
            }
            
            isPullDownOpen = isOpen;
            skillListRoot.SetActive(isPullDownOpen);
            pullDownOpenObject.gameObject.SetActive(isPullDownOpen);
            pullDownCloseObject.gameObject.SetActive(isPullDownOpen == false);
            viewData.SetOpenPullDown(isPullDownOpen);
        }
    }
}