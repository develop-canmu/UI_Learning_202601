using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;

namespace Pjfb
{
    public static class HowToPlayUtility
    {
        public enum TutorialType
        {
            ClubRoyal = 100001,
            AdviserDeck = 110001,
            AdviserTrainingDeck = 110101,
            AdviserClubRoyalTop = 110201,
        }
        
        public static async UniTask<CruFramework.Page.ModalWindow> OpenHowToPlayModal(long tutorialId, string title)
        {
            HowToPlayModal.HowToData howtoData = new HowToPlayModal.HowToData();
            // データ作る
            TutorialSettingMasterContainer tutorialSettingMaster = MasterManager.Instance.tutorialSettingMaster;
            TutorialSettingMasterObject tutorialSettingMasterObject = null;
            // IDと一致するタイプを探す
            foreach (var tutorialSetting in tutorialSettingMaster.values)
            {
                if (tutorialSetting.type == tutorialId)
                {
                    tutorialSettingMasterObject = tutorialSetting;
                    break;
                }
            }
            MatchCollection imageIds = Regex.Matches(tutorialSettingMasterObject.helpImageIdList, "[0-9]+");
            List<object> descriptions = (List<object>)MiniJSON.Json.Deserialize(tutorialSettingMasterObject.helpDescriptionList);
            howtoData.Title = title;
            int index = 0;
            foreach (var id in imageIds)
            {
                howtoData.Descriptions.Add(new HowToPlayModal.DescriptionData(PageResourceLoadUtility.GetHowToPlayPath(id.ToString()), (string)descriptions[index]));
                index++;
            }
            CruFramework.Page.ModalWindow howToModal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.HowToPlay, howtoData);
            return howToModal;
        }
    }
}