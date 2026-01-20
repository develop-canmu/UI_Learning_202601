using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;
using UnityEngine.Playables;

namespace Pjfb.Training
{
    public class TrainingMainArguments
    {
        
        [Flags]
        public enum Options
        {
            None = 0,
            /// <summary>トレーニング結果画面でボーナスを表示させる</summary>
            ShowBonus = 1 << 0,
            /// <summary>目標の表示ページへ</summary>
            OpenTargetPage = 1 << 1,
            /// <summary>コンセントレーションのエフェクト再生済み</summary>
            ConcentrationEffectEnd = 1 << 2,
            
            // 自動トレーニング完了
            AutoTrainingFinished = 1 << 3,
        }

        /// <summary>レスポンス内の配列の番号</summary>
        private const int CharacterListSupport = 0;
        /// <summary>レスポンス内の配列の番号</summary>
        private const int CharacterListFriend = 1;
        /// <summary>レスポンス内の配列の番号</summary>
        private const int CharacterListSpecialSupport = 2;
        /// <summary>レスポンス内の配列の番号</summary>
        private const int CharacterListAddEvent = 3;
        
        private long code = 0;
        
        /// <summary>トレーニング終了</summary>
        public bool IsEndTraining{get{return code == TrainingUtility.EndCode || code == TrainingUtility.ForceEndCode;}}
        
        private TrainingTrainingEvent trainingEvent = null;
        /// <summary>イベント情報</summary>
        public TrainingTrainingEvent TrainingEvent{get{return trainingEvent;}}
        
        private TrainingPending pending = null;
        /// <summary></summary>
        public TrainingPending Pending{get{return pending;}} 
        
        private TrainingBattlePending battlePending = null;
        /// <summary></summary>
        public TrainingBattlePending BattlePending{get{return battlePending;}}
        
        private CharacterStatus status = new CharacterStatus();
        /// <summary></summary>
        public CharacterStatus Status{get{return status;}}
        
        private TrainingEventReward reward = null;
        /// <summary>報酬</summary>
        public TrainingEventReward Reward{get{return reward;}}
        
        private CharacterStatus rewardStatus = new CharacterStatus();
        /// <summary></summary>
        public CharacterStatus RewardStatus{get{return rewardStatus;}}
        
        private TrainingCharaVariable characterVariable = null;
        /// <summary>キャラ情報</summary>
        public TrainingCharaVariable CharacterVariable{get{return characterVariable;}}
        
        private List<TrainingCharacterData> supportCharacterDatas = new List<TrainingCharacterData>();
        /// <summary>サポートキャラ</summary>
        public  List<TrainingCharacterData> SupportCharacterDatas{get{return supportCharacterDatas;}}
        
        private List<TrainingCharacterData> supportAndFriendCharacterDatas = new List<TrainingCharacterData>();
        /// <summary>サポートとフレンドキャラ</summary>
        public  List<TrainingCharacterData> SupportAndFriendCharacterDatas{get{return supportAndFriendCharacterDatas;}}
        
        private List<TrainingCharacterData> specialSupportCharacterDatas = new List<TrainingCharacterData>();
        /// <summary>サポートキャラ</summary>
        public  List<TrainingCharacterData> SpecialSupportCharacterDatas{get{return specialSupportCharacterDatas;}}
        
        private TrainingCharacterData friendCharacterData = new TrainingCharacterData();
        /// <summary>フレンド</summary>
        public TrainingCharacterData FriendCharacterData{get{return friendCharacterData;}}
        
        private TrainingCharacterData trainingCharacter = new TrainingCharacterData();
        /// <summary>トレーニングキャラ</summary>
        public TrainingCharacterData TrainingCharacter{get{return trainingCharacter;}}
        
        private TrainingFriend friend = null;
        /// <summary>Friend</summary>
        public TrainingFriend Friend{get{return friend;}}
        
        private FestivalPointProgress festivalPointProgress = null;
        /// <summary>イベント情報</summary>
        public FestivalPointProgress FestivalPointProgress{get{return festivalPointProgress;}}
        
        private FestivalEffectStatus festivalEffectStatus = null;
        /// <summary>イベント情報</summary>
        public FestivalEffectStatus FestivalEffectStatus{get{return festivalEffectStatus;}}

