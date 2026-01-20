using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEngine.UI;

namespace Pjfb.SystemUnlock
{
    public class SystemUnlockReference : MonoBehaviour
    {
        [SerializeField] private List<SystemUnlockDataManager.SystemUnlockNumber> systemUnlockNumber;
        [SerializeField] private GameObject unlockButton;
        [SerializeField] private GameObject lockObject;
        [SerializeField] private bool isScroll;
        public List<SystemUnlockDataManager.SystemUnlockNumber> SystemUnlockNumber => systemUnlockNumber;
        public GameObject UnlockButton => unlockButton;
        public GameObject LockObject => lockObject;
        public bool IsScroll => isScroll;
    }
}