using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Pjfb.Runtime.Scripts.Timeline;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace Pjfb.Utility
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TrainingDirectorBindingUtility : TagHelperUtility, ITrackTimeDilation
    {
        [Serializable]
        public class BindingData
        {
            [Serializable]
            public class AnimationClipOverrideData
            {
                public string Name;
                public string ClipTag;
                public AnimationClip ClipOverwrite;
            }
            [Serializable]
            public class TrackData
            {
                public Object target;
                public List<AnimationClipOverrideData> OverwriteClips;
                public int trackIDX;
            }
            public string groupID;
            public List<TrackData> targets;
            public bool IsBinded = false;
        }

        [Serializable]
        public class GlobalSpeedData
        {
            public AnimationCurve speedCurve;

            public async UniTask AnimateSpeedByIndex(long index, Action<float> speedChangeCB, int min = 0, int max = 500)
            {
                float idx = math.remap(min, max, 0, speedCurve.length-1, index);
                if (speedCurve.length <= (int)idx) return;
                var ptIdx = Mathf.Clamp(Mathf.FloorToInt(idx), 0, speedCurve.length -1);
                float minTime = speedCurve.keys[ptIdx].time;
                float maxTime = speedCurve.keys[Mathf.Clamp(ptIdx+1, 0, speedCurve.length -1)].time;
                var time = minTime;
                do
                {
                    var f = speedCurve.Evaluate(time);
                    speedChangeCB.Invoke(f);
                    if (time >= maxTime) break;
                    time += Time.smoothDeltaTime;
                    await UniTask.Yield();
                } while (true);
            }
        }

        [SerializeField] private GlobalSpeedData globalSpeedData;
        [SerializeField] private List<BindingData> bindingData;
        private List<BindingData> defaultData;
        private PlayableDirector attachedDirector;
        public float playSpeed = 1f;
        public float speedMultiplier = 1f;
        
        private static Regex _animPatterRegex = new Regex(@"(?<=_)\w+(?=_\w+)");
        [SerializeField] private List<TrackTimeDilationPlayableClip> dilationClips;

        protected override void Awake()
        {
            base.Awake();
            dilationClips.RemoveAll(x => x == null);
            attachedDirector = GetComponent<PlayableDirector>();
        }

        private void Update()
        {
            if (dilationClips.Count == 0)
            {
                if (attachedDirector != null && attachedDirector.playableGraph.IsValid())
                {
                    attachedDirector.playableGraph.GetRootPlayable(0).SetSpeed(playSpeed);
                }
            }
        }

        private void UpdateDilationTimes(float speed)
        {
            foreach (var dilationPlayableClip in dilationClips)
            {
                dilationPlayableClip.TimeReMapper = speed;
            }
        }

        public List<BindingData> BindingDataList => bindingData;
        public void SetPlaySpeed(float speed = 1f)
        {
            playSpeed = speed;
            UpdateDilationTimes(playSpeed);
        }

        public async void SetSpeedByBonusValue(long bonus)
        {
            await globalSpeedData.AnimateSpeedByIndex(bonus, speed =>
            {
                playSpeed = speed;
                UpdateDilationTimes(playSpeed);
            });
        }

        public void SetObject(string groupID, int idx, Object target = default)
        {
            var data = bindingData.Find(x => x.groupID.Equals(groupID));
            if (data != default && data.targets.Count > idx)
            {
                data.targets[idx].target = target;
            }
        }
        
        public BindingData.TrackData GetTrackData(string groupID, int idx)
        {
            var ret = default(BindingData.TrackData);
            var data = bindingData.Find(x => x.groupID.Equals(groupID));
            if (data != default && data.targets.Count > idx)
            {
                ret = data.targets[idx];
            }

            return ret;
        }

        public void SetAttachment(string groupID, Func<string, Transform> attachmentCB)
        {
            var data = bindingData.Find(x => x.groupID.Equals(groupID));
            foreach (var trackData in data.targets)
            {
                var go = trackData.target as GameObject;
                if (go != null)
                {
                    var positionConstraint = go.GetComponentsInChildren<AnimationPositionConstraintUtility>();
                    foreach (var utility in positionConstraint)
                    {
                        utility.SetAttachment(0, attachmentCB);
                    }
                }
            }

        }
        public void SetToBind(string groupID, bool isBind = false)
        {
            var data = bindingData.Find(x => x.groupID.Equals(groupID));
            if (data != default)
            {
                data.IsBinded = isBind;
            }
        }

        public void RebindPlayable()
        {
            attachedDirector.RebuildGraph();
            attachedDirector.RebindPlayableGraphOutputs();

        }

        private void OnValidate()
        {
            if (ValidateRebind()) return;
            RebindPlayable();
        }

        public bool ValidateRebind(bool forceReSetup = true)
        {
            if (bindingData == null || bindingData.Count == 0) return true;
            attachedDirector = GetComponent<PlayableDirector>();
            var trackList = attachedDirector.playableAsset.outputs.Select(x => x.sourceObject as TrackAsset).ToList();
            var animationTrackList = attachedDirector.playableAsset.outputs.Select(x => x.sourceObject as AnimationTrack).ToList();

            if (forceReSetup)
            {
                foreach (var data in bindingData)
                {
                    if (data.targets == null || data.targets.Count == 0) continue;
                    foreach (var dataTarget in data.targets)
                    {
                        if (dataTarget.target == default) continue;
                        trackList[dataTarget.trackIDX].muted = true;
                        (dataTarget.target as GameObject).SetActive(false);
                        if (dataTarget.OverwriteClips.Count > 0)
                        {
                            foreach (var animationClipOverrideData in dataTarget.OverwriteClips)
                            {
                                if (animationClipOverrideData.ClipOverwrite != null)
                                {
                                    animationClipOverrideData.ClipTag = _animPatterRegex.Match(animationClipOverrideData.ClipOverwrite.name).Value;                                
                                }
                            }
                        }
                    }
                }
            }


            foreach (var data in bindingData)
            {
                if (data.targets == null || data.targets.Count == 0) continue;
                if (data.IsBinded)
                {
                    foreach (var dataTarget in data.targets)
                    {
                        if (dataTarget.target == default) continue;
                        trackList[dataTarget.trackIDX].muted = false;
                        (dataTarget.target as GameObject).SetActive(true);
                        attachedDirector.SetGenericBinding(trackList[dataTarget.trackIDX], dataTarget.target);
                        if (dataTarget.OverwriteClips.Count > 0)
                        {
                            var timelineClips = animationTrackList[dataTarget.trackIDX].GetClips();
                            foreach (var animationClipOverrideData in dataTarget.OverwriteClips)
                            {
                                if (animationClipOverrideData.ClipOverwrite != null)
                                {
                                    var clip = timelineClips.FirstOrDefault(x =>
                                        x.displayName.Equals(animationClipOverrideData.Name))?.asset as AnimationPlayableAsset;
                                    if (clip != null)
                                    {
                                        clip.clip = animationClipOverrideData.ClipOverwrite;
                                    }
                                }
                            }
                        }                        
                    }
                }
                else
                {
                    foreach (var dataTarget in data.targets)
                    {
                        if (dataTarget.target == default) continue;
                        trackList[dataTarget.trackIDX].muted = true;
                        attachedDirector.SetGenericBinding(trackList[dataTarget.trackIDX], default);
                        (dataTarget.target as GameObject).SetActive(false);
                    }
                }
            }

            return false;
        }

        public void OnDilationTimeChange(float dilationTime)
        {
            speedMultiplier = dilationTime;
        }

        public void OnAddCurrentClip(TrackTimeDilationPlayableClip clip)
        {
            dilationClips.Add(clip);
        }

        public void OnRemoveCurrentClip(TrackTimeDilationPlayableClip clip)
        {
            dilationClips.Remove(clip);
        }

        protected override void OnDestroy()
        {
            dilationClips.Clear();
        }

        private void OnEnable()
        {
            if (attachedDirector == null) return;
            foreach (var clip in dilationClips)
            {
                clip.TimeReMapper = 1f;
            }
        }

        private void OnDisable()
        {
            if (attachedDirector == null) return;
        }
    }
}
