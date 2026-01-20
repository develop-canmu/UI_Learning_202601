using UnityEngine;
using Pjfb.Master;

namespace Pjfb
{
    [RequireComponent(typeof(SliceableIconImage))]
    public class TrainingActionCardCharaView : MonoBehaviour
    {
        private static readonly int TrainingCharacterHash = Animator.StringToHash("TrainingCharacter");
        private static readonly int SupportCharacterHash = Animator.StringToHash("SupportCharacter");
        
        [SerializeField]
        private Canvas effectCanvas = null;
        /// <summary>エフェクト用のキャンバス</summary>
        public Canvas EffectCanvas{get{return effectCanvas;}}
        
        [SerializeField]
        private GameObject extraRoot = null;
        /// <summary>Ex表示</summary>
        public GameObject ExtraRoot{get{return extraRoot;}}
        
        private SliceableIconImage iconImage;
        private SliceableIconImage IconImage => iconImage ? iconImage : iconImage = GetComponent<SliceableIconImage>();

        private Animator animator;
        private Animator Animator => animator ? animator : animator = GetComponent<Animator>();

        public void SetCharaId(long id)
        {
            IconImage.SetTexture(id);
            
            // mChar
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(id);
            // Ex
            extraRoot.SetActive(mChar.isExtraSupport);
        }

        public void SetCharacterType(bool isTrainingCharacter)
        {
            if (isTrainingCharacter)
            {
                Animator.SetTrigger(TrainingCharacterHash);
            }
            else
            {
                Animator.SetTrigger(SupportCharacterHash);
            }
        }
    }
}
