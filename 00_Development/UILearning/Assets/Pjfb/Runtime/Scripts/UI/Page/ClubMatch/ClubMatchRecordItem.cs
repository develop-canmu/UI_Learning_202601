using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using TMPro;

namespace Pjfb.ClubMatch
{
    public class ClubMatchRecordItem : ColosseumRecordItem
    {
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text clubNameText;
        [SerializeField] private TMP_Text directionTypeText;
        [SerializeField] private GameObject[] challengeObjectArray;
        [SerializeField] private GameObject[] fightObjectArray;
        [SerializeField] private Image frameImage;
        [SerializeField] private TMP_Text createDateText;
        public override void Init(ItemParamsBase value)
        {
            base.Init(value);
            playerNameText.text = data.history.name;
            clubNameText.text = data.history.groupInfo.name;
            createDateText.text = data.history.createdAt.TryConvertToDateTime().ToString("yyyy/MM/dd HH:mm:ss");
            
            challengeObjectArray.ForEach(obj => obj.SetActive(false));
            fightObjectArray.ForEach(obj => obj.SetActive(false));
            
            var directionIndex = data.history.directionType - 1;
            var directionArray = directionIndex == 0 ? challengeObjectArray : fightObjectArray;
            var resultIndex = data.history.result - 1;
            directionArray[resultIndex].SetActive(true);
            
            var stringKey = directionIndex == 0 ? "pvp.challenge" : "pvp.fight_back";
            directionTypeText.text = StringValueAssetLoader.Instance[stringKey];
            directionTypeText.color = resultColor;
            frameImage.color = resultColor;
        }

    }
}