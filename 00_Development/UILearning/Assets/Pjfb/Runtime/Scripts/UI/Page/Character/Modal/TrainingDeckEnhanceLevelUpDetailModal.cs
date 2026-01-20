using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;

namespace Pjfb
{
    public class TrainingDeckEnhanceLevelUpDetailModal : ModalWindow
    {
        public class Param
        {
            private long currentLevel;
            private long afterLevel;
            private TrainingDeckEnhanceListData enhanceListData;

            public long CurrentLevel{get => currentLevel;}
            public long AfterLevel{get => afterLevel;}
            public TrainingDeckEnhanceListData EnhanceListData{get => enhanceListData;}
            
            public Param(long currentLevel, long afterLevel, TrainingDeckEnhanceListData enhanceListData)
            {
                this.currentLevel = currentLevel;
                this.afterLevel = afterLevel;
                this.enhanceListData = enhanceListData;
            }
        }
        
        [SerializeField] private TrainingEnhanceDeckLevelUpEffectView levelUpEffectView;
        private Param param;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            param = (Param)args;
            
            levelUpEffectView.ShowLevelUpEnhanceView(param.CurrentLevel, param.AfterLevel, param.EnhanceListData);
            return base.OnPreOpen(args, token);
        }
    }
}