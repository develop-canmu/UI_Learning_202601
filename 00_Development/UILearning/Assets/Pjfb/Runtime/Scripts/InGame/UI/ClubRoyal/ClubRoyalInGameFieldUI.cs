using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameFieldUI : MonoBehaviour
    {
        [SerializeField] private Transform characterModelRoot;
        [SerializeField] private RectTransform characterUIRoot;
        [SerializeField] private RectTransform spotUIRoot;
        [SerializeField] private RectTransform threeDLayerRoot;
        [SerializeField] private RectTransform threeDRawImageTransform;
        [SerializeField] private GameObject dummySpotUIObject;
        [SerializeField] private ClubRoyalInGameSpotUI spotUIPrefab;
        [SerializeField] private ClubRoyalInGameSpotModelUI spotModelPrefab;
        [SerializeField] private ClubRoyalInGameFieldGroupUI groupUIPrefab;
        [SerializeField] private ClubRoyalInGameFieldBattleResultUI[] fieldBattleResultUIs;
        
        public float PositionXMin;
        public float PositionXMax;
        public float PositionYMin;
        public float PositionYMax;
        
        public float ViewPositionYDiff; // 背景が中央ではなく中央やや上部に寄っているので, 画面上部からのオフセッドとして定義

        private float PositionXRange;
        private float PositionXHalf;
        private float PositionYRange;
        private float PositionYHalf;

        private int mapWidth;
        
        private bool isInitialized = false;

        private Dictionary<int, ClubRoyalInGameSpotUI> spotUIs = new Dictionary<int, ClubRoyalInGameSpotUI>();
        private Dictionary<int, ClubRoyalInGameSpotModelUI> spotModelUIs = new Dictionary<int, ClubRoyalInGameSpotModelUI>();
        private Dictionary<GuildBattleCommonConst.GuildBattleTeamSide, List<ClubRoyalInGameFieldGroupUI>> groupUIDictionary = new Dictionary<GuildBattleCommonConst.GuildBattleTeamSide, List<ClubRoyalInGameFieldGroupUI>>()
        {
            { GuildBattleCommonConst.GuildBattleTeamSide.Left, new List<ClubRoyalInGameFieldGroupUI>() },
            { GuildBattleCommonConst.GuildBattleTeamSide.Right, new List<ClubRoyalInGameFieldGroupUI>() }
        };

        private Dictionary<int, Vector3> uiPositionDictionary = new Dictionary<int, Vector3>();
        private Dictionary<int, Vector3> modelPositionDictionary = new Dictionary<int, Vector3>();

        private const int DistanceFromCamera = 20;
        public const float BattleDirectionDuration = 3.5f;
        
        public void Initialize(GuildBattleFieldModel fieldModel)
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            
            PositionXRange = PositionXMax - PositionXMin;
            PositionXHalf = PositionXRange / 2.0f;
            PositionYRange = PositionYMax - PositionYMin;
            PositionYHalf = PositionYRange / 2.0f;

            InitializeSpotUI(fieldModel.MapSpotsDictionary.Values);
            FixViewPosition();
        }

        private Vector3 Get3DPositionFromCanvasPosition(Vector3 canvasPosition)
        {
            var viewPortPosition = Camera.main.WorldToViewportPoint(canvasPosition);
            // ViewportPoint -> WorldSpaceにする都合上, どうしてもキャラクターの描画位置がRawImageの中央ではなく上部に寄ってしまうので, それをFixするためにずらす.
            // RawImageの中央にキャラクターが来る形にならないと画面サイズの違う端末で描画したときにずれてしまうので.
            return AppManager.Instance.WorldManager.ThreeDWorldCamera.ViewportToWorldPoint(viewPortPosition);// + new Vector3(0, -2.8f, 0);
        }
        
        private Vector2 GetCanvasPosition(int targetPositionX, bool isBase, int maxDistance, GuildBattleCommonConst.GuildBattleTeamSide mySide, int laneIndex, int laneCountIgnoreBase, int satellitePositionX)
        {
            var positionX = 0.0f;
            var positionY = 0.0f;

            if (isBase)
            {
                if (targetPositionX > 0)
                {
                    positionX = PositionXMax;
                }
                else
                {
                    positionX = PositionXMin;
                }
            }
            else
            {
                positionX = PositionXRange * (targetPositionX / (float)maxDistance) - PositionXHalf;
                // 本拠地レーンは常に中央配置
                if (laneIndex >= 0)
                {
                    positionY = PositionYRange * (1 - (float)laneIndex / (laneCountIgnoreBase - 1)) - PositionYHalf;
                    var diff = Math.Min(targetPositionX, mapWidth - targetPositionX);
                    if (diff < satellitePositionX)
                    {
                        var diffPosY = PositionYHalf * (1 - ((float)diff / satellitePositionX));
                        positionY += laneIndex == 0 ? -diffPosY : diffPosY;
                    }
                }
            }

            if (mySide == GuildBattleCommonConst.GuildBattleTeamSide.Right)
            {
                positionX *= -1;
            }
            
            return new Vector2(positionX, positionY);
        }

        private void InitializeSpotUI(ICollection<GuildBattleMapSpotModel> mapSpots)
        {
            var mySide = PjfbGuildBattleDataMediator.Instance.PlayerSide;
            mapWidth = mapSpots.Max(spotModel => spotModel.PositionX);
            RegisterPositions(mapSpots);
            
            foreach (var spotModel in mapSpots)
            {
                var spotUI = Instantiate(spotUIPrefab, spotUIRoot);
                spotUI.Initialize(spotModel);
                var positionId = GuildBattleCommonLogic.GetLanePositionId(spotModel.LaneNumber, spotModel.PositionX);
                spotUI.transform.localPosition = GetUIPosition(positionId);
                if (spotModel.OccupyingSide == mySide)
                {
                    spotUI.transform.localPosition += new Vector3(-100, 0, 0);
                }
                else
                {
                    spotUI.transform.localPosition += new Vector3(100, 0, 0);
                }
                spotUIs.Add(spotModel.Id, spotUI);

                var spotModelUI = Instantiate(spotModelPrefab, characterModelRoot);
                spotModelUI.Initialize(mySide == spotModel.OccupyingSide,spotModel.IsBase);
                spotModelUI.transform.position = GetModelPosition(positionId);
                spotModelUIs.Add(spotModel.Id, spotModelUI);
            }
        }

        private void FixViewPosition()
        {
            // 画面サイズによって3D画像のサイズが変わる && 表示位置が中央からオフセットしている && 3DとUIの表示を混ぜるためにViewPoint->WorldPointを使っている都合上,
            // 諸々を計算しきってから表示位置を画面上部からのオフセットとして配置する.
            threeDLayerRoot.anchorMin = new Vector2(0.5f, 1.0f);
            threeDLayerRoot.anchorMax = new Vector2(0.5f, 1.0f);
            threeDLayerRoot.pivot = new Vector2(0.5f, 1.0f);
            threeDLayerRoot.anchoredPosition = new Vector3(0, ViewPositionYDiff, 0);
            
            spotUIRoot.anchorMin = new Vector2(0.5f, 1.0f);
            spotUIRoot.anchorMax = new Vector2(0.5f, 1.0f);
            spotUIRoot.pivot = new Vector2(0.5f, 1.0f);
            spotUIRoot.anchoredPosition = new Vector3(0, ViewPositionYDiff, 0);
            
            characterUIRoot.anchorMin = new Vector2(0.5f, 1.0f);
            characterUIRoot.anchorMax = new Vector2(0.5f, 1.0f);
            characterUIRoot.pivot = new Vector2(0.5f, 1.0f);
            characterUIRoot.anchoredPosition = new Vector3(0, ViewPositionYDiff, 0);
        }

        private Vector3 GetUIPosition(int positionId)
        {
            if (uiPositionDictionary.TryGetValue(positionId, out var vec))
            {
                return vec;
            }
            
            return Vector3.zero;
        }
        
        private Vector3 GetModelPosition(int positionId)
        {
            if (modelPositionDictionary.TryGetValue(positionId, out var vec))
            {
                return vec;
            }
            
            return Vector3.zero;
        }

        private void RegisterPositions(ICollection<GuildBattleMapSpotModel> mapSpots)
        {
            var laneNumbers = new List<int>();
            foreach (var mapSpot in mapSpots)
            {
                if (!laneNumbers.Contains(mapSpot.LaneNumber))
                {
                    laneNumbers.Add(mapSpot.LaneNumber);
                }
            }

            var satelliteSpotPositionX =
                mapSpots.OrderBy(spot => spot.PositionX).FirstOrDefault(spot => !spot.IsBase).PositionX;
            
            var movementValue = PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleMovementValue;
            var mySide = PjfbGuildBattleDataMediator.Instance.PlayerSide;
            foreach (var laneNum in laneNumbers)
            {
                // 本拠地を含むレーンを入れているので-1
                var laneIndex = laneNumbers.IndexOf(laneNum) - 1;
                var maxPositionIndex = mapWidth / movementValue;
                for (var i = 0; i <= maxPositionIndex; i++)
                {
                    var targetPositionX = movementValue * i;
                    var isBase = i == 0 || i == maxPositionIndex ;
                    var position = GetCanvasPosition(targetPositionX, isBase, mapWidth, mySide, laneIndex, laneNumbers.Count - 1, satelliteSpotPositionX);
                    var posId = GuildBattleCommonLogic.GetLanePositionId(laneNum, targetPositionX);
                    uiPositionDictionary.Add(posId, position);

                    /*
                    // デバッグ用
                    var dummy = Instantiate(dummySpotUIObject, dummySpotUIObject.transform.parent);
                    dummy.transform.localPosition = position;
                    dummy.AddComponent<Image>();
                    dummy.gameObject.SetActive(true);
                    */
                    
                    dummySpotUIObject.transform.localPosition = position;
                    dummySpotUIObject.transform.position = new Vector3(
                        dummySpotUIObject.transform.position.x,
                        dummySpotUIObject.transform.position.y,
                        DistanceFromCamera);
                    var modelPosition = Get3DPositionFromCanvasPosition(dummySpotUIObject.transform.position);
                    modelPositionDictionary.Add(posId, modelPosition);
                }
            }
        }

        private void ClearCreatedGroupUIParties()
        {
            foreach (var groupUIs in groupUIDictionary.Values)
            {
                foreach (var groupUI in groupUIs)
                {
                    groupUI.ClearParty();
                    groupUI.SetWillUpdateUI(true);
                }
            }
        }
        
        public void UpdateBeforeFightUnitView(GuildBattleFieldSituationModel latestSituation)
        {
            ClearCreatedGroupUIParties();
            foreach (var party in latestSituation.PjfbParties)
            {
                // マップ上にいる or 今回敗北した or 今回出陣した
                var isLoseParty = latestSituation.LosePartyIds.Contains(party.PartyId);
                var isDepartedParty = latestSituation.DepartedPartyIds.Contains(party.PartyId);
                var isShow = party.IsOnMap() || isLoseParty || isDepartedParty || latestSituation.ScoredPartyIds.Contains(party.PartyId);

                if (!isShow)
                {
                    continue;
                }

                var positionId = isLoseParty ? party.GetLaneAndPositionId() : party.GetLastLaneAndPositionId();
                var (doInitialize, groupUI) = GetOrCreateGroupUI(party.Side, positionId);

                // 新規表示なら初期位置設定.
                // 兵隊含めたアクティブのオンオフはFixUIで行っているが, 何度も処理する必要もないのでマーク用にここでアクティブセット
                if (!groupUI.gameObject.activeInHierarchy)
                {
                    groupUI.SetPositionId(positionId);
                    groupUI.SetInitialPosition(GetUIPosition(positionId), GetModelPosition(positionId));
                    groupUI.SetActiveIncludeUnits(true);
                }

                // 途中入場で初回表示の場合は連撃表示を出しておく.
                if (doInitialize && party.IsWinStreakCountGreaterThanCommendWinStreakCountForLog())
                {
                    var winStreakCountForLog = PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.CommendWinStreakCountForLog;
                    groupUI.PlayWinStreakUI((party.WinStreakCount / winStreakCountForLog) * winStreakCountForLog, true);
                }

                groupUI.AddParty(party);
                groupUI.SetWillUpdateUI(true);
            }

            FixGroupUI(true);
        }
        
        public void UpdateDeadUnitView(GuildBattleFieldSituationModel latestSituation)
        {
            foreach (var partyId in latestSituation.LosePartyIds)
            {
                var party = latestSituation.PjfbParties.FirstOrDefault(p => p.PartyId == partyId);
                if (party == null)
                {
                    continue;
                }

                var (doInitialize, groupUI) = GetOrCreateGroupUI(party.Side, party.GetLaneAndPositionId());
                groupUI.RemoveParty(party);
                groupUI.SetWillUpdateUI(true);
            }
            
            foreach (var partyId in latestSituation.ScoredPartyIds)
            {
                var party = latestSituation.PjfbParties.FirstOrDefault(p => p.PartyId == partyId);
                if (party == null)
                {
                    continue;
                }

                var (doInitialize, groupUI) = GetOrCreateGroupUI(party.Side, party.GetLaneAndPositionId());
                groupUI.RemoveParty(party);
                groupUI.SetWillUpdateUI(true);
            }
            
            FixGroupUI(false);
        }
        
        public void UpdateMoveUnitView(GuildBattleFieldSituationModel latestSituation)
        {
            foreach (var groupUIs in groupUIDictionary.Values)
            {
                for (var i = 0; i < groupUIs.Count; i++)
                {
                    var groupUI = groupUIs[i];
                    if (!groupUI.gameObject.activeSelf)
                    {
                        continue;
                    }
                    
                    if (groupUI.NeedSeparateUnits())
                    {
                        var moveParties = groupUI.GetWillMoveParties();
                        var moveParty = moveParties.FirstOrDefault();
                        var (doInitialize, newGroupUI) = GetOrCreateGroupUI(moveParty.Side, moveParty.GetLaneAndPositionId());

                        foreach (var party in moveParties)
                        {
                            groupUI.RemoveParty(party);
                            newGroupUI.AddParty(party);
                        }
                        
                        groupUI.SetWillUpdateUI(true);
                        newGroupUI.SetWillUpdateUI(true);
                        var lastPositionId = moveParty.GetLastLaneAndPositionId();
                        newGroupUI.SetInitialPosition(GetUIPosition(lastPositionId), GetModelPosition(lastPositionId));
                        var positionId = moveParty.GetLaneAndPositionId();

                        var willDisappearAtHalf = latestSituation.PjfbParties.Any(party =>
                            party.GetLaneAndPositionId() == moveParty.GetLaneAndPositionId() && party.LastXPosition != moveParty.LastXPosition && party.IsOnMap());
                        //var isTargetPositionUIShowing = groupUIDictionary[moveParty.Side].Any(ui => ui.gameObject.activeSelf && ui.PositionId == positionId);
                        newGroupUI.SetMoveTargetPosition(GetUIPosition(positionId), GetModelPosition(positionId), willDisappearAtHalf);
                        newGroupUI.SetPositionId(moveParty.GetLaneAndPositionId());
                        continue;
                    }

                    if (groupUI.NeedMovePosition())
                    {
                        var moveParty = groupUI.GetFirstParty();
                        var positionId = moveParty.GetLaneAndPositionId();
                        var willDisappearAtHalf = latestSituation.PjfbParties.Any(party =>
                            party.GetLaneAndPositionId() == moveParty.GetLaneAndPositionId() && party.LastXPosition != moveParty.LastXPosition && party.IsOnMap());
                        //var isTargetPositionUIShowing = groupUIDictionary[moveParty.Side].Any(ui => ui.gameObject.activeSelf && ui.PositionId == positionId);
                        groupUI.SetMoveTargetPosition(GetUIPosition(positionId), GetModelPosition(positionId), willDisappearAtHalf);
                        groupUI.SetPositionId(moveParty.GetLaneAndPositionId());
                    }
                }
            }
            
            FixGroupUI(false);
        }
        
        private void FixGroupUI(bool showLastMilitaryStrength)
        {
            foreach (var groupUIs in groupUIDictionary.Values)
            {
                foreach (var groupUI in groupUIs)
                {
                    if (groupUI.DoUpdate)
                    {
                        groupUI.SetActiveIncludeUnits(true);
                    }
                    
                    if (!groupUI.gameObject.activeSelf || !groupUI.DoUpdate)
                    {
                        continue;
                    }
                    
                    if (groupUI.HaveParty())
                    {
                        groupUI.SetActiveIncludeUnits(true);
                        groupUI.FixUI(showLastMilitaryStrength);
                    }
                    else
                    {
                        groupUI.SetActiveIncludeUnits(false);
                    }
                }
            }
        }

        private (bool, ClubRoyalInGameFieldGroupUI) GetOrCreateGroupUI(GuildBattleCommonConst.GuildBattleTeamSide side, int positionId)
        {
            var groupUIs = groupUIDictionary[side];

            ClubRoyalInGameFieldGroupUI reuseObj = null;
            foreach (var groupUI in groupUIs)
            {
                // 指定したPositionId(PosXとLaneNumberの組み合わせ)のものがあればそれを返す.
                if (groupUI.gameObject.activeSelf && groupUI.PositionId == positionId)
                {
                    return (false, groupUI);
                }

                // 生成済みで現在使ってないものがあればマーク
                if (!groupUI.gameObject.activeSelf && reuseObj == null)
                {
                    reuseObj = groupUI;
                }
            }

            if (reuseObj != null)
            {
                //reuseObj.SetArmyState(BattleConst.UnitUIState.Idle);
                //reuseObj.transform.SetAsFirstSibling();
                return (true, reuseObj);
            }

            var newGroupUI = Instantiate(groupUIPrefab, characterModelRoot);
            newGroupUI.gameObject.SetActive(false);
            newGroupUI.Initialize(side == PjfbGuildBattleDataMediator.Instance.PlayerSide, spotUIRoot);
            //newGroupUI.transform.SetAsFirstSibling();
            groupUIs.Add(newGroupUI);
            return (true, newGroupUI);
        }
        
        private ClubRoyalInGameFieldGroupUI GetActiveGroupUI(GuildBattleCommonConst.GuildBattleTeamSide side, int partyId)
        {
            var groupUIs = groupUIDictionary[side];
            foreach (var groupUI in groupUIs)
            {
                if (!groupUI.gameObject.activeSelf)
                {
                    continue;
                }

                if (groupUI.PartyIds.Contains(partyId))
                {
                    return groupUI;
                }
            }

            return null;
        }

        public void UpdateMapSpotView(Dictionary<int, GuildBattleMapSpotModel> mapSpotsDictionary)
        {
            foreach (var kvp in mapSpotsDictionary)
            {
                if (spotUIs.TryGetValue(kvp.Key, out var spotUI))
                {
                    spotUI.UpdateView(kvp.Value);
                }

                if (spotModelUIs.TryGetValue(kvp.Key, out var spotModelUI))
                {
                    spotModelUI.UpdateView(kvp.Value);
                }
            }
        }

        public async UniTask PlayBattleEffect(GuildBattleFieldSituationModel latestFieldSituation)
        {
            var isPlayingEffect = false;
            foreach (var matchingPartyIds in latestFieldSituation.EachLaneMatchingPartyIds)
            {
                // failsafe. 絶対2つあるはず.
                if (matchingPartyIds.Count == 2)
                {
                    var winPartyId = 0;
                    var losePartyId = 0;
                    if (latestFieldSituation.LosePartyIds.Contains(matchingPartyIds[0]))
                    {
                        winPartyId = matchingPartyIds[1];
                        losePartyId = matchingPartyIds[0];
                    }
                    else
                    {
                        winPartyId = matchingPartyIds[0];
                        losePartyId = matchingPartyIds[1];
                    }

                    PlayBattleEffect(latestFieldSituation, winPartyId, losePartyId).Forget();
                    isPlayingEffect = true;
                }
            }

            foreach (var scoredPartyId in latestFieldSituation.ScoredPartyIds)
            {
                PlayGoalEffect(latestFieldSituation, scoredPartyId);
                isPlayingEffect = true;
            }

            if (isPlayingEffect)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(BattleDirectionDuration), cancellationToken: destroyCancellationToken);
            }
        }

        private async UniTask PlayBattleEffect(GuildBattleFieldSituationModel latestFieldSituation, int winPartyId, int losePartyId)
        {
            // この時点ではGuildBattleDataMediator.Instance.BattlePartiesは更新していないので受け取ったデータからとる.
            var winParty = latestFieldSituation.PjfbParties.FirstOrDefault(party => party.PartyId == winPartyId);
            var loseParty = latestFieldSituation.PjfbParties.FirstOrDefault(party => party.PartyId == losePartyId);
            if (winParty == null || loseParty == null)
            {
                return;
            }

            var winGroupUI = GetActiveGroupUI(winParty.Side, winParty.PartyId);
            var loseGroupUI = GetActiveGroupUI(loseParty.Side, loseParty.PartyId);
            if (winGroupUI == null || loseGroupUI == null)
            {
                return;
            }

            var finalBallCount = winParty.GetBallCount();

            var isRecovered = false;
            var allyScore = 0;
            var enemyScore = 0;
            var winTeamBallCountAfterMatchUp = 0;
            var loseTeamBallCountAfterMatchUp = 0;
            if (winParty.Side == PjfbGuildBattleDataMediator.Instance.PlayerSide)
            {
                latestFieldSituation.BattleResultScores.TryGetValue(winParty.PartyId, out allyScore);
                latestFieldSituation.BattleResultScores.TryGetValue(loseParty.PartyId, out enemyScore);
                isRecovered = enemyScore > 0;
                winTeamBallCountAfterMatchUp = winParty.GetLastBallCount() - enemyScore;
                loseTeamBallCountAfterMatchUp = loseParty.GetLastBallCount() - allyScore;
            }
            else
            {
                latestFieldSituation.BattleResultScores.TryGetValue(winParty.PartyId, out enemyScore);
                latestFieldSituation.BattleResultScores.TryGetValue(loseParty.PartyId, out allyScore);
                isRecovered = allyScore > 0;
                winTeamBallCountAfterMatchUp = winParty.GetLastBallCount() - allyScore;
                loseTeamBallCountAfterMatchUp = loseParty.GetLastBallCount() - enemyScore;
            }
            
            if (Math.Abs(winParty.XPosition - loseParty.XPosition) > 100)
            {
                await UniTask.NextFrame(cancellationToken: destroyCancellationToken);
                await PlayLongRangeBattleEffect(winGroupUI, loseGroupUI, isRecovered, finalBallCount, allyScore, enemyScore, winTeamBallCountAfterMatchUp, loseTeamBallCountAfterMatchUp, winParty.WinStreakCount);
            }
            else
            {
                await UniTask.NextFrame(cancellationToken: destroyCancellationToken);
                await PlayShortRangeBattleEffect(winGroupUI, loseGroupUI, isRecovered, finalBallCount, allyScore, enemyScore, winTeamBallCountAfterMatchUp, loseTeamBallCountAfterMatchUp, winParty.WinStreakCount);
            }

            if (winParty.IsDefendingAtAnySpot())
            {
                winGroupUI.ReturnBallModel();
            }
        }
        
        private void PlayGoalEffect(GuildBattleFieldSituationModel latestFieldSituation, int scoredPartyId)
        {
            // この時点ではGuildBattleDataMediator.Instance.BattlePartiesは更新していないので受け取ったデータからとる.
            var scoredParty = latestFieldSituation.PjfbParties.FirstOrDefault(party => party.PartyId == scoredPartyId);
            if (scoredParty == null)
            {
                return;
            }

            var scoredGroupUI = GetActiveGroupUI(scoredParty.Side, scoredParty.PartyId);
            if (scoredGroupUI == null)
            {
                return;
            }

            if (!spotModelUIs.TryGetValue(scoredParty.TargetSpotId, out var spotModelUI))
            {
                return;
            }
            
            if (!spotUIs.TryGetValue(scoredParty.TargetSpotId, out var spotUI))
            {
                return;
            }

            var spotInfo = latestFieldSituation.SpotOccupationInfo.FirstOrDefault(spotInfo => spotInfo.Id == scoredParty.TargetSpotId);
            if (spotInfo == null)
            {
                return;
            }

            var lastHp = PjfbGuildBattleDataMediator.Instance.BattleField.MapSpotsDictionary[spotInfo.Id].RemainHP;
            var damageDealt = lastHp - spotInfo.RemainHP;
            
            if (!PjfbGuildBattleDataMediator.Instance.OnMapPjfbBattleParties.TryGetValue(scoredParty.PartyId, out var lastPartyData))
            {
                return;
            }
            
            if (spotInfo.IsBroken)
            {
                PlayLastGoalEffect(scoredGroupUI, spotModelUI, spotUI, lastPartyData.GetBallCount(), damageDealt, scoredParty, spotInfo).Forget();
            }
            else
            {
                PlayAliveGoalEffect(scoredGroupUI, spotModelUI, spotUI, lastPartyData.GetBallCount(), damageDealt, scoredParty, spotInfo).Forget();
            }
        }

        private async UniTask PlayShortRangeBattleEffect(ClubRoyalInGameFieldGroupUI winGroupUI, ClubRoyalInGameFieldGroupUI loseGroupUI,
            bool isRecovered, int finalBallCount, int allyScore, int enemyScore, int winTeamBallCountAfterMatchUp, int loseTeamBallCountAfterMatchUp, int winStreakCount)
        {
            InitialBattleEffect(winGroupUI, loseGroupUI);
            var centerPosition = (winGroupUI.GetResultUIPosition() + loseGroupUI.GetResultUIPosition()) / 2.0f;
            // 開始位置は適当にどっちか.
            if (BattleGameLogic.GetNonStateRandomValue(0, 2) == 0)
            {
                PlayFieldBattleResultEffect(allyScore, enemyScore, true, centerPosition);
                await winGroupUI.SetBallPosition(winGroupUI.GetBallRootPosition());
                winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Pass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
                winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Pass2);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            }
            else
            {
                PlayFieldBattleResultEffect(allyScore, enemyScore, false, centerPosition);
                winGroupUI.SetBallRotation(true);
                await winGroupUI.SetBallPosition(loseGroupUI.GetBallRootPosition());
                winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Pass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
                winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Pass2);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
                winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.Pass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
            }

            await AfterBattleEffect(winGroupUI, loseGroupUI, isRecovered, finalBallCount, winTeamBallCountAfterMatchUp,
                loseTeamBallCountAfterMatchUp, false, winStreakCount);
        }
        
        private async UniTask PlayLongRangeBattleEffect(ClubRoyalInGameFieldGroupUI winGroupUI, ClubRoyalInGameFieldGroupUI loseGroupUI,
            bool isRecovered, int finalBallCount, int allyScore, int enemyScore, int winTeamBallCountAfterMatchUp, int loseTeamBallCountAfterMatchUp, int winStreakCount)
        {
            loseGroupUI.ReturnBallModel();
            winGroupUI.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.SideStep);
            loseGroupUI.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.SideStep);
            winGroupUI.PlayWordEffect(BattleConst.ClubRoyalWordEffectType.MatchUp).Forget();
            loseGroupUI.PlayWordEffect(BattleConst.ClubRoyalWordEffectType.MatchUp).Forget();
            winGroupUI.PlayHeaderLeaderWordPhraseBalloon(ClubRoyalInGamePartyLeaderUI.WordPhraseType.OnMatchUp);
            loseGroupUI.PlayHeaderLeaderWordPhraseBalloon(ClubRoyalInGamePartyLeaderUI.WordPhraseType.OnMatchUp);
            
            var centerPosition = (winGroupUI.GetResultUIPosition() + loseGroupUI.GetResultUIPosition()) / 2.0f;
            if (BattleGameLogic.GetNonStateRandomValue(0, 2) == 0)
            {
                await winGroupUI.SetBallPosition(winGroupUI.GetBallRootPosition());
                PlayFieldBattleResultEffect(allyScore, enemyScore, true, centerPosition);
                winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LongPass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
                winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LongPass2);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            }
            else
            {
                PlayFieldBattleResultEffect(allyScore, enemyScore, true, centerPosition);
                winGroupUI.SetBallRotation(true);
                await winGroupUI.SetBallPosition(loseGroupUI.GetBallRootPosition());
                winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LongPass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
                winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LongPass2);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
                winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.LongPass1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.33f), cancellationToken: destroyCancellationToken);
            }

            await AfterBattleEffect(winGroupUI, loseGroupUI, isRecovered, finalBallCount, winTeamBallCountAfterMatchUp,
                loseTeamBallCountAfterMatchUp, true, winStreakCount);
        }

        private void InitialBattleEffect(ClubRoyalInGameFieldGroupUI winGroupUI, ClubRoyalInGameFieldGroupUI loseGroupUI)
        {
            loseGroupUI.ReturnBallModel();
            winGroupUI.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.SideStep);
            loseGroupUI.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.SideStep);
            winGroupUI.PlayWordEffect(BattleConst.ClubRoyalWordEffectType.MatchUp).Forget();
            loseGroupUI.PlayWordEffect(BattleConst.ClubRoyalWordEffectType.MatchUp).Forget();
            winGroupUI.PlayHeaderLeaderWordPhraseBalloon(ClubRoyalInGamePartyLeaderUI.WordPhraseType.OnMatchUp);
            loseGroupUI.PlayHeaderLeaderWordPhraseBalloon(ClubRoyalInGamePartyLeaderUI.WordPhraseType.OnMatchUp);            
        }

        private async UniTask AfterBattleEffect(ClubRoyalInGameFieldGroupUI winGroupUI, ClubRoyalInGameFieldGroupUI loseGroupUI,
            bool isRecovered, int finalBallCount, int winTeamBallCountAfterMatchUp, int loseTeamBallCountAfterMatchUp, bool isLongRange, int winStreakCount)
        {
            winGroupUI.ResetBallPosition();
            winGroupUI.SetBallRotation(false);

            winGroupUI.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Shoot);
            winGroupUI.PlayBallAnimation(isLongRange ? BattleConst.ClubRoyalBallAnimationType.LongShootHitOpponent : BattleConst.ClubRoyalBallAnimationType.ShootHitOpponent);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            
            winGroupUI.SetHeadLeaderBallCountText(winTeamBallCountAfterMatchUp);
            loseGroupUI.SetHeadLeaderBallCountText(loseTeamBallCountAfterMatchUp);

            loseGroupUI.PlayEffect(BattleConst.ClubRoyalBattleEffectType.DamageTaken).Forget();
            loseGroupUI.PlayWordEffect(BattleConst.ClubRoyalWordEffectType.Hit).Forget();
            loseGroupUI.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Dead);
            loseGroupUI.PlayCharacterBlinkAnimation().Forget();
            loseGroupUI.PlayHeadLeaderUIAnimation(ClubRoyalInGamePartyLeaderUI.AnimationType.Lose);
            winGroupUI.PlayHeaderLeaderWordPhraseBalloon(ClubRoyalInGamePartyLeaderUI.WordPhraseType.OnBeatEnemy);
            
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: destroyCancellationToken);
            ClubRoyalInGameUIMediator.Instance.PlayAbsorbBallUI(loseGroupUI.GetHeadLeaderUIPosition(), winGroupUI.GetHeadLeaderUIPosition(), null, destroyCancellationToken).Forget();
            loseGroupUI.PlayEffect(BattleConst.ClubRoyalBattleEffectType.DeadCharacter).Forget();

            if (winStreakCount % PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.CommendWinStreakCountForLog == 0)
            {
                winGroupUI.PlayWinStreakUI(winStreakCount, false);
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.8f), cancellationToken: destroyCancellationToken);
            
            winGroupUI.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Idle);
            winGroupUI.ResetModelPositionAndRotation();
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: destroyCancellationToken);
            winGroupUI.PlayHeadLeaderBallCountUpAnimation(isRecovered, finalBallCount);
        }
        
        private async UniTask PlayAliveGoalEffect(ClubRoyalInGameFieldGroupUI winGroupUI, ClubRoyalInGameSpotModelUI spotModelUI, ClubRoyalInGameSpotUI spotUI, int ballCount, int damageDealt,
            GuildBattlePartyModel scoredParty, GuildBattleCommonMapSpotModel spotModel)
        {
            winGroupUI.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Shoot);
            winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.ShootGoal);
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: destroyCancellationToken);
            if (BattleGameLogic.GetNonStateRandomValue(0, 2) == 0)
            {
                spotModelUI.PlayBlueRockManAnimation(ClubRoyalInGameSpotModelUI.BlueRockManAnimationType.JumpLeft);
            }
            else
            {
                spotModelUI.PlayBlueRockManAnimation(ClubRoyalInGameSpotModelUI.BlueRockManAnimationType.JumpRight);
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: destroyCancellationToken);
            spotUI.PlayDamagedAnimation();
            await UniTask.Delay(TimeSpan.FromSeconds(0.4f), cancellationToken: destroyCancellationToken);
            winGroupUI.PlayHeaderLeaderWordPhraseBalloon(ClubRoyalInGamePartyLeaderUI.WordPhraseType.OnBeatEnemy);
            winGroupUI.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Idle);
            winGroupUI.ResetModelPositionAndRotation();

            ClubRoyalInGameUIMediator.Instance.GoalCutInUI.PlayCutIn(scoredParty, spotModel, ballCount, damageDealt);
            await UniTask.Delay(TimeSpan.FromSeconds(0.8f), cancellationToken: destroyCancellationToken);
            winGroupUI.PlayCharacterBlinkAnimation().Forget();
            winGroupUI.PlayEffect(BattleConst.ClubRoyalBattleEffectType.DeadCharacter).Forget();
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            spotModelUI.PlayBlueRockManAnimation(ClubRoyalInGameSpotModelUI.BlueRockManAnimationType.Idle);
            await UniTask.NextFrame(cancellationToken: destroyCancellationToken);
            spotModelUI.ResetBlueRockManPositionAndRotation();
            
            if (scoredParty.PlayerIndex == PjfbGuildBattleDataMediator.Instance.PlayerIndex)
            {
                ClubRoyalInGameUIMediator.Instance.PlayAbsorbBallUI(spotUI.GetBallAbsorbStartPosition(), ClubRoyalInGameUIMediator.Instance.RemainBallCountUI.GetBallCountTextPosition(), null, destroyCancellationToken).Forget();
            }
        }
        
        private async UniTask PlayLastGoalEffect(ClubRoyalInGameFieldGroupUI winGroupUI, ClubRoyalInGameSpotModelUI spotModelUI, ClubRoyalInGameSpotUI spotUI, int ballCount, int damageDealt,
            GuildBattlePartyModel scoredParty, GuildBattleCommonMapSpotModel spotModel)
        {
            winGroupUI.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Shoot);
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: destroyCancellationToken);
            winGroupUI.PlayBallAnimation(BattleConst.ClubRoyalBallAnimationType.ShootHitBLM);
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: destroyCancellationToken);
            spotModelUI.PlayWordEffect(BattleConst.ClubRoyalWordEffectType.Hit).Forget();
            spotModelUI.PlayBlueRockManAnimation(ClubRoyalInGameSpotModelUI.BlueRockManAnimationType.Dead);
            spotModelUI.PlayBlueRockManBlinkAnimation().Forget();
            spotModelUI.PlayEffect(BattleConst.ClubRoyalBattleEffectType.DeadBLM).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            spotUI.PlayDamagedAnimation();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            
            winGroupUI.PlayHeaderLeaderWordPhraseBalloon(ClubRoyalInGamePartyLeaderUI.WordPhraseType.OnBeatEnemy);
            winGroupUI.PlayCharacterAnimation(ClubRoyalInGameFieldGroupUI.AnimationType.Idle);
            winGroupUI.ResetModelPositionAndRotation();
            ClubRoyalInGameUIMediator.Instance.GoalCutInUI.PlayCutIn(scoredParty, spotModel, ballCount, damageDealt);
            ClubRoyalInGameUIMediator.Instance.OccupySpotUI.PlayAnimation(GuildBattleCommonLogic.GetOtherTeamSide(scoredParty.Side));
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            winGroupUI.PlayCharacterBlinkAnimation().Forget();
            winGroupUI.PlayEffect(BattleConst.ClubRoyalBattleEffectType.DeadCharacter).Forget();
            
            if (scoredParty.PlayerIndex == PjfbGuildBattleDataMediator.Instance.PlayerIndex)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
                ClubRoyalInGameUIMediator.Instance.PlayAbsorbBallUI(spotUI.GetBallAbsorbStartPosition(), ClubRoyalInGameUIMediator.Instance.RemainBallCountUI.GetBallCountTextPosition(), null, destroyCancellationToken).Forget();
            }
        }

        private void PlayFieldBattleResultEffect(int allyScore, int enemyScore, bool isAllyFirst, Vector3 worldPosition)
        {
            var resultUI = fieldBattleResultUIs.FirstOrDefault(ui => !ui.gameObject.activeSelf);
            if (resultUI == null)
            {
                return;
            }

            resultUI.PlayAnimation(allyScore, enemyScore, isAllyFirst, worldPosition).Forget();
        }
    }
}