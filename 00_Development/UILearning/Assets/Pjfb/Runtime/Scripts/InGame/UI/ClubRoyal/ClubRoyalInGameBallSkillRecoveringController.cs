using UnityEngine;

public class ClubRoyalInGameBallSkillRecoveringController : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator;
    public bool IsRecoveringEffect { get; private set; } = false;
    public bool IsAnimating { get; private set; } = false;

    private const string RecoveryUpSkillOpenTrigger = "ContinuousRestorationOpen";
    private const string RecoveryUpSkillCloseTrigger = "ContinuousRestorationClose";
    
    public void SetState(bool isRecoveringEffect)
    {
        if(IsRecoveringEffect == isRecoveringEffect)
        {
            return;
        }

        IsRecoveringEffect = isRecoveringEffect;
        if (IsRecoveringEffect && !IsAnimating)
        {
            IsAnimating = true;
            targetAnimator.ResetTrigger(RecoveryUpSkillCloseTrigger);
            targetAnimator.SetTrigger(RecoveryUpSkillOpenTrigger);
        }
        else if(!IsRecoveringEffect && IsAnimating)
        {
            IsAnimating = false;
            targetAnimator.ResetTrigger(RecoveryUpSkillOpenTrigger);
            targetAnimator.SetTrigger(RecoveryUpSkillCloseTrigger);
        }
    }
}
