using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Animation
{
    #region Params
    public enum AnimationType
    {
        None,
        Move,
        Rotate,
        Scale,
        Blink,
    }
    public enum TargetType
    {
        Unset,
        CanvasGroup,
        Image,
        RectTransform,
        Renderer, SpriteRenderer,
        Rigidbody, Rigidbody2D,
        Text,
        Transform,
        TextMeshPro,
        TextMeshProUGUI
    }
    public enum RotateDirection
    {
        Clockwise,
        Counterclockwise
    }
    #endregion

    public class CommonAnimation : MonoBehaviour
    {
        #region Fields
        //Common Value
        public Component Target;
        public TargetType TargetType;
        public AnimationType AnimationType;
        public RotateDirection RotateDirection;
        public LoopType LoopType = LoopType.Yoyo;
        public Ease EaseType = Ease.Linear;
        public AnimationCurve EaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public float RotateSpeed = 1;
        public float Delay = 0;
        public float Duration = 1;
        public int Loops = 1;
        public bool OptionalBool0 = false;
        public bool IsValid = false;
        public bool AutoPlay = true;
        public bool EnableStartValue = false;

        //Start Values
        public float StartValueFloat = 0;
        public Vector3 StartValueV3 = Vector3.zero;
        public Color StartValueColor = Color.white;

        //End Values
        public float EndValueFloat = 0;
        public Vector3 EndValueV3 = Vector3.zero;
        public Color EndValueColor = Color.white;

        //Origin Values
        public AnimationType OriginType;
        public float OriginValueFloat = 0;
        public Vector3 OriginValueV3 = Vector3.zero;
        public Color OriginValueColor = Color.white;
        public Quaternion OriginQuaternion = Quaternion.identity;

        //Tween
        private Tween tween;

        private float rotateDiff = 10;

        #endregion

        #region Life Cycle

        private void Awake()
        {
            SaveOriginValue();
        }

        private void Start()
        {
            CreateTween(false, AutoPlay);
        }

        private void OnEnable()
        {
            CreateTween(true, AutoPlay,normalized:GetNormalizedTime());
        }

        private void OnDisable()
        {
            StopAnimation();
        }

        private void OnDestroy()
        {
            if (tween != null && tween.active) tween.Kill();
            tween = null;
        }

        #endregion

        #region Public Methods
        public static TargetType TypeToDOTargetType(Type t)
        {
            string str = t.ToString();
            int dotIndex = str.LastIndexOf(".");
            if (dotIndex != -1) str = str.Substring(dotIndex + 1);
            if (str.IndexOf("Renderer") != -1 && (str != "SpriteRenderer")) str = "Renderer";
            if (str == "RawImage" || str == "Graphic") str = "Image";
            return (TargetType)Enum.Parse(typeof(TargetType), str);
        }
        public void PlayAnimation(float normalized = 0) 
        {
#if UNITY_EDITOR
            if (tween != null && tween.active) tween.Rewind();
#endif
            CreateTween(true,true,normalized);
            
        }
        public void StopAnimation()
        {
            if (tween != null && tween.active)
            {
                tween.Kill();
#if UNITY_EDITOR
               ResetOriginValue();
#endif
            }
            tween = null;
        }
        public void CreateTween(bool regenerateIfExists = false,bool andPlay = true,float normalized = 0)
        {
            ResetOriginValue();
            if (tween != null)
            {
                if (tween.active)
                {
                    if (regenerateIfExists) tween.Kill();
                    else return;
                }
                tween = null;
            }

            //Tweenの作成
            if (EnableStartValue) TweenWithStartValue();
            else TweenWithoutStartValue();

            if (tween == null) return;

            tween.SetTarget(gameObject)
                 .SetLink(gameObject)
                 .SetLoops(Loops, LoopType)
                 .SetAutoKill(true)
                 .OnKill(() => tween = null);
            if (EaseType == Ease.INTERNAL_Custom) tween.SetEase(EaseCurve);
            else tween.SetEase(EaseType);
            
            if (Delay > 0)
            {
                tween.Pause();
                DOVirtual.DelayedCall(Delay, () => GoToNormalizedTime(normalized, andPlay)).SetLink(gameObject);
            }
            else
            {
                GoToNormalizedTime(normalized,andPlay);
            }
        }
        public void ResetStartEndValues()
        {
            EnableStartValue = false;
            EaseCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            RotateSpeed = 1;
            Delay = 0;
            Duration = 1;
            Loops = -1;
            LoopType = LoopType.Yoyo;
            OptionalBool0 = false;
            switch (AnimationType)
            {
                case AnimationType.Move:
                case AnimationType.Rotate:
                case AnimationType.Scale:
                    StartValueV3 = Vector3.zero;
                    StartValueFloat = 0;
                    EndValueV3 = Vector3.zero;
                    EndValueFloat = 0;
                    OptionalBool0 = AnimationType == AnimationType.Scale;
                    break;
                case AnimationType.Blink:
                    StartValueFloat = 1;
                    EndValueFloat = 0;
                    StartValueColor = Color.white;
                    EndValueColor = Color.white;
                    break;
            }

        }
        public void SaveOriginValue() 
        {
            if (Target == null || AnimationType == AnimationType.None) return;
            OriginType = AnimationType;
            switch (AnimationType)
            {
                case AnimationType.None:
                    break;
                case AnimationType.Move:
                    OriginValueV3 = transform.localPosition;
                    break;
                case AnimationType.Rotate:
                    OriginQuaternion = transform.localRotation;
                    break;
                case AnimationType.Scale:
                    OriginValueV3 = transform.localScale;
                    break;
                case AnimationType.Blink:
                    switch (TargetType)
                    {
                        case TargetType.Renderer:
                            OriginValueColor = ((Renderer)Target).material.color;
                            OriginValueFloat = OriginValueColor.a;
                            break;
                        case TargetType.SpriteRenderer:
                            OriginValueColor = ((SpriteRenderer)Target).color;
                            OriginValueFloat = OriginValueColor.a;
                            break;
                        case TargetType.Image:
                            OriginValueColor = ((Graphic)Target).color;
                            OriginValueFloat = OriginValueColor.a;
                            break;
                        case TargetType.Text:
                            OriginValueColor = ((Text)Target).color;
                            OriginValueFloat = OriginValueColor.a;
                            break;
                        case TargetType.CanvasGroup:
                            OriginValueFloat = ((CanvasGroup)Target).alpha;
                            break;
                        case TargetType.TextMeshProUGUI:
                            OriginValueColor = ((TextMeshProUGUI)Target).color;
                            OriginValueFloat = OriginValueColor.a;
                            break;
                        case TargetType.TextMeshPro:
                            OriginValueColor = ((TextMeshPro)Target).color;
                            OriginValueFloat = OriginValueColor.a;
                            break;
                    }
                    break;

            }
        }
        public void ResetOriginValue(bool isForced = false) 
        {
            if (AnimationType != OriginType && !isForced) return;

            switch (AnimationType)
            {
                case AnimationType.None:
                    break;
                case AnimationType.Move:
                    transform.localPosition = OriginValueV3;
                    break;
                case AnimationType.Rotate:
                    transform.localRotation = OriginQuaternion;
                    break;
                case AnimationType.Scale:
                    transform.localScale = OriginValueV3;
                    break;
                case AnimationType.Blink:
                    switch (TargetType)
                    {
                        case TargetType.Renderer:
                            ((Renderer)Target).material.color = OriginValueColor;
                            break;
                        case TargetType.SpriteRenderer:
                            ((SpriteRenderer)Target).color = OriginValueColor;
                            break;
                        case TargetType.Image:
                            ((Graphic)Target).color = OriginValueColor;
                            break;
                        case TargetType.Text:
                            ((Text)Target).color = OriginValueColor;
                            break;
                        case TargetType.CanvasGroup:
                            ((CanvasGroup)Target).alpha = OriginValueFloat;
                            break;
                        case TargetType.TextMeshProUGUI:
                           ((TextMeshProUGUI)Target).color = OriginValueColor;
                            break;
                        case TargetType.TextMeshPro:
                            ((TextMeshPro)Target).color = OriginValueColor;
                            break;
                    }
                    break;

            }
        }
        
        public float GetNormalizedTime()
        { 
            return tween == null ? 0 : (tween.hasLoops && IsYoyoBackwards()) ? (tween.position + tween.Duration(false))/tween.Duration(false) : tween.position/tween.Duration(false);
        }
        #endregion

        #region Protected and Private Methods
        
        private void TweenWithStartValue() 
        {
            switch (AnimationType)
            {
                case AnimationType.None:
                    break;
                case AnimationType.Move:
                    if (Target is RectTransform)
                    {
                        var rect = (RectTransform)Target;
                        var startAnchorPos = rect.anchoredPosition + (Vector2)StartValueV3;
                        var endAnchorPos = rect.anchoredPosition + (Vector2)EndValueV3;
                        tween = DOTween.To(() => startAnchorPos, pos => rect.anchoredPosition = pos, endAnchorPos, Duration);
                    }
                    else
                    {
                        tween = DOTween.To(() => StartValueV3, pos => transform.localPosition = pos, EndValueV3, Duration);
                    }
                    break;
                case AnimationType.Rotate:
                    var startValue = transform.localRotation;
                    var diff = RotateDirection switch
                    {
                        RotateDirection.Clockwise => -rotateDiff * RotateSpeed,
                        RotateDirection.Counterclockwise => rotateDiff * RotateSpeed,
                        _ => 0,
                    };
                    var endValue = startValue.eulerAngles + new Vector3(0, 0, diff);
                    tween = gameObject.transform.DOLocalRotate(endValue, Duration, RotateMode.FastBeyond360);
                    break;
                case AnimationType.Scale:
                    var startScale = OptionalBool0 ? new Vector3(StartValueFloat, StartValueFloat, StartValueFloat) : StartValueV3;
                    var endScale = OptionalBool0 ? new Vector3(EndValueFloat, EndValueFloat, EndValueFloat) : EndValueV3;
                    tween = DOTween.To(() => startScale, scale => transform.localScale = scale, endScale, Duration);
                    break;
                case AnimationType.Blink:
                    var startColor = StartValueColor;
                    startColor.a = StartValueFloat;
                    var endColor = EndValueColor;
                    endColor.a = EndValueFloat;
                    switch (TargetType)
                    {
                        case TargetType.Renderer:
                            var rendererTarget = (Renderer)Target;
                            tween = DOTween.To(() => startColor, color => rendererTarget.material.color = color, endColor, Duration);
                            break;
                        case TargetType.SpriteRenderer:
                            var spriteTarget = (SpriteRenderer)Target;
                            tween = DOTween.To(() => startColor, color => spriteTarget.color = color, endColor, Duration);
                            break;
                        case TargetType.Image:
                            var imageTarget = (Graphic)Target;
                            tween = DOTween.To(() => startColor, color => imageTarget.color = color, endColor, Duration);
                            break;
                        case TargetType.Text:
                            var textTarget = (Text)Target;
                            tween = DOTween.To(() => startColor, color => textTarget.color = color, endColor, Duration);
                            break;
                        case TargetType.CanvasGroup:
                            var canvasGroupTarget = (CanvasGroup)Target;
                            tween = DOTween.To(() => StartValueFloat, alpha => canvasGroupTarget.alpha = alpha, EndValueFloat, Duration);
                            break;
                        case TargetType.TextMeshProUGUI:
                            var textMeshProUGUITarget = (TextMeshProUGUI)Target;
                            tween = DOTween.To(() => startColor, color => textMeshProUGUITarget.color = color, endColor, Duration);
                            break;
                        case TargetType.TextMeshPro:
                            var textMeshProTarget = (TextMeshPro)Target;
                            tween = DOTween.To(() => startColor, color => textMeshProTarget.color = color, endColor, Duration);
                            break;
                    }
                    break;

            }
        }
        private void TweenWithoutStartValue() 
        {
            switch (AnimationType)
            {
                case AnimationType.None:
                    break;
                case AnimationType.Move:
                    tween = (Target is RectTransform) 
                        ? ((RectTransform)Target).DOAnchorPos(EndValueV3, Duration, OptionalBool0).SetRelative()
                        : transform.DOLocalMove(EndValueV3, Duration, OptionalBool0);
                    break;
                case AnimationType.Rotate:
                    var startValue = transform.localRotation;
                    var diff = RotateDirection switch
                    {
                        RotateDirection.Clockwise => -10 * RotateSpeed,
                        RotateDirection.Counterclockwise => 10 * RotateSpeed,
                        _ => 0,
                    };
                    var endValue = startValue.eulerAngles + new Vector3(0, 0, diff);
                    tween = gameObject.transform.DOLocalRotate(endValue, Duration, RotateMode.FastBeyond360);
                    break;
                case AnimationType.Scale:
                    tween = gameObject.transform.DOScale(OptionalBool0 ? new Vector3(EndValueFloat, EndValueFloat, EndValueFloat) : EndValueV3, Duration);
                    break;
                case AnimationType.Blink:
                    var endColor = EndValueColor;
                    endColor.a = EndValueFloat;
                    switch (TargetType)
                    {
                        case TargetType.Renderer:
                            tween = ((Renderer)Target).material.DOColor(endColor,Duration);
                            break;
                        case TargetType.SpriteRenderer:
                            tween = ((SpriteRenderer)Target).DOColor(endColor, Duration);
                            break;
                        case TargetType.Image:
                            tween = ((Graphic)Target).DOColor(endColor, Duration);
                            break;
                        case TargetType.Text:
                            tween = ((Text)Target).DOColor(endColor, Duration);
                            break;
                        case TargetType.CanvasGroup:
                            tween = ((CanvasGroup)Target).DOFade(EndValueFloat, Duration);
                            break;
                        case TargetType.TextMeshProUGUI:
                            tween = ((TextMeshProUGUI)Target).DOColor(endColor, Duration);
                            break;
                        case TargetType.TextMeshPro:
                            tween = ((TextMeshPro)Target).DOColor(endColor, Duration);
                            break;
                    }
                    break;

            }
        }

        private void GoToNormalizedTime(float normalized,bool andPlay)
        {
            if (tween == null) return;
            tween.Goto(Duration*normalized,andPlay);
        }

        private bool IsYoyoBackwards()
        {
            if (tween == null || !tween.hasLoops) return false;
            
            return !Mathf.Approximately(tween.ElapsedPercentage(false),tween.ElapsedDirectionalPercentage());
        }

        #endregion
    }

}



