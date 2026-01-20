using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework.UI;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine.UI;

namespace Pjfb
{
    public class SupportCharacterDetailModal : ModalWindow
    {
        
        public class Arguments
        {
            private int characterId = 0;
            /// <summary>MChar</summary>
            public int CharacterId{get{return characterId;}}
            
            private int characterLv = 0;
            /// <summary>Lv</summary>
            public int CharacterLv{get{return characterLv;}}
            
            private int liberationLv = 0;
            /// <summary>Lv</summary>
            public int LiberationLv{get{return liberationLv;}}
            
            private int trainingScenarioId = 0;
            /// <summary>選択したシナリオId</summary>
            public int TrainingScenarioId{get{return trainingScenarioId;}}
            
            public Arguments(int trainingScenarioId, int characterId, int lv, int liberationLv)
            {
                this.trainingScenarioId = trainingScenarioId;
                this.characterId = characterId;
                this.characterLv = lv;
                this.liberationLv = liberationLv;
            }
        }
        
        
        [SerializeField]
        private ScrollGrid practiceSkillScroll = null;
        
        [SerializeField]
        private ScrollGrid practiceMenuCardScroll = null;

        [SerializeField]
        private CharacterEventSkillScrollView eventSkillScrollView = null;
        
        [SerializeField]
        private CharacterIcon cahracterIcon = null;
        [SerializeField]
        private TMPro.TMP_Text characterNameText = null;
        [SerializeField]
        private TMPro.TMP_Text characterNickNameText = null;
        
        [SerializeField]
        private SupportCharacterDetailSheetManager sheetManager = null;
        
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments arguments = (Arguments)args;
            // mChar
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(arguments.CharacterId);
            
            // アイコン
            cahracterIcon.SetIcon(arguments.CharacterId, arguments.CharacterLv, arguments.LiberationLv);
            
            // 名前
            characterNickNameText.text = mChar.nickname;
            characterNameText.text = mChar.name;
            // 練習能力
            List<PracticeSkillInfo> practiceSkillDatas = PracticeSkillUtility.GetCharacterPracticeSkill(
                arguments.CharacterId, arguments.CharacterLv,
                arguments.TrainingScenarioId);
            // スクロールにセット
            List<PracticeSkillViewMiniGridItem.Info> practiceSkillDataList = new List<PracticeSkillViewMiniGridItem.Info>();
            foreach(PracticeSkillInfo skill in practiceSkillDatas)
            {
                practiceSkillDataList.Add( new PracticeSkillViewMiniGridItem.Info(skill, false, false) );
            }
            practiceSkillScroll.SetItems(practiceSkillDataList);
            
            
            // 練習メニューカード取得
            long[] cardIds = MasterManager.Instance.trainingCardCharaMaster.FindPracticeCardIds(arguments.CharacterId);
            // スクロールにセット
            practiceMenuCardScroll.SetItems(cardIds);
            
            sheetManager.OpenSheet(SupportCharacterDetailSheetType.PracticeAbility, null);
            // イベントスキル表示
            eventSkillScrollView.SetCharacter(arguments.CharacterId, arguments.CharacterLv, arguments.TrainingScenarioId, CharacterEventSkillScrollView.CharacterType.Support);
            
            return base.OnPreOpen(args, token);
        }
    }
}