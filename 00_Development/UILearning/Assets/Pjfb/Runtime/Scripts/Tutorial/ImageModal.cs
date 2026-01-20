using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
namespace Pjfb
{
    public class ImageModal : ModalWindow
    {
        [SerializeField] private Image image;
        private CancellationTokenSource source;
        private string imagePath;
        
        private readonly ResourcesLoader resourcesLoader = new ResourcesLoader();
        public static async UniTask<CruFramework.Page.ModalWindow> OpenModalAsync(string imagePath, ModalOptions options = ModalOptions.None)
        {
            return await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Image, args: imagePath, options);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            imagePath = (string)args;
            image.gameObject.SetActive(false);
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            SetBanner().Forget();
            base.OnOpened();
        }

        private async UniTask SetBanner()
        {
            DisposeToken();
            source = new CancellationTokenSource();
            var sprite = await resourcesLoader.LoadAssetAsync<Sprite>(imagePath, source.Token);
            if (image == null || sprite == null)
            {
                return;
            }
            image.gameObject.SetActive(true);
            image.SetNativeSize();
            image.sprite = sprite;
            source = null;
        }

        private void DisposeToken()
        {
            if(source != null)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
        }

        protected override void OnClosed()
        {
            DisposeToken();
            resourcesLoader.Release();
            base.OnClosed();
        }

        public void OnClickClose()
        {
            Close();
        }
    }
}