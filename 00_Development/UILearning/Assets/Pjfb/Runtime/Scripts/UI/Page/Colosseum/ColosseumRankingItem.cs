using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using Pjfb.UserData;

namespace Pjfb.Colosseum
{
    public enum BorderLineType
    {
        Non = 0,
        PromotionLine = 1,
        ResidueLine = 2,
    }

    public enum ScoreType
    {
        CurrentScore = 0,
        CumulativeScore = 1,
    }
 
    public class ColosseumRankingItem : PoolListItemBase
    {
        #region Params
        public class Data : ItemParamsBase
        {
            public ColosseumSeasonData colosseumSeasonData;
            public ColosseumRankingUser userData;
            public bool disableOnClickAction;
            public BorderLineType borderLineType;
            public ScoreType scoreType = ScoreType.CurrentScore;
            public bool toggleState = false;
            public ColosseumDeck colosseumDeck;
            public System.Action OnSizeChanged;
        }
        #endregion

        protected const int TOP_RANKING_BORDER = 3;

        [SerializeField] protected GameObject deckRoot;
        [SerializeField] protected TMP_Text rankText;
        [SerializeField] protected Image rankingImage;
        [SerializeField] protected TMP_Text totalPowerText;
        [SerializeField] protected OmissionTextSetter omissionTextSetter;
        [SerializeField] protected DeckRankImage deckRankImage;
        [SerializeField] protected Sprite[] rankingImageList;
        [SerializeField] protected CharacterVariableIcon leaderIcon;
        [SerializeField] protected CharacterVariableIcon[] characterVariableIconList;
        [SerializeField] protected UIToggle deckToggle;
        [SerializeField] protected int defaultSize = 0;
        [SerializeField] protected int showDeckSize = 0; 
        [SerializeField] private GameObject promotionLineObject;
        [SerializeField] private GameObject residueLineObject;

        protected Data data;
        protected ColosseumDeck colosseumDeck;

        public override void Init(ItemParamsBase value)
        {
            data = (Data)value;
            var ranking = data.scoreType == ScoreType.CurrentScore ? data.userData.ranking : data.userData.scoreRanking;
            if (ranking < 1) ranking = 1;
            var isTopUser = ranking <= TOP_RANKING_BORDER;
            rankingImage.gameObject.SetActive(isTopUser);
            rankText.gameObject.SetActive(!isTopUser);
            colosseumDeck = data.colosseumDeck;

            deckToggle.SetIsOnWithoutNotify(data.toggleState);
            deckRoot.SetActive(data.toggleState);
            if(data.toggleState) InitializeDeckUiAsync().Forget();

            if (promotionLineObject != null) promotionLineObject.SetActive(data.borderLineType == BorderLineType.PromotionLine);
            if (residueLineObject != null) residueLineObject.SetActive(data.borderLineType == BorderLineType.ResidueLine);
            
            if (isTopUser)
            {
                rankingImage.sprite = rankingImageList[ranking - 1];
            }
            else
            {
                rankText.text = ranking.ToString();   
            }

            BigValue totalPower = new BigValue(data.userData.combatPower);
            
            totalPowerText.text = totalPower.ToDisplayString(omissionTextSetter.GetOmissionData());

            var leader = data.userData.leaderIconChara;
            var leaderRank = StatusUtility.GetCharacterRank(new BigValue(leader.combatPower));
            leaderIcon.SetIconTextureWithEffectAsync(leader.mCharaId).Forget();
            leaderIcon.SetIcon(new BigValue(leader.combatPower) ,leaderRank);
            
            deckRankImage.SetTexture(StatusUtility.GetPartyRank(totalPower));
        }
        
        protected async UniTask UpdateDeckAsync()
        {
            if (colosseumDeck == null)
            {
                var sColosseumEventId = data.colosseumSeasonData.SeasonId;
                var targetUMasterId = data.userData.uMasterId;
                var userType = data.userData.userType;
                colosseumDeck = await ColosseumManager.RequestColosseumDeckAsync(sColosseumEventId, targetUMasterId, userType);
                data.colosseumDeck = colosseumDeck;
            }   
        }

        protected async UniTask InitializeDeckUiAsync()
        {
            
            await UpdateDeckAsync();

            if (colosseumDeck.charaList.Length != characterVariableIconList.Length)
            {
                return;
            }

            for (int i = 0; i < colosseumDeck.charaList.Length; i++)
            {
                var charaData = colosseumDeck.charaList[i];
                var charaIcon = characterVariableIconList[i];
                
                charaIcon.SetIconTextureWithEffectAsync(charaData.mCharaId).Forget();
                charaIcon.SetIcon(new BigValue(charaData.combatPower), charaData.rank, (RoleNumber)charaData.roleNumber);
            }
            
            deckRoot.SetActive(true);
            
        }

        protected virtual async UniTask OnClickBannerActionAsync()
        {
            // デッキ情報をまだ取得していなければここでとる
            await UpdateDeckAsync();
            AppManager.Instance.UIManager.PageManager.OpenPage(
                PageType.TeamConfirm, 
                true, 
                new TeamConfirmPage.PageParams(
                    PageType.Colosseum, 
                    ColosseumPageType.ColosseumMatching, 
                    colosseumDeck,
                    data.userData,
                    data.colosseumSeasonData)
            );
        }
        
        #region EventListeners
        public async void OnClickDeck(bool toggleState)
        {
            if (toggleState)
            {
                await InitializeDeckUiAsync();   
            }
            else
            {
                deckRoot.SetActive(false);
            }
            data.toggleState = toggleState;
            data.OnSizeChanged?.Invoke();
        }
        
        public void OnClickBanner()
        {
            if (data.disableOnClickAction)
            {
                return;
            }
            OnClickBannerActionAsync().Forget();
        }

        public void OnClickScoreButton()
        {
            // TODO: タップで後述の「スコア内訳ポップアップ」を展開
        }
        
        public override int GetItemHeight(int prefabHeight, ItemParamsBase itemParamsBase)
        {
            var item = (Data) itemParamsBase;
            int height = (item?.toggleState == true) ? showDeckSize : defaultSize;
            if (item?.borderLineType != BorderLineType.Non) height += 80;
            return height;
        }
        #endregion
    }
   
}
