using CruFramework.Page;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace Pjfb.Ranking
{
    public abstract class RankingCharacterPowerTabSheet : RankingTabSheet
    {
        [SerializeField] private RankingAffiliateTabSheetType sheetType;
        public RankingAffiliateTabSheetType SheetType => sheetType;
        // 選手戦力タブ
        // todo:3.12.0で実装
    }
}