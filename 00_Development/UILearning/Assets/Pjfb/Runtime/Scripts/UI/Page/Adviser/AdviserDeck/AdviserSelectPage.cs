using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using UnityEngine;

namespace Pjfb.Adviser
{
    public class AdviserSelectPage : DeckEditUCharaSelectPage
    {
        protected override DeckData CurrentDeckData => AdviserDeckPage.CurrentDeckData;
        protected override DeckListData DeckListData => AdviserDeckPage.DeckListData; 
        
        // スキル表示オブジェクト
        [SerializeField]
        private AdviserAbilityListTabSheetManager adviserSkillSheetManager;

        // エールスキルリスト
        [SerializeField]
        private AdviserSkillListView yellSkillListView = null;
        
        // サポートスキルリスト
        [SerializeField]
        private AdviserSkillListView supportSkillListView = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            adviserSkillSheetManager.OnOpenSheet -= UpdateSkillView;
            adviserSkillSheetManager.OnOpenSheet += UpdateSkillView;
            return base.OnPreOpen(args, token);
        }

        protected override void SetSkillView()
        {
            UpdateSkillView(adviserSkillSheetManager.CurrentSheetType);
        }

        /// <summary> スキルビューの更新 </summary>
        private void UpdateSkillView(AdviserAbilityListTabSheetType sheetType)
        {
            switch (sheetType)
            {
                case AdviserAbilityListTabSheetType.YellSkill:
                {
                    // エールスキル
                    yellSkillListView.SetView(BattleConst.AbilityType.GuildBattleManual, ShowingCharacter.MChara.id, ShowingCharacter.level, ShowingCharacter.newLiberationLevel);    
                    break;
                }
                case AdviserAbilityListTabSheetType.SupportSkill:
                {
                    // サポートスキル
                    supportSkillListView.SetView(BattleConst.AbilityType.GuildBattleAuto, ShowingCharacter.MChara.id, ShowingCharacter.level, ShowingCharacter.newLiberationLevel);
                    break;
                }
            }
        }
        
        /// <summary> 詳細モーダルを開く </summary>
        public override void OnClickCharacterDetail()
        {
            CharacterScrollData showScrollData = GetItemById(ShowingCharacter.charaId);
            BaseCharaDetailModalParams param = new BaseCharaDetailModalParams(showScrollData.SwipeableParams);
            CharacterDetailModalBase.Open(ModalType.AdviserDetail, param);
        }
    }
}