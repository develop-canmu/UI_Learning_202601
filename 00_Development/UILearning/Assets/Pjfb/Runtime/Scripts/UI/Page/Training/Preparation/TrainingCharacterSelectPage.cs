using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;
using Pjfb.Character;
using Pjfb.UserData;
using Pjfb.Voice;

namespace Pjfb.Training
{
    
    /// <summary>
    /// トレーニングキャラクタ選択画面
    /// </summary>
    
    public class TrainingCharacterSelectPage : TrainingPreparationPageBase
    {

        [SerializeField]
        private CharacterStatusView statusView = null;
        [SerializeField]
        private UserCharacterScroll userCharacterScroll = null;
        [SerializeField]
        private UIButton nextButton = null;
        [SerializeField] 
        private CharacterGrowthRateGroupView growthRateGroupView;
        
        
        private void SelectCharacterId(long id, long lv)
        {
            // 選択したキャラId
            Arguments.TrainingUCharId = id;
            // キャラ表示
            if(id < 0)
            {
                statusView.gameObject.SetActive(false);
            }
            else
            {
                //ユーザーキャラIDからキャラIdを取得
                long masterCharaId = UserDataManager.Instance.chara.Find(id).charaId;
                //成長率の表示
                growthRateGroupView.SetCharacter(masterCharaId, lv);
                statusView.gameObject.SetActive(true);
                statusView.SetUserCharacter(id, Arguments.TrainingScenarioId);
            }
            
            // 次へボタン
            nextButton.interactable = id >= 0;            
        }
        
        private void SelectFirstCharacter()
        {
            if(userCharacterScroll.ItemList.Count > 0)
            {
                // リストの最初のデータを取得
                CharacterScrollData charData = userCharacterScroll.ItemList[0];
                currentScrollData = charData;
                // スクロールアイコンも選択状態に
                userCharacterScroll.Scroll.SelectItem(userCharacterScroll.Scroll.GetItem(charData));
                // キャラを表示
                SelectCharacterId(charData.UserCharacterId, charData.CharacterLv);
            }
            else
            {
                userCharacterScroll.Scroll.DeselectAllItems();
                SelectCharacterId(-1, 0);
            }
        }


        public override void OnBackPage()
        {
            Initialize(false);
        }

        protected override void OnEnablePage(object args)
        { 
            base.OnEnablePage(args);
            Initialize( TransitionType != PageTransitionType.Back);
        }
        
        private void Initialize(bool isSelecrFirstCharacter)
        {
            // デッキ情報削除
            TrainingDeckUtility.SetCurrentTrainingDeck(null);
            
            // データ設定
            userCharacterScroll.TrainingScenarioId = Arguments.TrainingScenarioId;
            userCharacterScroll.SetUserCharacterList();
            userCharacterScroll.SetCharacterList();
            // スクロール更新
            userCharacterScroll.Refresh();
            
            if(currentScrollData == null || isSelecrFirstCharacter)
            {
                // 最初のキャラを選択
                SelectFirstCharacter();
            }
            else
            {
                
                // 選択し直し
                long id = currentScrollData.UserCharacterId;
                userCharacterScroll.SelectItemByUCharaId(id, out int index);
                if(index < 0)
                {
                    // 最初のキャラを選択
                    SelectFirstCharacter();
                }
                else
                {
                    currentScrollData = userCharacterScroll.ItemList[index];
                    SelectCharacterId(id, currentScrollData.CharacterLv);
                }
            }

            // 選択時
            userCharacterScroll.OnSelectedItem -= OnSelectedItem;
            userCharacterScroll.OnSelectedItem += OnSelectedItem;
            
            // ソートフィルター実行時
            userCharacterScroll.OnSortFilter -= OnSortFilter;
            userCharacterScroll.OnSortFilter += OnSortFilter;
            
            userCharacterScroll.OnReverseCharacterOrder -= OnSortFilter;
            userCharacterScroll.OnReverseCharacterOrder += OnSortFilter;

            userCharacterScroll.OnSwipeDetailModal = (data) =>
            {
                int index = userCharacterScroll.ItemListSrc.ToList().IndexOf(data);
                if (index >= 0) userCharacterScroll.Scroll.SelectItem(index);
                OnSelectedItem(data);
            };
        }

        private CharacterScrollData currentScrollData;
        private void OnSelectedItem(CharacterScrollData characterData)
        {
            // キャラ表示
            currentScrollData = characterData;
            SelectCharacterId(characterData.UserCharacterId, characterData.CharacterLv);
        }

        public void OpenDetail()
        {
            BaseCharacterDetailModal.Open(ModalType.BaseCharacterDetail,
                new BaseCharaDetailModalParams(currentScrollData.SwipeableParams, true, false, "training.character_info", -1, true));
        }
        
        

        private void OnSortFilter()
        {
            // 最初のキャラを選択
            SelectFirstCharacter();
        }

        /// <summary>
        /// UGUI
        /// </summary>
        public void OnNextButton()
        {
            // ボイス
            VoiceManager.Instance.PlaySystemVoiceForLocationTypeAsync(UserDataManager.Instance.chara.Find(Arguments.TrainingUCharId).MChara, VoiceResourceSettings.LocationType.SYSTEM_TRAINING_SELECT ).Forget();
            // サポートキャラ選択画面へ移動
            TrainingPreparationManager.OpenPage(TrainingPreparationPageType.SupportCharacterDeckSelect, true, Arguments);

        }
    }
}