        private MissionUserAndGuild[] missionList = null;
        /// <summary>ミッション情報</summary>
        public MissionUserAndGuild[] MissionList { get { return missionList; } }

        /// <summary>すでにバトル開始APIを叩いた後</summary>
        public bool IsBattleStarted{get{return battlePending.state == 2;}}
        
        private string actionName = string.Empty;
        /// <summary>行動名</summary>
        public string ActionName{get{return actionName;}set{actionName = value;}}
        
        private Options options = Options.None;
        /// <summary>オプション</summary>
        public Options OptionFlags{get{return options;}set{options = value;}}
        
        private long trainingCardId = 0;
        public PlayableDirector activeTimeline;

        /// <summary>トレーニングカードId</summary>
        public long TrainingCardId{get{return trainingCardId;}set{trainingCardId = value;}}
        
        private int selectedTrainingCardIndex = 0;
        /// <summary>トレーニングカードIndex</summary>
        public int SelectedTrainingCardIndex{get{return selectedTrainingCardIndex;}set{selectedTrainingCardIndex = value;}}
        
        private long[] joinSupportCharacters = null;

        /// <summary>練習に参加しているキャラ</summary>
        public long[] JoinSupportCharacters
        {
            get { return joinSupportCharacters; }
            set
            {
                if (value == null)
                {
                    joinSupportCharacters = null;
                    return;
                }
                
                // サポートキャラ
                List<long> supportCharaIdList = new List<long>(value);
                supportCharaIdList.RemoveAll(id => supportAndFriendCharacterDatas.Any(x => x.MCharId == id) == false);
                joinSupportCharacters = supportCharaIdList.ToArray();
            }
        }

        private bool isFromInGame = false;
        /// <summary>インゲームから来た場合</summary>
        public bool IsFromInGame{get{return isFromInGame;}set{isFromInGame = value;}}

        /// <summary>チップ枚数</summary>
        public long CurrentTipCount{get{return characterVariable.hp;}}
        
        private TrainingAutoResultStatus autoTrainingResult = null;
        /// <summary>自動トレーニング結果</summary>
        public TrainingAutoResultStatus AutoTrainingResult{get{return autoTrainingResult;}}
        
        private TrainingAutoPendingStatus autoTrainingPendingStatus = null;
        /// <summary>自動トレーニング情報</summary>
        public TrainingAutoPendingStatus AutoTrainingPendingStatus{get{return autoTrainingPendingStatus;}}
        
        private TrainingAutoUserStatus autoTrainingUserStatus = null;
        /// <summary>自動トレーニング情報</summary>
        public TrainingAutoUserStatus AutoTrainingUserStatus{get{return autoTrainingUserStatus;}}
        
        
        private TrainingPointStatus pointStatus = null;
        /// <summary>ポイント情報</summary>
        public TrainingPointStatus PointStatus{get{return pointStatus;}}
        
        public TrainingMainArgumentsKeeps ArgumentsKeeps { get; private set; }
      
#if UNITY_EDITOR
        private int debugOverrideConcentrationEffectId = -1; 
        /// <summary>
        /// エフェクト確認用のデバッグID
        /// </summary>
        public int DebugOverrideConcentrationEffectId{get{return debugOverrideConcentrationEffectId;}set{debugOverrideConcentrationEffectId = value;}}
        
        private TrainingConcentrationMasterObject.TrainingConcentrationType debugConcentrationEffectType = TrainingConcentrationMasterObject.TrainingConcentrationType.None; 
        /// <summary>
        /// エフェクト確認用のデバッグタイプ
        /// </summary>
        public TrainingConcentrationMasterObject.TrainingConcentrationType DebugConcentrationEffectType{get{return debugConcentrationEffectType;}set{debugConcentrationEffectType = value;}}
#endif
        
