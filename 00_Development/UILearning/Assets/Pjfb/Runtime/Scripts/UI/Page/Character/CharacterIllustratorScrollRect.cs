using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
using System.Reflection;
#endif

namespace Pjfb
{

#if UNITY_EDITOR
    //Custom ScrollRectのSerializeFieldパラメータ表示用のCustomEditor
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CharacterIllustratorScrollRect), true)]
    public class CustomScrollRectEditor : ScrollRectEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (serializedObject.targetObject is CharacterIllustratorScrollRect customScrollRect)
            {
                Type classType = customScrollRect.GetType();
                FieldInfo[] fieldInfos = classType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    FieldInfo fieldInfo = fieldInfos[i];
                    object[] attributes = fieldInfo.GetCustomAttributes(true);
                    bool isSerializeField = false;
                    foreach (var attribute in attributes)
                    {
                        if (attribute.GetType() == typeof(SerializeField))
                        {
                            isSerializeField = true;
                            break;
                        }
                    }
                    if (!isSerializeField) continue;
                    SerializedProperty serializedProperty = serializedObject.FindProperty(fieldInfo.Name);
                    if(serializedProperty != null) EditorGUILayout.PropertyField(serializedProperty);
                }
            }

            serializedObject.ApplyModifiedProperties();

            // 区切り線
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));

            base.OnInspectorGUI();


        }
    }
#endif

    public class CharacterIllustratorScrollRect : ScrollRect, IPointerClickHandler
    {
        [Serializable]
        public struct RangeClass
        {
            public float min, max;
        }

        enum ZoomState 
        {
            Non = 0,
            Max = 1,
            One = 2,
        }

        [SerializeField] private RectTransform ImageRect; 
        [SerializeField] private RangeClass RangeScale;  //拡大縮小の範囲
        [SerializeField] private int ResetToOneAniFrameCount = 10;
        [NonSerialized] public Action BottomButtonsAction = null;

        private ZoomState zoomState = ZoomState.Non;
        private float scale = 1.0f;
        private float pinchScale = 1.0f;
        private float pinchStartDistance = 0;    //ピンチ開始時の指間の距離
        private Vector2 defaultSize = Vector2.zero;
        private Vector2 defaultContentSize = Vector2.zero;
        private bool isPinch = false;   //ピンチ中であればtrue
        private bool isCallButtomAnimation = false;
        private bool isDragging = false;
        private bool isClose = false;
        private Coroutine AutoZoomCoroutine = null;
        private Coroutine ButtomButtonsCoroutine = null;

        //Touch Control相対位置計算用
        int lastInputCount = 0;
        Vector2 currentPos = Vector2.zero;
        Vector2 lastPos = Vector2.zero;

        public void Initialize(RectTransform target)
        {
            ImageRect = target;
            if (defaultSize == Vector2.zero && ImageRect != null) defaultSize = ImageRect.sizeDelta;
            pinchStartDistance = 0;
            zoomState = ZoomState.Non;
            isPinch = false;
            isCallButtomAnimation = false;
            isDragging = false;
            isClose = false;
            ResetSize();
        }

        /// <summary>
        /// min <= 新しい拡大率 <= max に設定する
        /// </summary>
        private float CheckScaleRange(float newScale)
        {

            float s;
            if (newScale >= RangeScale.max)
                s = RangeScale.max;
            else if (newScale <= RangeScale.min)
                s = RangeScale.min;
            else
                s = newScale;

            return s;
        }

        /// <summary>
        /// 設定された拡大率に基づいてオブジェクトの大きさを更新する
        /// </summary>
        private void UpdateScaling(float currentScale)
        {
            if (ImageRect != null) 
            {
                ImageRect.sizeDelta = defaultSize * currentScale;
                if (currentScale >= 1.9) //190%〜MAX%、左右100pxの余白追加
                {
                    var contentWidth = ImageRect.sizeDelta.x + 200;
                    var contentHeight = (ImageRect.sizeDelta.y > defaultContentSize.y) ? ImageRect.sizeDelta.y : defaultContentSize.y;
                    content.sizeDelta = new Vector2(contentWidth, contentHeight);
                }
                else 
                {
                    content.sizeDelta = defaultContentSize;
                }
            }
        }

        private void ResetSize()
        {
            scale = 1.0f;
            pinchScale = 1.0f;
            if (ImageRect != null) ImageRect.sizeDelta = defaultSize;
        }

        public void ResetToCenter() 
        {
            verticalNormalizedPosition = 0.5f;
            horizontalNormalizedPosition  = 0.5f;
            ResetSize();
        }

        public void SetContentSize(Vector2 size) 
        {
            defaultContentSize = size;
            content.sizeDelta = size;
        }


#region Animation
        private void SetAutoZoomState() 
        {
            if (isClose) 
            {
                zoomState = ZoomState.One;
                return;
            }

            scale = (defaultSize.x > 0) ? ImageRect.sizeDelta.x / defaultSize.x : 0;
            zoomState = (scale == 1) ? ZoomState.Max : ZoomState.One ;
        }

        private IEnumerator AutoZoomAnimation()
        {
            if (AutoZoomCoroutine != null) yield break;

            SetAutoZoomState();
            float targetScale = (zoomState == ZoomState.Max) ? RangeScale.max : 1.0f;
            int totalFrame = ResetToOneAniFrameCount;
            int frame = 0;
            while (frame < totalFrame)
            {
                float tempStep = (float)frame / (float)totalFrame;
                scale = Mathf.Lerp(scale, targetScale, Mathf.SmoothStep(0, 1, tempStep));
                if (zoomState == ZoomState.One) content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, Vector2.zero, Mathf.SmoothStep(0, 1, tempStep));
                UpdateScaling(scale);
                yield return null;
                frame++;
            }
            if (zoomState == ZoomState.One)
            {
                ResetToCenter();
            }
            else 
            {
                scale = targetScale;
                UpdateScaling(scale);
            }
            
            zoomState = ZoomState.Non;
            AutoZoomCoroutine = null;
        }

        IEnumerator CallButtonAnimation()
        {
            if (ButtomButtonsCoroutine != null) yield break;

            //double tap判断のため、適当に0.2秒を待ちます
            yield return new WaitForSeconds(0.2f);

            if (isCallButtomAnimation && !isDragging && !isClose) 
            {
                BottomButtonsAction?.Invoke();
            }
            ButtomButtonsCoroutine = null;
        }
        public void CallResetToOneAnimation() 
        {
            //Closeの場合、強制リセット
            if (AutoZoomCoroutine != null) 
            {
                StopCoroutine(AutoZoomCoroutine);
                AutoZoomCoroutine = null;
            }
            isClose = true;
            AutoZoomCoroutine = StartCoroutine(AutoZoomAnimation());
        }

