using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Pjfb.Menu
{
    public class ItemConfirmModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public long Id;
            public Action onClosed;
        }

        [SerializeField] private ItemIconContainer itemIconCell;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDetailText;
        [SerializeField] private TextMeshProUGUI itemCountText;
        [SerializeField] private OmissionTextSetter countOmissionTextSetter;
        [SerializeField] private TextMeshProUGUI paidCountText;
        [SerializeField] private OmissionTextSetter paidCountOmissionTextSetter;
        [SerializeField] private GameObject paidRoot;
        [SerializeField] private TextMeshProUGUI periodText;
        [SerializeField] private UIButton detailButton;
        
        private const int PaidItemId = 2;
        private WindowParams _windowParams;
        private PointExpiryMasterObject expiryObject = null;
        
        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ItemConfirm, data);
        }

        protected override UniTask OnOpen(CancellationToken token)
        {
            UpdateCount();
            return base.OnOpen(token);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            return base.OnPreOpen(args, token);
        }
        private void Init()
        {
            PointMasterObject pointDetail = MasterManager.Instance.pointMaster.FindData(_windowParams.Id);
            ItemIcon icon = itemIconCell.GetIcon<ItemIcon>();
            icon.SetTexture(_windowParams.Id);
            PointDescriptionMasterObject description = MasterManager.Instance.pointDescriptionMaster.FindByMPointId(_windowParams.Id);
            itemDetailText.text = description?.description ?? ""; 
            itemNameText.text = pointDetail.name;
            
            //有償アイテム設定
            paidRoot.SetActive(_windowParams.Id == PaidItemId);
            
            // 仮想アイテム関係
            PointAlternativeMasterObject pointAlternative = SearchPointAlternative();
            if (pointAlternative != null)
            {
                periodText.gameObject.SetActive(true);
                periodText.text = pointAlternative.endAt.TryConvertToDateTime().ToString(StringValueAssetLoader.Instance["point.expire_date_format"]);
            }
            else
            {
                periodText.gameObject.SetActive(false);
            }
            
            // 代替アイテムの場合は詳細ボタンを表示
            expiryObject = MasterManager.Instance.pointExpiryMaster.FindData(MasterManager.Instance.pointMaster.FindData(_windowParams.Id).mPointExpiryId);
            if(expiryObject != null)
            {
                if(expiryObject.itemPreviewType == (long)PointExpiryMasterObject.ItemPreviewType.DisplayExpiry)
                {
                    detailButton.gameObject.SetActive(true);
                }
                else
                {
                    detailButton.gameObject.SetActive(false);
                }
            }
            else
            {
                detailButton.gameObject.SetActive(false);
            }
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }

        public void OnClickDetail()
        {
            OnClickDetailAsync().Forget();
        }
        
        private  async UniTask OnClickDetailAsync()
        {
            PointGetPointDetailAPIRequest request = new PointGetPointDetailAPIRequest();
            PointGetPointDetailAPIPost post = new PointGetPointDetailAPIPost();
            post.mPointId = _windowParams.Id;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            PointGetPointDetailAPIResponse response = request.GetResponseData();
            
            ItemBreakdownDetailModal.ItemData expiryData = new ItemBreakdownDetailModal.ItemData(_windowParams.Id, response.pointExpiryList, response.pointHistoryList);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ItemBreakdownDetail,expiryData);
        }
        #endregion
        
        #region private
        // 仮想アイテムかどうか検索
        private PointAlternativeMasterObject SearchPointAlternative()
        {
            PointAlternativeMasterObject pointAlternativeMasterObject = null;
            DateTime endAt = default;
            foreach (var pointAlternative in MasterManager.Instance.pointAlternativeMaster.values)
            {
                if(_windowParams.Id != pointAlternative.mPointIdAlternative) continue;
                DateTime findPointAlternativeEndAt = pointAlternative.endAt.TryConvertToDateTime();
                // 暫定で有効期限が長いものを取得するように
                if (pointAlternativeMasterObject == null || endAt <  findPointAlternativeEndAt )
                {
                    pointAlternativeMasterObject = pointAlternative;
                    endAt = findPointAlternativeEndAt ;
                }
            }
            return pointAlternativeMasterObject;
        }

        // 所持数の更新
        private void UpdateCount()
        {
            // アイテムの所持数
            long pointValue;
            PointMasterObject pointMaster = MasterManager.Instance.pointMaster.FindData(_windowParams.Id);
            if (pointMaster.pointType == (long)PointMasterObject.PointType.ExternalPoint)
            {
                pointValue = UserDataManager.Instance.GetExpiryPointValue(_windowParams.Id);
            }
            else
            {
                pointValue = UserDataManager.Instance.point.Find(_windowParams.Id)?.value ?? 0;
            }
            
            BigValue count = new BigValue(pointValue);
            itemCountText.text = count.ToDisplayString(countOmissionTextSetter.GetOmissionData());
            // 有償アイテムの所持数
            if (_windowParams.Id == PaidItemId)
            {
                paidCountText.text = new BigValue(UserDataManager.Instance.pointPaid).ToDisplayString(paidCountOmissionTextSetter.GetOmissionData());
            }
        }
        #endregion
    }
}