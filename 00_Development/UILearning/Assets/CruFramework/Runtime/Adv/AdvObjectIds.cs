using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{

    public interface IAdvObjectId
    {
        string GetString();
    }
    
    public interface IHasAdvObjectCategory
    {
    }
    
    public interface IAdvObjectCategory
    {
        int Category{get;}
        void SetCategory(int category);
    }
    
    
    [System.Serializable]
    public class AdvStringId : IAdvObjectId
    {
        [SerializeField]
        private string value = string.Empty;
        /// <summary>文字列</summary>
        public string Value{get{return value;}set{this.value = value;}}
        
        string IAdvObjectId.GetString(){return value;}
    }
    
    [System.Serializable]
    public abstract class AdvObjectIds
    {
        public abstract int[] GetIds();
        public abstract string[] GetNames();
        public abstract object GetValueObject(int id);
        public abstract object AddNewValue();
    }
    
    [System.Serializable]
    public abstract class AdvObjectIds<T> : AdvObjectIds where T : IAdvObjectId, new()
    {
        [System.Serializable]
        private class ValueData
        {
            [SerializeField]
            public int id = 0;
            [SerializeField]
            public T value = new T();
        }
        
        [SerializeField]
        private int allocateId = 0;
        
        [SerializeField]
        private List<ValueData> values = new List<ValueData>();
        
        // キャッシュ
        private Dictionary<int, T> dicCache = null;
        
        
        public T this[int id]
        {
            get
            {
                return GetValue(id);
            }
        }

        /// <summary>新しいデータを追加</summary>
        public override object AddNewValue()
        {
            ValueData data = new ValueData();
            data.id = ++allocateId;
            values.Add(data);
            
            return data.value;
        }

        
        public override object GetValueObject(int id)
        {
            return GetValue(id);
        }
                
        // 値の取得
        public T GetValue(int id)
        {
            if(dicCache == null)
            {
                dicCache = GetDictionary();
            }
            
            if(dicCache.TryGetValue(id, out T result))
            {
                return result;
            }
            
            return default;
        }
        
        /// <summary>Dictionaryで取得</summary>
        public Dictionary<int, T> GetDictionary()
        {
            Dictionary<int, T> result = new Dictionary<int, T>();
            
            for(int i=0;i<values.Count;i++)
            {
                result.Add(values[i].id, values[i].value);
            }
            
            return result;
        }

        public override int[] GetIds()
        {
            int[] result = new int[values.Count];
            for(int i=0;i<values.Count;i++)
            {
                result[i] = values[i].id;
            }
            return result;
        }
        
        public override string[] GetNames()
        {
            string[] result = new string[values.Count];
            for(int i=0;i<values.Count;i++)
            {
                result[i] = values[i].value.GetString();
            }
            return result;
        }
        
        public bool HasValue(int id)
        {
            return GetValue(id) != null;
        }
    }
}