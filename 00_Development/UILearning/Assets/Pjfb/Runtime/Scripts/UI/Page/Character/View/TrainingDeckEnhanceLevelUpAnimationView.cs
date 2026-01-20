using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class TrainingDeckEnhanceLevelUpAnimationView : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text currentLvText;
        [SerializeField] private TMPro.TMP_Text afterLvText;
        
        [SerializeField] private Animator animator;
        
        private static readonly string OpenKey = "Open";
        private static readonly string CloseKey = "Close";
        
        public async UniTask PlayEffect(long currentLv, long afterLv)
        {
            this.gameObject.SetActive(true);
            currentLvText.text = currentLv.ToString();
            afterLvText.text = afterLv.ToString();
            animator.SetTrigger(OpenKey);

            // アニメーションが終わるまで待機
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(CloseKey));
            
            this.gameObject.SetActive(false);
        }
    }
}