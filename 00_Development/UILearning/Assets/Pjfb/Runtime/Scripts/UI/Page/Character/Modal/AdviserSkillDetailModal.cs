using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.Character
{
    /// <summary> アドバイザースキル詳細モーダル </summary>
    public class AdviserSkillDetailModal : ModalWindow
    {
        public class Param
        {
            // アビリティタイプ
            private BattleConst.AbilityType abilityType = BattleConst.AbilityType.None;
            public BattleConst.AbilityType AbilityType => abilityType;
            
            // キャラId
            private long mCharaId = 0;
            public long MCharaId => mCharaId;

            // スキルId
            private long skillId = 0;
            public long SkillId => skillId;
            
            // 現在のキャラレベル
            private long charaLevel = 0;
            public long CharaLevel => charaLevel;
            
            // 現在のキャラレベル
            private long liberationLevel = 0;
            public long LiberationLevel => liberationLevel;

            public Param(BattleConst.AbilityType abilityType, long mCharaId, long skillId, long charaLevel, long liberationLevel)
            {
                this.abilityType = abilityType;
                this.mCharaId = mCharaId;
                this.skillId = skillId;
                this.charaLevel = charaLevel;
                this.liberationLevel = liberationLevel;
            }
        }

        // モーダルのタイトル(スキルの発動タイプによって変更)
        [SerializeField]
        private TMP_Text modalTitle = null;
        
        // 現在選択しているアドバイザースキルView
        [SerializeField]
        private AdviserSkillView currentSelectAdviserSkillView;
        
        // スキルの発動タイプ
        [SerializeField]
        private TMP_Text abilityType = null;

        // スキルの最大発動回数
        [SerializeField]
        private TMP_Text maxActivateCount = null;

        // スキルの再使用までに必要なターン数
        [SerializeField]
        private TMP_Text coolDownTurnCount = null;
        
        // アドバイザースキル説明テキスト
        [SerializeField]
        private TMP_Text skillDescription = null;
        
        // レベル上昇ごとのアドバイザースキルリストView
        [SerializeField]
        private AdviserSkillListView adviserLevelUpSkillList = null;

        // レベル上昇スキルリストのタイトル
        [SerializeField]
        private TMP_Text levelUpSkillListTitle = null;
        
        private Param param = null;

        // アビリティスキルリスト
        private List<CharaAbilityInfo> skillList = new List<CharaAbilityInfo>();
        
        // 現在選択されているアドバイザースキル
        private int currentSelectIndex = 0;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            param = (Param)args;
            // Index初期化(0だとIndexが0の時、反応しなくなるので)
            currentSelectIndex = -1;

            // 該当するスキルのスキルレベル毎の情報を取得
            skillList = CharaAbilityUtility.GetLevelUpSkillList(param.AbilityType, param.MCharaId, param.SkillId, param.CharaLevel, param.LiberationLevel);
            
            // 表示スキルが無いなら無視
            if (skillList.Count <= 0)
            {
                return default;
            }
            
            // アビリティタイプ毎にタイトル文言を変更
            modalTitle.text = StringValueAssetLoader.Instance[$"adviser.skill_detail_modal.title.ability_type_{(int)param.AbilityType}"];
            
            // 最初に選択しておくIndex番号
            int firstSelectIndex = 0;
            // 解放済みアビリティリスト
            IEnumerable<CharaAbilityInfo> unlockAbilityList = skillList.Where(x => x.IsAbilityUnLock);
            
            // 解放済みのアビリティがあるなら最大スキルレベルを計算する
            if (unlockAbilityList.Any())
            {
                // 修得済み最大スキルレベル
                long requireMaxSkillLevel = skillList.Where(x => x.IsAbilityUnLock).Max(x => x.SkillLevel);

                for (int i = 0; i < skillList.Count; i++)
                {
                    // 現時点でのスキルレベルを最初の位置にしておく
                    if (skillList[i].SkillLevel == requireMaxSkillLevel)
                    {
                        firstSelectIndex = i;
                        break;
                    }
                }
            }

            // アビリティタイプごとに表示変更
            levelUpSkillListTitle.text = StringValueAssetLoader.Instance[$"adviser.skill_ability_type_{(int)param.AbilityType}"];
            // スキルレベルごとの表示をセット
            adviserLevelUpSkillList.SetView(skillList, param.CharaLevel, param.LiberationLevel);
            // スキル選択時のイベントを登録
            adviserLevelUpSkillList.SetSelectedSkillEvent(UpdateSelectedView);
            // スキルを選択して位置をスクロールする
            adviserLevelUpSkillList.SetSkillSelect(firstSelectIndex, true);
          
            return base.OnPreOpen(args, token);
        }

        /// <summary> 選択しているデータの更新 </summary>
        private void UpdateSelectedView(int index)
        {
            // 選択データが現在選択しているものと同じなら更新しない
            if (currentSelectIndex == index)
            {
                return;
            }
            
            // 現在選択しているアドバイザースキル
            CharaAbilityInfo currentAbilityInfo = skillList[index];
            AbilityMasterObject abilityMaster = currentAbilityInfo.GetAbilityMaster();
            
            // 現在選択しているアドバイザースキルの表示
            currentSelectAdviserSkillView.SetView(currentAbilityInfo, false);
            
            // 発動タイプに応じて文言変更
            abilityType.text = StringValueAssetLoader.Instance[$"ability.activation_type_{(int)currentAbilityInfo.AbilityType}"];
            // 発動可能回数
            maxActivateCount.text = abilityMaster.maxInvokeCount.ToString();
            // 再発動までのターン数
            coolDownTurnCount.text = abilityMaster.coolDownTurnCount.ToString();
            // スキル説明
            skillDescription.text = abilityMaster.description;
            
            // 選択しているIndex更新
            currentSelectIndex = index;
        }
    }
}