using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class ImageEffect : MonoBehaviour
    {
        [SerializeField] private Transform effectRoot;
        [SerializeField] private RawImage maskImage;
        public Transform EffectRoot
        {
            get { return effectRoot; }
        }
        
        public ItemIconType IconType;
        private GameObject effectObject;
        private string cachedEffectPath;

        private RectTransform _rectTransform;
        private RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = gameObject.transform as RectTransform;
                }
                return _rectTransform;
            }
        }

        private CancellationTokenSource cts;

        public void SetMask(Texture texture)
        {
            if (maskImage == null)
            {
                return;
            }
            maskImage.texture = texture;
        }

        public async UniTask LoadEffect(long id)
        {

            // リリース時は不要とされているがエフェクトの表示を設定からOFFにする機能を実装する場合、このメソッドで何もさせずにreturnさせる

            // エフェクトを読み込み中であればキャンセルして破棄
            Cancel();
            // 再生成
            cts = new CancellationTokenSource();
            
            var effectPath = GetKey(id);

            if (effectPath.Equals(cachedEffectPath) && effectObject != null)
            {
                return;
            }

            if (effectObject != null)
            {
                Destroy(effectObject);
            }
            
            if (id == 0)
            {
                return;
            }
            
            cachedEffectPath = effectPath;

            var loadEffect = await PageResourceLoadUtility.resourcesLoader.LoadAssetAsync<GameObject>(cachedEffectPath, cts.Token);
            if (loadEffect == null || this == null)
            {
                return;
            }

            var root = effectRoot != null ? effectRoot : gameObject.transform;
            effectObject = Instantiate(loadEffect, root);

            var effectRectTransform = effectObject.transform as RectTransform;
            if (effectRectTransform != null)
            {
                var effectScale = rectTransform.rect.size / effectRectTransform.rect.size;
                effectRectTransform.localScale = new Vector3(effectScale.x, effectScale.y, 1.0f);
            }
            
            // 完了したので破棄
            cts.Dispose();
            cts = null;
        }
        
        private string GetKey(long id)
        {
            switch (IconType)
            {
                case ItemIconType.UserIcon: return PageResourceLoadUtility.GetUserIconEffectPath(id);
                case ItemIconType.UserTitle: return PageResourceLoadUtility.GetUserTitleEffectPath(id);
                case ItemIconType.Character : return PageResourceLoadUtility.GetCharacterIconEffectPath(id);
                case ItemIconType.SpecialSupportCharacterCard : return PageResourceLoadUtility.GetSpecialSupportCharacterCardEffectPath(id);
                default: return "";
            }
        }

        private void Cancel()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }

        private void OnDestroy()
        {
            Cancel();
        }
    }
}