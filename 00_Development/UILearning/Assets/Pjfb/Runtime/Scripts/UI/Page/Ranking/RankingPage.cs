using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using TMPro;

namespace Pjfb.Ranking
{
    public class RankingPage : Page
    {
        public class ClientPreviewData
        {
            private List<long> idList = new List<long>();
            public List<long> IDList => idList;
            private HashSet<long> badgeFlg = new HashSet<long> (); 
            public HashSet<long>  BadgeFlg { get{return badgeFlg;}}
            
        }

        [Serializable]
        public class RankingAffiliateTabSheetData
        {
            [SerializeField] private RankingTabSheetType categoryType;
            public RankingTabSheetType CategoryType => categoryType;
            [SerializeField] private RankingAffiliateTabSheetManager sheet;
            public RankingAffiliateTabSheetManager Sheet => sheet;
        }
        
        public class RankingParam
        {
            private int firstId = 0;

            public int FirstId
            {
                get
                {
                    return firstId;
                }
                set
                {
                    firstId = value;
                }
            }
        }
        
        [SerializeField] private RankingTabSheetManager rankingTabSheetManager;
        [SerializeField] private RankingAffiliateTabSheetData[] affiliateTabSheetDataList;
        [SerializeField] private ScrollBanner scrollBanner;
        [SerializeField] private RankingTopScrollBannerItem oneBannerItem;
        [SerializeField] private RankingRewardView rankingRewardView = null;
        [SerializeField] private RankingTargetCharacterView rankingTargetCharacterView = null;
        // 各タブにつくバッジ
        [SerializeField] private RankingTabSheetManager.RankingTabBadge[] categoryTabBadge;
        // バナーにつくバッジ
        [SerializeField] private UIBadgeNotification[] arrowBadge;
        [SerializeField] private TextMeshProUGUI rankingPeriodText;
        [SerializeField] private RankingOwnAffiliate rankingOwnAffiliate;
        // 各タブのボタン
        [SerializeField] private RankingTabSheetButton[] categoryTabSheetButtons;
        // 矢印のオブジェクト
        [SerializeField] private GameObject buttonArrowScroll;
        // 育成対象のボタン
        [SerializeField] private GameObject buttonTargetCharacter;
        
        //  rankingClientPreviewMaster.IdのリストをDicで持っておく
        private Dictionary<RankingTabSheetType,ClientPreviewData> clientPreviewDatas = new Dictionary<RankingTabSheetType, ClientPreviewData>();
        private Dictionary<RankingTabSheetType,RankingTopScrollBannerItem.RankingBannerData> bannerDatas = new Dictionary<RankingTabSheetType,RankingTopScrollBannerItem.RankingBannerData>();
        
        // 表示中のランキングIDを持っておく
        private long useRankingClientId;
        private int indexNum = 0;
        private RankingAffiliateTabSheetType affiliateTabSheetType = RankingAffiliateTabSheetType.None;
        
        // 現在のカテゴリランキング（総戦力、選手戦力、ポイント）のタイプ
        private RankingTabSheetType categoryTabSheetType = RankingTabSheetType.None;
        
        // 更新前のバナーのインデックス
        private int currentBannerIndexNum = -1;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            foreach (RankingTabSheetType type in Enum.GetValues(typeof(RankingTabSheetType)))
            {
                clientPreviewDatas.Add(type, new ClientPreviewData());
                bannerDatas.Add(type, new RankingTopScrollBannerItem.RankingBannerData());
            }
            
