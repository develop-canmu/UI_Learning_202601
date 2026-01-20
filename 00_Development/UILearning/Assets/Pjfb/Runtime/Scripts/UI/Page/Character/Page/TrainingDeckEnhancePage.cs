using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;


namespace Pjfb.Character
{
    // View側にそれぞれのDeckTypeのデータを渡す用のクラス
    public class TrainingDeckEnhanceListData
    {
        private List<TrainingDeckEnhanceInfo> growthTargetEnhanceList;
        private List<TrainingDeckEnhanceInfo> trainingTargetEnhanceList;
        private List<TrainingDeckEnhanceInfo> friendEnhanceList;
        private List<TrainingDeckEnhanceInfo> supportEquipmentEnhanceList;

        // DeckType:GrowthTarget
        public List<TrainingDeckEnhanceInfo> GrowthTargetEnhanceList {get => growthTargetEnhanceList;}
        // DeckType:Training
        public List<TrainingDeckEnhanceInfo> TrainingTargetEnhanceList{get => trainingTargetEnhanceList;}
        // DeckType:Friend
        public List<TrainingDeckEnhanceInfo> FriendEnhanceList{get => friendEnhanceList;}
        // DeckType:SupportEquipment
        public List<TrainingDeckEnhanceInfo> SupportEquipmentEnhanceList{get => supportEquipmentEnhanceList;}
        
        public TrainingDeckEnhanceListData(List<TrainingDeckEnhanceInfo> growthEnhanceData, List<TrainingDeckEnhanceInfo> trainingEnhanceData, List<TrainingDeckEnhanceInfo> friendEnhanceData, List<TrainingDeckEnhanceInfo> spEquipmentEnhanceData)
        {
            growthTargetEnhanceList = growthEnhanceData;
            trainingTargetEnhanceList = trainingEnhanceData;
            friendEnhanceList = friendEnhanceData;
            supportEquipmentEnhanceList = spEquipmentEnhanceData;
        }
    }
    
    public class TrainingDeckEnhancePage : Page
    {
        [SerializeField] private GrowthLevelUpView levelUpView;
        
        [SerializeField] private TrainingEnhanceTabSheetManager sheetManager;

        [SerializeField] private TrainingEnhanceDeckEffectView deckEffectView;
        [SerializeField] private TrainingEnhanceDeckLevelUpEffectView deckLevelUpView;
        [SerializeField] private TrainingDeckEnhanceLevelUpAnimationView levelUpEffect;

        private TrainingDeckEnhanceListData enhanceListData;

        public long AfterLevel => levelUpView.AfterLv;
        private long currentLevel;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // シートを開いた時の処理を登録する
            sheetManager.OnPreOpenSheet -= UpdateView;
            sheetManager.OnPreOpenSheet += UpdateView;
            
            List<TrainingDeckEnhanceInfo> deckEnhanceTypeGrowthList = TrainingDeckEnhanceUtility.GetDeckEnhanceStatusDataList(DeckType.GrowthTarget);
            List<TrainingDeckEnhanceInfo> deckEnhanceTypeTrainingList = TrainingDeckEnhanceUtility.GetDeckEnhanceStatusDataList(DeckType.Training);
            List<TrainingDeckEnhanceInfo> deckEnhanceTypeFriendList = TrainingDeckEnhanceUtility.GetDeckEnhanceStatusDataList(DeckType.SupportFriend);
            List<TrainingDeckEnhanceInfo> deckEnhanceTypeSupportEquipmentList = TrainingDeckEnhanceUtility.GetDeckEnhanceStatusDataList(DeckType.SupportEquipment);

            enhanceListData = new TrainingDeckEnhanceListData(deckEnhanceTypeGrowthList, deckEnhanceTypeTrainingList, deckEnhanceTypeFriendList, deckEnhanceTypeSupportEquipmentList);
            
            currentLevel = TrainingDeckEnhanceUtility.CurrentLevel;
            long maxLevel = TrainingDeckEnhanceUtility.GetMaxEnhanceLevel();
            levelUpView.SetCanLevelUp(TrainingDeckEnhanceUtility.CanDeckEnhanceLevelUp);
            // Level更新時にほかのViewに更新をかける場合はこのコールバックに登録する
            levelUpView.SetModifyLevel(OnModifyLevel);
            // シートを開く時にもUpdateViewが実行されているのでレベルアップViewの初期化ではUpdateViewを実行しないように
            levelUpView.Initialize(currentLevel, maxLevel, TrainingDeckEnhanceUtility.GetDeckEnhancePointData(), false);
            
            // 編成強化のViewを初期化
            deckEffectView.Initialize();
            
            // シートを開く
            await sheetManager.OpenSheetAsync(TrainingEnhanceTabSheetType.DeckEffectScrollDynamic, null);
            
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
           
            // トレーニング編成強化のチュートリアル
            AppManager.Instance.TutorialManager.OpenTrainingDeckEnhanceTutorialAsync().Forget();
        }
        
        //// <summary> Levelが適用された際に今開いているシートを更新する </summary>
        private void OnModifyLevel(long level)
        {
            UpdateView(sheetManager.CurrentSheetType);
        }

        
        //// <summary> シートのタイプによってViewを更新する </summary>
        private void UpdateView(TrainingEnhanceTabSheetType type)
        {
            switch (type)
            {
                case TrainingEnhanceTabSheetType.DeckEffectScrollDynamic:
                    deckEffectView.UpdateView(currentLevel, AfterLevel, enhanceListData);
                    break;
                case TrainingEnhanceTabSheetType.DeckLvUpEffectScrollDynamic:
                    deckLevelUpView.UpdateView(currentLevel, AfterLevel, enhanceListData);
                    break;
            }
        }


        //// <summary> 強化ボタンを押した際の処理 </summary>
        public void OnClickGrowthButton()
        {
           EnhanceLevelUp().Forget();
        }

        //// <summary> 強化の実行 </summary>
        private async UniTask EnhanceLevelUp()
        {
            // API実行
            PlayerEnhanceAPIRequest request = new PlayerEnhanceAPIRequest();
            PlayerEnhanceAPIPost post = new PlayerEnhanceAPIPost();
            post.mPlayerEnhanceId = TrainingDeckEnhanceUtility.playerEnhanceId;
            post.level = AfterLevel;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            PlayerEnhanceAPIResponse responce = request.GetResponseData();
            // 強化データを更新
            PlayerEnhanceManager.UpdateEnhanceData(responce.playerEnhance);
            
            // フッターのバッジを更新する
            AppManager.Instance.UIManager.Footer.CharacterButton.SetNotificationBadge(BadgeUtility.IsCharacterBadge);
            
            // 演出を再生
            await levelUpEffect.PlayEffect(currentLevel, AfterLevel);

            TrainingDeckEnhanceLevelUpDetailModal.Param param = new TrainingDeckEnhanceLevelUpDetailModal.Param(currentLevel, AfterLevel, enhanceListData);
            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainingDeckEnhanceLevelUpDetail, param);

            // 閉じるまで待つ
            await modal.WaitCloseAsync();

            currentLevel = TrainingDeckEnhanceUtility.CurrentLevel;
            levelUpView.UpdateCurrentLv(currentLevel);
        }
    }
}