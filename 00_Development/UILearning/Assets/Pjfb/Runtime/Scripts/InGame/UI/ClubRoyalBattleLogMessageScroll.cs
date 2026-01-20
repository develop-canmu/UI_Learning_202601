using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.InGame;

namespace Pjfb
{
    public class ClubRoyalBattleLogMessageScroll : BattleLogMessageScroll
    {
        [SerializeField] private float viewDelay = 1.0f;
        
        public override void SetLogMessage()
        {
            if (ViewCoroutine != null || isDigestActivating)
            {
                return;
            }
            
            List<BattleLog> battleLogs = BattleLogMediator.Instance.GetViewableLog();
            if (battleLogs == null)
            {
                return;
            }
            
            BattleLog lastLog = battleLogs.LastOrDefault();
            if (lastLog != null)
            {
                float delay = viewDelay;
                ViewCoroutine = StartCoroutine(ViewLogCoroutine(delay));
                SetLog(lastLog);
            }
        }
        
        protected override IEnumerator ViewLogCoroutine(float initialDelay)
        {
            float nextLogDelay = initialDelay;
            
            while (nextLogDelay > 0.01f)
            {
                yield return new WaitForSeconds(nextLogDelay);
                
                List<BattleLog> battleLogs = BattleLogMediator.Instance.GetViewableLog();
                if (battleLogs == null || !battleLogs.Any())
                {
                    CruFramework.Logger.LogWarning("No more logs to process. Terminating loop.");
                    break;
                }

                BattleLog lastLog = battleLogs.LastOrDefault();
                if (lastLog != null)
                {
                    nextLogDelay = viewDelay;
                    SetLog(lastLog);
                }
            }
            
            ViewCoroutine = null;
        }
        
        private void SetLog(BattleLog log)
        {
            List<BattleLog> viewLogs = new List<BattleLog>();
            viewLogs.Add(log);
            LogScroller.SetItems(viewLogs);
            LogScroller.verticalNormalizedPosition = 1.0f;
        }
        
    }
}