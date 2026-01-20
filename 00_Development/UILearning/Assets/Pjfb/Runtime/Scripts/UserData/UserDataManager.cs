using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Colosseum;
using Pjfb.Networking.App.Request;
using Pjfb.Master;
using Pjfb.Extensions;
using Pjfb.Shop;
using Logger = CruFramework.Logger;

namespace Pjfb.UserData {

    public class UserDataManager : CruFramework.Utils.Singleton<UserDataManager> {

        public UserDataPointContainer point{get; private set;} = new UserDataPointContainer();
        public UserDataCharaPieceContainer charaPiece{get; private set;} = new UserDataCharaPieceContainer();

        public List<long> icon{get; private set;} = new List<long>();
		public List<long> title{get; private set;} = new List<long>();
		public List<long> chatStamp{get; private set;} = new List<long>();
		public List<long> tag{get; private set;} = new List<long>();
		public List<NativeApiTag> tagObj{get; private set;} = new List<NativeApiTag>();
        public List<long> userChatStamps => GetUserChatStamps();
        public long pointPaid = 0;
        
        private List<long> unlockSystem = new List<long>();
        public  IReadOnlyList<long> UnlockSystem{get{return unlockSystem;}}
        public  event Action onUpdateUnlockSystemData = null;

        public UserDataCharaContainer chara{get; private set;} = new UserDataCharaContainer();
        public UserDataCharaVariableContainer charaVariable{get; private set;} = new UserDataCharaVariableContainer();
        public UserDataSupportEquipmentContainer supportEquipment{get; private set;} = new UserDataSupportEquipmentContainer();
        /// <summary> mCharaIdでUserDataCharaを取得するためのコンテナ（サポート器具はmCharaIdの重複が発生するため弾く） </summary>
        public UserDataCharaContainer CharaDataByMCharaIdContainer { get; private set; } = new UserDataCharaContainer();
        
        public UserDataUser user{get; private set;} = new UserDataUser();
        
        public bool hasTrustPrize { get; private set; }
        
        public UserDataChara[] userCharaList =>  chara.data.Values.Where(x => x.CardType == CardType.Character).ToArray();
        public UserDataChara[] userSpecialCardList => chara.data.Values.Where(x => x.CardType == CardType.SpecialSupportCharacter).ToArray();
        public UserDataChara[] userNonExtraCardList => chara.data.Values.Where(x => x.CardType == CardType.SpecialSupportCharacter && x.MChara.isExtraSupport == false).ToArray();
        public UserDataChara[] userExtraCardList => chara.data.Values.Where(x => x.CardType == CardType.SpecialSupportCharacter && x.MChara.isExtraSupport).ToArray();
        public UserDataChara[] userAdviserList => chara.data.Values.Where(x => x.CardType == CardType.Adviser).ToArray();
        
        public Dictionary<long, ColosseumDailyStatus> ColosseumDailyStatus { get; } = new Dictionary<long, ColosseumDailyStatus>();

        // key is season id
        public Dictionary<long, ColosseumSeasonData> ColosseumSeasonDataList { get; } = new Dictionary<long, ColosseumSeasonData>();
        
        private long[] entryColosseumIdList = null;
        /// <summary> エントリー済みのColosseumEventId </summary>
        public long[] EntryColosseumIdList => entryColosseumIdList;
        
        /// <summary>ランク情報</summary>
        public ColosseumGradeData ColosseumGradeData { get; } = new ColosseumGradeData();

        public bool registeredLocalPushNotification { get; private set; } = false;
        public Pjfb.Networking.App.Request.WrapperIntList[] pushSettingList = null; // プッシュ通知設定
        // pushSettingListのインデックス
        public enum PushIndexType
        {
            Id = 0,
            OnOff = 1
        }
        public MissionUserAndGuild[] updateMissionList { get; private set; } = null;
        
        // 代替えアイテムの期限リスト
        public List<NativeApiPointExpiry> pointExpiryList { get; private set; } = new List<NativeApiPointExpiry>();

