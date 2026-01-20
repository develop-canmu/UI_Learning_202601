using UnityEngine;

namespace PolyQA
{
    public class Save
    {
        private const string KeyPrefix = "PolyQA.";
        
        public static string AgentIpAddress
        {
            get => PlayerPrefs.GetString(KeyPrefix + "AgentIpAddress");
            set => PlayerPrefs.SetString(KeyPrefix + "AgentIpAddress", value);
        }
    }
}