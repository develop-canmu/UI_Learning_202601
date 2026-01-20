using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework;
using CruFramework.ResourceManagement;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using UnityEngine.Events;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameUIMediator : SingletonMonoBehaviour<ClubRoyalInGameUIMediator>
    {
        [SerializeField] public ClubRoyalInGameHeaderUI HeaderUI;
        [SerializeField] public ClubRoyalInGameFooterUI FooterUI;
        [SerializeField] public ClubRoyalInGameBeforeFightTimerUI BeforeFightTimerUI;
        [SerializeField] public ClubRoyalInGameChatView ChatView;
        [SerializeField] public ClubRoyalInGameFieldUI FieldUI;
        [SerializeField] public ClubRoyalInGameActivePartyListUI ActivePartyListUI;
        [SerializeField] public ClubRoyalInGameActiveItemListUI ActiveItemListUI;
        [SerializeField] public ClubRoyalInGameActivateItemUI ActivateItemUI;
        [SerializeField] public ClubRoyalInGameRemainBallCountUI RemainBallCountUI;
        [SerializeField] public ClubRoyalInGameGoalCutInUI GoalCutInUI;
        [SerializeField] public ClubRoyalInGameOccupySpotUI OccupySpotUI;
        [SerializeField] public ClubRoyalInGameWinStreakUI WinStreakUI;
        [SerializeField] public ClubRoyalInGameStartDirectionUI StartDirectionUI;
        [SerializeField] public ClubRoyalInGameKickOffEffectUI KickOffUI;
        [SerializeField] public ClubRoyalInGameResultUI ResultUI;
        [SerializeField] public Transform AbsorbEffectRoot;
        [SerializeField] private ClubRoyalInGameAdviserUI AdviserUI;
        [SerializeField] public ClubRoyalBattleLogMessageScroll LogMessageScroller;

        public UnityEvent<GuildBattlePlayerData> OnPlayerDataUpdated = new UnityEvent<GuildBattlePlayerData>();

        public void Initialize()
        {
            if (AdviserUI != null)
            {
                // アドバイザー関連の表示を初回データが来るまで非表示
                AdviserUI.Display(false);
                OnPlayerDataUpdated.AddListener(AdviserUI.DisplayUpdate);
            }
        }

        public void Closed()
        {
            OnPlayerDataUpdated.RemoveListener(AdviserUI.DisplayUpdate);
        }
        
        private ResourcesLoader resourcesLoader;
        // 生成されたキャラクターモデル
        private Dictionary<GuildBattleCommonConst.GuildBattleTeamSide, Dictionary<long, List<GameObject>>>
            instantiatedCharacterModels = new Dictionary<GuildBattleCommonConst.GuildBattleTeamSide, Dictionary<long, List<GameObject>>>()
                {
                    { GuildBattleCommonConst.GuildBattleTeamSide.Left, new Dictionary<long, List<GameObject>>() },
                    { GuildBattleCommonConst.GuildBattleTeamSide.Right, new Dictionary<long, List<GameObject>>() }
                };

        private Dictionary<BattleConst.ClubRoyalBattleEffectType, List<ParticleSystem>>
            instantiatedBattleEffects = new Dictionary<BattleConst.ClubRoyalBattleEffectType, List<ParticleSystem>>()
            {
                { BattleConst.ClubRoyalBattleEffectType.Dribble, new List<ParticleSystem>() },
                { BattleConst.ClubRoyalBattleEffectType.Spawn, new List<ParticleSystem>() },
                { BattleConst.ClubRoyalBattleEffectType.DamageTaken, new List<ParticleSystem>() },
                { BattleConst.ClubRoyalBattleEffectType.DamageTakenBLM, new List<ParticleSystem>() },
                { BattleConst.ClubRoyalBattleEffectType.DeadCharacter, new List<ParticleSystem>() },
                { BattleConst.ClubRoyalBattleEffectType.DeadBLM, new List<ParticleSystem>() },
            };

        private List<Dictionary<BattleConst.ClubRoyalWordEffectType, List<ParticleSystem>>>
            instantiatedWordEffects = new List<Dictionary<BattleConst.ClubRoyalWordEffectType, List<ParticleSystem>>>()
            {
                new Dictionary<BattleConst.ClubRoyalWordEffectType, List<ParticleSystem>>()
                {
                    { BattleConst.ClubRoyalWordEffectType.Dash, new List<ParticleSystem>() },
                    { BattleConst.ClubRoyalWordEffectType.Spawn, new List<ParticleSystem>() },
                    { BattleConst.ClubRoyalWordEffectType.MatchUp, new List<ParticleSystem>() },
                    { BattleConst.ClubRoyalWordEffectType.Hit, new List<ParticleSystem>() }
                },
                new Dictionary<BattleConst.ClubRoyalWordEffectType, List<ParticleSystem>>()
                {
                    { BattleConst.ClubRoyalWordEffectType.Dash, new List<ParticleSystem>() },
                    { BattleConst.ClubRoyalWordEffectType.Spawn, new List<ParticleSystem>() },
                    { BattleConst.ClubRoyalWordEffectType.MatchUp, new List<ParticleSystem>() },
                    { BattleConst.ClubRoyalWordEffectType.Hit, new List<ParticleSystem>() }
                }
            };
        
        private List<Animator> instantiatedBallObjects = new List<Animator>();
        private List<ClubRoyalInGameAbsorbBallUI> instantiatedAbsorbBallUIs = new List<ClubRoyalInGameAbsorbBallUI>();
        
        private const string ballModelPath = "3D/Common/ball/lowPolyBallGvG.prefab";
        private const string absorbBallPath = "Prefabs/UI/Page/ClubRoyalInGame/ClubRoyalInGameAbsorbBallUI.prefab";

        public void SetResourceLoader(ResourcesLoader loader)
        {
            resourcesLoader = loader;
        }

        public async UniTask<GameObject> GetOrCreateCharacterModel(long mCharaId, bool isAlly, CancellationToken token)
        {
            var side = isAlly ? GuildBattleCommonConst.GuildBattleTeamSide.Left : GuildBattleCommonConst.GuildBattleTeamSide.Right;
            instantiatedCharacterModels[side].TryGetValue(mCharaId, out var list);
            if (list == null)
            {
                list = new List<GameObject>();
                instantiatedCharacterModels[side].Add(mCharaId, list);
            }

            var nonUsingModel = list.FirstOrDefault(model => !model.gameObject.activeSelf);
            if (nonUsingModel != null)
            {
                return nonUsingModel;
            }

            var modelTeamSide = isAlly ? BattleConst.TeamSide.Left : BattleConst.TeamSide.Right;
            var address = PageResourceLoadUtility.GetCharacter3DLowModelGameObjectPath(Convert.ToInt32(mCharaId.ToString().Substring(0, 5)), modelTeamSide);
            GameObject ret = null;
            await resourcesLoader.LoadAssetAsync<GameObject>(address, go =>
            {
                ret = Instantiate(go);
                list.Add(ret);
            }, token);

            return ret;
        }
        
        public async UniTask<ParticleSystem> GetOrCreateEffectParticle(BattleConst.ClubRoyalBattleEffectType effectType, CancellationToken token)
        {
            var list = instantiatedBattleEffects[effectType];
            var nonUsingObject = list.FirstOrDefault(obj => !obj.gameObject.activeSelf);
            if (nonUsingObject != null)
            {
                nonUsingObject.gameObject.SetActive(true);
                return nonUsingObject;
            }
            
            var address = PageResourceLoadUtility.GetClubRoyalInGameBattleEffect(effectType);
            ParticleSystem ret = null;
            await resourcesLoader.LoadAssetAsync<GameObject>(address, go =>
            {
                var newObj = Instantiate(go);
                ret = newObj.GetComponent<ParticleSystem>();
                list.Add(ret);
            }, token);

            return ret;
        }

        public async UniTask<Animator> GetOrCreateBallObject(CancellationToken token)
        {
            var nonUsingObject = instantiatedBallObjects.FirstOrDefault(obj => !obj.gameObject.activeSelf);
            if (nonUsingObject != null)
            {
                nonUsingObject.gameObject.SetActive(true);
                return nonUsingObject;
            }

            Animator ret = null;
            await resourcesLoader.LoadAssetAsync<GameObject>(ballModelPath, go =>
            {
                var newObj = Instantiate(go);
                ret = newObj.GetComponent<Animator>();
                instantiatedBallObjects.Add(ret);
            }, token);

            return ret;
        }
        
        public async UniTask<ParticleSystem> GetOrCreateWordEffectParticle(BattleConst.ClubRoyalWordEffectType effectType, GuildBattleCommonConst.GuildBattleTeamSide side, CancellationToken token)
        {
            var list = instantiatedWordEffects[(int)side][effectType];
            var nonUsingObject = list.FirstOrDefault(obj => !obj.gameObject.activeSelf);
            if (nonUsingObject != null)
            {
                nonUsingObject.gameObject.SetActive(true);
                return nonUsingObject;
            }
            
            var address = PageResourceLoadUtility.GetClubRoyalInGameWordEffect(effectType, side == GuildBattleCommonConst.GuildBattleTeamSide.Left);
            ParticleSystem ret = null;
            await resourcesLoader.LoadAssetAsync<GameObject>(address, go =>
            {
                var newObj = Instantiate(go);
                ret = newObj.GetComponent<ParticleSystem>();
                list.Add(ret);
            }, token);

            return ret;
        }
        
        public async UniTask PlayAbsorbBallUI(Vector3 startPosition, Vector3 endPosition, Action endCallback, CancellationToken token)
        {
            var nonUsingObject = instantiatedAbsorbBallUIs.FirstOrDefault(obj => !obj.gameObject.activeSelf);
            if (nonUsingObject != null)
            {
                nonUsingObject.PlayMoveAnimation(startPosition, endPosition, endCallback);
                return;
            }
            
            ClubRoyalInGameAbsorbBallUI ret = null;
            await resourcesLoader.LoadAssetAsync<GameObject>(absorbBallPath, go =>
            {
                var newObj = Instantiate(go, AbsorbEffectRoot, false);
                ret = newObj.GetComponent<ClubRoyalInGameAbsorbBallUI>();
                instantiatedAbsorbBallUIs.Add(ret);
            }, token);

            ret.PlayMoveAnimation(startPosition, endPosition, endCallback);
        }

        public void ReturnCharacterModel(GameObject characterModel)
        {
            characterModel.SetActive(false);
        }

        public void ReturnEffectParticle(ParticleSystem particle)
        {
            particle.gameObject.SetActive(false);
        }
        
        public void ReturnWordParticle(ParticleSystem particle)
        {
            particle.gameObject.SetActive(false);
        }
        
        public void ReturnBallModel(GameObject ballModel)
        {
            ballModel.SetActive(false);
            ballModel.transform.localPosition = Vector3.zero;
        }
    }
}