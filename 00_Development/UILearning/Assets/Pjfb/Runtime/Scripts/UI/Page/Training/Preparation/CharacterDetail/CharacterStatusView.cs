using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Training;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{

    public class CharacterStatusView : MonoBehaviour
    {
        
        public enum ViewCharacterType
        {
            Character, Support
        }
        
        [SerializeField]
        private ViewCharacterType viewType = ViewCharacterType.Character;
        public ViewCharacterType ViewType{get => viewType;}
        
        [SerializeField]
        private TMPro.TMP_Text nickNameText = null;
        [SerializeField]
        private TMPro.TMP_Text nameText = null;
        [SerializeField]
        private CharacterStatusValuesView statusView = null;
        
        [SerializeField]
        private CharacterRarity rarity = null;
        
        /// <summary>ステータス</summary>
        public  CharacterStatusValuesView StatusView{get{return statusView;}}

        [SerializeField]
        private ScrollGrid menuCardScrollGrid = null;
        
        [SerializeField]
        private CharacterCardImage standImage = null;
        [SerializeField]
        private SpecialSupportCardIcon specialSupportCardIcon = null;
        
        [SerializeField]
        private CharacterIcon icon = null;
        
        [SerializeField]
        private TMP_Text lvText = null;
        
        // uCharId
        private long characterId = -1;
        private long characterLv = 0;
        private long liberationLv = 0;
        // 選択中のシナリオId
        private long scenarioId = 0;
        
        private CardType cardType = CardType.None;
        
        /// <summary>キャラクタを表示</summary>
        public void SetUserCharacter(long uCharId, long scenarioId = -1)
        {
            // ユーザーデータ
            UserDataChara uChar = UserDataManager.Instance.chara.data[uCharId];
            SetCharacter(uChar.charaId, uChar.level, uChar.newLiberationLevel, scenarioId);
        }
        
        /// <summary>キャラクタを表示</summary>
        public void SetCharacter(long mCharId, long lv, long liberationLv, long scenarioId = -1)
        {
            // キャラマスタ
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(mCharId);            
            // ユーザーキャラIdを保持
            characterId = mCharId;
            characterLv = lv;
            this.liberationLv = liberationLv;
            this.scenarioId = scenarioId;
            
            cardType = MasterManager.Instance.charaMaster.FindData(characterId).cardType;
            
            // レア度
            if(rarity != null)
            {
                if(cardType == CardType.SpecialSupportCharacter)
                {
                    rarity.gameObject.SetActive(false);
                }
                else
                {
                    rarity.gameObject.SetActive(true);
                    // 基本レアリティ
                    long rarityId = MasterManager.Instance.charaMaster.FindData(mCharId).mRarityId;
                    long baseRarity = MasterManager.Instance.rarityMaster.FindData(rarityId).value;
                    // 現在レアリティ
                    rarityId = RarityUtility.GetRarityId(mCharId, liberationLv);
                    long currentRarity = MasterManager.Instance.rarityMaster.FindData(rarityId).value;
                    
                    rarity.SetRarity(currentRarity, baseRarity);
                }
            }
            
            // 立ち絵
            if(standImage != null)
            {
                if(string.IsNullOrEmpty(CharacterUtility.CharIdToStandingImageId(mCharId)))
                {
                    standImage.gameObject.SetActive(false);
                }
                else
                {
                    standImage.gameObject.SetActive(true);
                    standImage.SetTexture(mCharId);
                }
            }
            
            // カード絵
            if(specialSupportCardIcon != null)
            {
                if(cardType == CardType.SpecialSupportCharacter && string.IsNullOrEmpty(CharacterUtility.CharIdToStandingImageId(mCharId)))
                {
                    specialSupportCardIcon.gameObject.SetActive(true);
                    specialSupportCardIcon.SetIconTextureWithEffectAsync(mCharId).Forget();
                }
                else
                {
                    specialSupportCardIcon.gameObject.SetActive(false);
                }
            }
            
            // アイコン
            if(icon != null) 
            {
                icon.SetIcon(mCharId, lv, liberationLv);
            }
            
            // キャラ名
            nickNameText.text = mChar.nickname;
            nameText.text = mChar.name;
            
            if(lvText != null)
            {
                lvText.text = string.Format( StringValueAssetLoader.Instance["character.status.lv_current_max"], lv, CharacterUtility.GetMaxGrowthLevel(mCharId, lv, liberationLv) );
            }
            
            // ステータスの表示
            statusView.SetCharacter(characterId, characterLv, liberationLv);
            
            if(menuCardScrollGrid != null)
            {
                // キャラレベルに応じた表示用カードリストを取得
                List<TrainingCardCharaMasterObject> displayCards = MasterManager.Instance.trainingCardCharaMaster.GetDisplayCardListForCharaLevel(mCharId, lv);
    
                PracticeCardScrollItem.Arguments[] cardArgsArray = new PracticeCardScrollItem.Arguments[displayCards.Count];
                
                // 各カードを Arguments に変換
                for (int i = 0; i < displayCards.Count; i++)
                {
                    TrainingCardCharaMasterObject card = displayCards[i];
                    cardArgsArray[i] = new PracticeCardScrollItem.Arguments(
                        card.id,
                        card.mCharaId,
                        card.mTrainingCardId,
                        lv,
                        PracticeCardView.DisplayEnhanceUIFlags.Label | PracticeCardView.DisplayEnhanceUIFlags.NextLevel |
                        PracticeCardView.DisplayEnhanceUIFlags.DetailLabel | PracticeCardView.DisplayEnhanceUIFlags.DetailEnhanceUI);
                }

                // 表示
                menuCardScrollGrid.SetItems(cardArgsArray);
            }
        }
        
        /*
        /// <summary>UGUI</summary>
        public void OnDetailButton()
        {
            // キャラクタ詳細を開く
            switch(viewType)
            {
                case ViewCharacterType.Character:
                {
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CharacterDetail, new CharacterDetailModal.Arguments(scenarioId, characterId, characterLv, liberationLv) );
                    break;
                }
            
                case ViewCharacterType.Support:
                {
                    switch(cardType)
                    {
                        case CardType.Character:
                        {
                            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SupportCharacterDetail, new SupportCharacterDetailModal.Arguments(scenarioId, characterId, characterLv, liberationLv) );
                            break;
                        }
                        case CardType.SpecialSupportCharacter:
                        {
                            CharacterDetailModalBase.Open(ModalType.SpecialSupportCardDetail,
                                new BaseCharaDetailModalParams(
                                    new List<CharacterDetailData> { new(characterId, characterLv, liberationLv) }, 0, null));
                            break;
                        }
                    }
                    
                    break;
                }
            }
        }*/
    }
}
