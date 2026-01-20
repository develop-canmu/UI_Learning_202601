using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Club {
    public class FindClubPageTabButton : SheetSwitchButton<FindClubPageTabSheetManager, FindClubPageTabSheetType> {
        
        [SerializeField]
        Image _activeImage = null;
        [SerializeField]
        Image _deactiveImage = null;
        

        [SerializeField]
        Sprite _centerActiveImage = null;
        [SerializeField]
        Sprite _centerDeactiveImage = null;
        [SerializeField]
        Sprite _rightActiveImage = null;
        [SerializeField]
        Sprite _rightDeactiveImage = null;

        public void SetCenterImage(){
            _activeImage.sprite = _centerActiveImage;
            _deactiveImage.sprite = _centerDeactiveImage;
        }

        public void SetRightImage(){
            _activeImage.sprite = _rightActiveImage;
            _deactiveImage.sprite = _rightDeactiveImage;
        }

    }
}