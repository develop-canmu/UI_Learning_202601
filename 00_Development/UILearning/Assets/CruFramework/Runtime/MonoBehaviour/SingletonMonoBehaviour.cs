using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new System.Exception($"{typeof(T).Name} is nothing.");
                }
                return instance;
            }
        }

        public static bool Exists()
        {
            return instance != null;
        }

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError($"{typeof(T).Name} is already exists.");
                Destroy(gameObject);
                return;
            }

            instance = (T)this;
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnStart()
        {
        }
    }
}
