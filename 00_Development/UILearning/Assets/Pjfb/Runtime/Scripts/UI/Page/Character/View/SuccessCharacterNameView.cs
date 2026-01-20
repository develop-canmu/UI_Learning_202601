using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Character
{
    public class SuccessCharacterNameView : CharacterNameViewBase
    {
        [SerializeField] private CharacterVariableIcon characterVariableIcon;
        [SerializeField] private RankPowerUI rankPowerUI;

        public void InitializeUI(CharacterVariableDetailData chara)
        {
            InitializeUIByMChara(MasterManager.Instance.charaMaster.FindData(chara.MCharaId));
            
            if(rankPowerUI != null)
                rankPowerUI.InitializeCharacterVariableRankUI(chara);
           
            if(characterVariableIcon == null)   return;
            characterVariableIcon.SetIcon(chara);
            characterVariableIcon.SetActiveRoleNumberIcon(false);
        }
        
        public async UniTask InitializeUIAsync(CharacterVariableDetailData chara)
        {
            await InitializeUIByMCharaAsync(MasterManager.Instance.charaMaster.FindData(chara.MCharaId));
            
            if(rankPowerUI != null)
                rankPowerUI.InitializeCharacterVariableRankUI(chara);
           
            if(characterVariableIcon == null)   return;
            characterVariableIcon.SetIcon(chara);
            characterVariableIcon.SetActiveRoleNumberIcon(false);
            characterVariableIcon.SwipeableParams = null;
            
        }
        

        protected override void InitializeUIByMChara(CharaMasterObject mChara)
        {
            base.InitializeUIByMChara(mChara);
            
            if(characterVariableIcon == null)   return;
            characterVariableIcon.SetIconTextureWithEffectAsync(mChara.id).Forget();
        }
        
        protected override async UniTask InitializeUIByMCharaAsync(CharaMasterObject mChara)
        {
            await base.InitializeUIByMCharaAsync(mChara);
            
            if(characterVariableIcon == null)   return;
            characterVariableIcon.SetIconTextureWithEffectAsync(mChara.id).Forget();
        }
    }

}
