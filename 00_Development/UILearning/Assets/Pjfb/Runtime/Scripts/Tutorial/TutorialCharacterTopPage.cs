using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;

namespace Pjfb
{
    public class TutorialCharacterTopPage : CharacterTopPage
    {
        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) {
                switch(type) {
                    case PageManager.MessageType.EndFade:
                        AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
                        break;
                }
            }
            return base.OnMessage(value);
        }
    }
}