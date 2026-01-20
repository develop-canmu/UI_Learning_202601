// ReSharper disable once CheckNamespace

namespace CruFramework.Utils
{
    public abstract class Singleton<T> where T : class, new()
    {
        #region Singleton

        // Instance
        private static T inst;

        // Get Instance
        public static T Instance
        {
            get
            {
                if (inst == null)
                {
                    inst = new T();
                }

                return inst;
            }
        }

        // Protected New
        protected Singleton()
        {
            Init();
        }

        #endregion

        // コンストラクタ作成後のクラス初期化処理
        protected virtual void Init()
        {
        }
        
        protected virtual void OnRelease()
        {
        }
        
        public void Release()
        {
            OnRelease();
            inst = null;
        }
    }
}