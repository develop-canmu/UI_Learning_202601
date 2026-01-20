using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using DG.Tweening;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
    public class UIGachaFooterButton : MonoBehaviour, IUserDataHandler
    {
        public UIBadgeBalloon FreeGachaBalloon => freeGachaBalloon;
        public UIBadgeBalloon PendingBalloon => pendingBalloon;
        public UIBadgeBalloon PointAlternativeBalloon => pointAlternativeBalloon;

        [SerializeField] private UIBadgeBalloon freeGachaBalloon = null;
        [SerializeField] private UIBadgeBalloon pendingBalloon = null;
        [SerializeField] private UIBadgeBalloon pointAlternativeBalloon = null;
        
        [SerializeField] private CanvasGroupDOTweenCrossFade badgeBalloonCrossFade = null;

        void IUserDataHandler.OnUpdatedData()
        {
            UpdatePointAlternativeBalloon();
        }
        
        public void UpdateBallon( bool isFree, bool isPending)
        {
            UpdateFreeBallon(isFree);
            UpdatePendingBallon(isPending);
        }

        public void UpdateFreeBallon( bool isFree)
        {
            var freeBalloonText = StringValueAssetLoader.Instance["footer.gacha_free"];
            FreeGachaBalloon.SetActive(isFree);
            FreeGachaBalloon.SetText(freeBalloonText);
            badgeBalloonCrossFade.Play();
        }

        public void UpdatePendingBallon( bool isPending)
        {
            var pendingBalloonText = StringValueAssetLoader.Instance["gacha.pending"];
            PendingBalloon.SetActive(isPending);
            PendingBalloon.SetText(pendingBalloonText);
            badgeBalloonCrossFade.Play();
        }
        
        public void UpdatePointAlternativeBalloon()
        {
            var pointAlternativeBalloonText = StringValueAssetLoader.Instance["gacha.usable_alternativepoint"];
            bool showBalloon = CheckAlternativePoint();
            PointAlternativeBalloon.SetActive(showBalloon);
            PointAlternativeBalloon.SetText(pointAlternativeBalloonText);
            badgeBalloonCrossFade.Play();
        }

        // 仮想ポイントを所持しているか確認
        private bool CheckAlternativePoint()
        {
            foreach (var pointAlternative in MasterManager.Instance.pointAlternativeMaster.values)
            {
                if(pointAlternative.useType != (long)AlternativePointUseType.Gacha && pointAlternative.useType != (long)AlternativePointUseType.All) continue;
                if(pointAlternative.EndAtDateTime < AppTime.Now) continue;
                var uPointAlternative = UserDataManager.Instance.point.Find(pointAlternative.mPointIdAlternative);
                if ( uPointAlternative != null && uPointAlternative.value > 0  )
                {
                    return true; 
                }
            }
            return false;
        }
    }
}
