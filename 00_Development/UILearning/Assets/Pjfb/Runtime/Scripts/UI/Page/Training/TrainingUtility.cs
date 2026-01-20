using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CruFramework.Adv;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UserData;
using UnityEngine;
using Pjfb.UI;
using Pjfb.Adv;
using WrapperIntList = Pjfb.Networking.App.Request.WrapperIntList;

namespace Pjfb.Training
{
    public static class TrainingUtility
    {
        
        public const int API_ERROR_CODE_FRIEND_LIMIT = 93002;
        public const int API_ERROR_CODE_DATA_ERROR = 93003;
        
        public enum SupportCharacterType
        {
            TrainingChar  = 0,
            Normal  = 1,
            Friend  = 2,
            Special = 3,
            Add     = 4,
            Equipment = 5
        }
        
        public enum FriendFollowType
        {
            None         = 0,
            Follow       = 1,
            Followed     = 2,
            MutualFollow = 3
        }
        
        public enum PlayerType
        {
            Player = 1,
            Npc = 2
        }
        
        public enum CharacterTableType
        {
            TrainingCharacter = 2,
            Npc = 3,
        }
        
        public enum TargetState
        {
            Completed = 1,
            Progressing = 2,
            Failed = 3,
            
            /// <summary>Unity側で追加したステータス</summary>
            CurrentTarget = 9999,
        }
        
        public enum TargetResult
        {
            None = 0,
            Success = 1,
            Failed = 2,
        }

        public enum ScenarioSkipMode
        {
            Enable = 1,
            Disable = 2,
        }

        /// <summary> インスピレーションタイプ </summary>
        public enum InspirationType
        {
            Normal = 1,
            Flow = 2,
        }
        
        /// <summary> トレーニングのコンディションタイプ </summary>
        public enum TrainingConditionType
        {
            AWFUL = 0,
            NOTBAD = 1,
            GOOD = 2,
            BEST = 3,
            EXTREME = 4,
            AWAKENING = 5,
            
            // FlowはTierの概念から外れるので値を放しておく
            FLOW = 100,
        }
        
        /// <summary> タイプごとのインスピレーションデータ </summary>
        public class InspirationTypeData
        {
            // インスピレーションリスト(１回の表示は配列で表示するので)
            private TrainingInspire[] inspireList = null;
            public TrainingInspire[] InspireList => inspireList;

            private InspirationType type;
            public InspirationType Type => type;
            
            public InspirationTypeData(TrainingInspire[] inspireList, InspirationType type)
            {
                this.inspireList = inspireList;
                this.type = type;
            }
        }
        
        /// <summary> スキルカテゴリごとのデータ </summary>
        public class AbilityCategoryData
        {
            private TrainingAbility skill = null;
            public TrainingAbility Skill => skill;

            private AbilityMasterObject.AbilityCategory category;
            public AbilityMasterObject.AbilityCategory Category => category;
            
            public AbilityCategoryData(TrainingAbility skill, AbilityMasterObject.AbilityCategory category)
            {
                this.skill = skill;
                this.category = category;
            }
        }
        
        /// <summary>成功</summary>
        public const int ResponseSuccess = 0;
        /// <summary>データなし</summary>
        public const int ResponseNull = 99;
        
        /// <summary>トレーニング終了コード</summary>
        public const int EndCode = 99;
        /// <summary>トレーニング強制終了コード</summary>
        public const int ForceEndCode = 98;
        
        /// <summary>選択肢がなかった場合の結果</summary>
        public const int ChoiceNullResult = -1;
        /// <summary>休憩時のAPIに投げる番号</summary>
        public const int ProgressRest = -3;
        /// <summary>レベニューマッチのAPIに投げる番号</summary>
        public const int RevenueMatch = -4;
        
        /// <summary>育成キャラがサポートとして参加するレベル</summary>
        public const int TrainingCharacterSupportJoinLv = 100;
        
