using Cysharp.Threading.Tasks;
using Pjfb.LeagueMatch;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb.ClubRoyal
{
    // シーズン戦の場合の表示
    public class ClubRoyalSeasonBattleView : MonoBehaviour
    {
        // 所属クラブの戦績表示
        [SerializeField] private ClubRoyalBattleRecordView myClubMatchRecord;
        // 対戦相手の戦績表示
        [SerializeField] private ClubRoyalBattleRecordView opponentClubMatchRecord;
        // 対戦表
        [SerializeField] private LeagueMatchBoardUI matchBoard;
        // 現在のランク
        [SerializeField] private ClubRankImage rankImage;
        // 結果表示Rootオブジェクト
        [SerializeField] private GameObject resultRoot;
        // 結果用テキスト
        [SerializeField] private TMP_Text resultText;

        private LeagueMatchInfo matchInfo = null;
        private GroupLeagueMatchBoardInfo boardInfo = null;
        
         //// <summary> 入れ替え戦前の表示 </summary>
        public async UniTask SetView(LeagueMatchInfo matchInfo, GroupLeagueMatchBoardInfo boardInfo)
        {
            this.matchInfo = matchInfo;
            this.boardInfo = boardInfo;
            
            // 対戦表をセット
            SetLeagueBoard();
            
            // 入れ替え戦以外の全バトルが終了したか
            bool isAllBattleComplete = matchInfo.IsAllBattleComplete(true);
            // 入れ替え戦を除く全試合が終了したなら結果表示
            resultRoot.gameObject.SetActive(isAllBattleComplete);
            // 所属クラブの戦績を表示
            await myClubMatchRecord.SetView(matchInfo, boardInfo, true);
            
            // 全試合が終了してないなら表示(結果表示と入れ替わる)
            await opponentClubMatchRecord.SetView(matchInfo, boardInfo, isAllBattleComplete == false);
           
            // 入れ替え戦以外の対戦が終了しているなら結果文言を切り替える
            if (isAllBattleComplete)
            {
                // 順位
                long ranking = boardInfo.groupStatusDetailSelf.ranking;
                // イベントマスター
                ColosseumEventMasterObject colosseumEvent = matchInfo.MColosseumEvent;
                // グレード数
                long grade = matchInfo.SeasonData.UserSeasonStatus.gradeNumber;
                // グレードマスター
                ColosseumGradeMasterObject gradeMaster = MasterManager.Instance.colosseumGradeMaster.GetJoinedColosseumGradeMasterObject(colosseumEvent.mColosseumGradeGroupId, grade);

                // 結果の集計が完了したか
                if (IsCompeteCollectResult(boardInfo))
                {

                    // 入れ替え戦無しで昇格
                    if (boardInfo.isForcePromotedWithoutShiftMatch)
                    {
                        // 昇格後のグレードで表示するのでシーズン終了後のグレードで計算する
                        grade = matchInfo.SeasonData.UserSeasonStatus.groupSeasonStatus.gradeAfter;
                        gradeMaster = MasterManager.Instance.colosseumGradeMaster.GetJoinedColosseumGradeMasterObject(colosseumEvent.mColosseumGradeGroupId, grade);
                        resultText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.none_shift_battle_grade-up"], ranking, gradeMaster.name);
                    }
                    else
                    {
                        // 入れ替え戦に参加
                        if (matchInfo.CanShiftBattle)
                        {
                            // 対戦相手のグレード
                            long opponentGrade = matchInfo.SeasonData.UserSeasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber;
                            // 上位クラブとの入れ替え戦
                            // 対戦相手のグレードと比較する
                            if (grade < opponentGrade)
                            {
                                resultText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.grade-up_shift_battle"], ranking);
                            }
                            // 下位クラブとの入れ替え戦
                            // 入れ替え戦で対戦する相手のグレードが下なら降格戦
                            else if (grade > opponentGrade)
                            {
                                resultText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.grade-down_shift_battle"], ranking);
                            }
                            // 入れ替え戦に参加するようになっているがマスターの設定では条件を満たせないのでエラーを出す(表示は入れ替え戦未参加)
                            else
                            {
                                CruFramework.Logger.LogError("昇格戦、降格戦以外の入れ替え戦が発生しています");
                                resultText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.none_shift_battle"], ranking);
                            }
                        }
                        // 入れ替え戦未参加
                        else
                        {
                            resultText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.none_shift_battle"], ranking);
                        }
                    }
                }
                // 集計がまだの場合は集計中表示
                else
                {
                    resultText.text = StringValueAssetLoader.Instance["club-royal.announce.collecting_result"];
                }
            }
        }
        
        //// <summary> 対戦表データ設定 </summary>
        private void SetLeagueBoard()
        {
            // 対戦表の情報がないなら表示オフ
            if (boardInfo == null)
            {
                matchBoard.gameObject.SetActive(false);
            }
            // 対戦表をセット
            else
            {
                matchBoard.gameObject.SetActive(true);
                // ランク画像をセット
                long grade = matchInfo.SeasonData.UserSeasonStatus.gradeNumber;
                rankImage.SetTexture(grade);
                
                // 本日の対戦のGroupId
                long todayMatchGroupId = -1;
                if (boardInfo.todayMatch != null)
                {
                    todayMatchGroupId = boardInfo.todayMatch.sColosseumGroupStatusId;
                }
                // 対戦表を表示
                matchBoard.InitUI(boardInfo.rowList, boardInfo.groupStatusDetailSelf.sColosseumGroupStatusId, todayMatchGroupId);
            }
        }

        //// <summary> 集計が完了しているか </summary>
        private bool IsCompeteCollectResult(GroupLeagueMatchBoardInfo boardInfo)
        {
            foreach (GroupLeagueMatchBoardRow row in boardInfo.rowList)
            {
                foreach (GroupLeagueMatchBoardCell cell in row.cellList)
                {
                    // 試合が行われていない欄があるなら集計は未完了
                    if ((BattleResult)cell.result == BattleResult.Unprocessed)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}