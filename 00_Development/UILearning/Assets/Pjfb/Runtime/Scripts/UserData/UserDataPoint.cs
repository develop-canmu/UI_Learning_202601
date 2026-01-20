using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.App.Request;
using UniRx;

namespace Pjfb.UserData {
    
    public class UserDataPoint {
        public long pointId{get;private set;}
		public long value{get;private set;}
        public UserDataPoint( long pointId, long value ){
            this.pointId = pointId;
            this.value = value;
        }
    }


    public class UserDataPointContainer : UserDataContainer<long, UserDataPoint>, IDisposable {
        
        private ReactiveProperty<long> _gem = new ReactiveProperty<long>();
        /// <summary>課金通貨</summary>
        public IReadOnlyReactiveProperty<long> gem => _gem;
        public void UpdateGemValue()
        {
            // Config取得後に一度手動で設定を行う
            long gemId = ConfigManager.Instance.mPointIdGem;
            _gem.Value = data.ContainsKey(gemId) ? Find(gemId).value : 0;
        }
        
        /// <summary>
        /// データ更新
        /// </summary>
        public void Update( NativeApiPoint[] points ){
            if( points == null ) {
               return; 
            } 
            
            foreach ( var point in points) {
                var userPoint = new UserDataPoint(point.mPointId, point.value);
                // 課金通貨の場合はgemに反映
                if(userPoint.pointId == ConfigManager.Instance.mPointIdGem)
                {
                    _gem.Value = userPoint.value;
                }
                Update(userPoint.pointId, userPoint);
            }
            
            ForeachHandler((handler)=>{ handler.OnUpdatedData(); });
        }
        
        public void Dispose()
        {
            _gem?.Dispose();
        }
    }
}