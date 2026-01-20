using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using Pjfb.UserData;

namespace Pjfb.Character
{
    public class CharacterGrowthConfirmModal : CharacterLevelUpConfirmModalBase<CharacterGrowthConfirmModal.GrowthData>
    {
        private static readonly string LvStringValueKey = "character.status.lv_value";
        
        public class GrowthData : Data
        {
            public readonly List<PracticeSkillInfo> PracticeSkillDataList;
            public readonly GrowthCostData GrowthCostData;

            public GrowthData(long userCharacterId, long currentLv, long afterLv , List<PracticeSkillInfo> practiceSkillDataList, GrowthCostData growthCostData) 
                : base(userCharacterId,currentLv, afterLv)
            {
                PracticeSkillDataList = practiceSkillDataList;
                GrowthCostData = growthCostData;
            }
        }

        
        [Header("練習スキル表示領域")]
        [SerializeField] private Transform practiceSkillRoot;
        [SerializeField] private GameObject noPracticeSkillText;
        [SerializeField] private PracticeSkillTypeViewGridItem practiceSkillTypeViewGridItem;

        [Header("必要アイテム表示領域")]
        [SerializeField] private Transform itemIconRoot;
        [SerializeField] private ItemIconGridItem itemIconGridItem;
        //[SerializeField] private ItemIcon itemIcon;

        [Header("獲得練習カード表示領域")]
        [SerializeField] private Transform practiceCardRoot;
        [SerializeField] private GameObject noPracticeCardText;
        [SerializeField] private PracticeCardView practiceCardView;
        
        [Header("強化練習カード表示領域")]
        [SerializeField] private Transform enhancePracticeCardRoot;
        [SerializeField] private GameObject enhancePracticeCardRootObject;
        [SerializeField] private EnhancePracticeCardView enhancePracticeCardView;

        [Header("成長率表示領域")]
        [SerializeField] private CharacterGrowthRateGroupView grouthRateGroupView;
        [SerializeField] private GameObject growthRateRoot;
        [SerializeField][ColorValue] private string statusColorId = string.Empty;
        [SerializeField][ColorValue] private string statusHighlightColorId = string.Empty;


        protected override void InitializeUi()
        {
            currentLvText.text = string.Format(StringValueAssetLoader.Instance[LvStringValueKey], modalData.CurrentLv);
            afterLvText.text = string.Format(StringValueAssetLoader.Instance[LvStringValueKey], modalData.AfterLv);
            var itemIconGridItemDataList = new List<ItemIconGridItem.Data>();

            foreach (var costData in modalData.GrowthCostData.SumCost)
            {
                long mPointId = costData.Key;
                long requiredCount = costData.Value;
                long possessionValue = UserDataManager.Instance.point.Find(mPointId)?.value ?? 0;
                itemIconGridItemDataList.Add(new ItemIconGridItem.Data(mPointId, requiredCount, possessionValue, true));
            }

            SetItems(itemIconGridItemDataList, itemIconRoot);
            noPracticeSkillText.SetActive(modalData.PracticeSkillDataList.Count <= 0);
            SetSkillItems(modalData.PracticeSkillDataList, practiceSkillRoot);
            
            // 獲得練習カードを取得
            long mCharId = CharacterUtility.UserCharIdToMCharId(modalData.UserCharacterId);
            
            // モーダルタイトルのセット
            CardType cardType = MasterManager.Instance.charaMaster.FindData(mCharId).cardType;
            modalTitle.text = StringValueAssetLoader.Instance[$"character.card_type_{(int)cardType}.growth.confirm.title"];
            
            // 該当キャラの全カードを取得（カード概念ごとにグループ化）
            List<List<TrainingCardCharaMasterObject>> groupedCardList = MasterManager.Instance.trainingCardCharaMaster.GetGroupedPracticeCardOrderByLevel(mCharId);
            
            List<PracticeCardView.Data> newCardDataList = new();
            
            // 各グループごとに「新規獲得」を判定
            foreach (List<TrainingCardCharaMasterObject> group in groupedCardList)
            {
                // グループ内で一番レベルが低いカードを取得
                TrainingCardCharaMasterObject baseCard = group[0];
                // レベルアップで解放されたか
                if (baseCard.level > modalData.CurrentLv && baseCard.level <= modalData.AfterLv)
                {
                    // 新規カードとして追加
                    newCardDataList.Add(
                        new PracticeCardView.Data(
                            baseCard.mTrainingCardId,
                            baseCard.id,
                            mCharId,
                            modalData.AfterLv,
                            PracticeCardView.DisplayEnhanceUIFlags.Label | PracticeCardView.DisplayEnhanceUIFlags.DetailLabel | PracticeCardView.DisplayEnhanceUIFlags.DetailEnhanceUI,
                            modalData.CurrentLv));
                }
            }

            SetNewPracticeCards(newCardDataList.ToArray(), practiceCardRoot);
            noPracticeCardText.SetActive(newCardDataList.Count <= 0);
            
            EnhancePracticeCardView.Data[] enhanceCardArgs = GetEnhanceCardArguments(mCharId, modalData.CurrentLv, modalData.AfterLv);
            SetEnhancePracticeCards(enhanceCardArgs, enhancePracticeCardRoot);
            enhancePracticeCardRootObject.SetActive(enhanceCardArgs.Length > 0);

            CharaMasterObject charaData = MasterManager.Instance.charaMaster.FindData(mCharId);

            switch (charaData.cardType)
            {
                case CardType.Character:
                    growthRateRoot.SetActive(true);
                    grouthRateGroupView.SetGrowthResultPreview(mCharId, modalData.CurrentLv, modalData.AfterLv, statusColorId, statusHighlightColorId);
                    break;
                case CardType.SpecialSupportCharacter:
                case CardType.Adviser:
                    growthRateRoot.SetActive(false);
                    break;
            }
        }
        
      
        protected override async UniTask CallApi()
        {
            CharaGrowthAPIRequest request = new CharaGrowthAPIRequest();
            CharaGrowthAPIPost post = new CharaGrowthAPIPost();
            post.uCharaId = modalData.UserCharacterId;
            post.level = modalData.AfterLv;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            SetCloseParameter(true);
            Close();
        }

