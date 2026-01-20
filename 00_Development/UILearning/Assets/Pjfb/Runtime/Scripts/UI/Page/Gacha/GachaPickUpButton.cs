using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using Pjfb.Storage;

namespace Pjfb.Gacha
{
    public class GachaPickUpButton : MonoBehaviour {
        [SerializeField]
        private UIButton _button = null;
        [SerializeField]
        private GameObject _newBalloon = null;

        GachaChoiceData _data = null;

        public void Init( GachaChoiceData data ) {
            _data = data;
            if( data == null || data.GachaChoiceId <= 0 ) {
                _button.gameObject.SetActive(false);
                return;
            }  
            _button.gameObject.SetActive(true);
            UpdateViewNewBallon();
        }

        public void UpdateViewNewBallon() {
            _newBalloon.gameObject.SetActive(IsViewNewBallon(_data));
        }

        public bool IsViewNewBallon( GachaChoiceData data ) {
            var gachaSaveData = LocalSaveManager.saveData.gachaData;
            //セーブデータの存在チェック
            var targetPickUpDataList = gachaSaveData.lastCheckDate.Where(itr => itr.choiceId == data.GachaChoiceId);
            if( targetPickUpDataList.Count() <= 0 ) {
                return true;
            }


            //中身のチェック
            foreach( var pickUpData in targetPickUpDataList ){
                if( string.IsNullOrEmpty( pickUpData.lastConfirmedDate ) ) {
                    return true;
                }
                var lastConfirmedDate = DateTime.MinValue;
                if( !DateTime.TryParse( pickUpData.lastConfirmedDate , out lastConfirmedDate) ){
                    return true;
                }
                
                if( lastConfirmedDate <= data.ReleasedRecently ) {
                    return true;
                }
            }

            return false;
        }

    }
}
