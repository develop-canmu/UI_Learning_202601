using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.SystemUnlock;
using UnityEngine.Serialization;

namespace Pjfb
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] private SystemUnlockView unlockViewPrefab;
        private SystemUnlockView systemUnlockView;

        public SystemUnlockView SystemUnlockView
        {
            get
            {
                if (systemUnlockView == null)
                {
                    systemUnlockView = Instantiate(unlockViewPrefab, transform);
                }

                return systemUnlockView;
            }
        }
    }
}