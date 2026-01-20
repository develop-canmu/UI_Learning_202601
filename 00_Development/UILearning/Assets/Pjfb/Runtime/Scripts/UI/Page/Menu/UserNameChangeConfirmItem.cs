using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using Pjfb.UserData;
using Pjfb.Master;

namespace Pjfb
{
    public class UserNameChangeConfirmItem : MonoBehaviour
    {
        [SerializeField] private IconImage useImage = null;
        [SerializeField] private TextMeshProUGUI necessaryCountText = null;
        [SerializeField] private TextMeshProUGUI messageText = null;
        [SerializeField] private IconImage iconImage = null;
        [SerializeField] private TextMeshProUGUI beforeCountText = null;
        [SerializeField] private TextMeshProUGUI afterCountText = null;
        [SerializeField] private bool isEnoughUI;
        public void Init(PointMasterObject pointMaster,long pointValue ,long itemCount)
        {
            
            // ポイント画像セット
            useImage.SetTexture(pointMaster.id);
            iconImage.SetTexture(pointMaster.id);
            necessaryCountText.text = pointValue.ToString();

            //メッセージ
            if (isEnoughUI) 
            {
                beforeCountText.text = itemCount.ToString();
                afterCountText.text = (itemCount - pointValue).ToString();
                messageText.text = string.Format(StringValueAssetLoader.Instance["usernamechange.useitemtext"],
                     pointMaster.name,
                     pointValue
                     );
            }
            else
            {
                afterCountText.text = itemCount.ToString();
                messageText.text = messageText.text = string.Format(StringValueAssetLoader.Instance["usernamechange.notenoughttext"],pointMaster.name);
            }
        }
    }
}