using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Pjfb.Story
{
    public class StoryScenarioPoolListItem : PoolListItemBase
    {
        #region Params
        public enum State { New, NewWithAnimation, Complete, Lock };
        public class ItemParams : ItemParamsBase
        {
            public HuntEnemyMasterObject subStoryData;
            public List<PrizeJsonWrap> prizeDataList;
            public State state;
            public long currentPoint;
            public readonly Action<ItemParams> onClick;

            // Unlockedアニメーション後のオブジェクト管理用
            private bool isUnlockedAnimationFlg = false;
            public bool IsUnlockedAnimationFlg
            {
                get { return isUnlockedAnimationFlg; }
                set { isUnlockedAnimationFlg = value; }
            }
            
            public ItemParams(HuntEnemyMasterObject subStoryData, State state, List<PrizeJsonWrap> prizeDataList, long currentPoint, Action<ItemParams> onClick)
            {
                this.subStoryData = subStoryData;
                this.state = state;
                this.prizeDataList = prizeDataList;
                this.currentPoint = currentPoint;
                this.onClick = onClick;
            }
        }
        
        #endregion

        #region SerializeField
        [SerializeField] private GameObject newIconGameObject;
        [SerializeField] private GameObject lockGameObject;
        [SerializeField] private GameObject renshuShiaiGameObject;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subNameText;
        [SerializeField] private TextMeshProUGUI rewardCountText;
        [SerializeField] private TextMeshProUGUI requiredPointText;
        [SerializeField] private PrizeJsonView itemIcon;
        [SerializeField] private Image thumbnailImage;
        [SerializeField] private AnimatorController storyListItemAnimatorController;
        #endregion

        #region PublicFields
        public bool IsNewWithAnimationState => itemParams.state == State.NewWithAnimation;
        #endregion
        
        #region PrivateFields
        private ItemParams itemParams;
        private string TitleFormat => StringValueAssetLoader.Instance["event.scenario.listItem.title_format"];
        private string RequiredPointFormat => StringValueAssetLoader.Instance["event.scenario.listItem.required_point_format"];
        private CancellationTokenSource source = null;
        #endregion

        #region OverrideMethods
        private void Awake()
        {
            storyListItemAnimatorController.Init();
        }

        public override void Init(ItemParamsBase itemParamsBase)
        {
            itemParams = (ItemParams)itemParamsBase;
            UpdateDisplay(itemParams);
            base.Init(itemParamsBase);
        }
        #endregion

        #region PublicMethods
        public async UniTask PlayReleaseAnimation()
        {
            await storyListItemAnimatorController.Play("Unlocked");
            itemParams.IsUnlockedAnimationFlg = true;
        }
        #endregion
        
        #region PrivateMethods
        private void UpdateDisplay(ItemParams itemParams)
        {
            newIconGameObject.SetActive(itemParams.state == State.New);
            lockGameObject.SetActive(itemParams.state is State.Lock or State.NewWithAnimation && !itemParams.IsUnlockedAnimationFlg);
            var requiredPoint = itemParams.subStoryData.keyMPointValue - itemParams.currentPoint;
            requiredPointText.text = requiredPoint > 0 ? string.Format(RequiredPointFormat, itemParams.subStoryData.keyMPointValue - itemParams.currentPoint) : string.Empty;
            renshuShiaiGameObject.SetActive(itemParams.subStoryData.IsBattle);
            titleText.text = ConvertTitleString(itemParams.subStoryData.name);
            subNameText.text = itemParams.subStoryData.subName;
            // 初回報酬情報
            if (itemParams.state == State.Complete || !itemParams.prizeDataList.Any()) itemIcon.transform.parent.gameObject.SetActive(false);
            else {
                itemIcon.transform.parent.gameObject.SetActive(true);
                rewardCountText.text = itemParams.prizeDataList[0].args.value.ToString();
                itemIcon.SetView(itemParams.prizeDataList[0]);    
            }
            
            // StateがLockならアニメーションを一度再生させておく
            if (itemParams.state == State.Lock)
            {
                PlayLockAnimation().Forget();
            }
            
            SetThumbnail();
        }
        
        private void SetThumbnail()
        {
            if(source != null) {
                source.Cancel();
                source.Dispose();
                source = null;
            }
            thumbnailImage.gameObject.SetActive(false);
            source = new CancellationTokenSource();
            PageResourceLoadUtility.LoadAssetAsync<Sprite>(key: $"Images/StoryThumbnailScenario/story_thumbnail_scenario_{itemParams.subStoryData.id}.png",
                callback: sprite => {
                    thumbnailImage.sprite = sprite;
                    thumbnailImage.gameObject.SetActive(true);
                },
                token: source.Token).Forget();
        }
        
        /// <summary>
        /// 「第xxx章」を「第<size=80>xxx</size>章」に変換する
        /// 変換できない場合そのまま返す
        /// </summary>
        private string ConvertTitleString(string titleString)
        {
            return titleString.StartsWith(TitleFormat[0]) && titleString.EndsWith(TitleFormat[^1]) ? 
                string.Format(TitleFormat, titleString[1..^1]) : titleString;
        }
        
        private async UniTask PlayLockAnimation()
        {
            await storyListItemAnimatorController.Play("Locked");
        }
        
        #endregion
        
        
        #region EventListeners
        public void OnClickItem()
        {
            itemParams?.onClick?.Invoke(itemParams);
        }
        #endregion
    }
}