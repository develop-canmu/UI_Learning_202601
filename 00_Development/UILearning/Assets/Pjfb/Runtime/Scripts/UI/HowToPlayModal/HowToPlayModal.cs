using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

using Pjfb.Master;
using TMPro;

namespace Pjfb
{
    public class HowToPlayModal : ModalWindow
    {
        public class HowToData
        {
            private string title = string.Empty;
            /// <summary>タイトル</summary>
            public string Title{get{return title;}set{title = value;}}
            
            private List<DescriptionData> descriptions = new List<DescriptionData>();
            /// <summary>説明</summary>
            public List<DescriptionData> Descriptions{get{return descriptions;}}
        }
        
        public class DescriptionData
        {
            private string imageAddress = string.Empty;
            /// <summary>ImageId</summary>
            public  string ImageAddress{get{return imageAddress;}}
            
            private string description = string.Empty;
            /// <summary>説明</summary>
            public string Description{get{return description;}}

            public DescriptionData(string imageAddress, string description)
            {
                this.imageAddress = imageAddress;
                this.description = description;
            }
        }
        
        public static HowToData CreateHowToDataByScenarioId(long scenarioId)
        {
            TrainingScenarioMasterObject mTrainingScenario = MasterManager.Instance.trainingScenarioMaster.FindData(scenarioId);

            // Id
            MatchCollection ids = Regex.Matches(mTrainingScenario.imageId, "[0-9]+");
            // 説明
            List<object> descriptions = (List<object>)MiniJSON.Json.Deserialize(mTrainingScenario.description);
            
            HowToData howtoData = new HowToData();
            // タイトル
            howtoData.Title = StringValueAssetLoader.Instance["training.training_menu.detail.how_to_play.title"];
            int index = 0;
            foreach(var id in ids)
            {
                // テクスチャアドレスと説明を追加
                howtoData.Descriptions.Add(new DescriptionData( PageResourceLoadUtility.GetTrainingHowToImagePath(id.ToString()), (string)descriptions[index]));
                index++;
            }
            
            return howtoData;
        }

        public static void OpenHowToPlayModal(string imageIdList,string descriptionList, string title, Func<string,string> loadImage)
        {
            MatchCollection imageIds = Regex.Matches(imageIdList, "[0-9]+");
            List<object> descriptions = (List<object>)MiniJSON.Json.Deserialize(descriptionList);
            
            HowToData howToData = new HowToData();
            howToData.Title = title;
            int index = 0;
            foreach(Match id in imageIds)
            {
                howToData.Descriptions.Add(new DescriptionData(loadImage(id.ToString()), (string)descriptions[index]));
                index++;
            }
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.HowToPlay, howToData);
        }
        
        [SerializeField]
        private TMP_Text titleText = null;
        
        [SerializeField]
        private ScrollBanner scrollBanner = null;
        
        [SerializeField]
        private UIButton nextButton = null;
        [SerializeField]
        private UIButton closeButton = null;
        
        [SerializeField]
        private UIButton backButton = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            HowToData howtoData = (HowToData)args;
            
            // タイトル
            titleText.text = howtoData.Title;
            // 説明
            scrollBanner.SetBannerDatas(howtoData.Descriptions);
            
            scrollBanner.ScrollGrid.OnChangedPage -= OnChangePage;
            scrollBanner.ScrollGrid.OnChangedPage += OnChangePage;
            OnChangePage(0);
            return base.OnPreOpen(args, token);
        }
        
        private void OnChangePage(int page)
        {
            int pageCount = scrollBanner.ScrollGrid.PageCount ;
            // 戻るボタン
            backButton.gameObject.SetActive( page > 0 );
            // 次へボタン
            nextButton.gameObject.SetActive( page < pageCount && page != pageCount - 1);
            // 閉じるボタン
            closeButton.gameObject.SetActive( page == pageCount - 1 );
        }
        
        public void OnNextButton()
        {
            scrollBanner.ScrollGrid.NextPage();
        }
        
        public void OnBackButton()
        {
            scrollBanner.ScrollGrid.PrevPage();
        }
        
    }
}