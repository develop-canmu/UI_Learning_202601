using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Pjfb
{
    /// <summary>ページに必要なリソースの読み込みのUtilityクラス</summary>
    public static class PageResourceLoadUtility
    {
        /// <summary>ResourcesLoader</summary>
        public static ResourcesLoader resourcesLoader 
        {
            get { return AppManager.Instance.UIManager.PageManager.GetResourcesLoader(); } 
        }
        
        /// <summary>
        /// 非同期読み込み
        /// </summary>
        public static async UniTask LoadAssetAsync<T>(string key, Action<T> callback, CancellationToken token) where T : UnityEngine.Object
        {
            await resourcesLoader.LoadAssetAsync<T>(key, callback, token);
        }
        
        /// <summary>
        /// 非同期読み込み
        /// Release時に端末からキャッシュを削除
        /// </summary>
        public static async UniTask LoadAssetAsync<T>(string key, Action<T> callback, bool deleteCacheOnRelease, CancellationToken token) where T : UnityEngine.Object
        {
            await resourcesLoader.LoadAssetAsync<T>(key, callback, deleteCacheOnRelease, token);
        }

        /// <summary>
        /// 同期読み込み
        /// </summary>
        public static T LoadAsset<T>(string key) where T : UnityEngine.Object
        {
            return resourcesLoader.LoadAsset<T>(key);
        }
        
        /// <summary>
        /// 同期読み込み
        /// Release時に端末からキャッシュを削除
        /// </summary>
        public static T LoadAsset<T>(string key, bool deleteCacheOnRelease) where T : UnityEngine.Object
        {
            return resourcesLoader.LoadAsset<T>(key, deleteCacheOnRelease);
        }

        /// <summary>
        /// アイテムアイコンパス
        /// MPoint.id
        /// </summary>
        public static string GetItemIconImagePath(long id)
        { 
            // symbolName定義あり場合symbolNameを使います
            string symbolName = (id > 0) ? MasterManager.Instance.pointMaster.FindData(id).symbolName : "";
            if (symbolName.StartsWith("item_icon_"))
            {
                return ResourcePathManager.GetPath("ItemIcon", symbolName.Replace("item_icon_",""));
            }

            return ResourcePathManager.GetPath("ItemIcon", id);
        }
                
        /// <summary>
        /// 選手固有のアイコンパス
        /// MCharaParent.id
        /// </summary>
        public static string GetCharacterParentIconImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterParentIcon", id);
        }
        
        /// <summary>
        /// サポート器具
        /// ???
        /// </summary>
        public static string GetSupportEquipmentIconImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterIcon", id);
        }

        
        /// <summary>
        /// 強化指定選手アイコンパス
        /// MChara.id
        /// </summary>
        public static string GetCharacterIconImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterIcon", id);
        }
        
        /// <summary>
        /// 育成選手アイコンパス
        /// MChara.id
        /// </summary>
        public static string GetVariableCharacterIconImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterIcon", id);
        }
        
        /// <summary>
        /// 選手カードパス
        /// </summary>
        public static string GetCharacterCardImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterCard", id);
        }
        
        /// <summary>
        /// 選手カードパス
        /// </summary>
        public static string GetCharacterCardHomeImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterCardHome", id);
        }
        
        /// <summary>
        /// アドバイザーアイコンパス
        /// MChara.id
        /// </summary>
        public static string GetAdviserIconImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterIcon", id);
        }
        
        /// <summary>
        /// ホーム背景
        /// </summary>
        public static string GetCharacterCardHomeBackgroundImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterCardHomeBackground", id);
        }
        
        /// <summary>
        /// ホーム背景テキスト
        /// </summary>
        public static string GetCharacterCardHomeTextImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterCardHomeText", id);
        }
        
        /// <summary>
        /// 選手カード背景パス
        /// </summary>
        public static string GetCharacterCardBackgroundImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterCardBackground", id);
        }
        
        /// <summary>
        /// スペシャルサポートカードパス
        /// MChara.id
        /// </summary>
        public static string GetSpecialSupportCharacterCardImagePath(long id)
        {
            return ResourcePathManager.GetPath("SpecialSupportCharacterCard", id);
        }

        /// <summary>
        /// キャラピースアイコンパス
        /// MChara.id
        /// </summary>
        public static string GetCharacterPieceImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterPiece", id);
        }
        
        /// <summary>
        /// MIcon.id
        /// ユーザーアイコンパス
        /// </summary>
        public static string GetUserIconImagePath(long id)
        {
            return ResourcePathManager.GetPath("UserIcon", id);
        }
        
        /// <summary>
        /// ユーザー称号アイコンパス
        /// MTitle.id
        /// </summary>
        public static string GetUserTitleImagePath(long id)
        {
            return ResourcePathManager.GetPath("UserTitle", id);
        }
        
        /// <summary>
        /// チャットスタンプアイコンパス
        /// MChatStamp.id
        /// </summary>
        public static string GetChatStampIconImagePath(long id)
        {
            return ResourcePathManager.GetPath("ChatStamp", id);
        }
        
        /// <summary>
        /// デッキランクアイコンパス
        /// </summary>
        public static string GetDeckRankImagePath(long id, DeckRankImage.ImageSize size)
        {
            return $"Images/DeckRank{size}/deck_rank_icon_{id}.png";
        }
        
        /// <summary>
        /// クラブランクアイコンパス
        /// </summary>
        public static string GetClubRankImagePath(long id, DeckRankImage.ImageSize size)
        {
            return $"Images/ClubRank{size}/club_rank_icon_{id}.png";
        }
        
        /// <summary>
        /// 育成選手ランクアイコンパス
        /// </summary>
        public static string GetCharacterRankIconImagePath(long id, CharacterRankImage.Size size)
        {
            return $"Images/CharacterRank{size}/character_rank_icon_{id}.png";
        }
        
        /// <summary>
        /// TODO インゲームランクアイコンパス
        /// </summary>
        public static string GetCharacterInGameRankIconImagePath(long id)
        {
            return $"Images/CharacterInGameRank/ingame_font_rank_characterinfo_{id:D3}.png";
        }
        
        /// <summary>
        /// 練習カードパス
        /// </summary>
        public static string GetPracticeCardImagePath(long id)
        {
            return ResourcePathManager.GetPath("PracticeCard", id);
        }
        
        /// <summary>
        /// ステータスランクパス
        /// </summary>
        public static string GetCharacterStatusRankImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterStatusRank", id);
        }
        
        /// <summary>スキルアイコンパス</summary>
        public static string GetCharacterSkillImagePath(int id)
        {
            return ResourcePathManager.GetPath("CharacterSkill", id);
        }
        
        /// <summary>スキルレアリティの下地イメージパス</summary>
        public static string GetCharacterSkillRarityBaseImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterSkillRarityBase", id);
        }
        
        /// <summary>キャラスパインパス</summary>
        public static string GetCharacterSpinePath(string id)
        {
            return ResourcePathManager.GetPath("CharacterSpine", id);
        }
        
        public static string GetRarityImagePath(long id)
        {
            return ResourcePathManager.GetPath("Rarity", id);
        }
        
        /// <summary>
        /// Advログに表示するアイコン
        /// </summary>
        public static string GetCharacterAdvIconImagePath(string id)
        {
            return ResourcePathManager.GetPath("CharacterAdvIcon", id);
        }
        
        public static string GetTrainingHowToImagePath(string id)
        {
            return ResourcePathManager.GetPath("TrainingHowTo", id);
        }
        
        public static string GetRivalryHowToImagePath(string id)
        {
            return ResourcePathManager.GetPath("RivalryHowTo", id);
        }
        
        public static string GetEventHowToImagePath(string id)
        {
            return ResourcePathManager.GetPath("EventHowTo", id);
        }

        public static string GetRankingHowToPath(string id)
        {
            return ResourcePathManager.GetPath("RankingHowTo", id);
        }
        
        public static string GetHowToPlayPath(string id)
        {
            return ResourcePathManager.GetPath("HowToPlay", id);
        }
        
        /// <summary>
        /// デッキ編成画像
        /// </summary>
        public static string GetCharacterDeckImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterDeck", id);
        }
        
        /// <summary>
        /// インゲームのマッチアップ用キャラアイコン画像
        /// </summary>
        public static string GetCharacterInGameIconPath(long id)
        {
            return ResourcePathManager.GetPath("CharacterInGameIcon", id);
        }
        
        public static string GetRivalryEventBannerImagePath(long id)
        {
            return $"Images/RivalryEventBanner/rivalry_event_banner_{id}.png";
        }
        
        public static string GetRivalryRegularBannerImagePath(long difficulty)
        {
            return $"Images/RivalryRegularBanner/rivalry_regular_banner_{difficulty}.png";
        }
        
        public static string GetRivalryTopBannerImagePath(long mHuntId)
        {
            return $"Images/RivalryTopBanner/rivalry_top_banner_{mHuntId}.png";
        }

        public static string GetRivalryEventFrameImagePath(long difficulty)
        {
            return $"Images/RivalryEventFrame/rivalry_event_frame_{difficulty}.png";
        }
        
        public static string GetCharacterCardGachaImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterCardGacha", id);
        }
        
        public static string GetCharacterCardGachaBackgroundImagePath(long id)
        {
            return ResourcePathManager.GetPath("CharacterCardGachaBackground", id);
        }
        
        public static string GetCharacterTypeImagePath(CharacterType type, CharacterCharacterTypeImage.ImageType imageType)
        {
            string id = String.Empty;
            switch (type)
            {
                case CharacterType.Speed: id = "speed"; break;
                case CharacterType.Technique: id = "technique"; break;
                case CharacterType.Stamina: id = "stamina"; break;
                case CharacterType.Physical: id = "physical"; break;
                case CharacterType.FieldOfView: id = "sight"; break;
                case CharacterType.Kick: id = "kick"; break;
                case CharacterType.Intelligence: id = "intelligence"; break;
                case CharacterType.Condition: id = "condition"; break;
                default: throw new NotImplementedException();
            }

            switch (imageType)
            {
                case CharacterCharacterTypeImage.ImageType.Badge: return ResourcePathManager.GetPath("CharacterTypeIcon", id);
                case CharacterCharacterTypeImage.ImageType.Icon: return ResourcePathManager.GetPath("StatusTypeIcon", id); 
                default: throw new NotImplementedException();
            }
        }

        public static string GetStatusTypeIconImagePath(CharacterStatusType type)
        {
            string id = String.Empty;
            switch (type)
            {
                case CharacterStatusType.Speed: id = "speed"; break;
                case CharacterStatusType.Technique: id = "technique"; break;
                case CharacterStatusType.Stamina: id = "stamina"; break;
                case CharacterStatusType.Physical: id = "physical"; break;
                case CharacterStatusType.Kick: id = "kick"; break;
                case CharacterStatusType.Intelligence: id = "intelligence"; break;
                case CharacterStatusType.ShootRange: id = "shootrange"; break;
                default: throw new NotImplementedException();
            }
            return ResourcePathManager.GetPath("StatusTypeIcon", id);
        }
        
        public static string GetPracticeSkillIconImagePath(long id)
        {
            return ResourcePathManager.GetPath("PracticeSkillIcon", id);
        }

        /// <summary>
        /// アドバイザースキルアイコンパス
        /// m_ability.id
        /// </summary>
        public static string GetAdviserSkillIconImagePath(long id)
        {
            return ResourcePathManager.GetPath("AdviserSkillIcon", id);
        }
        
        /// <summary>
        /// クラブ・ロワイヤル中のバフアイコンパス
        /// BuffのID
        /// </summary>
        public static string GetClubRoyalBuffIconImagePath(long id)
        {
            return ResourcePathManager.GetPath("ClubRoyalBuffIcon", id);
        }

        
        public static string GetRivalryTeamRarityIconImagePath(long id)
        {
            return ResourcePathManager.GetPath("DeckRarityIcon", id);
        }
        
        public static string GetRoleNumberIconImagePath(RoleNumber type)
        {
            string id = String.Empty;
            switch (type)
            {
                case RoleNumber.FW: id = "fw"; break;
                case RoleNumber.MF: id = "mf"; break;
                case RoleNumber.DF: id = "df"; break;
                default: throw new NotImplementedException();
            }
            return ResourcePathManager.GetPath("RoleNumberIcon", id);
        }
        
        /// <summary>キャラ獲得演出オブジェクト</summary>
        public static string GetCharacterGetEffectObjectPath(long rarity, CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Character:
                case CardType.Adviser:
                {
                    return ResourcePathManager.GetPath("CharacterGetEffectObject", rarity);
                }
                case CardType.SpecialSupportCharacter : return ResourcePathManager.GetPath("SpecialSupportCharacterGetEffectObject", rarity);
                default: throw new NotImplementedException();
            }
        }
        
        /// <summary>キャラ獲得演出</summary>
        public static string GetCharacterGetEffectPath(long mCharaId)
        {
            return ResourcePathManager.GetPath("CharacterGetEffect", mCharaId);
        }
        
        public static string GetCharacterSecondBallCutinPath(long id)
        {
            return ResourcePathManager.GetPath("CharacterSecondBallCutin", id);
        }
        
        public static string GetAbilityNameImagePath(long id)
        {
            return ResourcePathManager.GetPath("AbilityNameImage", id);
        }
        
        /// <summary>
        /// PvPランクアイコンパス
        /// </summary>
        public static string GetColosseumRankImagePath(long id)
        {
            return $"Images/ColosseumRankIcon/colosseum_rank_icon_{id}.png";
        }

        /// <summary>
        /// PvPルームアイコンパス
        /// </summary>
        public static string GetColosseumRoomImagePath(long id)
        {
            return $"Images/ColosseumRoomIcon/colosseum_room_icon_{id}.png";
        }
        
        public static string GetSpecialSupportCharacterCardGachaImagePath(long id)
        {
            return ResourcePathManager.GetPath("SpecialSupportCharacterCardGacha", id);
        }
        
        public static string GetEventLogoImagePath(long id)
        {
            return ResourcePathManager.GetPath("EventLogo", id);
        }
        
        public static string GetEventTopBackgroundImagePath(long id)
        {
            return ResourcePathManager.GetPath("EventTopBackground", id);
        }
        
        public static string GetEventTopCharacterImagePath(long id)
        {
            return ResourcePathManager.GetPath("EventTopCharacter", id);
        }

        public static string GetEventTopStoryButtonImagePath(long id)
        {
            return ResourcePathManager.GetPath("EventTopStoryButton", id);
        }
        
        public static string GetEventTopEventPointRewardButtonImagePath(long id)
        {
            return ResourcePathManager.GetPath("EventTopEventPointRewardButton", id);
        }

        public static string GetCharacter3DLowModelGameObjectPath(long charaSameId, BattleConst.TeamSide side)
        {
            if (side == BattleConst.TeamSide.Left)
            {
                return ResourcePathManager.GetPath("Character3DModelLowBlue", charaSameId);
            }
            else
            {
                return ResourcePathManager.GetPath("Character3DModelLowRed", charaSameId);
            }
        }

        public static string GetUserIconEffectPath(long id)
        {
            return $"Effects/UserIconEffects/UserIconEffect_{id}.prefab";
        }
        
        public static string GetUserTitleEffectPath(long id)
        {
            return $"Effects/TitleEffects/TitleEffect_{id}.prefab";
        }

        public static string GetCharacterIconEffectPath(long id)
        {
            return $"Effects/CharacterIconRarityEffects/CharacterIconRarityEffect_{id}.prefab";
        }

        public static string GetSpecialSupportCharacterCardEffectPath(long id)
        {
            return $"Effects/SpecialSupportCardRarityEffects/SpecialSupportCardRarityEffect_{id}.prefab";
        }
        
        /// <summary>キャライラストエフェクト</summary>
        public static string GetCharacterCardEffectPath(long id)
        {
            return ResourcePathManager.GetPath("CharacterCardEffect", id);
        }
        
        /// <summary>キャライラストエフェクト</summary>
        public static string GetCharacterCardHomeEffectPath(long id)
        {
            return ResourcePathManager.GetPath("CharacterCardHomeEffect", id);
        }
        
        /// <summary>キャライラストエフェクト</summary>
        public static string GetCharacterCardGachaEffectPath(long id)
        {
            return ResourcePathManager.GetPath("CharacterCardGachaEffect", id);
        }
        
        /// <summary>クラブアイコン画像</summary>
        public static string GetClubEmblemPath(long id)
        {
            return ResourcePathManager.GetPath("ClubEmblem", id);
        }
        
        /// <summary>入れ替え戦ラベル</summary>
        public static string GetLeagueMatchSeasonEndLabelPath(long id)
        {
            return ResourcePathManager.GetPath("LeagueMatchSeasonEndLabel", id);
        }
        
        /// <summary>バッチの画像パス</summary>
        public static string GetMyBadgeImagePath(long id)
        {
            return ResourcePathManager.GetPath("MyBadgeImage", id);
        }
        
        /// <summary>バッチのエフェクトパス</summary>
        public static string GetMyBadgeEffectPath(long id)
        {
            return ResourcePathManager.GetPath("MyBadgeEffect", id);
        }
        
        public static string GetProfilePartIconImagePath(long id)
        {
            ProfilePartMasterObject master = MasterManager.Instance.profilePartMaster.FindData(id);
            switch (master.partType)
            {
                case ProfilePartMasterObject.ProfilePartType.ProfileFrame:
                    return ResourcePathManager.GetPath("CustomizeProfileFrame");
                case ProfilePartMasterObject.ProfilePartType.DisplayCharacter:
                    return ResourcePathManager.GetPath("CharacterWholeImageIcon");
                default:
                    throw new NotImplementedException();
            }
        }

        public static string GetTrainerCardCustomizeImagePath(long id)
        {
            return ResourcePathManager.GetPath("TrainerCardCustomizeImage", id);
        }
        
        /// <summary>トレーナーカード表示キャラのアイコンパス</summary>
        public static string GetDisplayCharacterIconPath(long id)
        {
            return ResourcePathManager.GetPath("TrainerCardDisplayCharacterIcon", id);
        }
        
        /// <summary>トレーナーカード表示キャラの画像パス</summary>
        public static string GetDisplayCharacterImagePath(long id)
        {
            return ResourcePathManager.GetPath("TrainerCardDisplayCharacterImage", id);
        }
        
        /// <summary>トレーナーカード表示キャラのエフェクトパス</summary>
        public static string GetDisplayCharacterEffectPath(long id)
        {
            return ResourcePathManager.GetPath("TrainerCardDisplayCharacterEffect", id);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetClubRoyalInGameBattleEffect(BattleConst.ClubRoyalBattleEffectType type)
        {
            switch (type)
            {
                case BattleConst.ClubRoyalBattleEffectType.Dribble:
                    return "Effects/prefabs/VFX_Dash.prefab";
                case BattleConst.ClubRoyalBattleEffectType.Spawn:
                    return "Effects/prefabs/VFX_Arrival.prefab";
                case BattleConst.ClubRoyalBattleEffectType.DamageTaken:
                    return "Effects/prefabs/VFX_Hit.prefab";
                case BattleConst.ClubRoyalBattleEffectType.DamageTakenBLM:
                    return "Effects/prefabs/VFX_Shocknoise.prefab";
                case BattleConst.ClubRoyalBattleEffectType.DeadCharacter:
                    return "Effects/prefabs/VFX_Vanish_sphere.prefab";
                case BattleConst.ClubRoyalBattleEffectType.DeadBLM:
                    return "Effects/prefabs/VFX_Extinction.prefab";
            }
            
            return string.Empty;
        }
        
        public static string GetClubRoyalInGameWordEffect(BattleConst.ClubRoyalWordEffectType type, bool isAlly)
        {
            switch (type)
            {
                case BattleConst.ClubRoyalWordEffectType.Dash:
                    return isAlly ? "Effects/prefabs/VFX_Word_blue_dada.prefab" : "Effects/prefabs/VFX_Word_red_dada.prefab";
                case BattleConst.ClubRoyalWordEffectType.Spawn:
                    return isAlly ? "Effects/prefabs/VFX_Word_blue_za.prefab" : "Effects/prefabs/VFX_Word_red_za.prefab";
                case BattleConst.ClubRoyalWordEffectType.MatchUp:
                    return isAlly ? "Effects/prefabs/VFX_Word_blue_zaza.prefab" : "Effects/prefabs/VFX_Word_red_zaza.prefab";
                case BattleConst.ClubRoyalWordEffectType.Hit:
                    return isAlly ? "Effects/prefabs/VFX_Word_blue_ga.prefab" : "Effects/prefabs/VFX_Word_red_ga.prefab";
            }
            
            return string.Empty;
        }

    }
}
