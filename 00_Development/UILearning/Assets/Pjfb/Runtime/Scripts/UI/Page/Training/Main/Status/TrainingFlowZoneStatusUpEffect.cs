using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace Pjfb.Training
{
    /// <summary> Flowゾーン中のステータス獲得エフェクト(キャラの背景に出すエフェクト) </summary>
    public class TrainingFlowZoneStatusUpEffect : MonoBehaviour
    {
        [SerializeField]
        private PlayableDirector director = null;
        
        // 待機フラグ
        private bool isWaiting = false;
        
        public async UniTask PlayAnimationAsync(CancellationToken token = default)
        {
            gameObject.SetActive(true);
            isWaiting = true;
            director.Play();
            
            // 再生終了まで待機
            await UniTask.WaitUntil(() => isWaiting == false, cancellationToken: token);
        }

        /// <summary> 再生が終わるまで待機 </summary>
        public async UniTask WaitAnimationEndAsync(CancellationToken token = default)
        {
            await UniTask.WaitUntil(() => director.state != PlayState.Playing, cancellationToken: token);
        }

        /// <summary> 待機フラグを解除して次のエフェクトに進ませる </summary>
        public void NextEffect()
        {
            isWaiting = false;
        }
        
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}