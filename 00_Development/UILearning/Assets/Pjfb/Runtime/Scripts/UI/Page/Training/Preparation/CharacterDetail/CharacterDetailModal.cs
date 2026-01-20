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
    public class CharacterDetailModal : ModalWindow
    {
        
        public class Arguments
        {
            private int characterId = 0;
            /// <summary>mChar</summary>
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
        private CharacterStatusView statusView = null;
        [SerializeField]
        private CharacterEventSkillScrollView eventSkillScrollView = null;
        [SerializeField]
        private CharacterGrowthRateGroupView growthRateView = null;
        
        private Arguments arguments = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            arguments = (Arguments)args;
            // ステータス表示
            statusView.SetCharacter(arguments.CharacterId, arguments.CharacterLv, arguments.LiberationLv);
            // 成長率
            growthRateView.SetCharacter( arguments.CharacterId, arguments.CharacterLv);            
            // イベントスキル表
            eventSkillScrollView.SetCharacter(arguments.CharacterId, arguments.CharacterLv, arguments.TrainingScenarioId, CharacterEventSkillScrollView.CharacterType.Main);
            
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {

        }
    }
}