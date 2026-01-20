using System.Collections.Generic;
using System.Linq;

namespace MagicOnion
{
    public class GuildBattleCommonFieldModel
    {
        #region Properties and SerializeField
        
        // マップ総距離
        public int TotalDistance { get; private set; }
        // レーン数
        public int LaneCount { get; private set; }
        // 拠点情報
        public Dictionary<int, GuildBattleCommonMapSpotModel> MapSpotsDictionary { get; protected set; }

        protected GuildBattleCommonMapSpotModel leftBaseSpot;
        protected GuildBattleCommonMapSpotModel rightBaseSpot;

        #endregion

        #region Fields
        #endregion

        #region Public Methods

        public void SetTotalDistance(long totalDistance)
        {
            TotalDistance = (int)totalDistance;
        }
        
        public void SetSpotData(Dictionary<int, GuildBattleCommonMapSpotModel> spotModels)
        {
            MapSpotsDictionary = spotModels;
        }
        
        public void UpdateSpotData(List<GuildBattleCommonMapSpotModel> serverSpotModels)
        {
            foreach (var spot in serverSpotModels)
            {
                if (MapSpotsDictionary.TryGetValue(spot.Id, out var targetSpot))
                {
                    targetSpot.SyncMapSpotModel(spot);
                }
            }
        }

        public GuildBattleCommonMapSpotModel GetMapSpot(int spotId)
        {
            MapSpotsDictionary.TryGetValue(spotId, out var ret);
            return ret;
        }
        
        public GuildBattleCommonMapSpotModel GetBaseSpot(GuildBattleCommonConst.GuildBattleTeamSide side)
        {
            switch (side)
            {
                case GuildBattleCommonConst.GuildBattleTeamSide.Left:
                    return leftBaseSpot;
                case GuildBattleCommonConst.GuildBattleTeamSide.Right:
                    return rightBaseSpot;
            }

            return null;
        }

        public long[] GetSideAliveSpotIds(GuildBattleCommonConst.GuildBattleTeamSide side)
        {
            var ids = new List<long>();
            foreach (var kvp in MapSpotsDictionary)
            {
                if (kvp.Value.OccupyingSide == side && !kvp.Value.IsBroken)
                {
                    ids.Add(kvp.Value.Id);
                }
            }

            return ids.ToArray();
        }

        public GuildBattleCommonMapSpotModel GetSpotByIndex(GuildBattleCommonConst.GuildBattleTeamSide side, int index)
        {
            // 現状NPCの防衛コマンドでのみ使っており, index == 1(本拠地指定)しかあまり意味がないのでこのOrderに.
            // もし意味をもたせるようならIdでソートなり考える.
            var orderedSpots = MapSpotsDictionary.Values
                .Where(spot => spot.OccupyingSide == side)
                .OrderByDescending(spot => spot.IsBase).ToList();

            if (orderedSpots.Count <= index)
            {
                return null;
            }

            return orderedSpots[index];
        }

        public List<int> GetDamagedSpotIds(GuildBattleCommonFieldSituationModel latestSituation)
        {
            var ret = new List<int>();
            foreach (var spotInfo in latestSituation.SpotOccupationInfo)
            {
                var beforeData = MapSpotsDictionary[spotInfo.Id];
                if (beforeData.RemainHP > spotInfo.RemainHP && spotInfo.RemainHP > 0)
                {
                    ret.Add(spotInfo.Id);
                }
            }

            return ret;
        }
        
        #endregion        
        
        #region Protected and Private Methods
        #endregion
    }
}