using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using Cysharp.Threading.Tasks;
using CruFramework.ResourceManagement;
using CruFramework;
using CruFramework.Engine.CrashLog;
using CruFramework.Page;
using CruFramework.Utils;
using Pjfb.Runtime.Scripts.Utility;

namespace Pjfb
{
    public enum ModalType
    {
        /// <summary>確認ダイアログ/summary>
        Confirm,
        /// <summary>確認ダイアログ(チェックボックス)/summary>
        ConfirmWithCheckbox,
        /// <summary>利用規約</summary>
        Terms,
        /// <summary>タイトルメニュー</summary>
        TitleMenu,
        /// <summary>ユーザーデータ引継ぎ</summary>
        UserTransfer,
        /// <summary>パスワード変更時のメールアドレス入力</summary>
        PasswordResettingInputMailAddress,
        /// <summary>パスワード変更</summary>
        PasswordResetting,
        /// <summary>お問い合わせ</summary>
        Inquiry,
        /// <summary>お問い合わせ入力</summary>
        InquiryInput,
        /// <summary>お問い合わせ確認画面</summary>
        InquiryConfirm,
        /// <summary>キャラクタの詳細</summary>
        CharacterDetail,
        /// <summary>称号詳細/summary>
        TitleDetail,

        /// <summary>キャラクタのスキル詳細</summary>
        CharacterSkill, 
        /// <summary>トレーニングのサポートキャラクタスキル一覧</summary>
        SupportCharacterDetail,
        /// <summary>トレーニング開始確認</summary>
        TrainingStartConfirm,
        /// <summary>トレーニング休憩確認</summary>
        TrainingRestConfirm,
        /// <summary>トレーニング練習試合確認</summary>
        TrainingPracticeConfirm,
        /// <summary>トレーニングメンバー情報</summary>
        TrainingMemberInfo,
        /// <summary>練習能力一覧</summary>
        TrainingPracticeSkill,
        /// <summary>メニュー</summary>
        TrainingMenu,
        /// <summary>オプション</summary>
        TrainingOption,
        /// <summary>カード合成確認</summary>
        TrainingFusionConfirm,
        /// <summary>おすすめ編成</summary>
        TrainingDeckRecommend,
        /// <summary>育成中キャラクタの詳細</summary>
        TrainingCharacterDetail,
        /// <summary>目標確認モーダル</summary>
        TrainingTargetConfirm,
        /// <summary>トレーニングメニュー詳細</summary>
        TrainingMenuDetail,
        /// <summary>遊び方</summary>
        HowToPlay,
        /// <summary>トレーニングメニュー詳細目標</summary>
        TrainingMenuDetailTarget,
        /// <summary>トレーニングフレンドフォロー</summary>
        TrainingFriendFollow,
        /// <summary>コレクションスキルリスト</summary>
        TrainingCombinationList,
        //// <summary> トレーニング編成強化詳細 </summary>
        TrainingDeckEnhanceDetail,
        //// <summary> トレーニング編成レベルアップ詳細 </summary>
        TrainingDeckEnhanceLevelUpDetail,
        
        /// <summary>レベニューマッチ</summary>
        RevenueMatch,
        /// <summary>インスピレーション確認</summary>
        GetInspiration,
        /// <summary>インスピレーション詳細</summary>
        InspirationDetail,
        /// <summary>カードユニオン情報</summary>
        CardUnionInformation,
        
        /// <summary>獲得ブースト一覧</summary>
        TrainingGetBoostList,
        
        /// <summary>練習カード引き直し確認</summary>
        TrainingCardRedoingConfirm,
        
        /// <summary>練習カードの報酬モーダル</summary>
        TrainingCardReward,
        
        /// <summary>自動トレーニング結果詳細</summary>
        AutoTraininglSummary,
        /// <summary>自動トレーニング育成方針</summary>
        AutoTrainingStatusPolicySetting,
        
        /// <summary>自動トレーニング開始確認</summary>
        AutoTrainingConfirm,
        
        /// <summary>サマリー</summary>
        AutoTrainingSelectedPracticeCardDetail,
        AutoTrainingInspirationList,
        AutoTrainingGetSkillList,
        AutoTrainingGetBoostList,
        