        /// <summary>練習カードが最大Lv</summary>
        public bool IsLvMaxPracticeCard(int index)
        {   
            // トレーニングId
            TrainingScenarioMasterObject mTrainingScenario = MasterManager.Instance.trainingScenarioMaster.FindData(pending.mTrainingScenarioId);  
            TrainingCard card = pending.handList[index];
            foreach(TrainingPracticeProgress progress in pending.practiceProgressList)
            {
                if(progress.practiceType == card.practiceType)
                {
                    EnhanceLevelMasterObject nextLevel = TrainingUtility.GetEnhanceMaster(mTrainingScenario.practiceMEnhanceId, progress.level + 1);
                    return nextLevel == null;
                }
            }
            
            return false;
        }
        
        /// <summary>パフォーマンスが最大Lv</summary>
        public bool IsLvMaxPerformance()
        {
            return pending.overallProgress.nextLevelValue <= 0;
        }
        
        /// <summary>成功率を表示するか</summary>
        public bool IsShowBonus()
        {
            return (options & Options.ShowBonus) != Options.None;
        }
        
        /// <summary>目標を表示するか</summary>
        public bool IsOpenTargetPage()
        {
            return (options & Options.OpenTargetPage) != Options.None && CurrentTarget != null;
        }
        
        /// <summary>オブションをチェック</summary>
        public bool HasOption(Options option)
        {
            return (options & option) != Options.None;
        }

        public long[] GetMemberIds()
        {
            List<long> result = new List<long>();
            result.Add( trainingCharacter.MCharId );
            
            
            return result.ToArray();
        }
        
        /// <summary>目標達成数を取得</summary>
        public int GetCompletedTargetCount()
        {
            int count = 0;
            foreach(TrainingGoal target in pending.goalList)
            {
                if(target.state == (int)TrainingUtility.TargetState.Completed)
                {
                    count++;
                }
            }
            return count;
        }
        
        public BattleV2Chara GetTrainingBattleCharacter()
        {
            foreach(BattleV2Chara c in battlePending.clientData.charaList)
            {
                BattleV2Player p = battlePending.clientData.playerList[c.playerIndex - 1];
                if(p.playerType == (int)TrainingUtility.PlayerType.Player && c.tableType == (int)TrainingUtility.CharacterTableType.TrainingCharacter)
                {
                    return c;
                }
            }
            return null;
        }

        public bool ExistsCurrentTarget => pending.nextGoalIndex >= 0;
        
        /// <summary>現在の目標</summary>
        public TrainingGoal CurrentTarget
        {
            get
            {
                return pending.nextGoalIndex < 0 ? null : pending.goalList[pending.nextGoalIndex];
            }
        }
        
        public long CurrentTargetRestTurn
        {
            get
            {
                return pending.nextGoalIndex < 0 ? 0 : pending.goalList[pending.nextGoalIndex].restTurnCount;
            }
        }
        
        public long CurrentTargetTurn
        {
            get
            {
                return pending.nextGoalIndex < 0 ? 0 : pending.goalList[pending.nextGoalIndex].turnCountDiff;
            }
        }
        
        /// <summary>目標が終わったか</summary>
        public bool IsCompleteTarget()
        {
            return reward != null && reward.hasAchievedGoal != (long)TrainingUtility.TargetResult.None;
        }
        
        /// <summary練習のLvup</summary>
        public bool IsLvupPractice
        {
            get
            {
                if(TrainingCardId <= 0 || selectedTrainingCardIndex < 0 || pending.practiceProgressList.Length <= 0)return false;
                return pending.practiceProgressList[selectedTrainingCardIndex].beforeLevel < pending.practiceProgressList[selectedTrainingCardIndex].level;
            }
        }
        
        /// <summaryパフォーマンスのLvup</summary>
        public bool IsLvupParformace
        {
            get
            {
                return pending.overallProgress.beforeLevel < pending.overallProgress.currentLevel;
            }
        }
        
        public TrainingGoal GetTarget(long mEventId)
        {
            foreach(TrainingGoal target in pending.goalList)
            {
                if(target.mTrainingEventId == mEventId)
                {
                    return target;
                }
            }
            return null;
        }
        
