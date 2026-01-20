using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;


namespace Pjfb.Combination
{
    /// <summary> スキルを所持するキャラクターアイコンをリスト表示するためのコンポーネントクラス </summary>
    public class CombinationSkillCharacterScroll : UserCharacterScroll
    {
        [SerializeField] 
        private UIButton selectClearAllButton = null;
        // キャラカウントが0の時に表示するテキストコンポーネント
        [SerializeField]
        private TextMeshProUGUI noCharacterTextComponent = null;
    
        private CombinationSkillSelectionController combinationSkillSelectionController = null;

        /// <summary> 未フィルターで表示キャラがいない場合のテキストフォーマットキー </summary>
        protected virtual string FormatStringNoCharaTextKey => "character.no_display_chara";
        
        /// <summary>
        /// 選択管理クラスを登録
        /// </summary>
        public void SetSelectionController(CombinationSkillSelectionController combinationSkillSelectionController)
        {
            this.combinationSkillSelectionController = combinationSkillSelectionController;
        }
        
        /// <summary>
        /// カードタイプに応じたソートデータを作成する
        /// </summary>
        protected override SortDataBase CreateSortDataByCardType()
        {
            return new CombinationSkillCharaIconSortData();
        }
        
        /// <summary>
        /// 表示するキャラクターのIDリストを設定し、表示を更新する
        /// </summary>
        public void SetSkillIconList(IEnumerable<long> iconIds)
        {
            SetCharacterListByIds(iconIds);
            SetCharacterList();
            // 選択済みデータの反映
            UpdateSelectionScrollData();
            Refresh();
            
            OnSelectedItem -= OnSelectItem;
            OnSelectedItem += OnSelectItem;
            SetButtonInteractable();
        }

        private void OnSelectItem(CharacterScrollData data)
        {
            // 今の選択状況に応じて分類
            if (data.IsSelecting == false)
            {
                // 選択数が99になった場合は選択不可に
                if (combinationSkillSelectionController.IsSelectionFull())
                {
                    return;
                }
                SelectedItem(data);
            }
            else
            {
                DeselectedItem(data);
            }
            
            // View更新
            scrollGrid.RefreshItemView();
        }

        public override void SetCharacterList()
        {
            base.SetCharacterList();
            // 選択状況でデータを更新
            UpdateSelectionScrollData();
        }

        /// <summary>
        /// キャラアイコン選択時
        /// </summary>
        private void SelectedItem(CharacterScrollData data)
        {
            int selectNumber = combinationSkillSelectionController.GetSelectionNumber(data.CharacterId);
            // 選択番号をセット(Controller側で選択済みならそのデータで反映)
            data.SelectionNumber = selectNumber > 0 ? selectNumber : combinationSkillSelectionController.GetCurrentSelectionNumber() + 1;
            data.IsSelecting = true;
            combinationSkillSelectionController.SelectCharacter(data.CharacterId, CardType, true);
            SetButtonInteractable();
        }

        /// <summary>
        /// キャラアイコン選択解除時
        /// </summary>
        private void DeselectedItem(CharacterScrollData data)
        {
            data.IsSelecting = false;
            combinationSkillSelectionController.SelectCharacter(data.CharacterId, CardType, false);
            data.SelectionNumber = 0;
            SetButtonInteractable();
        }

        /// <summary> 消えた選択番号分の値を詰める </summary>
        public void FillSelectionNumber(int deselectNumber)
        {
            List<CharacterScrollData> itemList = GetItemList();
            
            foreach (CharacterScrollData scrollData in itemList)
            {
                // 選択した番号より上の番号のものを詰める
                if (scrollData.SelectionNumber > deselectNumber)
                {
                    scrollData.SelectionNumber--;
                }
            }
        }
        
        /// <summary>
        /// ボタンの活性化を変える
        /// </summary>
        private void SetButtonInteractable()
        {
            selectClearAllButton.interactable = combinationSkillSelectionController.GetSelectedCharactersByCardType(CardType).Any();
        }

