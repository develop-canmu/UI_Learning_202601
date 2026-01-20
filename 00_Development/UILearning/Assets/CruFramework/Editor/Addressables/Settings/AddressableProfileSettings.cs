using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Editor.Addressable
{
    [Serializable]
    public class AddressableProfileSettings
    {
        [SerializeField]
        private string profileName = string.Empty;
        public string ProfileName
        {
            get { return profileName; }
            set { profileName = value; }
        }
        
        [SerializeField]
        private string remoteBuildPath = string.Empty;
        public string RemoteBuildPath
        {
            get { return remoteBuildPath; }
            set { remoteBuildPath = value; }
        }
        
        [SerializeField]
        private string remoteLoadPath = string.Empty;
        public string RemoteLoadPath
        {
            get { return remoteLoadPath; }
            set { remoteLoadPath = value; }
        }
    }
}
