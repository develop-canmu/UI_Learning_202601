using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandSetText : IAdvCommand
    {
        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.Texts))]
        [AdvDocument("表示するテキストId。")]
        private int textObjectId = 0;
        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.StringDatas))]
        [AdvDocument("表示する文字列。")]
        private int stringId = 0;
        
        [SerializeField]
        [AdvDocument("テキストの色。")]
        private Color color = Color.white;
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            AdvTextObject text = manager.GetText(textObjectId);
            if(text == null)return;
            
            if(stringId == 0)
            {
                text.gameObject.SetActive(false);
            }
            else
            {
                text.gameObject.SetActive(true);
                text.SetText(manager.Config.StringDatas[stringId].Name, color);
            }            
        }
    }
}
