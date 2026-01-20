using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Combination;
using Cysharp.Threading.Tasks;
using System;
using TMPro;

namespace Pjfb
{
    public class CombinationMatchScrollDynamic : CombinationScrollDynamicBase
    {
        [SerializeField] private TextMeshProUGUI noActivatingCombinationTextComponent;
        
        public IReadOnlyList<CombinationManager.CombinationMatch> MatchList => matchList.AsReadOnly(); 
        private List<CombinationManager.CombinationMatch> matchList;
        // マッチスキルのみSortFilterTypeが場所によって異なるため、メンバで可変式で保持しておく
        private SortFilterUtility.SortFilterType sortFilterType = SortFilterUtility.SortFilterType.ListCombinationMatch;
        /// <summary>このクラスで使用するソートフィルタータイプ</summary>
        protected override SortFilterUtility.SortFilterType SortFilterType => sortFilterType;

        /// <summary>ソートフィルターモーダルのタイプ</summary>
        protected override ModalType SortFilterModalType => ModalType.CombinationMatchSortFilter;
        
        // 更新フラグを設定するためのコールバック
        private Action onAppliedUpdateFlagCallback;
        // コールバック更新メソッド
        public void SetUpdateFlagCallback(Action callback)
        {
            onAppliedUpdateFlagCallback = callback;
        }
        
        // 編成キャラのmCharaIdリスト
        private List<long> deckCharaMCharaIdList;
        
        /// <summary>
        /// 全マッチスキルで初期化
        /// </summary>
        public void InitializeAll()
        {
            // 全件スキルリスト取得
            matchList = CombinationManager.GetAllCombinationMatchList();
            // 編成キャラリストをクリア
            deckCharaMCharaIdList = null;
            // 描画更新
            RefreshWithSortFilter();
        }
        
        /// <summary>
        /// 編成キャラから発動可能なマッチスキルで初期化
        /// </summary>
        /// <param name="idList">編成キャラのmCharaIdリスト</param>
        public void InitializeWithDeckCharacters(List<long> idList)
        {
            // 発動するスキルリスト取得
            matchList = CombinationManager.GetActivatingCombinationMatchList(idList);
            // 編成キャラのmCharaIdリストを保持
            deckCharaMCharaIdList = idList;
            // 描画更新
            RefreshWithSortFilter();
        }

        /// <summary>
        /// ソートフィルター設定を適用してスクロールを更新する
        /// </summary>
        public override void RefreshWithSortFilter()
        {
            // matchListがもしnullの場合は初期化処理に回す
            if (matchList == null)
            {
                InitializeAll();
                return;
            }
            
            // セーブデータ取得
            CombinationSkillFilterData filterData = SortFilterUtility.GetFilterDataByType(SortFilterType) as CombinationSkillFilterData;
            CombinationSkillSortData sortData = SortFilterUtility.GetSortDataByType(SortFilterType) as CombinationSkillSortData;
            if (filterData == null || sortData == null)
            {
                CruFramework.Logger.LogError($"RefreshWithSortFilter: フィルターかソートのデータがNullです, sortFilterType={SortFilterType}");
                return;
            }
            
            // フィルター適用したスキルリスト
            List<CombinationManager.CombinationMatch> filteredUserCharaDataList = matchList.GetFilterCombinationMatchList(SortFilterType, filterData.selectedCharaIdList);
            // フィルター適用後にソート適用したスキルリスト
            List<CombinationManager.CombinationMatch> sortedUserCharaDataList = filteredUserCharaDataList.GetSortCombinationMatchList(SortFilterType);
            // スクロールデータ作成
            List<CombinationMatchScrollData> scrollData = sortedUserCharaDataList.Select(combinationMatch => new CombinationMatchScrollData(combinationMatch)).ToList();
            
            // スクロール更新
            scrollDynamic.SetItems(scrollData);
            
            // ソート・フィルター表示を更新
            UpdateSortText();
            UpdateFilterText();
            
            // テキスト更新
            bool hasScrollData = scrollData.Count != 0;
            noActivatingCombinationText.SetActive(!hasScrollData);
            
            // データが無い場合、フィルター状態に応じてメッセージを切り替える
            if (!hasScrollData)
            {
                UpdateNoActivatingText();
            }
            
            // コールバックが必要な場所では呼ぶ
            onAppliedUpdateFlagCallback?.Invoke();
        }

        /// <summary> スクロールデータがない時に表示するテキストを更新する </summary>
        private void UpdateNoActivatingText()
        {
            bool isFiltering = SortFilterUtility.IsFilter(SortFilterType);
            string stringId = isFiltering ? "combination_filterIcon_noChara" : "common.no_combination_text";
            // テキスト更新
            if (StringValueAssetLoader.Instance.HasKey(stringId))
            {
                noActivatingCombinationTextComponent.text = StringValueAssetLoader.Instance[stringId];
            }
        }

        /// <summary> プレイヤーチームかどうかでSortFilterTypeを切り替える </summary>
        /// <param name="isPlayerDeck"></param>
        public void ChangeSortFilterType(bool isPlayerDeck)
        {
            if (isPlayerDeck)
            {
                sortFilterType = SortFilterUtility.SortFilterType.PlayerCombinationMatch;
            }
            else
            {
                sortFilterType = SortFilterUtility.SortFilterType.EnemyCombinationMatch;
            }
        }
        
        protected override SortFilterBaseModal<CombinationSkillSortData, CombinationSkillFilterData>.Data CreateSortFilterModalData()
        {
            // 編成キャラが設定されていない場合は通常のデータを返す
            if (deckCharaMCharaIdList == null || deckCharaMCharaIdList.Count == 0)
            {
                
                return new BaseCombinationSkillSortFilterModal<CombinationManager.CombinationMatch>.Data(
                    SortFilterSheetType.Filter, 
                    SortFilterType, 
                    GetDisplayCharacterIdList(matchList));
            }

            return new BaseCombinationSkillSortFilterModal<CombinationManager.CombinationMatch>.Data(
                SortFilterSheetType.Filter,
                SortFilterType,
                deckCharaMCharaIdList);
        }
    }
}
