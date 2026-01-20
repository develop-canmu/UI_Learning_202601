using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

using Pjfb.Master;

namespace Pjfb.Common
{
    /// <summary>
    /// 練習メニューカード詳細モーダル
    /// </summary>
    public class TrainingCardRewardModal : ModalWindow
    {
        
        /// <summary>
        /// モーダル引数
        /// </summary>
        public class Arguments
        {
            private TrainingCardMasterObject mTrainingCard = null;
            /// <summary>マスタ</summary>
            public TrainingCardMasterObject MTrainingCard{get{return mTrainingCard;}}

            private long mCharId = 0;
            /// <summary>所持者</summary>
            public long MCharId{get{return mCharId;}}
            
            private long charaLevel = 0;
            /// <summary>現在のキャラLv</summary>
            public long CharaLevel{get{return charaLevel;}}
            
            private TrainingCardCharaMasterObject cardCharaMaster = null;
            /// <summary>カードキャラマスターLv</summary>
            public TrainingCardCharaMasterObject CardCharaMaster{get{return cardCharaMaster;}}
            
            private bool showLevelLabel = true;
            /// <summary>現在のカード強化ラベルレベルの表示するか</summary>
            public bool ShowLevelLabel{get{return showLevelLabel;}}
            
            private bool showEnhanceUI = true;
            /// <summary>「強化前」「強化後」「Lv.XXで強化」の強化関連系UIを表示するか</summary>
            public bool ShowEnhanceUI{get{return showEnhanceUI;}}

            public Arguments(TrainingCardMasterObject mTrainingCard, long mCharId, long charaLevel, TrainingCardCharaMasterObject cardCharaMaster, bool showLevelLabel, bool showEnhanceUI)
            {
                this.mTrainingCard = mTrainingCard;
                this.mCharId = mCharId;
                this.charaLevel = charaLevel;
                this.cardCharaMaster = cardCharaMaster;
                this.showLevelLabel = showLevelLabel;
                this.showEnhanceUI = showEnhanceUI;
            }
        }
        
        // 強化グループに属するカード一覧
        private TrainingCardCharaMasterObject[] targetTrainingGroupCards = System.Array.Empty<TrainingCardCharaMasterObject>();
        // 現在表示中の強化段階
        private long currentDisplayCardLevel = 0;
        // 現在表示中の強化段階のインデックス
        private int currentCardIndex = 0;
        // 最大強化段階
        private long maxEnhanceLevel = 0;
        // モーダル引数
        private Arguments arguments;
        
        
        [SerializeField] 
        private PracticeCardImage image;
        
        [SerializeField] 
        private TrainingCardRewardSheetReward reward;

        [SerializeField] 
        private TrainingCardRewardSheetDescription description;
        
        [SerializeField]
        private GameObject characterIconRoot = null;
        [SerializeField]
        private IconImage characterIcon = null;
        
        [Header("強化Lv表示関連")]
        [SerializeField]
        private GameObject currentEnhanceLevelLabelRoot = null;
        [SerializeField]
        private TMP_Text currentEnhanceLevelValueText = null;
        [SerializeField]
        private GameObject nextEnhanceLevelRoot = null;
        [SerializeField]
        private TMP_Text nextEnhanceLevelValueText = null;
        [SerializeField]
        private GameObject afterEnhanceLevelArrow = null;
        [SerializeField]
        private GameObject beforeEnhanceLevelArrow = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {

            arguments = (Arguments)args;
            
            // アイコン
            if (arguments.MCharId > 0)
            {
                characterIconRoot.SetActive(true);
                characterIcon.SetTexture(arguments.MCharId);
            }
            else
            {
                characterIconRoot.SetActive(false);
            }
            
            TrainingCardCharaMasterObject argumentsCardChara = arguments.CardCharaMaster;
            
            // 基本カードでないかつ、強化グループが存在している場合
            if (arguments.MTrainingCard.cardGroupType != TrainingCardGroup.Basic && argumentsCardChara.groupId > 0)
            {
                targetTrainingGroupCards = MasterManager.Instance.trainingCardCharaMaster.FindByGroupIdOrderByLevel(argumentsCardChara.groupId, argumentsCardChara.mCharaId);
                
                // 強化Lv情報の初期設定
                currentDisplayCardLevel = argumentsCardChara.displayEnhanceLevel;
                maxEnhanceLevel = targetTrainingGroupCards.LastOrDefault()?.displayEnhanceLevel ?? 0;
                
                currentCardIndex = System.Array.FindIndex(targetTrainingGroupCards, cardChara => cardChara.id == argumentsCardChara.id);
            }
            // カード詳細情報を表示
            RefreshTrainingCardDetail();

            return base.OnPreOpen(args, token);
        }

