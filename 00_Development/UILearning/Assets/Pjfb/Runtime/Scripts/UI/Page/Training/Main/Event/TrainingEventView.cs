using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingEventView : MonoBehaviour
    {
        [SerializeField]
        private CharacterIcon characterIcon = null;
        [SerializeField]
        private TMPro.TMP_Text eventCategoryText = null;
        [SerializeField]
        private RubyTextMeshProUGUI eventNameText = null;
        
        
        public void SetEvent(long characterId, string eventCategory, string eventName)
        {
            // キャラアイコン
            if(characterId > 0)
            {
                characterIcon.gameObject.SetActive(true);
                characterIcon.SetImage(characterId);
            }
            else
            {
                characterIcon.gameObject.SetActive(false);
            }
            // イベントカテゴリ
            eventCategoryText.text = eventCategory;
            // イベント名
            eventNameText.UnditedText = eventName;
        }
    }
}