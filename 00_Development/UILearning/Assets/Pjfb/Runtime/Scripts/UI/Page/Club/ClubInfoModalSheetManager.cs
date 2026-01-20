using System;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.Page;
using Cysharp.Threading.Tasks;

namespace Pjfb.Club {
    public enum ClubInfoModalSheetType {
        Member,
        History,
    }
    


    

    public class ClubInfoModalSheetParam {
        public ClubData data = null;
        public bool showUserProfileOtherButtons = true; 
        public bool showHeaderButtons = true;
        public Action onFinishedDissolution = null;
        public Action onFinishedSecession = null;
    }


    public class ClubInfoModalSheetManager : SheetManager<ClubInfoModalSheetType> {

    }

    
    public abstract class ClubInfoModalSheet : Sheet {
        protected ClubInfoModalSheetParam _param = null;
        
        protected abstract UniTask InitView( ClubInfoModalSheetParam param, ClubAccessLevel myAccessLevel, long myUserId, ClubInfoModal modal, Func<ClubAccessLevel, UniTask> updateViewRequest );

        public async UniTask Init( ClubInfoModalSheetParam param, ClubAccessLevel myAccessLevel, long myUserId, ClubInfoModal modal, Func<ClubAccessLevel,UniTask> updateViewRequest ){
            _param = param;
            await InitView(param, myAccessLevel, myUserId, modal, updateViewRequest);
        }


    }
}