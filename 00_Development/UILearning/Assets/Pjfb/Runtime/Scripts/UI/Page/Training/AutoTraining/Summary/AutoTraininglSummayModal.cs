using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTraininglSummayModal : ModalWindow
    {
        
        [SerializeField]
        private CharacterVariableIcon characterIcon = null;
        
        [SerializeField]
        private TMP_Text nickNameText = null;
        [SerializeField]
        private TMP_Text nameText = null;
        [SerializeField]
        private TMP_Text scenarioNameText = null;
        
        
        [SerializeField]
        private AutoTrainingSummaryTurnView turnView = null;
        [SerializeField]
        private AutoTrainingSummaryStatusView statusView = null;
        [SerializeField]
        private AutoTrainingSummaryTrainingLvView trainingLvView = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            TrainingMainArguments arguments = (TrainingMainArguments)args;
            // 結果
            TrainingAutoResultStatus result = arguments.AutoTrainingResult;
            
            // MChar
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(arguments.TrainingCharacter.MCharId);
            // MTrainingScenario
            TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData( arguments.Pending.mTrainingScenarioId );
            
            // アイコン
            UserDataCharaVariable uChar = UserDataManager.Instance.charaVariable.Find(arguments.CharacterVariable.uCharaVariableId); 
            characterIcon.SetIcon(uChar);
            characterIcon.SetIconTextureWithEffectAsync(uChar.MChara.id).Forget();
            
            // ニックネーム
            nickNameText.text = mChar.nickname;
            // 名前
            nameText.text = mChar.name;
            // シナリオ名
            scenarioNameText.text = mScenario.name;
            
            // ターン情報
            turnView.SetData(arguments, result.turnList);
            // ステータス
            statusView.SetData(arguments, result);
            // トレーニングLv
            if(TrainingUtility.IsEnableTrainingLv(mScenario.id))
            {
                trainingLvView.gameObject.SetActive(true);
                trainingLvView.SetData(arguments);
            }
            else
            {
                trainingLvView.gameObject.SetActive(false);
            }
            
            
            return base.OnPreOpen(args, token);
        }
    }
}