using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace Pjfb
{
    public class UIBadgeBalloon : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI balloonText = null;
        public TextMeshProUGUI BalloonText => balloonText;
        [SerializeField]
        private TextMeshProUGUI subBalloonText = null;
        public TextMeshProUGUI SubBalloonText => subBalloonText;
        
        [SerializeField]
        private int minWidth = 168;
        
        [SerializeField]
        private int marginWidth = 0;
        
        [SerializeField]
        private string defaultBalloonString;
        
        private RectTransform _rectTransform = null;
        /// <summary>RectTransform</summary>
        public RectTransform rectTransform
        {
            get
            {
                if(_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }
                return _rectTransform;
            }
        }

        Sequence crossFadeSequence = null;

        
        public void SetText(string text)
        {
            // テキストセット
            balloonText.text = text;
            if (subBalloonText != null)
            {
                subBalloonText.gameObject.SetActive(false);
            }
            StopCrossFade();
            // リサイズ
            LayoutRebuilder.ForceRebuildLayoutImmediate(balloonText.rectTransform);
            // サイズ変更
            Vector2 size = rectTransform.sizeDelta;
            size.x = Math.Max(balloonText.rectTransform.rect.width + marginWidth, minWidth);
            rectTransform.sizeDelta = size;
            
        }
        
        public void SetTextToCrossFade(string text, string subText)
        {
            // テキストセット
            balloonText.text = text;
            subBalloonText.gameObject.SetActive(true);
            subBalloonText.text = subText;
            
            // リサイズ
            LayoutRebuilder.ForceRebuildLayoutImmediate(balloonText.rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(subBalloonText.rectTransform);
            
            // サイズ変更
            var resizeText = balloonText.rectTransform.rect.width > subBalloonText.rectTransform.rect.width ? balloonText : subBalloonText;
            Vector2 size = rectTransform.sizeDelta;
            size.x = Math.Max(resizeText.rectTransform.rect.width + marginWidth, minWidth);
            rectTransform.sizeDelta = size;

            //Fadeアニメーション
            StopCrossFade();
            var fadeTime = 0.5f;
            var intervalTime = 1.5f;

            subBalloonText.gameObject.SetActive(true);

            var color = balloonText.color;
            color.a = 1.0f;
            balloonText.color = color;
            var subBalloonColor = subBalloonText.color;
            subBalloonColor.a = 0.0f;
            subBalloonText.color = subBalloonColor;
            
            crossFadeSequence = DOTween.Sequence();
            crossFadeSequence.Append( balloonText.DOFade(0.0f, fadeTime) ).
            Join( subBalloonText.DOFade(1.0f, fadeTime) );
            crossFadeSequence.AppendInterval(intervalTime);
            crossFadeSequence.Append( balloonText.DOFade(1.0f, fadeTime) ).
            Join( subBalloonText.DOFade(0.0f, fadeTime) );
            crossFadeSequence.AppendInterval(intervalTime);
            crossFadeSequence.SetLoops(-1);

            crossFadeSequence.Play();
            
        }

        /// <summary>表示切り替え</summary>
        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
            if( !value ) {
               StopCrossFade();
            }
        }
        
        public void StopCrossFade()
        {
            if( crossFadeSequence != null ) {
                crossFadeSequence.Kill();
                crossFadeSequence = null;

                var color = balloonText.color;
                color.a = 1.0f;
                balloonText.color = color;
                subBalloonText.gameObject.SetActive(false);
            }
        }
        
        // ビルド時に副作用がノイズになるのでコメント。プレハブモードとかでバルーンをみたい時にオンにすると良さそう。
        // private void OnValidate()
        // {
        //     SetText(defaultBalloonString);
        // }

        
    }
}
