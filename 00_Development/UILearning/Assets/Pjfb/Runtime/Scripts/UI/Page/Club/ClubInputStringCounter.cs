using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;


namespace Pjfb.Club
{
    
    public class ClubInputStringCounter : MonoBehaviour {
        
        [SerializeField] 
        private ClubInputField _inputField = null;
        [SerializeField]
        private TextMeshProUGUI _counterText = null;
        void Start(){
            _inputField.onUpdateText = UpdateCounter;
            UpdateCounter(_inputField.inputField.text);
            
        }

        void UpdateCounter( string str ){ 
            _counterText.text = str.Length + "/" + _inputField.characterLimit.ToString();
        }

    }
}
