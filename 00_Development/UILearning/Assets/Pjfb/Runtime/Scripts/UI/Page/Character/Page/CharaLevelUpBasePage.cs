using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using Pjfb.Storage;
using Pjfb.Training;
using Pjfb.UserData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pjfb.Character
{
    public abstract class CharaLevelUpBasePage : Page
    {
        public class Data
        {
            public long UserCharacterId;
            public int CurrentIndex;
            public List<long> UserCharacterIdList;
            public Action<long> OnSwipeChara;

            public Data(long userCharacterId, int currentIndex, List<long> userCharacterIdList, Action<long> onSwipeChara)
            {
                UserCharacterId = userCharacterId;
                CurrentIndex = currentIndex;
                UserCharacterIdList = userCharacterIdList;
                OnSwipeChara = onSwipeChara;
            }
        }

        [SerializeField] private PracticeCardView practiceCardViewPrefab; 
        [SerializeField] private Transform practiceCardRoot;
        [SerializeField] private PracticeSkillsView practiceSkillsView;
        [SerializeField] protected GrowthLiberationTabSheetManager growthLiberationTabSheetManager;
        [SerializeField] private GrowthLiberationEffectView growthEffectView;
        [SerializeField] private GrowthLiberationEffectView liberationEffectView;
        [SerializeField] private SwipeUi swipeUi;


        private List<PracticeCardView> practiceCardList = new();
        private List<List<TrainingCardCharaMasterObject>> groupedCardCharaMasters = new();

        protected Data data;
        protected UserDataChara uChara;

        public abstract void OnClickDetailButton();
        protected abstract UniTask InitializeNameViewAsync(UserDataChara chara);
        protected List<CharacterDetailData> detailOrderList;

        protected virtual async UniTask InitializePageAsync()
        {
            swipeUi.EnableSwipe = data.UserCharacterIdList.Count > 1;
            swipeUi.OnNextAction = NextCharacter;
            swipeUi.OnPrevAction = PrevCharacter;
            await SetCharacter();
            await growthLiberationTabSheetManager.OpenSheetAsync(GrowthLiberationTabSheetType.Growth, null);
            detailOrderList = data.UserCharacterIdList
                .Select(x => new CharacterDetailData(UserDataManager.Instance.chara.Find(x))).ToList();
            growthLiberationTabSheetManager.InitializeUI(data.UserCharacterId, OnModifyGrowth, OnModifyLiberation, OnGrowthLevelUp, OnLiberationLevelUp, PlayEffect);
        } 
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (Data)args;
            await InitializePageAsync();
            await base.OnPreOpen(args, token);
        }
        

        private async UniTask SetCharacter()
        {
            uChara = UserDataManager.Instance.chara.Find(data.UserCharacterId);
            practiceSkillsView.InitializeUI(data.UserCharacterId);
            await InitializeNameViewAsync(uChara);            
            // 該当キャラの全カードを取得し、カード概念ごとにグループ化
            groupedCardCharaMasters = MasterManager.Instance.trainingCardCharaMaster.GetGroupedPracticeCardOrderByLevel(uChara.charaId);

            // Viewの数をグループ数に合わせる
            for (int i = practiceCardList.Count; i < groupedCardCharaMasters.Count; i++)
            {
                PracticeCardView card = Instantiate(practiceCardViewPrefab, practiceCardRoot);
                practiceCardList.Add(card);
            }

            for (int i = 0; i < practiceCardList.Count; i++)
            {
                practiceCardList[i].gameObject.SetActive(i < groupedCardCharaMasters.Count);
            }

            await UpdatePracticeCardView(uChara.level);
        }
        
        private async UniTask UpdatePracticeCardView(long baseReferenceCharaLv)
        {
            // 表示用カードリストを取得
            List<TrainingCardCharaMasterObject> cardCharaMasters = MasterManager.Instance.trainingCardCharaMaster.GetDisplayCardListForCharaLevel(uChara.charaId, baseReferenceCharaLv);
            for (int i = 0; i < practiceCardList.Count; i++)
            {
                if (i >= groupedCardCharaMasters.Count) break;
                
                PracticeCardView cardView = practiceCardList[i];
                TrainingCardCharaMasterObject targetMaster = cardCharaMasters[i];
                // 表示用カードをセット
                await cardView.SetCardAsync(
                    targetMaster.mTrainingCardId, 
                    targetMaster.id, 
                    uChara.MChara.id,
                    baseReferenceCharaLv,
                    PracticeCardView.DisplayEnhanceUIFlags.Label | PracticeCardView.DisplayEnhanceUIFlags.NextLevel | 
                    PracticeCardView.DisplayEnhanceUIFlags.DetailLabel | PracticeCardView.DisplayEnhanceUIFlags.DetailEnhanceUI,
                    uChara.level);
            }
        }

        /// <summary> キャラレベル適用 </summary>
        private void OnModifyGrowth(long level)
        {
            ModifyLevel(GrowthLiberationTabSheetType.Growth, level);
        }

        /// <summary> 能力解放レベル適用 </summary>
        private void OnModifyLiberation(long level)
        {
            ModifyLevel(GrowthLiberationTabSheetType.Liberation, level);
        }
        
        /// <summary> レベルの適用処理(該当するシートを開いていない場合は現在のレベルでView更新を行うので設定レベルを書き換える) </summary>
        private void ModifyLevel(GrowthLiberationTabSheetType sheetType, long level)
        {
            // 現在のシートタイプが適用するシートと一致しているか
            bool isCurrentSheet = growthLiberationTabSheetManager.CurrentSheetType == sheetType;
            
            switch (sheetType)
            {
                case GrowthLiberationTabSheetType.Growth:
                {
                    // 強化タブを開いていないなら現在レベルで設定する
                    if (isCurrentSheet == false)
                    {
                        level = uChara.level;
                    }
                    OnModifyGrowthLevel(level);
                    break;
                }
                case GrowthLiberationTabSheetType.Liberation:
                {
                    // 能力解放タブを開いていないなら現在の解放レベルで設定する
                    if (isCurrentSheet == false)
                    {
                        level = uChara.newLiberationLevel;
                    }
                    OnModifyLiberationLevel(level);
                    break;
                }
            }
        }
        
        protected virtual void OnModifyGrowthLevel(long lv)
        {
            practiceSkillsView.SetAfterUi(lv);
            UpdatePracticeCardView(lv).Forget();
        }
        
        protected virtual void OnModifyLiberationLevel(long lv)
        {
        }
        
        protected virtual async UniTask OnGrowthLevelUp()
        {
            await SetCharacter();
            detailOrderList = data.UserCharacterIdList
                .Select(x => new CharacterDetailData(UserDataManager.Instance.chara.Find(x))).ToList();
        }
        
        protected virtual async UniTask OnLiberationLevelUp()
        {
            await SetCharacter();
            detailOrderList = data.UserCharacterIdList
                .Select(x => new CharacterDetailData(UserDataManager.Instance.chara.Find(x))).ToList();
        }
        
        public void NextCharacter()
        {
            data.CurrentIndex++;
            if (data.CurrentIndex >= data.UserCharacterIdList.Count) data.CurrentIndex = 0; 
            SetCharaViewByIndexAsync().Forget();
            data.OnSwipeChara?.Invoke(data.UserCharacterIdList[data.CurrentIndex]);
        }
        
        public void PrevCharacter()
        {
            data.CurrentIndex--;
            if (data.CurrentIndex < 0) data.CurrentIndex = data.UserCharacterIdList.Count - 1;
            SetCharaViewByIndexAsync().Forget();
            data.OnSwipeChara?.Invoke(data.UserCharacterIdList[data.CurrentIndex]);
        }

        protected void SetCharacterByIndex(int index)
        {
            data.CurrentIndex = index;
            SetCharaViewByIndexAsync().Forget();
            data.OnSwipeChara?.Invoke(data.UserCharacterIdList[data.CurrentIndex]);
        }

        private async UniTask SetCharaViewByIndexAsync()
        {
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            data.UserCharacterId = data.UserCharacterIdList[data.CurrentIndex];
            uChara = UserDataManager.Instance.chara.Find(data.UserCharacterId);
            await InitializePageAsync();
            
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
        }

        private bool isPlayingEffect;

        private async UniTask PlayPracticeCardUnlockEffect(long currentLv, long afterLv)
        {
            await AppManager.Instance.LoadingActionAsync(async ()=>
            { 
                // アニメーション用のタスクリストを作成
                List<UniTask> animationTasks = new List<UniTask>();

                for (int i = 0; i < practiceCardList.Count; i++)
                {
                    if (i >= groupedCardCharaMasters.Count) break;
            
                    List<TrainingCardCharaMasterObject> groupedCards = groupedCardCharaMasters[i];
            
                    // 現在レベルとレベルアップ後のレベルの間に更新されるカードがあるか
                    bool hasUpgrade = groupedCards.Any(x => x.level > currentLv && x.level <= afterLv);
        
                    if(hasUpgrade)
                    {
                        // 現在レベルでこのカード概念の最小レベルカードが解放されているか判定
                        bool isCardAlreadyUnlocked = groupedCards[0].level <= currentLv;
                
                        if (isCardAlreadyUnlocked)
                        {
                            // 既に解放済みのカードが強化される
                            animationTasks.Add(practiceCardList[i].PlayEnhanceAnimationAsync());
                        }
                        else
                        {
                            // 新規カードが開放される
                            practiceCardList[i].SetLock();
                            animationTasks.Add(practiceCardList[i].PlayUnlockAnimationAsync());
                        }
                    }
                }
        
                // 開放と強化のアニメーションを並列実行
                await UniTask.WhenAll(animationTasks);
            });
        }



        private void PlayEffect(GrowthLiberationTabSheetType type, long mCharaId, long currentLv, long afterLv,
            NativeApiAutoSell autoSell, List<CombinationManager.CollectionProgressData> prevCollectionProgressDataList)
        {
            switch (type)
            {
                case GrowthLiberationTabSheetType.Growth:
                    if (growthEffectView == null) return;
                    growthEffectView.InitializeUi(mCharaId, currentLv, afterLv,
                        async () =>
                        {
                            await PlayPracticeCardUnlockEffect(currentLv, afterLv);
                            // Lv100到達していてまだLv最大時チュートリアルを見ていないならチュートリアルを再生する(キャラクターのみ)
                            if (CharacterUtility.HasJoinTrainingHimselfBonus(afterLv) && uChara.CardType == CardType.Character && LocalSaveManager.saveData.isPlayCharaLevelUpMaxTutorial == false)
                            {
                                await AppManager.Instance.TutorialManager.OpenCharaLevelUpBaseMaxTutorialAsync();
                                LocalSaveManager.saveData.isPlayCharaLevelUpMaxTutorial = true;
                                LocalSaveManager.Instance.SaveData();
                            }
                            OpenCanActiveCombinationCollectionModal(mCharaId, prevCollectionProgressDataList);
                        });

                    break;
                case GrowthLiberationTabSheetType.Liberation:
                    var onClose = new Func<UniTask>( async () =>
                    {
                        if (autoSell.prizeListGot != null && autoSell.prizeListSold != null &&
                            (autoSell.prizeListGot.Length > 0 || autoSell.prizeListSold.Length > 0))
                        {
                            // 自動売却が発生
                            var modalData = new AutoSellConfirmModal.Data(autoSell);
                            var modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AutoSellConfirm,
                                modalData);
                            await modal.WaitCloseAsync();
                        }
                        
                        // シークレット判定
                        ShopManager.TryShowSaleIntroduction(SaleIntroductionDisplayType.CharacterLiberationEffected);
                    });
                    if (liberationEffectView == null)
                    {
                        onClose.Invoke().Forget();
                        return;
                    }

                    liberationEffectView.InitializeUi(mCharaId, currentLv, afterLv, onClose);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void OpenCanActiveCombinationCollectionModal(long mCharaId, List<CombinationManager.CollectionProgressData> prevProgressDataList)
        {
            var unlockCombination = CombinationManager.IsUnLockCombination();
            if (!unlockCombination) return;
            //　今回解放できるようになったもののみ表示するので古いリストと比較する
            var newProgressDataList = CombinationManager.GetCanActiveProgressDataListByCharaId(mCharaId);
            var hasCanActiveCombination = false;
            var allCollectionList = CombinationManager.GetAllCombinationCollectionList();
            var collectionProgressDataList = new List<CombinationManager.CollectionProgressData>();
            foreach (var newProgress in newProgressDataList)
            {
                var collectionProgressData = new CombinationManager.CollectionProgressData(newProgress.MCombinationId, new List<long>());
                var isSameData = true;
                var prevProgressData = prevProgressDataList.FirstOrDefault(prevProgress =>
                    prevProgress.MCombinationId == newProgress.MCombinationId);
                if (prevProgressData != null)
                {
                    foreach (var progressId in newProgress.ProgressIdList)
                    {
                        if(prevProgressData.ProgressIdList.Contains(progressId)) continue;
                        collectionProgressData.ProgressIdList.Add(progressId);
                        isSameData = false;
                    }
                }
                else
                {
                    foreach (var progressId in newProgress.ProgressIdList)
                    {
                        collectionProgressData.ProgressIdList.Add(progressId);
                        isSameData = false;
                    }
                }
                // 古いデータと現在のデータが同じ場合はcontinue
                if(isSameData) continue;
                hasCanActiveCombination = true;
                collectionProgressDataList.Add(collectionProgressData);
            }
            
            if(!hasCanActiveCombination) return;
            CombinationCollectionActivatableNotificationModal.Open(new CombinationCollectionActivatableNotificationModal.Data(allCollectionList, collectionProgressDataList));
        }
    }
}
