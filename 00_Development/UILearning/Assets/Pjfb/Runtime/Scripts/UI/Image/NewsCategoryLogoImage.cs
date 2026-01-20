using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.News
{
    [RequireComponent(typeof(Image))]
    public class NewsCategoryLogoImage : CancellableImageWithEnum<NewsCategories>
    {
        protected override string GetAddress(NewsCategories type)
        {
            return NewsManager.CategoryLogoImageNameDictionary.TryGetValue(type, out var val) ? val :
                    NewsManager.CategoryLogoImageNameDictionary[NewsCategories.None];
        }

        public async override UniTask SetTextureAsync(NewsCategories type)
        {
            var key = GetAddress(type);

            if (string.IsNullOrEmpty(key)) gameObject.SetActive(false);
            else
            {
                gameObject.SetActive(true);
                await SetTextureAsync($"Images/UI/News/Images/{key}.png");
            }
        }
    }
}
