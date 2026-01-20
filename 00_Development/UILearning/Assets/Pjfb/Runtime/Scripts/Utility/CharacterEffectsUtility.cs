using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.VFX;

namespace Pjfb.Utility
{
    public class CharacterEffectsUtility : TagHelperUtility
    {
        [Serializable]
        public class VFXProperty
        {
            public string targetProperty;
            public VisualEffect targetVFX;
        }

        [Serializable]
        public class VFXAttachmentData
        {
            public string name;
            public Transform target;
        }
        
        [Serializable]
        public class VFXAnimationGroup
        {
            public string groupID;
            public AnimationCurve animationCurve;
            public List<VFXProperty> targetFXList;
            private CancellationTokenSource tokenSource;

            public void Animate(float speed = 1f)
            {
                if (tokenSource != null)
                {
                    tokenSource.Cancel();
                }
                tokenSource = new CancellationTokenSource();
                PlayMe(speed, tokenSource.Token);
            }
            
            private async void PlayMe(float speed = 1f, CancellationToken token = default)
            {
                float time = 0f;
                float clipLength = animationCurve.keys[animationCurve.length - 1].time;
                while (!token.IsCancellationRequested)
                {
                    time += Time.smoothDeltaTime * speed;
                    SetTime(time);
                    await UniTask.DelayFrame(1, PlayerLoopTiming.Update, token).SuppressCancellationThrow();
                    if (time >= clipLength)
                    {
                        break;
                    }
                }
            }

            public void SetTime(float time)
            {
                var value = animationCurve.Evaluate(time);
                foreach (var vfxProperty in targetFXList)
                {
                    if (vfxProperty.targetVFX == default) continue;
                    vfxProperty.targetVFX.SetFloat(vfxProperty.targetProperty, value);
                }
            }
        }
        [SerializeField] private List<VFXAnimationGroup> targetVFXGroups;
        
        [SerializeField] private List<VFXAttachmentData> vfxAttachmentList;

        public static List<CharacterEffectsUtility> GetFXListFromIDs(long[] ids)
        {
            var idList = ids.Select(l => l.ToString().Substring(0, 5)).ToArray();
            var helpers = GetHelpersInGroup(GroupEnum.VFX);
            helpers.RemoveAll(x => idList.Contains(x.id));
            return helpers.Select(x => (CharacterEffectsUtility)x).ToList();
        }

        public Transform GetAttachmentByName(string name)
        {
            var vfxatc = vfxAttachmentList.Find(x => x.name.Equals(name));
            if (vfxatc != null)
            {
                return vfxatc.target;
            }

            return default;
        }
        public void Animate(string groupID, float speed)
        {
            var group = targetVFXGroups.Find(g => g.groupID.Equals(groupID));
            if (group != null)
            {
                group.Animate(speed);
            }
        }

        public void SetTime(string groupID, float time)
        {
            var group = targetVFXGroups.Find(g => g.groupID.Equals(groupID));
            if (group != null)
            {
                group.SetTime(time);
            }
        }

        private void OnEnable()
        {
            foreach (var vfxGroup in targetVFXGroups)
            {
                if (vfxGroup == default) continue;
                vfxGroup.SetTime(0f);
            }
        }
    }
}