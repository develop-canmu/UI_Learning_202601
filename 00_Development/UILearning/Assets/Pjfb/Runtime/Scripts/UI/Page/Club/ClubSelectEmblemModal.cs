using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using CruFramework.ResourceManagement;
using CruFramework;

namespace Pjfb.Club
{
    public interface ISelectEmblemHandler {
        long emblemIconId{get;}
        void SetEmblem(Sprite sprite, long id);
        long[] CreateSelectIdList();
    }
    
    public class ClubSelectEmblemModal : ModalWindow
    {
        [SerializeField]
        ClubEmblemIcon _itemPrefab = null;

        [SerializeField]
        Transform _scrollContent = null;
        [SerializeField]
        UIButton _onChangeButton = null;
        

        ISelectEmblemHandler _handler = null;

        List<ClubEmblemIcon> _items = new List<ClubEmblemIcon>();

        long _selectedEmblemIconId = 0;
        long _currentEmblemIconId = 0;
        

        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _handler = (ISelectEmblemHandler)args;
            _selectedEmblemIconId = _handler.emblemIconId; 
            _currentEmblemIconId = _handler.emblemIconId;
            var ids = _handler.CreateSelectIdList();

            foreach( var id in ids ){
                var iconId = id;
                var item = Instantiate<ClubEmblemIcon>(_itemPrefab, _scrollContent);
                var sprite = await ClubUtility.LoadEmblemIcon(iconId, item.GetCancellationTokenOnDestroy());
                item.Init(iconId, _selectedEmblemIconId == iconId, OnClickEmblem);
                item.SetIconSprite(sprite);
                _items.Add(item);
            }
            
            UpdateChangeButtonState();
            await base.OnPreOpen(args, token);
        }
        
        public void OnClickCloseButton() {
            Close();
        }

        public void OnClickChangeButton() {
            ClubEmblemIcon selected = null;
            foreach( var item in _items ){
                if( item.isSelecting ) {
                    selected = item;
                    break;
                }
            }
            if( selected != null ) {
                _handler.SetEmblem( selected.emblemSprite, selected.id );
            }
            
            Close();
        }

        void OnClickEmblem( long id ) {
            _currentEmblemIconId = id;
            for( int i=0; i<_items.Count; ++i ){
                _items[i].UpdateSelectingState( _currentEmblemIconId == _items[i].id );
            }

            UpdateChangeButtonState();
        }

        void UpdateChangeButtonState(){
            ClubEmblemIcon selected = null;
            foreach( var item in _items ){
                if( item.isSelecting ) {
                    selected = item;
                    break;
                }
            }
            _onChangeButton.interactable = selected != null && selected.id != _selectedEmblemIconId;
        }

    }
}