        /// <summary> 選択状況を反映する </summary>
        public void UpdateSelectionScrollData()
        {
            var selectedCharaIdList = combinationSkillSelectionController.GetSelectedCharactersByCardType(CardType);
            foreach (CharacterScrollData scrollData in characterScrollDataList)
            {
                // 選択済み
                if (selectedCharaIdList.Contains(scrollData.CharacterId))
                {
                    scrollData.IsSelecting = true;
                    scrollData.SelectionNumber = combinationSkillSelectionController.GetSelectionNumber(scrollData.CharacterId);
                }
                else
                {
                    scrollData.IsSelecting = false;
                    scrollData.SelectionNumber = 0;
                }
            }
            SetButtonInteractable();
        }
        
        /// <summary>
        /// 全選択クリアボタンのイベントハンドラー
        /// </summary>
        public void OnClickClearAllButton()
        {
            combinationSkillSelectionController.ClearSelectionsByCardType(CardType);
            scrollGrid.RefreshItemView();
        }
        
        /// <summary>
        /// どのソート・フィルターモーダルを開くかを決定する。
        /// </summary>
        protected override async UniTask<CruFramework.Page.ModalWindow> GetDetermineModalWindow()
        {
            // cardTypeに応じて、開くモーダルの種類と引数を変える
            ModalType modalType = new ModalType();
            CombinationSkillCharaIconSortFilterModal.CombinationSkillCharaIconData args = new CombinationSkillCharaIconSortFilterModal.CombinationSkillCharaIconData(SortFilterSheetType.Filter, sortFilterType, CardType);
    
            switch (CardType)
            {
                case CardType.Character:
                {
                    modalType = ModalType.CharacterIconSortFilter;
                    break;
                }
                case CardType.Adviser:
                {
                    modalType = ModalType.AdviserIconSortFilter;
                    break;
                }
                case CardType.SpecialSupportCharacter:
                {
                    modalType = ModalType.SupportCardSortFilter;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(modalType, args, this.GetCancellationTokenOnDestroy());
        }

        /// <summary> ソートフィルターが掛かったUserDataCharaのリストを返す </summary>
        protected override List<UserDataChara> GetSortFilteredUserDataCharaList(List<UserDataChara> list)
        {
            CombinationSkillCharaIconFilterData filterData = (CombinationSkillCharaIconFilterData)SortFilterUtility.GetFilterDataByType(sortFilterType);
            
            // フィルター処理
            list = list.GetFilterCombinationSkillCharaIconList(sortFilterType, filterData.selectedRarityList);
            // ソート処理
            list = list.GetSortCombinationSkillCharaIconList(sortFilterType);

            return list;
        }

        /// <summary> キャラカウントが0の時に表示するテキストの更新処理 </summary>
        protected override void UpdateNoCharaText()
        {
            // 表示の切り替え
            base.UpdateNoCharaText();
            
            // カウントが0でなければ早期return
            if (GetItemList().Count > 0) return;

            // テキスト内容の切り替え
            noCharacterTextComponent.text = GetNonTextValue();
        }

        /// <summary> フィルター状況によってテキスト内容を切り替える </summary>
        protected virtual string GetNonTextValue()
        {
            // フィルター状況取得
            bool isFiltering = SortFilterUtility.IsFilter(sortFilterType);
            // フィルター中かどうかで表示内容を切り替え
            if (isFiltering)
            {
                string stringKey = "combination_filterIcon_noChara";
                return StringValueAssetLoader.Instance[stringKey];
            }
            else
            {
                return GetFormatStringByCardType();
            }
        }

        /// <summary> フォーマットに沿って文字列を返す </summary>
        protected string GetFormatStringByCardType()
        {
            string formatString = StringValueAssetLoader.Instance[FormatStringNoCharaTextKey];
            // カードタイプ埋め込み
            return string.Format(formatString, GetCardTypeString());
        }
        
        /// <summary> カードタイプの文字列を取得 </summary>
        private string GetCardTypeString()
        {
            switch (CardType)
            {
                case CardType.Character:
                    return StringValueAssetLoader.Instance["common.character"];
                case CardType.Adviser:
                    return StringValueAssetLoader.Instance["common.adviser"];
                case CardType.SpecialSupportCharacter:
                    return StringValueAssetLoader.Instance["common.special_support_card"];
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(CardType), CardType, "未対応のCardTypeです");
            }
        }
    }
}