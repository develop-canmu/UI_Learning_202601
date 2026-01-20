using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class TrainerCardMyBadge : MonoBehaviour
    {

        [SerializeField] private GameObject emptyBadge;
        [SerializeField] private MyBadgeImage badgeImage;
        
        // バッジが設定されている
        public async UniTask SetMyBadge(long badgeId, bool hasBadge, CancellationToken token = default)
        {
            // バッジイメージ
            badgeImage.gameObject.SetActive(hasBadge);
            // バッジ未設定表示
            emptyBadge.SetActive(hasBadge == false);
            // バッジが存在するならテクスチャをセット
            if (hasBadge)
            {
                // CancellableImageは内部でトークンを持っているので外付けでトークンを追加
                await badgeImage.SetTextureAsync(badgeId).AttachExternalCancellation(token);
            }
        }
    }
}