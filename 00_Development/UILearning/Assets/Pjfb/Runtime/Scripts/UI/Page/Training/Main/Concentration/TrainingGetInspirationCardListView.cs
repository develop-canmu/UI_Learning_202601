using System.Collections.Generic;
using System.Linq;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Training
{
    /// <summary>
    /// インスピレーションが付与された練習カード一覧表示用View
    /// </summary>
    public class TrainingGetInspirationCardListView : MonoBehaviour
    {
        [SerializeField] 
        private TrainingGetInspirationCardView cardViewPrefab = null;
        [SerializeField] 
        private Transform cardRoot = null;

        private List<TrainingGetInspirationCardView> cardItemList = new List<TrainingGetInspirationCardView>();
        
        /// <summary>
        /// インスピレーションが付与された練習カードリストを初期化して表示
        /// </summary>
        public void Initialize(TrainingMainArguments mainArguments, int selectCardIndex)
        {
            // 生成済みのものを削除
            foreach (TrainingGetInspirationCardView cardItem in cardItemList)
            {
                GameObject.Destroy(cardItem.gameObject);
            }

            cardItemList.Clear();

            // インスピレーションリスト
            TrainingInspire[] orderdList = mainArguments.Pending.inspireList.
                // グレード順
                OrderByDescending(v =>
                {
                    TrainingCardInspireMasterObject mCard =
                        MasterManager.Instance.trainingCardInspireMaster.FindData(v.id);
                    return mCard.grade;
                }).
                // 優先度順
                ThenByDescending(v =>
                {
                    TrainingCardInspireMasterObject mCard =
                        MasterManager.Instance.trainingCardInspireMaster.FindData(v.id);
                    return mCard.priority;
                }).
                // Id順
                ThenBy(v => { return v.id; }).ToArray();



            // カードリストを取得
            TrainingInspirationCardList[] cardList = TrainingUtility.GetGetInspirationList(orderdList);

            // ソート
            cardList = cardList.
                // 選択中のカード（cardIdとmCharaIdが一致）
                OrderByDescending((v) => selectCardIndex >= 0 &&
                                         v.MTrainingCardId == mainArguments.Pending
                                             .handList[selectCardIndex].mTrainingCardId &&
                                         v.MCharId == mainArguments.Pending
                                             .handList[selectCardIndex].mCharaId).
                // 手札にある
                ThenByDescending((v) => mainArguments.HasTrainingCard(v.MTrainingCardId)).
                // グレード
                ThenByDescending((v) => MasterManager.Instance.trainingCardMaster.FindData(v.MTrainingCardId).grade).
                // カード種別
                ThenBy((v) => MasterManager.Instance.trainingCardMaster.FindData(v.MTrainingCardId).practiceType).
                // カード種別
                ThenBy((v) => v.MTrainingCardId).
                // 配列化
                ToArray();

            for (int i = 0; i < cardList.Length; i++)
            {

                TrainingInspirationCardList card = cardList[i];
                // マスタ
                TrainingCardMasterObject mCard =
                    MasterManager.Instance.trainingCardMaster.FindData(card.MTrainingCardId);
                // 手札に所持
                bool hasHand = mainArguments.HasTrainingCard(card.MTrainingCardId);

                // 生成
                TrainingGetInspirationCardView view =
                    GameObject.Instantiate<TrainingGetInspirationCardView>(cardViewPrefab, cardRoot);

                // 選択（cardIdとmCharaIdが一致）
                bool isSelected = selectCardIndex >= 0 &&
                                  selectCardIndex ==
                                  mainArguments.GetTrainingCardHandIndex(card.MTrainingCardId) &&
                                  card.MCharId == mainArguments.Pending.handList[selectCardIndex]
                                      .mCharaId;

                // リストに追加
                cardItemList.Add(view);
                // 表示設定
                view.SetData(card);

                // 背景の色
                view.SetBackgroundColor(isSelected
                    ? ColorValueAssetLoader.Instance["training.practice_skill.selected"]
                    : Color.white);
                // 選択中
                view.SetSelectedBadge(isSelected);
                // 発生中
                view.SetOccurrenceBadge(isSelected == false && hasHand);
                // 発生率アップ
                bool isProbabilityUp = false;
                // カードが所持しているインスピレーションをチェック
                foreach (TrainingInspirationCardList.InspirationData inspiration in card.Inspirations)
                {
                    TrainingCardInspireMasterObject mInspiration =
                        MasterManager.Instance.trainingCardInspireMaster.FindData(inspiration.Id);
                    // 指定のデータがある場合は表示
                    if (mInspiration.rarePracticeEnhanceRate > 0)
                    {
                        // 発生率アップ
                        isProbabilityUp = true;
                        break;
                    }
                }

                // 発生率アップの表示切り替え
                view.SetProbabilityUpBadge(isProbabilityUp);
                
                // アクティブ
                view.gameObject.SetActive(true);
            }
        }
    }
}

