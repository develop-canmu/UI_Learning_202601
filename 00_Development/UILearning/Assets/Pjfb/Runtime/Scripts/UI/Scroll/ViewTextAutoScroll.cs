using Unity.VisualScripting;
using UnityEngine;

namespace Pjfb
{
    public class ViewTextAutoScroll : MonoBehaviour
    {
        
        private enum ScrollState
        {
            Initialize = 0,
            StartWait = 1,
            TextScroll = 2,
            EndWait = 3,
        }
        
        // スクロールの向き設定
        private enum ScrollDirection
        {
            MoveHorizontal = 0,
            MoveVertical = 1,
        }
        
        [SerializeField]
        private float startWaitDuration = 1.0f;
        [SerializeField]
        private float endWaitDuration = 1.0f;
        [SerializeField]
        private float textScrollSpeed = 30.0f;
        [SerializeField]
        private ScrollDirection scrollDirection = ScrollDirection.MoveHorizontal;
        
        
        [SerializeField]
        private RectTransform scrollTargetTransform;
        // マスクオブジェクトのRectTransform
        [SerializeField]
        private RectTransform maskTransform;
        
        private Vector3 initLocalPos;
        
        private ScrollState state = ScrollState.StartWait;
        private float stateTimer = 0;

        private void Start()
        {
            // ピボットを固定
            scrollTargetTransform.pivot = new Vector2(0, 1);
            // LocalPosition基準で動かすため、マスク側もpivotを固定
            maskTransform.pivot = new Vector2(0, 1);
            initLocalPos = new Vector3(0,0,0);
            
            SetState(ScrollState.Initialize);
        }
        
        private void SetState(ScrollState state)
        {
            this.state = state;
            stateTimer = 0;
        }
        
        /// <summary>
        /// テキストのスクロール
        /// 完了してる場合はTrue
        /// </summary>
        private bool TextScroll()
        {
            Vector3 targetLocalPos = scrollTargetTransform.localPosition;
            float diff = 0;
            switch (scrollDirection)
            {
                case ScrollDirection.MoveHorizontal:
                    diff =  scrollTargetTransform.rect.width - maskTransform.rect.width;
                    if(diff <= 0) break;
                    targetLocalPos.x = Mathf.Max(-diff, targetLocalPos.x - textScrollSpeed * Time.deltaTime);
                    scrollTargetTransform.localPosition = targetLocalPos;
                    return targetLocalPos.x <= -diff; 
                case ScrollDirection.MoveVertical:
                    diff = scrollTargetTransform.rect.height - maskTransform.rect.height;
                    if(diff <= 0) break;
                    targetLocalPos.y = Mathf.Max(-diff, targetLocalPos.y + textScrollSpeed * Time.deltaTime);
                    scrollTargetTransform.localPosition = targetLocalPos;
                    return targetLocalPos.y >= diff; 
            }
            return true;
        }
        
       private void Update()
        {
            switch (state)
            {
                // 初期化
                case ScrollState.Initialize:
                {
                    // 座標を初期化
                    scrollTargetTransform.localPosition = initLocalPos;
                    SetState(ScrollState.StartWait);
                    break;
                }

                // 最初の待機
                case ScrollState.StartWait:
                {
                    // 一定時間でテキストスクロールへ
                    if (stateTimer >= startWaitDuration)
                    {
                        SetState(ScrollState.TextScroll);
                    }

                    break;
                }

                // テキストスクロール
                case ScrollState.TextScroll:
                {
                    if (TextScroll())
                    {
                        SetState(ScrollState.EndWait);
                    }

                    break;
                }

                // 最後の待機
                case ScrollState.EndWait:
                {
                    // 一定時間で終了
                    if (stateTimer >= endWaitDuration)
                    {
                        SetState(ScrollState.Initialize);
                    }

                    break;
                }
            }

            stateTimer += Time.deltaTime;
        }
    }
}