        public List<long> mProfilePartIdList { get; private set; } = new List<long>();
        
        /// <summary>
        /// サーバーレスポンスからUserDataを更新する
        /// </summary>
        public void UpdateByResponseData( NativeApiItemContainer itemContainer, bool isInitialLoad = false ) {
            if( itemContainer.point != null ) {
                point.Update(itemContainer.point);
            }
            
            if( itemContainer.charaPiece != null ) {
                charaPiece.Update(itemContainer.charaPiece);
            }

            if( itemContainer.icon != null ) {
                UpdateArrayData(icon, itemContainer.icon);
            }

            if( itemContainer.title != null ) {
                UpdateArrayData(title, itemContainer.title);
            }

            if( itemContainer.chatStamp != null ) {
                UpdateArrayData(chatStamp, itemContainer.chatStamp);
            }
            
            if( itemContainer.tag != null ) {
                UpdateArrayData(tag, itemContainer.tag);
            }

            if( itemContainer.tagObj != null ) {
                UpdateArrayData(tagObj, itemContainer.tagObj);
            }

            if( itemContainer.tagLost != null ) {
                foreach( var lostTag in itemContainer.tagLost ){
                    tag.RemoveAll(i => i == lostTag);
                    tagObj.RemoveAll(i => i.adminTagId == lostTag);
                }
            }

            if( itemContainer.chara != null ) {
                chara.Update(itemContainer.chara);
                CharaDataByMCharaIdContainer.UpdateByMCharaId(itemContainer.chara);
            }

            if( itemContainer.charaVariable != null ) {
                charaVariable.Update(itemContainer.charaVariable);
            }
            
            if( itemContainer.charaVariableLost != null ) {
                charaVariable.Remove(itemContainer.charaVariableLost);
            }
            
            if( itemContainer.charaVariableTrainer != null ) {
                supportEquipment.Update(itemContainer.charaVariableTrainer);
            }

            if( itemContainer.charaVariableTrainerLost != null ) {
                supportEquipment.Remove(itemContainer.charaVariableTrainerLost);
                // 確認済みのデータから該当のデータを削除する
                SupportEquipmentManager.RemoveViewedUserSupportEquipmentId(itemContainer.charaVariableTrainerLost);
            }

            if (itemContainer.stamina != null) {
                StaminaUtility.UpdateStamina(itemContainer.stamina);
            }
            
            if(itemContainer.unlockedSystemNumber != null){ 
                UpdateArrayData(unlockSystem, itemContainer.unlockedSystemNumber);
                onUpdateUnlockSystemData?.Invoke();
            }
            
            if (itemContainer.pointPaid >= 0)
            {
                pointPaid = itemContainer.pointPaid;
            }

            // シークレットセール削除（購入済みなど）
            if (itemContainer.saleIntroductionLostList != null)
            {
                foreach (long id in itemContainer.saleIntroductionLostList)
                {
                    ShopManager.RemoveSaleIntroduction(id);
                }

#if !PJFB_REL && CRUFRAMEWORK_DEBUG
                // 開発環境のログ表示
                if (itemContainer.saleIntroductionLostList.Length > 0)
                {
                    Logger.Log($"シクレ失効：\n[{string.Join(", ", itemContainer.saleIntroductionLostList)}]");
                }
#endif
            }
            
            // シークレットセール更新
            if (ShopManager.HasSaleIntroduction(itemContainer.saleIntroductionList))
            {
#if !PJFB_REL && CRUFRAMEWORK_DEBUG
                // 開発環境のログ表示
                if (isInitialLoad == false && itemContainer.saleIntroductionList.Length > 0)
                {
                    Logger.Log("シクレ新規発生：\n" +
                               $"[{string.Join(", ", itemContainer.saleIntroductionList.Select(x => $"{x.mSaleIntroductionId}:{x.expireAt}:IsRemind={x.isRemind}"))}]\n" +
                               "発生中\n" +
                               $"[{string.Join(", ", ShopManager.SaleIntroductionList.Select(x => $"{x.mSaleIntroductionId}:{x.expireAt}:IsRemind={x.isRemind}"))}]"
                    );
                }
#endif
                
                ShopManager.UpdateSaleIntroduction(itemContainer.saleIntroductionList);
                AppManager.Instance.UIManager.Footer.ShopButton.SetNotificationBadge(true);

                // user/getData 以外は差分更新なので表示リストに登録する
                if (isInitialLoad == false)
                {
                    ShopManager.UpdateShowSaleIntroduction(itemContainer.saleIntroductionList);
                }
            }
            
            // トレーニングミッション更新リスト
            if(itemContainer.updatedMissionList != null) {
                updateMissionList = itemContainer.updatedMissionList;
            }

            if (itemContainer.pointExpiryList != null)
            {
                foreach (NativeApiPointExpiry containerPointExpiry in itemContainer.pointExpiryList)
                {
                    // 同じポイントID、有効期限のデータがあれば上書き
                    foreach (NativeApiPointExpiry userPointExpiry in pointExpiryList)
                    {
                        if(containerPointExpiry.mPointId != userPointExpiry.mPointId) continue;
                        if(containerPointExpiry.expireAt.TryConvertToDateTime() == userPointExpiry.expireAt.TryConvertToDateTime())
                        {
                            pointExpiryList.Remove(userPointExpiry);
                            break;
                        }
                    }
                    pointExpiryList.Add(containerPointExpiry);
                }
            }
            
            // プロフィール用データIDリスト
            if (itemContainer.mProfilePartIdList != null)
            {
                UpdateArrayData(mProfilePartIdList, itemContainer.mProfilePartIdList);
            }
        }

