using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.Character
{
    public class SupportEquipmentDetailModalParams : SwipeableDetailModalParams<SupportEquipmentDetailData>
    {
        public readonly bool CanSell;
        public readonly bool CanRedrawing;
        public readonly bool CanFavorite;
        public readonly bool FromPrizeJson;
        public readonly Action OnUpdateBadge;
        public Action<int,bool> OnChangeScrollFavorite;
        public SupportEquipmentDetailModalParams(SwipeableParams<SupportEquipmentDetailData> swipeableParams, bool canSell = true, bool canRedrawing = true, bool fromPrizeJson = false, Action onUpdateBadge = null,Action<int,bool> onChangeScrollFavorite = null,bool canFavorite = true,string titleStringKey = null) : base(swipeableParams, titleStringKey)
        {
            CanSell = canSell;
            CanRedrawing = canRedrawing;
            CanFavorite = canFavorite;
            FromPrizeJson = fromPrizeJson;
            OnUpdateBadge = onUpdateBadge;
            OnChangeScrollFavorite = onChangeScrollFavorite;
        }
    }
    
    public class SupportEquipmentDetailModal : SwipeableDetailModalWindow<SupportEquipmentDetailData, SupportEquipmentDetailModalParams>
    {
        public enum CloseUpdateType
        {
            None,
            Sell,
            Redrawing
        }
        
        [SerializeField] private SupportEquipmentNameView supportEquipmentNameView;
        [SerializeField] private SupportEquipmentPracticeSkillListView mainSupportEquipmentPracticeSkillListView;
        [SerializeField] private SupportEquipmentPracticeSkillListView subSupportEquipmentPracticeSkillListView;
        [SerializeField] private SupportEquipmentIcon supportEquipmentIcon;
        [SerializeField] private TMP_Text annotationText;
        [SerializeField] private UIButton sellButton;
        [SerializeField] private GameObject redrawingButton;
        // 抽選テーブル詳細表示ボタン
        [SerializeField] private UIButton lotteryTableDetailButton = null;
        [SerializeField] private UIToggle favoriteToggle;
        [SerializeField] private GameObject space;
        private UserDataSupportEquipment uSupportEquipment;
        
        protected override string defaultTitleStringKey => "character.support_equipment_.detail";
        
        protected override void Init()
        {
            SetCloseParameter(CloseUpdateType.None);
            supportEquipmentNameView.InitializeUI(objectDetail);
            uSupportEquipment = UserDataManager.Instance.supportEquipment.Find(objectDetail.USupportEquipmentId);
            mainSupportEquipmentPracticeSkillListView.InitializeUI(
                PracticeSkillUtility.GetCharacterPracticeSkill(objectDetail.MCharaId, objectDetail.Lv));
            List<PracticeSkillLotteryInfo> subSkillInfo = PracticeSkillLotteryUtility.GetCharaTrainerLotteryInfo(objectDetail.StatusIdList);
            subSupportEquipmentPracticeSkillListView.InitializeUI(subSkillInfo);
            
            var mCharaTrainerLottery = MasterManager.Instance.charaTrainerLotteryMaster.FindData(objectDetail.MChara.mCharaTrainerLotteryId);
            // 再抽選しようとしたときに isReLotteryLocked と m_chara_trainer_lottery.mCharaTrainerLotteryReloadGroupId を両方見て、どちらも再抽選が許される設定であれば再抽選処理が通ります。
            var canRedrawing = modalParams.CanRedrawing && mCharaTrainerLottery != null &&
                                  mCharaTrainerLottery.mCharaTrainerLotteryReloadGroupId != 0 &&
                                  uSupportEquipment is not null && !uSupportEquipment.isReLotteryLocked;
            //　サブスキルが全て上書き不可の場合注意文言を消す
            annotationText.gameObject.SetActive(objectDetail.StatusIdList == null || canRedrawing && subSkillInfo.Any(x => x.CanReloadOverwrite()));
            // PrizeJson経由で開いた際にサブのみスキルが表示されないため文言を変更
            annotationText.text = objectDetail.StatusIdList == null
                ? StringValueAssetLoader.Instance["character.support_equipment.detail.sub_practice_skill_annotation"]
                : StringValueAssetLoader.Instance["character.support_equipment_.description_.detail_description_01"];
        
            redrawingButton.gameObject.SetActive(canRedrawing);
            lotteryTableDetailButton.gameObject.SetActive(canRedrawing);
            // バッジがついている場合のみ既読の更新処理を行う
            if (SupportEquipmentManager.HasNewSupportEquipment(objectDetail.USupportEquipmentId))
            {
                SupportEquipmentManager.SaveViewedUserSupportEquipmentId(objectDetail.USupportEquipmentId);
                modalParams.OnUpdateBadge?.Invoke();
            }

            //トグルの表示はサポート器具一覧画面とトレーニングでのサポート器具選択画面のみ
            favoriteToggle.gameObject.SetActive(uSupportEquipment != null && !modalParams.FromPrizeJson && modalParams.CanFavorite);
            
            //トグルをお気に入りの状態によってそれぞれ変更
            if(uSupportEquipment != null)
            favoriteToggle.SetIsOnWithoutNotify(uSupportEquipment.isLocked);
            SellButtonInteractive();
            
        }

        public void OnClickSellButton()
        {
            SupportEquipmentSellConfirmModalWindow.Open(new SupportEquipmentSellConfirmModalWindow.WindowParams()
            {
                idList = new []{(long)uSupportEquipment.id},
                onConfirm = () => SetCloseParameter(CloseUpdateType.Sell)
            });
        }

        public void OnClickRedrawingButton()
        {
            SupportEquipmentRedrawingModal.Open(new SupportEquipmentRedrawingModal.Data(uSupportEquipment,
                () => SetCloseParameter(CloseUpdateType.Redrawing)));
        }

        /// <summary> 抽選テーブル詳細を開く </summary>
        public void OnClickLotteryTableDetailButton()
        {
            var modalData = new SupportEquipmentLotteryTableDetailModal.Param(uSupportEquipment);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SupportEquipmentLotteryAbilityDetail, modalData);
        }
        
        public void OnClickCloseButton()
        {
            SetCloseParameter(CloseUpdateType.None);
            Close();
        }
        

        public void OnClickToggle()
        {
            SetSupportEquipmentFavorite().Forget();
        }

        private void SellButtonInteractive()
        {
            sellButton.interactable = modalParams.CanSell && uSupportEquipment is not null &&
                                      !favoriteToggle.isOn &&
                                      !DeckUtility.ExistsSupportEquipment(uSupportEquipment.id);
            sellButton.gameObject.SetActive(sellButton.interactable);

            //売却ボタン非表示の際にダミーボタンを出す
            if (!redrawingButton.activeSelf && !sellButton.gameObject.activeSelf)
            {
                space.SetActive(false);
            }
            else
            {
                space.SetActive(!sellButton.gameObject.activeSelf);
            }
        }
        
        
        private async UniTask SetSupportEquipmentFavorite()
        {
            await favorite.SetSupportEquipmentFavorite(objectDetail.USupportEquipmentId,favoriteToggle.isOn);
            if (modalParams.OnChangeScrollFavorite != null)
            {
                modalParams.OnChangeScrollFavorite(currentIndex, favoriteToggle.isOn);
            }

            SellButtonInteractive();
        }
    }

    static public class favorite
    {
        static private Action onChangeFavorite;

        static public Action OnChangeFavorite
        {
            get { return onChangeFavorite; }
            set { onChangeFavorite = value; }
        }

        static public async UniTask SetSupportEquipmentFavorite(long id, bool flag)
        {
            CharaVariableTrainerLockAPIRequest request = new CharaVariableTrainerLockAPIRequest();
            CharaVariableTrainerLockAPIPost post = new CharaVariableTrainerLockAPIPost()
            {
                id = id,
                onFlg = flag ? 1 : 0
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            //todo コールバック実行
            onChangeFavorite?.Invoke();
        }
    }
}