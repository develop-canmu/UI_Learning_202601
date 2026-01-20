using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.RecommendChara
{
    public class MeBadge : MonoBehaviour
    {
        [SerializeField] private List<GameObject> targets;
        [SerializeField] private List<GameObject> hideTargets;
        
        public void SwitchBadge(bool isMe)
        {
            targets.ForEach(item => item.SetActive(isMe));
            hideTargets.ForEach(item => item.SetActive(!isMe));
        }
    }
}
