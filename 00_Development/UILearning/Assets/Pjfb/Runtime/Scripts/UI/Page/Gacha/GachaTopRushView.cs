using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pjfb.Gacha
{
    public class GachaTopRushView : MonoBehaviour {
        const long hourSecond = 60*60;
        
        [SerializeField]
        GameObject _dateRoot = null;
        [SerializeField]
        TextMeshProUGUI _minute = null;
        [SerializeField]
        TextMeshProUGUI _second = null;
        [SerializeField]
        TextMeshProUGUI _minuteLabel = null;
        [SerializeField]
        TextMeshProUGUI _secondLabel = null;
        [SerializeField]
        TextMeshProUGUI _hour = null;
        [SerializeField]
        TextMeshProUGUI _hourLabel = null;
        [SerializeField]
        Animator _animator = null;


        DateTime _endAt = default;
        public void Init( DateTime endAt ){
            _endAt = endAt;
            if( _animator != null ) {
                //前フレームのアニメーションが残るので強制的に更新させる
                _animator.Update(0.1f);
            }
            
            if( _dateRoot != null && GachaUtility.IsIndefinitePeriod(_endAt) ) {
                _dateRoot.SetActive(false);
            }
        }

    
        public void Update(){
            UpdateDateText();
        }
        
        void UpdateDateText(){
            if( _dateRoot != null && !_dateRoot.activeSelf ) {
                return;
            }

            var now = AppTime.Now;
            if( _endAt < now ) {
                return;
            }
            var span = _endAt - now;
            if( span.TotalSeconds > hourSecond ) {
                UpdateHourText( _hour, _hourLabel , (long)span.TotalSeconds / hourSecond );
                HideDateText( _minute, _minuteLabel );
                HideDateText( _second, _secondLabel );
            } else {
                UpdateDateText( _minute, _minuteLabel , span.Minutes);
                UpdateDateText( _second, _secondLabel, span.Seconds);
                HideDateText( _hour, _hourLabel );
            }
        }

        void UpdateDateText( TextMeshProUGUI text, TextMeshProUGUI label, int val){
            text.text = string.Format("{0:D2}", val);
            text.gameObject.SetActive(true);
            label.gameObject.SetActive(true);
        }

        void UpdateHourText( TextMeshProUGUI text, TextMeshProUGUI label, long val){
            text.text = val.ToString();
            text.gameObject.SetActive(true);
            label.gameObject.SetActive(true);
        }


        void HideDateText( TextMeshProUGUI text, TextMeshProUGUI label ){
            text.gameObject.SetActive(false);
            label.gameObject.SetActive(false);
        }
    }
}
