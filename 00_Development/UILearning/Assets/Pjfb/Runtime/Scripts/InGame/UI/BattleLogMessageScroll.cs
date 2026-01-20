using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb.InGame
{
    public class BattleLogMessageScroll : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private ScrollGrid logScroller;
        [SerializeField] private RectTransform scrollRect;
        
        protected Coroutine ViewCoroutine = null;
        protected bool isDigestActivating = false;
        private CancellationTokenSource source;
        protected ScrollGrid LogScroller => logScroller;
        
        public void SetVisible(bool isVisible)
        {
            canvasGroup.alpha = isVisible ? 1.0f : 0.0f;
        }
        
        public virtual void SetLogMessage()
        {
#if !PJFB_REL
            if (BattleDataMediator.Instance.StopLogPlay)
            {
                ViewCoroutine = StartCoroutine(ViewLogCoroutine(1.0f));
                return;
            }
#endif
            
            if (ViewCoroutine != null || isDigestActivating)
            {
                return;
            }
            
            var battleLogs = BattleLogMediator.Instance.GetViewableLog();
            if (battleLogs == null)
            {
                return;
            }

            if (BattleDataMediator.Instance.IsReplayMode)
            {
                var nextLog = BattleLogMediator.Instance.GetNextLog();
                if (nextLog?.DigestLog != null)
                {
                    if (nextLog.DigestLog.OffenceAbilityId > 0)
                    {
                        BattleDataMediator.Instance.NextMatchUpResult.OffenceAbilityId = nextLog.DigestLog.OffenceAbilityId;
                        BattleDataMediator.Instance.NextMatchUpResult.ActivateAbilityTimingType = nextLog.DigestLog.AbilityTiming;
                    }

                    if (nextLog.DigestLog.MainCharacterData?.CharaId > 0)
                    {
                        BattleDataMediator.Instance.NextMatchUpResult.OffenceCharacterId = nextLog.DigestLog.MainCharacterData.CharaId;
                        BattleDataMediator.Instance.NextMatchUpResult.TargetCharacterId = (nextLog.DigestLog.OtherCharacterDataList.FirstOrDefault()?.CharaId) ?? 0;
                    }
                }
            }
            
            var lastLog = battleLogs.LastOrDefault();
            if (lastLog != null)
            {
                if (lastLog.LogTiming == BattleConst.LogTiming.OnBattleEnd)
                {
                    BattleEventDispatcher.Instance.OnBattleEndCallback();
                    BattleLogMediator.Instance.RemoveLog(lastLog);
                    return;
                }

                if (lastLog.LogTiming == BattleConst.LogTiming.DoNextMatchUp)
                {
                    BattleEventDispatcher.Instance.OnCalledNextMatchUpCallback();
                }

                var delay = lastLog.Delay;
                if (BattleDataMediator.Instance.IsSkipToFinish)
                {
                    delay = 0.0f;
                }

                if (BattleDataMediator.Instance.IsDoubleSpeed)
                {
                    delay *= 0.5f;
                }

                if (!BattleDataMediator.Instance.IsSkipToFinishWithoutView)
                {
                    ViewCoroutine = StartCoroutine(ViewLogCoroutine(delay));                    
                }

                if (BattleDataMediator.Instance.IsReplayMode && lastLog.ElapsedTime > 0.0f)
                {
                    BattleDataMediator.Instance.GameTime = lastLog.ElapsedTime;
                }
                
                var isSkip = BattleDataMediator.Instance.IsSkipToFinish || BattleDataMediator.Instance.IsSkipToFinishWithoutView;
                BattleUIMediator.Instance.HeaderUI.SetData();
                if (lastLog.DigestLog != null && !isSkip)
                {
                    BattleDataMediator.Instance.NextMatchUpResult.RemainDistanceToShoot = lastLog.DigestLog.RemainDistanceToShoot;
                    BattleDataMediator.Instance.CommandData = lastLog.DigestLog.CommandData;

                    // TODO ダイジェスト演出があるならUIをダイジェスト側に切り替えて再生し、再生後に進める（）
                    var type = lastLog.DigestLog.Type;
                    if (type != BattleConst.DigestType.None)
                    {
                        BattleEventDispatcher.Instance.OnDigestActivatedCallback(type);
                        Pause();
                        source = new CancellationTokenSource();
                        BattleUIMediator.Instance.HeaderUI.gameObject.SetActive(BattleGameLogic.IsShowingHeaderDigestType(lastLog.DigestLog.Type));
                        BattleUIMediator.Instance.RadarUI.MoveBall(lastLog.BallPosition, lastLog.DigestLog);
                        BattleUIMediator.Instance.RadarUI.ShowFieldText(lastLog.OffenceSide, lastLog.IsInShootRange);
                        BattleDigestController.Instance.PlayAsync(lastLog.DigestLog,source.Token, () =>
                            { 
                                Resume();
                                BattleEventDispatcher.Instance.OnDigestClosedCallback(type);

                                if (lastLog.DigestLog?.Score?.Count > 0)
                                {
                                    BattleUIMediator.Instance.HeaderUI.SetScore(lastLog.DigestLog.Score[(int)BattleDataMediator.Instance.PlayerSide], lastLog.DigestLog.Score[(int)BattleDataMediator.Instance.EnemySide]);
                                }
                            },
                            () =>
                            {
                                // TODO フラグ渡す
                                BattleEventDispatcher.Instance.OnMatchUpActivatedCallback(lastLog.DigestLog.MatchUpDigestType == BattleConst.MatchUpDigestType.HideSomeWords);
                            }).Forget();
                        
                        if (type == BattleConst.DigestType.Special && BattleDataMediator.Instance.IsReplayGameMode)
                        {
                            BattleDataMediator.Instance.NextMatchUpResult.ResetActivatedAbilityData();
                        }
                    }
                    // スキップ時は即時反映
                    else if (lastLog.DigestLog?.Score?.Count > 0)
                    {
                        BattleUIMediator.Instance.HeaderUI.SetScore(lastLog.DigestLog.Score[(int)BattleDataMediator.Instance.PlayerSide], lastLog.DigestLog.Score[(int)BattleDataMediator.Instance.EnemySide]);                        
                    }
                }

                // メッセージのないログ(=イベント発火専用)のものは表示しない.
                if (string.IsNullOrEmpty(lastLog.MessageLog))
                {
                    BattleLogMediator.Instance.RemoveLog(lastLog);
                }

                // スキップ時はログの表示をしない
                if (!BattleDataMediator.Instance.IsSkipToFinishWithoutView)
                {
                    logScroller.SetItems(battleLogs);
                }
            }

            // スキップ時は変更しない
            if (!BattleDataMediator.Instance.IsSkipToFinishWithoutView)
            {
                logScroller.verticalNormalizedPosition = 1.0f;
            }
        }

        protected virtual IEnumerator ViewLogCoroutine(float nextLogDelay)
        {
            yield return new WaitForSeconds(nextLogDelay);
            ViewCoroutine = null;
            SetLogMessage();
        }

        public void Pause()
        {
            isDigestActivating = true;
            ViewCoroutine = null;
        }

        public void Resume()
        {
            isDigestActivating = false;
            if (gameObject.activeInHierarchy)
            {
                ViewCoroutine = StartCoroutine(ViewLogCoroutine(0f));
            }
        }

        public void Cancel()
        {
            Pause();
            // TODO：再リプレイ時用の為にfalseにしとく（仮処理）
            isDigestActivating = false;
            if (source != null)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
            BattleDigestController.Instance.ForceQuitCurrentDigest();                
        }

        private void OnDestroy()
        {
            if (source != null)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
            
            if (ViewCoroutine != null)
            {
                StopCoroutine(ViewCoroutine);
                ViewCoroutine = null;
            }
        }
    }
}