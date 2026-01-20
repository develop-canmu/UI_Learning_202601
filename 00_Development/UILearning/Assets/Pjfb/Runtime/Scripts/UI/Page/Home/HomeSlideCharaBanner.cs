using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using Pjfb.Voice;
using UnityEngine;

namespace Pjfb.Home
{
    public class HomeSlideCharaBanner : ScrollGridItem
    {
        #region Parameters
        public class Parameters
        {
            public readonly CharaMasterObject charaMasterObject;
         
            public long rarity { get; }
            public Parameters(CharaMasterObject charaMasterObject)
            {
                this.charaMasterObject = charaMasterObject;
                // 基本レアリティ
                var rarityId = MasterManager.Instance.charaMaster.FindData(charaMasterObject.id).mRarityId;
                rarity = MasterManager.Instance.rarityMaster.FindData(rarityId).value;
            }
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private CharacterCardHomeImage characterCardHomeImage;
        /// <summary>キャラ画像</summary>
        public CharacterCardHomeImage CharacterCardHomeImage
        {
            get { return characterCardHomeImage; }
        }
        
        [SerializeField] private RectTransform parent;
        #endregion
        
        #region PrivateFields
        private Parameters parameters;
        #endregion
        
        #region OverrideMethods
        protected override void OnSetView(object value)
        {
            parameters = (Parameters) value;

            parent.anchoredPosition = parameters.rarity > 1 ? new Vector2(0, 132) : Vector2.zero;
        }

        #endregion

        #region EventListener
        public void OnClickCharacterImage()
        {
            VoiceManager.Instance.StopVoice();
            VoiceManager.Instance.PlaySystemVoiceForLocationTypeAsync(parameters.charaMasterObject, VoiceResourceSettings.LocationType.SYSTEM_TAP).Forget();
        }
        #endregion
    }
}