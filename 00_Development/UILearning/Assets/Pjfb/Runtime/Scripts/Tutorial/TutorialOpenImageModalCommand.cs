using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class TutorialOpenImageModalCommand : TutorialCommandSetting.TutorialCommandData
    {
        [SerializeField] private string imagePath;
        private const string TUTORIAL_BANNER_KEY = "TutorialBanner";
        
        public override async UniTask ActionCommand(CancellationToken token)
        {
            imagePath = ResourcePathManager.GetPath(TUTORIAL_BANNER_KEY, imagePath);
            var window = await ImageModal.OpenModalAsync(imagePath);
            await window.WaitCloseAsync(token);
        }
    }
}