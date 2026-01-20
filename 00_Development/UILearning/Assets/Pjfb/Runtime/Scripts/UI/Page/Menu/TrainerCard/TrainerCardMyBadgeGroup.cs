using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class TrainerCardMyBadgeGroup : MonoBehaviour
    {
        [SerializeField] private TrainerCardMyBadge[] myBadgeList;

        public async UniTask SetMyBadgeList(long[] badgeIdList, CancellationToken token)
        {
            // バッジセットタスクリスト
            List<UniTask> badgeTaskList = new List<UniTask>();
            
            for(int i = 0; i < myBadgeList.Length; i++)
            {
                // 存在するバッジ
                bool hasBadge = i < badgeIdList.Length && badgeIdList[i] != 0;
                // タスクに追加
                badgeTaskList.Add(myBadgeList[i].SetMyBadge(badgeIdList[i], hasBadge, token));
            }

            await UniTask.WhenAll(badgeTaskList);
        }
    }
}