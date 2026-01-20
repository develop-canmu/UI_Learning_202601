using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;

namespace Pjfb.Club
{
    public class ClubEmblemIcon : MonoBehaviour {
        public long id {get; private set;}
        public bool isSelecting {get; private set;}
        public Sprite emblemSprite=>_image.sprite;
        


        [SerializeField]
        GameObject _selectingObject = null;
        [SerializeField]
        GameObject _selectedObject = null;

        [SerializeField]
        Image _image = null;

        System.Action<long> _onClickCallBack = null;


        public void Init( long id, bool isSelected, System.Action<long> onClick ){
            this.id = id;
            _onClickCallBack = onClick;
            UpdateSelectedState(isSelected);
            UpdateSelectingState(false);
        }

        public void SetIconSprite( Sprite sprite ){
            _image.sprite = sprite;
        }

        public void UpdateSelectingState( bool isSelecting ){
            _selectingObject.gameObject.SetActive(isSelecting);
            this.isSelecting = isSelecting;
        }
       

        public void OnClickEmblem(){
            _onClickCallBack?.Invoke(id);
        }

        void UpdateSelectedState( bool isSelected ){
            _selectedObject.gameObject.SetActive(isSelected);
        }
    }
}