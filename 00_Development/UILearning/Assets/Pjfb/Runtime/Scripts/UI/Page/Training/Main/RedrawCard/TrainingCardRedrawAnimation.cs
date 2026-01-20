using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

namespace Pjfb.Training
{
    public class TrainingCardRedrawAnimation : MonoBehaviour
    {
        private static readonly string OpenAnimation = "Open";
        private static readonly string RedoingOutAnimation = "RedoingOut";
        private static readonly string RedoingInAnimation = "RedoingIn";
        private static readonly string CloseAnimation = "Close";
        
        [SerializeField]
        private Animator animator = null;
        
        public async UniTask PlayRedoingOutAnimation()
        {
            await AnimatorUtility.WaitStateAsync(animator, OpenAnimation);
            await AnimatorUtility.WaitStateAsync(animator, RedoingOutAnimation);
        }
        
        public async UniTask PlayRedoingInAnimation()
        {
            await AnimatorUtility.WaitStateAsync(animator, RedoingInAnimation);
            await AnimatorUtility.WaitStateAsync(animator, CloseAnimation);
        }
    }
}