        private void SetSkillItems(List<PracticeSkillInfo> skillList, Transform root)
        {
            if (skillList.Count < 1)
            {
                return;
            }

            foreach (var item in skillList)
            {
                PracticeSkillTypeViewGridItem gridItem = GameObject.Instantiate(practiceSkillTypeViewGridItem, root);
                gridItem.gameObject.SetActive(true);
                gridItem.SetSkillData(item);
            }
        }
        
        /// <summary>
        /// レベルアップ前後で強化されるカードの引数配列を取得
        /// </summary>
        /// <param name="mCharId">キャラクターID</param>
        /// <param name="beforeLv">レベルアップ前の選手レベル</param>
        /// <param name="afterLv">レベルアップ後の選手レベル</param>
        private EnhancePracticeCardView.Data[] GetEnhanceCardArguments(long mCharId, long beforeLv, long afterLv)
        {
            // 該当キャラの全カードを取得（カード概念ごとにグループ化）
            List<List<TrainingCardCharaMasterObject>> groupedCardList = MasterManager.Instance.trainingCardCharaMaster.GetGroupedPracticeCardOrderByLevel(mCharId);

            List<EnhancePracticeCardView.Data> resultList = new();

            // 各グループごとに強化を判定
            foreach (List<TrainingCardCharaMasterObject> cardCharaMasters in groupedCardList)
            {
                TrainingCardCharaMasterObject beforeEnhanceCard = null;
                TrainingCardCharaMasterObject afterEnhanceCard = null;
                
                foreach (TrainingCardCharaMasterObject cardCharaMaster in cardCharaMasters)
                {
                    // 現在のレベル以下で一番強化レベルが高いカードを探す
                    if (cardCharaMaster.level <= beforeLv)
                    {
                        if (beforeEnhanceCard == null || cardCharaMaster.level > beforeEnhanceCard.level)
                        {
                            beforeEnhanceCard = cardCharaMaster;
                        }
                    }
                    
                    // 強化後のレベル以下で一番強化レベルが高いカードを探す
                    if (cardCharaMaster.level <= afterLv)
                    {
                        if (afterEnhanceCard == null || cardCharaMaster.level > afterEnhanceCard.level)
                        {
                            afterEnhanceCard = cardCharaMaster;
                        }
                    }
                }

                // 今回解放され、かつ即座に強化される場合
                if (beforeEnhanceCard == null && cardCharaMasters.Count > 0)
                {
                    // グループ内で一番レベルが低いカードを取得
                    TrainingCardCharaMasterObject baseCard = cardCharaMasters[0];
                    if (baseCard.level <= afterLv)
                    {
                        beforeEnhanceCard = baseCard;
                    }
                }
                
                // 強化があった場合のみ追加
                if (beforeEnhanceCard != null && afterEnhanceCard != null)
                {
                    if (beforeEnhanceCard.displayEnhanceLevel < afterEnhanceCard.displayEnhanceLevel)
                    {
                        resultList.Add(new EnhancePracticeCardView.Data(
                            beforeEnhanceCard.id,
                            afterEnhanceCard.id,
                            mCharId,
                            afterLv,
                            modalData.CurrentLv
                        ));
                    }
                }
            }
            
            return resultList.ToArray();
        }

        /// <summary>
        /// 新規獲得する練習カードを生成して表示
        /// </summary>
        private void SetNewPracticeCards(PracticeCardView.Data[] dataList, Transform root)
        {
            if (dataList.Length < 1)
            {
                return;
            }

            foreach (PracticeCardView.Data data in dataList)
            {
                PracticeCardView view = GameObject.Instantiate(practiceCardView, root);
                view.gameObject.SetActive(true);
                view.SetData(data);
            }
        }
        
        /// <summary>
        /// 強化される練習カードを生成して表示
        /// </summary>
        private void SetEnhancePracticeCards(EnhancePracticeCardView.Data[] args, Transform root)
        {
            if (args.Length < 1)
            {
                return;
            }

            foreach (EnhancePracticeCardView.Data data in args)
            {
                EnhancePracticeCardView view = GameObject.Instantiate(enhancePracticeCardView, root);
                view.gameObject.SetActive(true);
                view.SetData(data);
            }
        }

        private void SetItems(List<ItemIconGridItem.Data> list, Transform root)
        {
            if (list.Count < 1)
            {
                return;
            }

            foreach (var item in list)
            {
                ItemIconGridItem gridItem = GameObject.Instantiate(itemIconGridItem, root);
                gridItem.ItemIconGridItemData = item;
                gridItem.gameObject.SetActive(true);
                gridItem.SetItemData(item);
            }
        }
    }
}