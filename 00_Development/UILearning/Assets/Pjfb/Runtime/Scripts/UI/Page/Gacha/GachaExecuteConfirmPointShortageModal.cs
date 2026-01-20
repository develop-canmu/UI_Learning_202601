using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Pjfb.UserData;
using Pjfb.Master;
using System.Linq;
using Pjfb.Common;
using Pjfb.Extensions;

namespace Pjfb.Gacha
{
    public class GachaExecuteConfirmPointShortageModal : GachaExecuteConfirmModalBase
    {
        [SerializeField]
        private IconImage[] iconImages = null;
        
        [SerializeField]
        private TextMeshProUGUI necessaryCountText = null;
        
        [SerializeField]
        private TextMeshProUGUI possessionCountText = null;
        
        [SerializeField]
        private PossessionItemUi alternativePointUi = null;

        [SerializeField]
        private TextMeshProUGUI periodText = null;
        
        [SerializeField]
        private TextMeshProUGUI positiveButtonTex = null;
        [SerializeField]
        private UIButton positiveButton = null;

        long festivalTimetableId = 0;
        
        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);
            
            // ポイント画像セット
            foreach (IconImage iconImage in iconImages)
            {
                iconImage.SetTexture(CategoryData.PointId);
            }
            // 必要数
            necessaryCountText.text = CategoryData.Price.ToString();
            // ポイント所持数
            long possessionCount = 0;
            if(UserDataManager.Instance.point.data.ContainsKey(CategoryData.PointId))
            {
                possessionCount = UserDataManager.Instance.point.Find(CategoryData.PointId).value;
            }
            possessionCountText.text = possessionCount.ToString();

            
            // 仮想ポイント所持数
            var isUsableAlternative = GachaUtility.IsUsablePointAlternative(CategoryData.PointId, CategoryData);
            
            // 仮想ポイントを所持・使用できる場合は状態を表示
            alternativePointUi.gameObject.SetActive(isUsableAlternative);
            if (isUsableAlternative)
            {
                var uPointAlternative = GachaUtility.GetPointAlternative(CategoryData.PointId, CategoryData.GachaCategoryId);

                alternativePointUi.SetCount(uPointAlternative.UserData.pointId, uPointAlternative.UserData.value);
                periodText.text = uPointAlternative.AlternativeMasterObject.endAt.TryConvertToDateTime().ToString(StringValueAssetLoader.Instance["point.expire_date_format"]);
            }
            
            // メッセージ
            messageText.text = string.Format(StringValueAssetLoader.Instance["gacha.not_enough_point"], PointMaster.name);

            //イベントガチャか確認
            var isEventGacha = false;
            foreach( var festival in MasterManager.Instance.festivalMaster.values ){

                if( CategoryData.SettingData.GachaSettingId == festival.mGachaSettingId ) 
                {
                    isEventGacha = true;
                    break;
                }
            }
            festivalTimetableId = 0;
            if( isEventGacha ) {
                //イベントが開催期間中か
                var huntTimetableDictionary = MasterManager.Instance.huntTimetableMaster.values.ToDictionary(aData => aData.id);
                var festivalTimetable = MasterManager.Instance.festivalTimetableMaster;
                var festivalTimetableList = Pjfb.Event.EventManager.GetShowingFestivalTimetableList(festivalTimetable, huntTimetableDictionary);
                foreach( var timetable in festivalTimetableList ){
                    var festivalMaster = MasterManager.Instance.festivalMaster.FindData(timetable.mFestivalId);
                    if( CategoryData.SettingData.GachaSettingId == festivalMaster.mGachaSettingId ) {
                        festivalTimetableId = timetable.id;
                        break;
                    }
                }
            }

            if( isEventGacha ) {
                positiveButtonTex.text = string.Format(StringValueAssetLoader.Instance["gacha.to_event"], PointMaster.name);
                positiveButton.interactable = festivalTimetableId != 0;
            } else {
                positiveButtonTex.text = string.Format(StringValueAssetLoader.Instance["gacha.to_shop"], PointMaster.name);
            }

        }
        
        /// <summary>
        /// uGUI
        /// </summary>
        public void OnClickPositiveButton()
        {
            if( festivalTimetableId != 0 ) {
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Event, true, festivalTimetableId);
            } else {
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Shop, true, null);
            }
            
            Close();
        }
    }
}
