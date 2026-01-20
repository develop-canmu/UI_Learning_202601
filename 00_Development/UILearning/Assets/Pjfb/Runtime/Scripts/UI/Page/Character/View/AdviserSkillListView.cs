using System;
using System.Collections.Generic;
using CruFramework.UI;
using UnityEngine;

namespace Pjfb.Character
{
    /// <summary> アドバイザースキルリストView </summary>
    public class AdviserSkillListView : MonoBehaviour
    {
        [SerializeField]
        private ScrollGrid scroll;

        // 選択時のイベント処理
        private Action<int> selectedSkillEvent = null;
        
        /// <summary> 現在のレベルと能力解放レベルでのスキルの表示 </summary>
        public void SetView(BattleConst.AbilityType abilityType, long charaId, long level, long liberation)
        {
            // スキルを取得
            List<CharaAbilityInfo> skillInfoList = CharaAbilityUtility.GetAbilityListAll(abilityType, charaId, level, liberation);

            SetView(skillInfoList, level, liberation);
        }

        /// <summary> 現在のレベルと能力解放レベルでのスキルの表示 </summary>
        public void SetView(List<CharaAbilityInfo> abilityList, long level, long liberation)
        {
            // スクロール用アイテム
            List<AdviserSkillScrollGridItem.Param> itemList = new List<AdviserSkillScrollGridItem.Param>();

            foreach (CharaAbilityInfo info in abilityList)
            {
                itemList.Add(new AdviserSkillScrollGridItem.Param(info, level, liberation, false));
            }
            
            // エールスキルセット
            scroll.SetItems(itemList);
        }

        /// <summary> 強化後の差分をハイライトしたスキル表示 </summary>
        public void SetAfterView(BattleConst.AbilityType abilityType, long charaId, long currentLevel, long currentLiberation, long afterLevel, long afterLiberation)
        {
            // 現在レベルで修得済みのスキルを取得
            List<CharaAbilityInfo> currentSkillInfoList = CharaAbilityUtility.GetAbilityAcquireList(abilityType, charaId, currentLevel, currentLiberation);
            // 強化後のスキルを取得
            List<CharaAbilityInfo> afterLevelSkillInfoList = CharaAbilityUtility.GetAbilityListAll(abilityType, charaId, afterLevel, afterLiberation);
            
            List<AdviserSkillScrollGridItem.Param> itemList = new List<AdviserSkillScrollGridItem.Param>();
            
            foreach (CharaAbilityInfo afterSkillInfo in afterLevelSkillInfoList)
            {
                // レベルが上がっているか
                bool isLevelUp = false;
                // 新規スキルか
                bool isNewSkill = true;

                // アンロック済みのスキルのみレベルが上昇しているかを見る
                if (afterSkillInfo.IsAbilityUnLock)
                {
                    // 同一のスキルを探す
                    foreach (CharaAbilityInfo currentSkillInfo in currentSkillInfoList)
                    {
                        // スキルIdが一致するならスキルレベルを見てLvが上がっているか確認する
                        if (afterSkillInfo.SkillId == currentSkillInfo.SkillId)
                        {
                            isLevelUp = afterSkillInfo.SkillLevel > currentSkillInfo.SkillLevel;
                            // 取得済み
                            isNewSkill = false;
                            break;
                        }
                    }

                    // 新規スキルならレベルアップしたとみなす
                    if (isNewSkill)
                    {
                        isLevelUp = true;
                    }
                }

                itemList.Add(new AdviserSkillScrollGridItem.Param(afterSkillInfo, afterLevel, afterLiberation, isLevelUp));
            }
            
            // エールスキルセット
            scroll.SetItems(itemList);
        }

        /// <summary> スキルを選択した際のイベント処理の登録 </summary>
        public void SetSelectedSkillEvent(Action<int> selectedEvent)
        {
            selectedSkillEvent = selectedEvent;
            scroll.OnSelectedItemEvent -= OnSelectedSkill;
            scroll.OnSelectedItemEvent += OnSelectedSkill;
        }

        /// <summary> スキルを選択中に設定する </summary>
        public void SetSkillSelect(int index, bool isScrollPosition)
        {
            // 指定Indexのアイテムを選択する
            scroll.SelectItem(index);
            
            // 選択したアイテムの位置にスクロールするなら
            if (isScrollPosition)
            {
                scroll.ScrollToItemIndex(index);
            }
        }

        /// <summary> スキル選択時の処理 </summary>
        private void OnSelectedSkill(ScrollGridItem item)
        {
            // リスト格納番号
            int index = item.ItemId;
            // 選択時の処理
            if (selectedSkillEvent != null)
            {
                selectedSkillEvent(index);
            }
        }
    }
}