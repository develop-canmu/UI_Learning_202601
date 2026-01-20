using Pjfb.Master;
using TMPro;
using UnityEngine;

namespace Pjfb.Menu
{
    public class PushSettingToggle : MonoBehaviour
    {
        public UISlideToggle toggle;
        public TextMeshProUGUI optionName;
        public GameObject borderObj;
        public long pushSettingType;

        public void Init(PushMasterObject pushSetting)
        {
            toggle.isOn = pushSetting.sentDefault;
            optionName.text = pushSetting.name;
            pushSettingType = pushSetting.id;
            gameObject.SetActive(true);
        }
    }
}