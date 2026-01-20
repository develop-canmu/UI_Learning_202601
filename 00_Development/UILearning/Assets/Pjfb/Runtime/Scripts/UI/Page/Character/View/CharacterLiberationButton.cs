using System;
using System.Linq;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    /// <summary> キャラクター解放ボタン </summary>
    public class CharacterLiberationButton: MonoBehaviour
    {
        // アイテムが足りている時のフォーマット
        private static readonly string RequiredCountStringValueKey = "character.base_chara_detail.liberation_required_count";
        // アイテムが足りない時のフォーマット
        private static readonly string ShortageRequiredCountStringValueKey = "character.base_chara_detail.liberation_shortage_required_count";
        
        // 解放可能かに応じて閉じるボタンのテキストを変える
        private static readonly string CloseButtonStringValueKey = "common.close";
        private static readonly string CancelButtonStringValueKey = "common.cancel";
        
        [SerializeField] 
        private UIButton liberationButton = null;
        [SerializeField]
        private TMP_Text closeButtonText = null;
        [SerializeField] 
        private GameObject requiredItemRoot = null;
        [SerializeField] 
        private CharacterPieceIcon characterPieceIcon = null;
        [SerializeField]
        private TMP_Text requiredCountText = null;
        [SerializeField]
        private GameObject shortageAnnounceText = null;

        private CharaMasterObject chara = null;
        // キャラ解放実行時のモーダル側へのコールバック処理
        private Action<object> setCloseParameter = null;
        // 所持キャラピース数
        private long possesionCharaPieceValue = 0;
        // 解放に必要なキャラピース数
        private long requiredCharaPieceValue = 0;
        
        /// <summary> 初期化処理 </summary>
        public bool SetView(CharaMasterObject chara, bool canLiberation, Action<object> setCloseParameter = null)
        {
            this.chara = chara;
            this.setCloseParameter = setCloseParameter;
            // キャラを持っているか
            bool hasChara = UserDataManager.Instance.chara.HaveCharacterWithMasterCharaId(chara.id);
            canLiberation = canLiberation && chara.priceFromPiece > 0 && hasChara == false;
            
            // 必要アイテムルート
            requiredItemRoot.SetActive(canLiberation);
            // 解放ボタンの表示
            liberationButton.gameObject.SetActive(canLiberation);
            closeButtonText.text = canLiberation ? StringValueAssetLoader.Instance[CancelButtonStringValueKey] : StringValueAssetLoader.Instance[CloseButtonStringValueKey];
            
            // 解放が可能でない場合は後続の処理は無視
            if (canLiberation == false)
            {
                return false;
            }
            // ピースアイコンのセット
            characterPieceIcon.SetIconId(chara.id);
            // 所持ピース数
            possesionCharaPieceValue = UserDataManager.Instance.charaPiece.data.Values.FirstOrDefault(data => data.charaId == chara.id)?.value ?? 0;
            // 必要ピース数
            requiredCharaPieceValue = chara.priceFromPiece;
            // 解放に必要なアイテムが足りている
            bool hasEnoughItem = possesionCharaPieceValue >= requiredCharaPieceValue;

            // アイテムが足りているかに応じて文言を変更
            string formatKey = hasEnoughItem ? RequiredCountStringValueKey : ShortageRequiredCountStringValueKey;
            requiredCountText.text = string.Format(StringValueAssetLoader.Instance[formatKey], possesionCharaPieceValue, requiredCharaPieceValue);
            
            // アイテムが足りていないなら注意文言を表示
            shortageAnnounceText.SetActive(hasEnoughItem == false);

            // 足りているならボタンを活性化させる
            liberationButton.interactable = hasEnoughItem;

            return true;
        }

        /// <summary> 解放ボタン処理 </summary>
        public void OnClickPieceToCharaButton()
        {
            var afterPossessionData = possesionCharaPieceValue - requiredCharaPieceValue;
            var modalData = new PieceToCharaConfirmModal.Data(chara.id, possesionCharaPieceValue,
                afterPossessionData, setCloseParameter);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.PieceToCharaConfirm, modalData);
        }

    }
}