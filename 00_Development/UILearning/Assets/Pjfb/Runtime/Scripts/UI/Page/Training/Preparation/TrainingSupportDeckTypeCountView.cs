using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Training
{
    public class TrainingSupportDeckTypeCountView : MonoBehaviour
    {
        [SerializeField]
        private CharacterType type = CharacterType.None;
        /// <summary>タイプ</summary>
        public CharacterType Type{get{return type;}}
        
        [SerializeField]
        private TMP_Text countText = null;
        
        /// <summary>数の表示</summary>
        public void SetCount(int count)
        {
            countText.text = string.Format(StringValueAssetLoader.Instance["deck.type_count"], count);
        }
    }
}