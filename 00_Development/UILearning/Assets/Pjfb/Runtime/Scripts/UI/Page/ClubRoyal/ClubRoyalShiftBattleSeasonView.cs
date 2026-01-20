using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.LeagueMatch;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.ClubRoyal
{
    // クラブ・ロワイヤルの入れ替え戦期間中の表示
    public class ClubRoyalShiftBattleSeasonView : MonoBehaviour
    {
        // 背景Image
        [SerializeField] private Image backGroundImage;
        // ランク変動がない場合の背景画像
        [SerializeField] private Sprite backSpriteRankKeep;
        // 敗北時の背景画像
        [SerializeField] private Sprite backSpriteLose;
        // 勝利時の背景画像
        [SerializeField] private Sprite backSpriteWin;
        
        // 所属クラブの戦績表示
        [SerializeField] private ClubRoyalBattleRecordView myClubMatchRecord;
        // 対戦相手の戦績表示
        [SerializeField] private ClubRoyalBattleRecordView opponentClubMatchRecord;
        // 現在のランク
        [SerializeField] private ClubRankImage currentRank;
        // 次シーズンのランク
        [SerializeField] private ClubRankImage afterRank;

        // ランク変更ラベル
        [SerializeField] private Image rankChangeLabel;
        // ランク昇格時のラベル画像
        [SerializeField] private Sprite rankUpSprite;
        // ランク降格時のラベル画像
        [SerializeField] private Sprite rankDownSprite;
        // ランク維持ラベルポジティブ画像
        [SerializeField] private Sprite rankKeepPositiveSprite;
        // ランク維持ラベルネガティブ画像
        [SerializeField] private Sprite rankKeepNegativeSprite;
        
        // 矢印オブジェクト
        [SerializeField] private GameObject arrowObject;
        // ラベル表示オブジェクト
        [SerializeField] private GameObject announceLabelRoot;
        // ラベルに表示するテキスト
        [SerializeField] private TMP_Text announceLabelText;
        // 結果報告テキスト
        [SerializeField] private TMP_Text announceMessageText;
        
        public async UniTask SetView(LeagueMatchInfo matchInfo, GroupLeagueMatchBoardInfo boardInfo)
        {
            // 自クラブと対戦相手のクラブの戦績を表示
            await myClubMatchRecord.SetView(matchInfo, boardInfo, true);
            await opponentClubMatchRecord.SetView(matchInfo, boardInfo, true);

            // 試合結果
            long result = boardInfo.shiftMatchInfo.result;

            // GroupId
            long gradeGroupId = matchInfo.MColosseumEvent.mColosseumGradeGroupId;
            // グレード名
            string gradeName = "";
            
            // 自クラブのグレード
            long myGrade = matchInfo.SeasonData.UserSeasonStatus.gradeNumber;
            // 対戦相手のグレード
            long opponentGrade = boardInfo.shiftMatchInfo.gradeNumber;
            
            // 昇格戦か
            bool isGradeUpBattle = myGrade < opponentGrade;
            
            // 結果前
            if (ColosseumManager.HasResult(result) == false)
            {
                // 対戦前はRankKeep画像で固定
                backGroundImage.sprite = backSpriteRankKeep;
                // アナウンスラベルは非表示
                announceLabelRoot.SetActive(false);
                
                // Rank変動後の表示
                currentRank.gameObject.SetActive(true);
                afterRank.gameObject.SetActive(true);
                arrowObject.SetActive(true);
                // ランク変動のラベルは非表示に
                rankChangeLabel.gameObject.SetActive(false);
                
                // ランク表示
                await currentRank.SetTextureAsync(myGrade);
                await afterRank.SetTextureAsync(opponentGrade);

                // 昇格戦
                if (isGradeUpBattle)
                {
                    // 対戦相手のグレード名
                    gradeName = GetGradeName(gradeGroupId, opponentGrade);
                    // テキストをセット
                    announceMessageText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.grade-up_shift_battle.message"], gradeName);      
                }
                // 降格戦
                else
                {
                    // 自分のグレード名
                    gradeName = GetGradeName(gradeGroupId, myGrade);
                    // テキストをセット
                    announceMessageText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.grade-down_shift_battle.message"], gradeName);
                }
            }
            // 結果後
            else
            {
                // アナウンスラベルを表示
                announceLabelRoot.SetActive(true);
                
                // ランク変動後の表示のみに
                currentRank.gameObject.SetActive(false);
                afterRank.gameObject.SetActive(true);
                arrowObject.SetActive(false);
                // ランク変動ラベルを表示
                rankChangeLabel.gameObject.SetActive(true);
                
                // 勝利
                if (result == ColosseumManager.ResultWin)
                {
                    // 昇格戦
                    if (isGradeUpBattle)
                    {
                        // ランク昇格
                        backGroundImage.sprite = backSpriteWin;
                        rankChangeLabel.sprite = rankUpSprite;
                        await afterRank.SetTextureAsync(opponentGrade);
                        // 対戦相手のグレード名
                        gradeName = GetGradeName(gradeGroupId, opponentGrade);
                        announceMessageText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.grade-up_shift_battle.win"], gradeName);
                    }
                    // 降格戦
                    else
                    {
                        // 降格戦に勝った場合はランク維持
                        backGroundImage.sprite = backSpriteRankKeep;
                        rankChangeLabel.sprite = rankKeepPositiveSprite;
                        await afterRank.SetTextureAsync(myGrade);
                        // 自分のグレード名
                        gradeName = GetGradeName(gradeGroupId, myGrade);
                        announceMessageText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.grade-down_shift_battle.win"], gradeName);
                    }

                    // ラベルテキスト
                    announceLabelText.text = StringValueAssetLoader.Instance["club-royal.announce.shift_battle.win.label"];
                }
                // 敗北
                else if (result == ColosseumManager.ResultLose)
                {
                    backGroundImage.sprite = backSpriteLose;
                    
                    // 昇格戦
                    if (isGradeUpBattle)
                    {
                        rankChangeLabel.sprite = rankKeepNegativeSprite;
                        // 対戦相手のグレード名
                        gradeName = GetGradeName(gradeGroupId, opponentGrade);
                        await afterRank.SetTextureAsync(myGrade);
                        announceMessageText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.grade-up_shift_battle.lose"], gradeName);
                    }
                    // 降格戦
                    else
                    {
                        rankChangeLabel.sprite = rankDownSprite;
                        // 対戦相手のグレード名(相手のグレードに降格するので)
                        gradeName = GetGradeName(gradeGroupId, opponentGrade);
                        await afterRank.SetTextureAsync(opponentGrade);
                        announceMessageText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.grade-down_shift_battle.lose"], gradeName);
                    }
                    
                    // ラベルテキスト
                    announceLabelText.text = StringValueAssetLoader.Instance["club-royal.announce.shift_battle.lose.label"];
                }
                // 引き分け
                else if (result == ColosseumManager.ResultDraw)
                {
                    backGroundImage.sprite = backSpriteRankKeep;
                    // 昇格戦、降格戦ごとにラベルを変える
                    rankChangeLabel.sprite = isGradeUpBattle ? rankKeepNegativeSprite : rankKeepPositiveSprite;
                    await afterRank.SetTextureAsync(myGrade);
                    announceLabelText.text = StringValueAssetLoader.Instance["club-royal.announce.shift_battle.draw.label"];
                    announceMessageText.text = StringValueAssetLoader.Instance["club-royal.announce.shift_battle.draw.message"];
                }
                // タイプが見つからない場合はエラーを出す
                else
                {
                    CruFramework.Logger.LogError($"Colosseum Result Type Error : {result}");
                }
            }
        }
        
        //// <summary> 階級名を取得 </summary>
        private string GetGradeName(long groupId, long grade)
        {
            return MasterManager.Instance.colosseumGradeMaster.GetJoinedColosseumGradeMasterObject(groupId, grade).name;
        } 
    }
}