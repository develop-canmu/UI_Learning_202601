using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableBundledAssetSchemaSettingsView : VisualElement
    {
        private ScriptableObject scriptableObject = null;
        
        private IntegerField requestTimeoutField = null;
        private IntegerField httpRedirectLimitField = null;
        private IntegerField retryCountField = null;

        public AddressableBundledAssetSchemaSettingsView(ScriptableObject scriptableObject)
        {
            this.scriptableObject = scriptableObject;
            
            requestTimeoutField = new IntegerField();
            requestTimeoutField.label = "Request Timeout";
            Add(requestTimeoutField);
            
            httpRedirectLimitField = new IntegerField();
            httpRedirectLimitField.label = "Http Redirect Limit";
            Add(httpRedirectLimitField);
            
            retryCountField = new IntegerField();
            retryCountField.label = "Retry Count";
            Add(retryCountField);
        }

        public void UnregisterCallback()
        {
            requestTimeoutField.UnregisterCallback<ChangeEvent<int>, AddressableBundledAssetSchemaSettings>(OnRequestTimeoutChanged);
            httpRedirectLimitField.UnregisterCallback<ChangeEvent<int>, AddressableBundledAssetSchemaSettings>(OnHttpRedirectLimitChanged);
            retryCountField.UnregisterCallback<ChangeEvent<int>, AddressableBundledAssetSchemaSettings>(OnRetryCountChanged);
        }
        
        public void RegisterCallback(AddressableBundledAssetSchemaSettings schemaSettings)
        {
            
            requestTimeoutField.RegisterCallback<ChangeEvent<int>, AddressableBundledAssetSchemaSettings>(OnRequestTimeoutChanged, schemaSettings);
            httpRedirectLimitField.RegisterCallback<ChangeEvent<int>, AddressableBundledAssetSchemaSettings>(OnHttpRedirectLimitChanged, schemaSettings);
            retryCountField.RegisterCallback<ChangeEvent<int>, AddressableBundledAssetSchemaSettings>(OnRetryCountChanged, schemaSettings);
        }
        
        public void UpdateView(AddressableBundledAssetSchemaSettings schemaSettings)
        {
            requestTimeoutField.value = schemaSettings.RequestTimeout;
            httpRedirectLimitField.value = schemaSettings.HttpRedirectLimit;
            retryCountField.value = schemaSettings.RetryCount;
        }

        private void OnRequestTimeoutChanged(ChangeEvent<int> evt, AddressableBundledAssetSchemaSettings schemaSettings)
        {
            AddressableUtility.SetDirty(scriptableObject, () => schemaSettings.RequestTimeout = evt.newValue);
        }
        
        private void OnHttpRedirectLimitChanged(ChangeEvent<int> evt, AddressableBundledAssetSchemaSettings schemaSettings)
        {
            AddressableUtility.SetDirty(scriptableObject, () => schemaSettings.HttpRedirectLimit = evt.newValue);
        }
        
        private void OnRetryCountChanged(ChangeEvent<int> evt, AddressableBundledAssetSchemaSettings schemaSettings)
        {
            AddressableUtility.SetDirty(scriptableObject, () => schemaSettings.RetryCount = evt.newValue);
        }
    }
}
