using System;
using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Training
{
    /// <summary>
    /// カードユニオン一覧表示用View
    /// </summary>
    public class TrainingCardUnionInformationListView : MonoBehaviour
    {
        [SerializeField]
        private ScrollDynamic cardUnionInformationScrollDynamic = null;
        protected ScrollDynamic CardUnionInformationScrollDynamic => cardUnionInformationScrollDynamic;
        
        // 選出された練習メニューカード情報に対応するベースの色 
        [SerializeField]
        [ColorValue] 
        private string selectedColorKey = string.Empty;
        // カードユニオン対象のメニューカード情報に対応するベースの色
        [SerializeField]
        [ColorValue] 
        private string cardUnionTargetColorKey  = string.Empty;
        
        // 選出された育成選手の練習メニューカードのヘッダータイトルキー
        [SerializeField]
        [StringValue]
        private string selectedCardHeaderTitleKey = string.Empty;
        protected string SelectedCardHeaderTitleKey => selectedCardHeaderTitleKey;
        
        // カードユニオン対象の練習メニューカードのヘッダータイトルキー
        [SerializeField]
        [StringValue]
        private string cardUnionTargetHeaderTitleKey = string.Empty;
        protected string CardUnionTargetHeaderTitleKey => cardUnionTargetHeaderTitleKey;
        
        /// <summary>
        /// カードユニオンリストを初期化して表示
        /// </summary>
        public void Initialize(TrainingMainArguments mainArguments)
        {
            SetScrollData(mainArguments);
        }
        
        /// <summary>
        /// スペシャルレクチャー参加者リストを取得
        /// </summary>
        private List<long> GetSpecialLectureMembers(TrainingTrainingCardData[] trainingCardDataList, long mTrainingCardId)
        {
            List<long> resultCharaIdList = null;
            foreach (var cardData in trainingCardDataList)
            {
                if(cardData.id == mTrainingCardId)
                {
                    resultCharaIdList = cardData.mCharaIdList.ToList();
                    break;
                }
            }
            return resultCharaIdList;
        }

        // 注：修正時override先も確認すること
        protected virtual void SetScrollData(TrainingMainArguments mainArguments)
        {
            // スクロール用のデータリスト
            List<TrainingCardCharaMasterObject> sortedPracticeCardList = GetCardUnionPracticeCardList(mainArguments.Reward.concentrationUnionCard.trainingCardList, mainArguments.TrainingCardId);
            List<TrainingCardUnionInformationScrollDynamicItemSelector.ICardUnionScrollItem> scrollDataList = 
                GetCardScrollDataList(
                    mainArguments,
                    mainArguments.Reward.concentrationUnionCard,
                    sortedPracticeCardList, 
                    PracticeCardView.DisplayEnhanceUIFlags.DetailLabel);
            
            // アイテムリストの登録
            cardUnionInformationScrollDynamic.SetItems(scrollDataList);
        }
        
        // カードユニオン情報のスクロールデータを取得
        protected List<TrainingCardUnionInformationScrollDynamicItemSelector.ICardUnionScrollItem> GetCardScrollDataList(TrainingMainArguments mainArguments, TrainingUnionCardReward cardUnionData, List<TrainingCardCharaMasterObject> sortedPracticeCardList, PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags)
        {
            // スクロール用のデータリスト
            List<TrainingCardUnionInformationScrollDynamicItemSelector.ICardUnionScrollItem> scrollDataList = new List<TrainingCardUnionInformationScrollDynamicItemSelector.ICardUnionScrollItem>();

            // 選出された育成選手のカード情報をリスト化
            TrainingCardCharaMasterObject baseCard = MasterManager.Instance.trainingCardCharaMaster.values.FirstOrDefault(
                x => x.mCharaId == mainArguments.TrainingCharacter.MCharId && x.mTrainingCardId == cardUnionData.baseTrainingData.id);
            List<TrainingCardCharaMasterObject> selectedCardList = new List<TrainingCardCharaMasterObject> { baseCard };
            // ヘッダー付きのグループとしてスクロールデータに追加する
            // 一旦配列に変換
            TrainingTrainingCardData[] baseTrainingCardList = { cardUnionData.baseTrainingData };
            AddCardGroup(scrollDataList, selectedCardList, SelectedCardHeaderTitleKey, baseTrainingCardList, cardUnionData.baseTrainingData.id, displayEnhanceUIFlags);
            
            // カードユニオン対象の練習メニューカード情報をリスト化
            List<TrainingCardCharaMasterObject> cardUnionList = sortedPracticeCardList
                // カードユニオンの対象カードのみ
                .Where(card => card.mTrainingCardId != mainArguments.TrainingCardId)
                .ToList();
            // ヘッダー付きのグループとしてスクロールデータに追加する
            AddCardGroup(scrollDataList, cardUnionList, cardUnionTargetHeaderTitleKey, cardUnionData.trainingCardList, cardUnionData.baseTrainingData.id, displayEnhanceUIFlags);
            return scrollDataList;
        }
        
        protected List<TrainingCardCharaMasterObject> GetCardUnionPracticeCardList(TrainingTrainingCardData[] cardDatas, long trainingCardId)
        {
            List<TrainingCardCharaMasterObject> cardUnionCardCharacterList = new List<TrainingCardCharaMasterObject>();
           
            // カードユニオン対象カードとして、mTrainingCardIdとmCharaIdのリストを取得
            foreach (TrainingTrainingCardData mTrainingCharaCard in cardDatas)
            {
                // キャラIDとカードIDの両方で絞り込みしたものを追加
                TrainingCardCharaMasterObject card = MasterManager.Instance.trainingCardCharaMaster.values.FirstOrDefault(x => x.mCharaId == mTrainingCharaCard.mCharaId && x.mTrainingCardId == mTrainingCharaCard.id);
                if (card != null)
                {
                    cardUnionCardCharacterList.Add(card);
                }
            }
            
            // 選出された育成選手の練習メニューカードを最上位にしたリストを取得
            return GetSortOrderPracticeCardList(cardUnionCardCharacterList, trainingCardId);
        }
        
        /// <summary>
        /// 並び替えた練習カードリストを取得
        /// ソート基準(選出カードを最上位→解放レベル降順→カードID降順)
        /// </summary>
        private List<TrainingCardCharaMasterObject> GetSortOrderPracticeCardList(List<TrainingCardCharaMasterObject> practiceCardList, long mTrainingCardId)
        {
            List<TrainingCardCharaMasterObject>  sortedPracticeCardList = practiceCardList
                // 選出対象を一番上に
                .OrderByDescending(card => card.mTrainingCardId == mTrainingCardId)
                // 練習カードの解放レベルが高い順に
                .ThenByDescending(card => card.level)
                // ID順
                .ThenByDescending(card => card.mTrainingCardId)
                .ToList();

            return sortedPracticeCardList;
        }

        /// <summary>
        /// カード情報のベースの色を取得
        /// </summary>
        private Color GetBackgroundColor(long mTrainingCardId, long selectedCardId)
        {
            Color backgroundColor = (mTrainingCardId == selectedCardId)
                ? ColorValueAssetLoader.Instance[selectedColorKey]
                : ColorValueAssetLoader.Instance[cardUnionTargetColorKey];
            
            return backgroundColor;
        }
        
        /// <summary>
        /// カード情報データを生成
        /// </summary>
        private TrainingCardUnionInformationScrollItem.Argument CreateCardArgument(TrainingCardCharaMasterObject card, TrainingTrainingCardData[] trainingCardDataList, long baseTrainingCardId, PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags)
        {
            TrainingCardMasterObject mCard = MasterManager.Instance.trainingCardMaster.FindData(card.mTrainingCardId);
            List<long> specialLectureMembers = new List<long>();
               
            // 選出された練習メニューカード、カードユニオン対象のメニューカード情報に対応するベースの色を取得
            Color backgroundColor = GetBackgroundColor(card.mTrainingCardId, baseTrainingCardId);
            // 選出された練習メニューカードかどうかを判定
            bool isSelected = card.mTrainingCardId == baseTrainingCardId;
            
            // スペシャルレクチャーの判定
            if (mCard.cardGroupType == TrainingCardGroup.Flow || mCard.cardGroupType == TrainingCardGroup.Special)
            {
                // 参加キャラを取得
                specialLectureMembers = GetSpecialLectureMembers(trainingCardDataList, card.mTrainingCardId);
            }
            return new TrainingCardUnionInformationScrollItem.Argument(card.mTrainingCardId, card.id, card.mCharaId, specialLectureMembers, backgroundColor, isSelected, displayEnhanceUIFlags);
        }
        
        /// <summary>
        /// カードリストを変換し、ヘッダー付きのグループとしてスクロールデータに追加
        /// </summary>
        private void AddCardGroup(List<TrainingCardUnionInformationScrollDynamicItemSelector.ICardUnionScrollItem> scrollDataList, List<TrainingCardCharaMasterObject> sourceCardList, string headerTitleKey, TrainingTrainingCardData[] trainingCardDataList, long baseTrainingCardId, PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags)
        {
            if (sourceCardList.Count <= 0)
            {
                return;
            }
            // キーから設定する
            string headerTitle = StringValueAssetLoader.Instance[headerTitleKey];
            // ヘッダーをデータとして追加する
            scrollDataList.Add(new TrainingCardUnionInformationHeaderScrollItem.Argument(headerTitle));
            // カード情報一覧を作成
            foreach (TrainingCardCharaMasterObject card in sourceCardList)
            {
                TrainingCardUnionInformationScrollItem.Argument cardUnionArgument = CreateCardArgument(card, trainingCardDataList, baseTrainingCardId, displayEnhanceUIFlags);
                scrollDataList.Add(cardUnionArgument);
            }
        }
    }
}