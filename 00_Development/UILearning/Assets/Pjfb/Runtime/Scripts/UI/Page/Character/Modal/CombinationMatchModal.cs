using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Character;

namespace Pjfb.Deck
{
    // Todo Implement
    public class CombinationMatchModal : ModalWindow
    {
        [SerializeField] private CombinationMatchScrollDynamic activatingMatchScrollDynamic;
        [SerializeField] private CombinationMatchScrollDynamic allMatchScrollDynamic;
        [SerializeField] private TextMeshProUGUI activatingMatchTabText;
        [SerializeField] private TextMeshProUGUI activatingMatchNoticeText;
        [SerializeField] private CombinationMatchTabSheetManager sheetManager;
        
        #region Params

        public class WindowParams
        {
            public List<long> IdList;
            public Action OnClosed;
            public bool IsPlayerDeck = true;
        }

        #endregion
        private WindowParams _windowParams;

        // 各タブで描画更新が必要かどうか
        private Dictionary<CombinationMatchTabSheetType, bool> updateSheetFlags = new();
        // シートとスクロールの紐付け
        private Dictionary<CombinationMatchTabSheetType, CombinationMatchScrollDynamic> connectionSheetToScroll = new();
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CombinationMatch, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();

            // 更新フラグのセット
            updateSheetFlags.Clear();
            foreach (CombinationMatchTabSheetType type in Enum.GetValues(typeof(CombinationMatchTabSheetType)))
            {
                updateSheetFlags.Add(type, false);
            }
            // シートとスクロールの紐付け
            connectionSheetToScroll.Clear();
            connectionSheetToScroll.Add(CombinationMatchTabSheetType.Activating, activatingMatchScrollDynamic);
            connectionSheetToScroll.Add(CombinationMatchTabSheetType.All, allMatchScrollDynamic);
            
            // コールバック設定（他のタブの更新フラグをONにする）
            activatingMatchScrollDynamic.SetUpdateFlagCallback(() => SetUpdateFlagOn(CombinationMatchTabSheetType.All));
            allMatchScrollDynamic.SetUpdateFlagCallback(() => SetUpdateFlagOn(CombinationMatchTabSheetType.Activating));
            
            // シート展開時のイベントを設定
            sheetManager.OnPreOpenSheet -= UpdateSkillListOnSheet;
            sheetManager.OnPreOpenSheet += UpdateSkillListOnSheet;
            
            return base.OnPreOpen(args, token);
        }

        #region PrivateMethods
        private void Init()
        {
            // SortFilterTypeをチームによって変更
            activatingMatchScrollDynamic.ChangeSortFilterType(_windowParams.IsPlayerDeck);
            allMatchScrollDynamic.ChangeSortFilterType(_windowParams.IsPlayerDeck);
            
            // 編成キャラのみのマッチスキル初期化
            activatingMatchScrollDynamic.InitializeWithDeckCharacters(_windowParams.IdList);
            // 全てのマッチスキル初期化
            allMatchScrollDynamic.InitializeAll();
            int activatingCount = activatingMatchScrollDynamic.MatchList.Count;
            activatingMatchTabText.text = string.Format(StringValueAssetLoader.Instance["deck.combination_match.modal.active_list"], activatingCount);
            activatingMatchNoticeText.text = string.Format(StringValueAssetLoader.Instance["deck.combination_match.modal.notice"], activatingCount);
        }

        /// <summary> リスト更新フラグをONにする </summary>
        private void SetUpdateFlagOn(CombinationMatchTabSheetType type)
        {
            updateSheetFlags[type] = true;
        }

        /// <summary>  </summary>
        private void UpdateSkillListOnSheet(CombinationMatchTabSheetType type)
        {
            // 更新フラグが立っていない場合は無視
            if (!updateSheetFlags[type])
            {
                return;
            }
            
            // リスト更新
            if (connectionSheetToScroll.TryGetValue(type, out var scrollDynamic))
            {
                scrollDynamic.RefreshWithSortFilter();
                // フラグを下す
                updateSheetFlags[type] = false;
            }
        }
        
        #endregion

        #region EventListeners

        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.OnClosed);
        }
        #endregion
       
        
        
    }
}
