using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace CruFramework.Adv
{
    [System.Serializable]
    public abstract class AdvCommandFade : IAdvCommand, IAdvCommandSkip
    {
        public enum FadeType
        {
            In, Out
        }
        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.Fades))]
        [AdvDocument("フェードId。")]
        private int fadeId = 0;
        
        [SerializeField]
        [AdvDocument("フェードの種類。")]
        private FadeType type = FadeType.Out;

        [SerializeField]
        [AdvDocument("フェードにかかる時間。")]
        protected float duration = 1.0f;
        
        [SerializeField]
        [AdvDocument("アニメーションが終わるまで待つ。")]
        private bool isWait = true;
        
        /// <summary>FadeIn</summary>
        protected abstract void FadeIn(AdvManager manager, AdvFade fade);
        /// <summary>FadeOut</summary>
        protected abstract void FadeOut(AdvManager manager, AdvFade fade);
        
        void IAdvCommandSkip.Skip(AdvManager manager)
        {
            // フェード取得
            AdvFade fade = manager.GetFade(fadeId);
            if(fade == null)return;
            // 強制終了
            fade.ForceComplete();
        }
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            // フェード取得
            AdvFade fade = manager.GetFade(fadeId);
            if(fade == null)return;
            fade.gameObject.SetActive(true);
            // 待機する
            if(isWait && manager.IsFastMode == false)
            {
                manager.SetWaitTime(duration);
            }

            switch(type)
            {
                case FadeType.In:
                    FadeIn(manager, fade);
                    break;
                case FadeType.Out:
                    FadeOut(manager, fade);
                    break;
            }
        }
    }
}