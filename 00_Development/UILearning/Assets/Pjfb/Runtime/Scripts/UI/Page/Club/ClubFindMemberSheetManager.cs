using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.Page;

namespace Pjfb.Club {
    public enum ClubFindMemberSheetType {
        FindMember,
        RequestJoin,
        Soliciting
    }
    

    public class ClubFindMemberSheetManager : SheetManager<ClubFindMemberSheetType> {
    }
}