        /// <summary>Advのテキストに表示するしきいち</summary>
        public const int InspirationBoostAdvTextThreshold = 10000;
        
        public static readonly CharacterStatusType[] StatusUpTypes = new CharacterStatusType[]
        {
            CharacterStatusType.Stamina, CharacterStatusType.Speed, CharacterStatusType.Physical, CharacterStatusType.Technique, CharacterStatusType.Intelligence, CharacterStatusType.Kick
        };
        
        /// <summary>バトル結果</summary>
        public enum BattleResultType
        {
            Win  = 1,
            Lose = 2,
            Draw = 3,
        }
        
        public enum TimingType
        {
            /// <summary>ターン前</summary>
            BeforeTurn = 1001, 
            /// <summary>行動</summary>
            Action = 2001, 
            /// <summary>休憩とバトル</summary>
            RestAndBattle = 3001, 
            /// <summary>行動後</summary>
            AfterAction = 4001, 
            /// <summary>ターン後</summary>
            AfterTurn = 5001, 
        }
        
        private static TrainingConfig config = null;
        /// <summary>設定ファイル</summary>
        public static TrainingConfig Config{get{return config;}}
        
        private static bool canExit = true;
        /// <summary>退出可能</summary>
        public static bool CanExit{get{return canExit;}set{canExit = value;}}
        

        private static TrainingOverallProgress performanceProgressCache = null;
        /// <summary>
        /// レベルアップ演出に一個前の情報が必要
        /// インゲームに行くときに消えないように保持
        /// </summary>
        public static TrainingOverallProgress PerformanceProgressCache{get{return performanceProgressCache;}set{performanceProgressCache = value;}}
        
        /// <summary>3D演出を再生するか</summary>
        public static bool IsPlay3DEffect
        {
            get
            {
                return (AppConfig.DisplayType)LocalSaveManager.saveData.appConfig.trainingDisplayType == AppConfig.DisplayType.Standard;
            }
        }
        
        public static async UniTask LoadConfig()
        {
            await PageResourceLoadUtility.LoadAssetAsync<TrainingConfig>("Training/TrainingConfig.asset", (v)=>{config = v;}, default);
        }

        /// <summary> 指定のシナリオタイプが有効か？ </summary>
        public static bool IsEnableType(TrainingScenarioType type, long scenarioId)
        {
            TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(scenarioId);
            return mScenario.ScenarioType == type;
        }

        /// <summary>ブーストポイントが有効</summary>
        public static bool IsEnableBoostPoint(long scenarioId)
        {
            TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(scenarioId);
            return mScenario.mTrainingPointId != 0;
        }

        /// <summary> Spブーストが有効 </summary>
        public static bool IsEnableSpBoost(long scenarioId)
        {
            long pointId = MasterManager.Instance.trainingScenarioMaster.FindData(scenarioId).mTrainingPointId;
            return MasterManager.Instance.trainingPointMaster.FindData(pointId).enabledStatusEffectChara;
        }
        
        /// <summary> 引き直しが有効か </summary>
        public static bool IsEnableHandReload(long scenarioId)
        {
            TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(scenarioId);
            return MasterManager.Instance.trainingPointHandResetCostMaster.IsEnableHandReload(mScenario.mTrainingPointId);
        }
        
        /// <summary>パフォーマンスが有効</summary>
        public static bool IsEnableTrainingLv(long scenarioId)
        {
            TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(scenarioId);
            EnhanceLevelMasterObject[] masterObjects = MasterManager.Instance.enhanceLevelMaster.FindByMEnhanceId(mScenario.practiceMEnhanceId);
            return masterObjects.Length > 0;
        }
        
        /// <summary>パフォーマンスが有効</summary>
        public static bool IsEnablePerformace(long scenarioId)
        {
            TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(scenarioId);
            return mScenario.mCharaVariableConditionIdListForConditionEffect.Contains(',');
        }

