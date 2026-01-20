using System;
using Pjfb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.CustomHtml
{
    public class CustomHtmlObjectListItem : ListItemBase
    {
        #region ItemParam
        public class ItemParams : ItemParamsBase
        {
            public HtmlObjectParam objectParam;
            public Action<string> onClickButton;
        }
        #endregion

        #region SerializeFields
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI titleText; // <crz7-h>に囲まれたテキスト用
        [SerializeField] private GameObject titleParent;
        [SerializeField] private CancellableWebTexture bannerImage;
        [SerializeField] private GameObject button;
        [SerializeField] private TextMeshProUGUI buttonText;
        
        [SerializeField] private LayoutElement bannerImageLayoutElement;
        [SerializeField] private RawImage bannerRawImage;
        #endregion

        #region PrivateFields
        private ItemParams itemParams;
        private string _onClickStringValue = string.Empty;
        #endregion
        
        #region OverrideMethods
        public override void Init(ItemParamsBase itemParamsBase)
        {
            base.Init(itemParamsBase);
            itemParams = (ItemParams) itemParamsBase;
            UpdateDisplay(itemParams.objectParam);
        }
        #endregion

        #region PrivateMethods
        private void UpdateDisplay(HtmlObjectParam objectParam)
        {
            Reset();
            
            if (objectParam.nullableButton != null)
            {
                button.gameObject.SetActive(true);
                buttonText.text = objectParam.nullableButton.buttonText;
                _onClickStringValue = objectParam.nullableButton.onClickString;
            }
            else if (objectParam.nullableImage != null)
            {
                bannerImage.gameObject.SetActive(true);
                SetBanner(objectParam.nullableImage.imagePath);
                _onClickStringValue = objectParam.nullableImage.onClickString;
            }
            else if (objectParam.nullableText != null)
            {
                if (objectParam.nullableText.isTitle)
                {
                    titleText.text = objectParam.nullableText.text;
                    titleParent.SetActive(true);
                }
                else
                {
                    descriptionText.text = objectParam.nullableText.text;
                    descriptionText.gameObject.SetActive(true);
                }
            }
        }

        private async void SetBanner(string imagePath)
        {
            await bannerImage.SetTextureAsync($"{AppEnvironment.AssetBrowserURL}/{imagePath}");
            if (bannerRawImage.texture == null) return;
            
            bannerRawImage.texture.wrapMode = TextureWrapMode.Clamp;
            bannerRawImage.SetNativeSize();
            bannerImageLayoutElement.preferredHeight = bannerRawImage.mainTexture.height;
        }

        private void Reset()
        {
            bannerImage.gameObject.SetActive(false);
            titleParent.SetActive(false);
            descriptionText.gameObject.SetActive(false);
            button.SetActive(false);
            _onClickStringValue = string.Empty;
        }
        #endregion

        #region EventListener
        /// <summary>
        /// bannerImageとbuttonから呼ばれる
        /// </summary>
        public void OnClickButton()
        {
            itemParams.onClickButton.Invoke(_onClickStringValue);
        }
        #endregion
    }
}