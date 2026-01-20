using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Club;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.RecommendChara;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class RecommendCharaScrollItem : ScrollGridItem
    {
        [SerializeField] private CharacterVariableIcon characterVariableIcon;
        [SerializeField] private TextMeshProUGUI userNameText;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI totalPowerText;
        [SerializeField] private OmissionTextSetter omissionTextSetter;
        [SerializeField] private ClubAccessLevelBadge clubAccessLevelBadge;
        [SerializeField] private MeBadge meBadge;
        [SerializeField] private TrainingScenarioNameView trainingScenarioNameView;

        private RecommendCharaData data;

        public void OnSelected()
        {
            TriggerEvent(data);
        }

        protected override void OnSetView(object value)
        {
            data = (RecommendCharaData)value;
            
            characterVariableIcon.SetIconTextureWithEffectAsync(data.CharacterVariableDetailData.MCharaId).Forget();
            characterVariableIcon.SetIcon(data.CharacterVariableDetailData);
            characterVariableIcon.SetActiveRoleNumberIcon(false);
            characterVariableIcon.SetActiveCombatPower(false);
            
            // マスタ
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(data.CharacterVariableDetailData.MCharaId);
            // キャラ名
            characterNameText.text = string.Format( StringValueAssetLoader.Instance["common.character_name"], mChar.nickname, mChar.name );

            // 戦力
            totalPowerText.text = data.CharacterVariableDetailData.CombatPower.ToDisplayString(omissionTextSetter.GetOmissionData());
            
            
            userNameText.text = data.UserName;
            
            clubAccessLevelBadge.UpdateBadge(data.ClubAccessLevel);
            clubAccessLevelBadge.gameObject.SetActive(data.WantShowClubAccessLevel);
            
            meBadge.SwitchBadge(data.IsMe());

            SetTrainingScenarioView(data.CharacterVariableDetailData);
        }

        private void SetTrainingScenarioView(CharacterVariableDetailData value)
        {
            if (!value.HasTrainingScenario())
            {
                trainingScenarioNameView.SetEmpty();
                return;
            }
            
            var scenarioData = MasterManager.Instance.trainingScenarioMaster.FindData(value.TrainingScenarioId);
            trainingScenarioNameView.SetValue(scenarioData);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnProfileButton()
        {
            TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(data.UMasterId, showOtherButtons:data.IsMe() == false);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }
    }
}
