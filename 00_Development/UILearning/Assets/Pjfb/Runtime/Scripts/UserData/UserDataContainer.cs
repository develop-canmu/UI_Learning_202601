using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.UserData {

    public interface IUserDataHandler {
        /// <summary>
        /// データ更新後に呼ばれる
        /// </summary>
        void OnUpdatedData();
    }


    public class UserDataContainer<KEY, VALUE> where VALUE : class  {
        public Dictionary<KEY, VALUE> data => _data;
        

        Dictionary<KEY, VALUE> _data = new Dictionary<KEY, VALUE>();
        List<IUserDataHandler> _handler = new List<IUserDataHandler>();

        public VALUE Find( KEY key ){
            if( !_data.ContainsKey(key) ) {
                CruFramework.Logger.LogWarning("not find userData by " + this + " : key = " + key);
                return null;
            }
            return _data[key];
        }

        public bool Contains( KEY key ){
            return _data.ContainsKey(key);
        }

        /// <summary>
        /// データ更新
        /// </summary>
        public void Update( KEY key, VALUE value ){
            _data[key] = value;
        }

        /// <summary>
        /// データ削除
        /// </summary>
        public void Remove(KEY key) {
            _data.Remove(key);
        }

        /// <summary>
        /// ハンドラ追加
        /// </summary>
        public void AddHandler( IUserDataHandler handler ){
            if( _handler.Contains(handler) ){
                return;
            }
            _handler.Add(handler);
        }

        /// <summary>
        /// ハンドラ削除
        /// </summary>
        public void RemoveHandler(IUserDataHandler handler) {
            _handler.Remove(handler);
        }

        /// <summary>
        /// ハンドラForeach
        /// </summary>
        protected void ForeachHandler(System.Action<IUserDataHandler> func) {
            _handler.ForEach(func);
        }
    }
    
}