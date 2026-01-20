using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using Pjfb.Master;
using Pjfb.Extensions;

namespace Pjfb.Colosseum
{
 
    public class ColosseumRecordItem : ListItemBase
    {
        #region Params
        public class Data : ItemParamsBase
        {
            public ColosseumHistory history;
        }
        #endregion
        
        #region enum
        public enum ResultType
        {
            Win = 1,
            Lose = 2,
            Draw = 3,
        }
        #endregion

        [SerializeField] private GameObject deckRoot;
        [SerializeField] private TMP_Text beforeRankText;
        [SerializeField] private TMP_Text afterRankText;
        [SerializeField] private TMP_Text dateText;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private CharacterVariableIcon leaderIcon;
        [SerializeField] private CharacterVariableIcon[] characterVariableIconList;
        [SerializeField] private UIToggle deckToggle;
        [SerializeField] private DeckRankImage rankImage;
        [SerializeField] private TMP_Text combatPower;
        [SerializeField] private OmissionTextSetter omissionTextSetter;
        [SerializeField] private Image resultBackGroundImage;

        public Data data { get; private set; }
        private ColosseumDeck currentColosseumDeck;
        protected Color resultColor;

        public override void Init(ItemParamsBase value)
        {
            data = (Data)value;

            var dateTime = data.history.createdAt.TryConvertToDateTime();
            var timeSpan = AppTime.Now - dateTime;

            var formatKey = "";
            var timeValue = 0;
            
            if (timeSpan.Days > 0)
            {
                formatKey = "community.chat.days_ago";
                // n日前は99日を最大とする
                timeValue = Mathf.Min(timeSpan.Days,99);
            }
            else if (timeSpan.Hours > 0)
            {
                formatKey = "community.chat.hours_ago";
                timeValue = timeSpan.Hours;
            }
            else if (timeSpan.Days > 0)
            {
                formatKey = "community.chat.days_ago";
                timeValue = timeSpan.Days;
            }
            else if (timeSpan.Minutes > 0)
            {
                formatKey = "community.chat.minutes_ago";
                timeValue = timeSpan.Minutes;
            }
            else
            {
                formatKey = "community.chat.seconds_ago";
                timeValue = timeSpan.Seconds;
            }

            if (dateText != null)
            {
                dateText.text = string.Format(StringValueAssetLoader.Instance[formatKey], timeValue);
            }

            var result = (ResultType)data.history.result;
            var resultKey = "";
            switch (result)
            {
               case ResultType.Win:
                   resultKey = "pvp.history.win";
                   break;
               case ResultType.Lose:
                   resultKey = "pvp.history.lose";
                   break;
               case ResultType.Draw:
                   resultKey = "pvp.history.draw";
                   break;
            }
            
            resultColor = ColorValueAssetLoader.Instance[resultKey];

            resultText.text = StringValueAssetLoader.Instance[resultKey];
            resultBackGroundImage.color = resultColor;
            
            var beforeRank = data.history.rankBefore;
            var afterRank = data.history.rankAfter;
            var rankString = StringValueAssetLoader.Instance["pvp.record_rank.value"];
            beforeRankText.text = string.Format(rankString,beforeRank);
            afterRankText.text = string.Format(rankString,afterRank);
            currentColosseumDeck = null;
            deckRoot.SetActive(false);
            deckToggle.isOn = false;

            var leader = data.history.leaderIconChara;
            var leaderRank = StatusUtility.GetCharacterRank(new BigValue(leader.combatPower));
            leaderIcon.SetIconTextureWithEffectAsync(leader.mCharaId).Forget();
            leaderIcon.SetIcon(new BigValue(leader.combatPower), leaderRank);
        }

        private async void InitializeDeckUiAsync()
        {
            
            if (currentColosseumDeck == null)
            {
                currentColosseumDeck = await ColosseumManager.RequestGetHistoryDeckAsync(data.history.id);
            }   

            if (currentColosseumDeck.charaList.Length != characterVariableIconList.Length)
            {
                return;
            }

            BigValue totalCombatPower = BigValue.Zero;
            for (int i = 0; i < currentColosseumDeck.charaList.Length; i++)
            {
                var charaData = currentColosseumDeck.charaList[i];
                var charaIcon = characterVariableIconList[i];

                totalCombatPower += charaData.combatPower;
                charaIcon.SetIconTextureWithEffectAsync(charaData.mCharaId).Forget();
                charaIcon.SetIcon(new BigValue(charaData.combatPower), charaData.rank, (RoleNumber)charaData.roleNumber);
            }
            rankImage.SetTexture(StatusUtility.GetPartyRank(totalCombatPower));
            combatPower.text = totalCombatPower.ToDisplayString( omissionTextSetter.GetOmissionData());
            deckRoot.SetActive(true);
        }
        
        #region EventListeners
        public void OnClickDeck(bool toggleState)
        {
            if (toggleState)
            {
                InitializeDeckUiAsync();   
            }
            else
            {
                deckRoot.SetActive(false);
            }
        }
        #endregion
    }
   
}