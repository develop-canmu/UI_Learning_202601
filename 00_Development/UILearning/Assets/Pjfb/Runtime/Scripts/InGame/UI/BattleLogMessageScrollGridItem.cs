using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Pjfb.Master;

namespace Pjfb.InGame
{
    public class BattleLogMessageScrollGridItem : ScrollGridItem
    {
        /// <summary> スキルのレアリティ </summary>
        private enum AbilityRarity
        {
            // 白スキル
            White = 1,
            // 金スキル
            Gold = 2,
            // 虹スキル
            Rainbow = 3,
        }
        
        /// <summary> ログアニメーション構造 </summary>
        private struct LogAnimationConfig
        {
            public readonly string TriggerName;
            public readonly int Hash;
            public readonly int FrameIndex;

            public LogAnimationConfig(string triggerName, int hash, int frameIndex)
            {
                TriggerName = triggerName;
                Hash = hash;
                FrameIndex = frameIndex;
            }
        }
        
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI textWithIcon;
        [SerializeField] private CharacterInGameIconImage characterIcon;
        [SerializeField] private Sprite[] frameTextures;
        [SerializeField] private List<Image> frameImages;

        protected Animator Animator => animator;
        protected CharacterInGameIconImage CharacterIcon => characterIcon;
        protected TextMeshProUGUI Text => text;
        protected TextMeshProUGUI TextWithIcon => textWithIcon;
        protected Sprite[] FrameTextures => frameTextures;
        protected List<Image> FrameImages => frameImages;
        
        // 通常ログアニメーション設定
        private static readonly LogAnimationConfig DefaultLogConfig = new LogAnimationConfig("LogNormal", Animator.StringToHash("Base Layer.LogNormal"), 0);
        // 金色スキルログアニメーション設定
        private static readonly LogAnimationConfig Skill1LogConfig = new LogAnimationConfig("LogSkill001", Animator.StringToHash("Base Layer.LogSkill001"), 1);
        // 虹色スキルログアニメーション設定
        private static readonly LogAnimationConfig Skill2LogConfig = new LogAnimationConfig("LogSkill002", Animator.StringToHash("Base Layer.LogSkill002"), 2);
        // 黒色スキルログアニメーション設定
        private static readonly LogAnimationConfig Skill3LogConfig = new LogAnimationConfig("LogSkill003", Animator.StringToHash("Base Layer.LogSkill003"), 3);
        

        protected override void OnSetView(object value)
        {
            var data = (BattleLog)value;
            Sprite sprite = data.OffenceSide == BattleDataMediator.Instance.PlayerSide ? frameTextures[0] : frameTextures[1];

            // アニメーション設定を取得（スキル以外は通常ログを設定）
            LogAnimationConfig config = DefaultLogConfig;
            switch (data.AbilityCategory)
            {
                // 通常スキル
                case (long)AbilityMasterObject.AbilityCategory.Normal:
                    // レアリティに応じてアニメーションを切り替える
                    switch (data.Rarity)
                    {
                        case (long)AbilityRarity.White:
                            config = Skill1LogConfig;
                            break;
                        case (long)AbilityRarity.Gold:
                        case (long)AbilityRarity.Rainbow:
                            config = Skill2LogConfig;
                            break;
                    }
                    break;
                
                // FLOWスキル
                case (long)AbilityMasterObject.AbilityCategory.Flow:
                    // 黒色スキルアニメーション固定
                    config = Skill3LogConfig;
                    break;
            }

            // ベース画像をチームによって切り替える
            frameImages[config.FrameIndex].sprite = sprite;

            gameObject.SetActive(true);
            if (BattleDataMediator.Instance.IsSkipToFinish)
            {
                animator.Play(config.Hash, 0, 1.0f);
            }
            else
            {
                animator.SetTrigger(config.TriggerName);
            }
            characterIcon.gameObject.SetActive(false);

            var logText = data.MessageLog;
            // サイド反転ログのリプレイ時は色を反転
            if (!string.IsNullOrEmpty(logText) && BattleDataMediator.Instance.PlayerSide == BattleConst.TeamSide.Right)
            {
                logText = Regex.Replace(logText, $"{BattleConst.AllyTeamStringColorCode}|{BattleConst.EnemyTeamStringColorCode}",
                    match => string.Equals(match.Value, BattleConst.AllyTeamStringColorCode) ? BattleConst.EnemyTeamStringColorCode : BattleConst.AllyTeamStringColorCode);
            }
            
            if (data.IconMCharaId > 0)
            {
                characterIcon.gameObject.SetActive(true);
                characterIcon.SetTextureAsync(data.IconMCharaId).Forget();
                textWithIcon.text = logText;
                textWithIcon.gameObject.SetActive(true);
                text.gameObject.SetActive(false);
            }
            else
            {
                text.text = logText;
                text.gameObject.SetActive(true);
                textWithIcon.gameObject.SetActive(false);
            }
        }
    }
}