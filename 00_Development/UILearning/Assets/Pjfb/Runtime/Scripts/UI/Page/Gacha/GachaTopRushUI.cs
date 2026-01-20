using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;


namespace Pjfb.Gacha
{
    public class GachaTopRushUI : MonoBehaviour
    {
        const string bandAddressKey = "GachaRushBindAddress";
        
        [SerializeField]
        private GachaRushTopImageWebTexture _logoTexture = null;
        [SerializeField]
        private GameObject _defaultRushChange = null;
        [SerializeField]
        private GameObject _rushRoot = null;

        private Dictionary<long, GachaTopRushView> _bandObjectCache = new Dictionary<long, GachaTopRushView>();

        GachaRushData _currentData = null;

        /// <summary>
        /// RushのUI周り初期化
        /// </summary>
        public async UniTask Init( GachaTopPageData data, CancellationToken token ) {
            // cacheが残ったら全部削除
            if (_bandObjectCache != null && _bandObjectCache.Count > 0)
            {
                foreach (var objectCache in _bandObjectCache.Values)
                {
                    Destroy(objectCache.gameObject);
                }
            }
            _bandObjectCache = new Dictionary<long, GachaTopRushView>();
            var gachaDataList = data.GetGachaDatas(GachaType.Normal);
            foreach( var gachaData in gachaDataList ){
                
                var rushData = RetrieveRushData(gachaData);
                if( rushData == null || rushData.IsHiddenRush()) {
                    continue;
                }

                if( _bandObjectCache.ContainsKey(rushData.effectNumber) ) {
                    continue;
                }

                var path = CruFramework.ResourcePathManager.GetPath(bandAddressKey, rushData.effectNumber.ToString());

                GameObject prefab = null;
                await PageResourceLoadUtility.LoadAssetAsync<GameObject>(path, obj => prefab = obj, token);
                var bind = Instantiate<GameObject>(prefab, _rushRoot.transform);
                bind.gameObject.SetActive(false);
                var view = bind.gameObject.GetComponent<GachaTopRushView>();
                if( view != null ) {
                    view.Init(rushData.expiredAt);
                }
                _bandObjectCache.Add(rushData.effectNumber, view);
            }
        }

        /// <summary>
        /// ラッシュ用のUI更新
        /// </summary>
        /// <param name="data"></param>
        public void UpdateView( GachaSettingData data ) {
            
            if( data == null ) {
                UpdateViewDefault( false );
                return;
            }
            var canRush = CanRush( data );
            if( !canRush ) {
                UpdateViewDefault( false );
                return;
            }
            // 暫定対応
            // var isHideChanceUI = IsHideChanceUI( data );
            // if (isHideChanceUI)
            // {
            //     UpdateViewDefault (false );
            //     return;
            // }

            var rushData = RetrieveRushData( data );
            if( rushData == null || rushData.rushId == 0) {
                UpdateViewDefault( true );
                return;
            }

            var now = AppTime.Now;
            if( rushData.expiredAt <= now ) {
                UpdateViewDefault( true );
                return;
            }

            if( !_bandObjectCache.ContainsKey(rushData.effectNumber) ) {
                UpdateViewDefault( true );
                return;
            }

            if( _currentData != null && _currentData.rushId != 0 ){
                if( _currentData.rushId == rushData.rushId &&  _bandObjectCache[rushData.effectNumber].gameObject.activeSelf) {
                    //同じラッシュだったら何もしない
                    return;
                }
            }
            

            UpdateViewDefault( true );
            _currentData = rushData;
            _bandObjectCache[rushData.effectNumber].gameObject.SetActive(true);
            _bandObjectCache[rushData.effectNumber].Init(rushData.expiredAt);
            _logoTexture.gameObject.SetActive(true);
            _logoTexture.SetTexture(rushData.imageNumber);
            _defaultRushChange.gameObject.SetActive(false);
        }

        /// <summary>
        /// デフォルト表示
        /// </summary>        
        public void UpdateViewDefault( bool canRush ) {
            _currentData = null;

            //一旦すべてoff
            foreach( var obj in _bandObjectCache?.Values ){
                obj.gameObject.SetActive(false);
            }
            _logoTexture.gameObject.SetActive(false);

            
            _defaultRushChange.gameObject.SetActive(canRush);
        }

        /// <summary>
        /// チケット用表示
        /// </summary>
        public void UpdateViewTicket() {
            UpdateViewDefault(false);
            _defaultRushChange.gameObject.SetActive(false);
        }


        public GachaRushData RetrieveRushData(GachaSettingData data) {
            if( data.MultiGachaData.RushData != null && data.MultiGachaData.RushData.rushId > 0 ) {
                return data.MultiGachaData.RushData;
            }

            if( data.SingleGachaData.RushData != null && data.SingleGachaData.RushData.rushId > 0 ) {
                return data.SingleGachaData.RushData;
            }
            return null;
        }

        public bool IsHideChanceUI(GachaSettingData data) {
            if( data.MultiGachaData.RushData != null && data.MultiGachaData.RushData.IsHiddenRush()) {
                return true;
            }

            if( data.SingleGachaData.RushData != null && data.SingleGachaData.RushData.IsHiddenRush()) {
                return true;
            }
            return false;
        }
        


        public void Update(){
            if( _currentData == null ) {
                return;
            }

            var now = AppTime.Now;
            if( _currentData.expiredAt <= now ) {
                UpdateViewDefault( true );
                _currentData = null;
            }
            
        }


        bool CanRush(GachaSettingData data) {
            if( data.MultiGachaData != null && data.MultiGachaData.CanRush  ) {
                return true;
            }

            if( data.SingleGachaData != null && data.SingleGachaData.CanRush ) {
                return true;
            }
            return false;
        }

    }
}