        public UserDataChara[] GetUserDataCharaListByType(CardType type)
        {
            return type switch
            {
                CardType.Character => userCharaList,
                CardType.SpecialSupportCharacter => userSpecialCardList,
                CardType.Adviser => userAdviserList,
                CardType.None or _ => throw new NotImplementedException(),
            };
        }
        
        /// <summary>アイテムの所持数を返す</summary>
        public long GetPointValue(long mPointId)
        {
            if(point.data.TryGetValue(mPointId, out UserDataPoint result))
            {
                return result.value;
            }
            
            return 0;
        }
        
        public UserDataChara[] GetUserDataExtraCharaList()
        {
            return userExtraCardList;
        }
        
        public UserDataChara[] GetUserDataNonExtraCharaList()
        {
            return userNonExtraCardList;
        }
        
        public bool IsUnlockSystem(long systemId)
        {
            return unlockSystem.Contains(systemId);
        }

        public void UpdateFinishTutorialNumber(long[] finishTutorialNumber)
        {
            user?.UpdateFinishedTutorialStep(finishTutorialNumber);
        }

        public void UpdateHasTrustPrize(bool hasExist)
        {
            hasTrustPrize = hasExist;
        }

        public bool IsExpiredPass(long mStaminaId)
        {
            bool isExpired = false;
            foreach (var tag in tagObj)
            {
                var staminaAdditionMasterObject = MasterManager.Instance.staminaAdditionMaster.FindDataByTagAndStaminaId(tag.adminTagId, mStaminaId);
                if (staminaAdditionMasterObject == null) continue;
                isExpired = tag.expireAt.TryConvertToDateTime().IsPast(AppTime.Now);
            }
            // 有効期限が切れたパスを削除
            tagObj.RemoveAll(tag => tag.expireAt.TryConvertToDateTime().IsPast(AppTime.Now));
            return isExpired;
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            point?.Dispose();
            user?.Dispose();
        }

        public List<long> GetUserIcons()
        {
            var list = new List<long>();
            list.AddRange(MasterManager.Instance.iconMaster.values.Where(iconData => iconData.isPrimary).Select(data => data.id).OrderBy(i=>i));
            list.AddRange(icon.OrderBy(i => i));
            return list;
        }
        
