using System;
using CruFramework.Page;
using Cysharp.Threading.Tasks;



namespace Pjfb.Club {
    public class FindMemberSheetView : Sheet {

        protected ClubData _clubData = null;
        protected ClubAccessLevel _myAccessLevel = ClubAccessLevel.None;
        protected FindMemberPage _page = null;
        protected System.Action<FindMemberSheetView> _onFinishedInit = null;
        
        public void Init( ClubData data, ClubAccessLevel accessLevel, FindMemberPage page, System.Action<FindMemberSheetView> onFinishedInit ){
            _clubData = data;
            _myAccessLevel = accessLevel;
            _page = page;
            _onFinishedInit = onFinishedInit;
            InitView();
        }

        protected virtual void InitView(){
        }
    }
}