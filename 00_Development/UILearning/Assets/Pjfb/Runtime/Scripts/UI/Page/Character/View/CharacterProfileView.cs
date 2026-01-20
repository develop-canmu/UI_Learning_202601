using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using TMPro;

namespace Pjfb
{
    public class CharacterProfileView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI englishNameText;
        [SerializeField] private TextMeshProUGUI cvText;
        [SerializeField] private TextMeshProUGUI birthdayText;
        [SerializeField] private TextMeshProUGUI heightText;
        // 一旦なし
        //[SerializeField] private TextMeshProUGUI weightText;
        //[SerializeField] private TextMeshProUGUI specialityText;
        [SerializeField] private TextMeshProUGUI selfIntroductionText;

        public void InitializeUI(CharaParentMasterObject mCharaParent)
        {
            SetText(nameText, mCharaParent.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.Name));
            SetText(englishNameText, mCharaParent.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.EnglishName));
            SetText(cvText, 　string.Format(StringValueAssetLoader.Instance["character.profile.cv"], mCharaParent.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.CV)));
            SetText(birthdayText, mCharaParent.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.Birthday));
            SetText(heightText, mCharaParent.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.Height));
            // 一旦なし
            //SetText(weightText, mCharaParent.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.Weight));
            //SetText(specialityText, mCharaParent.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.Speciality));
            SetText(selfIntroductionText, mCharaParent.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.SelfIntroduction));
        }
        
        
        public void InitializeUI(CharaMasterObject mChara)
        {
            SetText(nameText, mChara.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.Name));
            SetText(englishNameText, mChara.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.EnglishName));
            SetText(cvText, mChara.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.CV));
            SetText(birthdayText, mChara.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.Birthday));
            SetText(heightText, mChara.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.Height));
            // 一旦なし
            //SetText(weightText, mChara.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.Weight));
            //SetText(specialityText, mChara.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.Speciality));
            SetText(selfIntroductionText, mChara.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.SelfIntroduction));
        }

        private void SetText(TextMeshProUGUI textObject, string content)
        {
            if(textObject == null)  return;
            textObject.text = content ?? string.Empty;
        }
    }
}