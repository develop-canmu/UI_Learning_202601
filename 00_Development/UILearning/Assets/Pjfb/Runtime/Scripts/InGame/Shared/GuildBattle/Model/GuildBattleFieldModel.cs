using System.Collections.Generic;
using System.Linq;
using MagicOnion;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    public class GuildBattleFieldModel : GuildBattleCommonFieldModel
    {
        public new Dictionary<int, GuildBattleMapSpotModel> MapSpotsDictionary { get; private set; }
        public Dictionary<int, GuildBattleCommonMapSpotModel> CommonMapSpotsDictionary => base.MapSpotsDictionary;
        
        
        public void SetData(BattleV2ClientData battleData)
        {
            SetTotalDistance(battleData.battleConquestField.sizeX);
            MapSpotsDictionary = new Dictionary<int, GuildBattleMapSpotModel>();
            base.MapSpotsDictionary = new Dictionary<int, GuildBattleCommonMapSpotModel>();
            var id = 1; // これデータで渡してくるようにしたいな.
            foreach (var spot in battleData.battleConquestField.battleConquestFieldSpotList)
            {
                var spotModel = new GuildBattleMapSpotModel(id++, spot);
                MapSpotsDictionary.Add(spotModel.Id, spotModel);
                base.MapSpotsDictionary.Add(spotModel.Id, spotModel);
            }

            leftBaseSpot = MapSpotsDictionary.Values.First(spot => spot.IsLeftBase);
            rightBaseSpot = MapSpotsDictionary.Values.First(spot => spot.IsRightBase);
            var eachLaneSpots = MapSpotsDictionary.Values
                .OrderBy(spot => spot.PositionX)
                .GroupBy(spot => spot.LaneNumber).ToList();
            foreach (var laneSpots in eachLaneSpots)
            {
                var spots = laneSpots.ToList();
                var mostLeftSpot = spots.FirstOrDefault();
                var mostRightSpot = spots.LastOrDefault();
                if (!mostLeftSpot.IsLeftBase)
                {
                    spots.Insert(0, (GuildBattleMapSpotModel)leftBaseSpot);
                }

                if (!mostRightSpot.IsRightBase)
                {
                    spots.Add((GuildBattleMapSpotModel)rightBaseSpot);
                }

                for (var i = 0; i < spots.Count; i++)
                {
                    if (i > 0)
                    {
                        spots[i].ConnectedSpotIdList[(int)BattleConst.TeamSide.Right].Add(spots[i-1].Id);
                    }

                    if (i < spots.Count - 1)
                    {
                        spots[i].ConnectedSpotIdList[(int)BattleConst.TeamSide.Left].Add(spots[i+1].Id);
                    }
                }
            }
        }
    }
}