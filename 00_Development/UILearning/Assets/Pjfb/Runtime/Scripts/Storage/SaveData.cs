using System;
using System.Collections.Generic;
using Pjfb.Club;
using Pjfb.Encyclopedia;
using Pjfb.Gacha;
using Pjfb.Menu;
using Pjfb.News;
using Pjfb.Ranking;
using Pjfb.RecommendChara;
using Pjfb.Shop;
using UnityEngine;
using static Pjfb.Rivalry.RivalryManager;

namespace Pjfb.Storage {
    
    [Serializable]
    public class SaveData {
        public bool isTermsAgreed = false;
        public long masterVersion = 0;
        public string catalogHash = string.Empty;
        public AppConfig appConfig = new AppConfig();
        public bool promptedPushPermission = false;
        
        /// <summary>強化選手 育成選手選択画面並べ替え/絞り込みデータ</summary>
        public BaseCharacterSortFilterData trainingSelectBaseCharacterSortFilterData = new();
        /// <summary>強化選手 サポート選手選択画面並べ替え/絞り込みデータ</summary>
        public BaseCharacterSortFilterData trainingSupportSelectBaseCharacterSortFilterData = new();
        /// <summary>強化選手 フレンド選択画面並べ替え/絞り込みデータ</summary>
        public BaseCharacterSortFilterData trainingFriendSelectBaseCharacterSortFilterData = new();
        /// <summary>強化選手 選手強化/能力開放画面並べ替え/絞り込みデータ</summary>
        public BaseCharacterSortFilterData growthLiberationListBaseCharacterSortFilterData = new ();
        /// <summary>強化選手 選手一覧/解放画面並べ替え/絞り込みデータ</summary>
        public BaseCharacterSortFilterData listBaseCharacterSortFilterData = new();
        /// <summary>強化選手 フレンド貸出設定画面並べ替え/絞り込みデータ</summary>
        public BaseCharacterSortFilterData friendBorrowingBaseCharacterSortFilterData = new();
        /// <summary>育成済み選手 選手一覧画面並べ替え/絞り込みデータ</summary>
        public SuccessCharacterSortFilterData listSuccessCharacterSortFilterData = new();
        /// <summary>育成済み選手 選手移籍画面並べ替え/絞り込みデータ</summary>
        public SuccessCharacterSortFilterData sellSuccessCharacterSortFilterData = new();
        /// <summary>育成済み選手 選手お気に入り画面並べ替え/絞り込みデータ</summary>
        public SuccessCharacterSortFilterData favoriteSuccessCharacterSortFilterData = new();
        /// <summary>育成済み選手 デッキ編成（選手選択）画面並べ替え/絞り込みデータ</summary>
        public SuccessCharacterSortFilterData deckCharacterSelectSuccessCharacterSortFilterData = new();
        /// <summary>スペシャルサポートカード 一覧画面並べ替え/絞り込みデータ</summary>
        public SpecialSupportCardSortFilterData listSpecialSupportCardSortFilterData = new();
        /// <summary>スペシャルサポートカード スペシャルサポートカード選択画面並べ替え/絞り込みデータ</summary>
        public SpecialSupportCardSortFilterData trainingSelectSpecialSupportCardSortFilterData = new();
        /// <summary>エクストラサポートカード エクストラサポートカード選択画面並び替え/絞り込みデータ</summary>
        public SpecialSupportCardSortFilterData trainingSelectExtraSupportCardSortFilterData = new();
        /// <summary>サポート器具 選手一覧画面並べ替え/絞り込みデータ</summary>
        public SupportEquipmentSortFilterData listSupportEquipmentSortFilterData = new();
        /// <summary>サポート器具 選手移籍画面並べ替え/絞り込みデータ</summary>
        public SupportEquipmentSortFilterData sellSupportEquipmentSortFilterData = new();
        /// <summary>サポート器具 選手お気に入り画面並べ替え/絞り込みデータ</summary>
        public SupportEquipmentSortFilterData favoriteSupportEquipmentSortFilterData = new();
        /// <summary>サポート器具 選択画面並べ替え/絞り込みデータ</summary>
        public SupportEquipmentSortFilterData trainingSelectSupportEquipmentSortFilterData = new();
        /// <summary>サポート器具 一括売却絞り込みデータ</summary>
        public SupportEquipmentSortFilterData AllSellSupportEquipmentSortFilterData = new();
        /// <summary>ユーザーアイコン 並び替え/絞り込みデータ</summary>
        public UserIconSortFilterData userIconSortFilterData = new ();
        /// <summary>称号 並べ替え/絞り込みデータ</summary>
        public UserTitleSortFilterData userTitleSortFilterData = new ();
        /// <summary>バッチ 並び替え/絞り込みデータ</summary>
        public MyBadgeSortFilterData myBadgeSortFilterData = new MyBadgeSortFilterData();
        /// <summary> アドバイザー 強化/解放画面 並び替え/絞り込みデータ </summary>
        public AdviserSortFilterData adviserGrowthLiberationSortFilterData = new AdviserSortFilterData();
        /// <summary> アドバイザー 一覧画面 並び替え/絞り込みデータ </summary>
        public AdviserSortFilterData adviserListSortFilterData = new AdviserSortFilterData();
        /// <summary> アドバイザー サポート選択画面 並び替え/絞り込みデータ </summary>
        public AdviserSortFilterData trainingAdviserListSortFilterData = new AdviserSortFilterData();
        /// <summary> アドバイザー デッキ編成画面　並び替え/絞り込みデータ </summary>
        public AdviserSortFilterData adviserDeckListSortFilterData = new AdviserSortFilterData();
        /// <summary> マッチスキル　並び替え/絞り込みデータ </summary>
        public CombinationSkillSortFilterData listCombinationMatchSortFilterData = new();
        /// <summary> トレーニングスキル　並び替え/絞り込みデータ </summary>
        public CombinationSkillSortFilterData listCombinationTrainingSortFilterData = new();
        /// <summary> コレクションスキル　並び替え/絞り込みデータ </summary>
        public CombinationSkillSortFilterData listCombinationCollectionSortFilterData = new();
        /// <summary> マッチスキル絞り込みの選手アイコン 並び替え/絞り込みデータ </summary>
        public CombinationSkillCharaIconSortFilterData combinationMatchCharaIconSortFilterData = new();
        /// <summary> マッチスキル絞り込みのアドバイザーアイコン 並び替え/絞り込みデータ </summary>
        public CombinationSkillCharaIconSortFilterData combinationMatchAdviserIconSortFilterData = new();
        /// <summary> トレーニングスキル絞り込みの選手アイコン 並び替え/絞り込みデータ </summary>
        public CombinationSkillCharaIconSortFilterData combinationTrainingCharaIconSortFilterData = new();
        /// <summary> トレーニングスキル絞り込みのアドバイザーアイコン 並び替え/絞り込みデータ </summary>
        public CombinationSkillCharaIconSortFilterData combinationTrainingAdviserIconSortFilterData = new();
        /// <summary> トレーニングスキル絞り込みのスペシャルサポートカードアイコン 並び替え/絞り込みデータ </summary>
        public CombinationSkillCharaIconSortFilterData combinationTrainingSpecialSupportIconSortFilterData = new();
        /// <summary> コレクションスキル絞り込みの選手アイコン 並び替え/絞り込みデータ </summary>
        public CombinationSkillCharaIconSortFilterData combinationCollectionCharaIconSortFilterData = new();
        /// <summary> コレクションスキル絞り込みのアドバイザーアイコン 並び替え/絞り込みデータ </summary>
        public CombinationSkillCharaIconSortFilterData combinationCollectionAdviserIconSortFilterData = new();
        /// <summary> コレクションスキル絞り込みのスペシャルサポートカードアイコン 並び替え/絞り込みデータ </summary>
        public CombinationSkillCharaIconSortFilterData combinationCollectionSpecialSupportIconSortFilterData = new();
        /// <summary> 自チーム発動マッチスキル　並び替え/絞り込みデータ </summary>
        public CombinationSkillSortFilterData playerCombinationMatchSortFilterData = new();
        /// <summary> 敵チーム発動マッチスキル　並び替え/絞り込みデータ </summary>
        public CombinationSkillSortFilterData enemyCombinationMatchSortFilterData = new();
        /// <summary> 自チーム発動マッチスキル絞り込みの選手アイコン　並び替え/絞り込みデータ </summary>
        public CombinationSkillCharaIconSortFilterData playerCombinationMatchCharaIconSortFilterData = new();
        /// <summary> 敵チーム発動マッチスキル絞り込みの選手アイコン　並び替え/絞り込みデータ </summary>
        public CombinationSkillCharaIconSortFilterData enemyCombinationMatchCharaIconSortFilterData = new();
        
