using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

using CruFramework.UI;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

using Pjfb.Common;
using Pjfb.UserData;

namespace Pjfb.Training
{
    /// <summary>
    /// 練習メニューカード表示用View
    /// 強化Lv情報、ロック状態、次の強化レベル情報を表示する
    /// </summary>
    public class PracticeCardView : MonoBehaviour
    {
        [Flags]
        public enum DisplayEnhanceUIFlags
        {
            None = 0,
            /// <summary>強化レベルラベル表示</summary>
            Label = 1 << 0,      // 1
            /// <summary>次の強化表示 "Lv.XXで強化"</summary>
            NextLevel = 1 << 1,       // 2
            /// <summary>詳細モーダルのレベルラベル表示</summary>
            DetailLabel = 1 << 2,// 4
            /// <summary>詳細モーダルの強化UI</summary>
            DetailEnhanceUI = 1 << 3,// 8
        }
        
        /// <summary>
        /// 表示データクラス
        /// </summary>
        public class Data
        {
            /// <summary>練習カードID</summary>
            public long CardId { get; }
            
            /// <summary>練習カードキャラクターID</summary>
            public long CardCharacterId { get; }

            /// <summary>マスターのキャラクターID</summary>
            public long MCharaId { get; }
            
            /// <summary>各種条件判定や表示などで使用される基準キャラクターレベル</summary>
            public long BaseReferenceCharaLv { get; }
            
            /// <summary>強化UI表示フラグ</summary>
            public DisplayEnhanceUIFlags DisplayEnhanceUIFlags { get;}
            
            /// <summary>ユーザーが持っている実際のキャラのレベル</summary>
            public long TargetUserCharaLv { get;}
            
            public Data(
                long cardId,
                long cardCharacterId,
                long mCharaId,
                long baseReferenceCharaLv,
                DisplayEnhanceUIFlags displayEnhanceUIFlags,
                long targetUserCharaLv = -1)
            
            {
                CardId = cardId;
                CardCharacterId = cardCharacterId;
                MCharaId = mCharaId;
                BaseReferenceCharaLv = baseReferenceCharaLv;
                DisplayEnhanceUIFlags = displayEnhanceUIFlags;
                TargetUserCharaLv = targetUserCharaLv;
            }
        }
    
        /// <summary>カードロックアニメーション</summary>
        public static readonly string UnlockedAnimation = "Unlocked";
        /// <summary>カードアンロックアニメーション</summary>
        public static readonly string LockedAnimation = "Locked";
        /// <summary>カード強化アニメーション</summary>
        public static readonly string EnhancedAnimation = "LevelUp";
    
        [SerializeField]
        private PracticeCardImage cardImage = null;
        [SerializeField]
        private GameObject lockRoot = null;
        [SerializeField]
        private TMP_Text lockText = null;
        [SerializeField]
        private Animator lockAnimator = null;
        [SerializeField]
        private GameObject lockNextEnhanceLevelRoot = null;
        [SerializeField]
        private TMP_Text lockNextEnhanceLevelText = null;
        
        [Header("強化Lv表示関連")]
        [SerializeField]
        private GameObject currentEnhanceLevelLabelRoot = null;
        [SerializeField]
        private TMP_Text currentEnhanceLevelValueText = null;
        [SerializeField]
        private GameObject nextEnhanceLevelRoot = null;
        [SerializeField]
        private TMP_Text nextEnhanceLevelText = null;
        [SerializeField]
        private Animator enhanceAnimator = null;
        
        private TrainingCardMasterObject mCard;
        /// <summary>カードマスタ</summary>
        public TrainingCardMasterObject MCard{get{return mCard;}}

        private TrainingCardCharaMasterObject cardChara;
        /// <summary>カードキャラマスタ</summary>
        public TrainingCardCharaMasterObject CardChara{get{return cardChara;}}
        
        // 表示中のデータクラス
        private Data cardData = null;
        /// <summary>
        /// データクラスから練習カード表示を設定
        /// </summary>
        public void SetData(Data data)
        {
            SetCard(data.CardId, data.CardCharacterId, data.MCharaId, data.BaseReferenceCharaLv, data.DisplayEnhanceUIFlags, data.TargetUserCharaLv);
        }
        
        public void SetCard(long cardId)
        {
            SetCard(cardId, -1, -1, -1, DisplayEnhanceUIFlags.None);
        }
        
        public void SetCard(long cardId, long cardCharacterId, long characterId, long baseReferenceCharaLv, DisplayEnhanceUIFlags displayEnhanceUIFlags, long targetUserCharaLv = -1)
        {
            SetCardAsync(cardId, cardCharacterId, characterId, baseReferenceCharaLv, displayEnhanceUIFlags, targetUserCharaLv).Forget();
        }
        
        public UniTask SetCardAsync(long cardId)
        {
            return SetCardAsync(cardId, -1, -1, -1, DisplayEnhanceUIFlags.None);
        }