        /// <summary>
        /// ステータス上昇・チップ獲得など、何らかの報酬が存在するかどうか
        /// </summary>
        /// <returns></returns>
        public bool AnyReward()
        {
            // スキル取得
            if(reward.getAbilityMapList.Length > 0)
            {
                return true;
            }
            
            // if文に直接条件を書くとIL2CPPでなぜかエラーになるので一度キャッシュ
            bool getInspire = reward.inspireList.Length > 0;
            // インスピ獲得
            if(getInspire)
            {
                return true;
            }
            
            // チップ(パフォーマンス)獲得
            if(reward.hp > 0)
            {
                return true;
            }
            
            // ステータス上昇
            foreach(CharacterStatusType type in Enum.GetValues(typeof(CharacterStatusType)))
            {
                if(rewardStatus[type] != 0)return true;
            }

            // スキル追加獲得
            if (reward.mAbilityTrainingPointStatusList.Length > 0)
            {
                return true;
            }
            
            // ターン延長
            if (reward.turnAddValue > 0)
            {
                return true;
            }

            // ターン延長ポイント変換
            if (reward.pointConvertAddedTurnValue > 0)
            {
                return true;
            }

            // コンディションポイント変換
            if (reward.conditionTierPointValue > 0)
            {
                return true;
            }
            
            return false;
        }

        public bool IsFirstActionTurnCurrent()
        {
            if (!ExistsCurrentTarget) return false;

            var eventType = (TrainingEventType)TrainingEvent.eventType;
            return eventType == TrainingEventType.Action && CurrentTarget.restTurnCount == CurrentTarget.turnCountDiff - 1;
        }

        public bool HasExtraTurnCurrentTarget()
        {
            if (!ExistsCurrentTarget) return false;
            
            return CurrentTarget.addedTurn > 0;
        }
        
        public bool HasContinueExtraTurnCurrentTarget()
        {
            if (!ExistsCurrentTarget) return false;
            if (!HasExtraTurnCurrentTarget()) return false;
            
            return CurrentTarget.addedTurn - CurrentTarget.firstAddedTurn > 0;
        }
        
        public bool IsStartFirstExtraTurnThisTurn()
        {
            if (!ExistsCurrentTarget) return false;
            if (IsNormalTurnCurrent()) return false;
            if (!HasExtraTurnCurrentTarget()) return false;

            return CurrentTarget.restFirstAddedTurnCount == CurrentTarget.firstAddedTurn;
        }
        
        public bool IsStartContinueExtraTurnThisTurn()
        {
            if (!ExistsCurrentTarget) return false;
            if (IsNormalTurnCurrent()) return false;
            if (IsFirstExtraTurnCurrent()) return false;
            if (!HasContinueExtraTurnCurrentTarget()) return false;

            return CurrentTarget.restAllAddedTurnCount == CurrentTarget.addedTurn - CurrentTarget.firstAddedTurn;
        }

        public bool IsNormalTurnCurrent()
        {
            if (!ExistsCurrentTarget) return false;
            if (CurrentTargetRestTurn > 0) return true;
            
            return CurrentTarget.addedTurn <= 0;
        }
        
        public bool IsFirstExtraTurnCurrent()
        {
            if (!ExistsCurrentTarget) return false;
            if (IsNormalTurnCurrent()) return false;
            if (CurrentTarget.restFirstAddedTurnCount > 0) return true;
            
            return CurrentTarget.addedTurn - CurrentTarget.firstAddedTurn <= 0;
        }
        
        public bool IsContinueExtraTurnCurrent()
        {
            if (!ExistsCurrentTarget) return false;
            if (IsNormalTurnCurrent()) return false;
            if (IsFirstExtraTurnCurrent()) return false;
            
            return CurrentTarget.restAllAddedTurnCount > 0;
        }
        
        public void UnlockOpenExtraTurnRightFirstPage()
        {
            ArgumentsKeeps.IsLockOpenExtraTurnRightFirstPage = false;
            
            if (IsFirstActionTurnCurrent())
            {
                ArgumentsKeeps.LatestShowGetExtraTurnRightFirstEffectGoalIndex = Pending.nextGoalIndex;
                ArgumentsKeeps.IsShownGetExtraTurnRightFirstEffect = true;
            }
        }