        /// <summary>コンディションの取得</summary>
        public static TrainingConditionStateData GetConditionState(long trainingScenarioId, long condition, long type)
        {
            TrainingConditionType conditionType = TrainingConditionType.AWFUL;
            
            // conditionのタイプ判定
            switch ((TrainingConditionTierType)type)
            {
                // 通常タイプ
                case TrainingConditionTierType.Normal:
                {
                    var tierList = MasterManager.Instance.trainingConditionTierMaster.FindTierSortedList(trainingScenarioId);

                    foreach (TrainingConditionTierMasterObject master in tierList)
                    {
                        // コンディションの発生最小値を超えてる
                        if (condition >= master.min)
                        {
                            conditionType = (TrainingConditionType)master.ConditionTier;
                            break;
                        }
                    }
                    break;
                }
                // Flow
                case TrainingConditionTierType.Flow:
                {
                    conditionType = TrainingConditionType.FLOW;
                    break;
                }
                default:
                {
                    CruFramework.Logger.LogError($"Not Find ConditionTierType : {type}");
                    break;
                }
            }

            return GetConditionState(conditionType);
        }
        
        /// <summary>コンディションの取得</summary>
        public static TrainingConditionStateData GetConditionState(TrainingConditionType type)
        {
            // 一致するコンディションタイプのConfigデータを返す
            foreach(TrainingConditionStateData c in config.ConditionStateData)
            {
                if(c.ConditionType == type)return c;
            }
            
            CruFramework.Logger.LogError($"Not Find ConditionTier : {type}");
            return config.ConditionStateData[^1];
        }
        
        /// <summary>引き直しが有効か</summary>
        public static bool IsEnableTrainingPracticeCardRedraw(long mTrainingScenarioId)
        {
            if (mTrainingScenarioId > 0)
            {
                TrainingScenarioMasterObject trainingScenarioMasterObject = MasterManager.Instance.trainingScenarioMaster.FindData(mTrainingScenarioId);
                if (trainingScenarioMasterObject.mTrainingPointId > 0)
                {
                    TrainingPointMasterObject trainingPointMasterObject = MasterManager.Instance.trainingPointMaster.FindData(trainingScenarioMasterObject.mTrainingPointId);
                    return trainingPointMasterObject.mTrainingPointHandResetCostGroup > 0;
                }
            }

            return false;
        }
        
        public static CharacterStatus CharVariableToStatus(TrainingCharaVariable variable)
        {
            CharacterStatus result = new CharacterStatus();
            
            result.Stamina      = new BigValue(variable.param1);
            result.Physical     = new BigValue(variable.param2);
            result.Kick         = new BigValue(variable.param4);
            result.Intelligence = new BigValue(variable.param5);
            result.Speed        = new BigValue(variable.spd);
            result.Technique    = new BigValue(variable.tec);
            
            return result;
        }
        
        public static CharacterStatus GetStatus(TrainingEventReward reward)
        {
            CharacterStatus result = new CharacterStatus();
            
            result.Stamina      = new BigValue(reward.param1);
            result.Physical     = new BigValue(reward.param2);
            result.Kick         = new BigValue(reward.param4);
            result.Intelligence = new BigValue(reward.param5);
            result.Speed        = new BigValue(reward.spd);
            result.Technique    = new BigValue(reward.tec);
            
            return result;
        }
        
        public static CharacterStatus GetStatus(TrainingCardReward reward)
        {
            CharacterStatus result = new CharacterStatus();
            
            result.Stamina      = new BigValue(reward.param1);
            result.Physical     = new BigValue(reward.param2);
            result.Kick         = new BigValue(reward.param4);
            result.Intelligence = new BigValue(reward.param5);
            result.Speed        = new BigValue(reward.spd);
            result.Technique    = new BigValue(reward.tec);
            
            return result;
        }
        
