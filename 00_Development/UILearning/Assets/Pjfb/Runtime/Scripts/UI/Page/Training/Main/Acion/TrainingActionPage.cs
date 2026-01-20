using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using Pjfb.Utility;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingActionPage : TrainingPageBase
    {
        [SerializeField]
        private TrainingStatusValuesView trainingStatusView = null;
        
        [SerializeField]
        private UIButton restButton = null;
        
        [SerializeField]
        protected TrainingActionCardView[] cardViews = null;
        
        [SerializeField]
        private GameObject joinCharacterIconRoot = null;
        [SerializeField]
        private IconImage[] joinCharactersIcon = null;
        [SerializeField]
        private GameObject[] joinCharactersExtraIcon = null;
        
        [SerializeField] 
        protected GameObject combinationLockRoot = null;
        [SerializeField] 
        protected GameObject combinationSkillCountRoot = null;
        [SerializeField]
        private TMP_Text combinationSkillCountText = null;
        
        [SerializeField]
        private UIButton revenueMatchButton = null;
        [SerializeField]
        private UIBadgeBalloon revenueMatchBalloon = null;
        
        [SerializeField]
        private TrainingInspirationDescriptionCrossfade inspirationIconCrossfade = null;
        [SerializeField]
        private UIButton getInspirationButton = null;
        
        [SerializeField]
        private TrainingActionBoostPointView boostPointView = null;

        [SerializeField]
        private TrainingBoostChanceView boostChanceViewPrefab = null;
        
        [SerializeField]
        private TrainingCardRedrawAnimation redrawAnimation = null;

        [SerializeField]
        private TrainingCardComboHighlightView cardComboHighlightView;
        
        // 生成したブースト表示
        private TrainingBoostChanceView boostChanceView = null;
        
        // 現在選択中のカードIndex
        protected int selectedCardIndex = -1;
        
        protected TrainingPending pending = null;
        
        // 現在のコンディションデータ
        private TrainingConditionStateData currentTrainingCondition = null;

        /// <summary> カードの表示更新 </summary>
        private void UpdateCardView(TrainingPending pending)
        {
            selectedCardIndex = -1;
            // 非選択状態のViewにセット
            SetDeselectView();
            
            // トレーニングId
            TrainingScenarioMasterObject mTrainingScenario = MasterManager.Instance.trainingScenarioMaster.FindData(pending.mTrainingScenarioId);  
            
            // カードの表示
            for(int i=0;i<cardViews.Length;i++)
            {
                if(i >= pending.handList.Length)
                {
                    cardViews[i].gameObject.SetActive(false);
                }
                else
                {
                    long[] joinCharacters = pending.handSupportMCharaIdList[i].l;
                    
                    List<long> trainingInspireIds = new List<long>();
                    // インスピレーション
                    trainingInspireIds.AddRange(MainArguments.GetInspirationIds(pending.handList[i].mTrainingCardId, pending.handList[i].mCharaId));

                    // カード表示
                    cardViews[i].SetCard(i, pending.handList, pending.mCharaId, joinCharacters, trainingInspireIds.ToArray(), OnSelectedCard);


                    // レベル情報
                    foreach(TrainingPracticeProgress progress in pending.practiceProgressList)
                    {
                        if(progress.practiceType == pending.handList[i].practiceType)
                        {
                            // 経験値
                            EnhanceLevelMasterObject currentLevel = TrainingUtility.GetEnhanceMaster(mTrainingScenario.practiceMEnhanceId, progress.level);
                            // 次のレベル
                            EnhanceLevelMasterObject nextLevel = TrainingUtility.GetEnhanceMaster(mTrainingScenario.practiceMEnhanceId, progress.level + 1);
                            
                            // 最大レベル？
                            bool isMaxLv = nextLevel == null;
                            // カードレベル
                            cardViews[i].SetCardLv(currentLevel == null ? 0 : progress.level, isMaxLv);
                            
                            // 最大レベルのときは表示しない
                            if(isMaxLv)
                            {
                                cardViews[i].SetExpProgress( 0, 0 );
                            }
                            else
                            {
                                long nextTotalExp = nextLevel.totalExp - currentLevel.totalExp;
                                float currentProgress = (float)(progress.exp - currentLevel.totalExp) / (float)(nextTotalExp);
                                float nextProgress = (float)(progress.exp - currentLevel.totalExp + pending.handRewardList[i].exp) / (float)nextTotalExp;
                                cardViews[i].SetExpProgress( currentProgress, nextProgress);
                            }
                        }
                    }

                    cardViews[i].gameObject.SetActive(true);
                }
            }
            
            // 現在の手札のカードリストとカードViewを渡して発動するコンボをセットする
            cardComboHighlightView.SetCardComboData(pending.handList, pending.handRewardList, cardViews);
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // キャラを表示
            MainPageManager.Character.gameObject.SetActive(true);
            SetCombinationUi();
            
            // カードの読み込み
            for(int i=0;i<cardViews.Length;i++)
            {
                await cardViews[i].CardView.SetCardAsync( MainArguments.Pending.handList[i].mTrainingCardId );
            }
            
            await base.OnPreOpen(args, token);
        }

        private void UpdateView()
        {
            this.pending = MainArguments.Pending;
            
            // カードの更新
            UpdateCardView(MainArguments.Pending);
            
            // キャラを表示
            MainPageManager.Character.gameObject.SetActive(true);
               
            // トレーニングId
            TrainingScenarioMasterObject mTrainingScenario = MasterManager.Instance.trainingScenarioMaster.FindData(MainArguments.Pending.mTrainingScenarioId);  
            // 休憩ボタン
            restButton.interactable = TrainingUtility.GetConditionState(pending.mTrainingScenarioId, pending.condition, pending.conditionType).CanRest;
            // レベニューマッチボタン
            revenueMatchButton.gameObject.SetActive( mTrainingScenario.intentionalEventCount > 0 );
            // レベニューマッチのBalloon表示
            if(MainArguments.Pending.intentionalEventList.Length > 0 && MainArguments.Pending.intentionalEventList[0].rarity > 0)
            {
                long rarity = MainArguments.Pending.intentionalEventList[0].rarity;
                revenueMatchBalloon.SetText( StringValueAssetLoader.Instance[$"training.revenue_match_balloon.rarity{rarity}"] );
                revenueMatchBalloon.SetActive(true);
            }
            else
            {
                revenueMatchBalloon.SetActive(false);
            }

            // ブーストボーナス
            if(TrainingUtility.IsEnableBoostPoint(MainArguments.Pending.mTrainingScenarioId) && MainArguments.PointStatus.mTrainingPointStatusEffectIdList.Length > 0)
            {
                Header.BoostView.SetData(MainArguments);
                Header.BoostView.gameObject.SetActive(true);
            }
            else
            {
                Header.BoostView.gameObject.SetActive(false);
            }
            
            // インスピレーションボタンの表示切り替え
            getInspirationButton.gameObject.SetActive(MainArguments.EnableInspiration());
            // 一つも獲得してない場合はグレーアウト
            getInspirationButton.interactable = MainArguments.Pending.inspireList.Length > 0;
            
            // ブーストポイント
            if(TrainingUtility.IsEnableBoostPoint(MainArguments.Pending.mTrainingScenarioId))
            {
                boostPointView.gameObject.SetActive(true);
                // ポイントマスタ
                boostPointView.SetView(MainArguments);
            }
            else
            {
                boostPointView.gameObject.SetActive(false);
            }
            
            // ヘッダーの更新
            Header.UpdateCondition(pending);
            // ヘッダーの表示
            Header.SetActiveActionPage();
            
            // コンセントレーション
            if (MainArguments.HasConcentrationEffect())
            {
                if (MainArguments.IsFlow())
                {
                    Header.ConcentrationLabelEffectPlayer.PlayEffect(TrainingConcentrationEffectType.Flow, MainArguments.GetConcentrationEffectId(), 1.0f);
                }
                else
                {
                    Header.ConcentrationLabelEffectPlayer.PlayEffect(TrainingConcentrationEffectType.Concentration, MainArguments.GetConcentrationEffectId(), 1.0f);
                }
            }

            // カードコンボのハイライト表示
            cardComboHighlightView.ShowHighlightAsync().Forget();
            
            // コンディション変化のメッセージ
            if(MainArguments.Reward != null && MainArguments.Reward.condition != 0)
            {
                SetMessage( TrainingUtility.GetConditionChangeMessage(MainArguments.Reward.condition) );
            }
            
            // コンディションチェック
            TrainingConditionStateData afterCondition = TrainingUtility.GetConditionState(MainArguments.Pending.mTrainingScenarioId, MainArguments.Pending.condition, MainArguments.Pending.conditionType);
            
            // コンディション変化によるボイス再生(初回遷移時も再生)
            if(currentTrainingCondition == null || currentTrainingCondition.VoiceType != afterCondition.VoiceType)
            {
                PlayTrainingCharacterVoice(afterCondition.VoiceType);
            }
            
            // コンディション
            currentTrainingCondition = afterCondition;
            
            // Advログの保存
            TrainingUtility.SaveLog(Adv);   
        }

        protected override void OnEnablePage(object args)
        {
            UpdateView();
            base.OnEnablePage(args);

#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            DebugAutoChoice();
#endif
        }

#if CRUFRAMEWORK_DEBUG && !PJFB_REL
        private void DebugAutoChoice()
        {
            if (!TrainingChoiceDebugMenu.EnabledAutoChoiceAction) return;

            switch (TrainingChoiceDebugMenu.CurrentActionChoiceMode)
            {
                case DebugActionChoiceMode.Manual:
                    break;
                case DebugActionChoiceMode.AutoMostGrowth:
                    DebugAutoChoiceMostGrowth();
                    break;
                case DebugActionChoiceMode.AutoLeastGrowth:
                    DebugAutoChoiceLeastGrowth();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DebugAutoChoiceMostGrowth()
        {
            var shouldRevenueMatch = revenueMatchButton.gameObject.activeInHierarchy && !IsAchievingNextGoal();
            if (shouldRevenueMatch)
            {
                OnRevenueMatchButton();
                return;
            }

            var shouldRest = restButton.interactable;
            if (shouldRest)
            {
                OnRestButton();
                return;
            }

            var i = TrainingChoiceDebugMenu.GetMostGrowthRewardIndex(pending.handRewardList);
            OnSelectedCard(cardViews[i]);
            OnSelectedCard(cardViews[i]);
        }

        private bool IsAchievingNextGoal()
        {
            var goal = MainArguments.CurrentTarget;
            if (goal == null)
            {
                return true;
            }
            
            var element = MasterManager.Instance.charaVariableConditionElementMaster.FinDataByCharaVariableConditionId(goal.mCharaVariableConditionId);
            if (element == null)
            {
                return true;
            }
            
            return MainArguments.CurrentTipCount >= element.value;
        }

        private void DebugAutoChoiceLeastGrowth()
        {
            var i = TrainingChoiceDebugMenu.GetLeastGrowthRewardIndex(pending.handRewardList);
            OnSelectedCard(cardViews[i]);
            OnSelectedCard(cardViews[i]);
        }
#endif

        protected virtual async UniTask DecisionCardAsync(int index)
        {
            // パフォーマンス表示を初期化
            Header.PerformaceView.Set(MainArguments.Pending.overallProgress, 0, false);
            // Flowレベルを更新
            Header.FlowLevelView.UpdateView(MainArguments);
            // マスタ取得
            TrainingCard card = pending.handList[cardViews[index].Index];
            TrainingCardMasterObject mCard = MasterManager.Instance.trainingCardMaster.FindData(card.mTrainingCardId);
            
            // 練習に参加しているキャラ
            long[] supportedIdList = MainArguments.Pending.handSupportMCharaIdList[selectedCardIndex].l;
            
            // Progress
            TrainingProgressAPIResponse res = await TrainingUtility.ProgressAPI(index);
            TrainingMainArguments args = new TrainingMainArguments(res, mCard.practiceName, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            
            args.TrainingCardId = card.mTrainingCardId;
            args.SelectedTrainingCardIndex = selectedCardIndex;
            args.JoinSupportCharacters = supportedIdList;
            
            // 結果画面に
            OpenPage(TrainingMainPageType.EventResult, args);
            
        }
        
        protected override UniTask<bool> OnPreClose(CancellationToken token)
        {
            // エフェクトの非表示
            foreach(TrainingActionCardView cardView in cardViews)
            {
                cardView.DisableJoinCharacterEffect();
            }
            return base.OnPreClose(token);
        }
        
        /// <summary>カードがドラッグされた</summary>
        private void OnBeginDrag(TrainingActionCardView cardView)
        {
            // 選択中のカードの選択を解除
            if(selectedCardIndex >= 0)
            {
                cardViews[selectedCardIndex].Deslect();
                selectedCardIndex = -1;
            }
        }

        /// <summary> カードを非選択状態にする </summary>
        private void DeselectCard()
        {
            // 選択中のカードの選択を解除
            if (selectedCardIndex >= 0)
            {
                cardViews[selectedCardIndex].Deslect();
                selectedCardIndex = -1;
            }
            
            SetDeselectView();
        }

        /// <summary> 非選択時のViewにセット </summary>
        private void SetDeselectView()
        {
            // ステータスの反映
            trainingStatusView.SetStatus(MainArguments.Status);
            // カードのステータスを初期化
            trainingStatusView.SetStatusUpValues(new CharacterStatus());
            // インスピレーション初期化
            inspirationIconCrossfade.SetIds(new long[0]);
            // 参加キャラ表示を消す
            joinCharacterIconRoot.SetActive(false);
            // 増加後のFLOWレベルを非表示
            Header.FlowLevelView.ShowAfterLevel(false);
        }

        /// <summary>カードが選択された</summary>
        private void OnSelectedCard(TrainingActionCardView cardView)
        {
            // ドラッグ中は選択不可
            if(UIDrag.IsDraggingObject)return;
            // マスタ取得
            TrainingCard card =　pending.handList[cardView.Index];
            TrainingCardMasterObject mCard = MasterManager.Instance.trainingCardMaster.FindData(card.mTrainingCardId);
            
            // ステータスを表示
            TrainingCardReward status = MainArguments.Pending.handRewardList[cardView.Index];
            trainingStatusView.SetStatusUpValues( TrainingUtility.GetStatus(status) );
            
            // 選択中のカードを選択した場合は決定
            if(selectedCardIndex == cardView.Index)
            {
                DecisionCardAsync(cardView.Index).Forget();
                return;
            }
            
            // パフォーマンス
            Header.PerformaceView.Set(MainArguments.Pending.overallProgress, status.hp, false);
            
            // 選択解除アニメーション
            if (selectedCardIndex >= 0)
            {
                cardViews[selectedCardIndex].Deslect();
            }
            // 選択したカードを記録
            selectedCardIndex = cardView.Index;
            
            int joinCharacterCount = pending.handSupportMCharaIdList[selectedCardIndex].l.Length;
            
            if(joinCharacterCount <= 0)
            {
                joinCharacterIconRoot.SetActive(false);
            }
            else
            {
                joinCharacterIconRoot.SetActive(true);
                // 参加しているキャラ
                for(int i=0;i<joinCharactersIcon.Length;i++)
                {
                    IconImage icon = joinCharactersIcon[i];
                    GameObject exRoot = joinCharactersExtraIcon[i];
                    if(joinCharacterCount <= i)
                    {
                        icon.Cancel();
                        icon.gameObject.SetActive(false);
                        exRoot.SetActive(false);
                    }
                    else
                    {
                        long charId = pending.handSupportMCharaIdList[selectedCardIndex].l[i];
                        icon.gameObject.SetActive(true);
                        icon.SetTexture(charId);
                        exRoot.SetActive( CharacterUtility.IsExtraCharacter(charId) );
                    }
                }
            }
            
            // 選択しているカードに乗っているインスピレーション
            List<long> inspirationIds = new List<long>();
            inspirationIds.AddRange(MainArguments.GetInspirationIds(card.mTrainingCardId, card.mCharaId));

            // Idを登録
            inspirationIconCrossfade.SetIds(inspirationIds);

            // 選択したカードを設定し連結エフェクトを表示する
            cardComboHighlightView.SelectCardComboConnectLineAsync(selectedCardIndex).Forget();
            
            // 獲得できる経験値の表示をする
            Header.FlowLevelView.SetAfterLevelView(MainArguments.Pending.concentrationExp, status.concentrationExp);

            // カード選択
            cardView.Select();
        }
        
        protected override void OnOpened(object args)
        {

        }
        
        
        /// <summary>UGUI</summary>
        public void OnRevenueMatchButton()
        {
            OnRevenueMatchButtonAsync().Forget();
        }
        
        private async UniTask OnRevenueMatchButtonAsync()
        {

            TrainingProgressAPIResponse response = null; 
            // モーダルを開く
            CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.RevenueMatch, MainArguments);
            // 閉じるまで待つ
            response = (TrainingProgressAPIResponse)await modalWindow.WaitCloseAsync();
            
            // 試合準備画面へ
            if(response != null)
            {
                OpenPage(TrainingMainPageType.Top, new TrainingMainArguments(response, MainArguments.ArgumentsKeeps));
            }
        }
        
        /// <summary>UGUI</summary>
        public void OnDetailButton()
        {
            // 習得済みスキルリスト
            List<SkillData> skillDatas = new List<SkillData>();
            foreach(TrainingAbility a in MainArguments.CharacterVariable.abilityList)
            {
                skillDatas.Add( new SkillData(a.id, a.level) );
            }
            
            foreach(TrainingSupport supportCharacter in MainArguments.Pending.supportDetailList)
            {
            }
            
            TrainingCharacterDetailModal.Arguments args = new TrainingCharacterDetailModal.Arguments(MainArguments.Pending.mTrainingScenarioId, MainArguments.Pending.mCharaId, MainArguments.TrainingCharacter.Lv, MainArguments.TrainingCharacter.LiberationId, MainArguments.Status, skillDatas);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingCharacterDetail, args);
        }
        
        /// <summary>UGUI</summary>
        public void OnCombinationListButton()
        {
            if (!CombinationManager.IsUnLockCombination())
            {
                var systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(CombinationManager.CombinationLockId);            
                if(systemLock != null && !string.IsNullOrEmpty(systemLock.description))
                {
                    string description = systemLock.description;
                    ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                    ConfirmModalData data = new ConfirmModalData(StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                    return;
                }
            }
            
            List<TrainingCharacterData> characterList = GetTrainingCharacterDataList();
        
            // モーダルに送るデータ
            TrainingCombinationListModal.Arguments args = new TrainingCombinationListModal.Arguments(characterList.ToArray(), true, false, MainArguments.Pending.createdAt);
        
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingCombinationList, args);
        }
        
        /// <summary>UGUI</summary>
        public void OnGetInspirationButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal( ModalType.GetInspiration, new TrainingGetInspirationModal.Arguments(MainArguments, selectedCardIndex) );
        }
        
        /// <summary>UGUI</summary>
        public void OnSkillListButton()
        {
            List<TrainingPracticeSkillModal.CharacterData> supportList = new List<TrainingPracticeSkillModal.CharacterData>();
            List<TrainingPracticeSkillModal.CharacterData> specialSupportList = new List<TrainingPracticeSkillModal.CharacterData>();
            List<TrainingPracticeSkillModal.CharacterData> equipmentList = new List<TrainingPracticeSkillModal.CharacterData>();
            List<TrainingPracticeSkillModal.CharacterData> adviserList = new List<TrainingPracticeSkillModal.CharacterData>();
            
            long[] joinCharacters = null;
            TrainingCard card = null;
            
            if(selectedCardIndex >= 0)
            {
                card = MainArguments.Pending.handList[selectedCardIndex];
                joinCharacters = MainArguments.Pending.handSupportMCharaIdList[selectedCardIndex].l;
            }
                
            foreach(TrainingSupport supportCharacter in MainArguments.Pending.supportDetailList)
            {
            
                TrainingCharacterData characterData = new TrainingCharacterData(supportCharacter.mCharaId, supportCharacter.level, supportCharacter.newLiberationLevel, supportCharacter.id);
                TrainingPracticeSkillModal.OptionType options = TrainingPracticeSkillModal.OptionType.None;
                // 選択中の練習に参加しているか
                if(joinCharacters != null)
                {
                    foreach(long charId in joinCharacters)
                    {
                        if(supportCharacter.mCharaId == charId)
                        {
                            options |= TrainingPracticeSkillModal.OptionType.Join;
                            break;
                        }
                    }
                }
            
                switch( (TrainingUtility.SupportCharacterType)supportCharacter.supportType)
                {
                    case TrainingUtility.SupportCharacterType.TrainingChar:
                        if (CharacterUtility.HasJoinTrainingHimselfBonus(characterData.Lv))
                        {
                            supportList.Add( new TrainingPracticeSkillModal.CharacterData(characterData, options));
                        }
                        break;
                    case TrainingUtility.SupportCharacterType.Add:
                    case TrainingUtility.SupportCharacterType.Friend:
                    case TrainingUtility.SupportCharacterType.Normal:
                        supportList.Add( new TrainingPracticeSkillModal.CharacterData(characterData, options));
                        break;
                    case TrainingUtility.SupportCharacterType.Special:
                    {
                        switch ((CardType)supportCharacter.cardType)
                        {
                            case CardType.SpecialSupportCharacter:
                            {
                                specialSupportList.Add(new TrainingPracticeSkillModal.CharacterData(characterData, options));
                                break;
                            }
                            // アドバイザーはスペシャルサポートタイプとして扱うのでカードタイプで分ける
                            case CardType.Adviser:
                            {
                                adviserList.Add(new TrainingPracticeSkillModal.CharacterData(characterData, options));
                                break;
                            }
                        }
                        break;
                    }
                    case TrainingUtility.SupportCharacterType.Equipment:
                        equipmentList.Add(new TrainingPracticeSkillModal.CharacterData(characterData, supportCharacter.statusIdList, options));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // 選択しているカードに付与されているスキルと、常時発動しているスキルをフィルタ
            var statusTypeList =
                MainArguments.Pending.activeTrainingStatusTypeList.Where(x => x.cardIndex == selectedCardIndex).ToArray();
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingPracticeSkill, new TrainingPracticeSkillModal.Arguments(card, statusTypeList, MainArguments.Pending.mTrainingScenarioId, true, supportList, specialSupportList, equipmentList, adviserList, false));
        }
        
        /// <summary>休憩</summary>
        public void OnRestButton()
        {
            RestButtonAsync().Forget();
        }
        
        /// <summary>休憩</summary>
        protected virtual async UniTask RestButtonAsync()
        {
            
            bool isExecute = true;
            // 表示するかオプションをチェック
            if(TrainingUtility.IsConfirmRestModal)
            {
                // モーダルを開く
                CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainingRestConfirm, null);
                // 閉じるまで待つ
                isExecute = (bool)await modalWindow.WaitCloseAsync();
            }
            
            // 休憩を実行
            if(isExecute)
            {
                // Progress
                TrainingProgressAPIResponse res =　await TrainingUtility.ProgressAPI(TrainingUtility.ProgressRest);
                // リザルトページを開く
                OpenPage(TrainingMainPageType.Top, new TrainingMainArguments(res, MainArguments.ArgumentsKeeps));
            }
        }
        
        /// <summary>UGUI</summary>
        public void OnBoostLevelUpButton()
        {
            // ビューがなければ読み込み
            if(boostChanceView == null)
            {
                boostChanceView = GameObject.Instantiate<TrainingBoostChanceView>(boostChanceViewPrefab, transform);
            }
            // 開く
            boostChanceView.Open(MainArguments, Adv, (newPending, newPoint) =>
                {
                    // 選択中のカードを解除
                    DeselectCard();
                    // Pending更新
                    MainArguments.UpdatePending(newPending);
                    // ポイント更新
                    MainArguments.UpdatePoint(newPoint);
                    // ビューの更新
                    UpdateView();
                },
                // 演出完了後にログを保存
                () => TrainingUtility.SaveLog(Adv));
        }
        
        /// <summary>UGUI</summary>
        public void OnHandReloadButton()
        {
            HandReloadButton().Forget();
        }
        
        /// <summary>カード引き直し</summary>
        private async UniTask HandReloadButton()
        { 
            bool isReloadCards = true;
            if (TrainingUtility.IsConfirmPracticeCardRedrawModal)
            { 
                CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainingCardRedoingConfirm, MainArguments.PointStatus);
                isReloadCards = (bool)await modal.WaitCloseAsync();
            } 

            if (isReloadCards)
            {
                // タッチガード表示
                AppManager.Instance.UIManager.System.TouchGuard.Show();
                // カード選択解除
                DeselectCard();

                // カードコンボのエフェクトをすべて止める
                await cardComboHighlightView.StopAllEffectAsync();
                
                // API
                TrainingResetHandAPIResponse responseData = await TrainingUtility.ResetHandAPI();
                
                // 手札更新
                MainArguments.UpdatePending(responseData.pending);
                // ポイントの更新
                MainArguments.UpdatePoint(responseData.pointStatus);
                
                // ポイントの見た目更新
                boostPointView.SetView(MainArguments);
                
                // SlideOut
                await redrawAnimation.PlayRedoingOutAnimation();
                
                // カードの更新
                UpdateCardView(MainArguments.Pending);
                
                // SlideIn
                await redrawAnimation.PlayRedoingInAnimation();
                
                // 演出完了後に再度ページの更新をする
                UpdateView();
                
                // ログを追加
                Adv.AddMessageLog(StringValueAssetLoader.Instance["training.log.hand_reload"], StringValueAssetLoader.Instance["training.log.hand_reload.description"], 0);
                
                // タッチガード非表示
                AppManager.Instance.UIManager.System.TouchGuard.Hide();
            }
        }
        
        protected virtual void SetCombinationUi()
        {
            List<TrainingCharacterData> trainingCharacterDataList = GetTrainingCharacterDataList();
            Dictionary<long, long> characterTupleDictionary = new Dictionary<long, long>();
            
            foreach(TrainingCharacterData c in trainingCharacterDataList)
            {
                characterTupleDictionary[c.MCharId] = c.Lv;
            }
            
            // コネクトスキル
            DateTime startAt = AppTime.Parse(MainArguments.Pending.createdAt);
            bool unlockCombination = CombinationManager.IsUnLockCombination();
            combinationLockRoot.SetActive(!unlockCombination);
            int trainingCombinationSkillCount = CombinationManager.GetActivatingCombinationTrainingListBefore(characterTupleDictionary, startAt).Count;
            combinationSkillCountRoot.SetActive(unlockCombination && trainingCombinationSkillCount > 0);
            // テキスト
            combinationSkillCountText.text = string.Format( StringValueAssetLoader.Instance["common.combination.current_active"], trainingCombinationSkillCount );
        }

        private List<TrainingCharacterData> GetTrainingCharacterDataList()
        {
            List<TrainingCharacterData> characterList = new List<TrainingCharacterData>();
            // トレーニングキャラ
            characterList.Add(MainArguments.TrainingCharacter);
            // サポートの追加
            foreach(TrainingSupport supportCharacter in MainArguments.Pending.supportDetailList)
            {
                TrainingCharacterData characterData = new TrainingCharacterData(supportCharacter.mCharaId, supportCharacter.level, supportCharacter.newLiberationLevel, -1);
                characterList.Add(characterData);
            }

            return characterList;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ObjectPoolUtility.UnloadAll();
        }
    }
}
