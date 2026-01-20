using System;
using System.Collections;
using System.Collections.Generic;

namespace Pjfb.InGame
{
    public class BattleEventDispatcher
    {
        public static BattleEventDispatcher Instance
        {
            get;
            private set;
        }

        public BattleEventDispatcher()
        {
            Instance = this;
        }

        public void Release()
        {
            Instance = null;
        }
        
        public Action OnSetBattleData;
        public Action OnAutoPlayActivatedAction;
        public Action<BattleConst.DigestType> OnDigestActivatedAction;
        public Action<BattleConst.DigestType> OnDigestClosedAction;
        public Action OnAddLogAction;
        public Action<BattleMatchUpCommandData> OnClickCommandButtonAction;
        public Action OnBattleStartAction;
        public Action OnBattleEndAction;
        public Action<bool> OnMatchUpActivatedAction;
        public Action<long, bool> OnActivateUseAbilityUIAction;
        public Action OnActivateActiveAbilityAction;
        public Action<bool> OnSwipedAbilityAction;
        public Action OnCalledNextMatchUpAction;
        public Action OnClickFooterSkipButtonAction;

        public Action<int> ReplayDigestAction;

        public void OnSetBattleDataCallback()
        {
            OnSetBattleData?.Invoke();
        }

        public void OnAutoPlayActivatedCallback()
        {
            OnAutoPlayActivatedAction?.Invoke();
        }
        
        public void OnDigestActivatedCallback(BattleConst.DigestType digestType)
        {
            OnDigestActivatedAction?.Invoke(digestType);
        }

        public void OnDigestClosedCallback(BattleConst.DigestType digestType)
        {
            OnDigestClosedAction?.Invoke(digestType);
        }
        
        public void OnAddLogCallback()
        {
            OnAddLogAction?.Invoke();
        }

        public void OnClickCommandButtonCallback(BattleMatchUpCommandData commandData)
        {
            OnClickCommandButtonAction?.Invoke(commandData);
        }
        
        public void OnBattleStartCallback()
        {
            OnBattleStartAction?.Invoke();
        }
        
        public void OnBattleEndCallback()
        {
            OnBattleEndAction?.Invoke();
        }

        public void OnMatchUpActivatedCallback(bool hideSomePhrase)
        {
            OnMatchUpActivatedAction?.Invoke(hideSomePhrase);
        }

        public void OnActivateUseAbilityUICallback(long abilityId, bool autoSwipe)
        {
            OnActivateUseAbilityUIAction?.Invoke(abilityId, autoSwipe);
        }

        public void OnActivateActiveAbilityCallback()
        {
            OnActivateActiveAbilityAction?.Invoke();
        }
        
        public void OnSwipedAbilityCallback(bool isSwiped)
        {
            OnSwipedAbilityAction?.Invoke(isSwiped);
        }
        
        public void OnCalledNextMatchUpCallback()
        {
            OnCalledNextMatchUpAction?.Invoke();
        }

        public void OnClickFooterSkipButtonCallback()
        {
            OnClickFooterSkipButtonAction?.Invoke();
        }

        public void OnClickReplayDigestCallback(int index)
        {
            ReplayDigestAction?.Invoke(index);
        }
    }
}