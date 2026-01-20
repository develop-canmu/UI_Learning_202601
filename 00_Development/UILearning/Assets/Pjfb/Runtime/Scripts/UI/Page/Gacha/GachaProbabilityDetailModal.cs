using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pjfb.Gacha
{
    public class GachaProbabilityDetailModal : ModalWindow
    {
        
        [SerializeField]
        GachaProbabilityGroupView _groupViewPrefab = null;

        [SerializeField]
        RectTransform _scrollContent = null;
        [SerializeField]
        TextMeshProUGUI _title = null;


        [SerializeField]
        float _groupSpace = 40.0f;

        GachaProbabilityData _data = null;
    
        protected override UniTask OnPreOpen(object args, CancellationToken token) {
            _data = (GachaProbabilityData)args;

            _title.text = string.Format( StringValueAssetLoader.Instance["gacha.probability_dialog_tile"], _data.name, _data.count);
            
            var setPosY = 0.0f;
            foreach( var group in _data.groups ){
                var groupView = Instantiate<GachaProbabilityGroupView>(_groupViewPrefab, _scrollContent);
                groupView.Init(group);
                var groupTransform = groupView.GetComponent<RectTransform>();
                var pos = groupTransform.anchoredPosition;
                pos.x = 0;
                pos.y = setPosY;
                groupTransform.anchoredPosition = pos;
                setPosY -= (groupView.height + _groupSpace);
            }


            var scrollContentSize = _scrollContent.sizeDelta; 
            scrollContentSize.y = -setPosY;
            _scrollContent.sizeDelta = scrollContentSize;

            return default;
        }   

        public void OnClickClose(){
            Close();
        }
    }
}
