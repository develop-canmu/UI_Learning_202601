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
    public class BattleDigestObjectH2MD : MonoBehaviour
    {
        [SerializeField]
        private PlayableDirector playableDirector = null;
        private CancellationTokenSource source = null;

        public async UniTask SetH2MDAssetAsync(ResourcesLoader resourcesLoader, BattleConst.DigestType type, BattleDigestCharacterData charData, bool isPlayer)
        {
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(charData.MCharaId);
            string key = GetAddress(type,　mChar, charData.AbilityId, isPlayer);
            // キャンセル
            Cancel();
            // トークン生成
            source = new CancellationTokenSource();
            // H2MDデータ読み込み
            await resourcesLoader.LoadAssetAsync<H2MDAsset>(key,
                h2mdAsset =>
                {
                    H2MDPlayableClip clip = playableDirector.GetTimelineClips<H2MDPlayableTrack2D, H2MDPlayableClip>().FirstOrDefault();
                    // H2MDアセットセット
                    if (clip == null)
                    {
                        CruFramework.Logger.LogWarning("H2MDClipが見つかりません");
                        return;
                    }
                    clip.H2MDAsset = h2mdAsset;
                },
                source.Token
            );
        }

        private string GetAddress(BattleConst.DigestType type, CharaMasterObject mChar,long abilityId, bool isPlayer )
        {
            
            string path = "";
            string folder = "";

            switch (type)
            {
                case BattleConst.DigestType.Goal:
                    var side = isPlayer ? "01" : "02";
                    path = $"{mChar.id}_{side}_GoalCutIn";
                    folder = "GoalCutIn";
                    break;
                case BattleConst.DigestType.Special:
                    path = $"{abilityId}_Skill_";
                    path += isPlayer ? "Blue" : "Red";
                    folder = "Skill";
                    break;
                default: 
                    CruFramework.Logger.LogError("不正なタイプが指定されています");
                    break;
            }

            // H2MD/InGame/GoalCutIn/10001004_GoalCutIn.h2md
            // H2MD/InGame/Skill/1000_Skill_Blue.h2md
            var address = $"H2MD/InGame/{folder}/{path}.h2md";
            
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