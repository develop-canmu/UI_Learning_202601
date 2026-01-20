using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandCamera : IAdvCommand, IAdvCommandSkip
    {
        
        public enum PositionType
        {
            Local = 1,
            Anchored = 2
        }
        
        [SerializeField]
        [AdvDocument("対象の座標。")]
        private Vector2 position = Vector2.zero;
        
        [SerializeField]
        private PositionType positionType = PositionType.Local;

        [SerializeField]
        [AdvDocument("ズーム設定。1が初期値。")]
        private float zoom = 1.0f;
        
        [SerializeField]
        [AdvDocument("操作が完了するまでの時間。")]
        private float duration = 0.5f;
        
        [SerializeField]
        [AdvDocument("操作が完了するまで待機する。")]
        private bool isWait = true;
        
        private Sequence sequence = null;
        
        
        void IAdvCommandSkip.Skip(AdvManager manager)
        {
            if(sequence != null)
            {
                sequence.Complete();
            }
        }

        void IAdvCommand.Execute(AdvManager manager)
        {
            // 操作する対象を取得
            RectTransform target = (RectTransform)manager.World.transform;
            
            // 即時反映
            if(duration <= 0)
            {
                switch(positionType)
                {
                    case PositionType.Local:
                        target.localPosition = position;
                        break;
                    case PositionType.Anchored:
                        target.anchoredPosition = position;
                        break;
                }
                
                target.localScale = new Vector3(zoom, zoom, 1.0f);
                return;
            }
            
            // 待機する
            if(isWait)
            {
                manager.IsStopCommand = true;
            }
            
            // シーケンスの初期化
            if(sequence != null)
            {
                sequence.Complete();
            }
            sequence = DOTween.Sequence();
            
            switch(positionType)
            {
                case PositionType.Local:
                    sequence.Append(target.DOLocalMove(-position * zoom, duration)).Join( target.DOScale(new Vector3(zoom, zoom, 1.0f), duration));
                    break;
                case PositionType.Anchored:
                    sequence.Append(target.DOAnchorPos(-position * zoom, duration)).Join( target.DOScale(new Vector3(zoom, zoom, 1.0f), duration));
                    break;
            }
            
            if(isWait)
            {
                sequence.onComplete += ()=>manager.IsStopCommand = false;
            }
            sequence.Play();
        }
    }
}
