using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CruFramework.Adv
{

    [System.Serializable]
    public class AdvCommandMoveObject : IAdvCommand, IAdvCommandSkip
    {
        
        [SerializeField]
        [AdvDocument("移動させるオブジェクト。")]
        private ulong objectId = 0;
        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.Positions))]
        [AdvDocument("移動させる場所。")]
        private int positionId = 0;
        
        [SerializeField]
        [AdvDocument("オフセット。")]
        private Vector3 offset = Vector3.zero;
        
        [SerializeField]
        [AdvDocument("移動にかかる時間。")]
        private float duration = 0.3f;
        
        [SerializeField]
        [AdvDocument("移動が終わるまで待機する。")]
        private bool isWait = true;
        
        private Tween tween = null;
        
        void IAdvCommandSkip.Skip(AdvManager manager)
        {
            if(tween != null)tween.Kill();
            AdvObject obj = manager.GetObject(objectId);
            if(obj == null)return;
            obj.transform.position = manager.GetWorldPosition(positionId, obj.transform.position, offset);
        }

        void IAdvCommand.Execute(AdvManager manager)
        {
            // スキップ中は処理しない               
            if(manager.IsSkipOrFastMode)return;
            
            AdvObject obj = manager.GetObject(objectId);
            if(obj == null)return;
            Vector3 targetPos = manager.GetWorldPosition(positionId, obj.transform.position, offset);
            
            if(duration > 0)
            {
                tween = DOTween.To(()=>obj.transform.position, (v)=>obj.transform.position = v, targetPos, duration);
                
                if(isWait)
                {
                    manager.SetWaitTime(duration);
                }
            }
            else
            {
                obj.transform.position = targetPos;
            }
        }
    }
}