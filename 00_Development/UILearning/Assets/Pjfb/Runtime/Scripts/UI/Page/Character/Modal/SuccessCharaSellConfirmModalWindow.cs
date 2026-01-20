using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using CruFramework.UI;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using Pjfb.UserData;
using Pjfb.Common;

namespace Pjfb.Character
{
    public class SuccessCharaSellConfirmModalWindow : ModalWindow
    {
        [SerializeField][StringValue] private string sellCompleteTitle = string.Empty;
        [SerializeField][StringValue] private string sellCompleteContent = string.Empty;
        [SerializeField] private ScrollGrid scroll;
        [SerializeField] private PossessionItemUi possessionItemUi;
        private UserDataPoint[] rewardPoint;
        private List<CharacterVariableDetailData> detailOrderList = new();
        
        #region Params

        public class WindowParams
        {
            public long[] idList;
            public Action onCancel;
            public Action onConfirm;
        }

        #endregion

        private WindowParams _windowParams;
        
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SuccessCharaSellConfirm, data);
        }
        

        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            InitializeView();
            InitializeScroller();
            return base.OnPreOpen(args, token);
        }

        #region PrivateMethods
        private void InitializeView()
        {
            Dictionary<long, long> rewardPointDictionary = new();
            foreach (var charaId in _windowParams.idList)
            {
                UserDataCharaVariable chara = UserDataManager.Instance.charaVariable.Find(charaId);
                var pointMaster = chara.MCharaRankPointMaster;
                if(pointMaster == null) continue;
                if(rewardPointDictionary.ContainsKey(pointMaster.mPointId))
                {
                    rewardPointDictionary[pointMaster.mPointId] += pointMaster.value;
                }
                else
                {
                    rewardPointDictionary[pointMaster.mPointId] = pointMaster.value;
                }
                detailOrderList.Add(new CharacterVariableDetailData(chara));
            }

            rewardPoint = rewardPointDictionary.Select(x => new UserDataPoint(x.Key, x.Value)).ToArray();
            // 一旦index = 0で
            possessionItemUi.SetAfterCountByAmount(rewardPoint[0].pointId, rewardPoint[0].value);
        }

        private void InitializeScroller()
        {
            List<CharacterVariableScrollData> scrollDataList = new List<CharacterVariableScrollData>();
            int index = 0;
            foreach (var charaId in _windowParams.idList)
            {
                UserDataCharaVariable chara =  UserDataManager.Instance.charaVariable.Find(charaId);
                if(chara == null) continue;
                CharacterVariableScrollData data = new CharacterVariableScrollData(chara, null,
                    new SwipeableParams<CharacterVariableDetailData>(detailOrderList, index++));
                scrollDataList.Add(data);
            }
            scroll.SetItems(scrollDataList);
        }
        #endregion

        #region EventListeners
        public void OnClickCancel()
        {
            Close(onCompleted: _windowParams.onCancel);
        }

        public async void OnClickConfirm()
        {
            long pointId = rewardPoint[0].pointId;
            long currentCount = UserDataManager.Instance.GetPointValue(pointId);
            long afterCount = currentCount + rewardPoint[0].value;
            
            CharaVariableSellAPIRequest request = new CharaVariableSellAPIRequest();
            CharaVariableSellAPIPost post = new CharaVariableSellAPIPost();
            post.idList = _windowParams.idList;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            CharaVariableSellAPIResponse response = request.GetResponseData();
            
            
            
            
            Close(onCompleted: () =>
            {
                GainRewardConfirmModal.Open(new GainRewardConfirmModal.WindowParams(
                    StringValueAssetLoader.Instance[sellCompleteTitle], 
                    StringValueAssetLoader.Instance[sellCompleteContent], 
                    pointId, currentCount, afterCount, null));
                _windowParams.onConfirm?.Invoke();
            });
        }
        #endregion
       
        
        
    }
}
