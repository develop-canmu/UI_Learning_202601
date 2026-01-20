using System.Collections;
using System.Collections.Generic;
using CruFramework;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine.UI;
using CruFramework.Addressables;

namespace Pjfb.Training
{
    public class TrainingGetInspirationView : MonoBehaviour
    {
        [SerializeField]
        private TrainingInspirationIcon icon = null;
        [SerializeField]
        private TMPro.TMP_Text nameText = null;

        [SerializeField]
        private TMPro.TMP_Text valueText = null;
        
        [SerializeField]
        private OmissionTextSetter valueOmissionTextSetter = null;
        
        [SerializeField]
        private TMPro.TMP_Text countText = null;
        
        [SerializeField]
        private TrainingInspirationCellImage baseImage = null;
        
        [SerializeField]
        private RectTransform maskTransform = null;
        /// <summary>マスク</summary>
        public RectTransform MaskTransform{get{return maskTransform;}}
        
        [SerializeField]
        private RectTransform effectRoot = null;
        
        // 背景のエフェクト
        private GameObject cellEffect = null;

        private long inspirationId = 0;
        
        /// <summary>名前のTransform</summary>
        public RectTransform NameTransform{get{return (RectTransform)nameText.transform;}}
        
        public void SetInspiration(long id, bool isEffect)
        {
            inspirationId = id;
            // マスタ
            TrainingCardInspireMasterObject mCard = MasterManager.Instance.trainingCardInspireMaster.FindData(id);
            // アイコン
            icon.SetInspirationId(id);
            // 名前
            nameText.text = mCard.name;
            
            long effextIndex = mCard.grade-1;
            // 下地をグレードに合わせる
            baseImage.SetTexture(mCard.grade);
            
            // エフェクトの削除
            if(cellEffect != null)
            {
                GameObject.Destroy(cellEffect);
            }
            
            if(isEffect)
            {
                // エフェクトのキー
                string effectKey = ResourcePathManager.GetPath("InspirationCellEffect", mCard.grade);
                
                if(AddressablesManager.HasResources<GameObject>(effectKey))
                {
                    // エフェクトの再生
                    PageResourceLoadUtility.LoadAssetAsync<GameObject>( ResourcePathManager.GetPath("InspirationCellEffect", mCard.grade), (effect)=>
                        {
                            cellEffect = GameObject.Instantiate<GameObject>(effect, effectRoot);
                        },
                        gameObject.GetCancellationTokenOnDestroy()
                    ).Forget();
                }
            }
            
            
            // 効果値を取得
            List<PracticeSkillInfo> skills = PracticeSkillUtility.GetTrainingCardInspirationPracticeSkill(id);
            // スキルが１つだけのときは効果値を表示
            if(skills.Count == 1)
            {
                valueText.text = skills[0].ToValueName(valueOmissionTextSetter.GetOmissionData());
            }
            else
            {
                valueText.text = string.Empty;
            }
        }
        
        public void SetCount(long count)
        {
            if(countText != null)
            {
                countText.text = string.Format(StringValueAssetLoader.Instance["auto_training.summary.count"], count);
            }
        }
        
        /// <summary>効果テキストの表示</summary>
        public void SetEnableValueText(bool enable)
        {
            valueText.gameObject.SetActive(enable);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnClick()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.InspirationDetail, new TrainingInspirationDetailModal.Argument(inspirationId) );
        }
    }
}