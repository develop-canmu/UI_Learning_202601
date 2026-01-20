using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using CruFramework.H2MD;
using UnityEngine.UI;

namespace CruFramework.Timeline
{
    public abstract class H2MDPlayableBehaviour : PlayableBehaviour
    {
        internal H2MDPlayableClip clip = null;

        private H2MDMovie h2mdMovie = null;
        private Texture2D movieTexture = null;

        private int movieFrame = -1;
        
        /// <summary>H2MDアセットが読み込まれた</summary>
        protected abstract void OnLoadAsset(Texture2D texture);

        /// <summary>再生</summary>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            // アセットが設定されてない
            if (clip.H2MDAsset == null) return;

            h2mdMovie = new H2MDMovie();
            // ファイルを開く
            movieTexture = h2mdMovie.OpenMem(clip.H2MDAsset.Bytes);
            // 読み込み通知
            OnLoadAsset(movieTexture);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            // h2md破棄
            if (h2mdMovie != null)
            {
                h2mdMovie.Dispose();
                h2mdMovie = null;
            }
            // テクスチャ破棄
            if (movieTexture != null)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(movieTexture);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(movieTexture);
                }
                movieTexture = null;
            }
        }

        /// <summary>Update的なの</summary>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // アセットが設定されてない
            if (h2mdMovie == null) return;
            
            // フレーム数
            int currentFrame = (int)(playable.GetTime() * h2mdMovie.GetFrameRate());
            // デコード
            Decode(currentFrame);
        }

        private void Decode(int frame)
        {
            // 前回と同じフレーム
            if (movieFrame == frame) return;
            // フレーム位置更新
            movieFrame = frame;
            // デコード
            h2mdMovie.Decode(movieFrame);

            if (clip.IsFlip)
            {
                h2mdMovie.GetImageFlip();
            }
            else
            {
                h2mdMovie.GetImage();
            }
        }
    }
}