        public void UnlockOpenExtraTurnLotteryPage()
        {
            ArgumentsKeeps.IsLockOpenExtraTurnLotteryPage = false;

            if (IsFirstExtraTurnCurrent())
            {
                ArgumentsKeeps.LatestShowFirstExtraTurnEffectGoalIndex = Pending.nextGoalIndex;
                ArgumentsKeeps.IsShownFirstExtraTurnEffect = true;
            }

            if (IsContinueExtraTurnCurrent())
            {
                ArgumentsKeeps.LatestShowContinueExtraTurnEffectGoalIndex = Pending.nextGoalIndex;
                ArgumentsKeeps.IsShownContinueExtraTurnEffect = true;
            }
        }
        
        /// <summary>手札にカードがあるか</summary>
        public bool HasTrainingCard(long mTrainingCardId)
        {
            return GetTrainingCardHandIndex(mTrainingCardId) >= 0;
        }
        
        /// <summary>手札にカードがあるか</summary>
        public int GetTrainingCardHandIndex(long mTrainingCardId)
        {
            for(int i=0;i<pending.handList.Length;i++)
            {
                if(pending.handList[i].mTrainingCardId == mTrainingCardId)return i;
            }
            return -1;
        }
        
        /// <summary>コンセントレーション終了ターン</summary>
        public bool IsEndConcentration()
        {
            return pending.isFinishedConcentration;
        }

        /// <summary> 再生するコンセントレーション演出があるか？ </summary>
        public bool HasConcentrationEffect()
        {
            return GetConcentrationEffectId() > 0;
        }
        
        /// <summary>
        /// エフェクトのId取得
        /// </summary>
        public int GetConcentrationEffectId()
        {
            // デバック演出Idセット時
#if UNITY_EDITOR
            if (debugOverrideConcentrationEffectId > 0)
            {
                return debugOverrideConcentrationEffectId;
            }
#endif

            // 終了している
            if (IsEndConcentration()) return -1;
            
            // マスタ取得
            TrainingConcentrationMasterObject master = GetActiveConcentrationMaster();

            // 表示マスタがない場合
            if (master == null)
            {
                return -1;
            }
            
            return (int)master.effectNumber;
        }

        /// <summary> コンセントレーションが発動したか？ </summary>
        public bool IsConcentrationReward()
        {
            return reward != null && reward.mTrainingConcentrationId > 0;
        }

        /// <summary> 利用するコンセントレーションのマスタを取得 </summary>
        public TrainingConcentrationMasterObject GetActiveConcentrationMaster()
        {
            long id = -1;
            
            // コンセントレーションの発生報酬があるか？
            if(IsConcentrationReward())
            {
                id = reward.mTrainingConcentrationId;
            }
            else
            {
                 // 表示するコンセントレーションがない場合はnullを返す
                 if (pending.mTrainingConcentrationId <= 0)
                 {
                     return null;
                 }

                 id = pending.mTrainingConcentrationId;
            }

            var master = MasterManager.Instance.trainingConcentrationMaster.FindData(id);

            // 指定先のマスタがない場合は明示的にエラーを出す
            if (master == null)
            {
                throw new Exception($"Not found TrainingConcentrationMasterObject. Id={id}");
            }

            return master;
        }

        /// <summary> インスピレーションが獲得可能か？ </summary>
        public bool EnableInspiration()
        {
           TrainingScenarioType type = MasterManager.Instance.trainingScenarioMaster.FindData(Pending.mTrainingScenarioId).ScenarioType;  
           return type == TrainingScenarioType.Concentration || type == TrainingScenarioType.Flow;
        }
        
        /// <summary>インスピレーションIdを取得</summary>
        public List<long> GetInspirationIds(long mTrainingCardId, long mCharaId)
        {
            List<long> result = new List<long>();
            foreach(TrainingInspire inspire in pending.inspireList)
            {
                if(inspire.mTrainingCardId == mTrainingCardId && inspire.mCharaId == mCharaId)
                {
                    result.Add(inspire.id);
                }
            }
            return result;
        }
        
