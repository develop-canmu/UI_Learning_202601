using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicOnion
{
    public static class GuildBattleCommonLogic
    {
        public static string GetHostAddress(string hostUrl, long hostPort)
        {
            var ret = string.Empty;

#if CRUFRAMEWORK_DEBUG
            if (hostUrl.Contains("localhost"))
            {
                ret = $"http://{hostUrl}:{hostPort}";
            }
            else
            {
                ret = $"https://{hostUrl}:{hostPort}";
            }
#else
            ret = $"https://{hostUrl}:{hostPort}";
#endif

            return ret;
        }
        
        public static GuildBattleCommonConst.GuildBattleTeamSide GetOtherTeamSide(GuildBattleCommonConst.GuildBattleTeamSide side)
        {
            switch (side)
            {
                case GuildBattleCommonConst.GuildBattleTeamSide.Left:
                    return GuildBattleCommonConst.GuildBattleTeamSide.Right;
                case GuildBattleCommonConst.GuildBattleTeamSide.Right:
                    return GuildBattleCommonConst.GuildBattleTeamSide.Left;
            }

            return GuildBattleCommonConst.GuildBattleTeamSide.All;
        }

        public static void SetPreMovePosition(ICollection<GuildBattleCommonPartyModel> battleParties, Dictionary<int, GuildBattleCommonMapSpotModel> mapSpots)
        {
            foreach (var party in battleParties)
            {
                if (!party.IsOnMap())
                {
                    continue;
                }
                
                if (party.IsDefendingAtAnySpot())
                {
                    party.ExpectedXPosition = party.XPosition;
                    continue;
                }
                
                var targetSpot = mapSpots[party.TargetSpotId];

                var movementValue = GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleMovementValue;
                if (party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left)
                {
                    party.ExpectedXPosition = party.XPosition + movementValue;
                    // 指定拠点を通り過ぎるようならそこで止める
                    if (!targetSpot.IsBroken && party.ExpectedXPosition >= targetSpot.PositionX)
                    {
                        party.ExpectedXPosition = targetSpot.PositionX;
                    }
                }
                else
                {
                    party.ExpectedXPosition = party.XPosition - movementValue;
                    // 指定拠点を通り過ぎていたら駐留状態に変更
                    if (!targetSpot.IsBroken && party.ExpectedXPosition <= targetSpot.PositionX)
                    {
                        party.ExpectedXPosition = targetSpot.PositionX;
                    }
                }
            }
        }
        
        public static void MakeMatching(ICollection<GuildBattleCommonPartyModel> battleParties, ref Dictionary<int, List<GuildBattleCommonPartyModel>> matchingParties, GuildBattleCommonMapSpotModel leftBase, GuildBattleCommonMapSpotModel rightBase)
        {
            // 最前線にいる戦える状態のユニットをセット
            foreach (var party in battleParties)
            {
                if (!party.IsOnMap())
                {
                    continue;
                }

                var laneNumber = party.LaneNumber;
                var sideIndex = (int)party.Side;
                // 本拠地の守備隊になっている部隊は, 全てのラインから進行されうるので, 全てのラインに対してマッチング出来るものとする
                if ((party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left && party.IsDefendingAtSpot(leftBase.Id)) ||
                    (party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Right && party.IsDefendingAtSpot(rightBase.Id)))
                {
                    foreach (var parties in matchingParties)
                    {
                        if (parties.Value[sideIndex] == null)
                        {
                            parties.Value[sideIndex] = party;
                        }
                    }
                }
                
                var frontParty = matchingParties[laneNumber][sideIndex];
                if (frontParty == null)
                {
                    matchingParties[laneNumber][sideIndex] = party;
                    continue;
                }

                if ((party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left && frontParty.ExpectedXPosition < party.ExpectedXPosition) ||
                    (party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Right && frontParty.ExpectedXPosition > party.ExpectedXPosition))
                {
                    matchingParties[laneNumber][sideIndex] = party;
                }
            }
            
            // 移動結果が交差する形になるなら交戦確定
            // 本拠地を守っている部隊は複数回マッチしてしまう可能性があるので, その場合は2回目以降のマッチングを解除する
            var usedPartyIds = new HashSet<int>();
            foreach (var party in matchingParties.Values)
            {
                var leftParty = party[(int)BattleConst.TeamSide.Left];
                var rightParty = party[(int)BattleConst.TeamSide.Right];

                if ((leftParty == null || rightParty == null) ||
                    (usedPartyIds.Contains(leftParty.PartyId) || usedPartyIds.Contains(rightParty.PartyId)) ||
                    rightParty.ExpectedXPosition > leftParty.ExpectedXPosition)
                {
                    party[(int)BattleConst.TeamSide.Left] = null;
                    party[(int)BattleConst.TeamSide.Right] = null;
                }
                // マッチ確定していたらバトル済みとしてマーク
                else
                {
                    usedPartyIds.Add(leftParty.PartyId);
                    usedPartyIds.Add(rightParty.PartyId);
                }
            }

            // 戦闘が発生する場所では移動を発生させなくする.
            foreach (var party in battleParties)
            {
                if (!party.IsOnMap())
                {
                    continue;
                }

                party.IsFighting = false;
                var matchingPositionParty = matchingParties[party.LaneNumber][(int)party.Side];
                if (matchingPositionParty == null)
                {
                    party.SyncLastMilitaryStrengthToMilitaryStrength();
                    continue;
                }

                if (party.XPosition == matchingPositionParty.XPosition)
                {
                    party.ExpectedXPosition = party.XPosition;
                    party.IsFighting = true;
                }

                if (party != matchingPositionParty)
                {
                    // 戦闘が発生しない軍隊の直前兵力を更新しておく.
                    party.SyncLastMilitaryStrengthToMilitaryStrength();
                }
            }
        }
        
        public static void DealSpotDamage(SortedDictionary<int, GuildBattleCommonPartyModel> battleParties,
            Dictionary<int, GuildBattleCommonMapSpotModel> mapSpots, int elapsedTurnCount,
            Dictionary<int, List<GuildBattleCommonPartyModel>> matchingParties)
        {
            var isAnyPartyOnSpots = new Dictionary<int, bool>(mapSpots.Count);
            var brokenSpotCounts = new int[] { 0, 0 };
            foreach (var mapSpot in mapSpots)
            {
                if (mapSpot.Value.IsBroken)
                {
                    brokenSpotCounts[(int)mapSpot.Value.OccupyingSide]++;
                }

                isAnyPartyOnSpots.Add(mapSpot.Key, false);
            }

            // 侵入可能かのフラグセット
            foreach (var partyKvp in battleParties)
            {
                var party = partyKvp.Value;
                if (party.IsOnMap() && party.IsDefendingAtAnySpot())
                {
                    isAnyPartyOnSpots[party.TargetSpotId] = true;
                }
            }

            foreach (var partyKvp in matchingParties)
            {
                foreach (var party in partyKvp.Value)
                {
                    // 勝利している場合は関係ない.
                    if (party == null || party.IsOnMap())
                    {
                        continue;
                    }

                    // 敗北している場合, 拠点上で戦闘していたら戦闘したターンには拠点ダメージを入れられたくないので.
                    if (party.IsStayingAtAnySpot())
                    {
                        isAnyPartyOnSpots[party.LastJoinedSpotId] = true;
                    }
                }
            }

            var totalMilitaryStrengths = new Dictionary<GuildBattleCommonMapSpotModel, List<GuildBattleCommonPartyModel>>();
            foreach (var partyKvp in battleParties)
            {
                var party = partyKvp.Value;
                // 戦場にいなければスルー
                if (!party.IsOnMap())
                {
                    continue;
                }

                // 防衛ならスルー
                if (party.IsDefendingAtAnySpot())
                {
                    continue;
                }

                // 防衛軍隊がいるなら拠点攻撃出来ないのでスルー
                if (isAnyPartyOnSpots[party.TargetSpotId])
                {
                    continue;
                }

                var targetSpot = mapSpots[party.TargetSpotId];
                // ないはずだけど一応.
                if (targetSpot.IsBroken)
                {
                    continue;
                }

                var isExceedSpotPosition = party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left
                    ? party.ExpectedXPosition >= targetSpot.PositionX
                    : party.ExpectedXPosition <= targetSpot.PositionX;
                // 移動先が拠点を超えないようならスルー
                if (!isExceedSpotPosition)
                {
                    continue;
                }

                if (!totalMilitaryStrengths.ContainsKey(targetSpot))
                {
                    totalMilitaryStrengths.Add(targetSpot, new List<GuildBattleCommonPartyModel>());
                }

                totalMilitaryStrengths[targetSpot].Add(party);
            }

            foreach (var kvp in totalMilitaryStrengths)
            {
                var spot = kvp.Key;
                var parties = kvp.Value;

                var totalMilitaryStrength = parties.Sum(party => party.MilitaryStrength.Sum());
                spot.TakeDamage(elapsedTurnCount, brokenSpotCounts[(int)spot.OccupyingSide], totalMilitaryStrength);
                // 拠点への攻撃条件を満たしている時点で移動が発生しなくなる.
                if (spot.RemainHP > 0)
                {
                    foreach (var party in parties)
                    {
                        party.IsFighting = true;
                        party.ExpectedXPosition = party.XPosition;
                    }
                }
            }
        }
        
        public static void SetPartyPosition(SortedDictionary<int, GuildBattleCommonPartyModel> battleParties, Dictionary<int, List<int>> frontPositions, Dictionary<int, GuildBattleCommonMapSpotModel> mapSpots, GuildBattleCommonMapSpotModel leftBaseSpot, GuildBattleCommonMapSpotModel rightBaseSpot)
        {
            // 毎回計算する必要もないんだよなぁ.
            var allLaneNumber = new List<int>(4);
            foreach (var mapSpot in mapSpots)
            {
                if (!allLaneNumber.Contains(mapSpot.Value.LaneNumber))
                {
                    allLaneNumber.Add(mapSpot.Value.LaneNumber);
                }
            }
            
            // 移動先の最前列をセット
            foreach (var partyKvp in battleParties)
            {
                var party = partyKvp.Value;
                if (!party.IsOnMap())
                {
                    continue;
                }
                
                var sideIndex = (int)party.Side;

                // 本拠地防衛中の軍隊はすべてのレーンに対して戦闘出来るため, すべてのレーンの最前線位置を書き換える.
                List<int> evaluateLaneNumber;
                var baseSpotId = party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left ? leftBaseSpot.Id : rightBaseSpot.Id;
                if (party.IsDefendingAtSpot(baseSpotId))
                {
                    evaluateLaneNumber = allLaneNumber;
                }
                else
                {
                    evaluateLaneNumber = new List<int> { party.LaneNumber };
                }

                foreach (var laneNum in evaluateLaneNumber)
                {
                    var frontPosition = frontPositions[laneNum][sideIndex];
                    if (frontPosition == -1)
                    {
                        frontPositions[laneNum][sideIndex] = party.ExpectedXPosition;
                    }
                    else
                    {
                        var isExceedFront = party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left ? party.ExpectedXPosition > frontPosition : party.ExpectedXPosition < frontPosition;
                        if (isExceedFront)
                        {
                            frontPositions[laneNum][sideIndex] = party.ExpectedXPosition;
                        }
                    }
                }
            }

            // 移動先最前列を超えない位置に移動するなら移動
            foreach (var partyKvp in battleParties)
            {
                var party = partyKvp.Value;
                if (!party.IsOnMap())
                {
                    continue;
                }
                
                if (party.IsDefendingAtAnySpot())
                {
                    continue;
                }
                
                party.LastXPosition = party.XPosition;
                var enemyFrontPosition = frontPositions[party.LaneNumber][(int)GetOtherTeamSide((GuildBattleCommonConst.GuildBattleTeamSide)party.Side)];
                if (enemyFrontPosition == -1 ||
                    (party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left && party.ExpectedXPosition < enemyFrontPosition) ||
                    party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Right && party.ExpectedXPosition > enemyFrontPosition)
                {
                    var targetSpot = mapSpots[party.TargetSpotId];
                    var isExceedSpotPosition = party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left ? party.ExpectedXPosition >= targetSpot.PositionX : party.ExpectedXPosition <= targetSpot.PositionX;
                    // 拠点に重なるようなら拠点のHPをチェック
                    if (isExceedSpotPosition)
                    {
                        // HPを削りきっていたら初めて入れる
                        if (targetSpot.RemainHP <= 0)
                        {
                            party.XPosition = party.ExpectedXPosition;
                            party.LastJoinedSpotId = party.TargetSpotId;
                        }
                    }
                    // 道中は特にチェックいらない
                    else
                    {
                        party.XPosition = party.ExpectedXPosition;
                    }
                }
            }
        }
        
        public static List<int> UpdateSpotOccupationInfo(Dictionary<int, GuildBattleCommonMapSpotModel> spots)
        {
            var updatedSpotIds = new List<int>(4);
            foreach (var kvp in spots)
            {
                if (!kvp.Value.IsBroken && kvp.Value.RemainHP <= 0)
                {
                    updatedSpotIds.Add(kvp.Value.Id);
                    kvp.Value.IsBroken = true;
                }
            }

            return updatedSpotIds;
        }
        
        public static void AddSpotBrokenBonusMilitaryStrength(ICollection<GuildBattleCommonPlayerData> players, List<int> brokenSpotIds)
        {
            foreach (var spotId in brokenSpotIds)
            {
                var spot = GuildBattleCommonDataMediator.Instance.BattleField.GetMapSpot(spotId);
                if (spot == null || spot.IsBase)
                {
                    continue;
                }

                var side = spot.OccupyingSide;
                foreach (var player in players)
                {
                    if (player.Side == side)
                    {
                        // 限界を超えて回復する.
                        player.AddAvailableMilitaryStrength(GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleAdditionalMilitaryStrengthPerSpotBroken, true);
                    }
                }
            }
        }
        
        public static void UpdateTargetSpot(Dictionary<int, GuildBattleCommonMapSpotModel> spots, IEnumerable<GuildBattleCommonPartyModel> battleParties)
        {
            foreach (var party in battleParties)
            {
                if (!party.IsOnMap())
                {
                    continue;
                }
                
                if (!party.IsDefendingAtAnySpot())
                {
                    continue;
                }
                
                var spot = spots[party.LastJoinedSpotId];
                if (!spot.IsBroken)
                {
                    continue;
                }

                GuildBattleCommonMapSpotModel targetSpot = null;
                var connectedSpotIds = spot.ConnectedSpotIdList[(int)party.Side];
                foreach (var connectedSpotId in connectedSpotIds)
                {
                    var connectedSpot = spots[connectedSpotId];
                    if (targetSpot == null)
                    {
                        targetSpot = connectedSpot;
                        continue;
                    }

                    if ((party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Left && targetSpot.PositionX < connectedSpot.PositionX) ||
                        party.Side == GuildBattleCommonConst.GuildBattleTeamSide.Right && targetSpot.PositionX > connectedSpot.PositionX)
                    {
                        targetSpot = connectedSpot;
                    }
                }

                if (targetSpot != null)
                {
                    party.TargetSpotId = targetSpot.Id;
                }
            }
        }
        
        public static void UpdatePlayerAvailableMilitaryStrength(ICollection<GuildBattleCommonPlayerData> players)
        {
            var recoveryMilitaryStrength = GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleRecoveryMilitaryStrengthPerTurn;
            foreach (var player in players)
            {
                // 自動配置で未接続なのに兵力を消費しているパターンがあるため, 未接続なら回復しない
                if (player.IsJoinedFirstTime)
                {
                    continue;
                }
                
                // 拠点占拠でキャップを超えて回復することがあるのでその場合はいじらない.
                if (player.AvailableMilitaryStrength >= player.MaxMilitaryStrength)
                {
                    continue;
                }
                
                player.AddAvailableMilitaryStrength(recoveryMilitaryStrength, false);
            }
        }
        
        public static bool IsBattleContinuing(ICollection<GuildBattleCommonMapSpotModel> spots, int remainTurnCount)
        {
            if (remainTurnCount <= 0)
            {
                return false;
            }
            
            foreach (var spot in spots)
            {
                if (spot.IsBase && spot.IsBroken)
                {
                    return false;
                }
            }

            return true;
        }
        
        public static (GuildBattleCommonConst.GuildBattleWinTeam, int, int) GetGuildBattleResult(
            int processedTurnCount, Dictionary<int, GuildBattleCommonMapSpotModel> mapSpotsDictionary,
            int leftWinningPoint, int rightWinningPoint, long leftGuildId, long rightGuildId)
        {
            var leftSidePoint = 0;
            var rightSidePoint = 0;
            var isLeftBaseBroken = false;
            var isRightBaseBroken = false;
            
            foreach (var spotKvp in mapSpotsDictionary)
            {
                var spot = spotKvp.Value;
                if (spot.IsBase && spot.IsBroken)
                {
                    // 基本的にないけど、上下のレーンがあってすれ違いする可能性がある以上、両方の本拠地が同時に落ちることはありえる…
                    if (spot.OccupyingSide == GuildBattleCommonConst.GuildBattleTeamSide.Left)
                    {
                        isLeftBaseBroken = true;
                    }else if (spot.OccupyingSide == GuildBattleCommonConst.GuildBattleTeamSide.Right)
                    {
                        isRightBaseBroken = true;
                    }
                }

                if (spot.IsBroken)
                {
                    if (spot.OccupyingSide == GuildBattleCommonConst.GuildBattleTeamSide.Left)
                    {
                        rightSidePoint++;
                    }
                    else
                    {
                        leftSidePoint++;
                    }
                }
            }
            
            var resultType = GuildBattleCommonConst.GuildBattleWinTeam.Draw; 
            if (isLeftBaseBroken && isRightBaseBroken)
            {
                resultType = GuildBattleCommonConst.GuildBattleWinTeam.Draw;
            }else if (isLeftBaseBroken != isRightBaseBroken)
            {
                resultType = isLeftBaseBroken ? GuildBattleCommonConst.GuildBattleWinTeam.Right : GuildBattleCommonConst.GuildBattleWinTeam.Left;
            }
            else
            {
                resultType = leftSidePoint == rightSidePoint ? GuildBattleCommonConst.GuildBattleWinTeam.Draw : leftSidePoint > rightSidePoint ? GuildBattleCommonConst.GuildBattleWinTeam.Left : GuildBattleCommonConst.GuildBattleWinTeam.Right;
            }

            // 勝敗が決しないようなら評価点による勝敗にする.
            // それでも決まらなかったらギルドIDが若い順. ドンマイ.
            var isDraw = resultType == GuildBattleCommonConst.GuildBattleWinTeam.Draw;
            if (isDraw)
            {
                if (leftWinningPoint == rightWinningPoint)
                {
                    resultType = leftGuildId < rightGuildId ? GuildBattleCommonConst.GuildBattleWinTeam.Left : GuildBattleCommonConst.GuildBattleWinTeam.Right;
                }
                else
                {
                    resultType = leftWinningPoint > rightWinningPoint ? GuildBattleCommonConst.GuildBattleWinTeam.Left : GuildBattleCommonConst.GuildBattleWinTeam.Right;
                }
            }

            // 最大ポイントから経過ターン数による減算分を計算して, 残った分を勝利点に加算.
            var turnBasedWinningPoint = Math.Max(0,
                (GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleMaxWinningPointFromTime -
                 processedTurnCount * (GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattlePerTurnDelayMilliseconds / 1000) / 60 *
                 GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleMinusWinningPointPerMin));
            // ここで計算された値はクライアントにパブリッシュしない(リザルトに映さない)ので, データの書き換え自体はしなくてOK
            switch (resultType)
            {
                case GuildBattleCommonConst.GuildBattleWinTeam.Left:
                    leftWinningPoint += GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleWinningPointWin + turnBasedWinningPoint;
                    break;
                case GuildBattleCommonConst.GuildBattleWinTeam.Right:
                    rightWinningPoint += GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleWinningPointWin + turnBasedWinningPoint;
                    break;
            }
            
            return (resultType, leftWinningPoint, rightWinningPoint);
        }
        
        public static void DecrementItemCoolTime()
        {
            foreach (var battlePlayer in GuildBattleCommonDataMediator.Instance.BattlePlayerDataByIndex)
            {
                var itemData = battlePlayer.Value.GuildBattleItemList;
                foreach (var data in itemData)
                {
                    if (data.CoolTime > 0)
                    {
                        data.CoolTime--;
                    }
                }
            }
        }
        
        /// <summary>
        /// 対象拠点に一番近い生きている味方の拠点IDを取得
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static int GetNearestAliveSpotId(GuildBattleCommonConst.GuildBattleTeamSide side, int targetSpotId)
        {
            var (spotId, depth) = GetNearestAliveSpotIdAndDepth(side, targetSpotId, 1);
            return spotId;
        }

        /// <summary>
        /// 接続された拠点の中から一番近い味方の拠点のSpotIdと距離を取得
        /// 注意: 道中の拠点が複数に接続されている場合は一番上の接続された拠点に反応してしまう. まあその場合は送る導線を設定しなくちゃいけないとかそもそも仕様的にまずくなってくるはずだからその時は再検討.
        /// </summary>
        /// <param name="side"></param>
        /// <param name="startSpotId"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static (int, int) GetNearestAliveSpotIdAndDepth(GuildBattleCommonConst.GuildBattleTeamSide side, int startSpotId, int depth)
        {
            var connectedSpotIdIndex = side == GuildBattleCommonConst.GuildBattleTeamSide.Left ? 1 : 0;
            var startSpot = GuildBattleCommonDataMediator.Instance.BattleField.GetMapSpot(startSpotId);
            var connectedSpotIds = startSpot.ConnectedSpotIdList[connectedSpotIdIndex];

            var currentDepth = int.MaxValue;
            var currentSpotId = -1;
            
            foreach (var connectedSpotId in connectedSpotIds)
            {
                var spot = GuildBattleCommonDataMediator.Instance.BattleField.GetMapSpot(connectedSpotId);
                // 出撃可能拠点ならその拠点を深さとする
                if (!spot.IsBroken && spot.OccupyingSide == side)
                {
                    if (currentDepth == int.MaxValue || currentDepth < depth)
                    {
                        currentDepth = depth;
                        currentSpotId = connectedSpotId;
                    }
                }
                // 出撃不可能なら再帰的に深さを求める
                else
                {
                    var (spotId, nextDepth) = GetNearestAliveSpotIdAndDepth(side, connectedSpotId, ++depth);
                    if (spotId > 0 && (currentDepth == int.MaxValue || nextDepth < currentDepth))
                    {
                        currentDepth = nextDepth;
                        currentSpotId = spotId;
                    }
                }
            }
            
            return (currentSpotId, currentDepth);
        }
        
        public static int GetNearestSpotId(GuildBattleCommonConst.GuildBattleTeamSide side, int startSpotId, int targetSpotId)
        {
            var connectedSpotIdIndex = side == GuildBattleCommonConst.GuildBattleTeamSide.Left ? 0 : 1;
            var startSpot = GuildBattleCommonDataMediator.Instance.BattleField.GetMapSpot(startSpotId);
            var connectedSpotIds = startSpot.ConnectedSpotIdList[connectedSpotIdIndex];

            foreach (var connectedSpotId in connectedSpotIds)
            {
                // 接続されているならそれを返す
                if (connectedSpotId == targetSpotId)
                {
                    return startSpot.Id;
                }
                // 直接接続されていないなら再帰的に求める
                else
                {
                    var spotId = GetNearestSpotIdAndDepth(side, connectedSpotId, targetSpotId);
                    if (spotId > 0)
                    {
                        return spotId;
                    }
                }
            }
            
            return -1;
        }

        private static int GetNearestSpotIdAndDepth(GuildBattleCommonConst.GuildBattleTeamSide side, int startSpotId, int targetSpotId)
        {
            var connectedSpotIdIndex = side == GuildBattleCommonConst.GuildBattleTeamSide.Left ? 0 : 1;
            var startSpot = GuildBattleCommonDataMediator.Instance.BattleField.GetMapSpot(startSpotId);
            var connectedSpotIds = startSpot.ConnectedSpotIdList[connectedSpotIdIndex];

            foreach (var connectedSpotId in connectedSpotIds)
            {
                // 接続されているならそれを返す
                if (connectedSpotId == targetSpotId)
                {
                    return startSpot.Id;
                }
                // 直接接続されていないなら再帰的に求める
                else
                {
                    var spotId = GetNearestSpotIdAndDepth(side, connectedSpotId, targetSpotId);
                    if (spotId > 0)
                    {
                        return spotId;
                    }
                }
            }
            
            return -1;
        }

        public static int GetLanePositionId(int laneNumber, int positionX)
        {
            return laneNumber + positionX;
        }

        public static float GetWinStreakStatusPenaltyRatio(int winStreakCount)
        {
            if (winStreakCount < GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleStatusPenaltyByWinStreakCountStartAt)
            {
                return 1.0f;
            }
            
            var penaltyRatio = Math.Max(GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleMaxStatusPenaltyByWinStreak, (1.0f - (winStreakCount - GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleStatusPenaltyByWinStreakCountStartAt + 1) * GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleStatusPenaltyPerWinStreak));
            return penaltyRatio;
        }
    }
}