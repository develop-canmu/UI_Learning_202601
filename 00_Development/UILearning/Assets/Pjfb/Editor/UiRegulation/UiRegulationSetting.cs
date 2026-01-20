using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pjfb.Extensions;
using SRF;

namespace Pjfb.UiRegulation
{
    
    [Serializable]
    public enum UiRegulationType
    {
        Page,
        Modal,
        Common,
    }
    
    [CreateAssetMenu]
    public class UiRegulationSetting : ScriptableObject
    {
        [Serializable]
        public class UiRegulationObject
        {
            public string name;
            public bool required;
            public UiRegulationObject[] child;

            public void ShowRoot(string path = "")
            {
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }

                if (!string.IsNullOrEmpty(path))
                {
                    path += "/";
                }

                path += name;
                
                child.ForEach(child => child.ShowRoot(path));
            }

            public void CheckRegulation(Transform root, List<string> missingList, List<string> unnecessaryList, List<string> anchorErrorList) 
            {
                CheckRegulation(root, missingList, unnecessaryList, anchorErrorList, this, true, root.gameObject.name);
                return;
            }
            
            public void CheckRegulation(Transform root, List<string> missingList, List<string> unnecessaryList, List<string> anchorErrorList, UiRegulationObject childObject, bool isParent, string path)
            {

                var isMatch = root.name == childObject.name;

                if (!isMatch && !isParent)
                {
                    return;
                }
                
                var targetChildren = root.GetChildren().ToList();
                
                if (targetChildren.Count == 0)
                {
                    return;
                }
                
                childObject.child.ForEach(child =>
                {
                    var rootPath = path;
                    if (!string.IsNullOrEmpty(rootPath))
                    {
                        rootPath += "/";
                    }
                    rootPath += child.name;
                    
                    var ret = targetChildren.FirstOrDefault(targetObj => targetObj.name == child.name);
                    if (ret == null && child.required)
                    {
                        // 必須のオブジェクトなし
                        missingList.Add($"{rootPath}がありません");
                    }
                    
                    if (ret != null)
                    {
                        // anchor設定のチェック
                        var t = ret.transform as RectTransform;
                        if (!(Mathf.Approximately(t.anchorMin.x,0) && 
                            Mathf.Approximately(t.anchorMin.y,0) &&
                            Mathf.Approximately(t.anchorMax.x,1) &&
                            Mathf.Approximately(t.anchorMax.y,1)))
                        {
                            anchorErrorList.Add(rootPath);
                        }
                        
                        CheckRegulation(ret.transform, missingList, unnecessaryList, anchorErrorList, child, false, rootPath);
                    }
                    targetChildren.Remove(ret);
                });

                if (childObject.child.Length == 0)
                {
                    return;
                }
                
                // targetChildren に残ったもの → regulationで推奨されていない
                targetChildren.ForEach(child =>
                {
                    var rootPath = path;
                    if (!string.IsNullOrEmpty(rootPath))
                    {
                        rootPath += "/";
                    }
                    rootPath += child.name;
                    unnecessaryList.Add(rootPath);
                });
                
            }
        }

        [Serializable]
        public class UiRegulationData
        {
            public UiRegulationType type;
            public UiRegulationObject setting;
            
        }
        
        public UiRegulationData[] RegulationDataList;
        
    }
}