using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.Page;

namespace Pjfb.Club {
    public enum FindClubPageTabSheetType {
        FindClub,
        SendRequest,
        SolicitationList,
        CreateClub
    }
    

    public class FindClubPageTabSheetManager : SheetManager<FindClubPageTabSheetType> {
        public ClubPage pageManager{get; private set;}

        public void Init( ClubPage page ){
            pageManager = page;
        }


    }
}