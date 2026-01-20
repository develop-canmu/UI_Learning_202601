using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTrainingStatusPolicySettingModal : ModalWindow
    {
        [SerializeField]
        private CharacterIcon characterIcon = null;
        [SerializeField]
        private TMP_Text nickNameText = null;
        [SerializeField]
        private TMP_Text nameText = null;
        
        [SerializeField]
        private AutoTrainingStatusPolicyStatusButton[] statusButtons = null;
        
        
        private AutoTrainingStatusPolicyStatusButton selectedButton = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            TrainingPreparationArgs arguments = (TrainingPreparationArgs)args;
            
            UserDataChara uChar = UserDataManager.Instance.chara.Find(arguments.TrainingUCharId);
            // アイコン
            characterIcon.SetIcon( uChar.charaId, uChar.level, uChar.newLiberationLevel );
            // 名前
            nickNameText.text = uChar.MChara.nickname;
            nameText.text = uChar.MChara.name;
            
            
            // 初期選択
            for(int i=0;i<statusButtons.Length;i++)
            {
                if(statusButtons[i].Id == arguments.AutoTrainingPolity)
                {
                    OnSelectStatus(statusButtons[i]);
                }
            }
            
            // 初期選択されなかった場合は０番目のボタンを選択
            if(selectedButton == null)
            {
                OnSelectStatus(statusButtons[0]);
            }
            
            return base.OnPreOpen(args, token);
        }

        /// <summary>UGUI</summary>
        public void OnSelectStatus(AutoTrainingStatusPolicyStatusButton button)
        {
            // 選択中のボタンを解除
            if(selectedButton != null)
            {
                selectedButton.SetSelect(false);
            }
            // 選択中のボタンを設定
            selectedButton = button;
            selectedButton.SetSelect(true);
        }
        
        /// <summary>UGUI</summary>
        public void OnNextButton()
        {
            // AutoTrainingTODO : 選択したステータスのIdを返す
            SetCloseParameter(selectedButton.Id);
            // 閉じる
            Close();
        }
    }
}