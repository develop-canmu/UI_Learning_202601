using System;
using System.Collections.Generic;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb.Combination
{
    /// <summary>
    /// スキルコネクト選択状態を管理するクラス
    /// </summary>
    public class CombinationSkillSelectionController
    {
        // 全タブ共通の選択状態管理
        private List<long> selectedCharacterIdList = new List<long>();

        public List<long> SelectedCharacterIdList => selectedCharacterIdList;
        
        /// <summary>
        /// 選択状態が変更されたときに呼び出されるコールバック
        /// </summary>
        public Action OnSelectionChanged { get; set; }
        
        // 各スクロールの参照を管理
        private Dictionary<CardType, CombinationSkillCharacterScroll> scrollByCardType = new Dictionary<CardType, CombinationSkillCharacterScroll>();
        
        // 各カードタイプごとの選択状態を管理
        private Dictionary<CardType, HashSet<long>> cardTypeSelections = new Dictionary<CardType, HashSet<long>>();

        private const int MAX_SELECTED_INDEX = 99;
        
        /// <summary>
        /// 各スクロール、カードタイプを登録
        /// </summary>
        public void RegisterScroll(CardType cardType, CombinationSkillCharacterScroll scroll)
        {
            scrollByCardType[cardType] = scroll;
            cardTypeSelections[cardType] = new HashSet<long>();
        }

        /// <summary> 今の選択数 </summary>
        public int GetCurrentSelectionNumber()
        {
            return selectedCharacterIdList.Count;
        }

        /// <summary> 指定キャラの選択番号 </summary>
        public int GetSelectionNumber(long characterId)
        {
            return selectedCharacterIdList.IndexOf(characterId) + 1;
        }
        
        /// <summary> 選択数が最大値かどうか判定 </summary>
        public bool IsSelectionFull()
        {
            return selectedCharacterIdList.Count >= MAX_SELECTED_INDEX;
        }

        /// <summary> 選択キャラIdのセット </summary>
        public void SetSelectionIdList(List<long> selectionCharaId)
        {
            selectedCharacterIdList = selectionCharaId;

            foreach (long mCharaId in selectionCharaId)
            {
               CardType cardType = MasterManager.Instance.charaMaster.FindData(mCharaId).cardType;

               if (cardTypeSelections.TryGetValue(cardType, out HashSet<long> selectIds))
               {
                   selectIds.Add(mCharaId);
               }
            }
        }

        /// <summary>
        /// キャラアイコン選択
        /// </summary>
        public void SelectCharacter(long characterId, CardType cardType, bool isSelected)
        {
            int selectionNumber = GetSelectionNumber(characterId);
            
            if (isSelected)
            {
                if (selectedCharacterIdList.Contains(characterId) == false)
                {
                    selectedCharacterIdList.Add(characterId);
                    cardTypeSelections[cardType].Add(characterId);
                }
            }
            else
            {
                selectedCharacterIdList.Remove(characterId);
                cardTypeSelections[cardType].Remove(characterId);
                
                // 選択解除された場合は消えた値分を詰める
                foreach (KeyValuePair<CardType, CombinationSkillCharacterScroll> keyValuePairvp in scrollByCardType)
                {
                    keyValuePairvp.Value.FillSelectionNumber(selectionNumber);
                }
            }
            
            // 選択状態が変更されたことを通知
            OnSelectionChanged?.Invoke();
        }
        
        /// <summary>
        /// 指定したカードタイプの選択状態取得
        /// </summary>
        public IReadOnlyCollection<long> GetSelectedCharactersByCardType(CardType cardType)
        {
           return cardTypeSelections[cardType];
        }
      
        /// <summary>
        /// 指定したカードタイプの選択を解除
        /// </summary>
        public void ClearSelectionsByCardType(CardType cardType)
        {
            if (!cardTypeSelections.TryGetValue(cardType, out HashSet<long> selection))
            {
                return;
            }
            
            // カードタイプの選択をクリア
            foreach (long characterId in selection)
            {
                selectedCharacterIdList.Remove(characterId);
            }
            selection.Clear();

            foreach (var pair in scrollByCardType)
            {
                // 選択解除したスクロールは全アイテム非選択に
                if (pair.Key == cardType)
                {
                    pair.Value.Scroll.DeselectAllItems();
                }
                // スクロールデータを更新
                pair.Value.UpdateSelectionScrollData();
            }
            
            // 選択状態が変更されたことを通知
            OnSelectionChanged?.Invoke();
        }
    }
}