            // ランキングごとにIDをリストに追加
            foreach (RankingClientPreviewMasterObject rankingData in MasterManager.Instance.rankingClientPreviewMaster.values.OrderByDescending(data => data.displayPriority))
            {
                // 定常開催もしくは表示期間内のランキングを各リストに追加
                if (rankingData.holdingType == RankingClientPreviewMasterObject.RankingHoldingType.RegularRanking || AppTime.IsInPeriod(rankingData.displayStartAt, rankingData.displayEndAt))
                {
                    if(rankingData.tableType == RankingClientPreviewMasterObject.TableType.PointRanking)
                    {
                        // イベントポイントランキング
                        clientPreviewDatas[RankingTabSheetType.TotalPoint].IDList.Add(rankingData.id);
                        RankingTopScrollBannerItem.RankingBannerData.Param param = new RankingTopScrollBannerItem.RankingBannerData.Param(rankingData.id, rankingData.imageId);
                        bannerDatas[RankingTabSheetType.TotalPoint].BannerParams.Add(param);
                        if (!LocalSaveManager.saveData.clientPreviewRankingIdConfirmList.Contains(rankingData.id))
                        {
                            clientPreviewDatas[RankingTabSheetType.TotalPoint].BadgeFlg.Add(rankingData.id);
                        }
                        continue;
                    }
                    if (rankingData.tableType == RankingClientPreviewMasterObject.TableType.UserRanking)
                    {
                        // 総戦力ランキングと選手戦力ランキング
                        UserRankingSettingMasterObject userRankingSetting = MasterManager.Instance.userRankingSettingMaster.FindData(rankingData.masterId);
                        switch (userRankingSetting.triggerType)
                        {
                            case RankingClientPreviewMasterObject.TriggerType.TotalPower:
                                clientPreviewDatas[RankingTabSheetType.TotalPower].IDList.Add(rankingData.id);
                                RankingTopScrollBannerItem.RankingBannerData.Param totalPowerParam = new RankingTopScrollBannerItem.RankingBannerData.Param(rankingData.id, rankingData.imageId);
                                bannerDatas[RankingTabSheetType.TotalPower].BannerParams.Add(totalPowerParam);
                                if (!LocalSaveManager.saveData.clientPreviewRankingIdConfirmList.Contains(rankingData.id))
                                {
                                    clientPreviewDatas[RankingTabSheetType.TotalPower].BadgeFlg.Add(rankingData.id);
                                }
                                continue;
                            case RankingClientPreviewMasterObject.TriggerType.CharacterPower:
                                clientPreviewDatas[RankingTabSheetType.CharacterPower].IDList.Add(rankingData.id);
                                RankingTopScrollBannerItem.RankingBannerData.Param characterRankingParam = new RankingTopScrollBannerItem.RankingBannerData.Param(rankingData.id, rankingData.imageId);
                                bannerDatas[RankingTabSheetType.CharacterPower].BannerParams.Add(characterRankingParam);
                                if (!LocalSaveManager.saveData.clientPreviewRankingIdConfirmList.Contains(rankingData.id))
                                {
                                    clientPreviewDatas[RankingTabSheetType.CharacterPower].BadgeFlg.Add(rankingData.id);
                                }
                                continue;
                        }
                    }
                    // todo: 3.12.0現在トレーナーレベルがないため未実装
                }
                else
                {
                    // 表示期間外のランキングは確認済みリストから削除
                    if(!LocalSaveManager.saveData.clientPreviewRankingIdConfirmList.Contains(rankingData.id)) continue; 
                    LocalSaveManager.saveData.clientPreviewRankingIdConfirmList.Remove(rankingData.id);
                }
            }
            LocalSaveManager.Instance.SaveData();
            
            indexNum = 0;
            bool isOpend = false;
            
            // 最初に開くシートを指定されている場合はそのシートを開く
            RankingParam rankingParam = (RankingParam)args ?? new RankingParam();
            if (rankingParam.FirstId != 0)
            {
                // 指定されたIDのランキングを検索して開く
                foreach(KeyValuePair<RankingTabSheetType,ClientPreviewData> value in clientPreviewDatas)
                {
                    for (int i = 0; i < value.Value.IDList.Count; i++)
                    {
                        if (value.Value.IDList[i] == rankingParam.FirstId)
                        {
                            isOpend = true;
                            indexNum = i;
                            rankingTabSheetManager.OpenSheet(value.Key, null);
                            break;
                        }
                    }
                }
            }

            if (!isOpend)
            {
                // カテゴリの最初からデータを確認して、最初にデータが見つかったシートを開く
                foreach (RankingTabSheetType type in Enum.GetValues(typeof(RankingTabSheetType)))
                {
                    if (clientPreviewDatas[type].IDList.Count != 0)
                    {
                        rankingTabSheetManager.OpenSheet(type, null);
                        break;
                    }
                }
            }

            for(int i = 0; i < categoryTabSheetButtons.Length; i++)
            {
                categoryTabSheetButtons[i].ShowFilter(clientPreviewDatas[categoryTabSheetButtons[i].SheetType].IDList.Count == 0);
            }