        /// <summary>自動トレーニング中断</summary>
        AutoTrainingAbortConfirm,
        
        /// <summary>即時完了</summary>
        AutoTrainingImmediateCompletion,
        
        /// <summary>パス購入導線</summary>
        AutoTrainingPassEffective,
        
        AutoTrainingRemainingTimesAdd,
        
        /// <summary>自動トレーニングカードユニオン情報</summary>
        AutoTrainingCardUnionInformation,
        
        /// <summary>ミッション</summary>
        Mission,

        /// <summary>報酬獲得ウインドウ</summary>
        Reward,
        
        /// <summary>ホーム画面のお知らせ</summary>
        News,
        /// <summary>ホーム画面のプレゼントボックス</summary>
        PresentBox,
        /// <summary>ホーム画面のロックアイテム</summary>
        LockedItem,
        /// <summary>ホーム画面の初心者ミッションのクリア演出</summary>
        HomeBeginnerMissionClear,
        /// <summary>ホーム画面の初心者ミッションの画面</summary>
        HomeBeginnerMissionPopup,
        /// <summary>キャンペーンバナー「詳細を見る」ボタンがついてるポップアップ</summary>
        CampaignBanner,
        /// <summary>キャンペーンバナー「詳細を見る」ボタンがついてるポップアップ</summary>
        SubscribeInfo,
        /// <summary>機能の汎用解放演出 </summary>
        SystemUnlock,
        
        /// <summary>Advメッセージログ</summary>
        AdvMessageLog,
        
        /// <summary>アイテム不足</summary>
        PointShortage,
        
        /// <summary>ショップパック一覧</summary>
        ShopPack,
        
        /// <summary>ショップ交換所</summary>
        ShopExchange,
        /// <summary>ショップ交換所の交換ポップアップ</summary>
        ShopExchangeConfirm,
        /// <summary>ショップ交換所の更新ポップアップ</summary>
        ShopExchangeUpdate,
        /// <summary>ショップ交換所の一括交換ポップアップ</summary>
        ShopBulkExchangeConfirm,
        /// <summary>ショップ年齢確認</summary>
        ShopAgeCheck,
        /// <summary>特定商取引法</summary>
        TransactionLow,
        /// <summary>資金決済法</summary>
        PaymentLaw,
        /// <summary>シークレットショップポップアップ</summary>
        SecretBanner,
        /// <summary>キャラポジション変更</summary>
        PositionChange,
        /// <summary>キャラ自動編成確認</summary>
        DeckRecommendationsConfirm,
        /// <summary>スペシャルカード詳細</summary>
        SpecialSupportCardDetail,
        /// <summary>スペシャルカード特殊能力説明</summary>
        CharacterCardDescription,
        /// <summary>チームランク更新報酬</summary>
        TeamRankUpdateReward,
        /// <summary>チームランク次回報酬</summary>
        TeamNextRankReward,
        /// <summary>チームランク報酬リスト</summary>
        TeamRankRewardList,
        /// <summary>キャラクター画面の育成キャラ詳細</summary>
        BaseCharacterDetail,
        /// <summary>サクセスキャラ詳細</summary>
        SuccessCharaDetail,
        /// <summary>サクセスキャラ移籍確認</summary>
        SuccessCharaSellConfirm,
        /// <summary>獲得報酬確認</summary>
        GainRewardConfirm,
        /// <summary> アドバイザー詳細 </summary>
        AdviserDetail,
        /// <summary> アドバイザースキル詳細 </summary>
        AdviserSkillDetail,
        /// <summary>アドバイザー用絞り込み/ソート画面</summary>
        AdviserSortFilter,
        /// <summary> アドバイザー強化確認 </summary>
        AdviserGrowthConfirm,
        /// <summary>キャラ自動編成確認</summary>
        AdviserDeckRecommendationsConfirm,
        /// <summary>編成中アドバイザースキル表示</summary>
        AdviserSkillList,
        
