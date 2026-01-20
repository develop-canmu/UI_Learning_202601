using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.MatchResult;
using Pjfb.Networking.App.Request;
using Pjfb.Rivalry;
using Pjfb.Training;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.InGame
{
    /// <summary>
    /// stub
    /// </summary>
    public class NewInGameResultPage : Page
    {
        [SerializeField] private UserIcon[] userIcons;
        [SerializeField] private TextMeshProUGUI[] userNameTexts;
        [SerializeField] private Image[] scoreImage;
        [SerializeField] private Sprite[] scoreFontSprites;        
        [SerializeField] private TextMeshProUGUI[] scoreLogTextOriginals;
        [SerializeField] private UIButton nextButton;
        [SerializeField] private UIButton statsButton;
        [SerializeField] private GameObject[] resultTextRoot;
        [SerializeField] private Animator animator;
        
        // Huntのみ：マッチ回数制限
        [SerializeField] private TextMeshProUGUI limitBalloonText;
        [SerializeField] private GameObject limitBalloonObject;
        
        // チップ獲得結果
        [SerializeField] private NewInGameTipResult tipResult = null;

        private ResultParam resultParam;
        
        private const string WinTrigger = "OpenResultWin";
        private const string LoseTrigger = "OpenResultLose";
        private const string DrawTrigger = "OpenResultDraw";
        private const string IdleTrigger = "IdleResult";
        private const string CloseKey = "CloseScore";

        public class ResultParam
        {
            public PageType OpenFrom;
            public int BattleResult;
            public HuntFinishAPIResponse HuntFinishResponse;
            public TrainingProgressAPIResponse TrainingResponse;
            public ColosseumAttackAPIResponse ColosseumResponse;
            public long UseDeckId;
            public object OtherData;

            public ResultParam(PageType openFrom, int battleResult, object otherData)
            {
                OpenFrom = openFrom;
                BattleResult = battleResult;
                OtherData = otherData;
            }
            
            public ResultParam(PageType openFrom, HuntFinishAPIResponse response, int battleResult)
            {
                OpenFrom = openFrom;
                HuntFinishResponse = response;
                BattleResult = battleResult;
            }
            
            public ResultParam(PageType openFrom, TrainingProgressAPIResponse response, int battleResult)
            {
                OpenFrom = openFrom;
                TrainingResponse = response;
                BattleResult = battleResult;
            }

            public ResultParam(PageType openFrom, ColosseumAttackAPIResponse response, int battleResult, long useDeckId)
            {
                OpenFrom = openFrom;
                ColosseumResponse = response;
                BattleResult = battleResult;
                UseDeckId = useDeckId;
            }
        }

        private void Awake()
        {
            nextButton.OnClickEx.AddListener(OnClickNextButton);
            statsButton.OnClickEx.AddListener(OnClickStatsButton);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            resultParam = (ResultParam)args;

            // リプレイから戻ってきたときはスルー
            if (!BattleDataMediator.Instance.IsReplayDigestMode)
            {
                SetUserInfo();
                SetScoreText();
                SetScoreLog();
                SetBalloon();
            }

            return base.OnPreOpen(args, token);
        }

        protected override UniTask OnOpen(object args)
        {
            if (BattleDataMediator.Instance.IsReplayDigestMode && BattleDataMediator.Instance.ReplayTargetCharacterId >= 0)
            {
                OpenPreviousCharacterStats();
            }
            
            return base.OnOpen(args);
        }

        private void SetUserInfo()
        {
            // シングルオンリー
            userNameTexts[0].text = BattleDataMediator.Instance.Players[(int)BattleDataMediator.Instance.PlayerSide].Name;
            userNameTexts[1].text = BattleDataMediator.Instance.Players[(int)BattleDataMediator.Instance.EnemySide].Name;
            
            // TODO アイコン情報とか渡ってきてないので一旦非表示
            userIcons[0].gameObject.SetActive(false);
            userIcons[1].gameObject.SetActive(false);
        }

        private void SetScoreText()
        {
            var scores = BattleDataMediator.Instance.Score;
            // 怖いので丸め
            var leftScore = Math.Clamp(scores[(int)BattleDataMediator.Instance.PlayerSide], 0, 9);
            var rightScore = Math.Clamp(scores[(int)BattleDataMediator.Instance.EnemySide], 0, 9);
            scoreImage[(int)BattleConst.TeamSide.Left].sprite = scoreFontSprites[leftScore];
            scoreImage[(int)BattleConst.TeamSide.Right].sprite = scoreFontSprites[rightScore];
        }

        private void SetScoreLog()
        {
            var scoreLog = BattleDataMediator.Instance.ScoreLog;
            var leftTextOriginal = scoreLogTextOriginals[0];
            var rightTextOriginal = scoreLogTextOriginals[1];
            leftTextOriginal.gameObject.SetActive(false);
            rightTextOriginal.gameObject.SetActive(false);

            for (var i = 0; i < scoreLog.Count; i++)
            {
                var character = BattleDataMediator.Instance.GetBattleCharacter(scoreLog[i]);
                if (character == null)
                {
                    continue;
                }

                var leftText = Instantiate(leftTextOriginal, leftTextOriginal.transform.parent, false);
                var rightText = Instantiate(rightTextOriginal, rightTextOriginal.transform.parent, false);
                leftText.text = " ";
                rightText.text = " ";
                leftText.gameObject.SetActive(true);
                rightText.gameObject.SetActive(true);
                if (character.Side == BattleDataMediator.Instance.PlayerSide)
                {
                    leftText.text = character.Name;
                }
                else
                {
                    rightText.text = character.Name;
                }
            }
        }

        private async void SetBalloon()
        {
            // ライバルリーバトルのみ
            if (resultParam.OpenFrom != PageType.Rivalry)
            {
                limitBalloonObject.SetActive(false);
                return;
            }

            var mHuntTimetableId = resultParam.HuntFinishResponse.mHuntTimetableId;
            var huntTimetableMasterObject = Master.MasterManager.Instance.huntTimetableMaster.FindData(mHuntTimetableId);
            if (huntTimetableMasterObject == null)
            { 
                limitBalloonObject.SetActive(false);
                return;
            }

            await RivalryManager.Instance.UpdateHuntResultStatus(mHuntTimetableId, resultParam.HuntFinishResponse.dailyPlayCount);
            var usedLimit = Rivalry.RivalryManager.Instance.huntResultList.FirstOrDefault(data => data.mHuntTimetableId == mHuntTimetableId)?.dailyPlayCount ?? 0;
            var remainingLimit = huntTimetableMasterObject.dailyPlayCount - usedLimit;
            limitBalloonObject.SetActive(remainingLimit > 0);
            limitBalloonText.text = string.Format(StringValueAssetLoader.Instance[huntTimetableMasterObject.playCountType == (long)HuntPlayCountType.Win ? "rivalry.match_limit.win" : "rivalry.match_limit.challenge"], remainingLimit);
        }

        private void OpenResultText()
        {
            var scores = BattleDataMediator.Instance.Score;
            var leftSideScore = scores[(int) BattleConst.TeamSide.Left];
            var rightSideScore = scores[(int) BattleConst.TeamSide.Right];

            var index = 0;
            if (leftSideScore == rightSideScore)
            {
                index = 2;
            }else if (leftSideScore > rightSideScore)
            {
                index = 0;
            }
            else
            {
                index = 1;
            }

            for (var i = 0; i < resultTextRoot.Length; i++)
            {
                var result = resultTextRoot[i];
                result.SetActive(false);
                if (i == index)
                {
                    result.SetActive(true);
                    Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(_ => result.SetActive(false)).AddTo(gameObject);
                }
            }
        }
        
        protected override void OnOpened(object args)
        {
            OpenResultText();
            
            var scores = BattleDataMediator.Instance.Score;
            var leftScore = Math.Clamp(scores[(int)BattleDataMediator.Instance.PlayerSide], 0, 9);
            var rightScore = Math.Clamp(scores[(int)BattleDataMediator.Instance.EnemySide], 0, 9);
            SetResultTextAnimatorTriggerAsync(leftScore, rightScore).Forget();

            base.OnOpened(args);

#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            DebugAutoChoice();
#endif
        }

#if CRUFRAMEWORK_DEBUG && !PJFB_REL
        private void DebugAutoChoice()
        {
            if (TrainingChoiceDebugMenu.EnabledAutoChoiceGame)
            {
                OnClickNextButton();
            }
        }
#endif
        
        private async UniTask SetResultTextAnimatorTriggerAsync(int leftScore, int rightScore)
        {
            if (BattleDataMediator.Instance.IsReplayDigestMode)
            {
                animator.SetTrigger(IdleTrigger);
                return;
            }
            
            // ボタンを押せないように
            nextButton.interactable = false;
            statsButton.interactable = false;
            
            if (leftScore > rightScore)
            {
                await AnimatorUtility.WaitStateAsync(animator, WinTrigger);
            }
            else if (leftScore < rightScore)
            {
                await AnimatorUtility.WaitStateAsync(animator, LoseTrigger);
            }
            else
            {
                await AnimatorUtility.WaitStateAsync(animator, DrawTrigger);
            }
            
            // ボタンを押せないように
            nextButton.interactable = true;
            statsButton.interactable = true;
        }

        protected virtual async void OnClickNextButton()
        {
            // failsafe
            if (resultParam == null)
            { 
                animator.SetTrigger(CloseKey);
                await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Home, false, null);
                return;
            }
            
            // リーグマッチトップ画面 リプレイ
            if (resultParam.OpenFrom == PageType.LeagueMatch)
            {
                animator.SetTrigger(CloseKey);
                await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.LeagueMatch, true, resultParam.OtherData);
                return;
            }
            
            switch (resultParam.OpenFrom, resultParam.OtherData)
            {
                case (PageType.Training, null):
                    // チップ獲得がある場合は演出表示
                    if(resultParam.TrainingResponse.eventReward.hp > 0)
                    {
                        await tipResult.PlayTipResultAnimationAsync(resultParam.TrainingResponse);
                    }
                    animator.SetTrigger(CloseKey);
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Training, false, resultParam.TrainingResponse);
                    break;
                case (PageType.Story, null):
                case (PageType.Rivalry, null):
                    animator.SetTrigger(CloseKey);
                    var nextParams = new MatchResultPage.Data();
                    nextParams.showHeaderAndFooter = false;
                    nextParams.pageType = resultParam.BattleResult == 1 ? MatchResultPageType.MatchResultWin : MatchResultPageType.MatchResultLose;
                    var mvpCharaMCharaId = BattleDataMediator.Instance.GetMVPCharacter(BattleConst.TeamSide.Left)?.MCharaId;
                    nextParams.args = resultParam.BattleResult == 1 ? 
                            new MatchResultWinPage.Data(resultParam.OpenFrom, resultParam.BattleResult, resultParam.HuntFinishResponse, mvpCharaMCharaId.Value) :
                            new MatchResultLosePage.Data(resultParam.OpenFrom, resultParam.BattleResult, resultParam.HuntFinishResponse);
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.MatchResult, true, nextParams);
                    break;
                case (PageType.Colosseum,null):
                    animator.SetTrigger(CloseKey);
                    nextParams = new MatchResultPage.Data();
                    nextParams.showHeaderAndFooter = false;
                    nextParams.pageType = resultParam.BattleResult == 1 ? MatchResultPageType.MatchResultWinColosseum : MatchResultPageType.MatchResultLose;
                    nextParams.args = resultParam.BattleResult == 1 ? 
                        new MatchResultWinColosseumPage.Data(resultParam.ColosseumResponse) :
                        new MatchResultLosePage.Data(resultParam.OpenFrom, resultParam.BattleResult, resultParam.ColosseumResponse, resultParam.UseDeckId);
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.MatchResult, true, nextParams);
                    break;
                case (PageType.ClubMatch,null):
                    animator.SetTrigger(CloseKey);
                    var playerName = BattleDataMediator.Instance.Players[0].Name;
                    nextParams = new MatchResultPage.Data();
                    nextParams.showHeaderAndFooter = false;
                    nextParams.pageType = resultParam.BattleResult == 1 ? MatchResultPageType.MatchResultWinClubMatch : MatchResultPageType.MatchResultLose;
                    nextParams.args = resultParam.BattleResult == 1 ? 
                        new MatchResultWinClubMatchPage.Data(resultParam.ColosseumResponse,playerName,resultParam.UseDeckId) :
                        new MatchResultLosePage.Data(resultParam.OpenFrom, resultParam.BattleResult, resultParam.ColosseumResponse, resultParam.UseDeckId);
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.MatchResult, true, nextParams);
                    break;
                default:
                    animator.SetTrigger(CloseKey);
                    var nextParam = (TeamConfirmPage.PageParams)resultParam.OtherData;
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.TeamConfirmTrainingMatch, true, nextParam);
                    break;
            }
        }

        private async void OpenPreviousCharacterStats()
        {
            await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.SelectPlaybackGoalScene, BattleDataMediator.Instance.ReplayTargetCharacterId);
        }

        private async void OnClickStatsButton()
        {
            await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.SelectPlayerDisplayStats, BattleDataMediator.Instance.Decks);
        }
    }
}
