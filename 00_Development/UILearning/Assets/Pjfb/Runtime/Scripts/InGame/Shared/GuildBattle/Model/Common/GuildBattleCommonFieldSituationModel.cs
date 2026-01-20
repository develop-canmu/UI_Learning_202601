using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace MagicOnion
{
    [MessagePackObject]
    public class GuildBattleCommonFieldSituationModel
    {
        #region Properties and SerializeField

        [Key(0)]
        public int RemainTurn;
        [Key(1)]
        public int TurnNumber;
        [Key(2)]
        public List<GuildBattleCommonMapSpotModel> SpotOccupationInfo;
        [Key(3)]
        public List<GuildBattleCommonPartyModel> Parties;
        [Key(5)]
        public List<List<int>> EachLaneMatchingPartyIds;
        [Key(6)]
        public List<int> LosePartyIds;
        [Key(7)]
        public List<int> DepartedPartyIds;
        [Key(8)]
        public List<int> DissolutionPartyIds;
        [Key(9)]
        public int[] WinningPoints;

        #endregion

        #region Fields

        private const int requiredPartyCount = 2;
        private const int firstPartyIndex = 0;
        private const int secondPartyIndex = 1;

        #endregion

        #region Public Methods
        
        public void SetData(int remainTurn, int turnNumber, ICollection<GuildBattleCommonMapSpotModel> spots, ICollection<GuildBattleCommonPartyModel> parties, ICollection<List<GuildBattleCommonPartyModel>> eachLaneMatchingParties, int[] winningPoints, List<int> departedPartyIds, List<int> dissolutionPartyIds)
        {
            RemainTurn = remainTurn;
            TurnNumber = turnNumber;
            SpotOccupationInfo = spots.ToList();
            Parties = parties.OrderBy(party => party.PartyId).ToList();
            EachLaneMatchingPartyIds = new List<List<int>>();
            LosePartyIds = new List<int>();
            DepartedPartyIds = departedPartyIds ?? new List<int>(0);
            DissolutionPartyIds = dissolutionPartyIds ?? new List<int>(0);
            WinningPoints = winningPoints;
            if (eachLaneMatchingParties == null)
            {
                return;
            }
            
            foreach (var matchingParties in eachLaneMatchingParties)
            {
                if (matchingParties.Count == requiredPartyCount &&
                    matchingParties[firstPartyIndex] != null && matchingParties[secondPartyIndex] != null)
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

        #endregion

        #region Protected and Private Methods

        #endregion
    }
}