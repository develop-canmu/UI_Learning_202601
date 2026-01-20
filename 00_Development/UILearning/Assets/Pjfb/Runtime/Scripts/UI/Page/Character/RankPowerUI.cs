using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class RankPowerUI : MonoBehaviour
    {
        [SerializeField] [ColorValue] private string normalColorValueKey;
        [SerializeField] [ColorValue] private string highlightColorValueKey;
        [SerializeField] private CharacterRankImage characterRankImage;
        [SerializeField] private DeckRankImage deckRankImage;
        [SerializeField] private TextMeshProUGUI powerText;
        [SerializeField] private OmissionTextSetter omissionTextSetter;

        public void InitializeCharacterVariableRankUI(CharacterVariableDetailData detailData)
        {
            var existsData = detailData is not null;
            if (!existsData)
            {
                InitializeCharacterNone();
                return;
            }
            
            characterRankImage.SetActiveImage(true);
            powerText.text = detailData.CombatPower.ToDisplayString(omissionTextSetter.GetOmissionData());
            LoadCharacterRankTexture(detailData.Rank);
        }

        public void InitializeCharacterRankUI(UserDataChara chara)
        {
            if (chara is null)
            {
                powerText.text = "0";
                return;
            }
            
            powerText.text = chara.combatPower.ToString();
            LoadCharacterRankTexture(StatusUtility.GetCharacterRank(new BigValue(chara.combatPower)));
        }

        public void InitializePartyRankUI(long rank, BigValue combatPower)
        {
            powerText.text = combatPower.ToDisplayString(omissionTextSetter.GetOmissionData());
            LoadPartyRankTexture(rank);
        }
        
        public void InitializePartyRankUI(BigValue combatPower)
        {
            powerText.text = combatPower.ToDisplayString(omissionTextSetter.GetOmissionData());
            LoadPartyRankTexture(StatusUtility.GetPartyRank(combatPower));
        }

        public void InitializePartyCombatPowerOnly(BigValue combatPower)
        {
            powerText.text = combatPower.ToDisplayString(omissionTextSetter.GetOmissionData());
        }
        
        public void SetCombatPowerTextColor(Color32 color)
        {
            powerText.color = color;
        }
        
        
        public void EnableHighlight()
        {
            powerText.color = ColorValueAssetLoader.Instance[highlightColorValueKey];
        }

        public void DisableHighlight()
        {
            powerText.color = ColorValueAssetLoader.Instance[normalColorValueKey];
        }

        private void InitializeCharacterNone()
        {
            if (characterRankImage != null)
            {
                characterRankImage.SetActiveImage(false);
            }
            
            if (powerText != null)
            {
                powerText.text = "-";
            }
        }

        private void LoadCharacterRankTexture(long rank)
        {
            if(characterRankImage == null)  return;
            characterRankImage.SetTextureAsync(rank).Forget();
        }
        
        private void LoadPartyRankTexture(long rank)
        {
            if(deckRankImage == null) return;
            deckRankImage.SetTextureAsync(rank).Forget();
        }
    }
}