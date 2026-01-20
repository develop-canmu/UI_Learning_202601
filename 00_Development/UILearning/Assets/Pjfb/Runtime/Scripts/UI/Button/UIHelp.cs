using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Menu;

namespace Pjfb
{
    public class UIHelp : MonoBehaviour
    {
        [SerializeField]
        [StringValue]
        private string stringId = string.Empty;
        public void OnOpenHelpModal()
        {
            string category = (string.IsNullOrEmpty(stringId)) ? "" : StringValueAssetLoader.Instance[stringId];
            HelpModalWindow.Open(new HelpModalWindow.WindowParams
            {
                categoryList = new List<string>{category}
            });
        }

        /// <summary> ヘルプで開くカテゴリを変更 </summary>
        public void SetHelpCategory(string category)
        {
            stringId = category;
        }
    }
}