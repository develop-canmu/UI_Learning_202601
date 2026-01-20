#if !PJFB_REL
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.DebugPage;
using Pjfb.InGame;

namespace Pjfb
{
    public class DebugBattleDigestController : BattleDigestController
    {
        [SerializeField] private DebugCutScenePage debugCutScenePage;
        protected override float GetDigestPlaySpeed(BattleConst.DigestType digestType,
            BattleDigestCharacterData mainCharacter, List<BattleDigestCharacterData> otherCharacters,
            BattleConst.TeamSide offenceSide)
        {
            return debugCutScenePage.IsEightSpeed ? 8.0f : 1.0f;
        }
    }
}
#endif