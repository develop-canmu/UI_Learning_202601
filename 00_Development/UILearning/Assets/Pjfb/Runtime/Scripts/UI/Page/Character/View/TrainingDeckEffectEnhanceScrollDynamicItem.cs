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
    // 編成効果
    public class TrainingDeckEffectEnhanceScrollDynamicItem : ScrollDynamicItem
    {
        // アイコン表示用
        [SerializeField] private ItemIconContainer itemIconContainer;
        // 育成対象枠の場合のオブジェクトの表示用
        [SerializeField] private GameObject trainingCharacter;
        // 育成対象枠以外の場合のオブジェクトの表示用
        [SerializeField] private GameObject deckFrame;
        // 効果対象枠のテキスト表示用
        [SerializeField] private TMP_Text deckFrameText;
        // 効果対象のテキストオブジェクト
        [SerializeField] private GameObject deckTypeLabel;
        // 効果対象のテキスト表示用
        [SerializeField] private TMP_Text deckTypeLabelText;
        // スキルとステータス表示用
        [SerializeField] private TrainingDeckEffectStatusGroupView groupViewPrefab;
        [SerializeField] private RectTransform groupViewRoot;

        // 生成したgroupView
        private List<TrainingDeckEffectStatusGroupView> groupViewList = new List<TrainingDeckEffectStatusGroupView>();
        
        protected override void OnSetView(object value)
        {
            BuffTargetData buffData = (BuffTargetData)value;
            // バフ対象が育成対象か
            bool isGrowthTarget = buffData.DeckType == TrainingDeckSlotType.GrowthTarget;
            
            // 育成対象枠なら育成対象枠用のオブジェクトをアクティブに
            trainingCharacter.SetActive(isGrowthTarget);
            deckFrame.SetActive(!isGrowthTarget);
            deckTypeLabel.SetActive(!isGrowthTarget);

            // 育成対象でないときは枠番号とラベルを表示する
            if (isGrowthTarget == false)
            {
                //ラベルの設定
                SetEnhanceTargetLabel(buffData.DeckType, buffData.DeckSlotIndex);
            }

            // 作成するオブジェクト数とキャッシュしてるオブジェクト数を引いた数(足りていない個数)
            int shortageValue = buffData.BuffCategoryDataList.Count - groupViewList.Count;
            // 足りていない分作成する
            for (int i = 0; i < shortageValue; i++)
            {
                TrainingDeckEffectStatusGroupView statusView = Instantiate(groupViewPrefab, groupViewRoot);
                groupViewList.Add(statusView);   
            }

            // いったん全部非表示
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

        private void SetEnhanceTargetLabel(TrainingDeckSlotType slotType, long index)
        {
            deckFrameText.text = string.Format(StringValueAssetLoader.Instance["training.deckEnhance.slotNumber"], index.ToString());
            // Typeによって文言を変える
            deckTypeLabelText.text = StringValueAssetLoader.Instance[$"training.deckEnhance.deckTypeLabel.{slotType.ToString()}"];
        }
    }
}