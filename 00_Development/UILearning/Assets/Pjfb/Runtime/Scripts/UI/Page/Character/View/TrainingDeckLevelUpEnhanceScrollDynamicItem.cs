using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using TMPro;
using UnityEngine.UI;

namespace Pjfb
{
    // レベルアップ効果
    public class TrainingDeckLevelUpEnhanceScrollDynamicItem : ScrollDynamicItem
    {
        public class Param
        {
            private BuffTargetData buffData;
            // レベルラベルを表示するか
            private bool isShowLevelLabel = false;
            
            public BuffTargetData BuffData{get => buffData;}
            public bool IsShowLevelLabel { get => isShowLevelLabel;}

            public Param(BuffTargetData buffData, bool isShowLevelLabel)
            {
                this.buffData = buffData;
                this.isShowLevelLabel = isShowLevelLabel;
            }
        }
        
        // 強化Lv.1とかの表示
        [SerializeField] private GameObject labelEnhanceDetail;
        [SerializeField] private TMP_Text labelEnhanceDetailText;

        // どの枠の強化ラベル(？枠目強化)
        [SerializeField] private GameObject lableEnhanceType;
        [SerializeField] private TMP_Text labelEnhanceTypText;
        
        // スキルとステータス表示用
        [SerializeField] private TrainingDeckEffectStatusGroupView groupViewPrefab;
        [SerializeField] private RectTransform groupViewRoot;

        // 強化Lvの表示をするか
        [SerializeField] private bool isLevelShow;
        
        // 生成したgroupView
        private List<TrainingDeckEffectStatusGroupView> groupViewList = new List<TrainingDeckEffectStatusGroupView>();
        
        protected override void OnSetView(object value)
        {
            Param param = (Param)value;

            BuffTargetData buffData = param.BuffData;
            
            // レベルの表示は一番先頭の要素のみ
            labelEnhanceDetail.SetActive(isLevelShow && param.IsShowLevelLabel);
            lableEnhanceType.SetActive(true);
            
            labelEnhanceDetailText.text = string.Format(StringValueAssetLoader.Instance["training.deckEnhance.LevelUpView.EnhanceLevel"], buffData.Level.ToString());
            
            
            switch (buffData.DeckType)
            {
                // 育成対象
                case TrainingDeckSlotType.GrowthTarget:
                    labelEnhanceTypText.text = StringValueAssetLoader.Instance["training.deckEnhance.LevelUpView.GrowthTarget"];
                    break;

                case TrainingDeckSlotType.SupportCharacter:
                case TrainingDeckSlotType.SupportCard:
                case TrainingDeckSlotType.ExSupportCard:
                case TrainingDeckSlotType.SupportEquipment:
                {
                    labelEnhanceTypText.text = string.Format(StringValueAssetLoader.Instance[$"training.deckEnhance.LevelUpView.{buffData.DeckType.ToString()}"], buffData.DeckSlotIndex);
                    break;
                }
            }
            
            // 作成するオブジェクト数とキャッシュしてるオブジェクト数を引いた数(足りていない個数)
            int shortageValue = buffData.BuffCategoryDataList.Count - groupViewList.Count;
            // 足りていない分作成する
            for (int i = 0; i < shortageValue; i++)
            {
                TrainingDeckEffectStatusGroupView statusView = Instantiate(groupViewPrefab, groupViewRoot);
                groupViewList.Add(statusView);   
            }

            // いったん非表示に
            foreach (TrainingDeckEffectStatusGroupView statusGroupView in groupViewList)
            {
                statusGroupView.gameObject.SetActive(false);

            }
            
            // カテゴリ分オブジェクトを生成
            for (int i = 0; i < buffData.BuffCategoryDataList.Count; i++)
            {
                groupViewList[i].gameObject.SetActive(true);
                groupViewList[i].SetEnhanceData(buffData.BuffCategoryDataList[i]);
            }
            
            // 全てセットし終わってからレイアウトの再計算処理
            LayoutRebuilder.ForceRebuildLayoutImmediate(UITransform);
        }
    }
}