        public static CharacterStatus GetStatus(TrainingCardLevelMasterObject target)
        {
            CharacterStatus result = new CharacterStatus();
            
            result.Stamina      = new BigValue(target.param1);
            result.Physical     = new BigValue(target.param2);
            result.Kick         = new BigValue(target.param4);
            result.Intelligence = new BigValue(target.param5);
            result.Speed        = new BigValue(target.spd);
            result.Technique    = new BigValue(target.tec);
            
            return result;
        }

        /// <summary> カードユニオンでの獲得ステータス </summary>
        public static CharacterStatus GetStatus(TrainingUnionCardReward target)
        {
            CharacterStatus result = new CharacterStatus();
            
            result.Stamina      = new BigValue(target.param1);
            result.Physical     = new BigValue(target.param2);
            result.Kick         = new BigValue(target.param4);
            result.Intelligence = new BigValue(target.param5);
            result.Speed        = new BigValue(target.spd);
            result.Technique    = new BigValue(target.tec);
            
            return result;
        }
        
        /// <summary>進行用API</summary>
        public static UniTask<TrainingProgressAPIResponse> ProgressAPI(long value)
        {
            return ProgressAPI(value, new TrainingProgressArgs());
        }
        
        /// <summary>進行用API</summary>
        public static UniTask<TrainingProgressAPIResponse> ProgressAPI(bool scenarioSkip)
        {
            TrainingProgressArgs args = new TrainingProgressArgs();
            args.isScenarioSkip = scenarioSkip ? (long)ScenarioSkipMode.Enable : (long)ScenarioSkipMode.Disable;
            return ProgressAPI(TrainingUtility.ChoiceNullResult, args);
        }
        
        /// <summary>進行用API</summary>
        public static async UniTask<TrainingProgressAPIResponse> ProgressAPI(long value, TrainingProgressArgs args)
        {
            // API
            TrainingProgressAPIRequest request = new TrainingProgressAPIRequest();
            //Token取得
            var token = await Pjfb.Networking.App.APIUtility.ConnectOneTimeTokeRequest();
            request.oneTimeToken = token.oneTimeToken;
            // Post
            TrainingProgressAPIPost post = new TrainingProgressAPIPost();
            post.value = value;
            post.args = args;
            request.SetPostData(post);
            // 実行
            await APIManager.Instance.Connect(request);

            var response = request.GetResponseData();
            
            // レスポンスを返す
            return response;
        }
        
        /// <summary>手札引き直しAPI</summary>
        public static async UniTask<TrainingResetHandAPIResponse> ResetHandAPI()
        {
            TrainingResetHandAPIRequest request = new TrainingResetHandAPIRequest();
            TrainingResetHandAPIPost post = new TrainingResetHandAPIPost();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            TrainingResetHandAPIResponse response = request.GetResponseData();
            return response;
        }
        
        /// <summary>休憩確認表示</summary>
        public static  bool IsConfirmRestModal
        {
            get
            {
                return LocalSaveManager.saveData.appConfig.IsTrainingConfirmRest;
            }
            set
            {
                LocalSaveManager.saveData.appConfig.IsTrainingConfirmRest = value;
            }
        }
        
        /// <summary>練習試合確認表示</summary>
        public static  bool IsConfirmPracticeGameModal
        {
            get
            {
                return LocalSaveManager.saveData.appConfig.IsTrainingConfirmPracticeGame;
            }
            set
            {
                LocalSaveManager.saveData.appConfig.IsTrainingConfirmPracticeGame = value;
            }
        }

        /// <summary>練習カード引き直し確認表示</summary>
        public static bool IsConfirmPracticeCardRedrawModal
        {
            get
            {
                return LocalSaveManager.saveData.appConfig.IsTrainingConfirmPracticeCardRedraw;
            }
            set
            {
                LocalSaveManager.saveData.appConfig.IsTrainingConfirmPracticeCardRedraw = value;
            }
        }
        
