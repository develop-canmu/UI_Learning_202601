using CruFramework.Page;

namespace Pjfb.Ranking
{
    /// <summary>各カテゴリにおける個人とクラブシートを管理するクラス</summary>
    public class RankingAffiliateTabSheetManager : SheetManager<RankingAffiliateTabSheetType>
    {
    }
    
    /// <summary>報酬画面にデータを受け渡す処理を定義したクラス</summary>
    public abstract class RankingTabSheet : Sheet
    {
        /// <summary>報酬画面のViewを更新する</summary>
        public abstract void OnUpdateRewardView(RankingRewardView rankingRewardView);
        
        // UIの更新処理
        // タブやバナー切り替え時に呼ばれる
        public abstract void UpdateView(object rankingData, bool isOmissionPoint);
    }
}