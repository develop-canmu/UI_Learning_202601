using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Ranking
{
    public class RankingTotalPointTabSheetClubTab : RankingTotalPointTabSheet
    {
        // イベントポイントクラブタブ
        protected override UniTask OnOpen(object args)
        {
            return base.OnOpen(args);
        }
        
        public override void UpdateView(object rankingData, bool isOmissionPoint)
        {
            throw new System.NotImplementedException();
        }
        
        public override void OnUpdateRewardView(RankingRewardView rankingRewardView)
        {
            throw new System.NotImplementedException();
            // TODO: 報酬画面のデータをセット
            // rankingRewardView.SetView(<報酬データ>);
        }
    }
}