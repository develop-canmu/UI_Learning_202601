using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class CharacterPracticeSkillModal : ModalWindow
    {
        public class Data
        {
            public readonly PracticeSkillInfo PracticeSkillData;

            // 効果値の表示をするか
            private bool showValue = true;
            public bool ShowValue => showValue;
            
            public Data(PracticeSkillInfo practiceSkillData, bool showValue)
            {
                PracticeSkillData = practiceSkillData;
                this.showValue = showValue;
            }
        }

        [SerializeField] private PracticeSkillTypeView practiceSkillTypeView;
        
        private Data modalData;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data) args;
            InitializeUi();
            return base.OnPreOpen(args, token);
        }
        
        private void InitializeUi()
        {
            practiceSkillTypeView.SetSkillData(modalData.PracticeSkillData, modalData.ShowValue);
        }
    }
}