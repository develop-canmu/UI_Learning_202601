using CruFramework.UI;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.Ranking
{
    /// <summary>総戦力の個人ランキング</summary>
    public class RankingTotalPowerTabSheetPersonalTab : RankingTotalPowerTabSheet
    {
        /// <summary>ユーザー情報のヘッダー</summary>
        [SerializeField]
        private RankingUserInfoView userInfoView = null;

        /// <summary>レスポンスのキャッシュ</summary>
        private RankingGetUserCommonRankListAPIResponse response = null;

        /// <summary>スクロール</summary>
        [SerializeField]
        private ScrollDynamic scrollDynamic = null;
        
        /// <summary>ランキング情報がない場合にアクティブにするオブジェクト</summary>
        [SerializeField]
        private GameObject annotationTextObj = null;
        
        /// <summary>カテゴリタブ、個人、クラブタブのが切替時にViewの更新を行う</summary>
        public override void UpdateView(object rankingData, bool isOmissionPoint)
        {
            // レスポンスの型にキャスト
            response = (RankingGetUserCommonRankListAPIResponse)rankingData;
            
            // 総戦力では必ずユーザー情報のヘッダーに値が入る
            // ユーザー名のセット
            userInfoView.SetName(UserDataManager.Instance.user.name);
            // ユーザーアイコンのセット
            userInfoView.SetUserIcon(UserDataManager.Instance.user.mIconId);
            // 順位のセット
            userInfoView.SetHeaderRank(response.ranking.myRank);
            // 総戦力値とデッキランクアイコンのセット
            // レスポンスの総戦力値からデッキランクアイコンのIdを取得
            BigValue totalPower = new BigValue(response.ranking.myValue);
            long totalPowerRankIconId = StatusUtility.GetPartyRank(totalPower);
            userInfoView.SetTotalPower(totalPower, totalPowerRankIconId);
            // 総戦力のUIに切り替える
            userInfoView.ShowTotalPowerUserInfoHeaderView();
            
            // ヘッダー内に総戦力関連の情報があるかを判定
            if (response.ranking.myRank == 0 || totalPower == BigValue.Zero || response.ranking.myDeckCharaList == null || response.ranking.myDeckCharaList.Length == 0)
            {
                // ヘッダー内に総戦力関連の情報がない場合
                userInfoView.ShowEmptyTotalPower(true);
                // 総戦力情報がない場合はデッキ部分を非表示にする
                userInfoView.DeckView.ShowDeck(false);
            }
            else
            {
                userInfoView.ShowEmptyTotalPower(false);
            }
            
            // ユーザー情報を表示するヘッダーのDeckに渡すパラメータの作成
            RankingTotalPowerDeckView.CharacterData[] deckInfo = new RankingTotalPowerDeckView.CharacterData[response.ranking.myDeckCharaList.Length];
            for (int i = 0; i < deckInfo.Length; i++)
            {
                RankDeckChara deckChara = response.ranking.myDeckCharaList[i];
                deckInfo[i] = new RankingTotalPowerDeckView.CharacterData(deckChara.mCharaId, deckChara.combatPower, deckChara.charaRank, deckChara.roleNumber);
            }
            
            // ユーザー情報を表示するヘッダーにデッキ情報をセット
            userInfoView.DeckView.SetView(deckInfo);
            
            // スクロールに渡すアイテムのリストを生成する
            RankingTotalPowerScrollDynamicItem.RankingUserData[] rankingUserDataArray = new RankingTotalPowerScrollDynamicItem.RankingUserData[response.ranking.deckRankList.Length];
            
            // ランキング情報がない場合は、その旨を示すアノテーションを表示する
            annotationTextObj.SetActive(response.ranking.deckRankList.Length == 0);
            
            for (int i = 0; i < response.ranking.deckRankList.Length; i++)
            {
                // Deckに渡すパラメータの作成
                // Deckのキャラ数分の配列を作成
                RankDeckChara[] deckCharaList = response.ranking.deckRankList[i].deckCharaList;
                RankingTotalPowerDeckView.CharacterData[] rankingDeckInfo = new RankingTotalPowerDeckView.CharacterData[deckCharaList.Length];
                
                for (int n = 0; n < deckCharaList.Length; n++)
                {
                    RankDeckChara deckChara = deckCharaList[n];
                    rankingDeckInfo[n] = new RankingTotalPowerDeckView.CharacterData(deckChara.mCharaId, deckChara.combatPower, deckChara.charaRank, deckChara.roleNumber);
                }
                
                // アイテムの作成
                RankingTotalPowerScrollDynamicItem.RankingUserData rankingUserData =
                    new RankingTotalPowerScrollDynamicItem.RankingUserData(
                        response.ranking.deckRankList[i].id,
                        response.ranking.deckRankList[i].rank,
                        response.ranking.deckRankList[i].mIconId,
                        response.ranking.deckRankList[i].name,
                        new BigValue(response.ranking.deckRankList[i].combatPower),
                        response.ranking.deckRankList[i].deckRank,
                        rankingDeckInfo,
                        // 自身がランクインしているか
                        response.ranking.deckRankList[i].id == UserDataManager.Instance.user.uMasterId
                    );
                
                rankingUserDataArray[i] = rankingUserData;
            }
            
            // スクロールにアイテムリストを渡す
            scrollDynamic.SetItems(rankingUserDataArray);
            
            // 開いた際に全てのアイテムを読み込むようにする
            scrollDynamic.ForceLoadAll();
        }
        
        /// <summary>個人、クラブタブ切替時に報酬データをセットする</summary>
        public override void OnUpdateRewardView(RankingRewardView rankingRewardView)
        {
            rankingRewardView.SetView(response.ranking.rankingPrizeList, response.ranking.myRank, RankingAffiliateTabSheetType.Personal);
        }
    }
}