        /// <summary>クラブランキング</summary>
        ClubRanking,
        /// <summary>クラブ情報</summary>
        ClubInfo,
        /// <summary>勧誘時のクラブ情報</summary>
        SolicitationClubInfo,
        /// <summary>クラブ編集</summary>
        EditClubTop,
        /// <summary>クラブメンバ役職管理</summary>
        ClubMemberPostManage,
        /// <summary>クラブエンブレム選択</summary>
        ClubSelectEmblem,
        /// <summary>クラブメンバ検索設定</summary>
        ClubFindMemberSetting,
        /// <summary>クラブ作成確認</summary>
        CreateClubConfirm,
        /// <summary>クラブ勧誘</summary>
        ClubInvitationUser,
        /// <summary>クラブ解散確認</summary>
        ClubDissolutionConfirm,
        /// <summary>クラブお知らせボード</summary>
        ClubInformationBoard,
        /// <summary>クラブお知らせボード編集</summary>
        ClubEditInformationBoard,
        
        /// <summary>メニュー画面</summary>
        Menu,
        /// <summary>引繼ぎ設定画面</summary>
        UserTransferSetting,
        /// <summary>引繼ぎ手順画面</summary>
        TransferFlow,
        /// <summary>文章表示画面</summary>
        Article,
        /// <summary>トレーナーカード</summary>
        TrainerCard,
        /// <summary>自己紹介編集画面</summary>
        IntroductionEdit,
        /// <summary>アイコン変更</summary>
        UserIconChange,
        /// <summary>Title変更</summary>
        UserTitleChange,
        /// <summary>アイテム一覧</summary>
        ItemList,
        /// <summary>ヘルプ画面</summary>
        Help,
        /// <summary>設定画面</summary>
        Configuration,
        /// <summary>クラブ勧誘設定画面</summary>
        ClubSolicitationSettings,
        /// <summary>アイテム詳細確認画面</summary>
        ItemConfirm,
        /// <summary>アイテム詳細内訳画面</summary>
        ItemBreakdownDetail,
        /// <summary>フォロー画面</summary>
        Follow,
        /// <summary>フォロー、ブロック確認画面</summary>
        FollowConfirm,
        /// <summary>フォロー検索画面</summary>
        FollowSearch,
        /// <summary>一括エール画面</summary>
        YellAll,
        /// <summary>エール送信画面</summary>
        YellSend,
        /// <summary>エール履歴画面</summary>
        YellHistory,
        /// <summary>スタミナ不足画面</summary>
        StaminaInsufficient,
        /// <summary>スタミナ不足確認画面</summary>
        StaminaInsufficientConfirm,
        /// <summary>遊び方画面</summary>
        Rules,
        /// <summary>アイコン並び替え/絞り込み画面</summary>
        UserIconSortFilter,
        /// <summary>称号並び替え/絞り込み画面</summary>
        UserTitleSortFilter,
        /// <summary>表示キャラ設定(リスト表示)</summary>
        TrainerCardDisplayCharacterList,
        /// <summary>表示キャラ設定(キャラ選択)</summary>
        TrainerCardDisplayCharacterSelect,
        /// <summary>トレーナーカード着せ替え</summary>
        TrainerCardCustomize,
        /// <summary>表示キャラ設定</summary>
        TrainerCardDisplayCharacterSetting,
        /// <summary>バッチ変更</summary>
        MyBadgeChange,
        /// <summary>バッチ 並び替え/絞り込み</summary>
        MyBadgeSortFilter,
        
        /// <summary>Pvpランキング</summary>
        ColosseumRanking,
        /// <summary>Pvp対戦履歴</summary>
        ColosseumRecord,
        /// <summary>Pvp過去戦績</summary>
        ColosseumHistory,
        /// <summary>Pvpランキング報酬</summary>
        ColosseumRewardConfirm,
        /// <summary>Pvp結果</summary>
        HomeEndOfSeason,

