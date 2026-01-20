using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;
using TMPro;

namespace Pjfb.Club
{
    public class ClubCharacterIcon : MonoBehaviour {
        
        [SerializeField]
        UserIcon _character = null;
        [SerializeField]
        Image _captain = null;
        [SerializeField]
        Image _subCaptain = null;
        
        public void UpdateIcon( long iconId ){
            _character.SetIconId(iconId);
        }

        public void UpdateBadge( long roleId ){
            var accessLevel = ClubUtility.CreateAccessLevel( roleId ); 
            UpdateBadge(accessLevel);
        }

        public void UpdateBadge( ClubAccessLevel level ){
            _captain.gameObject.SetActive(level == ClubAccessLevel.Master);
            _subCaptain.gameObject.SetActive(level == ClubAccessLevel.SubMaster);
        }

        public void HideBadge( long roleId ){
            _captain.gameObject.SetActive(false);
            _subCaptain.gameObject.SetActive(false);
        }
    }
}