using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using Pjfb.Master;
using DG.Tweening;


namespace Pjfb.Gacha
{
    public class GachaPrizeIconPendingInfo : MonoBehaviour
    {
        public GachaPendingFrameData pendingData => _pendingData;
        public bool canRetry => _canRetry;
        
        [SerializeField]
        private GameObject _pendingFilter = null;

        [SerializeField]
        private GameObject _infoRoot = null;
        [SerializeField]
        private UIBadgeBalloon _retryCount = null;

        GachaPrizeIcon _prizeIcon = null;
        GachaPendingFrameData _pendingData = null;
        System.Action<GachaPrizeIcon> _onClickPending = null;
        bool _canRetry = false;

        public void HidePendingView( ){
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 保留表示に更新
        /// </summary>
        public void UpdatePendingView( GachaPrizeIcon prizeIcon, GachaPendingFrameData pendingData, System.Action<GachaPrizeIcon> onClickPending ){
            _prizeIcon = prizeIcon;
            _pendingData = pendingData;
            _onClickPending = onClickPending;
            RefreshView();
            
        }

        public void UpdatePendingData( GachaPrizeIcon prizeIcon, Pjfb.Networking.App.Request.GachaPendingFrame pendingData, System.Action<GachaPrizeIcon> onClickPending ){
            _prizeIcon = prizeIcon;
            _pendingData = new GachaPendingFrameData(pendingData);
            _onClickPending = onClickPending;
        }


        public void RefreshView(){
            gameObject.SetActive(true);
            _canRetry = _pendingData != null && _pendingData.CanRetry;
            
            _pendingFilter.gameObject.SetActive( !canRetry );
            _infoRoot.SetActive( canRetry );

            if( !canRetry ) {
                return;
            }
            
            _retryCount.SetText( string.Format(StringValueAssetLoader.Instance["gacha.count_balloon_text"], _pendingData.RetryLimit - 
            _pendingData.RetryCount, _pendingData.RetryLimit) );
        }


        public void OnClickPending( ){
            _onClickPending?.Invoke( _prizeIcon );
        }

        public void OnLongTap( ){
            var mainView = _prizeIcon.MainView;
            if( mainView.IconType == ItemIconType.Item ){
                (mainView.GetIcon() as ItemIcon)?.OnLongTap();
            } else if( mainView.IconType == ItemIconType.Character ){
                (mainView.GetIcon() as CharacterIcon)?.OnLongTap();
            } else if( mainView.IconType == ItemIconType.VariableCharacter  ){
                (mainView.GetIcon() as CharacterVariableIcon)?.OnLongTap();
            } else if( mainView.IconType == ItemIconType.SpecialSupportCharacterCard  ){
                (mainView.GetIcon() as SpecialSupportCardIcon)?.OnLongTap();
            } else if( mainView.IconType == ItemIconType.CharacterPiece  ){
                (mainView.GetIcon() as CharacterPieceIcon)?.OnLongTap();
            } else if( mainView.IconType == ItemIconType.UserIcon  ){
                (mainView.GetIcon() as UserIcon)?.OnLongTap();
            } else if( mainView.IconType == ItemIconType.UserTitle  ){
                (mainView.GetIcon() as UserTitleIcon)?.OnLongTap();
            } else if( mainView.IconType == ItemIconType.ChatStamp  ){
                (mainView.GetIcon() as ChatStampIcon)?.OnLongTap();
            } else if( mainView.IconType == ItemIconType.SupportEquipment  ){
                (mainView.GetIcon() as CharacterIcon)?.OnLongTap();
            }else if( mainView.IconType == ItemIconType.ProfilePart  ){
                (mainView.GetIcon() as ProfilePartIcon)?.OnLongTap();
            }
        }


        
    }
}