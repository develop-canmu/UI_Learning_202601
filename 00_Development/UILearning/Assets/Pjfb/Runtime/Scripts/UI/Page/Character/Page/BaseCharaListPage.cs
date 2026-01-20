using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    /// <summary> キャラクターリスト(キャラ解放機能) </summary>
    public class BaseCharaListPage : CharacterListBasePage
    {
        [SerializeField] protected CharacterGetEffect characterGetEffect = null;
        private long selectingMCharaId;

        /// <summary> 詳細モーダルタイプ </summary>
        protected virtual ModalType DetailModalType => ModalType.BaseCharacterDetail;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // キャラクターリストを更新
            UpdateCharacterList();
            return base.OnPreOpen(args, token);
        }
        
        
        protected override void OnSelectCharacter(object args)
        {
            CharacterScrollData data = (CharacterScrollData)args;
            // 解放を行った際にリストを更新する必要があるためOpenModalAsyncで開く
            OpenDetailModal(data).Forget();
        }

        protected virtual async UniTask OpenDetailModal(CharacterScrollData data)
        {
            // キャラクタ詳細を開く
            CruFramework.Page.ModalWindow modalWindow =
                await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(DetailModalType,
                    new BaseCharaDetailModalParams(data.SwipeableParams, true, true));
            selectingMCharaId = data.CharacterId;
            var response = (CharaPieceToCharaAPIResponse)await modalWindow.WaitCloseAsync();
            // キャラ解放をしているかチェック
            if (response == null) return;
            // キャラ解放後にキャラクターリストを更新
            UpdateCharacterList();
            characterScroll.Scroll.DeselectAllItems();
            // 演出前にヘッダーとフッターを非表示にする
            AppManager.Instance.UIManager.Header.Hide();
            AppManager.Instance.UIManager.Footer.Hide();
            await characterGetEffect.PlayAsync(selectingMCharaId);
            // 演出完了後にヘッダーとフッターを表示にする
            AppManager.Instance.UIManager.Header.Show();
            AppManager.Instance.UIManager.Footer.Show();
            // 自動売却があるかどうかチェック
            var autoSell = response.autoSell;
            if (autoSell.prizeListGot == null || autoSell.prizeListSold == null ||
                (autoSell.prizeListGot.Length <= 0 && autoSell.prizeListSold.Length <= 0)) return;

            var autoSellModalData = new AutoSellConfirmModal.Data(autoSell);
            // 自動売却モーダルを開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.AutoSellConfirm, autoSellModalData);
        }

        /// <summary> キャラクターリストの更新 </summary>
        protected void UpdateCharacterList()
        {
            // ユーザーの所持キャラ未所持キャラのリスト更新
            characterScroll.SetUserCharacterList();
            // キャラ解放可能なキャラをセット(未所持のキャラリストからピースが足りているものから絞り込む)
            characterScroll.SetEnablePieceToCharacterIds(
                characterScroll.NonHasUserCharacterList.Where(data => data.IsPossiblePieceToChara())
                .Select(data => data.charaId).ToList());
            // 最終的な表示するスクロールデータをセット
            characterScroll.SetCharacterList();
            // 所持キャラ数の更新
            possessionCount = UserDataManager.Instance.GetUserDataCharaListByType(characterScroll.CardType).Length;
            // 最大キャラ数の更新
            maxCount = characterScroll.GetListMaxCount();
            // 所持キャラテキストの更新
            SetPossessionText();
            // 表示スクロールを更新
            Refresh();
        }
        
        protected override void OnSwipeDetailModal(CharacterScrollData scrollData)
        {
            selectingMCharaId = scrollData.CharacterId;
            characterScroll.SelectItem(scrollData);
        }
    }
}
