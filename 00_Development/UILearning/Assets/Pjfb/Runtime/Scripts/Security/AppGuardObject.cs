using UnityEngine;
using DiresuUnity;

namespace Pjfb
{
    public class AppGuardObject : MonoBehaviour
    {
        public static bool IsActive { get; set; } = false;
        private bool isInitialized = false;

        #region MonoBehaviour Event
        private void Update()
        {
            InitAppGuard();
        }

        #endregion

        private void InitAppGuard()
        {
            if (!AppInitializer.IsInitialized || !IsActive || isInitialized) return;
            isInitialized = true;

            Diresu.S(UserData.UserDataManager.Instance.user.uMasterId.ToString());
            Diresu.O(Callback);
        }

        private void Callback(string data)
        {
            Debug.Log($"AppGuard Unity Callback >>> {data}");
        }
    }
}
