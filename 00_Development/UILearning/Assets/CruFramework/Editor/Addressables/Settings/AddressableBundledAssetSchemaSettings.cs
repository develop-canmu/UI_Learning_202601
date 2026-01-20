using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Editor.Addressable
{
    [Serializable]
    public class AddressableBundledAssetSchemaSettings
    {
        [SerializeField]
        private int requestTimeout = 60;
        /// <summary>タイムアウト間隔</summary>
        public int RequestTimeout
        {
            get { return requestTimeout; }
            set { requestTimeout = value; }
        }

        [SerializeField]
        private int httpRedirectLimit = -1;
        /// <summary>リダイレクト数</summary>
        public int HttpRedirectLimit
        {
            get { return httpRedirectLimit; }
            set { httpRedirectLimit = value; }
        }

        [SerializeField]
        private int retryCount = 3;
        /// <summary>リトライカウント</summary>
        public int RetryCount
        {
            get { return retryCount; }
            set { retryCount = value; }
        }
    }
}