        /// <summary>ガチャ提供割合リスト</summary>
        GachaProbabilityList,
        /// <summary>ガチャ提供割合詳細</summary>
        GachaProbabilityDetail,
        /// <summary>ガチャ実行確認</summary>
        GachaExecuteConfirm,
        /// <summary>ガチャ実行確認(ポイント不足)</summary>
        GachaExecuteConfirmPointShortage,
        /// <summary>ガチャピックアップ</summary>
        GachaPickUp,
        /// <summary>ガチャピックアップ選択</summary>
        GachaPickUpSelect,
        /// <summary>ガチャ自動売却による獲得</summary>
        GachaAutoGot,
        /// <summary>ガチャ引き直し確認</summary>
        GachaRetryConfirm,
        /// <summary>ポイント足りない時のガチャ引き直し</summary>
        GachaRetryPointShortage,
        /// <summary>BoxGacha詳細</summary>
        GachaBoxDetail,
        /// <summary>BoxGachaのリセット確認</summary>
        GachaBoxDetailResetConfirm,
        /// <summary>BoxGachaの目玉商品モーダル</summary>
        GachaBoxHighlightRewardConfirm,
        
        /// <summary>レビュー画面</summary>
        Review,
        
        /// <summary>育成キャラ用絞り込み/ソート画面</summary>
        BaseCharacterSortFilter,
        /// <summary>スペシャルサポートカード用絞り込み/ソート画面</summary>
        SpecialSupportCardSortFilter,
        /// <summary>サクセスキャラ用絞り込み/ソート画面</summary>
        SuccessCharacterSortFilter,
        /// <summary>信頼度報酬リスト</summary>
        TrustRewardList,
        /// <summary>強化確認</summary>
        CharacterGrowthConfirm,
        /// <summary>能力解放確認</summary>
        CharacterLiberationConfirm,
        /// <summary>キャラクタの練習能力詳細</summary>
        CharacterPracticeSkill, 
        /// <summary>自動売却</summary>
        AutoSellConfirm,
        
        /// <summary>ユーザー情報登録</summary>
        TutorialInputUserData,
        /// <summary>初期ユーザーアイコン選択</summary>
        TutorialSelectUserIcon,
        /// <summary>キャライラスト </summary>
        CharacterIllustrator,
        /// <summary>キャラ解放確認</summary>
        PieceToCharaConfirm,
        
        /// <summary>途中データの確認モーダル</summary>
        PendingConfirm,
        
        /// <summary>一枚絵ポップアップ</summary>
        Image,
        
        /// インゲーム
        /// <summary>スキップ確認ポップアップ</summary>
        InGameSkip,
        /// <summary>キャラクターバトルデータ簡易一覧</summary>
        SelectPlayerDisplayStats,
        /// <summary>キャラクターバトルデータ詳細</summary>
        StatDetail,
        /// <summary>ゴールリプレイシーン選択</summary>
        SelectPlaybackGoalScene,

        /// <summary>メンテナンス</summary>
        Maintenance,
        /// <summary>交換所成功確認</summary>
        ShopExchangeSuccessConfirm,
        /// <summary>キャラピース詳細</summary>
        CharacterPieceDetail,
        /// <summary>試合の終了条件</summary>
        ExitConditions,
        /// <summary>戦略選択</summary>
        StrategyChoice,
        /// <summary>プレイヤーアイコン詳細</summary>
        UserIconDetail,
        /// <summary>ショップ商品購入最終確認</summary>
        ShopProductPurchase,
        /// <summary>イベント報酬リスト</summary>
        EventPointRewardList,
        /// <summary>特攻キャラ確認画面ポップアップ</summary>
        EventBoost,
        /// <summary>イベントランキング</summary>
        EventRanking,
        /// <summary>スタンプ詳細</summary>
        StampDetail,
        /// <summary>汎用不足ポイント通知</summary>
        CommonExecuteConfirmPointShortage,
        /// <summary>汎用不足ポイント使用確認</summary>
        CommonExecuteConfirm,
        /// <summary>マッチスキルモーダル</summary>
        CombinationMatch,
        /// <summary>コレクションスキル効果一覧</summary>
        CombinationCollectionPracticeSkillList,
        /// <summary>コレクションスキル解放一覧</summary>
        CombinationCollectionSkillUnlocked,
        /// <summary>コレクションスキル解放可能一覧</summary>
        CombinationCollectionActivatableNotification,
        /// <summary>コネクトマッチスキルソートフィルター</summary>
        CombinationMatchSortFilter,
        /// <summary>コネクトトレーニングスキルソートフィルター</summary>
        CombinationTrainingSortFilter,
        /// <summary>コネクトコレクションスキルソートフィルター</summary>
        CombinationCollectionSortFilter,
        /// <summary>コネクトスキルソートフィルター内の選手のソート・フィルター</summary>
        CharacterIconSortFilter,
        /// <summary>コネクトスキルソートフィルター内のアドバイザーのソート・フィルター</summary>
        AdviserIconSortFilter,
        /// <summary>コネクトスキルソートフィルター内のサポカのソート・フィルター</summary>
        SupportCardSortFilter,
        /// <summary>クラブマッチの個人ランキング</summary>
        ClubMatchPersonalRanking,
        /// <summary>クラブマッチのスコア内訳</summary>
        ClubMatchScoreBreakdown,
        /// <summary>クラブマッチの報酬確認</summary>
        ClubMatchReward,
        /// <summary>クラブマッチの対戦履歴</summary>
        ClubMatchRecord,
        /// <summary>クラブマッチの過去履歴</summary>
        ClubMatchPastRecord,
        /// <summary>クラブマッチ結果</summary>
        HomeEndOfSeasonClubMatch,
        /// <summary>クラブマッチ参加条件</summary>
        ParticipationConditions,
        /// <summary>クラブマッチ用チーム一覧</summary>
        ClubTeamSummary,
        /// <summary>報酬ブースト詳細</summary>
        BoostEffect,
        /// <summary>コレクションスキルセット詳細</summary>
        CombinationCollectionSkillSetDetail,
        /// <summary>トレーニングスキル解放可能一覧</summary>
        CombinationTrainingSkillSetDetail,

