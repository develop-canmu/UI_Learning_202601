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
using Pjfb.UserData;

namespace Pjfb.Character
{
    public class SupportEquipmentSellConfirmModalWindow : ModalWindow
    {
        [SerializeField] private ScrollGrid supportEquipmentScroller;
        [SerializeField] private ScrollGrid uPointScroller;
        private List<ItemIconGridItem.Data> uPointScrollDataList;
        private List<SupportEquipmentDetailData> detailOrderList = new();
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
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SupportEquipmentSellConfirm, data);
        }
        

        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            InitializeScroller();
            return base.OnPreOpen(args, token);
        }

        #region PrivateMethods
        private void InitializeScroller()
        {
            Dictionary<long, long> uPointDictionary = new();
            Dictionary<long, PointCharaMasterObject> mPointCharaDictionary =
                MasterManager.Instance.pointCharaMaster.values.ToDictionary(x => x.mCharaId, x => x);


            List<SupportEquipmentScrollData> supportEquipmentScrollDataList = new List<SupportEquipmentScrollData>();
            detailOrderList.Clear();
            int index = 0;
            foreach(var uSupportEquipmentId in _windowParams.idList)
            {
                UserDataSupportEquipment uSupportEquipment = UserDataManager.Instance.supportEquipment.Find(uSupportEquipmentId);
                if(uSupportEquipment is null) continue;
                detailOrderList.Add(new SupportEquipmentDetailData(uSupportEquipment));

                SupportEquipmentScrollData data = new SupportEquipmentScrollData(uSupportEquipment,
                    new SwipeableParams<SupportEquipmentDetailData>(detailOrderList, index++), onUpdateBadge: () =>
                    {
                        supportEquipmentScroller.Refresh();
                        AppManager.Instance.UIManager.Footer.CharacterButton.SetNotificationBadge(BadgeUtility
                            .IsCharacterBadge);
                    });
                supportEquipmentScrollDataList.Add(data);

                if (mPointCharaDictionary.TryGetValue(uSupportEquipment.charaId, out var mPointChara))
                {
                    if(uPointDictionary.ContainsKey(mPointChara.mPointId))
                    {
                        uPointDictionary[mPointChara.mPointId] += mPointChara.valueToSale;
                    }
                    else
                    {
                        uPointDictionary[mPointChara.mPointId] = mPointChara.valueToSale;
                    }
                }
            }
            
            supportEquipmentScroller.SetItems(supportEquipmentScrollDataList);

            uPointScrollDataList = new List<ItemIconGridItem.Data>();
            foreach (var keyValuePair in uPointDictionary)
            {
                ItemIconGridItem.Data data = new ItemIconGridItem.Data(keyValuePair.Key, keyValuePair.Value, true);
                uPointScrollDataList.Add(data);
            }
            uPointScroller.SetItems(uPointScrollDataList);
        }
        #endregion

        #region EventListeners
        public void OnClickCancel()
        {
            Close(onCompleted: _windowParams.onCancel);
        }

        public void OnClickConfirm()
        {
            SellSupportEquipment().Forget();
        }
        #endregion

        private async UniTask SellSupportEquipment()
        {
            CharaVariableTrainerSellAPIRequest request = new CharaVariableTrainerSellAPIRequest();
            CharaVariableTrainerSellAPIPost post = new CharaVariableTrainerSellAPIPost
            {
                idList = _windowParams.idList
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            CharaVariableTrainerSellAPIResponse response = request.GetResponseData();
            
            _windowParams.onConfirm?.Invoke();
            AppManager.Instance.UIManager.ModalManager.RemoveTopModals((m) => m is not SupportEquipmentSellCompletionModalWindow);
            Close(onCompleted: () =>
            {
                SupportEquipmentSellCompletionModalWindow.Open(new SupportEquipmentSellCompletionModalWindow.WindowParams()
                {
                    uPointScrollDataList = uPointScrollDataList,
                });
                
                
            });
        }
        
        
    }
}
