using Pjfb.Master;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    public class TrainingScenarioNameView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;

        public void SetValue(TrainingScenarioMasterObject value)
        {
            nameText.text = value.name;
        }

        public void SetEmpty()
        {
            nameText.text = StringValueAssetLoader.Instance["character.empty_training_scenario"];
        }
    }
}
