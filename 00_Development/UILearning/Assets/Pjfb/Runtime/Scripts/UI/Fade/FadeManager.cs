using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb
{
    public enum FadeType
    {
        Color,
    }

    public class FadeManager : MonoBehaviour
    {
        public enum State
        {
            Idle,
            FadeOut,
            FadeOutComplete,
            FadeIn,
        }

        [SerializeField]
        private UIColorFade colorFade = null;
        
        private State currentState = State.Idle;
        public State CurrentState{get{return currentState;}}
        
        private IFade currentFade = null;

        /// <summary>フェードアウト</summary>
        public async UniTask FadeOutAsync(FadeType fadeType)
        {
            // アイドル状態以外は処理しない
            if(currentState != State.Idle) return;
            // ステートをフェードアウトに変更
            currentState = State.FadeOut;
            // フェード取得
            currentFade = GetFade(fadeType);
            // 待機
            await currentFade.FadeOutAsync(this.GetCancellationTokenOnDestroy());
            // ステートをフェード完了に変更
            currentState = State.FadeOutComplete;
        }

        /// <summary>フェードイン</summary>
        public async UniTask FadeInAsync()
        {
            // フェードアウトが完了していなければ処理しない
            if(currentState != State.FadeOutComplete) return;
            // ステートをフェードインに変更
            currentState = State.FadeIn;
            // 待機
            await currentFade.FadeInAsync(this.GetCancellationTokenOnDestroy());
            // ステートをアイドル状態に変更
            currentState = State.Idle;
        }
        
        /// <summary>フェード取得</summary>
        public IFade GetFade(FadeType fade)
        {
            switch (fade)
            {
                case FadeType.Color: return colorFade;
                default: throw new Exception($"FadeType:{fade}が定義されてません。");
            }
        }
    }
}
