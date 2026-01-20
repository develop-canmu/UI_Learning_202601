using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using TMPro;
using CruFramework.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Club;

namespace Pjfb.EventRanking {
    public abstract class EventRankingListItem : ScrollGridItem {
        
        [SerializeField]
        protected TextMeshProUGUI _rankText = null;
        [SerializeField]
        protected Image _rankImage = null;
        [SerializeField]
        protected Sprite[] _rankSprite = null;
        [SerializeField]
        protected Image _currentRankFrame = null;



        protected void UpdateRankView( long rank, bool isCurrent ){
            if( rank == 0 ) {
                _rankText.gameObject.SetActive(true);
                _rankImage.gameObject.SetActive(false);
                _rankText.text = StringValueAssetLoader.Instance["ranking.rank_blank"];
                return;
            }
            var imageViewRank = 3;

            _rankText.gameObject.SetActive(rank > imageViewRank);
            _rankImage.gameObject.SetActive(rank <= imageViewRank);
            if( rank <= imageViewRank ) {
                _rankImage.sprite = _rankSprite[ rank - 1 ];
            } else {
                _rankText.text = rank.ToString();
            }
            _currentRankFrame.gameObject.SetActive(isCurrent);
        }

        
    }
}