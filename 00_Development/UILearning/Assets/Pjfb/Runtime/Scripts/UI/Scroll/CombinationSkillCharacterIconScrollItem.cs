using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class CombinationSkillCharacterIconScrollItem : CharacterScrollItem
    {
        // 選択番号ルートオブジェクト
        [SerializeField]
        private GameObject selectionNumberRoot = null;
        
        // 選択番号
        [SerializeField]
        private TMP_Text selectionNumber = null;
        
        protected override void OnSetView(object value)
        {
            base.OnSetView(value);
            if (characterIcon != null)
            {
                characterIcon.SetIcon(characterData.CharacterId, characterData.CharacterLv, characterData.LiberationLv);
                characterIcon.CanLiberation = characterData.HasOption(CharacterScrollDataOptions.CanLiberation);
                characterIcon.CanGrowth = characterData.HasOption(CharacterScrollDataOptions.CanGrowth);

                // キャラアイコン詳細表示の可否を設定
                characterIcon.OpenDetailModal = !characterData.HasOption(CharacterScrollDataOptions.DisableDetailModal);

                // レベルを非表示
                characterIcon.SetActiveLv(false);
                // キャラクタータイプアイコンを非表示
                characterIcon.SetActiveCharacterTypeIcon(false);
            }
           
            // 未所持表示
            nonPossessionRoot.SetActive(characterData.HasOption(CharacterScrollDataOptions.NonPossession));
            characterIcon.SwipeableParams = characterData.SwipeableParams;
            characterIcon.GetTrainingScenarioId = characterData.GetTrainingScenarioId;

            SetSelect(characterData.IsSelecting);
        }

        /// <summary> 選択オブジェクトの表示 </summary>
        private void SetSelect(bool isSelect)
        {
            selectingRoot.SetActive(isSelect);
            selectionNumberRoot.SetActive(isSelect);
            selectionNumber.text = characterData.SelectionNumber.ToString();
        }
        
        // スクロール側のアイテム選択イベントで処理するので何もしない
        protected override void OnSelectedItem()
        {
            return;
        }
        
        protected override void OnDeselectItem()
        {
           return;
        }
    }
}
