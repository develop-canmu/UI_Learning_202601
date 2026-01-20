using System;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

using Pjfb.Master;

namespace Pjfb.Character
{
    public class CharacterSkillModal : ModalWindow
    {
        #region Params

        public class WindowParams
        {
            public long id;
            public long level;
            public Action onClosed;
        }

        #endregion
        private WindowParams _windowParams;

        [SerializeField] private CharacterDetailSkillView characterDetailSkillView;
        
        
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CharacterSkill, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        #region PrivateMethods
        private void Init()
        {
            long id = _windowParams.id;
            long level = _windowParams.level;
            characterDetailSkillView.SetSkillId(id, level);
        }
        #endregion

        #region EventListeners

        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }
        #endregion
       
        
        
    }
}
