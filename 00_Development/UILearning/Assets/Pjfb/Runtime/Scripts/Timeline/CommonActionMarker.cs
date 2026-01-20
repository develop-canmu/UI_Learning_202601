using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pjfb
{
    /// <summary> 別イベントを共通で呼び出せるようにするマーカー </summary>
    public class CommonActionMarker : Marker, INotification
    {
        [SerializeField]
        private string actionName = string.Empty;
        public string ActionName => actionName;
        
        public PropertyName id => new PropertyName("ActionName");
    }
}