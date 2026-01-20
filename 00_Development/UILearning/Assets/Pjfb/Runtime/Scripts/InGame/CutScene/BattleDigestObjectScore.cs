using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodeStage.AntiCheat.ObscuredTypes;
using CruFramework.Audio;
using CruFramework.H2MD;
using CruFramework.ResourceManagement;
using CruFramework.Timeline;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Spine;
using UnityEngine;
using Spine.Unity;
using UnityEngine.Playables;

namespace Pjfb.InGame
{
    public class BattleDigestObjectScore : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer currentLeftScore;
        [SerializeField] private SpriteRenderer currentRightScore;
        [SerializeField] private SpriteRenderer beforeLeftScore;
        [SerializeField] private SpriteRenderer beforeRightScore;
        [SerializeField] private SpriteRenderer afterLeftScore;
        [SerializeField] private SpriteRenderer afterRightScore;
        [SerializeField] private GameObject leftRoot;
        [SerializeField] private GameObject rightRoot;
        private CancellationTokenSource source = null;

        public async UniTask SetScoreAsync(ResourcesLoader resourcesLoader, BattleConst.TeamSide offenceSide, List<int> score)
        {
            // TODO スコアのオブジェクトだけ常設の形で構成作り替えた方が幸せそう
            
            bool isOffenceLeft = offenceSide == BattleDataMediator.Instance.PlayerSide;
            string beforeLeftScoreKey = GetAddress(isOffenceLeft ? score[(int)BattleDataMediator.Instance.PlayerSide] - 1 : score[(int)BattleDataMediator.Instance.PlayerSide]);
            string beforeRightScoreScoreKey = GetAddress(isOffenceLeft ? score[(int)BattleDataMediator.Instance.EnemySide] : score[(int)BattleDataMediator.Instance.EnemySide] - 1);
            string afterScoreKey = GetAddress( isOffenceLeft ? score[(int)BattleDataMediator.Instance.PlayerSide] : score[(int)BattleDataMediator.Instance.EnemySide]);
            
            // キャンセル
            Cancel();
            // トークン生成
            source = new CancellationTokenSource();
            // Sprite読み込み&オブジェクト制御
            await resourcesLoader.LoadAssetAsync<Sprite>(beforeLeftScoreKey, sprite =>
            {
                beforeLeftScore.sprite = sprite;
                if (isOffenceLeft)
                {
                    currentLeftScore.gameObject.SetActive(false);
                }
                else
                {
                    currentLeftScore.sprite = sprite;
                }
            }  ,source.Token);
            await resourcesLoader.LoadAssetAsync<Sprite>(beforeRightScoreScoreKey, sprite =>
            {
                beforeRightScore.sprite = sprite;
                if (isOffenceLeft)
                {
                    currentRightScore.sprite = sprite;
                }
                else
                {
                    currentRightScore.gameObject.SetActive(false);
                }
            },source.Token);
            await resourcesLoader.LoadAssetAsync<Sprite>(afterScoreKey, sprite =>
            {
                if (isOffenceLeft)
                {
                    rightRoot.gameObject.SetActive(false);
                    afterLeftScore.sprite = sprite;
                }
                else
                {
                    leftRoot.gameObject.SetActive(false);
                    afterRightScore.sprite = sprite;
                }
            },source.Token);
        }

        private string GetAddress(int point)
        {
            // Sprites/InGame/Number/Goal_no_00.png
            var address = $"Sprites/InGame/Number/Goal_no_{point:D2}.png";
            
            CruFramework.Logger.Log(address);
            return address;
        }

        /// <summary>キャンセル</summary>
        protected virtual void Cancel()
        {
            // キャンセル
            if(source != null)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
        }

        protected virtual void OnDestroy()
        {
            // ゲームオブジェクト削除時にキャンセル
            Cancel();
        }
    }
}