using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Extensions;
using UnityEngine;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb
{
    public class HomeSubscribeButton : MonoBehaviour
    {
        #region Params
        public class Parameters
        {
            public List<NativeApiTagContainer> orderedOpenTagObject;
            public Action<Parameters> onClickButton;

            public Parameters(IEnumerable<NativeApiTag> uTagObj, IEnumerable<LoginPassMasterObject> mLoginPass, Action<Parameters> onClickButton)
            {
                this.onClickButton = onClickButton;

                var mLoginPassDictionary = mLoginPass.ToDictionaryOfList(aData => aData.adminTagId);
                var now = AppTime.Now;
                orderedOpenTagObject = uTagObj
                    .Select(aData => new NativeApiTagContainer {
                        expireAtDateTime = aData.expireAt.TryConvertToDateTime(), 
                        nativeApiTag = aData,
                        loginPassMasterObjectList = mLoginPassDictionary.TryGetValue(aData.adminTagId, out var loginPass) ? loginPass : null,
                    })
                    .Where(aData => aData.loginPassMasterObjectList != null && aData.expireAtDateTime.IsFuture(now))
                    .OrderBy(aData => aData.expireAtDateTime)
                    .ToList();
            }
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private TextMeshProUGUI buttonText = null;
        #endregion

        #region PrivateFields
        private Parameters _parameters = null;
        private NativeApiTagContainer _showingRemainTimeTag;
        #endregion

        #region PublicMethods
        public void Init(Parameters parameters)
        {
            _parameters = parameters;
            if (_parameters.orderedOpenTagObject.Count <= 0) {
                _showingRemainTimeTag = null;
                SetActive(false);
            } else {
                _showingRemainTimeTag = _parameters.orderedOpenTagObject.First();;
                UpdateBadge(_showingRemainTimeTag);
                SetActive(true);    
            }
        }
        #endregion

        #region PrivateMethods
        private void UpdateBadge(NativeApiTagContainer showingRemainTimeTag)
        {
            var text = showingRemainTimeTag.expireAtDateTime.GetPreciseRemainingString(AppTime.Now,
                textFormat: StringValueAssetLoader.Instance["subscribe.home.remain_format"],
                defaultString: StringValueAssetLoader.Instance["subscribe.home.remain_end"]);
            buttonText.text = text;
        }

        private void SetActive(bool isActive) 
        {
            gameObject.SetActive(isActive);
        }
        
        private void Update()
        {
            if(_showingRemainTimeTag != null) UpdateBadge(_showingRemainTimeTag);
        }
        #endregion

        #region EventListener
        public void OnClickButton()
        {
            _parameters?.onClickButton?.Invoke(_parameters);
        }
        #endregion
    }
    
    public class NativeApiTagContainer
    {
        public NativeApiTag nativeApiTag;
        public List<LoginPassMasterObject> loginPassMasterObjectList;
        public DateTime expireAtDateTime;
    }
}