        /// <summary>コンセントレーションの状態が変わった</summary>
        public bool IsChangeConcentration()
        {
            // 演出済み
            if( HasOption(Options.ConcentrationEffectEnd) )return false;
            
            if(reward != null)
            {
                // 開始
                if(reward.mTrainingConcentrationId > 0)return true;
                // 延長
                if(reward.isConcentrationExtended)return true;
                // グレードアップ
                if(reward.isConcentrationGradeUp)return true;
            }
            
            // 終了
            if(IsEndConcentration())return true;
            
            return false;
        }
        
        /// <summary>ポイントの更新</summary>
        public void UpdatePoint(TrainingPointStatus point)
        {
            pointStatus = point;
        }

        /// <summary>進捗更新</summary>
        public void UpdatePending(TrainingPending trainingPending)
        {
            pending = trainingPending;
        }
        
        /// <summary>インスピレーションのレベルを取得</summary>
        public TrainingCardInspireLevelMasterObject GetInspirationLv(out TrainingCardInspireLevelMasterObject next)
        {
            // 現在のExp
            long exp = pending.inspireExp;
            
            // 次のレベルを初期化
            next = null;
            // マスタをレベル順に取得
            TrainingCardInspireLevelMasterObject[] mLevels = MasterManager.Instance.trainingCardInspireLevelMaster.OrderedByLevel;
            
            // マスタからレベルを取得
            for(int i=0;i<mLevels.Length;i++)
            {
                TrainingCardInspireLevelMasterObject mLevel = mLevels[i];

                if(mLevel.exp <= exp)
                {
                    // 次のレベルを取得
                    if(i > 0)
                    {
                        next = mLevels[i - 1];
                    }
                    return mLevel;
                }
            }

            next = mLevels[mLevels.Length-1];
            return mLevels[mLevels.Length-1];
        }

        /// <summary> Flow状態か?(引数のフラグで発動ターンを無視するか決定)  </summary>
        public bool IsFlow()
        {
            // デバック演出タイプセット時
#if UNITY_EDITOR
            if (debugConcentrationEffectType != TrainingConcentrationMasterObject.TrainingConcentrationType.None)
            {
                return debugConcentrationEffectType == TrainingConcentrationMasterObject.TrainingConcentrationType.Flow;
            }
#endif
            
            // 終了している
            if (IsEndConcentration()) return false;
            
            TrainingConcentrationMasterObject master = GetActiveConcentrationMaster();

            // 表示マスタがない場合
            if (master == null)
            {
                return false;
            }
            
            TrainingConcentrationMasterObject.TrainingConcentrationType type = master.GetConcentrationType();
            
            return type == TrainingConcentrationMasterObject.TrainingConcentrationType.Flow;
        }

        /// <summary> カードユニオンが発生するか？ </summary>
        public bool HasCardUnion()
        {
            if (reward == null) return false;
            if (reward.concentrationUnionCard == null) return false;
            if (reward.concentrationUnionCard.trainingCardList == null) return false;

            return reward.concentrationUnionCard.trainingCardList.Length > 0;
        }

        /// <summary>
        /// トレーニング参加キャラクターの中から、mCharaIdが一致するキャラデータを持ってくる
        /// </summary>
        public TrainingCharacterData GetTrainingCharacterData(long mCharaId)
        {
            // トレーニングに参加しているキャラ一覧
            List<TrainingCharacterData> trainingCharacterList = new List<TrainingCharacterData>();
            trainingCharacterList.Add(TrainingCharacter);
            trainingCharacterList.AddRange(SupportAndFriendCharacterDatas);
            trainingCharacterList.AddRange(SpecialSupportCharacterDatas);
            
            // トレーニングに参加しているキャラからブースト発動キャラを探す
            foreach (TrainingCharacterData charaData in trainingCharacterList)
            {
                if (charaData.MCharId == mCharaId)
                {
                    return charaData;
                }
            }

            CruFramework.Logger.LogError($"編成されていないキャラです {mCharaId}");
            return default;
        }

