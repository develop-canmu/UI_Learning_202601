using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchResultScrollGridItem : ScrollGridItem
    {
        public class Arguments
        {
            private GroupLeagueMatchMatchHistory history = null;
            /// <summary>試合結果</summary>
            public GroupLeagueMatchMatchHistory History
            {
                get { return history; }
            }
            
            private ColosseumEventMasterObject mColosseumEvent = null;
            /// <summary>マスタ</summary>
            public ColosseumEventMasterObject MColosseumEvent
            {
                get { return mColosseumEvent; }
            }
            
            public Arguments(GroupLeagueMatchMatchHistory history, ColosseumEventMasterObject mColosseumEvent)
            {
                this.history = history;
                this.mColosseumEvent = mColosseumEvent;
            }
        }
        
        [SerializeField]
        private LeagueMatchResultView view = null;

        protected override void OnSetView(object value)
        {
            Arguments arguments = (Arguments)value;
            view.SetData(arguments.History, arguments.MColosseumEvent);
        }
    }
}