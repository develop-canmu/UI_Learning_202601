using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Deck
{ 

// spd		    スピード（万分率）
// tec		    テクニック（万分率）
// param1(sta)	スタミナ（万分率）
// param2(phy)	フィジカル（万分率）
// param3(sig)	視野（万分率）
// param4(kic)	キック（万分率）
// param5(wis)	賢さ（万分率）
    
    
    public class RecommendationsTargetCharacter
    {
        public RecommendationsTargetCharacter(UserDataCharaVariable chara)
        {
            Chara = chara;
            A = DeckRecommendationsUtility.CalculateComparisionValueA(Chara);
            B = DeckRecommendationsUtility.CalculateComparisionValueB(Chara);
            C = DeckRecommendationsUtility.CalculateComparisionValueC(Chara);
            
            SetBestRolePriority();
        }

        public string TempName;
        public readonly UserDataCharaVariable Chara;
        /// <summary>[比較値A]＝[キック値x係数A]と[テクニック値x係数B]と[スピード値x係数C]の平均値</summary> 	
        public float A;
        /// <summary>[比較値B]＝[スタミナ値x係数D]と[賢さ値x係数E]と[テクニック値x係数F]の平均値</summary>
        public float B;
        /// <summary>[比較値C]＝[賢さ値x係数G]と[テクニック値x係数H]と[フィジカル値x係数I]の平均値</summary>
        public float C;


        public RoleNumber BestRole { get; private set; }
        public RoleNumber SecondBestRole{ get; private set; }
        
        private void SetBestRolePriority()
        {
            if (A >= B)
            {
                if (A >= C)
                {
                    BestRole = RoleNumber.FW;
                    SecondBestRole = (B >= C) ? RoleNumber.MF : RoleNumber.DF;
                }
                else
                {
                    BestRole = RoleNumber.DF;
                    SecondBestRole = RoleNumber.FW;
                }
            }
            else
            {
                if (B >= C)
                {
                    BestRole = RoleNumber.MF;
                    SecondBestRole = (A >= C) ? RoleNumber.FW : RoleNumber.DF;
                }
                else
                {
                    BestRole = RoleNumber.DF;
                    SecondBestRole = RoleNumber.MF;
                }
            }
        }
    }

    public class DeckRecommendationsResult
    {
        public readonly DeckSlotCharacter[] RecommendationResult = new DeckSlotCharacter[DeckUtility.BattleDeckSlotCount];

        private Dictionary<RoleNumber, int> RoleCount = new()
        {
            { RoleNumber.FW, 0 },
            { RoleNumber.MF, 0 },
            { RoleNumber.DF, 0 },
        };

        public int GetCount(RoleNumber roleNumber) => RoleCount[roleNumber];


        public bool HasCompleteRecommendation => CurrentIndex == DeckUtility.BattleDeckSlotCount;
        public int CurrentIndex;
        public void AddCharacter(DeckSlotCharacter chara)
        {
            RecommendationResult[CurrentIndex++] = chara;
            RoleCount[chara.Position]++;
        }
    }
    
    public static class DeckRecommendationsUtility
    {
        private static int tempCoefficient = 1;
        private const int MaxPositionCount = 3;

        private static IReadOnlyList<RoleNumber> AllRole = new[] { RoleNumber.FW, RoleNumber.MF, RoleNumber.DF };
        
        public static DeckSlotCharacter[] GetRecommendedDeck(List<RecommendationsTargetCharacter> recommendTargetCharaList)
        {
            return (recommendTargetCharaList.Count == DeckUtility.BattleDeckSlotCount)
                ? GetRecommendedCharaPatternA(recommendTargetCharaList)
                : GetRecommendedCharaPatternB(recommendTargetCharaList);
        }

        public static List<RecommendationsTargetCharacter> GetRecommendTargetChara(HashSet<long> excludeIdList)
        {
            var allChara = UserDataManager.Instance.charaVariable.data.Values.Where(x => !excludeIdList.Contains(x.id)).OrderByDescending(x => x.combatPower);

            var charaList = new List<RecommendationsTargetCharacter>();

            HashSet<long> targetIdList = new();

            long minimumPower = long.MinValue;
            foreach (var chara in allChara)
            {
                long parentMCharaId = chara.ParentMCharaId;
                if(targetIdList.Contains(parentMCharaId))    continue;
                if (charaList.Count >= DeckUtility.BattleDeckSlotCount && chara.combatPower < minimumPower) break;
                charaList.Add(new RecommendationsTargetCharacter(chara));
                minimumPower = chara.combatPower;
                targetIdList.Add(parentMCharaId);
            }

            return charaList;
        }

        /// <summary>ちょうど5人の場合</summary>
        private static DeckSlotCharacter[] GetRecommendedCharaPatternA(List<RecommendationsTargetCharacter>  charaList)
        {
            DeckRecommendationsResult result = new();

            foreach (var role in AllRole)
            {
                var bestResultChara = charaList.SortRecommendationsCharacterBy(role);
                charaList.Remove(bestResultChara);
                result.AddCharacter(new DeckSlotCharacter(bestResultChara.Chara, role));
            }
            

            foreach (var targetChara in charaList)
            {
                RoleNumber role = targetChara.BestRole;
                result.AddCharacter(new DeckSlotCharacter(targetChara.Chara, role));
            }
            
            return result.RecommendationResult;
        }

        private class PositionCandidates
        {
            public List<RecommendationsTargetCharacter> Candidates = new();
            public bool HasCandidates => Candidates.Count > 0;
            
            public void SortCandidates(RoleNumber priorityRoleNumber)
            {
                switch (priorityRoleNumber)
                {
                    case RoleNumber.FW:
                        Candidates = Candidates.OrderByDescending(x => x.A).ThenBy(x => Mathf.Max(x.B, x.C)).ToList();
                        break;
                    case RoleNumber.MF:
                        Candidates = Candidates.OrderByDescending(x => x.B).ThenBy(x => Mathf.Max(x.A, x.C)).ToList();
                        break;
                    case RoleNumber.DF:
                        Candidates = Candidates.OrderByDescending(x => x.C).ThenBy(x => Mathf.Max(x.A, x.B)).ToList();
                        break;
                }
            }
        }
        
        
        /// <summary>5人超えた場合</summary>
        private static DeckSlotCharacter[] GetRecommendedCharaPatternB(List<RecommendationsTargetCharacter>  charaList)
        {
            DeckRecommendationsResult result = new DeckRecommendationsResult();
            var samePowerCharaGroup = charaList.GroupBy(x => x.Chara.combatPower);

            RoleNumber currentSelectingRole;
            foreach (var charaGroup in samePowerCharaGroup)
            {
                var samePowerCharaList = charaGroup.ToList();
                if (samePowerCharaList.Count > DeckUtility.BattleDeckSlotCount - result.CurrentIndex) break;

                currentSelectingRole = RoleNumber.FW;             
                var candidatesDictionary = new Dictionary<RoleNumber, PositionCandidates>
                {
                    { RoleNumber.FW, new PositionCandidates() },
                    { RoleNumber.MF, new PositionCandidates() },
                    { RoleNumber.DF, new PositionCandidates() }
                };


                foreach (var chara in charaGroup)
                {
                    candidatesDictionary[chara.BestRole].Candidates.Add(chara);
                }

     
                while (candidatesDictionary.Values.Any(x => x.HasCandidates))
                {
                    PositionCandidates candidateContainer = candidatesDictionary[currentSelectingRole];
                    int remainCount = MaxPositionCount - result.GetCount(currentSelectingRole);

                    // 1人超えた場合
                    if (candidateContainer.Candidates.Count > remainCount)
                    {
                        // 同じAを持ってる場合BかCの最大値を持ってるキャラを除く
                        candidateContainer.SortCandidates(currentSelectingRole);

                        var excludedCandidate = candidateContainer.Candidates.Last();
                        candidateContainer.Candidates.Remove(excludedCandidate);
                        candidatesDictionary[excludedCandidate.SecondBestRole].Candidates.Add(excludedCandidate);
                    }

                    foreach (var candidate in candidateContainer.Candidates)
                    {
                        charaList.Remove(candidate);
                        result.AddCharacter(new DeckSlotCharacter(candidate.Chara, currentSelectingRole));
                    }

                    candidateContainer.Candidates.Clear();

                    currentSelectingRole = NextRole(currentSelectingRole);
                }
                
            }

            foreach (var role in AllRole)
            {
                if (result.GetCount(role) == 0)
                {
                    var bestResultChara = charaList.SortRecommendationsCharacterBy(role);
                    charaList.Remove(bestResultChara);
                    result.AddCharacter(new DeckSlotCharacter(bestResultChara.Chara, role));
                }

                if (result.HasCompleteRecommendation) return result.RecommendationResult;
            }
            
        
            // FW -> MF -> DF 順で
            currentSelectingRole = RoleNumber.FW;
            while (!result.HasCompleteRecommendation)
            {
                var bestResultChara = charaList.SortRecommendationsCharacterBy(currentSelectingRole);
                charaList.Remove(bestResultChara);
                result.AddCharacter(new DeckSlotCharacter(bestResultChara.Chara, currentSelectingRole));
                currentSelectingRole = NextRole(currentSelectingRole);
            }
            
            return result.RecommendationResult;
        }

        private static RoleNumber NextRole(RoleNumber currentRole)
        {
            switch (currentRole)
            {
                case RoleNumber.FW:
                    return RoleNumber.MF;
                case RoleNumber.MF:
                    return RoleNumber.DF;
                case RoleNumber.DF:
                    return RoleNumber.FW;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentRole), currentRole, null);
            }
        }


        private static RecommendationsTargetCharacter SortRecommendationsCharacterBy(this List<RecommendationsTargetCharacter> charaList, RoleNumber role)
        {
            return role switch
            {
                RoleNumber.FW => charaList.SortRecommendationsCharacterByValueA(),
                RoleNumber.MF => charaList.SortRecommendationsCharacterByValueB(),
                RoleNumber.DF => charaList.SortRecommendationsCharacterByValueC(),
                _ => charaList.FirstOrDefault()
            };
        }
        
        private static RecommendationsTargetCharacter SortRecommendationsCharacterByValueA(this List<RecommendationsTargetCharacter> charaList)
        {
            return charaList.OrderByDescending(x => x.Chara.combatPower)
                .ThenByDescending(x => x.A)
                .ThenByDescending(x => x.Chara.param1)
                .ThenByDescending(x => x.Chara.param2)
                .ThenByDescending(x => x.Chara.param5)
                .ThenBy(x => x.Chara.charaId).FirstOrDefault();
        }
        private static RecommendationsTargetCharacter SortRecommendationsCharacterByValueB(this List<RecommendationsTargetCharacter> charaList)
        {
            return charaList.OrderByDescending(x => x.Chara.combatPower)
                .ThenByDescending(x => x.B)
                .ThenByDescending(x => x.Chara.param1)
                .ThenByDescending(x => x.Chara.param4)
                .ThenByDescending(x => x.Chara.param2)
                .ThenBy(x => x.Chara.charaId).FirstOrDefault();
        }

        private static RecommendationsTargetCharacter SortRecommendationsCharacterByValueC(this List<RecommendationsTargetCharacter> charaList)
        {
            return charaList.OrderByDescending(x => x.Chara.combatPower)
                .ThenByDescending(x => x.C)
                .ThenByDescending(x => x.Chara.param1)
                .ThenByDescending(x => x.Chara.spd)
                .ThenByDescending(x => x.Chara.param4)
                .ThenBy(x => x.Chara.charaId).FirstOrDefault();
        }

        
        
        public static float CalculateComparisionValueA(UserDataCharaVariable chara)
        {
            return (float)(chara.param4 * tempCoefficient + chara.tec * tempCoefficient + chara.spd * tempCoefficient) / 3;
        }
        public static float CalculateComparisionValueB(UserDataCharaVariable chara)
        {
            return (float)(chara.param1 * tempCoefficient + chara.param5 * tempCoefficient + chara.tec * tempCoefficient) / 3;
        }
        public static float CalculateComparisionValueC(UserDataCharaVariable chara)
        {
            return (float)(chara.param5 * tempCoefficient + chara.tec * tempCoefficient + chara.param2 * tempCoefficient) / 3;
        }

    }
}

