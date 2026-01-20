using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.LeagueMatch;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.ClubRoyal
{
    //// <summary> クラブロワイヤル自動配置モーダル </summary>
    public class ClubRoyalAutoFormationModalWindow : ModalWindow
    {
        public class Param
        {
            private LeagueMatchInfo matchInfo = null;
            public LeagueMatchInfo MatchInfo => matchInfo;

            private GroupLeagueMatchBoardInfo boardInfo = null;
            public GroupLeagueMatchBoardInfo BoardInfo => boardInfo;

            public Param(LeagueMatchInfo matchInfo, GroupLeagueMatchBoardInfo boardInfo)
            {
                this.matchInfo = matchInfo;
                this.boardInfo = boardInfo;
            }
        }

        [SerializeField] private List<Toggle> toggleList;

        // 決定ボタン
        [SerializeField] private UIButton applyButton;

        private Param param = null;

        // 自動配置に設定されているゴールのIndex番号(変更前) 
        private HashSet<long> defaultSpotIndexList = new HashSet<long>();

        // アクティブなゴールIndex番号(変更後)
        private HashSet<long> activeSpotIndexList = new HashSet<long>();

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            param = (Param)args;
            // 更新したかのパラメータを設定
            SetCloseParameter(false);
            
            // 自動配置するゴールのIndex
            long[] spotIndexList = param.BoardInfo.battleGameliftSetting.teamPlacement.spotIndexList;

            if (spotIndexList != null)
            {
                // 現在の設定状況を表示
                foreach (long index in spotIndexList)
                {
                    // toggleのIndex番号(toggleは0からなので-1)
                    int toggleIndex = (int)index - 1;
                    toggleList[toggleIndex].SetIsOnWithoutNotify(true);
                    // 初期状態のIndex番号を追加しておく
                    defaultSpotIndexList.Add(index);
                    activeSpotIndexList.Add(index);
                }
            }

            // 変更前は決定ボタンを非活性に
            applyButton.interactable = false;
            return base.OnPreOpen(args, token);
        }

        //// <summary> トグルをクリックしたときの処理 </summary>
        public void OnClickToggleButton(int toggleIndex)
        {
            // 選択されたトグルを取得
            Toggle toggle = toggleList[toggleIndex];

            // 自動配置のIndex番号に変換
            int spotIndex = toggleIndex + 1;

            // Onにした場合はListに追加
            if (toggle.isOn)
            {
                activeSpotIndexList.Add(spotIndex);
            }
            // Offの場合はListから削除
            else
            {
                activeSpotIndexList.Remove(spotIndex);
            }

            // 決定ボタンを押せるか(初期状態から変更があるか)
            bool isApplyButtonInteractable = false;

            // 要素数が違うなら変更されている
            if (defaultSpotIndexList.Count != activeSpotIndexList.Count)
            {
                isApplyButtonInteractable = true;
            }
            // 要素が同じ場合は中の要素が一致しているかで判断する
            else
            {
                isApplyButtonInteractable = defaultSpotIndexList.All(x => activeSpotIndexList.Contains(x)) == false;
            }

            // 変更がないならボタンは押せない
            applyButton.interactable = isApplyButtonInteractable;
        }

        public void OnClickSetAutoFormationButton()
        {
            OnClickSetAutoFormationButtonAsync().Forget();
        }

        //// <summary> 自動配置の決定時の処理 </summary>
        public async UniTask OnClickSetAutoFormationButtonAsync()
        {
            // 現在、自動配置の変更ができるか
            if (ClubRoyalManager.CanChangeAutoFormationSetting(param.MatchInfo) == false)
            {
                // 自動配置設定モーダルを閉じる
                await CloseAsync();
                ClubRoyalManager.OpenCantChangeAutoFormationModal(param.MatchInfo).Forget();
                return;
            }

            BattleGameliftEditPreparationAPIRequest request = new BattleGameliftEditPreparationAPIRequest();
            BattleGameliftEditPreparationAPIPost post = new BattleGameliftEditPreparationAPIPost();
            // ゲームタイプを取得
            long eventType = param.MatchInfo.GetBattleGameLiftMasterObject().eventType;
            post.mBattleGameliftId = param.MatchInfo.SeasonData.MColosseumEvent.inGameSystemId;
            post.eventType = eventType;
            post.masterId = param.MatchInfo.SeasonData.SeasonHome.mColosseumEventId;
            post.setting = new PlayerGameliftOptionSetting();
            post.setting.userId = UserDataManager.Instance.user.uMasterId;
            post.setting.teamPlacement = new PlayerGameliftOptionTeamPlacement();
            // 新しく設定したIndex番号をセット
            post.setting.teamPlacement.spotIndexList = activeSpotIndexList.ToArray();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            SetCloseParameter(true);
            Close();
        }

        //// <summary> キャンセルボタンクリック時の処理 </summary>
        public void CancelButton()
        {
            // 何か変更されているなら変更未確定な旨を表示するモーダルを開く.
            if (applyButton.interactable)
            {
                ConfirmModalButtonParams negativeButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], (m)=>m.Close());
                ConfirmModalButtonParams positiveButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.move"], (m)=>
                {
                    m.Close();
                    Close();
                });
                string title = StringValueAssetLoader.Instance["club-royal.auto_formation_modal.caution.title"];
                string message = StringValueAssetLoader.Instance["club-royal.auto_formation_modal.not_saved.message"];
                ConfirmModalData data = new ConfirmModalData(title, message, string.Empty, positiveButton, negativeButton);
            
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                return;
            }

            Close();
        }
    }
}