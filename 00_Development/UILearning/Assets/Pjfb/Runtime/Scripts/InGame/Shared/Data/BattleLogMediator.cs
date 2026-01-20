using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
using CodeStage.AntiCheat.ObscuredTypes;
#endif

namespace Pjfb.InGame
{
    // ちょっとここ実装しながら作り変えるかもしれない.
    // 想定としては BoltServer -> BoltClient に MatchUpResultの発行
    // 受け取ったMatchUpResultからLogMediatorにログを積んでいく
    // 積まれたログを逐次UIに反映させていく.
    // 基本的にログの発行される数(積まれる数) > 再生される頻度になって, ダイジェスト発生で時間的な辻褄は合わせる予定.

    public class LogParse
    {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        public List<BattleLog> logData;
        public List<BattleCharacterStatModel> stats;
#else
        public List<BattleLog> logData { get; set; }
        public List<BattleCharacterStatModel> stats { get; set; }
#endif
    }
    
    [Serializable]
    public class BattleDigestLog
    {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        public BattleConst.DigestType Type;
        public BattleDigestCharacterData MainCharacterData;
        public BattleDigestCharacterData MarkCharacterData;
        public List<BattleDigestCharacterData> OtherCharacterDataList = new List<BattleDigestCharacterData>();
        public BattleConst.TeamSide OffenceSide;
        public int DistanceToGoal;
        public List<int> Score = new List<int>();
        public BattleConst.MatchUpDigestType MatchUpDigestType;
        public bool IsLastScoreToEnd;
        public long OffenceAbilityId;
        public BattleConst.AbilityEvaluateTimingType AbilityTiming;
        public int RemainDistanceToShoot;
        public int CommandData;
#else
        public BattleConst.DigestType Type { get; set; }
        public BattleDigestCharacterData MainCharacterData { get; set; }
        public BattleDigestCharacterData MarkCharacterData { get; set; }
        public List<BattleDigestCharacterData> OtherCharacterDataList  { get; set; } =
 new List<BattleDigestCharacterData>();
        public BattleConst.TeamSide OffenceSide { get; set; }
        public int DistanceToGoal { get; set; }
        public List<int> Score { get; set; } = new List<int>();
        public BattleConst.MatchUpDigestType MatchUpDigestType { get; set; }
        public bool IsLastScoreToEnd { get; set; }
        public long OffenceAbilityId { get; set; }
        public BattleConst.AbilityEvaluateTimingType AbilityTiming { get; set; }
        public int RemainDistanceToShoot { get; set; }
        public int CommandData { get; set; }
#endif
    }

    [Serializable]
    public class BattleDigestCharacterData
    {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        public long CharaId;
        public long MCharaId;
        public float StaminaRatio;
        public BattleConst.TeamSide Side;
        public bool IsAce;
        public long AbilityId;
#else
        public long CharaId { get; set; }
        public long MCharaId { get; set; }
        public float StaminaRatio { get; set; }
        public BattleConst.TeamSide Side { get; set; }
        public bool IsAce { get; set; }
        public long AbilityId { get; set; }
#endif

        public void SetData(BattleCharacterModel model)
        {
            CharaId = model.id;
            MCharaId = model.MCharaId;
            StaminaRatio = model.GetStaminaRate();
            Side = model.Side;
            IsAce = model.IsAceCharacter;
        }
    }

    [Serializable]
    public class BattleLog
    {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        public BattleConst.LogTiming LogTiming;
        public BattleConst.TeamSide OffenceSide;
        public string MessageLog;
        public long Rarity;
        public long AbilityCategory;
        public long IconMCharaId;
        public float Delay;
        public BattleDigestLog DigestLog;
        public int BallPosition = -1;
        public bool IsInShootRange;
        public float ElapsedTime;
#else
        public BattleConst.LogTiming LogTiming { get; set; }
        public BattleConst.TeamSide OffenceSide { get; set; }
        public string MessageLog { get; set; }
        public long Rarity { get; set; }
        public long AbilityCategory { get; set; }
        public long IconMCharaId { get; set; }
        public float Delay { get; set; }
        public BattleDigestLog DigestLog { get; set; }
        public int BallPosition { get; set; } = -1;
        public bool IsInShootRange { get; set; }
        public float ElapsedTime { get; set; }
#endif
    }

    public class DigestLog
    {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        public int Index;
        public int ScoreIndex;
        public List<long> ReferencedCharacterIds = new List<long>();
        public List<BattleLog> BattleLogList;
        public BattleCharacterModel PrimaryCharacter;
        public BattleCharacterModel SecondCharacter;
        public bool IsCross;
        public long AbilityId;
#else
        public int Index { get; set; }
        public int ScoreIndex { get; set; }
        public List<long> ReferencedCharacterIds  { get; set; } = new List<long>();
        public List<BattleLog> BattleLogList { get; set; }
        public BattleCharacterModel PrimaryCharacter { get; set; }
        public BattleCharacterModel SecondCharacter { get; set; }
        public bool IsCross { get; set; }
        public long AbilityId { get; set; }
#endif
    }
    
    public class BattleLogMediator
    {
        public BattleLogMediator()
        {
            Instance = this;
        }

        public void Release()
        {
            Instance = null;
        }

        public static BattleLogMediator Instance { get; private set; }

        // 一旦試合の形式をList<string>でキャッシュしていく形でも大丈夫か…? 試合全体でどのぐらいのログ量になるのか未知数…
        // 問題ありそうだったらconstで用意した文字列に対応するKeyと中身の文字列を保存していくような形にするか.
        public List<BattleLog> BattleLogs = new List<BattleLog>();
        public List<DigestLog> DigestLogLists = new List<DigestLog>();
        private List<BattleLog> temporaryDigestLogs = new List<BattleLog>();

        private int DirectionIndex = 0;

        public void ClearLogs()
        {
            BattleLogs.Clear();
            DigestLogLists.Clear();
            temporaryDigestLogs.Clear();
        }

        private void ResetRandomDirectionType()
        {
            // まあいまはLRしかないから0,1だけでええやろ.
            DirectionIndex = BattleGameLogic.GetNonStateRandomValue(0, 2);
        }
        
        public void AddBattleEndLog()
        {
            var result = new BattleMatchUpResult();
            AddLog(BattleDataMediator.Instance.OffenceSide, string.Empty, result, BattleConst.LogTiming.MatchUpResult, digestTiming: BattleConst.DigestTiming.GoalGameSet);

            var logData = new BattleLog();
            logData.LogTiming = BattleConst.LogTiming.OnBattleEnd;
            BattleLogs.Add(logData);
            BattleEventDispatcher.Instance.OnAddLogCallback();
        }
        
        public void AddTimeUpLog()
        {
            var result = new BattleMatchUpResult();
            AddLog(BattleDataMediator.Instance.OffenceSide, string.Empty, result, BattleConst.LogTiming.MatchUpResult, digestTiming: BattleConst.DigestTiming.TimeUp);
            
            var logData = new BattleLog();
            logData.LogTiming = BattleConst.LogTiming.OnBattleEnd;
            BattleLogs.Add(logData);
            BattleEventDispatcher.Instance.OnAddLogCallback();
        }

        // ログを吐かせるにあたって破綻した.
        // MatchUpResultじゃなくてActionResultみたいな形にしてどこで発行されたか(キックオフ, スローイン, マッチアップ)を持つ形にしないと無理.
        public void AddKickOffLog(BattleMatchUpResult matchUpResult)
        {
            var player = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.NextBallOwnerId);
            AddLog(player.Side, $"キックオフ！{player.GetNameWithTeamSideColorCode()}がボールをキープ！", matchUpResult, BattleConst.LogTiming.KickOff, digestTiming:BattleConst.DigestTiming.KickOff);
        }