        /// <summary>ブーストチャンスエフェクトスキップ</summary>
        public static bool IsSkilBoostChanceEffect
        {
            get
            {
                return LocalSaveManager.saveData.appConfig.IsTrainingSkipBoostChanceEffect;
            }
            set
            {
                LocalSaveManager.saveData.appConfig.IsTrainingSkipBoostChanceEffect = value;
            }
        }
        
        /// <summary>オートモード</summary>
        public static AppAdvAutoMode AutoMode
        {
            get
            {
                return (AppAdvAutoMode)LocalSaveManager.saveData.trainingData.AutoMode;
            }
            set
            {
                LocalSaveManager.saveData.trainingData.AutoMode = (int)value;
            }
        }
        
        /// <summary>ログの保存</summary>
        public static void SaveLog(AdvManager adv)
        {
            LocalSaveManager.saveData.trainingData.AdvLogDatas = adv.MessageLogs.ToArray();
            LocalSaveManager.Instance.SaveData();
        }
        
        /// <summary>ログの読み込み</summary>
        public static void LoadLog(AdvManager adv)
        {
            adv.SetLogs(LocalSaveManager.saveData.trainingData.AdvLogDatas);
        }
        
        /// <summary>ログの初期化</summary>
        public static void DeleteLog()
        {
            LocalSaveManager.saveData.trainingData.AdvLogDatas = new AdvMessageLogData[0];
        }
        
        public static  int LogCount
        {
            get
            {
                return LocalSaveManager.saveData.trainingData.AdvLogDatas == null ? 0 : LocalSaveManager.saveData.trainingData.AdvLogDatas.Length;
            }
        }
        
        /// <summary>コンディション変化のメッセージ</summary>
        public static string GetConditionChangeMessage(long condition)
        {
            // 変化なし
            if(condition == 0)return string.Empty;
            // 変化量
            long absCondition = Math.Abs(condition);
            // メッセージを取得
            for(int i=0;i<config.ConditionMessageThresholdData.Length;i++)
            {
                TrainingConditionMessageThresholdData data = config.ConditionMessageThresholdData[i];
                if(data.Value <= absCondition)
                {
                    string key = condition < 0 ? data.DownMessageKey : data.UpMessageKey;
                    return StringValueAssetLoader.Instance[key];
                }
            }
            
            return string.Empty;
        }
        
        /// <summary>練習カードの強化テーブル</summary>
        public static EnhanceLevelMasterObject[] GetEnhanceMaster(long mEnhanceId)
        {
            return MasterManager.Instance.enhanceLevelMaster.FindByMEnhanceId(mEnhanceId);
        }
        
        /// <summary>練習カードの強化テーブル</summary>
        public static long GetEnhanceMaxLv(long mEnhanceId)
        {
            EnhanceLevelMasterObject[] masterObjects = MasterManager.Instance.enhanceLevelMaster.FindByMEnhanceId(mEnhanceId);
            long maxLv = 0;
            foreach(EnhanceLevelMasterObject master in masterObjects)
            {
                maxLv = Math.Max( master.level, maxLv);
            }
            return maxLv;
        }
        
        /// <summary>練習カードの強化テーブル</summary>
        public static EnhanceLevelMasterObject GetEnhanceMaster(long mEnhanceId, long lv)
        {
            EnhanceLevelMasterObject[] masterObjects = MasterManager.Instance.enhanceLevelMaster.FindByMEnhanceId(mEnhanceId);
            foreach(EnhanceLevelMasterObject master in masterObjects)
            {
                if(master.level == lv)return master;
            }
            return null;
        }
        
        public static void OpenHelpModal()
        {
            HelpModalWindow.WindowParams p = new HelpModalWindow.WindowParams();
            p.categoryList = new List<string>(Config.HelpCategories);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Help, p);
        }
        
        public static void OpenAbordModal(Action onAbord = null)
        {
            OpenAbordModalAsync(onAbord).Forget();
        }
        
