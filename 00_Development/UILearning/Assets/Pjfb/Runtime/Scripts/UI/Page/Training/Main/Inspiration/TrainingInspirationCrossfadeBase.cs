using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using System.Linq;
using Pjfb.Master;

namespace Pjfb.Training
{
    public abstract class TrainingInspirationCrossfadeBase<TView> : MonoBehaviour where TView : Component
    {
        
        private const int GroupCount = 2;
        
        [SerializeField]
        private Animator animator = null;

        [SerializeField]
        private TView viewPrefab = null;        
        [SerializeField]
        private int viewCount = 3;
        [SerializeField]
        private CanvasGroup group1 = null;
        [SerializeField]
        private CanvasGroup group2 = null;

        // グループ
        private TView[,] viewGroup = null;
        
        // 表示しているグループ
        private int currentGroupIndex = 0;
        // リストのグループ数
        private int viewListGroupCount = 0;
        // 表示するアイコンリスト
        private List<long> inspirationIds = new List<long>();
        
        /// <summary>アニメーション自動</summary>
        protected  virtual bool IsAutoAnimation{get{return true;}}
        
        /// <summary>切り替えが必要</summary>
        public bool NeedChangeAnimation{get{return inspirationIds.Count > viewCount;}}
        
        private void Awake()
        {
            viewGroup = new TView[GroupCount, viewCount];
            //　Viewを生成
            for(int i=0;i<viewCount;i++)
            {
                viewGroup[0, i] = GameObject.Instantiate<TView>(viewPrefab, group1.transform);
                viewGroup[1, i] = GameObject.Instantiate<TView>(viewPrefab, group2.transform);
            }
        }
        
        /// <summary>Idを登録</summary>
        public void SetIds(List<long> inspirationIds)
        {
            SetIds(inspirationIds.ToArray());
        }

        /// <summary>Idを登録</summary>
        public void SetIds(long[] inspirationIds)
        {
            // なし
            if(inspirationIds.Length <= 0)
            {
                // 非表示
                gameObject.SetActive(false);
                return;
            }
            
            // ソート
            inspirationIds = inspirationIds
                .OrderByDescending(v=> MasterManager.Instance.trainingCardInspireMaster.FindData(v).grade)
                .ThenByDescending(v=> MasterManager.Instance.trainingCardInspireMaster.FindData(v).priority)
                .ThenByDescending(v=> v)
                .ToArray();
            
            // リストをコピー
            this.inspirationIds.Clear();
            this.inspirationIds.AddRange(inspirationIds);
            // リストのグループ数を計算
            viewListGroupCount = (inspirationIds.Length - 1) / viewCount + 1;
            // 表示
            gameObject.SetActive(true);
            // 表示グループ初期化
            currentGroupIndex = 0; 
            
            // 数が指定以下の場合はアニメーターはオフにする
            animator.enabled = false;
            animator.enabled = NeedChangeAnimation && IsAutoAnimation;
            // グループ初期化
            group1.alpha = 1.0f;
            group2.alpha = 0;
            // アニメーションが動かないときはグループ２は非表示にしておく            
            group2.gameObject.SetActive(NeedChangeAnimation);
            
            // 通知
            OnPreUpdateView();
            // 表示更新
            UpdateGroupViews();
            // 通知
            OnUpdateView();
        }
        
        private void UpdateGroupViews()
        {
            int index = currentGroupIndex % GroupCount;
            int start = currentGroupIndex % viewListGroupCount * viewCount;
            
            for(int i=0;i<viewCount;i++)
            {
                // View位置
                int viewIndex = i + start;
                // View
                TView view = viewGroup[index, i];

                // 数をチェック
                if(viewIndex < inspirationIds.Count)
                {
                    view.gameObject.SetActive(true);
                    OnSetView(view, inspirationIds[viewIndex]);
                }
                else
                {
                    view.gameObject.SetActive(false);
                }
            }
            
            currentGroupIndex++;
        }
        
        /// <summary>
        /// Animator
        /// </summary>
        public void FadeEnd()
        {
            // 自動再生がOffのときはアニメーションを切る
            if(IsAutoAnimation == false)
            {
                animator.enabled = false;
            }
            
            OnFadeEnd();
        }
        
        /// <summary>
        /// Animator
        /// </summary>
        public void NextImage()
        {
            // 通知
            OnPreUpdateView();
            // Viewの更新
            UpdateGroupViews();
            // 通知
            OnUpdateView();
        }
        
        /// <summary>フェードの再生</summary>
        protected void PlayFadeAnimation()
        {
            animator.enabled = true;
        }
        
        /// <summary>Viewの表示</summary>
        protected abstract void OnSetView(TView view, long id);
        
        /// <summary>グループ更新時</summary>
        protected virtual void OnPreUpdateView(){}
        /// <summary>グループ更新時</summary>
        protected virtual void OnUpdateView(){}
        /// <summary>初期化</summary>
        protected virtual void OnInitialize(){}
        /// <summary>フェード終了時</summary>
        protected virtual void OnFadeEnd(){}
    }
}