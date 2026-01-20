using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    [RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
    public class ContentsPerformanceTuner : MonoBehaviour
    {

        [Serializable]
        protected class childInfo
        {
            public ContentsPerformanceTuner target;
            public float originalAlpha;
        }

        [SerializeField] private List<childInfo> children;
        private ContentsPerformanceTuner root;
        private bool isRoot;
        private Rect _tRect = new Rect();
        private Rect _pRect = new Rect();
        private Vector3[] tCorners = new Vector3[4];
        public RectTransform AttachedRectTransform { get; private set; }
        public CanvasGroup AttachedCanvasGroup { get; private set; }

        private void Awake()
        {
            children = new List<childInfo>();
            AttachedRectTransform = GetComponent<RectTransform>();
            AttachedCanvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            var carr = GetComponentsInParent<ContentsPerformanceTuner>(true);
            root = carr[^1];
            if (root != null && root != this)
            {
                root.RegisterComponent(this);    
            }
        }

        private void RevertAlpha()
        {
            foreach (var child in children)
            {
                child.target.AttachedCanvasGroup.alpha = child.originalAlpha;
            }
        }

        private void OnDisable()
        {
            if (root != null)
            {
                UnRegisterComponent(this);
                if (isRoot)
                {
                    RevertAlpha();
                }
            }
        }

        private void Update()
        {
            //Do Stuff
            if (!isRoot) return;
            if (AttachedCanvasGroup.alpha == 0) return;
            Vector3[] pCorner = new Vector3[4];
            AttachedRectTransform.GetWorldCorners(pCorner);
            _pRect.xMin = pCorner[0].x;
            _pRect.yMin = pCorner[0].y;
            _pRect.xMax = pCorner[2].x;
            _pRect.yMax = pCorner[2].y;
            var trashBin = new List<ContentsPerformanceTuner.childInfo>();
            foreach (var tuner in children)
            {
                //Possible of removal outside main thread.
                if (tuner.target == null)
                {
                    trashBin.Add(tuner);
                    continue;
                };
                var isInside = IsRectTransformInsideParent(tuner.target.AttachedRectTransform);
                tuner.target.AttachedCanvasGroup.alpha = isInside ? tuner.originalAlpha : 0;
            }

            foreach (var trash in trashBin)
            {
                children.Remove(trash);
            }
            // **********
        }
    
        public bool IsRectTransformInsideParent(RectTransform target)
        {
            if (target == null) return false;
            target.GetWorldCorners(tCorners);
            _tRect.xMin = tCorners[0].x;
            _tRect.yMin = tCorners[0].y;
            _tRect.xMax = tCorners[2].x;
            _tRect.yMax = tCorners[2].y;

            return _pRect.Overlaps(_tRect);
        }

        public void RegisterComponent(ContentsPerformanceTuner target)
        {
            if (target == null) return;
            var t = children.Find(t => t.target.Equals(target));
            if (t == null)
            {
                children.Add(new childInfo(){target = target, originalAlpha = target.AttachedCanvasGroup.alpha});
            }
            isRoot = children.Count > 0;
        }
    
        public void UnRegisterComponent(ContentsPerformanceTuner target)
        {
            if (target == null) return;
            children.RemoveAll(t => t.target.Equals(target));
        }
    }
}