        public List<VoiceBadgeReadData> voiceBadgeReadData = new(); 
        /// <summary>トレーニング用のデータ</summary>
        public TrainingLocalSaveData trainingData = new TrainingLocalSaveData();

        public GachaLocalSaveData gachaData = new GachaLocalSaveData();

        /// <summary>ショップの確認済みのパックIDデータ</summary>
        public string shopConfirmedBillingRewardIdString = string.Empty;

        /// <summary>ショップのステップアップ解放演出確認済みのパックIDデータ</summary>
        public string shopConfirmedStepUpUnLockEffectBillingRewardIdString = string.Empty;
        
        /// <summary>ショップの購入方法切り替え情報</summary>
        public ShopManager.PurchaseType shopPurchaseType = ShopManager.PurchaseType.Price;
        
        /// <summary>ショップの購入方法切り替えフラグ</summary>
        public bool shopPurchaseTypeChangeFlag = false;
        
        /// <summary>試合スキップ</summary>
        public bool skipMatchData = false;

        /// <summary>確認済の称号リスト</summary>
        public List<long> viewedTitles = new();
        
        /// <summary>確認済のバッチリスト</summary>
        public long[] viewedMyBadgeIds = Array.Empty<long>();
        
        /// <summary>確認済のトレーナーカード着せ替えリスト</summary>
        public List<long> viewedCustomizeFrameIds = new();
        
