using CruFramework.Page;
using UnityEngine;
using Pjfb.UserData;
using Pjfb.Networking.App.Request;

namespace Pjfb.Ranking
{
    public class RankingRewardSheetPersonalTab : Sheet
    {
        /// <summary>報酬画面の個人のヘッダー</summary>
        [SerializeField]
        private RankingUserInfoView userInfoView = null;

        /// <summary>報酬画面の個人のヘッダーをセットする</summary>
        public void SetCharacterPowerUserInfoHeader(long myRank, RankDeckChara myChara)
        {
            // ヘッダー部分のアクティブ化
            userInfoView.gameObject.SetActive(true);
            
            // 選手戦力のヘッダーを表示
            userInfoView.ShowCharacterPowerUserInfoHeaderView();
            
            // ユーザー名のセット
            userInfoView.SetName(UserDataManager.Instance.user.name);
            
            // 順位のセット
            userInfoView.SetHeaderRank(myRank);
            
            // 育成対象の育成済みキャラがいるかを判定
            if (myChara == null || myChara.mCharaId == 0)
            {
                // 育成対象の育成済みキャラがいない場合は、いない場合の表示処理を行う
                userInfoView.ShowEmptyCharacterPower(true);
                return;
            }

            // 育成対象の育成済みキャラがいる場合
            userInfoView.ShowEmptyCharacterPower(false);
            
            // 育成対象の育成済みキャラにおけるキャラ名、戦力、アイコン、キャラランクをセット
            userInfoView.SetCharacter(myChara.mCharaId, new BigValue(myChara.combatPower), myChara.charaRank);
            
            // そのキャラを育成したシナリオをセット
            userInfoView.SetScenario(myChara.mTrainingScenarioId);
        }

        /// <summary>総戦力個人報酬画面のヘッダーをセットする</summary>
        public void SetTotalPowerUserInfoHeader(long myRank, BigValue totalPower, RankDeckChara[] myDeckCharaList)
        {
            // ヘッダー部分のアクティブ化
            userInfoView.gameObject.SetActive(true);
            
            // 総戦力のヘッダーを表示
            userInfoView.ShowTotalPowerUserInfoHeaderView();
            
            // ユーザー名のセット
            userInfoView.SetName(UserDataManager.Instance.user.name);
            
            // ユーザーアイコンのセット
            userInfoView.SetUserIcon(UserDataManager.Instance.user.mIconId);
            
            // 順位のセット
            userInfoView.SetHeaderRank(myRank);
            
            // 総戦力関連の情報があるかを判定
            if (myRank == 0 || totalPower == 0 || myDeckCharaList == null || myDeckCharaList.Length == 0)
            {
                // 総戦力関連の情報がない場合
                userInfoView.ShowEmptyTotalPower(true);
                return;
            }
            
            // 総戦力関連の情報がある場合
            userInfoView.ShowEmptyTotalPower(false);
            
            // 総戦力値とデッキランクアイコンのセット
            // レスポンスの総戦力値からデッキランクアイコンのIdを取得
            long totalPowerRankIconId = StatusUtility.GetPartyRank(totalPower);
            userInfoView.SetTotalPower(totalPower, totalPowerRankIconId);
            
            // チームボタンクリック時に表示するDeck情報の作成
            RankingTotalPowerDeckView.CharacterData[] deckInfo = new RankingTotalPowerDeckView.CharacterData[myDeckCharaList.Length];
            for (int i = 0; i < deckInfo.Length; i++)
            {
                RankDeckChara deckChara = myDeckCharaList[i];
                deckInfo[i] = new RankingTotalPowerDeckView.CharacterData(deckChara.mCharaId, deckChara.combatPower, deckChara.charaRank, deckChara.roleNumber);
            }
            
            // ユーザー情報を表示するヘッダーにデッキ情報をセット
            userInfoView.DeckView.SetView(deckInfo);
        }
        
        /// <summary>
        /// ヘッダーの表示を切り替える
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowUserInfoHeader(bool isShow)
        {
            userInfoView.gameObject.SetActive(isShow);
        }
    }
}