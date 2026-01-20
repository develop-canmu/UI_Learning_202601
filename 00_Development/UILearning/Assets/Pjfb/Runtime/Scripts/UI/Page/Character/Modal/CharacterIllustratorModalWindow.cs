using System;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Master;

namespace Pjfb.Character
{
    public class CharacterIllustratorModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public WindowParams(long id, CardType cardType)
            {
                Id = id;
                CardType = cardType;
            }
            public long Id;
            public CardType CardType;
        }
        #endregion
        private WindowParams _windowParams;



        [SerializeField] private CharacterIllustratorScrollRect characterIllustratorScrollRect;
        [SerializeField] private CharacterCardHomeImage characterCardHomeImage;
        [SerializeField] private IconImage specialSupportCardImage;
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CharacterIllustrator, data);
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            await Init();
            await base.OnPreOpen(args, token);
        }

        #region PrivateMethods
        private async UniTask Init()
        {
           

            switch (_windowParams.CardType)
            {
                case CardType.Character:
                    characterIllustratorScrollRect.Initialize(characterCardHomeImage.GetComponent<RectTransform>());
                    specialSupportCardImage.gameObject.SetActive(false);
                    await characterCardHomeImage.SetTextureAsync(_windowParams.Id);
                    characterCardHomeImage.gameObject.SetActive(true);
                    break;
                case CardType.SpecialSupportCharacter:
                    characterIllustratorScrollRect.Initialize(specialSupportCardImage.GetComponent<RectTransform>());
                    characterCardHomeImage.gameObject.SetActive(false);
                    await specialSupportCardImage.SetTextureAsync(_windowParams.Id);
                    specialSupportCardImage.gameObject.SetActive(true);
                    break;
                case CardType.Adviser:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            characterIllustratorScrollRect.BottomButtonsAction = OnClickClose;
            Input.multiTouchEnabled = true;
        }
        #endregion

        #region EventListeners

        public void OnClickClose()
        {
            Input.multiTouchEnabled = false;
            Close();
        }
        #endregion
       
        
        
    }
}