            // イベント登録
            foreach (RankingTabSheetType type in Enum.GetValues(typeof(RankingTabSheetType)))
            {
                foreach (RankingAffiliateTabSheetData sheetData in affiliateTabSheetDataList)
                {
                    if (sheetData.CategoryType != type) continue;
                    sheetData.Sheet.OnOpenSheet -= OnUpdateAffiliateTab;
                    sheetData.Sheet.OnOpenSheet += OnUpdateAffiliateTab;
                }
            }
            
            UpdateCategoryBadge();
            
            rankingTabSheetManager.OnOpenSheet -= OnCategoryChangeTab;
            rankingTabSheetManager.OnOpenSheet += OnCategoryChangeTab;
            
            // バナーの設定
            InitBannerItem();
            scrollBanner.ScrollGrid.OnChangedPage -= OnScrollBanner;
            scrollBanner.ScrollGrid.OnChangedPage += OnScrollBanner;

            UpdateUseRankingId(indexNum);
            
            RankingCategoryTabSheet sheet = (RankingCategoryTabSheet)rankingTabSheetManager.CurrentSheet;
            sheet.RankingAffiliateTabSheetManager.OpenSheet(RankingAffiliateTabSheetType.Personal , null);
            
            // 遊び方表示
            CheckShowHowToPlay();
            
            // 報酬画面におけるタグ切替のイベントを登録
            rankingRewardView.RankingAffiliateTabSheetManager.OnOpenSheet -= OnUpdateAffiliateTab;
            rankingRewardView.RankingAffiliateTabSheetManager.OnOpenSheet += OnUpdateAffiliateTab;
            
            await base.OnPreOpen(args, token);
        }
        
        // 総戦力、選手戦力、ポイントのタブをクリックした時の処理
        private void OnCategoryChangeTab(RankingTabSheetType type)
        {
            rankingOwnAffiliate.Close();
            rankingRewardView.gameObject.SetActive(false);
            indexNum = 0;
            UpdateUseRankingId(indexNum);
            InitBannerItem();
            OnUpdateAffiliateTab(RankingAffiliateTabSheetType.Personal);
            CheckShowHowToPlay();
            UpdateCategoryBadge();
        }

        // 初回のみ遊び方を表示
        private void CheckShowHowToPlay()
        {
            if (LocalSaveManager.saveData.rankingHelpViewedSet.Contains(rankingTabSheetManager.CurrentSheetType) == false)
            {
                OpenHowToPlayModal();
                LocalSaveManager.saveData.rankingHelpViewedSet.Add(rankingTabSheetManager.CurrentSheetType);
                LocalSaveManager.Instance.SaveData();
            }

        }
        
        public void OnScrollBanner(int index)
        {
            indexNum = index;
            scrollBanner.ScrollGrid.RefreshItemView();
            BannerUpdate();
        }
        
        // バナーの更新処理
        private void BannerUpdate()
        {
            UpdateUseRankingId(indexNum);
            OnUpdateAffiliateTab(affiliateTabSheetType);
        }

        private void InitBannerItem()
        {
            if(bannerDatas[rankingTabSheetManager.CurrentSheetType].BannerParams.Count > 1)
            {
                oneBannerItem.gameObject.SetActive(false);
                scrollBanner.ScrollGrid.gameObject.SetActive(true);
                scrollBanner.SetBannerDatas(bannerDatas[rankingTabSheetManager.CurrentSheetType].BannerParams);
                scrollBanner.SetIndex(indexNum);
                buttonArrowScroll.SetActive(true);
            }
            else
            {
                oneBannerItem.SetBannerData(bannerDatas[rankingTabSheetManager.CurrentSheetType].BannerParams[indexNum]);
                oneBannerItem.gameObject.SetActive(true);
                scrollBanner.ScrollGrid.gameObject.SetActive(false);
                buttonArrowScroll.SetActive(false);
            }
        }
        
        // カテゴリーバッジの更新処理
        private void UpdateCategoryBadge()
        {
            foreach (RankingTabSheetType type in Enum.GetValues(typeof(RankingTabSheetType)))
            {
                foreach (RankingTabSheetManager.RankingTabBadge badgeObject in categoryTabBadge)
                {
                    if(badgeObject.Type != type) continue;
                    bool badgeFlg = clientPreviewDatas[type].BadgeFlg.Count > 0;
                    badgeObject.BadgeObject.SetActive(badgeFlg);
                    if(rankingTabSheetManager.CurrentSheetType == type)
                    {
                        foreach (UIBadgeNotification arrowObject  in arrowBadge)
                        {
                            arrowObject.SetActive(badgeFlg);
                        }
                    }
                }
            }
        }
        
