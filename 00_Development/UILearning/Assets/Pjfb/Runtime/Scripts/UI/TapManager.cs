using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class TapManager : MonoBehaviour
    {
        [SerializeField] private GameObject tapEffect;
        [SerializeField] private ParticleSystem tapParticleSystem;

        
        private Vector3 position;
        private void Update()
        {
            
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                position = Input.mousePosition;
                tapEffect.transform.position = AppManager.Instance.UIManager.UICamera.ScreenToWorldPoint(position + AppManager.Instance.UIManager.UICamera.transform.forward * 5);
                tapParticleSystem.Play();
            }
#elif UNITY_IOS || UNITY_ANDROID 
            if (Input.touchCount > 0)
            {
                Touch touch= Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    position = touch.position;
                    tapEffect.transform.position = AppManager.Instance.UIManager.UICamera.ScreenToWorldPoint(position + AppManager.Instance.UIManager.UICamera.transform.forward * 5);
                    tapParticleSystem.Play();    
                }
            }
#endif
        }
    }
}