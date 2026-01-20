using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework.Audio;
using CruFramework.Adv;
using Spine;
using Spine.Unity;

namespace Pjfb.Adv
{
    public class AdvSpineCharacter : AdvSpineSkeletonGraphicObject, IAdvSpeaker
    {
        [SerializeField]
        private float lipSyncInterval = 0.1f;

        // 口パク
        private bool isLipSync = false;
        private float lipSyncTimer = 0;
        private bool closeMouth = false;
        private TrackEntry mouthTrack = null;
        
        // 目ぱち
        private bool isEyeBlink = false;
        private AdvSpineAnimationNameId eyeBlinkAnimation = null;
        private float eyeBlinkTimer = 0;
        
        private AudioPlayer audioPlayer = null;
        private float[] voiceDatas = null;

        public override void LoadSkeletonData(AdvManager manager, SkeletonDataAsset skeletonDataAsset)
        {
            base.LoadSkeletonData(manager, skeletonDataAsset);
            // 表情
            AdvSpineFaceId face = manager.Config.SpineFaces[manager.Config.DefaultSpineFaceId];
            SetFace(manager, face);
        }

        public void SetFace(AdvManager manager, AdvSpineFaceId face)
        {
            // アニメーションIdの取得
            AdvSpineAnimationNameId bodyAnimation = manager.Config.SpineAnimationNames[face.BodyAnimationId];
            AdvSpineAnimationNameId faceAnimation = manager.Config.SpineAnimationNames[face.FaceAnimationId];
            AdvSpineAnimationNameId eyeAnimation = manager.Config.SpineAnimationNames[face.EyeAnimationId];
            eyeBlinkAnimation = manager.Config.SpineAnimationNames[face.EyeBlinkAnimationId];
            AdvSpineAnimationNameId mouthAnimation = manager.Config.SpineAnimationNames[face.MouthAnimationId];
            
            // 体のアニメーション再生
            SpineUtility.PlayAnimation(SkeletonGraphic, bodyAnimation.Layer, bodyAnimation.Name, true);
            // 顔のアニメーション再生
            SpineUtility.PlayAnimation(SkeletonGraphic, faceAnimation.Layer, faceAnimation.Name, true);
            // 目のアニメーション再生
            SpineUtility.PlayAnimation(SkeletonGraphic, eyeAnimation.Layer, eyeAnimation.Name, true);
            // 口のアニメーション
            mouthTrack = SpineUtility.PlayAnimation(SkeletonGraphic, mouthAnimation.Layer, mouthAnimation.Name, false);

            // 口パクしない表情
            isLipSync = face.IsLipSync;
            // アニメーション時間で制御するのでスケールを0にする
            mouthTrack.TimeScale = 0;
            
            // 目ぱち
            isEyeBlink = face.IsEyeBlink;
            eyeBlinkTimer = SpineUtility.GetEyeBlinkInterval();
        }

        void IAdvSpeaker.OnStopVoice()
        {
            if(mouthTrack != null)
            {
                mouthTrack.TrackTime = 0;
            }
            audioPlayer = null;
            voiceDatas = null;
        }
        
        void IAdvSpeaker.OnPlayVoice(AudioPlayer audioPlayer)
        {
            this.audioPlayer = audioPlayer;
            // Clipのデータ取得
            voiceDatas = audioPlayer.GetClipData();
            // タイマー初期化
            lipSyncTimer = 0;
        }

        private void Update()
        {
            UpdateMouthAnimation();
            UpdateEyeBlinkAnimation();
        }
        
        private void UpdateEyeBlinkAnimation()
        {
            if(!isEyeBlink) return;
            eyeBlinkTimer -= Time.deltaTime;
            if(eyeBlinkTimer <= 0)
            {
                SpineUtility.PlayEyeBlinkAnimation(SkeletonGraphic, eyeBlinkAnimation.Layer, eyeBlinkAnimation.Name);
                eyeBlinkTimer += SpineUtility.GetEyeBlinkInterval();
            }
        }
        
        private void UpdateMouthAnimation()
        {
            // リップシンクしない
            if(!isLipSync) return;
            // ボイスデータがない
            if(voiceDatas == null) return;
            // アニメーションが再生されてない
            if(mouthTrack == null) return;
            //再生が終わっている
            if(audioPlayer.AudioSource.isPlaying == false) return;
            
            // タイマー更新
            lipSyncTimer += Time.deltaTime;
            if(lipSyncTimer >= lipSyncInterval)
            {
                // タイマーリセット
                lipSyncTimer -= lipSyncInterval;
                
                // サンプル
                float voiceData = 0;
                // サンプリング位置
                int timeSamples = audioPlayer.AudioSource.timeSamples * audioPlayer.AudioSource.clip.channels;
                // サンプリング
                if(timeSamples < voiceDatas.Length-1)
                {
                    // 左右の音を足す
                    voiceData = Mathf.Abs(voiceDatas[timeSamples]);
                }
                
                // 音圧が一定以下
                if(voiceData <= 0.005f)
                {
                    mouthTrack.TrackTime = 0;
                    return;
                }

                // 閉じ口
                if(mouthTrack.TrackTime <= 0)
                {
                    closeMouth = false;
                    mouthTrack.TrackTime = voiceData > 0.5f ? mouthTrack.AnimationEnd : mouthTrack.AnimationEnd * 0.5f;
                }
                // 開き口
                else if(mouthTrack.TrackTime >= mouthTrack.AnimationEnd)
                {
                    closeMouth = true;
                    mouthTrack.TrackTime = voiceData < 0.1f ? 0 : mouthTrack.AnimationEnd * 0.5f;
                }
                // 中間
                else
                {
                    if(!closeMouth)
                    {
                        closeMouth = voiceData < 0.1f;
                    }
                    mouthTrack.TrackTime = closeMouth ? 0 : mouthTrack.AnimationEnd;
                }
            }
        }
    }
}
