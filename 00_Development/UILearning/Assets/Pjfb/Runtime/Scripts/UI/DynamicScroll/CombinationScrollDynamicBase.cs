using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;

namespace Pjfb
{
    /// <summary>
    /// コンビネーション系スクロールの基底クラス
    /// </summary>
    [RequireComponent(typeof(ScrollDynamic))]
    public abstract class CombinationScrollDynamicBase : MonoBehaviour
    {
        [SerializeField] protected ScrollDynamic scrollDynamic;
        [SerializeField] protected GameObject noActivatingCombinationText;
        [SerializeField] protected TextMeshProUGUI sortText;
        [SerializeField] protected TextMeshProUGUI priorityText;
        [SerializeField] protected Image sortIcon;
        [SerializeField] protected TextMeshProUGUI filterText;

        /// <summary>このクラスで使用するソートフィルタータイプ</summary>
        protected abstract SortFilterUtility.SortFilterType SortFilterType{ get; }
        
        /// <summary>ソートフィルターモーダルのタイプ</summary>
        protected abstract ModalType SortFilterModalType { get; }
        
        /// <summary>
        /// ソート・フィルター適用後の更新処理（継承された先のクラスで実装）
        /// </summary>
        public abstract void RefreshWithSortFilter();
        
        /// <summary>
        /// ソートフィルターボタン押下時の処理
        /// </summary>
        public void OnClickSortFilterButton()
        {
            OpenSortFilterModal().Forget();
        }
        
        /// <summary>
        /// ソートボタン押下時の処理（ソート順を反転する）
        /// </summary>
        public void OnClickSortButton()
        {
            CombinationSkillSortData sortData = SortFilterUtility.GetSortDataByType(SortFilterType) as CombinationSkillSortData;
            if (sortData == null)
            {
                CruFramework.Logger.LogError($"OnClickSortButton: ソートのデータがNullです, sortFilterType={SortFilterType}, class={GetType().Name}");
                return;
            }
            
            // ソート順を反転させて保存
            sortData.orderType = SortFilterUtility.GetReversalOrderType(sortData.orderType);
            SortFilterUtility.SaveSortData(SortFilterType, sortData);
            RefreshWithSortFilter();
        }

        /// <summary>
        /// ソートのテキスト更新
        /// </summary>
        protected void UpdateSortText()
        {
            CombinationSkillSortData sortData = SortFilterUtility.GetSortDataByType(SortFilterType) as CombinationSkillSortData;
            if (sortData == null)
            {
                CruFramework.Logger.LogError($"UpdateSortText: ソートのデータがNullです, sortFilterType={SortFilterType}, class={GetType().Name}");
                return;
            }
            
            // テキスト更新
            sortText.text = StringValueAssetLoader.Instance[SortFilterUtility.GetSortOrderKey(sortData.orderType)];
            priorityText.text = StringValueAssetLoader.Instance[SortFilterUtility.GetSortPriorityKey(sortData.priorityType)];
            sortIcon.transform.localScale = new Vector3(1, sortData.orderType == OrderType.Descending ? 1 : -1, 1);

        }

        /// <summary>
        /// 絞り込みしているかのテキスト更新
        /// </summary>
        protected void UpdateFilterText()
        {
            filterText.text = StringValueAssetLoader.Instance[SortFilterUtility.GetIsFilterKey(SortFilterUtility.IsFilter(SortFilterType))];
        }

        /// <summary>
        /// キャラクターIDリストを組み立てる
        /// </summary>
        protected List<long> GetDisplayCharacterIdList(IEnumerable<ICombinationSortable> combinationItems)
        {
            List<long> displayCharacterIdList = new();

            foreach (ICombinationSortable combination in combinationItems)
            {
                IReadOnlyList<long> characterIds = combination.GetCharacterIconIds();
                foreach (long id in characterIds)
                {
                    displayCharacterIdList.Add(id);
                }
            }

            return displayCharacterIdList;
        }

        /// <summary>
        /// ソートフィルターモーダルに渡すデータを作成する
        /// </summary>
        protected virtual SortFilterBaseModal<CombinationSkillSortData, CombinationSkillFilterData>.Data CreateSortFilterModalData()
        {
            return new SortFilterBaseModal<CombinationSkillSortData, CombinationSkillFilterData>.Data(SortFilterSheetType.Filter, SortFilterType);
        }

        /// <summary>
        /// ソートフィルターモーダルを開く
        /// </summary>
        private async UniTask OpenSortFilterModal()
        {
            bool isApplied = false;
            CruFramework.Page.ModalWindow modalWindow = null;
            
            try
            {
                SortFilterBaseModal<CombinationSkillSortData, CombinationSkillFilterData>.Data data = CreateSortFilterModalData();
                modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(SortFilterModalType, data, this.GetCancellationTokenOnDestroy());
                
                // モーダルが閉じられるのを待つ
                isApplied = (bool)await modalWindow.WaitCloseAsync(this.GetCancellationTokenOnDestroy());
   
            }
            catch (OperationCanceledException)
            {
                // モーダルが開いている場合は閉じる
                if (modalWindow != null && !modalWindow.IsClosed())
                {
                    modalWindow.Close();
                }
            }
            
            // 適用が押されていたらリストを更新する
            if (isApplied)
            {
                RefreshWithSortFilter();
            }
        }
    }
}