        private List<long> GetUserChatStamps()
        {
            var list = new List<long>();
            list.AddRange(MasterManager.Instance.chatStampMaster.values.Where(iconData => iconData.isFreeFlg == 1).Select(data => data.id).OrderBy(i=>i));
            list.AddRange(chatStamp.OrderBy(i => i));
            return list;
        }

        void UpdateArrayData<T>( List<T> data, T[] updateDataArray ){
            foreach(T updateData in updateDataArray ) {
                if( !data.Contains(updateData) ) {
                    data.Add(updateData);
                }
            }
        }
        
        public void UpdateColosseumDailyStatus(long sColosseumEventId, ColosseumDailyStatus dailyStatus)
        {
            ColosseumDailyStatus[sColosseumEventId] = dailyStatus;
        }

        public ColosseumDailyStatus GetColosseumDailyStatus(long sColosseumEventId)
        {
            if (ColosseumDailyStatus == null || !ColosseumDailyStatus.ContainsKey(sColosseumEventId))
            {
                return null;
            }
            return ColosseumDailyStatus[sColosseumEventId];
        }

        public ColosseumSeasonHome GetAvailableSeasonHome(long mColosseumEventId)
        {
            return ColosseumSeasonDataList.Values.FirstOrDefault(data => data.MColosseumEvent?.id == mColosseumEventId)?.SeasonHome;
        }

        public ColosseumSeasonData GetColosseumSeasonData(long sColosseumEventId)
        {
            if (ColosseumSeasonDataList == null || !ColosseumSeasonDataList.ContainsKey(sColosseumEventId))
            {
                return null;
            }
            return ColosseumSeasonDataList[sColosseumEventId];
        }

        public List<PointGetBonusMasterObject> GetActivePointGetBonus()
        {
            List<PointGetBonusMasterObject> retList = new List<PointGetBonusMasterObject>();
            foreach (var tag in tagObj)
            {
                if (tag.expireAt.TryConvertToDateTime().IsPast(AppTime.Now)) continue;
                var master = MasterManager.Instance.pointGetBonusMaster.GetObjectByAdminTagId(tag.adminTagId);
                if (master == null) continue;
                retList.Add(master);
            }
            return retList;
        }
        
        public void UpdateColosseumSeasonDataList(ColosseumHomeData colosseumHomeData)
        {
            // キャッシュクリア
            ColosseumSeasonDataList.Clear();

            // シーズン情報
            foreach (ColosseumSeasonHome seasonHome in colosseumHomeData.seasonList ?? new ColosseumSeasonHome[0])
            {
                // シーズン情報
                if(!ColosseumSeasonDataList.ContainsKey(seasonHome.id))
                {
                    ColosseumSeasonDataList.Add(seasonHome.id, new ColosseumSeasonData());
                    ColosseumSeasonDataList[seasonHome.id].SeasonId = seasonHome.id;
                    ColosseumSeasonDataList[seasonHome.id].MColosseumEventId = seasonHome.mColosseumEventId;
                }
                ColosseumSeasonDataList[seasonHome.id].SeasonHome = seasonHome;
            }
            // ユーザー過去戦績
            foreach (ColosseumUserSeasonStatus unreadFinished in colosseumHomeData.unreadList ?? new ColosseumUserSeasonStatus[0])
            {
                // シーズン情報
                if(!ColosseumSeasonDataList.ContainsKey(unreadFinished.sColosseumEventId))
                {
                    ColosseumSeasonDataList.Add(unreadFinished.sColosseumEventId, new ColosseumSeasonData());
                    ColosseumSeasonDataList[unreadFinished.sColosseumEventId].SeasonId = unreadFinished.sColosseumEventId;
                    ColosseumSeasonDataList[unreadFinished.sColosseumEventId].MColosseumEventId = unreadFinished.mColosseumEventId;
                }
                ColosseumSeasonDataList[unreadFinished.sColosseumEventId].UnreadUserSeasonStatus = unreadFinished;
            }
            // ユーザー戦績
            foreach (ColosseumUserSeasonStatus userSeasonStatus in colosseumHomeData.userSeasonStatusList ?? new ColosseumUserSeasonStatus[0])
            {
                // シーズン情報
                if(!ColosseumSeasonDataList.ContainsKey(userSeasonStatus.sColosseumEventId))
                {
                    ColosseumSeasonDataList.Add(userSeasonStatus.sColosseumEventId, new ColosseumSeasonData());
                    ColosseumSeasonDataList[userSeasonStatus.sColosseumEventId].SeasonId = userSeasonStatus.sColosseumEventId;
                    ColosseumSeasonDataList[userSeasonStatus.sColosseumEventId].MColosseumEventId = userSeasonStatus.mColosseumEventId;
                }
                ColosseumSeasonDataList[userSeasonStatus.sColosseumEventId].UserSeasonStatus = userSeasonStatus;
            }
            
            // クリア
            ColosseumGradeData.Clear();
            // グレード
            foreach (ColosseumGroupGrade groupGrade in colosseumHomeData.groupGradeList)
            {
                ColosseumGradeData.AddDeta(groupGrade.mColosseumGradeGroupId, groupGrade.gMasterId, groupGrade);
            }

            // エントリー済みのColosseumEventIdをセット
            UpdateColosseumEntryIdList(colosseumHomeData.entryMColosseumEventIdList);
        }
        
