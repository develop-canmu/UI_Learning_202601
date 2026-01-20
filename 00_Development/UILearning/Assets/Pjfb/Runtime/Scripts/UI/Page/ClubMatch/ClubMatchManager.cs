using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.ClubMatch
{
    public enum ClubMatchSeasonChangeModalType
    {
        Start,
        End,
    }
    
    [Obsolete("ColosseumStateを使用してください")]
    public enum ClubMatchState
    {
        Unknown,
        EndedSeason,
        NextSeason,
        OnSeason
    }

    public enum ClubDeckCondition
    {
        Extreme = 1,
        Best = 2,
        Good = 3,
        NotBad = 4,
        Awful = 5,
    }

    public static class ClubMatchUtility
    {
        public static void OpenProfile(long userId, ColosseumPlayerType playerType)
        {
            //すでにプロフィールを開いていたら奥のを削除する
            AppManager.Instance.UIManager.ModalManager.RemoveModals(window => window is Pjfb.Menu.TrainerCardModalWindow);
            ColosseumManager.OpenUserProfileModal(userId, playerType);
        }
        public static async UniTask OpenClubInfo( long clubId, long userId, long groupType ){
            if ((ColosseumGroupType) groupType != ColosseumGroupType.Club)
            {
                ConfirmModalWindow.Open(new ConfirmModalData(
                StringValueAssetLoader.Instance["common.error"],
                StringValueAssetLoader.Instance["club.group.error"], string.Empty,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],
                    (window => window.Close()))));
                return;
            }
            //クラブ情報表示
            var request = new GuildGetGuildAPIRequest();
            var post = new GuildGetGuildAPIPost();
            post.gMasterId = clubId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            
            var param = new ClubInfoModal.Param();
            param.clubData = new ClubData(response.guild);
            param.myUserID = userId;
            param.showUserProfileOtherButtons = false;
            param.showHeaderButtons = false;
            param.onFinishedDissolution = null;
            param.onFinishedSecession = null;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubInfo, param);
        }
        
        public static async UniTask<ClubData> GetClubData()
        {
            var request = new GuildGetGuildAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            return new ClubData(response.guild);
        }
        
        public static void OpenSeasonChangeModal(ClubMatchSeasonChangeModalType modalType)
        {
            bool isHome = AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.Home;
            
            ConfirmModalData data = new ConfirmModalData();
            // タイトル
            data.Title = (modalType == ClubMatchSeasonChangeModalType.Start) ? StringValueAssetLoader.Instance["club.season.start_title"] : StringValueAssetLoader.Instance["club.season.end_title"];
            
            if(isHome)
            {
                // メッセージ
                data.Message = (modalType == ClubMatchSeasonChangeModalType.Start) ? 
                    StringValueAssetLoader.Instance["club.season.start_update_data"] : 
                    StringValueAssetLoader.Instance["club.season.end_update_data"];
                  
                // ボタンイベント  
                data.NegativeButtonParams = new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance["common.executeUpdate"],
                    async window =>
                    {
                        // API実行
                        ColosseumGetHomeDataAPIRequest request = new ColosseumGetHomeDataAPIRequest();
                        ColosseumGetHomeDataAPIPost post = new ColosseumGetHomeDataAPIPost();
                        post.getTurn = 0;
                        request.SetPostData(post);
                        await APIManager.Instance.Connect(request);
                        // ウィンドウ閉じる
                        window.Close();
                    }
                );
            }
            else
            {
                // メッセージ
                data.Message = (modalType == ClubMatchSeasonChangeModalType.Start) ? 
                    StringValueAssetLoader.Instance["club.season.start_back_home"] : 
                    StringValueAssetLoader.Instance["club.season.end_back_home"];
                
                // ボタンイベント
                data.NegativeButtonParams = new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance["common.to_home"],
                    window => 
                    {
                        AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
                        window.Close(()=>AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, true, null));
                    }
                );
            }

            // モーダルを開く
            ConfirmModalWindow.Open(data);
        }
        
        public static ClubMatchBanner.Data GetClubMatchBannerData()
        {
            List<ClubMatchBanner.Data> bannerDataList = new List<ClubMatchBanner.Data>();
            // シーズン中の情報取得
            foreach (ColosseumSeasonData seasonData in UserDataManager.Instance.ColosseumSeasonDataList.Values)
            {
                // シーズン中じゃない 
                if(!seasonData.IsOnSeason) continue;
                // マスタがない
                if(seasonData.MColosseumEvent == null) continue;
                // クラブマッチじゃない
                if(seasonData.MColosseumEvent.clientHandlingType != ColosseumClientHandlingType.ClubMatch) continue;
                // データ追加
                ClubMatchBanner.Data bannerData = new ClubMatchBanner.Data();
                bannerData.SeasonData = seasonData;
                bannerDataList.Add(bannerData);
            }

            // 開催中のクラブマッチが存在しない
            if(!bannerDataList.Any())
            {
                DateTime now = AppTime.Now;
                // マスタから
                foreach (ColosseumEventMasterObject mColosseumEvent in MasterManager.Instance.colosseumEventMaster.values)
                {
                    // クラブマッチじゃない
                    if(mColosseumEvent.clientHandlingType != ColosseumClientHandlingType.ClubMatch) continue;
                    // イベントが既に終了している
                    if(mColosseumEvent.endAt.TryConvertToDateTime().IsPast(now)) continue;
                    
                    ClubMatchBanner.Data bannerData = new ClubMatchBanner.Data();
                    // シーズンデータ
                    bannerData.SeasonData = new ColosseumSeasonData();
                    bannerData.SeasonData.MColosseumEventId = mColosseumEvent.id;
                    // 次シーズン
                    DateTime startAt = mColosseumEvent.startAt.TryConvertToDateTime();
                    if(startAt.IsFuture(now))
                    {
                        bannerData.NextSeasonStartAt = startAt + TimeSpan.FromHours(1);
                    }
                    else
                    {
                        TimeSpan diff = now - startAt;
                        // 今までに開催したクラブマッチの回数
                        int count = (int)(diff.TotalDays / mColosseumEvent.cycleDays);
                        // 休止期間に入っていたら回数を1増やす
                        if(diff.TotalDays - mColosseumEvent.cycleDays * count - (mColosseumEvent.cycleDays - mColosseumEvent.intervalMarginDays) >= 0)
                        {
                            count += 1;
                        }
                        // 過去に開催した日数から計算
                        bannerData.NextSeasonStartAt = startAt + TimeSpan.FromDays(mColosseumEvent.cycleDays * count);
                        // PvPの仕様の名残でシーズン開始は1時から始まる
                        bannerData.NextSeasonStartAt += TimeSpan.FromHours(1);
                    }
                    
                    // 開催日数後
                    bannerData.NextSeasonEndAt = bannerData.NextSeasonStartAt + TimeSpan.FromDays(mColosseumEvent.cycleDays - mColosseumEvent.intervalMarginDays);
                    // PvPの仕様の名残でシーズン開始は1時から始まるので引く
                    bannerData.NextSeasonEndAt -= TimeSpan.FromHours(1);
                    // HH:59:59まで
                    bannerData.NextSeasonEndAt -= TimeSpan.FromTicks(1);
                    bannerDataList.Add(bannerData);
                }
            }

            return bannerDataList.FirstOrDefault();
        }

        public static bool IsClubMatchSeason()
        {
            var now = AppTime.Now;
            if (now.Year != isOnSeasonLastUpdateTime.Year ||
                now.Month != isOnSeasonLastUpdateTime.Month ||
                now.Day != isOnSeasonLastUpdateTime.Day ||
                now.Hour != isOnSeasonLastUpdateTime.Hour ||
                now.Minute != isOnSeasonLastUpdateTime.Minute)
            {
                isOnSeason = GetClubMatchBannerData().SeasonData.IsOnSeason;
                isOnSeasonLastUpdateTime = now;
            }

            return isOnSeason;
        }
         
        private static bool isOnSeason;
        private static DateTime isOnSeasonLastUpdateTime = DateTime.MinValue;

        
        private static Dictionary<ClubDeckCondition, ClubConditionData> ClubConditionDataDictionary;
        public static ClubConditionData GetConditionData(ClubDeckCondition targetCondition)
        {
            if(ClubConditionDataDictionary is null)
            {
                ClubConditionDataDictionary = new Dictionary<ClubDeckCondition, ClubConditionData>();
                foreach (var mColosseumBattleCorrection in MasterManager.Instance.colosseumBattleCorrectionMaster.values)
                {
                    ClubDeckCondition condition = (ClubDeckCondition)mColosseumBattleCorrection.labelNumber;
                    if(!ClubConditionDataDictionary.ContainsKey(condition)){
                        
                        ClubConditionDataDictionary.Add(condition, new ClubConditionData(condition, new BigValue(mColosseumBattleCorrection.rate + BigValue.DefaultRateValue)));
                    }
                }

                if (!ClubConditionDataDictionary.ContainsKey(ClubDeckCondition.Awful))
                {
                    ClubConditionDataDictionary.Add(ClubDeckCondition.Awful, new ClubConditionData(ClubDeckCondition.Awful, BigValue.RateValue));
                }
            }

            return ClubConditionDataDictionary[targetCondition];
        }

    }
    
    public class ClubConditionData
    {
        public ClubDeckCondition condition;
        public string animationKey;
        public BigValue combatPowerAmplifier;
        public Color combatPowerTextColor;
        public ClubConditionData(ClubDeckCondition condition, BigValue combatPowerAmplifier)
        {
            this.condition = condition;
            this.combatPowerAmplifier = combatPowerAmplifier;

            switch (condition)
            {
                case ClubDeckCondition.Awful:
                    animationKey = "Condition001";
                    combatPowerTextColor = ColorValueAssetLoader.Instance["808080"];
                    break;
                case ClubDeckCondition.NotBad:
                    animationKey = "Condition002";
                    combatPowerTextColor = ColorValueAssetLoader.Instance["f661b4"];
                    break;
                case ClubDeckCondition.Good:
                    animationKey = "Condition003";
                    combatPowerTextColor = ColorValueAssetLoader.Instance["white"];
                    break;
                case ClubDeckCondition.Best:
                    animationKey = "Condition004";
                    combatPowerTextColor = ColorValueAssetLoader.Instance["8e9ffa"];
                    break;
                case ClubDeckCondition.Extreme:
                    animationKey = "Condition005";
                    combatPowerTextColor = ColorValueAssetLoader.Instance["8e9ffa"];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(condition));
            }
        }

    }
    
    
}