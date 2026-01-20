using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CruFramework.Editor.Addressable
{
    [Serializable]
    public class AddressableGroupRuleByRegex : AddressableGroupRule
    {
        public enum RegexMatchType
        {
            Wildcard = 0,
            Regex = 1,
        }
        
        [SerializeField]
        private RegexMatchType matchType = RegexMatchType.Wildcard;
        public RegexMatchType MatchType
        {
            get { return matchType; }
            set { matchType = value; }
        }
        
        [SerializeField]
        private string pathRegex = string.Empty;
        public string PathRegex
        {
            get { return pathRegex; }
            set { pathRegex = value; }
        }
        
        [SerializeField]
        private string groupName = string.Empty;
        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }
        
        public override bool IsMatch(string path)
        {
            pathRegex = pathRegex.Trim();
            if (string.IsNullOrEmpty(pathRegex)) return false;
            
            switch (matchType)
            {
                case RegexMatchType.Wildcard:
                {
                    if (pathRegex.Contains("*") || pathRegex.Contains("?"))
                    {
                        string regex = "^" + Regex.Escape(pathRegex).Replace(@"\*", ".*").Replace(@"\?", ".");
                        return Regex.IsMatch(path, regex);
                    }
                    else
                    {
                        return path.StartsWith(pathRegex);
                    }
                        
                }
                case RegexMatchType.Regex:
                {
                    return Regex.IsMatch(path, pathRegex);
                }
                default:
                {
                    throw new NotImplementedException($"{matchType} is not implemented.");
                }
            }
        }

        public override string GetGroupName(string path)
        {
            return ParseReplacement(path, groupName);
        }

        public string ParseReplacement(string path, string name)
        {
            if (string.IsNullOrWhiteSpace(pathRegex) || string.IsNullOrWhiteSpace(name)) return null;
            
            string replacement = name.Trim().Replace('\\', '/').Replace('/', '-');
            replacement = AddressableImportRegex.ParsePath(path, replacement);
            if (matchType == RegexMatchType.Regex)
            {
                replacement = Regex.Replace(path, pathRegex, replacement);
            }
            return replacement;
        }
    }
    
    public class AddressableImportRegex
    {
        private const string PathRegex = @"\$\{PATH\[\-{0,1}\d{1,3}\]\}";
        
        public static string[] GetPathArray(string path)
        {
            return path.Split('/');
        }
        
        public static string GetPathAtArray(string path, int index)
        {
            return GetPathArray(path)[index];
        }
        
        public static string ParsePath(string path, string replacement)
        {
            string[] pathArray = path.Split('/');
            MatchCollection matches = Regex.Matches(replacement, PathRegex);
            string[] parsedMatches = new string[matches.Count];
            for(int i = 0; i < matches.Count; i++)
            {
                string matchString = matches[i].ToString();
                int startIndex = matchString.IndexOf('[') + 1;
                int endIndex = matchString.IndexOf(']');
                int index = int.Parse(matchString.Substring(startIndex, endIndex - startIndex));
                
                while (index > pathArray.Length - 1)
                {
                    index -= pathArray.Length - 1;
                }
                while (index < 0)
                {
                    index += pathArray.Length - 1;
                }
                
                parsedMatches[i] = GetPathAtArray(path, index);
            }

            string[] regexArray = Regex.Split(replacement, PathRegex);
            string finalPath = string.Empty;
            for(int i = 0; i < regexArray.Length; i++)
            {
                finalPath += regexArray[i];
                if (i < parsedMatches.Length)
                {
                    finalPath += parsedMatches[i];
                }
            }
            
            return finalPath;
        }
    }
}
