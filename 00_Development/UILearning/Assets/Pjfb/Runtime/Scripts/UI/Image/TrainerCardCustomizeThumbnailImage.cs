using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb
{
    /// <summary>
    /// トレーナーカードの着せ替え背景のサムネイル画像
    /// </summary>
    public class TrainerCardCustomizeThumbnailImage : CancellableImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetTrainerCardCustomizeImagePath(id);
        }
    }
}