#endregion Animation

#region TouchControl

        /// <summary>
        /// single & doubleクリックの処理
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isDragging || isClose || Input.touchCount > 1) return;

            Action<int> clickAction = (ClickCount) =>
            {
                if (ClickCount == 1)
                {
                    if (ButtomButtonsCoroutine != null) return;
                    isCallButtomAnimation = !isDragging;
                    ButtomButtonsCoroutine = StartCoroutine(CallButtonAnimation());
                }
                else if (ClickCount == 2)
                {
                    if (AutoZoomCoroutine != null) return;
                    isCallButtomAnimation = false;
                    AutoZoomCoroutine = StartCoroutine(AutoZoomAnimation());
                }
            };

#if UNITY_EDITOR
            clickAction(eventData.clickCount);
#else
            var touch = Input.touches.FirstOrDefault(data=> data.fingerId == eventData.pointerId);
            clickAction(touch.tapCount);
#endif
        }


        /// <summary>
        /// PointerEventData位置相対変化値の計算
        /// </summary>
        private Vector2 UpdateRelativePos(Vector2 averagePos)
        {
            if (lastInputCount != Input.touchCount)
            {
                lastInputCount = Input.touchCount;
                lastPos = averagePos;
                return currentPos;
            }
            float diffX = averagePos.x - lastPos.x;
            float diffY = averagePos.y - lastPos.y;
            currentPos.x += diffX;
            currentPos.y += diffY;
            lastInputCount = Input.touchCount;
            lastPos = averagePos;
            return currentPos;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (isClose) return;
            eventData.position = Input.mousePosition;
            currentPos = eventData.position;
            lastPos = eventData.position;
            base.OnBeginDrag(eventData);
            isDragging = true;
        }

        /// <summary>
        /// マルチタッチがあるので、基本PointerEventDataのposition値はInput.mousePositionを使います。
        /// </summary>
        public override void OnDrag(PointerEventData eventData)
        {
            if (isClose) return;
            eventData.position = UpdateRelativePos(Input.mousePosition);

            base.OnDrag(eventData);
            PinchControl();
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (isClose) return;
            eventData.position = UpdateRelativePos(Input.mousePosition);
#if UNITY_EDITOR
            base.OnEndDrag(eventData);
            isDragging = false;
#else
            if (Input.touches.All(data => data.phase == TouchPhase.Ended)) 
            {
                base.OnEndDrag(eventData);
                isDragging = false;
            }
#endif
            PinchControl();
        }
        private void PinchControl()
        {
            //Viewにタッチするの判断
            var touchesInView = Input.touches.Where(t => EventSystem.current.IsPointerOverGameObject(t.fingerId) && t.phase != TouchPhase.Ended).ToList();

            //タッチ開始時を感知し、初期化処理をする
            if (touchesInView.Count == 2 && !isPinch)
            {
                scale = ImageRect.sizeDelta.x / defaultSize.x;
                isPinch = true;
                float distance = Vector2.Distance(touchesInView[0].position, touchesInView[1].position);
                pinchScale = scale;
                pinchStartDistance = distance;
            }
            else if (isPinch) //タッチ中の処理
            {
                //タッチ終了を感知し、終了処理をする
                if (touchesInView.Count < 2)
                {
                    isPinch = false;
                    scale = ImageRect.sizeDelta.x / defaultSize.x;
                    return;
                }
                float distance = Vector2.Distance(touchesInView[0].position, touchesInView[1].position);
                scale = CheckScaleRange((distance / pinchStartDistance) * pinchScale);
                UpdateScaling(scale);
            }
           
        }
#endregion TouchControl
    }
}

