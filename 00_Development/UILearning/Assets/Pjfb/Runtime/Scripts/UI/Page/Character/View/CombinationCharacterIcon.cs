using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
    public class CombinationCharacterIcon : MonoBehaviour
    {
        [SerializeField] private CharacterIcon characterIcon;
        [SerializeField] private GameObject noPossessionRoot;
        
        public void Initialize(CharacterDetailData characterDetailData, IReadOnlyDictionary<long, UserDataChara> mCharaIdPossessionDictionary, bool showCharaLevel)
        {
            // null の場合判定不要
            if (mCharaIdPossessionDictionary is null)
            {
                characterIcon.SetIcon(characterDetailData);
                noPossessionRoot.SetActive(false);
            }
            else
            {
                
                mCharaIdPossessionDictionary.TryGetValue(characterDetailData.MCharaId, out var userDataChara);
                                
                if (userDataChara is null)
                {
                    characterIcon.SetIcon(characterDetailData);
                    noPossessionRoot.SetActive(true);
                }
                else
                {
                    characterIcon.SetIcon( new CharacterDetailData(userDataChara.id, characterDetailData.MCharaId, characterDetailData.Lv, characterDetailData.LiberationLevel) );
                    noPossessionRoot.SetActive(false);
                }
                
                characterIcon.SetActiveRarity(true);
            }
            
            characterIcon.SetActiveLv(showCharaLevel);
        }
    }
}
