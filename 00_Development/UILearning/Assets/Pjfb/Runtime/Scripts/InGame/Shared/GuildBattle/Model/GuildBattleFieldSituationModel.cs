using System.Collections.Generic;
using System.Linq;
using MagicOnion;
using MessagePack;

namespace Pjfb
{
    [MessagePackObject]
    public class GuildBattleFieldSituationModel : GuildBattleCommonFieldSituationModel {
        [Key(101)]
        public List<GuildBattlePartyModel> PjfbParties;
        [Key(102)]
        public List<int> ScoredPartyIds;
        [Key(103)]
        public Dictionary<int, int> BattleResultScores;

        public void SetData(
            int remainTurn, int turnNumber,
            Dictionary<int, GuildBattleMapSpotModel> spots,
            SortedDictionary<int, GuildBattlePartyModel> parties,
            Dictionary<int, List<GuildBattleCommonPartyModel>> eachLaneMatchingParties,
            int[] winningPoints, List<int> departedPartyIds, List<int> dissolutionPartyIds, List<int> scoredPartyIds,
            Dictionary<int, int> battleResultScores)

        {
            RemainTurn = remainTurn;
            TurnNumber = turnNumber;
            SpotOccupationInfo = new List<GuildBattleCommonMapSpotModel>(spots?.Count ?? 0);
            foreach (var kvp in spots)
            {
                SpotOccupationInfo.Add(kvp.Value);
            }

            PjfbParties = new List<GuildBattlePartyModel>(parties?.Count ?? 0);
            base.Parties = new List<GuildBattleCommonPartyModel>(parties?.Count ?? 0);
            foreach (var kvp in parties)
            {
                PjfbParties.Add(kvp.Value);
                base.Parties = new List<GuildBattleCommonPartyModel>(parties?.Count ?? 0);
            }

            EachLaneMatchingPartyIds = new List<List<int>>();
            LosePartyIds = new List<int>();
            DepartedPartyIds = departedPartyIds ?? new List<int>(0);
            DissolutionPartyIds = dissolutionPartyIds ?? new List<int>(0);
            ScoredPartyIds = scoredPartyIds ?? new List<int>(0);
            WinningPoints = winningPoints;
            BattleResultScores = battleResultScores;
            if (eachLaneMatchingParties == null)
            {
                return;
            }

            foreach (var matchingPartiesKvp in eachLaneMatchingParties)
            {
                var matchingParties = matchingPartiesKvp.Value;
                // TODO MAGIC NUMBER
                if (matchingParties.Count == 2 &&
                    matchingParties[0] != null && matchingParties[1] != null)
                {
                    var partyIds = matchingParties.Select(party => party.PartyId).ToList();
                    EachLaneMatchingPartyIds.Add(partyIds);
                }

                foreach (var party in matchingParties)
                {
                    if (party != null && !party.IsOnMap())
                    {
                        LosePartyIds.Add(party.PartyId);
                    }
                }
            }
        }
    }
}