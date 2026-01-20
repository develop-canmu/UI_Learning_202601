using System.Collections;
using CruFramework.UI;
using UnityEngine;

namespace Pjfb
{
    public class CharacterSkillScrollGrid : MonoBehaviour
    {
        [SerializeField] private ScrollGrid scrollGrid;
        [SerializeField] private GameObject noSkillText;
        
        public void SetItems(IList list)
        {
            noSkillText.SetActive(list.Count == 0);
            scrollGrid.SetItems(list);
        }

        public void Refresh()
        {
            scrollGrid.Refresh();
        }
    }
}
