using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using System.Linq;
using Pjfb.Master;

namespace Pjfb.Training
{
    public class TrainingInspirationDescriptionCrossfade : TrainingInspirationCrossfadeBase<TrainingGetInspirationView>
    {
        
        private enum AnimationState
        {
            Initialize, StartWait, TextScroll, EndWait, Complete
        }
        
        [SerializeField]
        private float startWaitDuration = 1.0f;
        [SerializeField]
        private float endWaitDuration = 1.0f;
        [SerializeField]
        private float textScrollSpeed = 30.0f;
        
        // 自動アニメーションOff
        protected override bool IsAutoAnimation{ get{return false;}}
        // 表示中のViewリスト
        private List<TrainingGetInspirationView> currentViews = new List<TrainingGetInspirationView>();
        // フェードアウトするView
        private List<TrainingGetInspirationView> fadeoutViews = new List<TrainingGetInspirationView>();
        
        private AnimationState state = AnimationState.StartWait;
        private float stateTimer = 0;
        
        private void SetState(AnimationState state)
        {
            this.state = state;
            stateTimer = 0;
        }

        protected override void OnFadeEnd()
        {
            InitializePosition(fadeoutViews);
        }

        protected override void OnPreUpdateView()
        {
            fadeoutViews.Clear();
            fadeoutViews.AddRange(currentViews);
            // リストの初期化
            currentViews.Clear();
        }
        
        protected override void OnUpdateView()
        {
            InitializePosition(currentViews);
            // ステート初期化
            SetState(AnimationState.Initialize);
        }

        protected override void OnSetView(TrainingGetInspirationView view, long id)
        {
            view.SetInspiration(id, true);
            // 表示リストに追加
            currentViews.Add(view);
        }
        
        /// <summary>
        /// テキストのスクロール
        /// 完了してる場合はTrue
        /// </summary>
        private bool TextScroll(TrainingGetInspirationView view)
        {
            float diff = view.NameTransform.sizeDelta.x - view.MaskTransform.rect.width;
            if(diff <= 0)return true;

            Vector3 p = view.NameTransform.localPosition;
            p.x = Mathf.Max(-diff, p.x - textScrollSpeed * Time.deltaTime);
            view.NameTransform.localPosition = p;
            
            return p.x <= -diff;
        }
        
        private void InitializePosition(List<TrainingGetInspirationView> list)
        {
            foreach(TrainingGetInspirationView view in list)
            {
                // 座標を初期化
                Vector3 p = view.NameTransform.localPosition;
                p.x = 0;
                view.NameTransform.localPosition = p;
            }
        }
        
        private void Update()
        {
            switch(state)
            {
                // 初期化
                case AnimationState.Initialize:
                {
                    InitializePosition(currentViews);
                    SetState(AnimationState.StartWait);
                    break;
                }
            
                // 最初の待機
                case AnimationState.StartWait:
                {
                    // 一定時間でテキストスクロールへ
                    if(stateTimer >= startWaitDuration)
                    {
                        SetState(AnimationState.TextScroll);
                    }
                    break;
                }
                
                // テキストスクロール
                case AnimationState.TextScroll:
                {
                    int completeCount = 0;
                    // 各Viewのスクロール
                    foreach(TrainingGetInspirationView v in currentViews)
                    {
                        if(TextScroll(v))
                        {
                            completeCount++;
                        }
                    }
                    // 全部終わった
                    if(completeCount >= currentViews.Count)
                    {
                        SetState(AnimationState.EndWait);
                    }
                
                    break;
                }
                
                // 最後の待機
                case AnimationState.EndWait:
                {
                    // 一定時間で終了
                    if(stateTimer >= endWaitDuration)
                    {
                        // 切り替えが必要な場合
                        if(NeedChangeAnimation)
                        {
                            SetState(AnimationState.Complete);
                            PlayFadeAnimation();
                        }
                        else
                        {
                            SetState(AnimationState.Initialize);
                        }
                    }
                    break;
                }
            }
            
            stateTimer += Time.deltaTime;
        }
        
    }
}