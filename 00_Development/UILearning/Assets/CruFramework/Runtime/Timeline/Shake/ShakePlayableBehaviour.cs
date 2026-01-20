using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace CruFramework.Timeline
{
    public class ShakePlayableBehaviour : PlayableBehaviour
    {
        internal GameObject target = null;
        internal ShakePlayableClip clip { get; set; }
        
        private Vector3 initPosition = Vector3.zero;

        public override void OnGraphStart(Playable playable)
        {
            if (target == null) return;
            initPosition = target.transform.position;
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (target == null) return;
            target.transform.position = initPosition;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (target == null) return;
            Vector3 value = Vector3.zero;
            float time = (float)playable.GetTime();
            float duration = (float)playable.GetDuration();
            // パーリンノイズの位置
            float pos = Mathf.InverseLerp(0, duration, time) * 255;
            // 乱数取得
            value.x = clip.FreezeX ? 0 : GetPerlinNoiseValue(pos, 0);
            value.y = clip.FreezeY ? 0 : GetPerlinNoiseValue(0, pos);
            value.z = clip.FreezeZ ? 0 : GetPerlinNoiseValue(pos, pos);
            
            // 強さを適用
            value *= clip.Strength;

            // 時間経過で揺れの量を減衰させる
            float vibrato = clip.Vibrato;
            float ratio = 1.0f - time / duration;
            vibrato *= ratio;
            value.x = Mathf.Clamp(value.x, -vibrato, vibrato);
            value.y = Mathf.Clamp(value.y, -vibrato, vibrato);
            value.z = Mathf.Clamp(value.z, -vibrato, vibrato);
            // 座標更新
            target.transform.position = initPosition + value;
        }

        private float GetPerlinNoiseValue(float x, float y)
        {
            float perlinNoise = Mathf.PerlinNoise(x, y);
            // -1.0f~1.0fに変換
            return (perlinNoise - 0.5f) * 2.0f;
        }
    }
}
