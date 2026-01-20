using System;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.Page;
using Cysharp.Threading.Tasks;

namespace Pjfb.EventRanking {
    public enum EventRankingModalSheetType {
        Club,
        InClub,
    }
    

    public class EventRankingModalSheetManager : SheetManager<EventRankingModalSheetType> {
        [SerializeField]
        EventRankingModalSheet[] _sheet = null;
        public void SetParam(Pjfb.Club.ClubData clubData, long pointId) {
            foreach(var sheet in _sheet){
                sheet.SetParam( clubData, pointId );
            }
        }
    }

    
    public abstract class EventRankingModalSheet : Sheet {

        protected Pjfb.Club.ClubData clubData {get;set;}
        protected long pointId {get;set;}
        public void SetParam(Pjfb.Club.ClubData clubData, long pointId) {
            this.clubData = clubData;
            this.pointId = pointId;
        }

    }
}