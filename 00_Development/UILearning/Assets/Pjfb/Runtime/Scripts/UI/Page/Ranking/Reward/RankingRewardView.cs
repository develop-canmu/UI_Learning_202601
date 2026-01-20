using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.Ranking
{
    /// <summary>報酬画面における個人、クラブのViewを定義しているクラス</summary>
    public abstract class RewardView : MonoBehaviour
    {
        /// <summary>報酬のスクロールビュー</summary>
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        
        /// <summary>報酬がない場合に表示するテキストオブジェクト</summary>
        [SerializeField]
        private TextMeshProUGUI annotationText = null;

        /// <summary>報酬が設定されているか否かでScroll内のcontentのアクティブを切り替える</summary>
        [SerializeField]
        private GameObject scrollContentObj = null;

        /// <summary>報酬が設定されているか否かでScroll内のheaderのアクティブを切り替える</summary>
        [SerializeField]
        private GameObject scrollHeaderObj = null;

        /// <summary>報酬がない場合に呼ぶViewの処理</summary>
        public void SetEmpty()
        {
            // Scrollオブジェクトを非表示にする
            SetScrollActive(false);
            
            annotationText.text = StringValueAssetLoader.Instance["ranking.none_reanking_reword"];
            annotationText.gameObject.SetActive(true);
        }
        
        /// <summary>報酬がある場合に呼ぶViewの処理</summary>
        public async UniTask SetViewAsync(RankingRewardListItem.Param[] paramArray)
        {
            // アクティブ切替と同じフレームでScroll構築をするため、1フレーム待機する
            await UniTask.DelayFrame(1);
            
            // Scrollとアノテーションテキストのアクティブを切り替える
            SetScrollActive(true);
            annotationText.gameObject.SetActive(false);
            
            // ScrollGridにアイテムをセットする
            scrollGrid.SetItems(paramArray);
        }
        
        /// <summary>Scrollオブジェクトの表示を切り替える</summary>
        private void SetScrollActive(bool isActive)
        {
            // verticalScrollbarのアクティブ切替はプロパティで行う必要がある
            scrollGrid.vertical = isActive;
            scrollContentObj.SetActive(isActive);
            scrollHeaderObj.SetActive(isActive);
        }
    }
    
    /// <summary>ランキング報酬画面のViewを管理するクラス</summary>
    public class RankingRewardView : MonoBehaviour
    {
        /// <summary>個人、クラブシートを開くためのマネージャー</summary>
        [SerializeField]
        private RankingAffiliateTabSheetManager rankingAffiliateTabSheetManager = null;

        /// <summary>報酬画面におけるタブ切替のイベント登録を行うための参照</summary>
        public RankingAffiliateTabSheetManager RankingAffiliateTabSheetManager => rankingAffiliateTabSheetManager;

        /// <summary>個人の報酬画面のView</summary>
        [SerializeField]
        private RewardView personalView = null;

        /// <summary>クラブの報酬画面のView</summary>
        [SerializeField]
        private RewardView clubView = null;
        
        /// <summary>個人もしくはクラブのView</summary>
        private RewardView rewardView = null;
        
        /// <summary>
        /// 個人もしくはクラブシートが所持する報酬データを元に報酬画面で表示する
        /// </summary>
        /// <param name="rankingPrizes">報酬データの配列</param>
        /// <param name="sheetType">個人かクラブか</param>
        // TODO: 末端の個人、クラブシートから所持しているrankingDataの報酬が引数に渡されるので、各ScrollGridに対する処理を実装
        public void SetView(RankingPrize[] rankingPrizes, long myRank, RankingAffiliateTabSheetType sheetType)
        {
            // sheetTypeから個人、クラブのViewをキャッシュしておく
            switch (sheetType)
            {
                case RankingAffiliateTabSheetType.Personal:
                    rewardView = personalView;
                    break;
                case RankingAffiliateTabSheetType.Club:
                    rewardView = clubView;
                    break;
            }
            
            // 報酬があるかの判定を行う
            if (rankingPrizes.Length == 0)
            {
                // 報酬がない場合はテキストを表示する
                rewardView.SetEmpty();
                return;
            }
            
            // 縦並びに配置するScrollGridに渡すデータを作成する
            RankingRewardListItem.Param[] paramArray = new RankingRewardListItem.Param[rankingPrizes.Length];

            // 報酬リストのデータをセットする
            for (int i = 0; i < rankingPrizes.Length; i++)
            {
                RankingRewardListItem.Param param = new RankingRewardListItem.Param
                (
                    rankingPrizes[i].upperRanking,
                    rankingPrizes[i].lowerRanking,
                    rankingPrizes[i].prizeList,
                    rankingPrizes[i].upperRanking <= myRank && myRank <= rankingPrizes[i].lowerRanking
                );

                paramArray[i] = param;
            }
            
            // sheetTypeによってViewをセットする
            rankingAffiliateTabSheetManager.OpenSheet(sheetType, null);
            
            // 報酬ありの個人、クラブのViewをセットする
            rewardView.SetViewAsync(paramArray).Forget();
        }
    }
}