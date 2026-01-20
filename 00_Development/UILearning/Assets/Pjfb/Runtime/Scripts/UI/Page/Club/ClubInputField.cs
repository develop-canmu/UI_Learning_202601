using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb;

namespace Pjfb.Club
{
    
    public class ClubInputField : MonoBehaviour {
        
        public TMP_InputField inputField => _inputField;
        public int characterLimit => _characterLimit;
        public string text{ 
            get{
                return inputField.text;
            }
            set {
                inputField.text = value;
            }
        } 
        

        public System.Action<string> onUpdateText{get;set;} = null;

        [SerializeField] 
        private TMP_InputField _inputField = null;
        [SerializeField] 
        private int _characterLimit = 0;
        [SerializeField] 
        private bool _isSingleLine = true;

        [SerializeField] 
        private bool _isName = false;
        [SerializeField] 
        private UnityEvent<string> onEndEdit = new UnityEvent<string>();



        void Awake(){
            _inputField.onValidateInput = OnValidateInput;
            _inputField.onEndEdit.AddListener(OnEndEdit);
        }

        void OnDestroy() {
            if( _inputField != null ) {
                _inputField.onValidateInput = null;
                _inputField.onEndEdit.RemoveListener(OnEndEdit);
            }
        }

        public char OnValidateInput( string input, int charIndex, char addedChar ){
            var str = StringUtility.OnValidateInput(input, charIndex, addedChar, int.MaxValue, _inputField.fontAsset,removeOtherSymbol:true);
            return str;
        }

        public void OnEndEdit( string str ){
            var limit = _characterLimit;
            if( limit <= 0  ) {
                limit = int.MaxValue;
            }
            if( _isName ) {
                _inputField.text = StringUtility.GetLimitNumUserName(_inputField.text, limit);
            } else {
                var inputStr = _inputField.text;
                if( _isSingleLine ) {
                    inputStr = inputStr.RemoveLineEnd();
                }
                if( string.IsNullOrEmpty( inputStr.RemoveBeginAndEndSpace() ) ) {
                    inputStr = "";
                }
                inputStr = StringUtility.RemoveRichTextTag(StringUtility.RemoveEmojiAndCombiningCharacter(inputStr,true));
                if (inputStr.Length > limit){
                    inputStr = inputStr.Substring(0, limit);
                }
                _inputField.text = inputStr;
            }
            
            onUpdateText?.Invoke(_inputField.text);
            onEndEdit?.Invoke(_inputField.text);
        }

        public void OnClickInputField(){
            _inputField.ActivateInputField();
        }
    }
}
