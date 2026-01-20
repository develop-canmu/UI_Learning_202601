using Pjfb.UserData;

namespace Pjfb.Combination
{
    public class CombinationMatchCharacterScroll : CombinationSkillCharacterScroll
    {
        /// <summary> 未フィルターで編成キャラがいない場合のテキストフォーマットのキー </summary>
        protected override string FormatStringNoCharaTextKey => "character.no_deck_chara";
        
        /// <summary> プレイヤーチームかどうかでSortFilterTypeを切り替える </summary>
        public void ChangeCharaIconSortFilterType(SortFilterUtility.SortFilterType type)
        {
            switch (type)
            {
                case SortFilterUtility.SortFilterType.PlayerCombinationMatch:
                    sortFilterType = SortFilterUtility.SortFilterType.PlayerCombinationMatchCharaIcon;
                    break;
                case SortFilterUtility.SortFilterType.EnemyCombinationMatch:
                    sortFilterType = SortFilterUtility.SortFilterType.EnemyCombinationMatchCharaIcon;
                    break;
                case SortFilterUtility.SortFilterType.ListCombinationMatch:
                    sortFilterType = SortFilterUtility.SortFilterType.CombinationMatchCharaIcon;
                    break;
            }
        }
        
        /// <summary> UserDataChara から CharacterScrollData を作成 </summary>
        protected override CharacterScrollData CreateCharacterScrollData(UserDataChara charaData, int scrollIndex, bool isHasCharacter)
        {
            CharacterScrollData characterScrollData = base.CreateCharacterScrollData(charaData, scrollIndex, isHasCharacter);
            
            if (sortFilterType == SortFilterUtility.SortFilterType.EnemyCombinationMatchCharaIcon)
            {
                characterScrollData.AddOption(CharacterScrollDataOptions.DisableDetailModal);
            }
            
            return characterScrollData;
        }

        /// <summary> 表示キャラが居ない場合のテキストの更新 </summary>
        protected override string GetNonTextValue()
        {
            // チーム編成画面かどうか
            if (IsTeamConfirmPage())
            {
                // 編成キャラがいるかどうか
                if (GetListMaxCount() > 0)
                {
                    // フィルター処理 通す
                    return base.GetNonTextValue();
                }
                else
                {
                    // フィルター処理 通さない
                    return GetFormatStringByCardType();
                }
            }
            else
            {
                // フィルター処理 通す
                return base.GetNonTextValue();
            }
        }

        /// <summary> チーム編成画面かどうか </summary>
        private bool IsTeamConfirmPage()
        {
            return sortFilterType == SortFilterUtility.SortFilterType.PlayerCombinationMatchCharaIcon ||
                   sortFilterType == SortFilterUtility.SortFilterType.EnemyCombinationMatchCharaIcon;
        }
    }
}