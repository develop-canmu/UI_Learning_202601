using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using DG.Tweening;

namespace Pjfb
{
    public interface ISyncTargetDOTween
    {
        /// <summary> アニメーション </summary>
        public Tween Tween();
        
        /// <summary> アニメーションターゲット取得 </summary>
        public GameObject GetAnimationTarget();

        public DOTweenSyncParameter GetParameter();
        
        /// <summary> 同期時にパラメータを適用する処理 </summary>
        public void ApplySyncParameter(DOTweenSyncParameter parameter);
    }

    public abstract class DOTweenSyncParameter
    {
    }
    
    /// <summary> DOTweenアニメーション同期クラス </summary>
    public static class DOTweenSyncManager
    {
        private static Dictionary<string, List<ISyncTargetDOTween>> syncGroupList = new Dictionary<string, List<ISyncTargetDOTween>>();

        public static void ApplySync(ISyncTargetDOTween sync, string key)
        {
            // 引数チェック
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            
            // まだ同期させるアニメーションがないなら同期対象として登録する
            if (syncGroupList.TryGetValue(key, out List<ISyncTargetDOTween> syncList) == false)
            {
                syncList = new List<ISyncTargetDOTween>();
                syncGroupList.Add(key, syncList);
            }

            syncList.Add(sync);

            ISyncTargetDOTween target = null;

            foreach (ISyncTargetDOTween s in syncList)
            {
                if(s == sync) continue;
                // アニメーションがない
                if(s.Tween() == null) continue;
                // アニメーション稼働中でない
                if (s.Tween().active == false) continue;
                // 非アクティブ
                if (s.GetAnimationTarget().activeSelf == false) continue;

                target = s;
                break;
            }
            
            float position = 0f;

            if (target != null)
            {
                // 遅延処理を含んでの時間を取得
              position = target.Tween().fullPosition;
              sync.ApplySyncParameter(target.GetParameter());
            }
            
            // 同期位置にアニメーションをとばす
            sync.Tween().Goto(position, true);
        }

        /// <summary> 同期登録解除 </summary>
        public static void UnRegister(this ISyncTargetDOTween sync, string key)
        {
            if (syncGroupList.TryGetValue(key, out List<ISyncTargetDOTween> syncList) == false)
            {
                return;
            }

            syncList.Remove(sync);
        }

        public static void Clear(string key)
        {
            syncGroupList.Remove(key);
        }
    }
}