        /// <summary>
        /// 練習カード詳細情報の表示を更新
        /// 強化Lv情報、習得可能スキル、カード詳細を表示する
        /// </summary>
        private void RefreshTrainingCardDetail()
        {
            TrainingCardMasterObject currentDisplayCard = GetCurrentTrainingCardMaster();
            
            // カード画像設定
            image.SetTexture(currentDisplayCard.imageId);
            
            if (currentDisplayCardLevel > 0)
            {
                // 強化Lvラベルの表示更新
                currentEnhanceLevelLabelRoot.SetActive(arguments.ShowLevelLabel);
                currentEnhanceLevelValueText.text = string.Format(StringValueAssetLoader.Instance["practice_card.enhance_level_label"], currentDisplayCardLevel, maxEnhanceLevel);
                // キャラが強化レベル未満のレベルかつベースカード(解放カード)でない場合、次の強化Lv表示更新
                if(arguments.CharaLevel < targetTrainingGroupCards[currentCardIndex].level && currentCardIndex != 0)
                {
                    // 次の強化Lv表示更新
                    nextEnhanceLevelRoot.SetActive(arguments.ShowEnhanceUI);
                    nextEnhanceLevelValueText.text = string.Format(StringValueAssetLoader.Instance["practice_card.enhance_next_chara_level"], targetTrainingGroupCards[currentCardIndex].level);
                }
                else
                {
                    nextEnhanceLevelRoot.SetActive(false);
                }
            }
            else
            {
                currentEnhanceLevelLabelRoot.SetActive(false);
                nextEnhanceLevelRoot.SetActive(false);
            }
            
            // 習得可能スキル
            reward.SetDisplay(currentDisplayCard.id);
            // 練習メニューカード詳細
            description.SetDisplay(currentDisplayCard.description);
            
            RefreshDisplayEnhanceAllowView();
        }

        /// <summary>
        /// 現在表示する強化Lvのカードマスタを取得
        /// 強化グループがない場合はargumentsのカードマスタを返す
        /// </summary>
        private TrainingCardMasterObject GetCurrentTrainingCardMaster()
        {
            // 強化グループがある場合は現在の強化Lvのカードマスタを返す
            if (maxEnhanceLevel > 0)
            {
                // 強化グループ内から現在表示中の強化Lvのカードを探す
                foreach (TrainingCardCharaMasterObject cardChara in targetTrainingGroupCards)
                {
                    if(cardChara.displayEnhanceLevel == currentDisplayCardLevel)
                    {
                        return MasterManager.Instance.trainingCardMaster.FindData(cardChara.mTrainingCardId);
                    }
                }
            }
            
            // 強化グループがない場合は元のカードマスタを返す
            return arguments.MTrainingCard;
        }

        /// <summary>
        /// 強化の矢印の表示を更新
        /// </summary>
        private void RefreshDisplayEnhanceAllowView()
        {
            bool hasPrevious = currentDisplayCardLevel > 1;
            bool hasNext = currentDisplayCardLevel < maxEnhanceLevel;
            
            // 強化Lv矢印ボタンの表示制御
            beforeEnhanceLevelArrow.SetActive(hasPrevious && arguments.ShowEnhanceUI);
            afterEnhanceLevelArrow.SetActive(hasNext && arguments.ShowEnhanceUI);
        }

        /// <summary>
        /// 次の強化Lv詳細を表示
        /// </summary>
        public void OnClickRefreshDetailNextLevel()
        {
            currentDisplayCardLevel += 1;
            currentCardIndex += 1;
            // カード詳細情報を表示
            RefreshTrainingCardDetail();
        }

        /// <summary>
        /// 前の強化Lv詳細を表示
        /// </summary>
        public void OnClickRefreshDetailPreviousLevel()
        {
            currentDisplayCardLevel -= 1;
            currentCardIndex -= 1;
            // カード詳細情報を表示
            RefreshTrainingCardDetail();
        }
    }
}