        private void Initialize(
            long code, string actionName, TrainingTrainingEvent trainingEvent, TrainingPending pending, TrainingBattlePending battlePending, TrainingCharaVariable charVariable,
            TrainingEventReward reward, TrainingFriend friend, FestivalPointProgress festivalPointProgress, FestivalEffectStatus festivalEffectStatus, MissionUserAndGuild[] missionList,
            TrainingPointStatus pointStatus,
            TrainingMainArgumentsKeeps argumentsKeeps,
            Options options
            )
        {
            // リザルト
            this.code = code;
            // 行動名
            this.actionName = string.IsNullOrEmpty(actionName) ? trainingEvent.name : actionName;
            // オプション
            this.options = options;
            // イベント
            this.trainingEvent = trainingEvent;
            // 保留情報
            this.pending = pending;
            this.battlePending = battlePending;
            // キャラ情報
            characterVariable = charVariable;
            // キャラのステータス
            this.status = TrainingUtility.CharVariableToStatus(charVariable);
            // 報酬
            this.reward = reward;
            // フレンド
            this.friend = friend;
            // ポイント
            this.pointStatus = pointStatus;
            
            // イベント情報
            this.festivalPointProgress = festivalPointProgress;
            this.festivalEffectStatus = festivalEffectStatus;
            
            // ミッション進捗情報
            this.missionList = missionList;
            
            // サポートキャラ
            supportCharacterDatas.Clear();
            specialSupportCharacterDatas.Clear();

            foreach(TrainingSupport support in pending.supportDetailList)
            {
                switch( (TrainingUtility.SupportCharacterType)support.supportType)
                {
                    case TrainingUtility.SupportCharacterType.Normal:
                        // サポート
                        supportCharacterDatas.Add( new TrainingCharacterData( support.mCharaId, support.level, support.newLiberationLevel, -1 ) );
                        break;
                    case TrainingUtility.SupportCharacterType.Special:
                        // スペシャルサポート
                        specialSupportCharacterDatas.Add( new TrainingCharacterData( support.mCharaId, support.level, support.newLiberationLevel, -1 ) );
                        break;
                    case TrainingUtility.SupportCharacterType.Friend:
                        // フレンド
                        friendCharacterData = new TrainingCharacterData(support.mCharaId, support.level, support.newLiberationLevel, -1);
                        break;
                    case TrainingUtility.SupportCharacterType.TrainingChar:
                        // 本人
                        trainingCharacter = new TrainingCharacterData(support.mCharaId, support.level, support.newLiberationLevel, -1);
                        break;
                }
            }
            
            supportAndFriendCharacterDatas.Clear();
            supportAndFriendCharacterDatas.AddRange(supportCharacterDatas);
            supportAndFriendCharacterDatas.Add(friendCharacterData);
            
            // 現在の目標のステートを書き換え
            if(pending.nextGoalIndex >= 0)
            {
                pending.goalList[pending.nextGoalIndex].state = (int)TrainingUtility.TargetState.CurrentTarget;
            }
            

            // 報酬ステータス
            if(reward != null)
            {
                rewardStatus = TrainingUtility.GetStatus(reward);
            }

            // カードユニオン発生時はカードの選択がないのでデータを上書きしボーナス値の表示を行う
            if (HasCardUnion())
            {
                TrainingCardId = this.reward.concentrationUnionCard.baseTrainingData.id;
                JoinSupportCharacters = this.reward.concentrationUnionCard.baseTrainingData.mCharaIdList;
                OptionFlags |= Options.ShowBonus;
            }

            ArgumentsKeeps = argumentsKeeps;
            ArgumentsKeeps.UpdateExtraTurnFlags(Pending.nextGoalIndex);
        }
        
        public TrainingMainArguments(TrainingTrainingEvent trainingEvent, TrainingPending pending, TrainingBattlePending battlePending, TrainingCharaVariable charVariable, TrainingEventReward reward, TrainingMainArgumentsKeeps argumentsKeeps, Options options = Options.None)
        {
            Initialize(0, string.Empty, trainingEvent, pending, battlePending, charVariable, reward, null, null, null, null, null, argumentsKeeps, options);
        }
        
        public TrainingMainArguments(TrainingTrainingEvent trainingEvent, TrainingPending pending, TrainingBattlePending battlePending, TrainingCharaVariable charVariable, TrainingEventReward reward, TrainingPointStatus pointStatus, TrainingMainArgumentsKeeps argumentsKeeps, Options options = Options.None)
        {
            Initialize(0, string.Empty, trainingEvent, pending, battlePending, charVariable, reward, null, null, null, null, pointStatus, argumentsKeeps, options);
        }
        
