using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.App.Request;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchBoardUI : MonoBehaviour
    {
        readonly List<string> _matchBoardIndexes = new List<string>()
        {
            "A","B","C","D","E","F","G","H"
        };
        
        //// <summary> ボードに表示する結果 </summary>
        public enum ResultType
        {
            // 勝利数
            WinCount,
            // 得点数
            Score,
        }
        
        [SerializeField] private MatchBoardRowUI linePrefab;
        [SerializeField] private RectTransform headerParent;
        [SerializeField] private RectTransform resultPointParent;
        [SerializeField] private LeagueMatchIndex leagueMatchIndexPrefab;
        [SerializeField] private Color myColor, opponentColor, defaultColor;
        [SerializeField] private ResultType resultType;
        
        private readonly List<MatchBoardRowUI> _recordItems = new List<MatchBoardRowUI>();
        private readonly List<LeagueMatchIndex> _headerIndexItems = new List<LeagueMatchIndex>();

        void ClearItems()
        {
            foreach (var item in _recordItems)
            {
                Destroy(item.gameObject);
            }
            _recordItems.Clear();
            
            foreach (var item in _headerIndexItems)
            {
                Destroy(item.gameObject);
            }
            _headerIndexItems.Clear();
        }
        
        public async void InitUI(GroupLeagueMatchBoardRow[] matchBoardRows, long myGroupId, long todayMatchGroupId)
        {
            ClearItems();
            
            foreach (var matchBoardRow in matchBoardRows)
            {
                var indexInList = (int)matchBoardRow.groupIndex - 1;
                var targetLine = Instantiate(linePrefab, transform, true);
                var index = _matchBoardIndexes[indexInList];
                
                targetLine.gameObject.name = index;
                targetLine.transform.localScale = Vector3.one;
                targetLine.gameObject.SetActive(true);

                Color GetIndexColor(long groupId)
                {
                    Color targetColor;
                    if (groupId == myGroupId)
                    {
                        targetColor = myColor;
                    }
                    else if (groupId == todayMatchGroupId)
                    {
                        targetColor = opponentColor;
                    }
                    else
                    {
                        targetColor = defaultColor;
                    }
                    return targetColor;
                }
                
                await targetLine.Setup(matchBoardRow, resultType, index, GetIndexColor,
                    matchBoardRow.sColosseumGroupStatusId == todayMatchGroupId);
                _recordItems.Add(targetLine);
                
                var teamIndexTop = Instantiate(leagueMatchIndexPrefab, headerParent, true);
                teamIndexTop.gameObject.name = index + "HeaderIndex";
                teamIndexTop.transform.localScale = Vector3.one;
                teamIndexTop.gameObject.SetActive(true);
                teamIndexTop.IniUI(index, GetIndexColor(matchBoardRow.sColosseumGroupStatusId));
                _headerIndexItems.Add(teamIndexTop);
                
            }
            
            resultPointParent.SetAsLastSibling();
            resultPointParent.gameObject.SetActive(true);
        }
    }
}