using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Character;
using TMPro;



namespace Pjfb.Gacha
{
    public class GachaPickUpSelectedListItem : MonoBehaviour {

        [SerializeField]
        TextMeshProUGUI _title = null;

        [SerializeField]
        ScrollGrid _scrollGrid = null;
        
        [SerializeField]
        GameObject _newImage = null;
        

        long _choiceId = 0;
        GachaPickUpElementData _data = null;
        Action _onUpdateSaveData = null;
        Action<long, long, int, float> _onSelected = null;
        List<long> _selectedId = null;
        float _scrollPos = 0f;
        

        public void Init( long choiceId, GachaPickUpElementData data, List<long> selectedId,  Action onUpdateSaveData , Action<long,long,int,float> onSelected, float scrollPos) {
            _data = data;
            _choiceId = choiceId;
            _title.text = data.name;
            _onUpdateSaveData = onUpdateSaveData;
            _onSelected = onSelected;
            _selectedId = selectedId;
            _newImage.gameObject.SetActive( IsViewNewImage(choiceId, data) );
            _scrollPos = scrollPos;
            
            CreateIcons(data, selectedId);
        }


        public void UpdateView( ) {
            _newImage.gameObject.SetActive( IsViewNewImage(_choiceId, _data) );
        }



        public void OnClick( GachaPickUpIcon.Param iconParam ) {
            var param = new GachaPickUpSelectModal.Param();
            param.choiceId = _choiceId;
            param.index = iconParam.index;
            param.pickUpData = _data;
            param.onClickSave = _onSelected;
            param.selectedId = _selectedId;
            param.onUpdateSaveData = _onUpdateSaveData;
            param.scrollPos = _scrollGrid.horizontalNormalizedPosition;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.GachaPickUpSelect, param);
        }

        void CreateIcons( GachaPickUpElementData data, List<long> selectedId ) 
        {
            List<CharacterDetailData> detailOrderList = new();
            var dataList = new List<GachaPickUpIcon.Param>();
            int detailOrderIndex = 0;
            
            for( int i=0; i<data.choiceCount; ++i ) {
                GachaPickUpSelectableCharacterData selectableCharacterData = null;
                if( selectedId != null && selectedId[i] != 0  ) {
                    selectableCharacterData = data.selectableCharacters.FirstOrDefault(itr => itr.id == selectedId[i]);
                    
                }
                
                var param = new GachaPickUpIcon.Param();
                param.characterData = selectableCharacterData;
                param.onClickIcon = OnClick;
                param.isSelected = false;
                param.index = i;
                param.isNew = false;
                param.initTime = Time.time;

                if (param?.characterData?.prize.args.mCharaId > 0)
                {
                    detailOrderList.Add(new CharacterDetailData(param.characterData.prize.args.mCharaId, 1, 0));
                    param.swipeableParams = new SwipeableParams<CharacterDetailData>(detailOrderList, detailOrderIndex++);
                }
                
                dataList.Add(param);
            }
            _scrollGrid.SetItems(dataList);
            _scrollGrid.horizontalNormalizedPosition = _scrollPos;
        }

        /// <summary>
        /// newImageを表示するか
        /// </summary>
        bool IsViewNewImage( long choiceId, GachaPickUpElementData data ){
            
            //セーブデータにIdが保存されていなかったら表示
            foreach( var selectable in data.selectableCharacters ){

                if( !GachaUtility.IsConfirmedPickUpSelectableData( choiceId, data.id, selectable ) ) {
                    return true;
                }
            }

            return false;
        }
       
    }
}