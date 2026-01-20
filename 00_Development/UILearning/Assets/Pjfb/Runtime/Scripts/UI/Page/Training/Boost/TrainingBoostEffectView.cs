using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine.UI;

namespace Pjfb
{
    public class TrainingBoostEffectView : MonoBehaviour
    {
        [SerializeField]
        private CancellableImage baseImage = null;
        [SerializeField]
        private TMPro.TMP_Text nameText = null;
        [SerializeField]
        private OmissionTextSetter nameOmissionSetter = null;
        [SerializeField]
        private CharacterIcon icon = null;

        /// <summary>
        /// ブースト効果表示
        /// </summary>
        /// <param name="mTrainingPointStatusEffectId">効果ID</param>
        /// <param name="characterData">キャラデータ</param>
        /// <param name="isShowCharaInfo">キャラ情報を表示するか</param>
        public void Set(long mTrainingPointStatusEffectId, TrainingCharacterData? characterData = null, bool isShowCharaInfo = true)
        {
            // マスタ
            TrainingPointStatusEffectMasterObject mEffect = MasterManager.Instance.trainingPointStatusEffectMaster.FindData(mTrainingPointStatusEffectId);
            // イメージ
            baseImage.SetTexture($"Images/UI/Training/BoostBase/training_base_boostcell_{mEffect.imageId}.png");
            // 説明文
            nameText.text = mEffect.GetDisplayDescription(nameOmissionSetter.GetOmissionData());
            
            // 有効なキャラデータがあるか
            bool showIcon = characterData.HasValue && characterData.Value.MCharId > 0;
            icon.gameObject.SetActive(showIcon);
            
            // キャラデータの表示方法分岐
            if (showIcon && isShowCharaInfo)
            {
                // キャラデータを普通に表示する
                icon.SetIconAsync(characterData.Value.MCharId, characterData.Value.Lv, characterData.Value.LiberationId).Forget();
                // レベルは非表示
                icon.SetActiveLv(false);
            }
            else if (showIcon)
            {
                // サムネだけ表示する
                icon.SetIconTextureWithEffectAsync(characterData.Value.MCharId).Forget();
                icon.SetActiveLv(false);
                icon.SetActiveCharacterTypeIcon(false);
                icon.SetActiveRarity(false);
            }
        }
    }
}