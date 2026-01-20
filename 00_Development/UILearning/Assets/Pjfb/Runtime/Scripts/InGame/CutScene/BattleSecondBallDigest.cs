using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    public class BattleSecondBallDigest : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] characterSprites;
        [SerializeField] private SpriteRenderer[] characterSpriteBackgrounds;
        [SerializeField] private Animator animator;
        [SerializeField] [ColorValue] private string leftColorValueKey;
        [SerializeField] [ColorValue] private string rightColorValueKey;
        private CancellationTokenSource source = null;

        private int winnerIndex;         
        public async UniTask SetSpriteAsync(ResourcesLoader resourcesLoader, BattleConst.DigestType type, BattleDigestCharacterData winnerCharacter, List<BattleDigestCharacterData> joinedCharacter)
        {
            var mainKey = GetAddress(type,　winnerCharacter.MCharaId);
            List<string> otherKeyList = new List<string>();
            foreach (var character in joinedCharacter)
            {
                otherKeyList.Add(GetAddress(type, character.MCharaId));
            }
            // キャンセル
            Cancel();
            // トークン生成
            source = new CancellationTokenSource();

            // Spriteデータ読み込み
            var joinCharacterCount = 0;
            winnerIndex = BattleGameLogic.GetNonStateRandomValue(0, characterSprites.Length);
            for (var i = 0; i < characterSprites.Length; i++)
            {
                if (winnerIndex == i)
                {
                    await resourcesLoader.LoadAssetAsync<Sprite>(mainKey, sprite => characterSprites[i].sprite = sprite, source.Token);
                    var colorKey = winnerCharacter.Side == BattleDataMediator.Instance.PlayerSide ? leftColorValueKey : rightColorValueKey;
                    characterSpriteBackgrounds[i].color = ColorValueAssetLoader.Instance[colorKey];
                }
                else
                {
                    await resourcesLoader.LoadAssetAsync<Sprite>(otherKeyList[joinCharacterCount], sprite => characterSprites[i].sprite = sprite, source.Token);
                    var colorKey = joinedCharacter[joinCharacterCount].Side == BattleDataMediator.Instance.PlayerSide ? leftColorValueKey : rightColorValueKey;
                    characterSpriteBackgrounds[i].color = ColorValueAssetLoader.Instance[colorKey];
                    joinCharacterCount++;
                }
            }
        }

        private string GetAddress(BattleConst.DigestType type, long id)
        {
            string address = "";

            switch (type)
            {
                case BattleConst.DigestType.SecondBall2:
                case BattleConst.DigestType.SecondBall3:
                case BattleConst.DigestType.SecondBall4:
                    address = PageResourceLoadUtility.GetCharacterSecondBallCutinPath(id);
                    break;
                default: 
                    CruFramework.Logger.LogError("不正なタイプが指定されています");
                    break;
            }

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

        public void OnMiddleActionInvoked()
        {
            animator.SetTrigger($"{winnerIndex + 1}");
        }
    }
}