        /// <summary>
        /// 練習カードの表示を設定
        /// </summary>
        /// <param name="cardId">練習カードID</param>
        /// <param name="cardCharacterId">練習カードキャラクターID</param>
        /// <param name="mCharacterId">マスターのキャラクターID</param>
        /// <param name="baseReferenceCharaLv">条件判定で使用されるキャラレベル</param>
        /// <param name="displayEnhanceUIFlags">強化UI表示フラグ</param>
        /// <param name="targetUserCharaLv">ユーザーのキャラレベル</param>
        public async UniTask SetCardAsync(long cardId, long cardCharacterId, long mCharacterId, long baseReferenceCharaLv, DisplayEnhanceUIFlags displayEnhanceUIFlags, long targetUserCharaLv = -1)
        {
            // mCard
            mCard = MasterManager.Instance.trainingCardMaster.FindData(cardId);
            
            // cardData
            cardData = new Data(cardId, cardCharacterId, mCharacterId, baseReferenceCharaLv, displayEnhanceUIFlags, targetUserCharaLv);
            bool showEnhanceLabel = cardData.DisplayEnhanceUIFlags.HasFlag(DisplayEnhanceUIFlags.Label);
            bool showNextEnhance = cardData.DisplayEnhanceUIFlags.HasFlag(DisplayEnhanceUIFlags.NextLevel);
            
            if (mCard == null) return;

            bool hasNextEnhance = false;
            string formattedNextEnhanceLevelText = string.Empty;
            if (cardCharacterId > 0)
            {
                cardChara = MasterManager.Instance.trainingCardCharaMaster.FindData(cardCharacterId);
            }
            // 強化Lv情報の表示
            if (cardChara != null && cardChara.groupId > 0)
            {
                // 強化Lv情報の取得
                TrainingCardCharaMasterObject[] groupCards = MasterManager.Instance.trainingCardCharaMaster.FindByGroupIdOrderByLevel(cardChara.groupId, cardChara.mCharaId);
                
                long currentEnhanceLv = cardChara.displayEnhanceLevel;
                long maxEnhanceLv = groupCards.Max(card => card.displayEnhanceLevel);
                
                // 強化Lvテキスト設定 "Lv.{0}/{1}"
                currentEnhanceLevelLabelRoot.SetActive(showEnhanceLabel);
                currentEnhanceLevelValueText.text = string.Format(StringValueAssetLoader.Instance["practice_card.enhance_level_label"], currentEnhanceLv, maxEnhanceLv);
                
                // 次の強化Lvがある場合
                if (currentEnhanceLv < maxEnhanceLv)
                {
                    TrainingCardCharaMasterObject nextCard = groupCards[currentEnhanceLv];
                    // 次の強化Lvテキスト設定 "Lv.{0}で強化"
                    formattedNextEnhanceLevelText = string.Format(StringValueAssetLoader.Instance["practice_card.enhance_next_chara_level"], nextCard.level);
                    hasNextEnhance = true;
                }
            }
            else
            {
                // 強化Lv情報がない場合は非表示
               currentEnhanceLevelLabelRoot.SetActive(false);
            }

            // ロック状態を考慮したい場合
            if (mCharacterId >= 0 && baseReferenceCharaLv >= 0)
            {
                TrainingCardCharaMasterObject mCardChar = MasterManager.Instance.trainingCardCharaMaster.FindData(cardCharacterId);
                // ロック状態
                if (mCardChar.level > baseReferenceCharaLv)
                {
                    lockRoot.SetActive(true);
                    // ロック状態であれば強化レベルラベルは非表示
                    currentEnhanceLevelLabelRoot.SetActive(false);
                    // テキスト
                    lockText.text = string.Format(StringValueAssetLoader.Instance["practice_card.lock"], mCardChar.level);
                    lockAnimator.SkipToEnd(LockedAnimation);
                    // 次の強化レベル表示
                    if (hasNextEnhance)
                    {
                        lockNextEnhanceLevelRoot.SetActive(showNextEnhance);
                        lockNextEnhanceLevelText.text = formattedNextEnhanceLevelText;
                    }
                    else
                    {
                        lockNextEnhanceLevelRoot.SetActive(false);
                    }
                    nextEnhanceLevelRoot.SetActive(false);
                }
                else
                {
                    lockRoot.SetActive(false);
                    // 次の強化レベル表示
                    lockNextEnhanceLevelRoot.SetActive(false);
                    if (hasNextEnhance)
                    {
                        nextEnhanceLevelRoot.SetActive(showNextEnhance);
                        nextEnhanceLevelText.text = formattedNextEnhanceLevelText;
                    }
                    else
                    {
                        nextEnhanceLevelRoot.SetActive(false);
                    }
                }
            }
            else
            {
                lockRoot.SetActive(false);
            }
            
            // 画像の読み込み
            await cardImage.SetTextureAsync(mCard.imageId);
        }

        public async UniTask PlayUnlockAnimationAsync()
        {
            lockRoot.SetActive(true);
            await AnimatorUtility.WaitStateAsync(lockAnimator, UnlockedAnimation);
        }
        
        /// <summary>強化アニメーションを再生</summary>
        public async UniTask PlayEnhanceAnimationAsync()
        {
            await AnimatorUtility.WaitStateAsync(enhanceAnimator, EnhancedAnimation);
        }

        public void OnLongTap()
        {
            long characterLevel;
            // ユーザーのキャラレベルが設定されていればそちらを優先
            if (cardData.TargetUserCharaLv > 0)
            {
                characterLevel = cardData.TargetUserCharaLv;
            }
            else
            {
                characterLevel = cardData.BaseReferenceCharaLv;
            }
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingCardReward, 
                new TrainingCardRewardModal.Arguments(
                mCard,
                0,
                characterLevel,
                cardChara,
                cardData.DisplayEnhanceUIFlags.HasFlag(DisplayEnhanceUIFlags.DetailLabel),
                cardData.DisplayEnhanceUIFlags.HasFlag(DisplayEnhanceUIFlags.DetailEnhanceUI)));
        }

        public void SetLock()
        {
            lockRoot.SetActive(true);
        }
    }
}