        // ランキングIDを更新
        private void UpdateUseRankingId(int idNum)
        {
            useRankingClientId = clientPreviewDatas[rankingTabSheetManager.CurrentSheetType].IDList[idNum];
            // バッジの更新処理
            if (!LocalSaveManager.saveData.clientPreviewRankingIdConfirmList.Contains(useRankingClientId))
            {
                LocalSaveManager.saveData.clientPreviewRankingIdConfirmList.Add(useRankingClientId);
                LocalSaveManager.Instance.SaveData();
                clientPreviewDatas[rankingTabSheetManager.CurrentSheetType].BadgeFlg.Remove(useRankingClientId);
                UpdateCategoryBadge();
            }
            RankingClientPreviewMasterObject data = MasterManager.Instance.rankingClientPreviewMaster.FindData(useRankingClientId);
            
            // 常設のランキングの場合
            if (data.holdingType == RankingClientPreviewMasterObject.RankingHoldingType.RegularRanking)
            {
                // 開催期間テキストを非表示
                rankingPeriodText.gameObject.SetActive(false);
                
                // 表示期間のRootを非表示にする
                rankingTargetCharacterView.ShowDateTime(false);
                // 育成対象ボタンを選手戦力のみ表示する
                buttonTargetCharacter.SetActive(rankingTabSheetManager.CurrentSheetType == RankingTabSheetType.CharacterPower);
            }
            else
            {
                rankingPeriodText.gameObject.SetActive(true);
                DateTime startAt = data.startAt.TryConvertToDateTime();
                DateTime endAt = data.endAt.TryConvertToDateTime();
                DateTime displayStartAt = data.displayStartAt.TryConvertToDateTime();
                DateTime displayEndAt = data.displayEndAt.TryConvertToDateTime();
                
                string startAtStr = startAt.GetNewsDateTimeString();
                string endAtStr = endAt.GetNewsDateTimeString();
                string displayStartAtStr = displayStartAt.GetNewsDateTimeString();
                string displayEndAtStr = displayEndAt.GetNewsDateTimeString();
                
                rankingPeriodText.text = string.Format(StringValueAssetLoader.Instance["ranking.period"], startAtStr, endAtStr);
               
                // 表示期間のRootを表示する
                rankingTargetCharacterView.ShowDateTime(true);
                // 育成対象ボタンを選手戦力のみ表示する
                buttonTargetCharacter.SetActive(rankingTabSheetManager.CurrentSheetType == RankingTabSheetType.CharacterPower);
                
                // 表示期間のテキストをセット
                rankingTargetCharacterView.SetDate(string.Format(StringValueAssetLoader.Instance["ranking.target_character.period"], displayStartAtStr, displayEndAtStr));
            }
            
        }

        /// <summary>現在のViewが既に更新されているかを判定する</summary>
        private bool IsViewUpdateComplete(RankingAffiliateTabSheetType type)
        {
            // カテゴリタブ、個人・クラブタブ、バナーのインデックスが全て同じ場合は更新済みと判定
            return categoryTabSheetType == rankingTabSheetManager.CurrentSheetType && indexNum == currentBannerIndexNum && affiliateTabSheetType == type;
        }
        
