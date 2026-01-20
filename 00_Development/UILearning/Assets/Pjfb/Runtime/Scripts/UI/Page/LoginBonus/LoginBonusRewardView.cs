using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using Pjfb.Master;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.LoginBonus
{
    public class LoginBonusRewardView : ScrollGridItem
    {
        #region Parameters
        public class Info
        {
            public int index;
            public int startDay; 
            public long currentDay;
            public long count;
            public Func<bool> IsShowEnd;
            public long stampId;
        }
        
        const int oneLineCount = 4;
        const int shotTwoLineCount = 6;
        const int shotTwoLineUpperCount = 3;
        const int twoLineUpperCount = 4;
        
        [SerializeField] Transform _upperRewardList;
        [SerializeField] Transform _lowerRewardList;
        [SerializeField] List<LoginBonusListItem> _items;

        Vector3 _defaultUpperRewardListPosition = Vector3.zero;
        public Info info = new Info();
        #endregion
        
        protected override void OnSetView(object value)
        {
            info = value as Info;
            if(info == null) return;
            CreateItems();
        }

        void CreateItems() {
            _items.ForEach(item => item.gameObject.SetActive(false));
            _lowerRewardList.gameObject.SetActive(info.count > oneLineCount);
            if( info.count <= oneLineCount ) {
                _upperRewardList.transform.localPosition = new Vector3(_defaultUpperRewardListPosition.x, 0, _defaultUpperRewardListPosition.z);
                for( int i=1; i<=info.count; ++i )
                {
                    int prizeDay = info.startDay + i;
                    var item = CreateItem(_upperRewardList.transform,prizeDay,i-1);   
                    _items.Add(item);
                }
            } else if( info.count <= shotTwoLineCount ) {
                for( int i=1; i<=info.count; ++i ){
                    int prizeDay = info.startDay + i;
                    var parent = i <= shotTwoLineUpperCount ? _upperRewardList.transform : _lowerRewardList.transform;
                    var item = CreateItem(parent,prizeDay,i-1);   
                    _items.Add(item);
                }
            } else {
                for( int i=1; i<=info.count; ++i ){
                    int prizeDay = info.startDay + i;
                    var parent = i <= twoLineUpperCount ? _upperRewardList.transform : _lowerRewardList.transform;
                    var item = CreateItem(parent,prizeDay,i-1);   
                    _items.Add(item);
                }
            }
            
        }

        LoginBonusListItem CreateItem( Transform parent,int prizeDay ,int index) {
            var prizeMaster = MasterManager.Instance.loginStampPrizeMaster.FindData( info.stampId ,prizeDay);
            var item = _items[index];
            item.gameObject.SetActive(true);
            item.Init(prizeMaster.prizeJson.First(),prizeDay,CheckShowStamp(prizeDay));
            return item;
        }

        public void ShowGetStampAnimation(bool toEnd = false)
        {
            _items.FirstOrDefault(item => item.day == info.currentDay)?.OpenStamp(toEnd);
        }

        private bool CheckShowStamp(int prizeDay)
        {
            if (info?.IsShowEnd == null) return false;
            return (info.IsShowEnd.Invoke()) ?  prizeDay <= info.currentDay : prizeDay < info.currentDay;
        }
    }
}