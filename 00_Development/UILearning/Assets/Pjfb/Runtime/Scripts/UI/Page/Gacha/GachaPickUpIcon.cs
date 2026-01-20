using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;
using TMPro;
using Pjfb.Networking.App;
using Pjfb.UserData;
using DG.Tweening;
using Pjfb.Character;

namespace Pjfb.Gacha
{
    public class GachaPickUpIcon : ScrollGridItem {
        public class Param {
            public GachaPickUpSelectableCharacterData characterData = null;
            public Action<Param> onClickIcon = null;
            public bool isSelected = false;
            public float initTime = 0.0f;
            public int index = -1;
            public bool isNew = false;
            public SwipeableParams<CharacterDetailData> swipeableParams;
            // GachaPickUpSelectModalで選択されたか
            public bool isChoiced = false;
        }

        public GachaPickUpSelectableCharacterData characterData => _param.characterData;

        [SerializeField]
        GameObject _selectedFilter = null;

        [SerializeField]
        Image _noPossessedBadge = null;
        
        [SerializeField]
        Image _newBadge = null;
        
        [SerializeField]
        PrizeJsonView _icon = null;
        [SerializeField]
        GameObject _iconRoot = null;
        [SerializeField]
        GameObject _emptyIcon = null;
        [SerializeField]
        GameObject _selectFrame = null;
        [SerializeField]
        GameObject[] _ignoreGrayOutObject = null;
        


        Param _param = null;

        List<Graphic> _grayOutTarget = null;

        Sequence _badgeAnimationSequence = null;



        protected override void OnSetView(object value){
            _param = (Param)value;

            if( _param.characterData == null ) {
                _emptyIcon.SetActive(true);
                _iconRoot.SetActive(false);
            } else {
                _emptyIcon.SetActive(false);
                _iconRoot.SetActive(true);
                _selectedFilter.SetActive(false);
                _selectFrame.SetActive(false);
                
               _icon.SetView(_param.characterData.prize);
                if( _icon.IconType == ItemIconType.Character || _icon.IconType == ItemIconType.SupportEquipment) {
                    var characterIcon = _icon.GetIcon<CharacterIcon>();
                    characterIcon.SetActiveLv(false);
                    characterIcon.SetActiveCount(false);
                    characterIcon.SwipeableParams = _param.swipeableParams;
                }
                _noPossessedBadge.gameObject.SetActive(false);
                _newBadge.gameObject.SetActive(_param.isNew);
                var prizeData = PrizeJsonUtility.GetPrizeContainerData(_param.characterData.prize);
                var prizeType = prizeData.itemIconType;
                if(  prizeType == ItemIconType.Character ) {
                    _noPossessedBadge.gameObject.SetActive(!UserDataManager.Instance.chara.HaveCharacterWithMasterCharaId(_param.characterData.prize.args.mCharaId));
                }
                
                //ユーザーデータに該当のサポート器具を持っているか確認　(*スペシャルサポートカードはItemIconType.Characterになる)
                else if ( prizeType == ItemIconType.SupportEquipment )
                {
                    _noPossessedBadge.gameObject.SetActive(!UserDataManager.Instance.supportEquipment.HaveCharacterWithMasterCharaId(_param.characterData.prize.args.variableTrainerMCharaId));
                }

                PlayBadgeAnimation();
                
                if( _param.isSelected ) {
                    CreateGrayOutTarget();
                }
                SetSelectedState(_param.isSelected);
                SetSelectFrameActive(_param.isChoiced);
            }
        }

        public void OnClickIcon(){
            _param.onClickIcon?.Invoke(_param);
        }
       
        public void SetSelectFrameActive( bool isActive ){
            _selectFrame.SetActive(isActive);
            // ピックアップ数が少ない時にOnSetViewが呼ばれないのでnullチェック
            if(_param != null)
            {
                _param.isChoiced = isActive;
            }
        }

        void SetSelectedState( bool isSelect ){
            _selectedFilter.SetActive(isSelect);
            if (_grayOutTarget == null || _grayOutTarget.Count == 0)
            {
                return;
            }
            
            var color = isSelect ? Color.gray : Color.white;
            foreach( var graphic in _grayOutTarget ) {
                graphic.color = color;
            }
        }

        void CreateGrayOutTarget( ){
            _grayOutTarget = new List<Graphic>();;
            var currentIcon = _icon.GetIcon();
            foreach(var image in  currentIcon.transform.GetComponentsInChildren<Graphic>(true)){
                if( _ignoreGrayOutObject.Any( itr=> itr == image.gameObject ) ) {
                    continue;
                }
                _grayOutTarget.Add(image);
            }
        }

         void PlayBadgeAnimation(){

            if( _badgeAnimationSequence != null ) {
                _badgeAnimationSequence.Kill();
                _badgeAnimationSequence = null;
                _noPossessedBadge.color = Color.white;
                _newBadge.color = Color.white;
            }

            if( !_noPossessedBadge.gameObject.activeSelf || !_newBadge.gameObject.activeSelf ){
                return;
            }

            //両方表示する場合のみアニメーションする
            var viewColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            var hideColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            _noPossessedBadge.color = viewColor;
            _newBadge.color = hideColor;

            var fadeTime = 0.5f;
            var intervalTime = 1.5f;
            _badgeAnimationSequence = DOTween.Sequence();
            
            _badgeAnimationSequence.AppendInterval(intervalTime);
            _badgeAnimationSequence.Append( _noPossessedBadge.DOFade(0.0f, fadeTime) ).
            Join( _newBadge.DOFade(1.0f, fadeTime) );
            _badgeAnimationSequence.AppendInterval(intervalTime);
            _badgeAnimationSequence.Append( _noPossessedBadge.DOFade(1.0f, fadeTime) ).
            Join( _newBadge.DOFade(0.0f, fadeTime) );
            _badgeAnimationSequence.SetLoops(-1);

            var elapsedTime = Time.time - _param.initTime;
            _badgeAnimationSequence.Goto(elapsedTime, true);
            _badgeAnimationSequence.Play();
        }

        void OnDestroy(){
            if( _badgeAnimationSequence != null ) {
                _badgeAnimationSequence.Kill();
            }
            _badgeAnimationSequence = null;
        }
       
    }
}