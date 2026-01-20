using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Pjfb
{
    /// <summary> 共通マーカーからのイベント受け取りクラス </summary>
    public class CommonActionMarkerReceiver : MonoBehaviour, INotificationReceiver
    {
        [Serializable]
        private class EventData
        {
            // イベント名
            public string Name = string.Empty;
            public UnityEvent Action = null;
        }
        
        [SerializeField]
        private EventData[] actionList = null;
        
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            CommonActionMarker marker = notification as CommonActionMarker;
            if (marker == null)
            {
                return;
            }
            
            // 指定イベントを実行
            ExecuteAction(marker.ActionName);
        }
        
        /// <summary> イベント名から一致するイベントを発火 </summary>
        private void ExecuteAction(string actionName)
        {
            foreach (EventData data in actionList)
            {
                if (data.Name == actionName)
                {
                    data.Action.Invoke();
                    return;
                }
            }
            
            CruFramework.Logger.LogError($"Not found action {actionName}");
        }
    }
}