        /// <summary>シナリオ特攻</summary>
        ScenarioSpecialAttack,
        /// <summary>サポート器具売却確認</summary>
        SupportEquipmentSellConfirm,
        /// <summary>サポート器具の詳細</summary>
        SupportEquipmentDetail,
        /// <summary>サポート器具売却完了</summary>
        SupportEquipmentSellCompletion,
        /// <summary>サポート器具再抽選</summary>
        SupportEquipmentRedrawing,
        /// <summary>サポート器具再抽選確認</summary>
        SupportEquipmentRedrawingConfirm,
        /// <summary>サポート器具再抽選演出</summary>
        SupportEquipmentRedrawingEffect,
        /// <summary>サポート器具再抽選結果</summary>
        SupportEquipmentRedrawingResult,
        /// <summary> サポート器具抽選テーブル詳細 </summary>
        SupportEquipmentLotteryAbilityDetail,
        /// <summary>サポート器具用絞り込み/ソート画面</summary>
        SupportEquipmentSortFilter,
        /// <summary>サポート器具練習スキル絞り込み画面</summary>
        SupportEquipmentFilterPracticeSkillSelection,
        /// <summary>ユーザの名前変更</summary>
        UserNameChange,
        /// <summary>ユーザの名前変更確認</summary>
        UserNameChangeConfirm,
        /// <summary>サポート器具一括売却用絞り込み画面</summary>
        SupportEquipmentSellFilter,
        
        /// <summary>リーグ戦メニュー</summary>
        LeagueMatchMenu,
        /// <summary>リーグ戦履歴</summary>
        LeagueMatchRecord,
        /// <summary>リーグ戦今季の戦績</summary>
        LeagueMatchResultSeason,
        /// <summary>リーグ戦報酬確認</summary>
        LeagueMatchRewardConfirm,
        /// <summary>リーグ戦登録チーム情報</summary>
        LeagueMatchRegisteredTeamConfirm,
        /// <summary>リーグ戦ランキング</summary>
        LeagueMatchRanking,
        
        /// <summary>ホームのリーグ終了時のモーダル</summary>
        HomeEndOfSeasonLeagueMatch,
        /// <summary>ホームの入れ替え戦終了時のモーダル</summary>
        HomeEndOfPromotionAndRelegation,
        
        /// <summary>ホームの簡易大会終了時のモーダル</summary>
        HomeEndOfInstantTournament,
        
        /// <summary>ランキング対象キャラ表示モーダル</summary>
        RankingTargetTrainingCharacterComfirm,
        
        /// <summary>バッチ詳細</summary>
        MyBadgeDetail,
        
