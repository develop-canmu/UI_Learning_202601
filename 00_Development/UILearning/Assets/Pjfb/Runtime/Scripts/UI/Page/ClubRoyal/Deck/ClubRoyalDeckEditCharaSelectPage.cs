using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using Pjfb.Deck;
using Pjfb.UserData;

namespace Pjfb.ClubRoyal
{
    public class ClubRoyalDeckEditCharaSelectPage : DeckEditCharaSelectPage
    {
        public class PageParam : DeckEditCharaSelectData { }

        private static readonly string DupulicateConfirmTextPath = "character.deck.maxdupulicate_confirm";
        
        protected override DeckData CurrentDeckData => ClubRoyalDeckPage.CurrentDeckData;

        [SerializeField]
        private TextMeshProUGUI mCharaDuplicateConfirmText;
        
        /// <summary>編成済みキャラのリスト</summary>
        private HashSet<long> allDeckUCharaIdList = new ();
        /// <summary>編成済みの同一キャラのDic</summary>
        private Dictionary<long, long> allDeckMCharaIdCountDictionary= new Dictionary<long, long>();

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 同一キャラの編成数上限のテキスト上限ない場合は一応表示しないようにしておく
            if (ClubRoyalDeckPage.MaxDuplicateMCharaCount == -1)
            {
                mCharaDuplicateConfirmText.gameObject.SetActive(false);
            }
            else
            {
                mCharaDuplicateConfirmText.gameObject.SetActive(true);
                mCharaDuplicateConfirmText.text = string.Format(StringValueAssetLoader.Instance[DupulicateConfirmTextPath], ClubRoyalDeckPage.MaxDuplicateMCharaCount);
            }
            
            // 編成してるキャラの数を取得
            UpdateFormatedChara();
            
            return base.OnPreOpen(args, token);
        }
        
        /// <summary>画像表示の切り替え</summary>
        protected override void SetBadge()
        {
            foreach (CharacterVariableScrollData scrollData in GetItems())
            {
                // 編成中のキャラ
                if (scrollData.id == currentEditingId)
                {
                    scrollData.DeckBadgeType = DeckBadgeType.CurrentEditing;
                }
                // 現在のチームに編成済みのキャラ
                else if(formattingIdSet.Contains(scrollData.id))
                {
                    scrollData.DeckBadgeType = DeckBadgeType.Formatting;
                }
                // 他のチームに編成されている
                else if (allDeckUCharaIdList.Contains(scrollData.id))
                {
                    scrollData.DeckBadgeType = DeckBadgeType.AssignedByOtherTeam;
                }
                // 同一キャラの編成数が上限に達している
                else if (ClubRoyalDeckPage.MaxDuplicateMCharaCount != -1 && allDeckMCharaIdCountDictionary.GetValueOrDefault(scrollData.MCharaId, 0) >= ClubRoyalDeckPage.MaxDuplicateMCharaCount)
                {
                    scrollData.DeckBadgeType = DeckBadgeType.ReachMCharaLimit;
                }
            }
        }
        
        /// <summary>編成済みのキャラを更新</summary>
        private void UpdateFormatedChara()
        {
            allDeckUCharaIdList.Clear();
            allDeckMCharaIdCountDictionary.Clear();
            foreach (DeckData deckData in ClubRoyalDeckPage.DeckListData.DeckDataList)
            {
                if (deckData.Index == ClubRoyalDeckPage.CurrentDeckIndex) continue;
                foreach (long id in deckData.GetMemberIds())
                {
                    if(id == DeckUtility.EmptyDeckSlotId) continue;
                    UserDataCharaVariable uChara = UserDataManager.Instance.charaVariable.Find(id);
                    if (uChara == null) continue;
                    long mCharaId = uChara.charaId;
                    // 編成しているキャラのIdをリストに追加
                    allDeckUCharaIdList.Add(id);
                    // 同一キャラの編成数をカウント
                    if (!allDeckMCharaIdCountDictionary.TryAdd(mCharaId, 1))
                    {
                        allDeckMCharaIdCountDictionary[mCharaId] += 1;
                    }
                }
            }
        }
        
        protected override void SetConfirmButtonInteractable()
        {
            confirmButton.interactable = selectingChara != null && 
                                         !allDeckUCharaIdList.Contains(selectingChara.id) && 
                                         allDeckMCharaIdCountDictionary.GetValueOrDefault(selectingChara.charaId, 0) < ClubRoyalDeckPage.MaxDuplicateMCharaCount;
        }
    }
}