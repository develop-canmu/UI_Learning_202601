using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class TutorialFocusCommand : TutorialCommandSetting.TutorialCommandData
    {
        [SerializeField] private string focusObjectId;
        [SerializeField] private FocusObjectType focusType = FocusObjectType.Default;
        [SerializeField] private int valueId;
        public override async UniTask ActionCommand(CancellationToken token)
        {
            TutorialCommandTarget targetObject = await FindTarget(focusObjectId,token);
            
            switch (focusType)
            {
                case FocusObjectType.Character:
                    // スクロールを一時的に無効化
                    var scrollGrid = targetObject.GetComponent<ScrollGrid>();
                    bool useScroll = scrollGrid.Interactable;
                    scrollGrid.Interactable = false;
                    
                    var characterObjects = targetObject.GetComponentsInChildren<CharacterScrollItem>();
                    GameObject target = characterObjects.FirstOrDefault(icon => valueId == icon.characterData.CharacterId)?.gameObject;
                    await AppManager.Instance.TutorialCommandManager.FocusTarget(target, token);
                    scrollGrid.Interactable = useScroll;
                    break;
                case FocusObjectType.Default:
                    await AppManager.Instance.TutorialCommandManager.FocusTarget(targetObject.gameObject, token);
                    break;
            }

        }
    }
}