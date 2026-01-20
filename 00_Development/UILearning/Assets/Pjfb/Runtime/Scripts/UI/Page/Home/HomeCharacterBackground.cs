using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb.Home
{
    public class HomeCharacterBackground : MonoBehaviour
    {
        #region Parameters
        public class Parameters
        {
            public CharaMasterObject charaMasterObject;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private CharacterCardHomeBackgroundImage charaBackgroundImage;
        [SerializeField] private CharacterCardHomeTextImage charaTextImage;
        #endregion

        #region PublicMethods
        public async UniTask SetDisplay(Parameters parameters)
        {
            await charaBackgroundImage.SetTextureAsync(parameters.charaMasterObject.parentMCharaId);
            await charaTextImage.SetTextureAsync(parameters.charaMasterObject.parentMCharaId);
        }
        #endregion
    }
}
