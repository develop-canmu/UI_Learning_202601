using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using TMPro;

namespace Pjfb
{
    public class AdviserSkillListModalWindow : ModalWindow
    {
        public class ModalParam
        {
            private CharacterDetailData[] deckCharaList;
            public CharacterDetailData[] DeckCharaList => deckCharaList;
            
            public ModalParam(CharacterDetailData[] deckCharaList)
            {
                this.deckCharaList = deckCharaList;
            }
        }
        
        [SerializeField]
        private ScrollGrid[] skillScrollGrid = null;
        
        [SerializeField]
        private TextMeshProUGUI[] emptyText = null;

        private ModalParam modalParam;
        
        // アビリティタイプ
        private readonly BattleConst.AbilityType[] abilityType = new BattleConst.AbilityType[]
        {
            BattleConst.AbilityType.GuildBattleManual,
            BattleConst.AbilityType.GuildBattleAuto,
        };
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalParam = (ModalParam)args;
            InitView();
            return base.OnPreOpen(args, token);
        }

        
        // ビューの更新処理
        private void InitView()
        {
            foreach (int sheetNum in Enum.GetValues(typeof(AdviserAbilityListTabSheetType)))
            {
                List<AdviserSkillCharacterScrollGridItem.ItemParam> itemParams = new List<AdviserSkillCharacterScrollGridItem.ItemParam>();
                foreach (CharacterDetailData characterData in modalParam.DeckCharaList)
                {
                    if(characterData == null) continue;
                    List<CharaAbilityInfo> skillList = CharaAbilityUtility.GetAbilityAcquireList(abilityType[sheetNum], characterData.MChara.id, characterData.Lv, characterData.LiberationLevel);
                    foreach (CharaAbilityInfo skill in skillList)
                    {
                        if(skill.IsAbilityUnLock == false) continue;
                        itemParams.Add(new AdviserSkillCharacterScrollGridItem.ItemParam(characterData, skill));
                    }
                }
                
                emptyText[sheetNum].gameObject.SetActive(itemParams.Count == 0);
                skillScrollGrid[sheetNum].SetItems(itemParams);
            }
        }
        
    }
}