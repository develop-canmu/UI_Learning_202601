using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;

namespace Pjfb
{
    public class CombinationCharacterVariableIcon : MonoBehaviour
    {
        [SerializeField] private CharacterVariableIcon characterVariableIcon;
        [SerializeField] private GameObject activatorRoot;
        [SerializeField] private CharacterRarity characterRarity;
        
        public void Initialize(long mCharaId, bool isActivator)
        {
            // todo : ui確認
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
            characterVariableIcon.SetIconTextureWithEffectAsync(mCharaId).Forget();
            characterRarity.SetBaseRarity(mChara.mRarityId);
            activatorRoot.SetActive(isActivator);
        }
    }
}