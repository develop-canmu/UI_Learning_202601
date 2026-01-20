using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Colosseum;
using Pjfb.Community;
using Pjfb.Gacha;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.News;
using Pjfb.Ranking;
using Pjfb.Rivalry;
using Pjfb.Shop;
using Pjfb.Training;
using Pjfb.UserData;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.Utility
{
    public static class ServerActionCommandUtility
    {
        /// <summary>
        /// onClick設計：
        /// 1.「onClick=openNews:1562」：getArticleListで取得した記事情報からArticle.urlでページを開きます
        /// 2.「onClick=link:https://www.rudel.jp」：ブラウザを開くのでurlschemeも対応される
        /// 3.「onClick=openPage:home」：ホーム画面を開く
        /// 4.「onClick=openPage:shop」：ショップページを開く
        /// 5.「onClick=openPage:gacha」：ガチャ画面を開く
        /// 6.「onClick=openPage:follow」：フォロー画面（エールミッションのため）
        /// 7.「onClick=openPage:chara」：強化画面手前の選手選択画面（強化ミッションと能力解放ミッションのため）
        /// 9.「onClick=openPage:rivalry」：ライバルリー一覧ページ
        /// 9.1.「onClick=openPage:rivalry:{m_hunt.id}」：ライバルリーページ（ID指定）
        /// 10.「onClick=openPage:story」：ストーリーTOPページ
        /// 11.「onClick=openPage:training」：トレーニング一覧ページ
        /// 11.1.「onClick=openPage:training:{m_training_scenario.id}」：トレーニングページ（ID指定）
        /// 12.「onClick=openPage:club」：クラブページ
        /// 13.「onClick=openPage:pvp」：PVPページ
        /// 14.「onClick=survey:{googleフォームのurl}?entry.{フレンドコード入力id}={friend_code}」：googleフォームをブラウザで表示する
        /// 14.1. <friend_code>はアプリ側がフレコードで上書きする
        /// 15. 「onClick=openDetail:{m_chara.id}」：m_chara.card_typeによってレベルマックスでキャラ詳細画面を表示する
        /// </summary>
        /// <param name="nullableCommandParam">crz7-imgやcrz7-buttonのonClickパラメーターになります</param>
        public static void ProceedAction(string nullableCommandParam)
        {
            var splitCommand = nullableCommandParam?.Split(':');
            if (splitCommand?.Length < 2)
            {
                Logger.LogWarning($"ServerActionCommandManager.ProceedAction command not valid. nullableCommand:{nullableCommandParam}");
                return;
            }
            Logger.Log($"ServerActionCommandUtility.ProceedAction {nullableCommandParam}");
            
            switch (splitCommand[0])
            {
                case "openNews":
                    NewsManager.TryShowNewsByArticleWithUrl(url: splitCommand[1]);
                    break;
                case "openPage":
                    switch (splitCommand[1])
                    {
                        case "shop": TryOpenShopPage(splitCommand); break;
                        case "gacha": TryOpenGachaPage(splitCommand); break;
                        case "home": TryCloseWindowsAndOpenPage(PageType.Home, null); break;
                        // エールミッション用：フォロー一覧からエールを送るモーダルを開く
                        case "follow": TryOpenFollowYellAllModal().Forget(); break;
                        case "chara": TryCloseWindowsAndOpenPage(PageType.Character, new CharacterPage.Data(CharacterPageType.BaseCharaGrowthLiberationList)); break;
                        case "rivalry": TryOpenRivalryPage(splitCommand); break;
                        case "story": TryCloseWindowsAndOpenPage(PageType.Story, null); break;
                        case "training": TryOpenTrainingPage(splitCommand); break;
                        case "club": TryOpenClubPage(); break;
                        case "pvp": TryOpenColosseumPage(); break;
                        case "ranking": TryOpenRankingPage(splitCommand).Forget(); break;
                        // 他に遷移先を追加したい場合、ここに足す
                        
                        default: Logger.LogError($"ServerActionCommandManager.ProceedAction openPage is not opening page. nullableCommandParam:{nullableCommandParam}"); break;
                    }
                    break;
                case "link":
                    var linkAddress = nullableCommandParam?.Substring(splitCommand[0].Length + 1); // メモ：link:{linkAddress}
                    Application.OpenURL(linkAddress);
                    break;
                case "survey":
                    var googleFormUrl = nullableCommandParam?.Substring(splitCommand[0].Length + 1); // メモ：link:{googleFormUrl}
                    var friendCode = UserDataManager.Instance.user.friendCode;
                    googleFormUrl = googleFormUrl?.Replace("{friend_code}", Uri.EscapeDataString(friendCode));
                    Application.OpenURL(googleFormUrl);
                    break;
                case "openDetail":
                    TryOpenCharaDetail(splitCommand);
                    break;
                case "openTutorial":
                    AppManager.Instance.TutorialCommandManager.ActionTutorial(splitCommand[1]).Forget();
                    break;
            }
        }

        private static void TryOpenGachaPage(string[] splitCommand)
        {
            var gachaId = splitCommand.Length >= 3 && int.TryParse(splitCommand[2], out var parsedId) ? parsedId : 0;
            TryCloseWindowsAndOpenPage(PageType.Gacha, new GachaPageArgs(gachaId));
        }

        private static void TryOpenShopPage(string[] splitCommand)
        {
            var typeValue = splitCommand.Length >= 3 ? splitCommand[2] : string.Empty;
            var idValue = splitCommand.Length >= 4 && int.TryParse(splitCommand[3], out var parsedId) ? parsedId : 0;
            var param = new ShopPageArgs();
            switch (typeValue)
            {
                case "pack":
                    param.mBillingRewardBonusId = idValue;
                    TryCloseWindowsAndOpenPage(PageType.Shop, param);
                break;
                case "exchange":
                    if (idValue != 0)
                    {
                        ShopExchangeModal.Open(idValue);
                    }
                    else
                    {
                        param.selectedTab = ShopTabSheetType.Exchange;
                        TryCloseWindowsAndOpenPage(PageType.Shop, param);
                    }
                    break;
                default:
                    TryCloseWindowsAndOpenPage(PageType.Shop, param);
                break;
            }
        }

        private static void TryOpenRivalryPage(string[] splitCommand)
        {
            // サポート器具上限チェック
            if (SupportEquipmentManager.ShowOverLimitModal()) 
            {
                return;
            }

            var huntId = splitCommand.Length >= 3 && long.TryParse(splitCommand[2], out var parsedId) ? parsedId : 0;
            var huntData = RivalryManager.rivalryCacheData.huntMasterObjectDataList.Find(aData => aData.id.Equals(huntId));
            if (huntData != null) {
                if (RivalryManager.rivalryCacheData.huntTimeTableDictionary.TryGetValue(huntId, out var timeTableDatas)) {
                    foreach (var timeTableData in timeTableDatas) {
                        if (timeTableData.type != 1) continue;
                        if (!RivalryManager.Instance.CanMatchAboveLimit(timeTableData.id))
                        {
                            // 上限超えた試合場合TOPに移動
                            TryCloseWindowsAndOpenPage(PageType.Rivalry, null);
                        }
                        else if (timeTableData.mPointId == 0) {
                            // レギュラー画面へ
                            var pageParams = new RivalryPage.Data(RivalryPageType.RivalryRegular, new RivalryRegularPage.PageParams {
                                huntMasterObject = huntData,
                                huntTimetableMasterObject = timeTableData
                            });
                            TryCloseWindowsAndOpenPage(PageType.Rivalry, pageParams);
                        } else {
                            // イベントマッチ画面へ
                            var pageParams = new RivalryPage.Data(RivalryPageType.RivalryEvent, new RivalryEventPage.PageParams {
                                huntMasterObject = huntData,
                                huntTimetableMasterObject = timeTableData,
                                HuntEnemyMasterObjectList = RivalryManager.rivalryCacheData.huntEnemyObjectList[huntId],
                                overrideClickBackButton = AppManager.Instance.UIManager.PageManager.PrevPage
                            });
                            TryCloseWindowsAndOpenPage(PageType.Rivalry, pageParams);
                        }
                        break;
                    }

                    return;
                } 
                Logger.LogError($"ServerActionCommandManager.TryOpenRivalryPage unable to open rivalry by id. huntId:{huntId}");
            }
            
            // ライバルリーTOP画面へ
            TryCloseWindowsAndOpenPage(PageType.Rivalry, null);
        }

        private static async void TryOpenTrainingPage(string[] splitCommand)
        {
            await TryCloseWindows();
            var trainingScenarioId = splitCommand.Length >= 3 && int.TryParse(splitCommand[2], out var parsedId) ? parsedId : 0;
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.TrainingPreparation, stack: false, new TrainingPreparation.Arguments(trainingScenarioId) );
        }

        private static void TryOpenClubPage()
        {
            if (UserDataManager.Instance.IsUnlockSystem(Pjfb.Club.ClubUtility.clubLockId)) TryCloseWindowsAndOpenPage(PageType.Club, null);
            else {
                var systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(Pjfb.Club.ClubUtility.clubLockId);
                var description = systemLock?.description;
                if(string.IsNullOrEmpty(description)) return;

                var button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], onClick: modal => modal.Close());
                var confirmModalData = new ConfirmModalData(title: StringValueAssetLoader.Instance["special_support.release_condition"], message: description, caution: string.Empty, button);
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, confirmModalData);
            }
        }
        
        private static async void TryOpenColosseumPage()
        {
            await TryCloseWindows();
            ColosseumPage.OpenPage(true,null);
        }

        private static async UniTask TryOpenRankingPage(string[] splitCommand)
        {
            string rankingId = splitCommand.Length >= 3 ? splitCommand[2] : string.Empty;
            RankingPage.RankingParam rankingParam = new RankingPage.RankingParam();
            int id = 0;
            id = int.TryParse(rankingId, out id) ? id : 0;
            rankingParam.FirstId = id;
            await TryCloseWindows();
            OpenPage(PageType.Ranking, rankingParam);
        }
        
        private static void TryOpenCharaDetail(string[] splitCommand)
        {
            if (splitCommand.Length <= 1 || !long.TryParse(splitCommand[1], out var mCharaId)) return;

            var mChara = MasterManager.Instance.charaMaster.values.FirstOrDefault(aData => aData.id == mCharaId);
            if (mChara == null) return;
            
            var detailOrderList = new List<CharacterDetailData>{new(mChara)};
            var swipeAbleParams = new SwipeableParams<CharacterDetailData>(detailOrderList, 0);
            if (mChara.cardType != CardType.None)
            {
                CharacterDetailModalBase.OpenCharacterDetailModal(mChara.cardType, new BaseCharaDetailModalParams(swipeAbleParams, canOpenCharacterEncyclopediaPage: false));
            }
        }
        
        private static async UniTask TryCloseWindowsAndOpenPage(PageType pageType, object args)
        {
            await TryCloseWindows();
            OpenPage(pageType, args);
        }

        private static async UniTask TryCloseWindows(Action onFinish = null)
        {
            var modalManager = AppManager.Instance.UIManager.ModalManager;
            modalManager.RemoveTopModalsIgnoreTop(_ => true);
            var modalWindow = modalManager.GetTopModalWindow();
            if (modalWindow != null) await modalWindow.CloseAsync();
            onFinish?.Invoke();
        }

        /// <summary>
        /// フォロー中のユーザーにエールを送るモーダルを開く。エールミッションで使用される
        /// </summary>
        private static async UniTask TryOpenFollowYellAllModal()
        {
            await CommunityManager.GetTodayYellDetailAPI();
            await TryCloseWindows();
            CommunityManager.OpenYellAllModal();
        }

        private static void OpenPage(PageType pageType, object args)
        {
            var pageManager = AppManager.Instance.UIManager.PageManager;
            
            // withArgumentは何かしら細かい情報を表示させるフラグ
            // withArgument:trueの場合画面に表示されている情報を比べないため、必ずOpenPageを実行する
            var withArgument = args != null;
            if (withArgument || pageManager.CurrentPageType != pageType)
            {
                pageManager.OpenPage(pageType, stack: true, args);
            }
        }
    }
}
