using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.ClubMatch;
using UnityEngine;
using TMPro;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.LeagueMatch
{
    public class MatchBoardRowUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI clubName;
        [SerializeField] private ClubEmblemImage clubEmblem;
        [SerializeField] private LeagueMatchIndex tableIndex;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private RectTransform resultParent;
        [SerializeField] private MatchResultCell resultCellPrefab;
        [SerializeField] private GameObject vsFlag;
        [SerializeField] private GameObject inactiveFlag;
        
        private GroupLeagueMatchBoardRow boardRow = null;
        
        public async UniTask Setup(GroupLeagueMatchBoardRow boardRow, LeagueMatchBoardUI.ResultType resultType, string rowIndex, Func<long, Color> getIndexColor, bool currentMatch = false)
        {
            this.boardRow = boardRow;
            this.clubName.text = boardRow.name;
            this.tableIndex.IniUI(rowIndex, getIndexColor(boardRow.sColosseumGroupStatusId));
            
            switch (resultType)
            {
                // 勝利数
                case LeagueMatchBoardUI.ResultType.WinCount:
                {
                    resultText.text = boardRow.winCount.ToString();
                    break;
                }
                // 得点数
                case LeagueMatchBoardUI.ResultType.Score:
                {
                    resultText.text = boardRow.score.ToString();
                    break;
                }
            }
           
            vsFlag.gameObject.SetActive(currentMatch);
            await clubEmblem.SetTextureAsync(boardRow.mGuildEmblemId);
            
            foreach (var boardCellData in boardRow.cellList)
            {
                var resultCell = Instantiate(resultCellPrefab, transform, true);
                resultCell.gameObject.name = "Cell"+boardCellData.groupIndex;
                resultCell.transform.localScale = Vector3.one;
                resultCell.gameObject.SetActive(true);
                resultCell.ShowResult((BattleResult)boardCellData.result);
                resultParent.SetAsLastSibling();
            }
        }
        
        /// <summary>
        /// uGUI
        /// </summary>
        public void OnLongTapClub()
        {
            LeagueMatchUtility.OpenClubInfo(boardRow.groupId, boardRow.groupType).Forget();
        }
    }
}