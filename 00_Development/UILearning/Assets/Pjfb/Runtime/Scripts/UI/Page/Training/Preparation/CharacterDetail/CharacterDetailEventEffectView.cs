using System.Collections;
using System.Collections.Generic;
using CruFramework;
using CruFramework.ResourceManagement;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb
{
    public class CharacterDetailEventEffectView : MonoBehaviour
    {
        public static readonly string ResourcePathKey = "CharacterDetailEventEffectLabel";
        
        [SerializeField]
        private TMP_Text effectText = null;
        
        [SerializeField]
        private Transform effectLabelRoot = null;
        
        [SerializeField]
        private GameObject addLabel = null;
        
        private GameObject effectLabelCache = null;
        
        public void SetEffectText(string text)
        {
            effectText.text = text;
        }
        
        public async UniTask SetLabel(int iconId)
        {
            // 生成済みの場合は削除
            if (effectLabelCache != null)
            {
                Destroy(effectLabelCache);
            }
            
            // idが1以上の場合のみラベルを生成
            if (iconId > 0)
            {
                // アセット読み込み
                string key = ResourcePathManager.GetPath(ResourcePathKey, iconId);
                GameObject label = await PageResourceLoadUtility.resourcesLoader.LoadAssetAsync<GameObject>(key);
            
                effectLabelCache = Instantiate(label, effectLabelRoot);
            }
        }
        
        public void SetAddLabel(bool isAdd)
        {
            addLabel.SetActive(isAdd);
        }
    }
}