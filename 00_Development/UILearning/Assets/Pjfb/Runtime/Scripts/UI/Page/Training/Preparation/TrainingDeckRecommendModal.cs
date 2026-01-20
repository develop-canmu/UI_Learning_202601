using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingDeckRecommendModal : ModalWindow
    {

        
        [SerializeField]
        private TrainingDeckRecommendToggleGroup toggleGroup = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            SetCloseParameter(false);
            return base.OnPreOpen(args, token);
        }
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnOkButton()
        {
            // 選択しているタイプ
            TrainingDeckUtility.RecommendType type = toggleGroup.GetSelectedValue();

            
            TrainingPreparationArgs args = (TrainingPreparationArgs)ModalArguments; 
            // 編成メンバー取得
            long[] members = TrainingDeckUtility.GetRecommendMembers(type, args.TrainingUCharId, args.FriendList, args.TrainingScenarioId, out CharaV2FriendLend friend);
            // デッキを変更して返す
            DeckData deckData = args.DeckList.GetDeck(args.PartyNumber);
            for(int i = 0; i < members.Length; i++)
            {
                deckData.SetMemberId(i, members[i]);
            }
            // フレンド
            deckData.Friend = friend;
            
            // フレンド選択で表示されるリストを切り替える
            args.ChangeFriendSelectedUserChara(UserDataManager.Instance.chara.Contains(deckData.Friend.id));
            
            
            SetCloseParameter(true);
            // 閉じる
            Close();
        }
    }
}