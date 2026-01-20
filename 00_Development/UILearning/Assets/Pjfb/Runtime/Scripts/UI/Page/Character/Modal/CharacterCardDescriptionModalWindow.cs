using Cysharp.Threading.Tasks;
using Pjfb.Master;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    public class CharacterCardDescriptionModalWindow : ModalWindow
    {
        [SerializeField] TextMeshProUGUI descriptionText;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            CharaMasterObject mChara = (CharaMasterObject)args;
            descriptionText.text = mChara.description;
            return base.OnPreOpen(args, token);
        }


        #region EventListeners
        public void OnClickClose()
        {
            Close();
        }
        #endregion
    }
}