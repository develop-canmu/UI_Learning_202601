using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Pjfb.EventRanking {

    [RequireComponent(typeof(ScrollRect))]
    public class NestedScrollEventThrower : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {
        [SerializeField]
        ScrollRect _scrollRect = null;

        bool _routeToParent = false;
        
        // Handlers
        List<IInitializePotentialDragHandler> _initializePotentialDragHandlers = new List<IInitializePotentialDragHandler>();
        List<IDragHandler> _dragHandlers = new List<IDragHandler>();
        List<IBeginDragHandler> _beginDragHandler = new List<IBeginDragHandler>();
        List<IEndDragHandler> _endDragHandler = new List<IEndDragHandler>();

        public void Start(){
            //最初にターゲットになる親のEventHandlerをキャッシュしておく
            FindAndSetEventThrowParents();
        }

        /// <summary>
        /// イベントを投げる親の検索と設定
        /// </summary>
        void FindAndSetEventThrowParents(){
            _initializePotentialDragHandlers = FindParentEventSystemHandler<IInitializePotentialDragHandler>();
            _dragHandlers = FindParentEventSystemHandler<IDragHandler>();
            _beginDragHandler = FindParentEventSystemHandler<IBeginDragHandler>();
            _endDragHandler = FindParentEventSystemHandler<IEndDragHandler>();
        }
	
        public void OnInitializePotentialDrag (PointerEventData eventData) {
            DoEventSystemHandlers<IInitializePotentialDragHandler>( _initializePotentialDragHandlers, (parent) => { parent.OnInitializePotentialDrag(eventData); });
        }
        
        public void OnDrag (UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(_routeToParent) {
                DoEventSystemHandlers<IDragHandler>( _dragHandlers, (parent) => { parent.OnDrag(eventData); });
            }
        }
        
        public void OnBeginDrag (UnityEngine.EventSystems.PointerEventData eventData)
        {
            if( !_scrollRect.horizontal && Math.Abs (eventData.delta.x) > Math.Abs (eventData.delta.y) ) {
                _routeToParent = true;
            } else if ( !_scrollRect.vertical && Math.Abs (eventData.delta.x) < Math.Abs (eventData.delta.y) ){
                _routeToParent = true;
            } else {
                _routeToParent = false;
            }

            if(_routeToParent) {
                DoEventSystemHandlers<IBeginDragHandler>( _beginDragHandler, (parent) => { parent.OnBeginDrag(eventData); });
            } 
        }
        
        public void OnEndDrag (UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(_routeToParent){
                DoEventSystemHandlers<IEndDragHandler>( _endDragHandler, (parent) => { parent.OnEndDrag(eventData); });
            }
            _routeToParent = false;
        }

        /// <summary>
        /// 指定してEventHandlerを検索してリストで返す
        /// </summary>
        List<T> FindParentEventSystemHandler<T>() where T : class, IEventSystemHandler
        {
            var parent = transform.parent;
            var parentHandlers = new List<T>();
            while(parent != null) {
                foreach(var component in parent.GetComponents<Component>()) {
                    if(component is T) {
                        parentHandlers.Add( component as T );
                    }
                }
                parent = parent.parent;
            }
            return parentHandlers;
        }
        
        /// <summary>
        /// 渡されたEventHandlerListに対してActionを実行する
        /// </summary>
        void DoEventSystemHandlers<T>( List<T> handlers , Action<T> action) where T: class, IEventSystemHandler
        {
            for( int i=0; i<handlers.Count; ++i ) {
                var handler = handlers[i];
                if( handler == null ) {
                    continue;
                }
                action( handler as T );
            }
        }

        
        
    
    }



 
}