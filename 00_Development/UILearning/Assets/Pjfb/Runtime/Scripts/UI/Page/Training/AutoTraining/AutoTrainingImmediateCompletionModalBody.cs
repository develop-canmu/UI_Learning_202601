using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTrainingImmediateCompletionModalBody : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text messageText = null;
        [SerializeField]
        private PossessionItemUi possessionItem = null;
        
        /// <summary>メッセージ</summary>
        public void SetMessage(string msg)
        {
            messageText.text = msg;
        }
        
        /// <summary>アイテム数</summary>
        public void SetItemCount(long pointId, long before, long after)
        {
            possessionItem.SetAfterCount(pointId, before, after);
        }
        
        /// <summary>赤色にする</summary>
        public void SetShortageColor()
        {
            possessionItem.SetShortageColor();
        }
    }
}