        public TrainingMainArguments(TrainingTrainingEvent trainingEvent, TrainingPending pending, TrainingBattlePending battlePending, TrainingCharaVariable charVariable, TrainingEventReward reward, TrainingFriend friend, FestivalPointProgress festivalPointProgress, FestivalEffectStatus festivalEffectStatus, MissionUserAndGuild[] missionList , TrainingMainArgumentsKeeps argumentsKeeps, Options options = Options.None)
        {
            Initialize(0, string.Empty, trainingEvent, pending, battlePending, charVariable, reward, friend, festivalPointProgress, festivalEffectStatus, missionList, null, argumentsKeeps, options);
        }
        
        
        public TrainingMainArguments(TrainingMainArguments other, TrainingMainArgumentsKeeps argumentsKeeps, Options options = Options.None)
        {
            Initialize(other.code, string.Empty, other.trainingEvent, other.pending, other.battlePending, other.characterVariable, other.reward, other.friend, other.festivalPointProgress, other.FestivalEffectStatus, other.MissionList, other.pointStatus, argumentsKeeps, options);
        }
        
        public TrainingMainArguments(TrainingMainArguments other, TrainingEventReward reward, Options options = Options.None)
        {
            Initialize(other.code, string.Empty, other.trainingEvent, other.pending, other.battlePending, other.characterVariable, reward, other.friend, other.festivalPointProgress, other.FestivalEffectStatus, other.MissionList, other.pointStatus, ArgumentsKeeps == null ? new TrainingMainArgumentsKeeps() : ArgumentsKeeps, options);
        }
        
        public TrainingMainArguments(TrainingProgressAPIResponse response, TrainingMainArgumentsKeeps argumentsKeeps, Options options = Options.None)
        {
            FestivalPointProgress progress = response.festivalPointProgressList == null || response.festivalPointProgressList.Length <= 0 ? null : response.festivalPointProgressList[0];
            FestivalEffectStatus effectStatus = response.festivalEffectStatusList == null || response.festivalEffectStatusList.Length <= 0 ? null : response.festivalEffectStatusList[0];
            Initialize(response.code, string.Empty, response.trainingEvent, response.pending, response.battlePending, response.charaVariable, response.eventReward, response.friend, progress, effectStatus, UserDataManager.Instance.updateMissionList, response.pointStatus, argumentsKeeps, options);
        }
        
        public TrainingMainArguments(TrainingProgressAPIResponse response, string actionName, TrainingMainArgumentsKeeps argumentsKeeps, Options options = Options.None)
        {
            FestivalPointProgress progress = response.festivalPointProgressList == null || response.festivalPointProgressList.Length <= 0 ? null : response.festivalPointProgressList[0];
            FestivalEffectStatus effectStatus = response.festivalEffectStatusList == null || response.festivalEffectStatusList.Length <= 0 ? null : response.festivalEffectStatusList[0];
            Initialize(response.code, actionName, response.trainingEvent, response.pending, response.battlePending, response.charaVariable, response.eventReward, response.friend, progress, effectStatus, UserDataManager.Instance.updateMissionList, response.pointStatus, argumentsKeeps, options);
        }
        
        public TrainingMainArguments(TrainingFinishAutoAPIResponse response, TrainingAutoPendingStatus pendingStatus, TrainingAutoUserStatus userStatus)
        {
            FestivalPointProgress progress = response.festivalPointProgressList == null || response.festivalPointProgressList.Length <= 0 ? null : response.festivalPointProgressList[0];
            FestivalEffectStatus effectStatus = response.festivalEffectStatusList == null || response.festivalEffectStatusList.Length <= 0 ? null : response.festivalEffectStatusList[0];
            Initialize(0, "_", null, response.pending, null, response.charaVariable, null, response.friend, progress, effectStatus, UserDataManager.Instance.updateMissionList, null ,new TrainingMainArgumentsKeeps(), Options.AutoTrainingFinished);
            autoTrainingResult = response.result;
            autoTrainingPendingStatus = pendingStatus;
            autoTrainingUserStatus = userStatus;
        }
    }
}
