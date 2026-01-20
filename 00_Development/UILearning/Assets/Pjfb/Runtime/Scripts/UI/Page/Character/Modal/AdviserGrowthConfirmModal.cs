using System.Collections.Generic;
using System.Linq;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    public class AdviserGrowthConfirmModal : CharacterGrowthConfirmModal
    {
        // アドバイザースキルルート
        [SerializeField]
        private Transform adviserSkillRoot = null;
        
        // アドバイザースキル表示アイテム
        [SerializeField]
        private AdviserSkillView adviserSkillItem = null;

        // 新規獲得アドバイザースキルがない場合の表示
        [SerializeField]
        private GameObject noNewAdviserSkillText = null;
        
        /// <summary> 初期化処理 </summary>
        protected override void InitializeUi()
        {
            base.InitializeUi();
            // キャラId
            UserDataChara uChara = UserDataManager.Instance.chara.Find(modalData.UserCharacterId);

            // 表示するスキルリスト
            List<CharaAbilityInfo> adviserSkillList = new List<CharaAbilityInfo>();
            //  エールスキルの新規獲得スキル追加
            adviserSkillList.AddRange(CharaAbilityUtility.GetNewAbilityOrLevelUpAbility(BattleConst.AbilityType.GuildBattleManual, uChara.charaId, modalData.CurrentLv, uChara.newLiberationLevel, modalData.AfterLv, uChara.newLiberationLevel));
            // サポートスキルの新規獲得スキル追加
            adviserSkillList.AddRange(CharaAbilityUtility.GetNewAbilityOrLevelUpAbility(BattleConst.AbilityType.GuildBattleAuto, uChara.charaId, modalData.CurrentLv, uChara.newLiberationLevel, modalData.AfterLv, uChara.newLiberationLevel));

            // 新規獲得スキルがあるか
            bool hasNewSkill = adviserSkillList.Count > 0;
            // 獲得スキルが無いなら表示
            noNewAdviserSkillText.gameObject.SetActive(hasNewSkill == false);
            
            // 獲得スキルがないなら後続処理はとばす
            if (hasNewSkill == false)
            {
                return;
            }
            
            // 新規スキルごとにViewを生成
            foreach (CharaAbilityInfo info in adviserSkillList
                         // スキルId昇順
                         .OrderBy(x => x.SkillId)
                         // 解放キャラレベル昇順
                         .ThenBy(x => x.OpenCharaLevel)
                         // 解放限界突破レベル昇順
                         .ThenBy(x => x.OpenLiberationLevel))
            {
               AdviserSkillView skillView = Instantiate(adviserSkillItem, adviserSkillRoot);
               skillView.gameObject.SetActive(true);
               skillView.SetSkillView(info, modalData.AfterLv, uChara.newLiberationLevel, false);
            }
        }
    }
}