using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(PositionConstraint))]
public class AnimationPositionConstraintUtility : MonoBehaviour
{
    [SerializeField] private PositionConstraint attachedConstraint;
    [SerializeField] private List<string> targetAttachementName;
    public void SetAttachment(int idx, Func<string, Transform> setCallback)
    {
        if (attachedConstraint == null) return;
        attachedConstraint.SetSource(idx, new ConstraintSource()
        {
            sourceTransform = setCallback.Invoke(targetAttachementName[idx]),
            weight = 1f
        });
    }
}