        public void UpdateColosseumEntryIdList(long[] entryIdList)
        {
            this.entryColosseumIdList = entryIdList;
        }

        public void UpdateColosseumSeasonStatus(ColosseumUserSeasonStatus colosseumUserSeasonStatus)
        {
            var sColosseumEventId = colosseumUserSeasonStatus?.sColosseumEventId ?? 0;
            
            // ランクマッチが集計完了してアプリ再起動せずに次シーズンに参加する場合にjoinが実行されるよう
            // 意図的にseasonStatusのキャッシュをnull更新する場合があるためsColosseumEventIdも別で渡す
            if(!ColosseumSeasonDataList.ContainsKey(sColosseumEventId))
            {
                ColosseumSeasonDataList.Add(sColosseumEventId, new ColosseumSeasonData());
                ColosseumSeasonDataList[sColosseumEventId].SeasonId = sColosseumEventId;
                
            }

            if (ColosseumSeasonDataList[sColosseumEventId].MColosseumEventId == 0 && colosseumUserSeasonStatus != null)
            {
                ColosseumSeasonDataList[sColosseumEventId].MColosseumEventId = colosseumUserSeasonStatus.mColosseumEventId;
            }
            
            ColosseumSeasonDataList[sColosseumEventId].UserSeasonStatus = colosseumUserSeasonStatus;
        }

        public void ResetColosseumUserSeasonStatus(long mColosseumEventId)
        {
            var seasonData = ColosseumSeasonDataList.Values.FirstOrDefault(data => data.MColosseumEventId == mColosseumEventId);
            if (seasonData == null)
            {
                return;
            }
            seasonData.UserSeasonStatus = null;
        }

        public void UpdatePushSettingList(Pjfb.Networking.App.Request.WrapperIntList[] setList)
        {
            pushSettingList = setList;
        }

        public void UpdateRegisteredLocalPushNotification(bool registered)
        {
            registeredLocalPushNotification = registered;
        }
        
        // 有効期限内の代替えアイテムの所持数を返す
        public long GetExpiryPointValue(long mPointId)
        {
            long pointValue = 0;
            foreach (NativeApiPointExpiry pointExpiry in pointExpiryList)
            {
                if(pointExpiry.mPointId != mPointId) continue;
                if(AppTime.IsInPeriodEndAt(pointExpiry.expireAt) == false) continue;
                pointValue += pointExpiry.value;
            }
            return pointValue;
        }

        /// <summary>
        /// 社内ユーザーであるかどうかの取得
        /// </summary>
        public bool IsCompanyUser()
        {
            //ユーザ種別（0: 一般, 1: 社内, 2: 社内ヘビー）
            return user.userType == 1 || user.userType == 2;
        }
    }
}