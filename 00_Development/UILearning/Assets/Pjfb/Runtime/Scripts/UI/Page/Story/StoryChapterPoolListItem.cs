using System;
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
    public class StoryChapterPoolListItem : PoolListItemBase
    {
        #region Params
        public enum State { New, NewWithAnimation, Complete };
        public class ItemParams : ItemParamsBase
        {
            public HuntStageMasterObject storyData;
            public long currentProgress;
            public State state;
            public readonly Action<ItemParams> onClick;

            public ItemParams(HuntStageMasterObject storyData, State state, long currentProgress, Action<ItemParams> onClick)
            {
                this.storyData = storyData;
                this.currentProgress = currentProgress;
                this.state = state;
                this.onClick = onClick;
            }
        }
        
        #endregion

        #region SerializeField
        [SerializeField] private GameObject newIconGameObject;
        [SerializeField] private GameObject completeIconGameObject;
        [SerializeField] private GameObject lockGameObject;
        [SerializeField] private TextMeshProUGUI storyTitleText;
        [SerializeField] private TextMeshProUGUI storySubNameText;
        [SerializeField] private TextMeshProUGUI storyDescText;
        [SerializeField] private Image thumbnailImage;
        [SerializeField] private AnimatorController storyListItemAnimatorController;
        #endregion
        
        #region PublicFields
        public bool IsNewWithAnimationState => itemParams.state == State.NewWithAnimation;
        #endregion
        
        #region PrivateFields
        private ItemParams itemParams;
        private const string TitleFormat = "第 <size=70>{0}</size> 章";
        private CancellationTokenSource source = null;
        #endregion
        
        #region PublicMethods
        public async UniTask PlayReleaseAnimation()
        {
            await storyListItemAnimatorController.Play("Release");
        }
        #endregion
        
        #region OverrideMethods
        public override void Init(ItemParamsBase itemParamsBase)
        {
            itemParams = (ItemParams)itemParamsBase;
            UpdateDisplay(itemParams);
            base.Init(itemParamsBase);
        }
        #endregion
        
        #region PrivateMethods
        private void UpdateDisplay(ItemParams itemParams)
        {
            newIconGameObject.SetActive(itemParams.state == State.New);
            completeIconGameObject.SetActive(itemParams.state == State.Complete);
            lockGameObject.SetActive(itemParams.state == State.NewWithAnimation);
            storyTitleText.text = ConvertTitleString(itemParams.storyData.name);
            storySubNameText.text = itemParams.storyData.subName;
            
            var chapterTotalProgressCount = itemParams.storyData.progressMax - itemParams.storyData.progressMin + 1;
            var chapterCurrentProgress = Mathf.Min(chapterTotalProgressCount, itemParams.currentProgress - itemParams.storyData.progressMin);
            storyDescText.text = $"{chapterCurrentProgress}/{chapterTotalProgressCount}";
            
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
            PageResourceLoadUtility.LoadAssetAsync<Sprite>(key: $"Images/StoryThumbnailChapter/story_thumbnail_chapter_{itemParams.storyData.id}.png",
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
        #endregion
        
        
        #region EventListeners
        public void OnClickItem()
        {
            itemParams?.onClick?.Invoke(itemParams);
        }
        #endregion
    }
}