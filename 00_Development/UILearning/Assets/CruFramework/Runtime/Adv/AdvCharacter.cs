using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{

    public abstract class AdvCharacter : AdvObject
    {
        /// <summary>ハイライト時</summary>
        public abstract void Highlight();        
        /// <summary>グレーアウト時</summary>
        public abstract void Grayout();
        
        private AdvCharacterDataId data = null;
        /// <summary>データ</summary>
        public AdvCharacterDataId Data{get{return data;}set{data = value;}}
        
        public virtual async UniTask PreLoadResource(AdvManager manager, string id)
        {
            manager.ErrorLog($"{nameof(AdvObject)}::{nameof(AdvCharacter.PreLoadResource)}をoverrideしてください");
            await UniTask.Delay(0);
        }
        
        public virtual void LoadResource(AdvManager manager, string id)
        {
            manager.ErrorLog($"{nameof(AdvObject)}::{nameof(AdvCharacter.LoadResource)}をoverrideしてください");
        }
        
        public virtual void ToFront()
        {
        }
    }
}
