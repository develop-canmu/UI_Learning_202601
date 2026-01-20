using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.ClubMatch
{
    public class HomeEndOfSeasonClubMatchPrizeIcon : ScrollGridItem
    {
        [SerializeField] private Image iconImage;
        public Master.PrizeJsonWrap data;

        protected override async void OnSetView(object value)
        {
            data = value as Master.PrizeJsonWrap;
            if (data == null) return;
            await Init(data);
        }

        public async UniTask Init(Master.PrizeJsonWrap value)
        {
            // TODO: トロフィー追加
            switch (value.type)
            {
                case "point":
                    string path = PageResourceLoadUtility.GetItemIconImagePath(value.args.mPointId);
                    await PageResourceLoadUtility.LoadAssetAsync<Sprite>(path, sprite => iconImage.sprite = sprite, this.GetCancellationTokenOnDestroy());
                    break;
                case "guildEmblem":
                    await ClubUtility.LoadAndSetEmblemIcon(iconImage, value.args.mGuildEmblemId);
                    break;
            }
            gameObject.SetActive(true);
        }
    }
}