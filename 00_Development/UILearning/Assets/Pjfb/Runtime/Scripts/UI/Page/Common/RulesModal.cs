using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class RulesModal : ModalWindow
    {
        [SerializeField]
        private TextMeshProUGUI rulesText = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            rulesText.text = (string)args;
            return base.OnPreOpen(args, token);
        }
    }
}