        /// <summary>確認済みのトレーナーカード表示キャラ</summary>
        public List<long> viewedDisplayCharaIds = new();

        /// <summary>クラブの通知確認用</summary>
        public ClubCheckNotificationData clubCheckNotificationData = new ();
        
        /// <summary>確認済みのマッチスキルIDデータ</summary>
        public string combinationMatchIdString = string.Empty;
        /// <summary>確認済みのトレーニングスキルIDデータ</summary>
        public string combinationTrainingIdString = string.Empty;

        /// <summary>確認済みのイベントIDデータ</summary>
        public string festivalIdString = string.Empty;

        /// <summary>ライバルリー奪選手選択</summary>
        public StealCharaEnemyDataContainer stealCharaEnemyDataContainer = new StealCharaEnemyDataContainer();

        /// <summary>お知らせ記事強制表示データ</summary>
        public NewsManager.NewsArticleForcedDisplayData NewsArticleForcedDisplayData = new NewsManager.NewsArticleForcedDisplayData();
        
        /// <summary>確認済みのユーザーのサポート器具Idリスト</summary>
        public List<long> viewedUserSupportEquipmentIdList = new();
        
        /// <summary>注目の育成選手通知確認用</summary>
        public RecommendCharaCheckNotificationData RecommendCharaCheckNotificationData = new ();

        //// <summary> キャラのレベルアップ最大時チュートリアルを再生したか </summary>
        public bool isPlayCharaLevelUpMaxTutorial = false;
        
        /// <summary> 確認済みのclientPreviewRankingIdリスト </summary>
        public List<long> clientPreviewRankingIdConfirmList = new List<long>();
        
        /// <summary> ランキングの遊び方表示 </summary>
        public List<RankingTabSheetType> rankingHelpViewedSet = new List<RankingTabSheetType>();
        
        /// <summary> 確認済みのチュートリアルリスト </summary>
        public List<long> tutorialIdConfirmList = new List<long>();

        /// <summary> 防衛だけはだめだよポップアップ表示フラグ </summary>
        public bool isPartyBalanceAnnounceModalOpened = false;

        /// <summary> 確認済みのm_colosseum_EventId </summary>
        public List<long> viewedMColosseumEventIdList = new List<long>();
    }

    [Serializable]
    public class ImmutableSaveData {
        public string appToken => _appToken;
        
        [SerializeField]
        private string _appToken = "";

        public ImmutableSaveData(){
        }
        public ImmutableSaveData( string appToken ){
            _appToken = appToken;
        }
    }
}
