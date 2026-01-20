using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using Pjfb.Master;
using DG.Tweening;


namespace Pjfb.Gacha
{
    public class GachaPrizeIconWrap : MonoBehaviour {
                
        public GachaPrizeIcon prizeIcon => _prizeIcon;
        public GachaPendingFrameData pendingData => _prizeIcon.Pending?.pendingData;
        
        [SerializeField]
        private GachaPrizeIcon prizeIconPrefab = null;

        GachaPrizeIcon _prizeIcon = null;

        System.Action<GachaPrizeIconWrap> _onClickPending = null;


        public void CreateIcon(){
            if( _prizeIcon != null ) {
                Destroy(_prizeIcon.gameObject);
                _prizeIcon = null;
            }
            _prizeIcon = GameObject.Instantiate<GachaPrizeIcon>(prizeIconPrefab, transform);
        }

        /// <summary>
        /// 保留表示に更新
        /// </summary>
        public void UpdatePendingView( GachaPendingFrameData pendingData, System.Action<GachaPrizeIconWrap> onClickPending ){
            _onClickPending = onClickPending;
            _prizeIcon.UpdatePendingView( pendingData, OnClickPending );
        }

        public void UpdatePendingView( Pjfb.Networking.App.Request.GachaPendingFrame pendingData ){
            _prizeIcon.UpdatePendingView( pendingData, OnClickPending );
        }

        public void OnClickPending( GachaPrizeIcon icon ){
            _onClickPending?.Invoke(this);
        }


    }
}