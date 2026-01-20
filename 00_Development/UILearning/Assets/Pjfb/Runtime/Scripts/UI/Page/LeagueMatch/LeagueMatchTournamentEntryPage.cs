using System;
using System.Collections.Generic;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Extensions;
using Pjfb.LeagueMatch;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.LeagueMatchTournament
{
    public class LeagueMatchTournamentEntryPage : Page
    {
        public class TournamentEntryPageData
        {
            // トーナメント情報
            private LeagueMatchTournamentInfo tournamentInfo = null;
            public LeagueMatchTournamentInfo TournamentInfo => tournamentInfo;
            
            public TournamentEntryPageData(LeagueMatchTournamentInfo tournamentInfo)
            {
                this.tournamentInfo = tournamentInfo;
            }
        }
        
        // トーナメント名
        [SerializeField]
        private TextMeshProUGUI tournamentNameText = null;
        // 開催期間
        [SerializeField]
        private TextMeshProUGUI tournamentDate = null;
        // エントリー状況のテキスト
        [SerializeField]
        private TextMeshProUGUI tournamentEntryStatusText = null;
        // エントリー条件表示のベースオブジェクト
        [SerializeField]
        private EntryConditionsItem entryConditionsItemBase = null;
        [SerializeField]
        private TextMeshProUGUI noConditionsText = null;
        // エントリー期間のテキスト
        [SerializeField]
        private TextMeshProUGUI entryPeriodText = null;
        
        // エントリーボタン
        [SerializeField]
        private UIButton entryButton = null;
        // エントリーボタンのテキスト
        [SerializeField]
        private TextMeshProUGUI entryButtonText = null;
        
        [SerializeField]
        private ScrollGrid entryConditionScrollGrid = null;
        
        // メニューバッジ
        [SerializeField]
        private UIBadgeNotification menuBadge = null;
        
        // 大会のデータ
        private TournamentEntryPageData data = null;
        
        // エントリー条件オブジェクトのリスト
        
        // エントリー可能かどうか
        private bool isCanEntry = false;
        
        // 自身のクラブ情報
        private ClubData clubData = null;
        
        // 更新用のタイマー
        private float updateTimer = 0;
        
        // 参加クラブ数の上限なし(サーバー定義)
        private const int UnlimitedParticipant = -1;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 最新のクラブ情報を取得する
            clubData = new ClubData(await GetGuildGuildAPIAsync());
            
            data = (TournamentEntryPageData)args;
            UpdateView();
            
            // 条件オブジェクトのベースを非表示にする
            entryConditionsItemBase.gameObject.SetActive(false);
            
            await base.OnPreOpen(args, token);
        }

        // ビューの更新
        private void UpdateView()
        {
            // エントリーできるかの初期化
            isCanEntry = true;
            
            // 大会名
            tournamentNameText.text = data.TournamentInfo.MColosseumEvent.name;
            // 開催期間
            DateTime startAt = data.TournamentInfo.SeasonStartAt;
            DateTime endAt = data.TournamentInfo.SeasonEndAt;
            tournamentDate.text = string.Format(StringValueAssetLoader.Instance["tournament.entry_date"], startAt.GetDateTimeString(), endAt.GetDateTimeString());
            
            List<EntryConditionsItem.EntryConditionItemData> entryConditionItemDataList = new List<EntryConditionsItem.EntryConditionItemData>();

            // 参加人数制限がある場合はエントリー条件欄の先頭に表示する(サーバー側で-1の場合は無制限判定)
            if (data.TournamentInfo.MColosseumEvent.participantLimit != UnlimitedParticipant)
            {
                string conditionText = StringValueAssetLoader.Instance["tournament.entry_condition.participant_limit"];
                string limitText = data.TournamentInfo.MColosseumEvent.participantLimit.ToString();
                EntryConditionsItem.EntryConditionItemData entryConditionItemData = new EntryConditionsItem.EntryConditionItemData(conditionText, limitText, true);
                entryConditionItemDataList.Add(entryConditionItemData);
            }
            
            // 条件がない場合はそのままエントリー可にする
            if (!string.IsNullOrEmpty(data.TournamentInfo.MColosseumEvent.entryConditionJson))
            {
                // Jsonで送られてくるエントリー条件をパース
                EntryCondition[] conditionArray = JsonHelper.FromJson<EntryCondition>(data.TournamentInfo.MColosseumEvent.entryConditionJson);
                
                // 省略表記用のデータを取得
                StringUtility.OmissionTextData entryConditionOmissionTextData = entryConditionsItemBase.EntryConditionOmissionTextSetter.GetOmissionData();
                StringUtility.OmissionTextData clubOmissionTextData = entryConditionsItemBase.ClubStatusOmissionTextSetter.GetOmissionData();
                // 設定されているエントリー条件をループで回してエントリー可能かの判定+達成状況を表示する
                for (int i = 0; i < conditionArray.Length; i++)
                {
                    string conditionExplanation = string.Empty;
                    string clubStatusText = string.Empty;
                    BigValue clubValue;

                    BigValue conditionValue = conditionArray[i].Value;

                    switch (conditionArray[i].ConditionType)
                    {
                        // クラブランク
                        case TournamentConditionType.GuildRank:
                            clubValue = new BigValue(clubData.rankId);
                            conditionExplanation = string.Format(StringValueAssetLoader.Instance["tournament.entry_condition.guild_rank"], MasterManager.Instance.guildRankMaster.FindData((long)conditionValue.Value).name);
                            clubStatusText = MasterManager.Instance.guildRankMaster.FindData(clubData.rankId).name;
                            break;
                        // シーズンランク
                        case TournamentConditionType.SeasonRank:

                            long gradeGroupId = MasterManager.Instance.colosseumEventMaster.FindData(conditionArray[i].TargetId).mColosseumGradeGroupId;

                            // クラブのシーズンランクを取得
                            long clubGradeNumber = UserDataManager.Instance.ColosseumGradeData.GetGradeNumber(gradeGroupId, UserDataManager.Instance.user.gMasterId);
                            clubValue = new BigValue(clubGradeNumber);

                            // 条件のシーズンランクを取得
                            ColosseumGradeMasterObject gradeMasterObject = MasterManager.Instance.colosseumGradeMaster.GetJoinedColosseumGradeMasterObject(gradeGroupId, (long)conditionValue.Value);
                            conditionExplanation = string.Format(StringValueAssetLoader.Instance["tournament.entry_condition.season_rank"], gradeMasterObject.name);
                            clubStatusText = MasterManager.Instance.colosseumGradeMaster.GetJoinedColosseumGradeMasterObject(gradeGroupId, clubGradeNumber).name;
                            break;
                        // メンバー数
                        case TournamentConditionType.MemberCount:
                            clubValue = new BigValue(clubData.memberList.Count);
                            conditionExplanation = string.Format(StringValueAssetLoader.Instance["tournament.entry_condition.member_count"], conditionValue);
                            clubStatusText = clubValue.ToString();
                            break;
                        // クラブ総戦力
                        case TournamentConditionType.GuildCombatPower:
                            clubValue = clubData.power;
                            conditionExplanation = string.Format(StringValueAssetLoader.Instance["tournament.entry_condition.guild_combat_power"], conditionValue.ToDisplayString(entryConditionOmissionTextData));
                            clubStatusText = clubValue.ToDisplayString(clubOmissionTextData);
                            break;
                    }

                    // エントリー可能か判定
                    bool isEntryCondition = clubValue >= conditionValue;
                    if (isEntryCondition == false)
                    {
                        isCanEntry = false;
                    }

                    // エントリー条件のオブジェクト
                    EntryConditionsItem.EntryConditionItemData entryConditionItemData = new EntryConditionsItem.EntryConditionItemData(conditionExplanation, clubStatusText, isEntryCondition);
                    entryConditionItemDataList.Add(entryConditionItemData);
                }
            }
            
            // エントリー条件表示領域の更新
            entryConditionScrollGrid.SetItems(entryConditionItemDataList);
            noConditionsText.gameObject.SetActive(entryConditionItemDataList.Count == 0);
            
            // キャプテンorサブキャプテンのみButtonを表示する
            ClubAccessLevel accessLevel = ClubUtility.CreateAccessLevel( UserDataManager.Instance.user.uMasterId, clubData);
            bool isHasGuildRole = accessLevel == ClubAccessLevel.Master || accessLevel == ClubAccessLevel.SubMaster;
            entryButton.gameObject.SetActive(isHasGuildRole);
            // エントリー状況で表示を変更する
            SetEntryObjects();
            // エントリー受付終了までのテキスト更新
            UpdatePeriodText();
            
            // メニューバッジの更新
            LeagueMatchUtility.UpdateMenuNotification(menuBadge);
        }

        private void Update()
        {
            // データが無いなら更新しない
            if (data == null || clubData == null)
            {
                return;
            }
            
            updateTimer += Time.deltaTime;
            if (updateTimer >= LeagueMatchTournamentManager.UpdateTimeInterval)
            {
                UpdatePeriodText();
                updateTimer = 0;
            }
        }

        private void UpdatePeriodText()
        {
            if (data.TournamentInfo.IsEntryTerm)
            {
                // 残り時間表示
                string remainingTimeString = data.TournamentInfo.EntryEndAt.GetPreciseRemainingString(AppTime.Now);
                entryPeriodText.text = string.Format(StringValueAssetLoader.Instance["tournament.entry_period"], remainingTimeString);
            }
            else
            {
                // エントリー期間終了
                entryPeriodText.text = StringValueAssetLoader.Instance["tournament.entry_period_end"];
            }
        }

        // 自分のクラブ情報を取得
        private async UniTask<GuildGuildStatus> GetGuildGuildAPIAsync()
        {
            GuildGetGuildAPIRequest request = new GuildGetGuildAPIRequest();
            GuildGetGuildAPIPost post = new GuildGetGuildAPIPost();
            // 自分のクラブ情報は0を指定
            post.gMasterId = 0;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData().guild;
        }
        
        // エントリー状況で変動するオブジェクト表示周り
        private void SetEntryObjects()
        {
            if (data.TournamentInfo.IsEntryTerm == false)
            {
                // エントリー受付期間終了
                tournamentEntryStatusText.text = StringValueAssetLoader.Instance["tournament.entry_period_end"];
                // エントリーボタンを非表示に
                entryButton.gameObject.SetActive(false);
            }
            else if (data.TournamentInfo.IsEntry)
            {
                // エントリー中
                tournamentEntryStatusText.text = StringValueAssetLoader.Instance["tournament.entry_status.to_entry"];
                entryButtonText.text = StringValueAssetLoader.Instance["tournament.entry_button.to_entry"];
            }
            else
            {
                // エントリーしていない
                tournamentEntryStatusText.text = StringValueAssetLoader.Instance["tournament.entry_status.not_entry"];
                entryButtonText.text = StringValueAssetLoader.Instance["tournament.entry_button.not_entry"];
            }
        }
        
        
        // エントリー
        private async UniTask EntryTournamentAPIAsync()
        {
            // エントリーAPIを叩く
            ColosseumRegisterEntryAPIRequest request = new ColosseumRegisterEntryAPIRequest();
            ColosseumRegisterEntryAPIPost post = new ColosseumRegisterEntryAPIPost();
            post.mColosseumEventId = data.TournamentInfo.MColosseumEvent.id;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            // エントリー状況を更新
            UserDataManager.Instance.UpdateColosseumEntryIdList(request.GetResponseData().entryMColosseumEventIdList);
        }

        
        // エントリー解除
        private async UniTask CancelEntryAPIAsync()
        {
            // エントリー解除APIを叩く
            ColosseumCancelEntryAPIRequest request = new ColosseumCancelEntryAPIRequest();
            ColosseumCancelEntryAPIPost post = new ColosseumCancelEntryAPIPost();
            post.mColosseumEventId = data.TournamentInfo.MColosseumEvent.id;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            // エントリー状況を更新
            UserDataManager.Instance.UpdateColosseumEntryIdList(request.GetResponseData().entryMColosseumEventIdList);
        }

        /// <summary>エントリーボタンクリック時処理</summary>
        public void OnClickEntryButton()
        {
            ClickEntryButtonAsync().Forget();
        }

        /// <summary>エントリーボタン処理</summary>
        private async UniTask ClickEntryButtonAsync()
        {
            if (data.TournamentInfo.IsEntry)
            {
                // エントリー解除
                await CancelEntryAsync();
            }
            else
            {
                if (isCanEntry)
                {
                    // エントリー
                    await EntryTournamentAPIAsync();
                }
                else
                {
                    // 条件を満たしていなければエントリーさせずにモーダルを表示
                    ParticipationConditionsModal.Params param = new ParticipationConditionsModal.Params();
                    param.title = StringValueAssetLoader.Instance["instant_tournament.participation_conditions_modal.title"];
                    param.message = StringValueAssetLoader.Instance["instant_tournament.participation_conditions_modal.message"];
                    param.colosseumEventMaster = data.TournamentInfo.MColosseumEvent;
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ParticipationConditions, param);
                }
            }
                        
            // エントリー状況関連のテキストを変更する
            SetEntryObjects();
        }

        // エントリーのキャンセル処理
        private async UniTask CancelEntryAsync()
        {
            // エントリーキャンセルの確認モーダルを表示
            ConfirmModalData modalData = new ConfirmModalData();
            modalData.Title = StringValueAssetLoader.Instance["league.match.tournament.entry_cancel_confirm.title"];
            modalData.Message = StringValueAssetLoader.Instance["league.match.tournament.entry_cancel_confirm"];
            
            // キャンセル実行ボタン
            modalData.PositiveButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.ok"], 
                async (window) => 
                {
                    // エントリー解除APIを叩く
                    await CancelEntryAPIAsync();
                    window.Close(); 
                });
            
            // 閉じるボタン
            modalData.NegativeButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.cancel"],
                window => window.Close()
            );
            
            ModalWindow modalWindow = (ModalWindow)await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, modalData);
            await modalWindow.WaitCloseAsync();
        }
        
        
        public void OnClickMenuButton()
        {
            LeagueMatchManager.OnClickLeagueMatchMenu(data.TournamentInfo);
        }
    }
}