using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CruFramework.UI
{
    [FrameworkDocument("UI", nameof(UIDrag), "ドラッグ&ドロップ拡張コンポーネント")]
    public class UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        
        private static List<UIDrag> draggingObjects = new List<UIDrag>();
        /// <summary>ドラッグしているオブジェクト</summary>
        public static IReadOnlyList<UIDrag> DraggingObjects{get{return draggingObjects;}}
        
        /// <summary>ドラッグ中</summary>
        public static bool IsDraggingObject{get{return draggingObjects.Count > 0;}}

        public enum DragTriggerType
        {
            None, Drag,
            //Hold
        }
        
        [SerializeField]
        private DragTriggerType triggerType = DragTriggerType.Drag;
        /// <summary>ドラッグ開始条件</summary>
        public DragTriggerType TriggerType{get{return triggerType;}set{triggerType = value;}}
        
        [SerializeField]
        private bool isCreateClone = true;
        /// <summary>クローンの生成</summary>
        public bool IsCraeteClone{get{return isCreateClone;}set{isCreateClone = value;}}
        
        [SerializeField]
        private GameObject cloneParent = null;
        /// <summary>クローン生成親</summary>
        public GameObject CloneParent{get{return cloneParent;}set{cloneParent = value;}}
        
        [SerializeField]
        private UnityEvent onBeginDrag = new UnityEvent();
        /// <summary>ドラッグ時</summary>
        public UnityEvent OnBeginDrag{get{return onBeginDrag;}}

        // [SerializeField]
        // private float holdTriggerTime = 0.2f;
        
        private object dragData = null;
        /// <summary>ドラッグデータ</summary>
        public object DragData{get{return dragData;}set{dragData = value;}}
        
        private bool isDragging = false;
        /// <summary>ドラッグ中</summary>
        public  bool IsDragging{get{return isDragging;}}
        
        private RectTransform UITransform{get{return (RectTransform)transform;}}
        
        // 生成したクローン
        private UIDrag cloneObject = null;
        
        // 指のId
        private int touchId = -1;
        // 指の位置
        private Vector2 beginDragPosition = Vector2.zero;
        // // 押下時間
        // private float pointerDownTime = 0; 
        // // 押下中
        // private bool isDown = false;
        
        void IBeginDragHandler.OnBeginDrag(PointerEventData e)
        {
            if(triggerType == DragTriggerType.Drag)
            {
                DoDrag(e.pointerId, e.position);
            }
        }
        
        void IEndDragHandler.OnEndDrag(PointerEventData e)
        {
            if(triggerType == DragTriggerType.Drag)
            {
                EndDrag();
            }
        }
        
        void IDragHandler.OnDrag(PointerEventData e)
        {
            if(triggerType == DragTriggerType.Drag)
            {
                if(cloneObject != null)
                {
                    Canvas canvas = gameObject.GetComponentInParent<Canvas>();
                    RectTransformUtility.ScreenPointToLocalPointInRectangle( (RectTransform)cloneObject.UITransform.parent, e.position, canvas.worldCamera, out Vector2 p);
                    cloneObject.UITransform.localPosition = p;
                }
            }
        }
        
        void IPointerDownHandler.OnPointerDown(PointerEventData e)
        {
            // pointerDownTime = 0;
            // isDown = true;
        }
        
        void IPointerUpHandler.OnPointerUp(PointerEventData e)
        {
            // pointerDownTime = 0;
            // isDown = false;
        }
        
        /// <summary>ドラッグの終了</summary>
        private void DoDrag(int touchId, Vector2 position)
        {
            this.touchId = touchId;
            beginDragPosition = position;
            DoDrag();
        }
        
        /// <summary>ドラッグの終了</summary>
        public void EndDrag()
        {
            // ドラッグしてない
            if(isDragging == false)return;
            // ドラッグ解除
            isDragging = false;
            // クローンを破棄
            if(cloneObject != null)
            {
                GameObject.Destroy(cloneObject.gameObject);
                cloneObject = null;
            }
            
            // リストから削除
            draggingObjects.Remove(this);
        }
                
                
        /// <summary>ドラッグ開始</summary>
        public void DoDrag()
        {
            // 既にドラッグ中
            if(isDragging)return;
            // ドラッグ中にする
            isDragging = true;
            
            // リストに追加
            draggingObjects.Add(this);
            
            // クローンの生成
            if(isCreateClone)
            {
                cloneObject = GameObject.Instantiate<UIDrag>(this, cloneParent == null ? transform.parent : cloneParent.transform, true);
                // クローンにレイキャスト判定をなくす
                Graphic[] graphics = cloneObject.gameObject.GetComponentsInChildren<Graphic>(true);
                foreach(Graphic graphic in graphics)
                {
                    graphic.raycastTarget = false;
                }
            }
            
            OnBeginDrag.Invoke();
        }
        
        
        // TODO 長押し実装
        // private void Update()
        // {
        //     if(triggerType == DragTriggerType.Hold && isDown)
        //     {
        //         pointerDownTime += Time.deltaTime;
        //         if(pointerDownTime >= holdTriggerTime)
        //         {
        //             
        //         }
        //     }
        // }
    }
}