        /// <summary>トレーナーカード全身図詳細</summary>
        TrainerCardDisplayCharacterDetail,
        /// <summary>トレーナーカード着せ替え詳細</summary>
        TrainerCardCustomizeFrameDetail,

        /// <summary>クラブ・ロワイヤルのメニュー</summary>
        ClubRoyalMenu,
        /// <summary>クラブ・ロワイヤルの試合履歴モーダル</summary>
        ClubRoyalRecord,
        /// <summary>クラブ・ロワイヤルの報酬確認モーダル</summary>
        ClubRoyalRewardConfirm,

        /// <summary>クラブロワイヤルインゲーム 拠点モーダル</summary>
        ClubRoyalInGameSpot,
        /// <summary>クラブロワイヤルインゲーム チーム選択モーダル</summary>
        ClubRoyalInGameSelectParty,
        /// <summary>クラブロワイヤルインゲーム チーム解散モーダル</summary>
        ClubRoyalInGameDissolveParty,
        /// <summary>クラブロワイヤルインゲーム アイテム使用モーダル</summary>
        ClubRoyalInGameConfirmUseItem,

        //// <summary> クラブ・ロワイヤルの自動配置設定モーダル </summary>
        ClubRoyalAutoFormation,
        
        /// <summary> クラブ・ロワイヤルの結果表示モーダル </summary>
        HomeEndOfSeasonClubRoyal,
        
        /// <summary> クラブ・ロワイヤル エールスキルの選択モーダル </summary>
        ClubRoyalInGameAdviserYellSkillSelectConfirm,
        
        /// <summary> クラブ・ロワイヤル サポートスキルの確認モーダル </summary>
        ClubRoyalInGameAdviserSupportSkillSelectConfirm,
        
        /// <summary> 編成制限モーダル </summary>
        FormationLimitation,
    }
    
    
    public abstract class ModalManager<T> : CruFramework.Page.ModalManager<T> where T : System.Enum
    {
        private ResourcesLoader resourcesLoader = new ResourcesLoader();
        
        protected async override UniTask<CruFramework.Page.ModalWindow> OnLoadModalResource(T modal, CancellationToken token)
        {
            // モーダルウィンドウ取得
            return await resourcesLoader.LoadAssetAsync<CruFramework.Page.ModalWindow>(GetAddress(modal), token);
        }
        
        /// <summary>モーダルが全て閉じた時の通知</summary>
        protected override void OnCloseLastModalWindow()
        {
            resourcesLoader.Release();
        }
        
        /// <summary>マネージャが破棄された</summary>
        private void OnDestroy()
        {
            resourcesLoader.Release();
        }

        public override async UniTask<CruFramework.Page.ModalWindow> OpenModalAsync(T modal, object args, CancellationToken token, ModalOptions options = ModalOptions.None)
        {         
#if UNITY_EDITOR
            // ツールシーンで使うときエラー出るので
            if(AppManager.Instance == null)
            {
                return await base.OpenModalAsync(modal, args, token, options);
            }
#endif
            
            CruFramework.Page.ModalWindow result = null;
            await AppManager.Instance.LoadingActionAsync(async ()=>
            {
                modal.CrashLogFlow("Modals open.");
                result = await base.OpenModalAsync(modal, args, token, options);
            });             
            return result;
        }

        public override UniTask CloseModalAsync(CruFramework.Page.ModalWindow modalWindow, Action OnCompleted, CancellationToken token)
        {
#if UNITY_EDITOR
            // ツールシーンで使うときエラー出るので
            if(AppManager.Instance == null)
            {
                return base.CloseModalAsync(modalWindow, OnCompleted, token);
            }
#endif
            
            return AppManager.Instance.LoadingActionAsync(()=>
            {
                return base.CloseModalAsync(modalWindow, OnCompleted, token);
            });
        }

        protected abstract string GetAddress(T modal);
    }
    
    public class ModalManager : ModalManager<ModalType>
    {
        protected override string GetAddress(ModalType modal)
        {
            switch (modal)
            {
                // 特殊な配置はcaseで区切る
                default: return ResourcePathManager.GetPath("ModalAddress", modal.ToString());
            }
        }
    }
}
