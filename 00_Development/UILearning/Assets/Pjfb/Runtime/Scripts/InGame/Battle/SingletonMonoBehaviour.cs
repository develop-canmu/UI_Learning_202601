using UnityEngine;

namespace Pjfb.Battle
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private const string DefaultTag = "SingletonObject";

        protected static T inst;

        public static T Instance
        {
            get
            {
                if (inst == null)
                {
                    inst = FindOrCreateObject();
                }

                return inst;
            }
        }


        #region Mono Event

        protected virtual void Awake()
        {
            CheckInstance();
        }

        #endregion


        // コンストラクタ作成後のクラス初期化処理
        protected virtual void Init()
        {
        }

        public void DestroySingleton()
        {
            if (inst == null) return;

            var obj = inst.gameObject;
            if (obj != null)
            {
                DestroyImmediate(obj);
            }

            inst = null;
        }

        protected bool CheckInstance()
        {
            if (inst == null)
            {
                inst = (T)this;
                return true;
            }

            if (Instance == this)
            {
                return true;
            }

            Destroy(this);
            return false;
        }


        private static T FindOrCreateObject()
        {
            var type = typeof(T);

            T ret;
            var targets = GameObject.FindGameObjectsWithTag(DefaultTag);
            foreach (var t in targets)
            {
                ret = (T)t.GetComponent(type);
                if (ret == null) continue;
                ret.Init();
                return ret;
            }


            var obj = new GameObject(typeof(T).Name);
            obj.tag = DefaultTag;
            ret = obj.AddComponent<T>();
            ret.Init();
            return ret;
        }
    }
}