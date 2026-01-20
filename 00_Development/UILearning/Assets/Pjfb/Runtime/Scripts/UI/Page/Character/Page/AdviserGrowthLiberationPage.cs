using System;
using System.Collections.Generic;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    public class AdviserGrowthLiberationPage : CharaLevelUpBasePage
    {
        // アドバイザー背景イメージ
        [SerializeField] 
        private CharacterCardBackgroundImage adviserBackgroundImage;
        
        [SerializeField]
        private AdviserNameView nameView = null;

        [SerializeField]
        private AdviserAbilityTabSheetManager sheetManager;

        // エールスキルリスト
        [SerializeField]
        private AdviserSkillListView yellSkillListView = null;
        
        // サポートスキルリスト
        [SerializeField]
        private AdviserSkillListView supportSkillListView = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // シートを開いたときの更新
            sheetManager.OnOpenSheet -= OnOpenSheetView;
            sheetManager.OnOpenSheet += OnOpenSheetView;
            return base.OnPreOpen(args, token);
        }

        /// <summary> 初期化処理 </summary>
        protected override async UniTask InitializePageAsync()
        {
            await base.InitializePageAsync();
            // 親キャラIdで背景イメージをセット
            await adviserBackgroundImage.SetTextureAsync(uChara.ParentMCharaId);
            // 初期タブを設定
            await sheetManager.OpenSheetAsync(sheetManager.FirstSheet, null);
        }

        /// <summary> レベルの適用時 </summary>
        protected override void OnModifyGrowthLevel(long lv)
        {
            // アドバイザースキル更新(現在開いているシートのみ更新する)
            UpdateSkillView(sheetManager.CurrentSheetType, lv, uChara.newLiberationLevel);
            base.OnModifyGrowthLevel(lv);
        }

        /// <summary> 解放レベルの適用時 </summary>
        protected override void OnModifyLiberationLevel(long lv)
        {
            // 解放レベル変更時はレアリティアイコンも変化
            nameView.SetAfterRarity(uChara.id, lv);
            // アドバイザースキル更新(現在開いているシートのみ更新する)
            UpdateSkillView(sheetManager.CurrentSheetType, uChara.level, lv);
            base.OnModifyLiberationLevel(lv);
        }

        /// <summary> スキルビューの更新 </summary>
        private void UpdateSkillView(AdviserAbilityTabSheetType sheetType, long afterCharaLevel, long afterLiberationLevel)
        {
            // 練習能力はベースクラス側でセットされるのでここでは対応しない
            switch (sheetType)
            {
                case AdviserAbilityTabSheetType.YellSkill:
                {
                    // エールスキルセット
                    yellSkillListView.SetAfterView(BattleConst.AbilityType.GuildBattleManual, uChara.MChara.id, uChara.level, uChara.newLiberationLevel, afterCharaLevel, afterLiberationLevel);    
                    break;
                }
                case AdviserAbilityTabSheetType.SupportSkill:
                {
                    // サポートスキルセット
                    supportSkillListView.SetAfterView(BattleConst.AbilityType.GuildBattleAuto, uChara.MChara.id, uChara.level, uChara.newLiberationLevel, afterCharaLevel, afterLiberationLevel);
                    break;
                }
            }
        }

        /// <summary> シートを開いた時の更新 </summary>
        private void OnOpenSheetView(AdviserAbilityTabSheetType sheetType)
        {
            UpdateSkillView(sheetType, growthLiberationTabSheetManager.GrowthAfterLevel, growthLiberationTabSheetManager.LiberationAfterLevel);
        }
        
        /// <summary> 詳細モーダルを開く </summary>
        public override void OnClickDetailButton()
        {
            BaseCharaDetailModalParams param = new BaseCharaDetailModalParams(new SwipeableParams<CharacterDetailData>(detailOrderList, data.CurrentIndex, SetCharacterByIndex));
            CharacterDetailModalBase.Open(ModalType.AdviserDetail, param);
        }

        /// <summary> 名前表示セット </summary>
        protected override async UniTask InitializeNameViewAsync(UserDataChara chara)
        {
            await nameView.InitializeUIAsync(chara);
        }
    }
}