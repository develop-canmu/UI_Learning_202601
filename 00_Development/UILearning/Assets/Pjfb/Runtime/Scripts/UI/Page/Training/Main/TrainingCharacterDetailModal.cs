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

namespace Pjfb.Training
{
    public class TrainingCharacterDetailModal : ModalWindow
    {
        
        public class Arguments
        {
            private long characterId = 0;
            /// <summary>mChar</summary>
            public long CharacterId{get{return characterId;}}
            
            private long characterLv = 0;
            /// <summary>Lv</summary>
            public long CharacterLv{get{return characterLv;}}
            
            private long liberationLv = 0;
            /// <summary>Lv</summary>
            public long LiberationLv{get{return liberationLv;}}
            
            private long trainingScenarioId = 0;
            /// <summary>選択したシナリオId</summary>
            public long TrainingScenarioId{get{return trainingScenarioId;}}
            
            private CharacterStatus status = new CharacterStatus();
            /// <summary>ステータス</summary>
            public CharacterStatus Status{get{return status;}}
            
            private List<SkillData> skillDatas = null;
            /// <summary>Skill</summary>
            public  List<SkillData> SkillDatas{get{return skillDatas;}}

            public Arguments(long trainingScenarioId, long characterId, long lv, long liberationLv, CharacterStatus status, List<SkillData> skillDatas)
            {
                this.trainingScenarioId = trainingScenarioId;
                this.characterId = characterId;
                this.characterLv = lv;
                this.status = status;
                this.skillDatas = skillDatas;
                this.liberationLv = liberationLv;
            }
        }
        
        [SerializeField]
        private CharacterStatusView statusView = null;
        [SerializeField]
        private CharacterSkillScrollGrid eventSkillScrollView = null;
        [SerializeField]
        private CharacterGrowthRateGroupView growthRateView = null;
        
        private Arguments arguments = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            arguments = (Arguments)args;
            // ステータス表示
            statusView.SetCharacter(arguments.CharacterId, arguments.CharacterLv, arguments.LiberationLv);
            statusView.StatusView.SetStatus(arguments.Status);
            // 成長率
            growthRateView.SetCharacter( arguments.CharacterId, arguments.CharacterLv);            
            // イベントスキル表
            eventSkillScrollView.SetItems(arguments.SkillDatas);
            
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {

        }
    }
}