        /// <summary>
        /// 個人、クラブのタブをクリックした時の処理
        /// 報酬画面とランキング画面の同期を取るために、両方のシートを開くようにしている
        /// </summary>
        /// <param name="type">開くシートのタイプ</param>
        private async void OnUpdateAffiliateTab(RankingAffiliateTabSheetType type)
        {
            // 現在の状態で既にUpdateView()が呼ばれている場合は処理を行わない
            if (IsViewUpdateComplete(type))
            {
                return;
            }
            
            // メソッド内部の実行を一度だけ通るようにするために、現在の状態を更新
            // カテゴリタイプを更新
            categoryTabSheetType = rankingTabSheetManager.CurrentSheetType;
            // バナーのインデックスを更新
            currentBannerIndexNum = indexNum;
            // 現在開いている個人・クラブタブを更新
            affiliateTabSheetType = type;

            object rankingData;
            if (type == RankingAffiliateTabSheetType.Personal)
            {
                rankingData = await rankingTabSheetManager.GetUserRankingResponseData(useRankingClientId);
            }
            else
            {
                rankingData = await rankingTabSheetManager.GetGuildRankingResponseData(useRankingClientId);
            }
            
            rankingOwnAffiliate.Close();
            
            // 報酬画面とランキング画面の個人、クラブの同期を取るために、両方開くようにする。既に開いている場合は内部で開かないようにしている
            RankingCategoryTabSheet sheet = (RankingCategoryTabSheet)rankingTabSheetManager.CurrentSheet;
            sheet.RankingAffiliateTabSheetManager.OpenSheet(type , null);
            rankingRewardView.RankingAffiliateTabSheetManager.OpenSheet(type, null);

            // 開いたシートのViewを更新する。シートのアクティブ管理は別で行われている。
            sheet.UpdateView(rankingData);
            sheet.UpdateRewardView(rankingRewardView);
            
            if (type == RankingAffiliateTabSheetType.Personal)
            {
                // 個人の報酬画面で表示するユーザー情報のヘッダーをセットする
                RankingRewardSheetPersonalTab rewardPersonalTab = (RankingRewardSheetPersonalTab)rankingRewardView.RankingAffiliateTabSheetManager.CurrentSheet;
                RankingGetUserCommonRankListAPIResponse personalRankingData = (RankingGetUserCommonRankListAPIResponse)rankingData;
                switch (rankingTabSheetManager.CurrentSheetType)
                {
                    case RankingTabSheetType.TotalPower:
                        rewardPersonalTab.SetTotalPowerUserInfoHeader(personalRankingData.ranking.myRank, new BigValue(personalRankingData.ranking.myValue), personalRankingData.ranking.myDeckCharaList);
                        break;
                    case RankingTabSheetType.CharacterPower:
                        rewardPersonalTab.SetCharacterPowerUserInfoHeader(personalRankingData.ranking.myRank, personalRankingData.ranking.myChara);
                        break;
                    case RankingTabSheetType.TrainerLevel:
                    case RankingTabSheetType.TotalPoint:
                        rewardPersonalTab.ShowUserInfoHeader(false);
                        break;
                }
            }
        }
        
        /// <summary>
        /// 報酬切り替えボタンの処理
        /// </summary>
        public void OnClickSwitchButton()
        {
            if (rankingRewardView.gameObject.activeSelf)
            {
                rankingRewardView.gameObject.SetActive(false); 
                rankingTabSheetManager.CurrentSheet.gameObject.SetActive(true);
            }
            else
            {
                rankingRewardView.gameObject.SetActive(true);
                rankingTabSheetManager.CurrentSheet.gameObject.SetActive(false);
                
                // Unityのレイアウト計算後にScrollを構築する
                RankingCategoryTabSheet sheet = (RankingCategoryTabSheet)rankingTabSheetManager.CurrentSheet;
                sheet.UpdateRewardView(rankingRewardView);
            }
        }
        
        // 遊び方ボタン
        public void OnClickHowToPlayButton()
        {
            OpenHowToPlayModal();
        }
        
        /// <summary>
        /// ランキング対象表示ボタン
        /// </summary>
        public void OnClickRankingTargetButton()
        {
            rankingTargetCharacterView.OpenDetailModal(useRankingClientId);
        }

        // 遊び方モーダルを開く
        private void OpenHowToPlayModal()
        { 
            RankingClientPreviewMasterObject rankingData = MasterManager.Instance.rankingClientPreviewMaster.FindData(useRankingClientId);
            
            HowToPlayModal.OpenHowToPlayModal(rankingData.helpImageIdList, rankingData.helpDescriptionList, StringValueAssetLoader.Instance["ranking.help"], PageResourceLoadUtility.GetRankingHowToPath);
        }

        /// <summary>ランキングページを閉じる処理</summary>
        protected override void OnClosed()
        {
            // Viewの更新回数を制御するために必要なキャッシュを初期化する
            categoryTabSheetType = RankingTabSheetType.None;
            currentBannerIndexNum = -1;
            affiliateTabSheetType = RankingAffiliateTabSheetType.None;
            
            base.OnClosed();
        }
    }
}