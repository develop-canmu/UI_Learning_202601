using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class TutorialAwaitButtonCommand : TutorialCommandSetting.TutorialCommandData
    {
        [SerializeField] private string focusObjectId;
        
        public override async UniTask ActionCommand(CancellationToken token)
        {
            TutorialCommandTarget targetObject = await FindTarget(focusObjectId, token);
            UIButton button = targetObject.gameObject.GetComponent<UIButton>();
            await button.OnClickAsync(token);
        }
    }
}