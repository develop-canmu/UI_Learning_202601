using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using CruFramework;
using CruFramework.UI;
using TMPro;


namespace Pjfb.Club
{
    public class MemberTopClubInformation : MonoBehaviour {
        [SerializeField]
        ClubEmblemImage _emblem = null;
        [SerializeField]
        ClubRankImage _rank = null;
        [SerializeField]
        TextMeshProUGUI _clubName = null;
        [SerializeField]
        TextMeshProUGUI _memberQuantity = null;
        [SerializeField]
        TextMeshProUGUI _power = null;
        [SerializeField]
        OmissionTextSetter _powerOmissionTextSetter = null;


        public async UniTask UpdateGuildView( ClubData status ){
            _clubName.text = status.name;
            _memberQuantity.text = ClubUtility.CreateMemberQuantityString( status.memberList.Count );
            _power.text = status.power.ToDisplayString( _powerOmissionTextSetter.GetOmissionData());
            await _emblem.SetTextureAsync(status.emblemId);
            await _rank.SetTextureAsync(status.rankId);
        }
        
    }
}
