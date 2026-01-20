using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;

using CruFramework.UI;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Training;
using TMPro;
using UnityEngine.UI;

namespace Pjfb
{

    public class CharacterDetailComboView : MonoBehaviour
    {
    
        [SerializeField]
        private TMPro.TMP_Text nameText = null;
        [SerializeField]
        private GameObject labelObject = null;

        // 条件達成ラベルテキスト
        [SerializeField]
        private TMP_Text conditionArchiveLabelText = null;
        
        [SerializeField]
        private GameObject minLevelObject = null;
        [SerializeField]
        private TMPro.TMP_Text minLevelText = null;
        
        public void SetName(string name)
        {
            nameText.text = name;
        }
        
        public void ShowLabel(bool show)
        {
            labelObject.SetActive(show);
        }

        /// <summary> 条件達成ラベル文言セット </summary>
        public void SetConditionArchiveLabelText(string text)
        {
            conditionArchiveLabelText.text = text;
        }

        public void ShowMinLevel(string minLevelCondition)
        {
            minLevelObject.SetActive(true);
            minLevelText.text = minLevelCondition;
        }
    }
}