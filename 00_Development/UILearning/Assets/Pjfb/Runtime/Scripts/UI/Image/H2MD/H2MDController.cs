using System.Collections;
using System.Collections.Generic;
using CruFramework.H2MD;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.UI
{
    public class H2MDController : MonoBehaviour
    {
        [SerializeField] private H2MDPlayer h2mdPlayer;
        
        // 再生チェック
        public bool isPlaying {get{return h2mdPlayer.IsPlaying;}}

        private void Awake()
        {
            AppManager.Instance.UIManager.H2MDUIManager.AddEffect(this);
        }
        
        private void OnDestroy()
        {
            if(AppManager.Instance == null) return;
            AppManager.Instance.UIManager.H2MDUIManager.RemoveEffect(this);
        }
        
        public void Stop()
        {
            h2mdPlayer.Stop();
        }
        
        public void Play()
        {
            h2mdPlayer.Play();
        }
    }
}