        public static async UniTask<CruFramework.Page.ModalWindow> OpenAbordModalAsync(Action onAbord = null)
        {
            ConfirmModalData modalWindow = new ConfirmModalData(
                StringValueAssetLoader.Instance["training.option.abort_title"],
                StringValueAssetLoader.Instance["training.option.abort_message"],
                StringValueAssetLoader.Instance["training.option.abort_caution"],
                new ConfirmModalButtonParams( StringValueAssetLoader.Instance["common.ok"], async (m)=>
                {
                    if(onAbord != null)onAbord();
                    m.SetCloseParameter(true);
                    // データを破棄
                    TrainingAbortAPIRequest abortRequest = new TrainingAbortAPIRequest();
                    await APIManager.Instance.Connect(abortRequest);
                    // モーダルを全て閉じる
                    m.Manager.RemoveTopModalsIgnoreTop((m)=>true);
                    await m.CloseAsync();
                    // ホームへ移動
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, true, null);
                }),
                new ConfirmModalButtonParams( StringValueAssetLoader.Instance["common.cancel"], (m)=> 
                {
                    m.SetCloseParameter(false);
                    m.Close();
                })
            );
            
            return await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, modalWindow);
        }
        
        
        
        public static List<BattleV2Chara> GetBattleCharacterList(BattleV2ClientData clientData, PlayerType playerType)
        {
            
            List<BattleV2Chara> result = new List<BattleV2Chara>();
            foreach(BattleV2Chara c in clientData.charaList)
            {
                BattleV2Player p = clientData.playerList[c.playerIndex - 1];
                if(p.playerType == (int)playerType)
                {
                    result.Add(c);
                }
            }
            return result;
        }
        
        public static WrapperIntList[] CreateBattleRoleList(BattleV2ClientData clientData)
        {
            // RoleList
            List<WrapperIntList> roleList = new List<WrapperIntList>();
            // プレイヤの所持キャラ
            List<BattleV2Chara> playerCharacters = GetBattleCharacterList(clientData, PlayerType.Player);
                
            foreach(BattleV2Chara c in playerCharacters)
            {
                WrapperIntList role = new WrapperIntList();
                role.l = new long[3];
                role.l[0] = c.tableType;
                role.l[1] = c.id;
                role.l[2] = c.roleNumber;
                roleList.Add(role);
            }
                
            // 敵の所持キャラ
            List<BattleV2Chara> enemyCharacters = GetBattleCharacterList(clientData, PlayerType.Npc);

            foreach(BattleV2Chara c in enemyCharacters)
            {
                WrapperIntList role = new WrapperIntList();
                role.l = new long[3];
                role.l[0] = c.tableType;
                role.l[1] = c.id;
                role.l[2] = c.roleNumber;
                roleList.Add(role);
            }
            
            return roleList.ToArray();
        }
        
        public static async UniTask<TrainingBattleStartAPIResponse> BattleStartAPI(BattleV2ClientData clientData)
        {
            // Request
            TrainingBattleStartAPIRequest request = new TrainingBattleStartAPIRequest();
            // Post
            TrainingBattleStartAPIPost post = new TrainingBattleStartAPIPost();
            // Role
            post.idRoleList = CreateBattleRoleList(clientData);
            // OptionValue
            post.deckOptionValue = clientData.playerList[0].optionValue;
            // Request
            request.SetPostData(post);
            // API
            await APIManager.Instance.Connect( request );
            
            return request.GetResponseData();
        }
        
        /// <summary>最大グレードの取得</summary>
        public static long GetMaxGrade(TrainingInspire[] inspirations)
        {
            // 最大グレード
            long grade = 0;
            // 獲得インスピレーション
            foreach(TrainingInspire inspiration in inspirations)
            {
                // インスピレーションマスタ
                TrainingCardInspireMasterObject mInspiration = MasterManager.Instance.trainingCardInspireMaster.FindData( inspiration.id );
                // 練習カードマスタ
                TrainingCardMasterObject mCard = MasterManager.Instance.trainingCardMaster.FindData(inspiration.mTrainingCardId);
                // 最大グレード
                if(grade < mInspiration.grade)
                {
                    grade = mInspiration.grade;
                }
            }
            
            return grade;
        }
        
        /// <summary>取得しているインスピレーションをカードごと、キャラごとに取得</summary>
        public static TrainingInspirationCardList[] GetGetInspirationList(TrainingInspire[] inspirations)
        {
            return inspirations
                // (mCharaId, mTrainingCardId, mTrainingCardCharaId) でグループ化
                .GroupBy(x => (x.mCharaId, x.mTrainingCardId, x.mTrainingCardCharaId))
                // 各グループごとに TrainingInspirationCardList を生成
                .Select(group => new TrainingInspirationCardList(
                    group.Key.mTrainingCardId,
                    group.Key.mCharaId,
                    group.Key.mTrainingCardCharaId,
                    // グループ内の要素を InspirationData 配列に変換
                    group.Select(inspire => new TrainingInspirationCardList.InspirationData(inspire.id, true)).ToArray(), PracticeCardView.DisplayEnhanceUIFlags.DetailLabel
                ))
                // 結果を配列に変換
                .ToArray();
        }
        
        /// <summary>取得しているインスピレーションをカードごと、キャラごとに取得</summary>
        public static TrainingInspirationCardList[] GetGetInspirationList(TrainingInspire[] inspirations, TrainingInspire[] acquiredInspirations)
        {
            // 新規
            TrainingInspirationCardList[] newList = GetGetInspirationList(inspirations);
            // 獲得済み
            TrainingInspirationCardList[] acquiredList = GetGetInspirationList(acquiredInspirations);
            // 結果
            List<TrainingInspirationCardList> result = new List<TrainingInspirationCardList>();
            
            // 新しくデータを生成
            foreach(TrainingInspirationCardList inspiration in newList)
            {
                // インスピレーション
                List<TrainingInspirationCardList.InspirationData> dataList = new List<TrainingInspirationCardList.InspirationData>();
                
                // 新規獲得数
                Dictionary<long, int> newCount = new Dictionary<long, int>();

                // 新規
                foreach(TrainingInspirationCardList.InspirationData inspirationData in inspiration.Inspirations)
                {
                    // 新規獲得数
                    if(newCount.ContainsKey(inspirationData.Id) == false)
                    {
                        newCount.Add(inspirationData.Id, 0);
                    }
                    // カウント
                    newCount[inspirationData.Id]++;
                    
                    dataList.Add( new TrainingInspirationCardList.InspirationData(inspirationData.Id, true) );
                }
                
                // 既存
                foreach(TrainingInspirationCardList acquiredData in acquiredList)
                {
                    // 同じカードとキャラ
                    if(acquiredData.MTrainingCardId == inspiration.MTrainingCardId && acquiredData.MCharId == inspiration.MCharId)
                    {
                        // 既存を追加
                        foreach(TrainingInspirationCardList.InspirationData inspirationData in acquiredData.Inspirations)
                        {
                            // 新規獲得数
                            if(newCount.TryGetValue(inspirationData.Id, out int count))
                            {
                                newCount[inspirationData.Id]--;
                            }
                            
                            count--;
                            if(count < 0)
                            {
                                dataList.Add( new TrainingInspirationCardList.InspirationData(inspirationData.Id, false) );
                            }
                        }
                    }
                }

                // 結果に追加
                result.Add( new TrainingInspirationCardList(inspiration.MTrainingCardId, inspiration.MCharId, inspiration.MTrainingCardCharaId, dataList.ToArray(), PracticeCardView.DisplayEnhanceUIFlags.DetailLabel) );
            }
            
            return result.ToArray();
        }

        /// <summary> 指定タイプのインスピレーションを取得 </summary>
        public static InspirationTypeData GetInspirationType(TrainingInspire[] inspirations, InspirationType type)
        {
            List<TrainingInspire> typeInspirations = new List<TrainingInspire>();
            
            foreach (TrainingInspire inspiration in inspirations)
            {
                if(inspiration.type == (int)type)
                {
                    typeInspirations.Add(inspiration);
                }
            }

            // 該当タイプがないならnullを返す
            if (typeInspirations.Count == 0)
            {
                return null;
            }
            
            return new InspirationTypeData(typeInspirations.ToArray(), type);
        }

        /// <summary> 獲得インスピレーションのキューデータを取得 </summary>
        public static Queue<InspirationTypeData> GetInspirationQueue(TrainingInspire[] inspirations)
        {
            Queue<InspirationTypeData> result = new Queue<InspirationTypeData>();

            // データが無いなら空で返す
            if (inspirations == null || inspirations.Length == 0)
            {
                return result;
            }
            
            // Flow
            InspirationTypeData flowType = GetInspirationType(inspirations, InspirationType.Flow);
            if (flowType != null)
            {
                result.Enqueue(flowType);
            }
            
            // 通常
            InspirationTypeData normalType = GetInspirationType(inspirations, InspirationType.Normal);
            if (normalType != null)
            {
                result.Enqueue(normalType);
            }

            return result;
        }

        /// <summary> 指定カテゴリのスキルを取得 </summary>
        public static List<AbilityCategoryData> GetAbilityCategoryList(TrainingAbility[] abilityList, AbilityMasterObject.AbilityCategory categoryType)
        {
            List<AbilityCategoryData> result = new List<AbilityCategoryData>();
            
            foreach (TrainingAbility ability in abilityList)
            {
               AbilityMasterObject.AbilityCategory category = MasterManager.Instance.abilityMaster.FindData(ability.id).CategoryEnum;
               if (category == categoryType)
               {
                   result.Add(new AbilityCategoryData(ability, categoryType));
               }
            }

            return result;
        }

        /// <summary> 獲得スキルのキューデータを取得 </summary>
        public static Queue<AbilityCategoryData> GetAbilityCategoryQueue(TrainingAbility[] abilityList)
        {
            Queue<AbilityCategoryData> result = new Queue<AbilityCategoryData>();

            // データが無いなら空で返す
            if (abilityList == null || abilityList.Length == 0)
            {
                return result;
            }
            
            // スキルカテゴリごとにキューに追加
            // Flowスキル
            foreach (var abilityCategoryData in GetAbilityCategoryList(abilityList, AbilityMasterObject.AbilityCategory.Flow))
            {
                result.Enqueue(abilityCategoryData);
            }
             
            // 通常スキル
            foreach (var abilityCategoryData in TrainingUtility.GetAbilityCategoryList(abilityList, AbilityMasterObject.AbilityCategory.Normal))
            {
                result.Enqueue(abilityCategoryData);
            }
            
            return result;
        }
        
        /// <summary>メッセージ表示するブースト文字列</summary>
        public static string GetInspirationBoostMessage(long boostValue)
        {
            // しきい値より低いものは表示しない
            if(boostValue <= TrainingUtility.InspirationBoostAdvTextThreshold)return string.Empty;
            
            // 小数点第１位まで
            long value1 = boostValue / 100;
            long value2 = boostValue % 100 / 10;
            return string.Format(StringValueAssetLoader.Instance["training.inspiration_boost__message"], value1, value2);
        }
        
        /// <summary>ターン延長の最大延長数</summary>
        private static long maxAddTurn = 0;
        
        /// <summary>ターン延長の最大延長数の設定</summary>
        public static void SetMaxAddTurn(long maxAddTurnValue)
        {
            maxAddTurn = maxAddTurnValue;
        }

        /// <summary>ターン延長の最大延長数の取得</summary>
        public static long GetMaxAddTurn()
        {
            return maxAddTurn;
        }
    }
}
