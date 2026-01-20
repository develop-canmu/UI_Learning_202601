using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame
{
    public class NewInGameHeaderUI : MonoBehaviour
    {
        [SerializeField] private RawImage[] scoreImages;
        [SerializeField] private RawImage clockImage;
        [SerializeField] private Texture[] fontTexture;
        [SerializeField] private Texture[] clockTexture;
        [SerializeField] private Texture timeUpTexture;
        [SerializeField] private Image alertImage;

        private Tween alertTween;

        public void SetData()
        {
            SetRemainTime(BattleDataMediator.Instance.GameTime);
        }

        public void SetAsReplayMode()
        {
            var scoreLog = BattleLogMediator.Instance.BattleLogs.FirstOrDefault(log => log.DigestLog?.Score.Count == 2);
            if (scoreLog != null)
            {
                var leftScore = scoreLog.DigestLog.Score[(int)BattleDataMediator.Instance.PlayerSide];
                var rightScore = scoreLog.DigestLog.Score[(int)BattleDataMediator.Instance.EnemySide];
                if (scoreLog.OffenceSide == BattleDataMediator.Instance.PlayerSide)
                {
                    leftScore--;
                }
                else
                {
                    rightScore--;
                }
                
                SetScore(leftScore, rightScore);
            }
        }

        public void SetScore(int leftSideScore, int rightSideScore)
        {
            var leftTextureIndex = Math.Clamp(leftSideScore, 0, fontTexture.Length);
            var rightTextureIndex = Math.Clamp(rightSideScore, 0, fontTexture.Length);
            scoreImages[0].texture = fontTexture[leftTextureIndex];
            scoreImages[1].texture = fontTexture[rightTextureIndex];

            if (rightSideScore == BattleConst.RequiredScore - 1)
            {
                StartAlertFade();
            }
        }

        public void SetRemainTime(float elapsedTime)
        {
            if (elapsedTime <= 0.0f)
            {
                alertTween?.Kill();
                alertImage.gameObject.SetActive(false);
            }
            
            // failsafe
            if (clockTexture.Length == 0)
            {
                clockImage.gameObject.SetActive(false);
                return;
            }

            if (elapsedTime >= BattleConst.BattleTime)
            {
                clockImage.texture = timeUpTexture;
                return;
            }
            
            var elapsedRate = elapsedTime / BattleConst.BattleTime;
            var ratePerTexture = 1.0f / clockTexture.Length;
            for (var i = 0; i < clockTexture.Length; i++)
            {
                if (elapsedRate < ratePerTexture * (i + 1))
                {
                    clockImage.texture = clockTexture[i];
                    break;
                }
            }

            // 仕様に書いてないけどおれのエゴを出していく.
            if (elapsedRate >= 0.8f)
            {
                var leftScore = BattleDataMediator.Instance.GetScore(BattleConst.TeamSide.Left);
                var rightScore = BattleDataMediator.Instance.GetScore(BattleConst.TeamSide.Right);
                if (leftScore < rightScore)
                {
                    StartAlertFade();
                }
            }
        }

        private void StartAlertFade()
        {
            alertImage.gameObject.SetActive(true);
            alertImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            alertTween = alertImage.DOFade(1.0f, 1.0f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}