        public void AddMatchUpLog(BattleMatchUpResult matchUpResult)
        {
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.NextBallOwnerId);
            var defencePlayer = offencePlayer.MarkCharacter;
            if (offencePlayer != null && defencePlayer != null)
            {
                AddLog(offencePlayer.Side, $"マッチアップ！{offencePlayer.GetNameWithTeamSideColorCode()} VS {defencePlayer.GetNameWithTeamSideColorCode()}！", matchUpResult, BattleConst.LogTiming.OnMatchUp, 1.0f);
            }
        }

        public void AddMatchUpActivatedLog(BattleMatchUpResult matchUpResult)
        {
            if (matchUpResult.MatchUpDigestType == BattleConst.MatchUpDigestType.None)
            {
                return;
            }
            
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.OnMatchUp, 1.0f, BattleConst.DigestTiming.MatchUp);
        }

        public void AddActiveAbilityLog(BattleMatchUpResult matchUpResult)
        {
            var abilityUser = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceAbilityUserCharacterId);
            var abilityModel = abilityUser.AbilityList.FirstOrDefault(ability => ability.BattleAbilityMaster.id == matchUpResult.OffenceAbilityId); 
            AddLog(abilityUser.Side, string.Empty, matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f, BattleConst.DigestTiming.Special, abilityUser, abilityModel.BattleAbilityMaster.id);
        }

        public void AddPreMatchUpResultLog(BattleMatchUpResult matchUpResult)
        {
            switch (matchUpResult.ActionType)
            {
                case BattleConst.MatchUpActionType.Through:
                    AddInitialRunThroughLog(matchUpResult);
                    break;
                case BattleConst.MatchUpActionType.Pass:
                    AddInitialPassLog(matchUpResult);
                    break;
                case BattleConst.MatchUpActionType.Shoot:
                    AddInitialShootLog(matchUpResult);
                    break;
                case BattleConst.MatchUpActionType.Cross:
                    AddInitialCrossLog(matchUpResult);
                    break;
            }
        }
        
        public void AddMatchUpResultLog(BattleMatchUpResult matchUpResult)
        {
            ResetRandomDirectionType();
            AddInsertAbilityLog(matchUpResult);
            
            switch (matchUpResult.ActionType)
            {
                case BattleConst.MatchUpActionType.None:
                    AddDribbleLog(matchUpResult);
                    break;
                case BattleConst.MatchUpActionType.Through:
                    AddRunThroughLog(matchUpResult);
                    break;
                case BattleConst.MatchUpActionType.Pass:
                    AddPassLog(matchUpResult);
                    break;
                case BattleConst.MatchUpActionType.Shoot:
                    // シュートは常にダイジェストオン
                    AddShootLog(matchUpResult);
                    break;
                case BattleConst.MatchUpActionType.Cross:
                    AddCrossLog(matchUpResult);
                    break;
            }
        }
        
        private void AddInsertAbilityLog(BattleMatchUpResult matchUpResult)
        {
            foreach (var insertDigest in matchUpResult.InsertDigests)
            {
                var mes = $"{insertDigest.Item2.GetNameWithTeamSideColorCode()}の【{insertDigest.Item3.GetColoredAbilityName()}】！";
                AddLog(insertDigest.Item2.Side, mes, matchUpResult, BattleConst.LogTiming.OnMatchUp, digestTiming: BattleConst.DigestTiming.None, triggeredCharacter: insertDigest.Item2, abilityId: insertDigest.Item3.BattleAbilityMaster.id);
            }
            
            //matchUpResult.InsertDigests.Clear();
        }

        private void AddDribbleLog(BattleMatchUpResult matchUpResult)
        {
            //ドリブル	[選手名]がドリブルで敵陣に切り込む！	同時にボールを前進させる
            var player = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.NextBallOwnerId);
            if (player == null)
            {
                return;
            }

            // ダイジェストはドリブルを起点とする.
            ResetTemporaryLog();

            AddLog(player.Side, $"{player.GetNameWithTeamSideColorCode()}がドリブルで敵陣に切り込む！", matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.Dribble);
        }

        private void AddInitialRunThroughLog(BattleMatchUpResult matchUpResult)
        {
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            if (offencePlayer == null)
            {
                return;
            }
            
            // 必殺技導線
            if (matchUpResult.ReplaceDigests.Any(digest => digest.Item1 == BattleConst.DigestTiming.Through))
            {
                foreach (var replaceDigest in matchUpResult.ReplaceDigests.Where(digest => digest.Item1 == BattleConst.DigestTiming.Through))
                {
                        AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f,
                            BattleConst.DigestTiming.Special, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                }
            }
            
            AddLog(offencePlayer.Side, $"{offencePlayer.GetNameWithTeamSideColorCode()}が突破をしかける！", matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.Through);
        }
        
        private void AddRunThroughLog(BattleMatchUpResult matchUpResult)
        {
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            if (offencePlayer == null)
            {
                return;
            }

            // 直導線ではここで最初, 必殺技キャンセル導線でも積み直しになるため
            AddInitialRunThroughLog(matchUpResult);

            if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.Success)
            {
                // 突破(成功)	[ボール保持者]が突破に成功！
                AddLog(offencePlayer.Side, $"{offencePlayer.GetNameWithTeamSideColorCode()}が突破に成功！", matchUpResult, BattleConst.LogTiming.MatchUpResult);
            }
            
            var defencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.DefenceCharacterId);
            if (defencePlayer == null)
            {
                return;
            }
            
            if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.Failed)
            {
                // 突破(ボール奪取)	"[ボール保持者]が突破に失敗…
                // 突破(弾き)	"[ボール保持者]が突破に失敗…
                AddLog(offencePlayer.Side, $"{offencePlayer.GetNameWithTeamSideColorCode()}が突破に失敗…", matchUpResult, BattleConst.LogTiming.MatchUpResult);
            }

            if (matchUpResult.BallInterferenceType == BattleConst.BallInterferenceType.Catched)
            {
                // [マーク相手]がボールを奪取！"	攻守交代
                AddLog(offencePlayer.Side, $"{defencePlayer.GetNameWithTeamSideColorCode()}がボールを奪取！", matchUpResult, BattleConst.LogTiming.MatchUpResult);
            }

            if (matchUpResult.BallInterferenceType == BattleConst.BallInterferenceType.Blocked)
            {
                // [マーク相手]がボールを弾いた！"	
                AddLog(offencePlayer.Side, $"{defencePlayer.GetNameWithTeamSideColorCode()}がボールを弾いた！", matchUpResult, BattleConst.LogTiming.MatchUpResult);
                if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.BallOut)
                {
                    AddThrowInLog(matchUpResult);
                }
                else if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.LooseBall)
                {
                    AddLooseBallLog(matchUpResult);
                }
            }
        }

        private void AddInitialPassLog(BattleMatchUpResult matchUpResult)
        {
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            var targetPlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.TargetCharacterId);
            if (offencePlayer == null || targetPlayer == null)
            {
                return;
            }
            
            // 必殺技導線
            if (matchUpResult.ReplaceDigests.Any(digest => digest.Item1 == BattleConst.DigestTiming.Pass))
            {
                foreach (var replaceDigest in matchUpResult.ReplaceDigests.Where(digest => digest.Item1 == BattleConst.DigestTiming.Pass))
                {
                    if (replaceDigest.Item3.BattleAbilityMaster.cutInType == 0)
                    {
                        AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f,
                            BattleConst.DigestTiming.None, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                    }
                    else
                    {
                        AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f,
                            BattleConst.DigestTiming.None, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                        AddLog(offencePlayer.Side,
                            $"{offencePlayer.GetNameWithTeamSideColorCode()}は{targetPlayer.GetNameWithTeamSideColorCode()}にパス！",
                            matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f, digestTiming: BattleConst.DigestTiming.Pass);
                    }
                }
            }
            // 専用演出なければ汎用再生
            else{
                AddLog(offencePlayer.Side,
                    $"{offencePlayer.GetNameWithTeamSideColorCode()}は{targetPlayer.GetNameWithTeamSideColorCode()}にパス！",
                    matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f, digestTiming: BattleConst.DigestTiming.Pass);
            }
        }

        private void AddPassLog(BattleMatchUpResult matchUpResult)
        {
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            var targetPlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.TargetCharacterId);
            if (offencePlayer == null || targetPlayer == null)
            {
                return;
            }

            AddInitialPassLog(matchUpResult);
            
            var passCutPlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.DefenceActionCharacterId);
            if (passCutPlayer == null)
            {
                // パスカットの必殺技じゃないけど、タイミング的にはパスカットと同じタイミングなので. 紛らわしくてすみません.
                if (matchUpResult.ReplaceDigests.Any(digest => digest.Item1 == BattleConst.DigestTiming.PassCut))
                {
                    foreach (var replaceDigest in matchUpResult.ReplaceDigests.Where(digest => digest.Item1 == BattleConst.DigestTiming.PassCut))
                    {
                        // 専用演出
                        if (replaceDigest.Item3.BattleAbilityMaster.cutInType == 0)
                        {
                            AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f, BattleConst.DigestTiming.Special, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                        }
                        // 簡易演出 -> 汎用演出
                        else
                        {
                            AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f, BattleConst.DigestTiming.Special, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                        }
                    }
                }

                AddLog(offencePlayer.Side, $"パス成功！ボールは{targetPlayer.GetNameWithTeamSideColorCode()}へ！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f);
                return;
            }

            // シュート	[ボール保持者]がシュート！	次のログまでちょっとウェイト
            if (matchUpResult.ReplaceDigests.Any(digest => digest.Item1 == BattleConst.DigestTiming.PassCut))
            {
                foreach (var replaceDigest in matchUpResult.ReplaceDigests.Where(digest => digest.Item1 == BattleConst.DigestTiming.PassCut))
                {
                    // 専用演出
                    if (replaceDigest.Item3.BattleAbilityMaster.cutInType == 0)
                    {
                        AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f, BattleConst.DigestTiming.Special, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                    }
                    // 簡易演出 -> 汎用演出
                    else
                    {
                        AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f, BattleConst.DigestTiming.Special, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                        if (matchUpResult.BallInterferenceType == BattleConst.BallInterferenceType.Catched)
                        {
                            // パス(パスカット奪取)	"[選手名]がパスを止めた！
                            // そのままボールを奪取！"	パスカット発生時に追加
                            AddLog(offencePlayer.Side, $"{passCutPlayer.GetNameWithTeamSideColorCode()}がパスを止めた！", matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.PassCut);
                        }

                        if (matchUpResult.BallInterferenceType == BattleConst.BallInterferenceType.Blocked)
                        {
                            // パス(パスカット弾き)	[選手名]がパスを弾いた！	パスカット発生時に追加
                            AddLog(offencePlayer.Side, $"{passCutPlayer.GetNameWithTeamSideColorCode()}がパスを弾いた！", matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming: BattleConst.DigestTiming.PassCut);
                        }
                    }
                }

                if (matchUpResult.BallInterferenceType == BattleConst.BallInterferenceType.Catched)
                {
                    // パス(パスカット奪取)	"[選手名]がパスを止めた！
                    // そのままボールを奪取！"	パスカット発生時に追加
                    AddLog(offencePlayer.Side, $"そのままボールを奪取！", matchUpResult, BattleConst.LogTiming.MatchUpResult);
                }
            
                if (matchUpResult.BallInterferenceType == BattleConst.BallInterferenceType.Blocked)
                {
                    // パス(パスカット弾き)	[選手名]がパスを弾いた！	パスカット発生時に追加
                    if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.BallOut)
                    {
                        AddThrowInLog(matchUpResult);
                    }else if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.LooseBall)
                    {
                        AddLooseBallLog(matchUpResult);
                    }
                }
            }
            // 専用演出なければ汎用再生
            else
            {
                if (matchUpResult.BallInterferenceType == BattleConst.BallInterferenceType.Catched)
                {
                    // パス(パスカット奪取)	"[選手名]がパスを止めた！
                    // そのままボールを奪取！"	パスカット発生時に追加
                    AddLog(offencePlayer.Side, $"{passCutPlayer.GetNameWithTeamSideColorCode()}がパスを止めた！", matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.PassCut);
                    AddLog(offencePlayer.Side, $"そのままボールを奪取！", matchUpResult, BattleConst.LogTiming.MatchUpResult);
                }
            
                if (matchUpResult.BallInterferenceType == BattleConst.BallInterferenceType.Blocked)
                {
                    // パス(パスカット弾き)	[選手名]がパスを弾いた！	パスカット発生時に追加
                    AddLog(offencePlayer.Side, $"{passCutPlayer.GetNameWithTeamSideColorCode()}がパスを弾いた！", matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.PassCut);
                    if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.BallOut)
                    {
                        AddThrowInLog(matchUpResult);
                    }else if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.LooseBall)
                    {
                        AddLooseBallLog(matchUpResult);
                    }
                }
            }
        }

        private void AddInitialShootLog(BattleMatchUpResult matchUpResult, long ballOwnerId = -1)
        {
            var offencePlayerId = ballOwnerId > 0 ? ballOwnerId : matchUpResult.OffenceCharacterId;
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(offencePlayerId);
            if (offencePlayer == null)
            {
                return;
            }
            
            // シュート	[ボール保持者]がシュート！	次のログまでちょっとウェイト
            if (matchUpResult.ReplaceDigests.Any(digest => digest.Item1 == BattleConst.DigestTiming.Shoot))
            {
                foreach (var replaceDigest in matchUpResult.ReplaceDigests.Where(digest => digest.Item1 == BattleConst.DigestTiming.Shoot))
                {
                    if (replaceDigest.Item3.BattleAbilityMaster.cutInType == 0)
                    {
                        AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f,
                            BattleConst.DigestTiming.None, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                    }
                    else
                    {
                        AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f,
                            BattleConst.DigestTiming.None, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                        AddLog(offencePlayer.Side, $"{offencePlayer.GetNameWithTeamSideColorCode()}がシュート！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f,
                            digestTiming:BattleConst.DigestTiming.Shoot);
                    }
                }
            }
            // 専用演出なければ汎用再生
            else
            {
                AddLog(offencePlayer.Side, $"{offencePlayer.GetNameWithTeamSideColorCode()}がシュート！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f,
                    digestTiming:BattleConst.DigestTiming.Shoot);
            }
        }

        private void AddShootLog(BattleMatchUpResult matchUpResult, long ballOwnerId = -1)
        {
            var offencePlayerId = ballOwnerId > 0 ? ballOwnerId : matchUpResult.OffenceCharacterId;
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(offencePlayerId);
            if (offencePlayer == null)
            {
                return;
            }

            // 直導線ではここで最初, 必殺技キャンセル導線でも積み直しになるため
            AddInitialShootLog(matchUpResult, offencePlayer.id);

            // ちょっとシュートブロック周りのログ整備するのにいろいろ追加しなきゃいけないから後回し.
            var shootBlockPlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.DefenceActionCharacterId);
            if (shootBlockPlayer != null)
            {
                // シュートブロック	[ブロッカー]がボールに足を伸ばす！
                // $"{shootBlockPlayer.GetNameWithTeamSideColorCode()}がボールに足を伸ばす！"
                //AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.MatchUpResult);
                
                // 専用で差し替え
                if (matchUpResult.ReplaceDigests.Any(digest => digest.Item1 == BattleConst.DigestTiming.ShootBlock && digest.Item3.BattleAbilityMaster.cutInType == 0))
                {
                    foreach (var replaceDigest in matchUpResult.ReplaceDigests.Where(digest => digest.Item1 == BattleConst.DigestTiming.ShootBlock && digest.Item3.BattleAbilityMaster.cutInType == 0))
                    {
                        AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f,
                            BattleConst.DigestTiming.Special, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                    }
                    
                    // シュートブロック(失敗)	"しかしボールに届かなかった！
                    // シュートブロック(弾き)	[ブロッカー]がシュートを止めた！	
                    // シュートブロック(かすり)	"[ブロッカー]の足先がボールに触れる！
                    if (matchUpResult.ShootBlockType == BattleConst.BallInterferenceType.Blocked)
                    {
                        if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.BallOut)
                        {
                            AddThrowInLog(matchUpResult);
                        }else if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.LooseBall)
                        {
                            AddLooseBallLog(matchUpResult);
                        }
                        return;
                    }
                }
                else
                {
                    // 汎用を差し込んで共通をその後再生
                    if (matchUpResult.ReplaceDigests.Any(digest => digest.Item1 == BattleConst.DigestTiming.ShootBlock && digest.Item3.BattleAbilityMaster.cutInType == 1))
                    {
                        foreach (var replaceDigest in matchUpResult.ReplaceDigests.Where(digest => digest.Item1 == BattleConst.DigestTiming.ShootBlock && digest.Item3.BattleAbilityMaster.cutInType == 1))
                        {
                            AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f,
                                BattleConst.DigestTiming.Special, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                        }
                    }

                    // シュートブロック(失敗)	"しかしボールに届かなかった！
                    // シュートブロック(弾き)	[ブロッカー]がシュートを止めた！	
                    // シュートブロック(かすり)	"[ブロッカー]の足先がボールに触れる！
                    switch (matchUpResult.ShootBlockType)
                    {
                        case BattleConst.BallInterferenceType.Blocked:
                            // $"{shootBlockPlayer.GetNameWithTeamSideColorCode()}がシュートを止めた！"
                            AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.ShootBlock);
                            if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.BallOut)
                            {
                                AddThrowInLog(matchUpResult);
                            }else if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.LooseBall)
                            {
                                AddLooseBallLog(matchUpResult);
                            }
                            return;
                        case BattleConst.BallInterferenceType.Touched:
                            // $"{shootBlockPlayer.GetNameWithTeamSideColorCode()}の足先がボールに触れる！"
                            AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.ShootBlock);
                            break;
                        case BattleConst.BallInterferenceType.None:
                            // "しかしボールに届かなかった！"
                            AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.ShootBlock);
                            break;
                    }
                }
            }
            
            AddLog(offencePlayer.Side, "ボールはゴールへ！", matchUpResult, BattleConst.LogTiming.MatchUpResult);

            if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.Success)
            {
                // ゴール	"キーパーとれない！ゴール！
                // "キーパーとれない！ゴール！"
                AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.ShootResult);
                // スコア X VS Y"	Xは青、Yは赤文字
                var leftScore = BattleDataMediator.Instance.Score[(int)BattleConst.TeamSide.Left];
                var rightScore = BattleDataMediator.Instance.Score[(int)BattleConst.TeamSide.Right];
                // $"スコア <color=blue>{leftScore}</color> VS <color=red>{rightScore}</color>"
                AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.Goal);
                AddDigestLog();
            }
            else
            {
                // サイドが変わる可能性あるのでこそっと表示用の変更
                // もうわけがわからな〜〜〜いｗ
                matchUpResult.IsInShootRange = false;
                
                if (matchUpResult.BallInterferenceType == BattleConst.BallInterferenceType.Catched)
                {
                    // ゴール失敗(キーパーキャッチ)	キーパーがボールをキャッチ！
                    // "キーパーがボールをキャッチ！"
                    AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.ShootResult);
                    AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.ThrowIn);
                }
                else
                {
                    // ゴール失敗(キーパー弾き)	キーパーがボールを弾いた！
                    // ゴール失敗(ゴールポスト)	ボールがポストに弾かれた！	
                    // アウトオブバウンズ	ボールはコートの外へ！
                    // "ボールはコートの外へ！"
                    // スローイン
                    if (matchUpResult.MatchUpResult == BattleConst.MatchUpResult.BallOut)
                    {
                        AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.ShootResult);
                        AddThrowInLog(matchUpResult);
                    }
                    // ボールが場外に出なかった場合なのでセカンドボール
                    else
                    {
                        AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.MatchUpResult, digestTiming:BattleConst.DigestTiming.ShootResult);
                        AddLooseBallLog(matchUpResult);
                    }
                }
            }
        }

        private void AddInitialCrossLog(BattleMatchUpResult matchUpResult)
        {
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            var targetPlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.TargetCharacterId);
            if (offencePlayer == null || targetPlayer == null)
            {
                return;
            }

            // クロス必殺技
            var nicePassMes = matchUpResult.IsNicePass ? "絶好のクロス！" : "クロス！";
            if (matchUpResult.ReplaceDigests.Any(digest => digest.Item1 == BattleConst.DigestTiming.Cross))
            {
                foreach (var replaceDigest in matchUpResult.ReplaceDigests.Where(digest => digest.Item1 == BattleConst.DigestTiming.Cross))
                {
                    if (replaceDigest.Item3.BattleAbilityMaster.cutInType == 0)
                    {
                        AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f,
                            BattleConst.DigestTiming.None, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                    }
                    else
                    {
                        AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f,
                            BattleConst.DigestTiming.None, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                        AddLog(offencePlayer.Side, $"{offencePlayer.GetNameWithTeamSideColorCode()}から{targetPlayer.GetNameWithTeamSideColorCode()}に{nicePassMes}", matchUpResult, BattleConst.LogTiming.MatchUpResult,
                            digestTiming:BattleConst.DigestTiming.Cross);
                    }
                }
            }
            // クロス必殺技がある場合は上で引っかかる
            // シュート必殺技がある場合は以降のAddShootLogで処理するため. 二回目入ってきてシュート必殺技がある場合はクロス打ち上げから再開じゃないのでスルー
            else if (matchUpResult.ReplaceDigests.All(digest => digest.Item1 != BattleConst.DigestTiming.Shoot))
            {
                AddLog(offencePlayer.Side, $"{offencePlayer.GetNameWithTeamSideColorCode()}から{targetPlayer.GetNameWithTeamSideColorCode()}に{nicePassMes}", matchUpResult, BattleConst.LogTiming.MatchUpResult,
                    digestTiming:BattleConst.DigestTiming.Cross);
            }
        }

        private void AddCrossLog(BattleMatchUpResult matchUpResult)
        {
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            var targetPlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.TargetCharacterId);
            if (offencePlayer == null || targetPlayer == null)
            {
                return;
            }
            
            AddInitialCrossLog(matchUpResult);

            AddShootLog(matchUpResult, targetPlayer.id);
        }

        private void AddLooseBallLog(BattleMatchUpResult matchUpResult)
        {
            // サイドが変わる可能性あるのでこそっと表示用の変更
            // もうわけがわからな〜〜〜いｗ
            matchUpResult.IsInShootRange = false;
            
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            if (offencePlayer == null)
            {
                return;
            }
            
            var nextBallOwner = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.NextBallOwnerId);
            if (nextBallOwner == null)
            {
                return;
            }
            
            //セカンドボール	"セカンドボール！
            // セカンドボールは常にダイジェストオン
            if (matchUpResult.ReplaceDigests.Any(digest => digest.Item1 == BattleConst.DigestTiming.SecondBall && digest.Item3.BattleAbilityMaster.cutInType == 0))
            {
                foreach (var replaceDigest in matchUpResult.ReplaceDigests.Where(digest => digest.Item1 == BattleConst.DigestTiming.SecondBall && digest.Item3.BattleAbilityMaster.cutInType == 0))
                {
                    AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f, BattleConst.DigestTiming.Special, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                }
            }
            else
            {
                if (matchUpResult.ReplaceDigests.Any(digest => digest.Item1 == BattleConst.DigestTiming.SecondBall && digest.Item3.BattleAbilityMaster.cutInType == 1))
                {
                    foreach (var replaceDigest in matchUpResult.ReplaceDigests.Where(digest => digest.Item1 == BattleConst.DigestTiming.SecondBall && digest.Item3.BattleAbilityMaster.cutInType == 1))
                    {
                        AddLog(replaceDigest.Item2.Side, $"{replaceDigest.Item2.GetNameWithTeamSideColorCode()}の【{replaceDigest.Item3.GetColoredAbilityName()}】！", matchUpResult, BattleConst.LogTiming.MatchUpResult, 1.0f, BattleConst.DigestTiming.Special, replaceDigest.Item2, replaceDigest.Item3.BattleAbilityMaster.id);
                    }
                }

                AddLog(offencePlayer.Side, $"セカンドボール！", matchUpResult, BattleConst.LogTiming.LooseBall);
                // [選手名]がボールを奪取！"	特に参加者の名前は出さなくていいかなと
                // $"{nextBallOwner.GetNameWithTeamSideColorCode()}がボールを奪取！"
                AddLog(offencePlayer.Side, string.Empty, matchUpResult, BattleConst.LogTiming.LooseBall, digestTiming:BattleConst.DigestTiming.SecondBall);
            }
        }
        
        private void AddThrowInLog(BattleMatchUpResult matchUpResult)
        {
            // サイドが変わる可能性あるのでこそっと表示用の変更
            // もうわけがわからな〜〜〜いｗ
            matchUpResult.IsInShootRange = false;
            
            var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.OffenceCharacterId);
            if (offencePlayer == null)
            {
                return;
            }
            
            //セカンドボール	"セカンドボール！
            // セカンドボールは常にダイジェストオン
            AddLog(offencePlayer.Side, $"ボールアウト！", matchUpResult, BattleConst.LogTiming.ThrowIn, digestTiming: BattleConst.DigestTiming.OutBall);
            var nextBallOwner = BattleDataMediator.Instance.GetBattleCharacter(matchUpResult.NextBallOwnerId);
            if (nextBallOwner == null)
            {
                return;
            }

            var side = matchUpResult.IsSideChanged ? BattleGameLogic.GetOtherSide(offencePlayer.Side) : offencePlayer.Side;
            AddLog(side, $"スローイン！ ボールは{nextBallOwner.GetNameWithTeamSideColorCode()}へ", matchUpResult, BattleConst.LogTiming.ThrowIn, digestTiming: BattleConst.DigestTiming.ThrowIn);
        }

        private void AddLog(BattleConst.TeamSide side, string log, BattleMatchUpResult result, BattleConst.LogTiming logTiming, float delay = 0.5f, BattleConst.DigestTiming digestTiming = BattleConst.DigestTiming.None,
            BattleCharacterModel triggeredCharacter = null, long abilityId = -1)
        {
            // 仕組み変わりすぎ.
            delay = 0.0f;
            
            // 一旦ラップ.
            // ディレイを加味したBoltServerTimeを付与していって, クライアント側では常に監視して再生開始タイミング以降だったらログに積んでく, とかならスムーズにいけそう. ドラスマのコマンドと思想は全く一緒.
            var logData = new BattleLog();
            logData.OffenceSide = side;
            logData.Delay = delay;
            logData.LogTiming = logTiming;
            logData.BallPosition = result.NextBallPosition;
            logData.ElapsedTime = BattleDataMediator.Instance.GameTime;
            BattleLogs.Add(logData);
            temporaryDigestLogs.Add(logData);

            if (!string.IsNullOrEmpty(log))
            {
                logData.MessageLog = log;
            }
            
            if (abilityId > 0 && triggeredCharacter != null)
            {
                logData.IconMCharaId = triggeredCharacter.MCharaId;

                // モデル取得
                BattleAbilityModel model = triggeredCharacter.GetAbilityModelByAbilityId(abilityId);
                // アニメーション用のスキル情報を取得
                logData.Rarity = model.BattleAbilityMaster.rarity;
                logData.AbilityCategory = model.BattleAbilityMaster.abilityCategory;
            }

            // クロスをあげられてシュートが決まっているのでnextBallOwnerId=-1のケースもある.
            logData.DigestLog = CreateDigestLog(side, result, digestTiming, triggeredCharacter, abilityId);
            logData.IsInShootRange = result.IsInShootRange;

            /*
            // バトル終了ログは必ず出す
            if (logTiming == BattleConst.LogTiming.OnBattleEnd)
            {
                logData.DigestLog = CreateDigestLog(side, result, digestTiming, triggeredCharacter, abilityId);
            }
            else
            {
                if (isSkipToFinish)
                {
                    BattleEventDispatcher.Instance.OnAddLogCallback();
                    return;
                }

                logData.DigestLog = CreateDigestLog(side, result, digestTiming, triggeredCharacter, abilityId);
                logData.IsInShootRange = result.IsInShootRange;
            }
            */
            
            BattleEventDispatcher.Instance.OnAddLogCallback();
        }

        public void AddAbilityLog(List<Tuple<BattleCharacterModel, BattleAbilityModel>> activatedAbilities)
        {
            foreach (var activatedAbility in activatedAbilities)
            {
                AddAbilityLog(activatedAbility);
            }
        }

        private void AddAbilityLog(Tuple<BattleCharacterModel, BattleAbilityModel> activatedAbility)
        {
            var character = activatedAbility.Item1;
            var ability = activatedAbility.Item2;
            var logData = new BattleLog();
            logData.Delay = 0.1f;
            logData.MessageLog = $"{character.GetNameWithTeamSideColorCode()}の【{ability.GetColoredAbilityName()}】が発動!";
            // レアリティ取得
            logData.Rarity = ability.BattleAbilityMaster.rarity;
            // カテゴリ取得
            logData.AbilityCategory = ability.BattleAbilityMaster.abilityCategory;
            logData.IconMCharaId = character.MCharaId;
            logData.OffenceSide = character.Side;
            BattleLogs.Add(logData);
            BattleEventDispatcher.Instance.OnAddLogCallback();
            
            character.Stats.AddActivityStat(BattleCharacterStatModel.StatType.PassiveSkill, false, ability.BattleAbilityMaster.id);
        }

        public void AddAbilityLogGuildBattle(List<Tuple<BattleCharacterModel, BattleAbilityModel>> activatedAbilities)
        {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
            foreach (Tuple<BattleCharacterModel, BattleAbilityModel> activatedAbility in activatedAbilities)
            {
                // サポートスキルでない場合ログを出さない
                if (activatedAbility.Item2.BattleAbilityMaster.abilityType != (long)BattleConst.AbilityType.GuildBattleAuto) continue;
                AddAbilityLogGuildBattle(activatedAbility);
            }
#endif
        }

        private void AddAbilityLogGuildBattle(Tuple<BattleCharacterModel, BattleAbilityModel> activatedAbility)
        {
            BattleCharacterModel character = activatedAbility.Item1;
            BattleAbilityModel ability = activatedAbility.Item2;
            BattleLog logData = new BattleLog();
            logData.Delay = 0.1f;

#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
            logData.MessageLog = StringValueAssetLoader.Instance["clubroyalingame.support_skill.battle_log_format"].Format(
                ability.BattleAbilityMaster.name,
                ability.BattleAbilityMaster.useMessage
                );
#else
            logData.MessageLog = $"{ability.BattleAbilityMaster.name},{ability.BattleAbilityMaster.useMessage}";
#endif
            logData.Rarity = 0;
            logData.AbilityCategory = 0;
            logData.IconMCharaId = character.MCharaId;
            logData.OffenceSide = BattleConst.TeamSide.Left;
            BattleLogs.Add(logData);
            BattleEventDispatcher.Instance.OnAddLogCallback();

        }
        
        public void AddNextMatchUpLog()
        {
            var logData = new BattleLog();
            logData.LogTiming = BattleConst.LogTiming.DoNextMatchUp;
            BattleLogs.Add(logData);
            BattleEventDispatcher.Instance.OnAddLogCallback();
        }

        private BattleDigestLog CreateDigestLog(BattleConst.TeamSide side, BattleMatchUpResult result, BattleConst.DigestTiming timing, BattleCharacterModel triggeredCharacter, long abilityId)
        {
            if (result == null)
            {
                return null;
            }
            
            var log = new BattleDigestLog();
            log.OtherCharacterDataList = new List<BattleDigestCharacterData>();
            log.OffenceSide = side;
            log.DistanceToGoal = BattleGameLogic.GetRemainDistanceToGoal(BattleDataMediator.Instance.OffenceSide, BattleDataMediator.Instance.BallPosition);
            log.MatchUpDigestType = result.MatchUpDigestType;

            if (timing == BattleConst.DigestTiming.MatchUp)
            {
                var character = BattleDataMediator.Instance.GetBattleCharacter(result.OffenceCharacterId);
                var matchUpCommandData = BattleDataMediator.Instance.NextCommandData;
                log.RemainDistanceToShoot = BattleGameLogic.GetRemainDistanceToGoal(character.Side, BattleDataMediator.Instance.BallPosition) - character.GetCurrentShootRange();
                if (matchUpCommandData.Any(command => command.ActionType == BattleConst.MatchUpActionType.Shoot))
                {
                    log.CommandData |= (int)BattleConst.LogActionBits.HasShoot;
                }
                if (matchUpCommandData.Any(command => command.ActionType == BattleConst.MatchUpActionType.Cross))
                {
                    log.CommandData |= (int)BattleConst.LogActionBits.HasCross;
                }
                if (matchUpCommandData.Any(command => command.ActionType == BattleConst.MatchUpActionType.Pass))
                {
                    if (matchUpCommandData.Any(command => BattleGameLogic.IsLongPass(command.ActionType, command.ActionDetailType)))
                    {
                        log.CommandData |= (int)BattleConst.LogActionBits.HasLongPass;
                    }
                    else if (matchUpCommandData.Any(command => BattleGameLogic.IsBackPass(command.ActionType, command.ActionDetailType)))
                    {
                        log.CommandData |= (int)BattleConst.LogActionBits.HasBackPass;
                    }
                    else
                    {
                        log.CommandData |= (int)BattleConst.LogActionBits.HasPass;
                    }
                }
            }

            var score = BattleDataMediator.Instance.GetScore(side);
            if (result.ScoredCharacterId > 0)
            {
                score--;
            }

            log.IsLastScoreToEnd = score >= BattleConst.RequiredScore - 1;
            
            // ダイジェスト演出はメインのキャラとそれ以外(0人以上）で構成され
            // メインキャラからセリフ等が開始される
            // 誰がメインキャラになるかは各演出による

            switch (timing)
            {
                case BattleConst.DigestTiming.None:
                    log.Type = BattleConst.DigestType.None;
                    break;
                case BattleConst.DigestTiming.KickOff:
                {
                    // キックオフ
                    // メイン：ボール保持者にボールを渡すキャラがメイン
                    // その他：キックオフによってボール保持者になった1名
                    log.Type = side == BattleConst.TeamSide.Left
                        ? BattleConst.DigestType.KickOffL
                        : BattleConst.DigestType.KickOffR;

                    // メイン
                    var mainCharaId =
                        result.RoundOffenceCharacterIds.FirstOrDefault(id => id != result.NextBallOwnerId);
                    var character = BattleDataMediator.Instance.GetBattleCharacter(mainCharaId);
                    log.MainCharacterData = new  BattleDigestCharacterData();
                    log.MainCharacterData.SetData(character);

                    //　その他
                    character = BattleDataMediator.Instance.GetBattleCharacter(result.NextBallOwnerId);
                    log.OtherCharacterDataList.Add(new BattleDigestCharacterData());
                    log.OtherCharacterDataList[0].SetData(character);
                    break;
                }
                case BattleConst.DigestTiming.Dribble:
                {
                    // ドリブル
                    // メイン：ボール保持者がメイン
                    // その他：ボール保持者以外のラウンド参加者1名(OFもDFもあり得る)
                    log.Type = side == BattleConst.TeamSide.Left
                        ? BattleConst.DigestType.DribbleL
                        : BattleConst.DigestType.DribbleR;

                    // メイン
                    var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(result.NextBallOwnerId);
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(offencePlayer);
                    log.MarkCharacterData = new BattleDigestCharacterData();
                    log.MarkCharacterData.SetData(offencePlayer.MarkCharacter);

                    // その他
                    // OFかDFかはランダム
                    BattleCharacterModel character = null;
                    var isOffence = BattleGameLogic.GetNonStateRandomValue(0, 100) > 50;
                    var offenceOtherCharaIds =
                        result.RoundOffenceCharacterIds.Where(d => d != result.NextBallOwnerId).ToList();
                    if (isOffence && offenceOtherCharaIds.Count > 0)
                    {
                        var id = offenceOtherCharaIds.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
                        character = BattleDataMediator.Instance.GetBattleCharacter(id);
                    }
                    else
                    {
                        // DFの場合はボール保持者のマークキャラ（DFは必ずいるはず）
                        character = offencePlayer.MarkCharacter;
                    }

                    if (character != null)
                    {
                        var data = new BattleDigestCharacterData();
                        data.SetData(character);
                        log.OtherCharacterDataList.Add(data);
                    }
                }
                    break;
                case BattleConst.DigestTiming.MatchUp:
                {
                    // マッチアップ
                    // メイン：ボール保持者
                    // その他：ボール保持者のマークキャラ1名
                    log.Type = BattleConst.DigestType.MatchUp;

                    // メイン
                    var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(result.OffenceCharacterId);
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(offencePlayer);

                    // その他
                    var defencePlayer = offencePlayer.MarkCharacter;
                    if (defencePlayer != null)
                    {
                        log.OtherCharacterDataList.Add(new BattleDigestCharacterData());
                        log.OtherCharacterDataList[0].SetData(defencePlayer);
                    }

                }
                    break;
                case BattleConst.DigestTiming.Through:
                {
                    // 突破
                    // メイン：マッチアップ時に突破を行ったキャラ
                    // その他：マッチアップ時の突破を行ったキャラをマークしていたキャラ1名

                    //　メイン
                    var offencePlayer =
                        BattleDataMediator.Instance.GetBattleCharacter(result.OffenceCharacterId);
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(offencePlayer);

                    // その他
                    var defencePlayer = offencePlayer.MarkCharacter;
                    log.OtherCharacterDataList.Add(new BattleDigestCharacterData());
                    log.OtherCharacterDataList[0].SetData(defencePlayer);

                    switch (offencePlayer.PrimaryParam)
                    {
                        case BattleConst.StatusParamType.Technique:
                            if (side == BattleConst.TeamSide.Left)
                            {
                                log.Type = result.MatchUpResult == BattleConst.MatchUpResult.Success
                                    ? BattleConst.DigestType.TechnicMatchUpWinL
                                    : BattleConst.DigestType.TechnicMatchUpLoseL;
                            }
                            else
                            {
                                log.Type = result.MatchUpResult == BattleConst.MatchUpResult.Success
                                    ? BattleConst.DigestType.TechnicMatchUpWinR
                                    : BattleConst.DigestType.TechnicMatchUpLoseR;
                            }

                            break;
                        case BattleConst.StatusParamType.Physical:
                            if (side == BattleConst.TeamSide.Left)
                            {
                                log.Type = result.MatchUpResult == BattleConst.MatchUpResult.Success
                                    ? BattleConst.DigestType.PhysicalMatchUpWinL
                                    : BattleConst.DigestType.PhysicalMatchUpLoseL;
                            }
                            else
                            {
                                log.Type = result.MatchUpResult == BattleConst.MatchUpResult.Success
                                    ? BattleConst.DigestType.PhysicalMatchUpWinR
                                    : BattleConst.DigestType.PhysicalMatchUpLoseR;
                            }

                            break;
                        case BattleConst.StatusParamType.Speed:
                            if (side == BattleConst.TeamSide.Left)
                            {
                                log.Type = result.MatchUpResult == BattleConst.MatchUpResult.Success
                                    ? BattleConst.DigestType.SpeedMatchUpWinL
                                    : BattleConst.DigestType.SpeedMatchUpLoseL;
                            }
                            else
                            {
                                log.Type = result.MatchUpResult == BattleConst.MatchUpResult.Success
                                    ? BattleConst.DigestType.SpeedMatchUpWinR
                                    : BattleConst.DigestType.SpeedMatchUpLoseR;
                            }

                            break;
                        default:
                            return null;
                    }
                }
                    break;
                case BattleConst.DigestTiming.Cross:
                {
                    // クロス
                    // メイン：マッチアップ時にクロスを行ったキャラ
                    // その他：クロスでボールを受け取るキャラ1名
                    log.Type = BattleConst.DigestType.Cross;

                    // メイン
                    var offencePlayer =
                        BattleDataMediator.Instance.GetBattleCharacter(result.OffenceCharacterId);
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(offencePlayer);

                    // その他
                    var targetPlayer =
                        BattleDataMediator.Instance.GetBattleCharacter(result.TargetCharacterId);
                    log.OtherCharacterDataList.Add(new BattleDigestCharacterData());
                    log.OtherCharacterDataList[0].SetData(targetPlayer);
                }
                    break;
                case BattleConst.DigestTiming.Pass:
                {
                    // パス
                    // メイン：マッチアップ時にパスを行ったキャラ
                    // その他：パスでボールを受け取るキャラ1名
                    log.Type = BattleConst.DigestType.PassSuccess;
                    var passCutPlayer = BattleDataMediator.Instance.GetBattleCharacter(result.DefenceActionCharacterId);
                    if (passCutPlayer != null)
                    {
                        // 失敗
                        log.Type = BattleConst.DigestType.PassFailed;
                    }

                    // メイン
                    var offencePlayer =
                        BattleDataMediator.Instance.GetBattleCharacter(result.OffenceCharacterId);
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(offencePlayer);

                    // その他
                    var targetPlayer =
                        BattleDataMediator.Instance.GetBattleCharacter(result.TargetCharacterId);
                    log.OtherCharacterDataList.Add(new BattleDigestCharacterData());
                    log.OtherCharacterDataList[0].SetData(targetPlayer);
                }
                    break;
                case BattleConst.DigestTiming.PassCut:
                {
                    // パスカット
                    // メイン：パスカットしたキャラ
                    // その他：無し
                    log.Type = result.BallInterferenceType == BattleConst.BallInterferenceType.Catched
                        ? BattleConst.DigestType.PassCutCatch
                        : BattleConst.DigestType.PassCutBlock;

                    // メイン
                    var passCutPlayer = BattleDataMediator.Instance.GetBattleCharacter(result.DefenceActionCharacterId);
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(passCutPlayer);
                }
                    break;
                case BattleConst.DigestTiming.Shoot:
                {
                    // シュート
                    // メイン：シュートしたキャラ
                    // その他：無し
                    log.Type = BattleConst.DigestType.Shoot;

                    var offencePlayerId = result.TargetCharacterId > 0 ? result.TargetCharacterId : result.OffenceCharacterId;
                    var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(offencePlayerId);
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(offencePlayer);

                    // クロスからのシュートのフローでDFが不足していてマークについていないケースがありうす(クロス打ち上げ元には絶対にマークがつくが, 参加者全員にマークが必ずつくわけではない.)
                    // この場合は演出上の都合のみ(リザルトのダイジェスト選択で相手側の表示)なので, クロス打ち上げ元のキャラクターを表示しちゃう.
                    if (offencePlayer.MarkCharacter == null)
                    {
                        offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(result.OffenceCharacterId);
                    }
                    log.OtherCharacterDataList = new List<BattleDigestCharacterData>() { new BattleDigestCharacterData() };
                    log.OtherCharacterDataList[0].SetData(offencePlayer.MarkCharacter);
                }
                    break;
                case BattleConst.DigestTiming.ShootBlock:
                    // シュートブロック
                    // メイン：シュートブロック参加したキャラ
                    // その他：無し
                    switch (result.ShootBlockType)
                    {
                        case BattleConst.BallInterferenceType.Blocked:
                            log.Type = DirectionIndex == 0 ? BattleConst.DigestType.ShootBlockL : BattleConst.DigestType.ShootBlockR;
                            break;
                        case BattleConst.BallInterferenceType.Touched:
                            log.Type = DirectionIndex == 0 ? BattleConst.DigestType.ShootBlockTouchL : BattleConst.DigestType.ShootBlockTouchR;
                            break;
                        case BattleConst.BallInterferenceType.None:
                            log.Type = DirectionIndex == 0 ? BattleConst.DigestType.ShootBlockNotReachL : BattleConst.DigestType.ShootBlockNotReachR;
                            break;
                    }

                    // メイン
                    var shootBlockPlayer =
                        BattleDataMediator.Instance.GetBattleCharacter(result.DefenceActionCharacterId);
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(shootBlockPlayer);

                    break;
                case BattleConst.DigestTiming.ShootResult:
                {
                    // シュート結果
                    // メイン：シュートしたキャラ
                    // その他：無し
                    if (result.MatchUpResult == BattleConst.MatchUpResult.Success)
                    {
                        log.Type = DirectionIndex == 0 ? BattleConst.DigestType.ShootResultSuccessL : BattleConst.DigestType.ShootResultSuccessR;
                    }
                    else
                    {
                        if (result.BallInterferenceType == BattleConst.BallInterferenceType.Catched)
                        {
                            log.Type = BattleConst.DigestType.ShootResultCatch;
                        }
                        else if (result.BallInterferenceType == BattleConst.BallInterferenceType.HitGoalPost)
                        {
                            log.Type = DirectionIndex == 0 ? BattleConst.DigestType.ShootResultPostL : BattleConst.DigestType.ShootResultPostR;
                        }
                        else
                        {
                            log.Type = DirectionIndex == 0 ? BattleConst.DigestType.ShootResultPunchL : BattleConst.DigestType.ShootResultPunchR;
                        }
                    }

                    // メイン
                    var offencePlayerId = result.TargetCharacterId > 0 ? result.TargetCharacterId : result.OffenceCharacterId;
                    var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(offencePlayerId);
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(offencePlayer);
                }
                    break;
                case BattleConst.DigestTiming.Goal:
                {
                    // ゴール
                    // メイン：シュートしたキャラ
                    // その他：無し
                    log.Type = BattleConst.DigestType.Goal;

                    // メイン
                    var offencePlayerId = result.TargetCharacterId > 0 ? result.TargetCharacterId : result.OffenceCharacterId;
                    var offencePlayer = BattleDataMediator.Instance.GetBattleCharacter(offencePlayerId);
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(offencePlayer);
                    log.Score = new List<int>(BattleDataMediator.Instance.Score);
                }
                    break;
                case BattleConst.DigestTiming.GoalGameSet:
                {
                    log.Type = BattleConst.DigestType.Goal_GameSet;
                }
                    break;
                case BattleConst.DigestTiming.SecondBall:
                {
                    // セカンドボール
                    // メイン：ボールを獲得したキャラ
                    // その他：メインキャラ以外のセカンドボール参加者1~3名
                    log.Type = BattleConst.DigestType.SecondBall2;
                    if (result.JoinLooseBallCompetitionCharacterIds.Count > 3)
                    {
                        log.Type = BattleConst.DigestType.SecondBall4;
                    }
                    else if (result.JoinLooseBallCompetitionCharacterIds.Count > 2)
                    {
                        log.Type = BattleConst.DigestType.SecondBall3;
                    }
                    else
                    {
                        log.Type = BattleConst.DigestType.SecondBall2;
                    }

                    // メイン
                    var character = BattleDataMediator.Instance.GetBattleCharacter(result.NextBallOwnerId);
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(character);

                    // その他
                    BattleDigestCharacterData reverChara = null;
                    foreach (var id in result.JoinLooseBallCompetitionCharacterIds)
                    {
                        if (id == result.NextBallOwnerId)
                        {
                            continue;
                        }

                        character = BattleDataMediator.Instance.GetBattleCharacter(id);
                        if (reverChara == null && character.Side != log.MainCharacterData.Side)
                        {
                            reverChara = new BattleDigestCharacterData();
                            reverChara.SetData(character);
                            log.OtherCharacterDataList.Insert(0, reverChara);
                        }
                        else
                        {
                            var data = new BattleDigestCharacterData();
                            data.SetData(character);
                            log.OtherCharacterDataList.Add(data);
                        }
                    }
                }
                    break;
                case BattleConst.DigestTiming.OutBall:
                    // アウトボール
                    // メイン：なし
                    // その他：なし
                    log.Type = BattleConst.DigestType.OutBall;
                    break;
                case BattleConst.DigestTiming.ThrowIn:
                    //  スローイン
                    // メイン：スローインしたキャラ
                    // その他：なし
                    // キャプテン以外の適当なキャラ
                    if (result.BallInterferenceType == BattleConst.BallInterferenceType.Catched)
                    {
                        log.Type = BattleConst.DigestType.ThrowInKeeper;
                    }
                    else
                    {
                        log.Type = BattleConst.DigestType.ThrowIn;
                        var throwInCharacter = BattleDataMediator.Instance.Decks[(int)side][BattleGameLogic.GetNonStateRandomValue(1, 5)];
                        log.MainCharacterData = new BattleDigestCharacterData();
                        log.MainCharacterData.SetData(throwInCharacter);
                    }

                    break;
                case BattleConst.DigestTiming.TimeUp:
                    log.Type = BattleConst.DigestType.TimeUp;
                    break;
                case BattleConst.DigestTiming.Special:
                    log.Type = BattleConst.DigestType.Special;
                    log.MainCharacterData = new BattleDigestCharacterData();
                    log.MainCharacterData.SetData(triggeredCharacter);
                    log.MainCharacterData.AbilityId = abilityId;

                    if (BattleDataMediator.Instance.NextMatchUpResult.OffenceAbilityId == abilityId)
                    {
                        log.OffenceAbilityId = BattleDataMediator.Instance.NextMatchUpResult.OffenceAbilityId;
                        log.AbilityTiming = BattleDataMediator.Instance.NextMatchUpResult.ActivateAbilityTimingType;
                    }
                    break;
                default:
                    return null;
            }

            return log;
        }

        private int viewedIndex = 0;
        private List<BattleLog> ViewableLog = new List<BattleLog>();
        public List<BattleLog> GetViewableLog()
        {
            if (viewedIndex >= BattleLogs.Count)
            {
                return null;
            }
            
            ViewableLog.Add(BattleLogs[viewedIndex]);
            viewedIndex++;
            return ViewableLog;
        }

        public BattleLog GetNextLog()
        {
            if (viewedIndex >= BattleLogs.Count)
            {
                return null;
            }

            return BattleLogs[viewedIndex];
        }

        public void RemoveLog(BattleLog log)
        {
            if (log == null)
            {
                return;
            }

            // スキップ時はログ再生用にリムーブしちゃまずいので.
            if (BattleDataMediator.Instance.IsSkipToFinishWithoutView)
            {
                return;
            }
            
            BattleLogs.Remove(log);
            ViewableLog.Remove(log);
            viewedIndex--;
        }

        public void RemoveCachedLogDigest()
        {
            var isSkipToFinish = BattleDataMediator.Instance.IsSkipToFinish;
            foreach (var log in BattleLogs)
            {
                if (log.DigestLog == null || log.LogTiming == BattleConst.LogTiming.OnBattleEnd)
                {
                    continue;
                }

                if (log.LogTiming != BattleConst.LogTiming.OnMatchUp)
                {
                    log.DigestLog = null;
                    continue;
                }
                
                if (isSkipToFinish)
                {
                    log.DigestLog.Type = BattleConst.DigestType.None;
                    continue;
                }

                if (log.DigestLog.OffenceSide == BattleConst.TeamSide.Right || !(log.DigestLog.MainCharacterData?.IsAce ?? false))
                {
                    log.DigestLog = null;
                }
            }
        }

        private void ResetTemporaryLog()
        {
            temporaryDigestLogs = new List<BattleLog>();
        }

        private void AddDigestLog()
        {
            var battleEndLog = new BattleLog();
            battleEndLog.LogTiming = BattleConst.LogTiming.OnBattleEnd;
            temporaryDigestLogs.Add(battleEndLog);
            
            // 途中からスキップした場合の考慮.
            // ログが重複してしまうので重複していたら再生用にリムーブ. クロス経由の場合もシュートが複数個あるはず.
            if (temporaryDigestLogs.Count(log => log.DigestLog?.Type is BattleConst.DigestType.Shoot) >= 2)
            {
                var targetLog = temporaryDigestLogs.FirstOrDefault(log => log.DigestLog?.Type is BattleConst.DigestType.MatchUp);
                temporaryDigestLogs.Remove(targetLog);
                targetLog = temporaryDigestLogs.FirstOrDefault(log => log.DigestLog?.Type is BattleConst.DigestType.Cross);
                temporaryDigestLogs.Remove(targetLog);
                targetLog = temporaryDigestLogs.FirstOrDefault(log => log.DigestLog?.Type is BattleConst.DigestType.Shoot);
                temporaryDigestLogs.Remove(targetLog);
            }

            var digestLog = new DigestLog();
            digestLog.Index = DigestLogLists.Count;
            digestLog.ScoreIndex = BattleDataMediator.Instance.Score[(int)BattleDataMediator.Instance.OffenceSide];
            digestLog.BattleLogList = temporaryDigestLogs;
            
            var crossLog = temporaryDigestLogs.FirstOrDefault(log => log.DigestLog?.Type == BattleConst.DigestType.Cross);
            var shootLog = temporaryDigestLogs.FirstOrDefault(log => log.DigestLog?.Type == BattleConst.DigestType.Shoot);
            if (crossLog != null)
            {
                digestLog.IsCross = true;
                var primaryCharaData = crossLog.DigestLog.MainCharacterData;
                var secondCharaData = crossLog.DigestLog.OtherCharacterDataList.FirstOrDefault();
                var primaryChara = BattleDataMediator.Instance.Decks[(int)primaryCharaData.Side].FirstOrDefault(chara => chara.MCharaId == primaryCharaData.MCharaId);
                var secondChara = BattleDataMediator.Instance.Decks[(int)secondCharaData.Side].FirstOrDefault(chara => chara.MCharaId == secondCharaData.MCharaId);

                digestLog.PrimaryCharacter = primaryChara;
                digestLog.SecondCharacter = secondChara;
                
                digestLog.ReferencedCharacterIds.Add(primaryChara.id);
                digestLog.ReferencedCharacterIds.Add(secondChara.id);
            }
            else if(shootLog != null)
            {
                var primaryCharaData = shootLog.DigestLog.MainCharacterData;
                var secondCharaData = shootLog.DigestLog.OtherCharacterDataList.FirstOrDefault();
                var primaryChara = BattleDataMediator.Instance.Decks[(int)primaryCharaData.Side].FirstOrDefault(chara => chara.MCharaId == primaryCharaData.MCharaId);
                var secondChara = BattleDataMediator.Instance.Decks[(int)secondCharaData.Side].FirstOrDefault(chara => chara.MCharaId == secondCharaData.MCharaId);
                
                digestLog.PrimaryCharacter = primaryChara;
                digestLog.SecondCharacter = secondChara;
                
                digestLog.ReferencedCharacterIds.Add(primaryChara.id);
            }
            
            // クロス && ダイレクトシュート必殺技両方のケア
            var abilityLog = temporaryDigestLogs.LastOrDefault(log => log.DigestLog != null && log.DigestLog.Type == BattleConst.DigestType.Special && log.DigestLog.MainCharacterData.Side == log.DigestLog.OffenceSide);
            if (abilityLog != null)
            {
                digestLog.AbilityId = abilityLog.DigestLog.MainCharacterData.AbilityId;
            }
            
            DigestLogLists.Add(digestLog);

            ResetTemporaryLog();
        }

        public void AddDigestLogByReplayLogs(List<BattleLog> battleLogs)
        {
            var battleLogList = new List<BattleLog>();
            foreach (var battleLog in battleLogs)
            {
                // ゴールリプレイはドリブルが起点となるので, ドリブルだったらリセット
                var digestLogType = battleLog.DigestLog?.Type ?? BattleConst.DigestType.None;
                if (digestLogType == BattleConst.DigestType.DribbleL ||
                    digestLogType == BattleConst.DigestType.DribbleR)
                {
                    battleLogList.Clear();
                }

                battleLogList.Add(battleLog);

                if (digestLogType != BattleConst.DigestType.Goal)
                {
                    continue;
                }

                var battleEndLog = new BattleLog();
                battleEndLog.LogTiming = BattleConst.LogTiming.OnBattleEnd;
                battleLogList.Add(battleEndLog);
                
                var score = battleLog.DigestLog.Score;
                var digestLog = new DigestLog();
                digestLog.Index = DigestLogLists.Count;
                digestLog.ScoreIndex = score[(int)battleLog.OffenceSide];
                digestLog.BattleLogList = battleLogList;

                var crossLog = battleLogList.FirstOrDefault(log => log.DigestLog?.Type == BattleConst.DigestType.Cross);
                var shootLog = battleLogList.FirstOrDefault(log => log.DigestLog?.Type == BattleConst.DigestType.Shoot);
                if (crossLog != null)
                {
                    digestLog.IsCross = true;
                    var primaryCharaData = crossLog.DigestLog.MainCharacterData;
                    var secondCharaData = crossLog.DigestLog.OtherCharacterDataList.FirstOrDefault();
                    var primaryChara = BattleDataMediator.Instance.Decks[(int)primaryCharaData.Side]
                        .FirstOrDefault(chara => chara.MCharaId == primaryCharaData.MCharaId);
                    var secondChara = BattleDataMediator.Instance.Decks[(int)secondCharaData.Side]
                        .FirstOrDefault(chara => chara.MCharaId == secondCharaData.MCharaId);

                    digestLog.PrimaryCharacter = primaryChara;
                    digestLog.SecondCharacter = secondChara;

                    digestLog.ReferencedCharacterIds.Add(primaryChara.id);
                    digestLog.ReferencedCharacterIds.Add(secondChara.id);
                }
                else if (shootLog != null)
                {
                    var primaryCharaData = shootLog.DigestLog.MainCharacterData;
                    var secondCharaData = shootLog.DigestLog.OtherCharacterDataList.FirstOrDefault();
                    var primaryChara = BattleDataMediator.Instance.Decks[(int)primaryCharaData.Side]
                        .FirstOrDefault(chara => chara.MCharaId == primaryCharaData.MCharaId);
                    var secondChara = BattleDataMediator.Instance.Decks[(int)secondCharaData.Side]
                        .FirstOrDefault(chara => chara.MCharaId == secondCharaData.MCharaId);

                    digestLog.PrimaryCharacter = primaryChara;
                    digestLog.SecondCharacter = secondChara;

                    digestLog.ReferencedCharacterIds.Add(primaryChara.id);
                }

                // クロス && ダイレクトシュート必殺技両方のケア
                var abilityLog = battleLogList.LastOrDefault(log =>
                    log.DigestLog != null && log.DigestLog.Type == BattleConst.DigestType.Special &&
                    log.DigestLog.MainCharacterData.Side == log.DigestLog.OffenceSide);
                if (abilityLog != null)
                {
                    digestLog.AbilityId = abilityLog.DigestLog.MainCharacterData.AbilityId;
                }

                DigestLogLists.Add(digestLog);
                battleLogList = new List<BattleLog>();
            }
        }

        public void PrepareForReplay(int index)
        {
            viewedIndex = 0;

            if (index == -1)
            {
                ViewableLog.Clear();
            }
            else
            {
                BattleLogs.Clear();
                ViewableLog.Clear();
                BattleLogs.AddRange(DigestLogLists[index].BattleLogList);
            }
        }
    }
}   