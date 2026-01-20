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
    public class MemberTopMatchSchedule : MonoBehaviour {
        [SerializeField]
        TextMeshPro _matchType = null;
        [SerializeField]
        TextMeshPro _date = null;
        [SerializeField]
        TextMeshPro _enemyName = null;
        [SerializeField]
        TextMeshPro _result = null;

        public void UpdateView( GuildBattleMatchingMatchingStatus status ) {
            
            _date.text = status.startAt;
            _enemyName.text = status.opponentGuildName;
            
            //todo サーバー待ちで固定
            _matchType.text = "練習試合";
            _result.text = "WIN";
        }
    }
}
