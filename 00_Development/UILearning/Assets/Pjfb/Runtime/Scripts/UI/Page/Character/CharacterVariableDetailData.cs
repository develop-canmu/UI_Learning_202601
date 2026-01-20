using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb
{
    public class CharacterVariableDetailData
    {
        private const int CharacterLevelEmptyValue = 0;
        private const int TrainingScenarioEmptyId = 0;
        
        private long mCharaId = 0;
        public long MCharaId { get { return mCharaId; } }
        
        private CharacterStatus status;
        /// <summary>ステータス</summary>
        public CharacterStatus Status { get { return status; } }
        
        private SkillData[] abilityList = null;
        /// <summary>スキルリスト</summary>
        public SkillData[] AbilityList { get { return abilityList; } }
        
        private BigValue combatPower = BigValue.Zero;
        /// <summary>戦力</summary>
        public BigValue CombatPower { get { return combatPower; } }
        
        private long rank = 0;
        /// <summary>ランク</summary>
        public long Rank { get { return rank; } }
        
        private long characterLevel;
        /// <summary>
        /// 育成時の強化レベル
        /// </summary>
        public long CharacterLevel { get { return characterLevel; } }

        private long trainingScenarioId;
        /// <summary>
        /// 育成したトレーニングシナリオ
        /// </summary>
        public long TrainingScenarioId { get { return trainingScenarioId; } }

        /// <summary>
        /// 育成時に発動していたコレクションスキル
        /// </summary>
        private IReadOnlyList<CombinationOpenedMinimum> openedCollections;
        public IReadOnlyList<CombinationOpenedMinimum> OpenedCollections { get { return openedCollections; } }
        
        /// <summary>
        /// <para>1	mCharaId	int	mCharaId</para>
        /// <para>2	level	int	レベル</para>
        /// <para>3	newLiberationLevel	int	潜在解放レベル</para>
        /// <para>4	supportType サポートタイプ。0 => 育成キャラ自身、1 => 通常、2 => フレンド、3 => スペシャルサポート、4 => 追加サポートキャラ</para>
        /// </summary>
        public TrainingSupport[] SupportDetailJson { get; private set; }
        public CharacterVariableDetailData(
            long mCharaId, CharacterStatus status, SkillData[] abilityList,
            long characterLevel, long trainingScenarioId,
            IReadOnlyList<CombinationOpenedMinimum> combinationCollections
            )
        {
            SetData(mCharaId, status, abilityList, characterLevel, trainingScenarioId, combinationCollections);
        }
        
        public CharacterVariableDetailData(UserDataCharaVariable uCharaVariable)
        {
            // ステータス
            CharacterStatus status = StatusUtility.ToCharacterStatus(uCharaVariable);
            // スキル
            SkillData[] abilityList = new SkillData[uCharaVariable.abilityList.Length];
            for(int i = 0; i < abilityList.Length; i++)
            {
                abilityList[i] = new SkillData(uCharaVariable.abilityList[i].l[0], uCharaVariable.abilityList[i].l[1]);
            }
            // データセット
            SetData(
                uCharaVariable.charaId, status, abilityList, 
                CharacterLevelEmptyValue, uCharaVariable.mTrainingScenarioId,
                uCharaVariable.comboBuffList,
                uCharaVariable.supportDetailJson
            );
        }
        
        public CharacterVariableDetailData(CharaNpcMasterObject mCharaNpc)
        {
            // ステータス
            CharacterStatus status = StatusUtility.ToCharacterStatus(mCharaNpc);
            // スキル
            SkillData[] abilityList = new SkillData[mCharaNpc.abilityList.Length];
            for(int i = 0; i < abilityList.Length; i++)
            {
                abilityList[i] = new SkillData(mCharaNpc.abilityList[i].l[0], mCharaNpc.abilityList[i].l[1]);
            }
            // データセット
            SetData(
                mCharaNpc.mCharaId, status, abilityList, 
                CharacterLevelEmptyValue, TrainingScenarioEmptyId
            );
        }
        
        public CharacterVariableDetailData(BattleV2Chara battleChar)
        {
            // ステータス
            CharacterStatus status = StatusUtility.ToCharacterStatus(battleChar);
            // スキル
            SkillData[] abilityList = new SkillData[battleChar.abilityList.Length];
            for(int i = 0; i < abilityList.Length; i++)
            {
                abilityList[i] = new SkillData(battleChar.abilityList[i].l[0], battleChar.abilityList[i].l[1]);
            }
            // データセット
            SetData(
                battleChar.mCharaId, status, abilityList, 
                CharacterLevelEmptyValue, TrainingScenarioEmptyId
            );
        }
        
        public CharacterVariableDetailData(CharaVariableRecommendStatus recommendStatus)
        {
            // ステータス
            CharacterStatus status = StatusUtility.ToCharacterStatus(recommendStatus);
            // スキル
            SkillData[] abilityList = new SkillData[recommendStatus.abilityList.Length];
            for(int i = 0; i < abilityList.Length; i++)
            {
                abilityList[i] = new SkillData(recommendStatus.abilityList[i].l[0], recommendStatus.abilityList[i].l[1]);
            }
            // データセット
            SetData(
                recommendStatus.mCharaId, status, abilityList, 
                recommendStatus.level, recommendStatus.mTrainingScenarioId,
                recommendStatus.comboBuffList,
                recommendStatus.supportDetailJson
            );
        }
        
        /// <summary>データセット</summary>
        private void SetData(
            long mCharaId, CharacterStatus status, SkillData[] abilityList,
            long characterLevel, long trainingScenarioId,
            IReadOnlyList<CombinationOpenedMinimum> combinationCollections = null,
            TrainingSupport[] supportDetailJson = null
            )
        {
            this.mCharaId = mCharaId;
            this.status = status;
            this.abilityList = abilityList;
            this.combatPower = StatusUtility.GetCombatPower(status, abilityList);
            this.rank = StatusUtility.GetRank(CharaRankMasterStatusType.CharacterTotal, combatPower);
            this.characterLevel = characterLevel;
            this.trainingScenarioId = trainingScenarioId;
            this.openedCollections = combinationCollections ?? Array.Empty<CombinationOpenedMinimum>();
            this.SupportDetailJson = supportDetailJson;
        }

        public bool HasTrainingInfo() => SupportDetailJson != null && SupportDetailJson.Any();
        public bool HasCharacterLevel() => CharacterLevel != CharacterLevelEmptyValue;
        public bool HasTrainingScenario() => TrainingScenarioId != TrainingScenarioEmptyId;
    }
}
