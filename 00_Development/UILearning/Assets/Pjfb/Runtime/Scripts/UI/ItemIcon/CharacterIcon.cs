using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Training;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pjfb
{
    public enum BaseCharacterType
    {
        Character,
        SpecialSupportCard,
        TrainingCharacter,
        SupportCharacter,
        SupportEquipment,
        Adviser,
    }
    
    public class CharacterIcon : ItemIconBase
    {
        public override ItemIconType IconType { get { return ItemIconType.Character; } }
        
        [SerializeField]
        private BaseCharacterType baseCharacterType = BaseCharacterType.Character;
        public BaseCharacterType BaseCharaType{get => baseCharacterType; set => baseCharacterType = value; }
        
        public UserDataChara UChara => uChara;
        
        [SerializeField]
        private bool openDetailModal = true;
        /// <summary>詳細を開ける</summary>
        public bool OpenDetailModal{get{return openDetailModal;}set{openDetailModal = value;}}
        
        [SerializeField]
        private bool canOpenCharacterEncyclopediaPage = true;
        
        
        [SerializeField]
        private bool canLiberation = false;
        /// <summary>解放可能</summary>
        public bool CanLiberation 
        { 
            get { return canLiberation; }
            set { canLiberation = value; }
        }
        
        [SerializeField]
        private bool canGrowth = false;
        /// <summary>強化可能</summary>
        public bool CanGrowth 
        { 
            get { return canGrowth; }
            set { canGrowth = value; }
        }

        [SerializeField]
        private TMPro.TMP_Text lvText = null;
        
        [SerializeField]
        private CharacterRarity rarity = null;
        
        [SerializeField]
        private RarityImage supportCharacterRarityImage = null;
        
        [SerializeField]
        private ImageEffect iconImageEffect = null;
        
        [SerializeField]
        private CharacterCharacterTypeImage characterTypeIcon = null;
        
        [SerializeField]
        private GameObject rarityRoot = null;
        
        [SerializeField]
        private GameObject lvRoot = null;
        
        [SerializeField]
        private GameObject extraRoot = null;
        
        [SerializeField] private GameObject countRoot;
        [SerializeField] private TextMeshProUGUI countText;
        
        
        private CharacterDetailData iconData = null;
        
        private CharaMasterObject mChara = null;
        private UserDataChara uChara = null;
        
        private long[] statusIdList = null;
        
        /// <summary>アイコンセット</summary>
        public async UniTask SetIconAsync(CharacterDetailData data)
        {
            iconData = data;
            
            // uChara
            uChara = UserDataManager.Instance.chara.data.Values.FirstOrDefault(d => d.charaId == data.MCharaId);
            
            // mChar
            mChara = MasterManager.Instance.charaMaster.FindData(data.MCharaId);
            
            List<UniTask> taskList = new List<UniTask>();

            // 基本レアリティ
            long rarityId = mChara.mRarityId;
            if(mChara.cardType == CardType.Character || mChara.cardType == CardType.Adviser)
            {
                long baseRarity = MasterManager.Instance.rarityMaster.FindData(rarityId).value;
                // 現在レアリティ
                rarityId = RarityUtility.GetRarityId(data.MCharaId, data.LiberationLevel);
                long currentRarity = MasterManager.Instance.rarityMaster.FindData(rarityId).value;
                // レアリティセット
                SetRarity(currentRarity, baseRarity);
            }
            // 星表示以外
            else if(mChara.cardType != CardType.None)
            {
                long rarity = MasterManager.Instance.rarityMaster.FindData(rarityId).value;
                taskList.Add(SetRarityAsync(rarity));
            }
            
            // レベルセット
            SetLv(data.Lv);
            // キャラタイプ
            taskList.Add(SetCharacterTypeIconAsync(mChara.charaType));
            // エフェクト
            taskList.Add(SetEffectAsync(data.MCharaId));
            // アイコン
            taskList.Add(SetIconIdAsync(data.MCharaId));
            
            // サポート器具の場合はレベル非表示
            if(mChara.cardType == CardType.SupportEquipment)
            {
                SetActiveLv(false);
            }
            
            // ex
            extraRoot.SetActive(mChara.isExtraSupport);
            
            // 待機
            await UniTask.WhenAll(taskList); 
        }
        
        public void SetStatusIdList(long[] statusIdList)
        {
            this.statusIdList = statusIdList;
        }
        
        /// <summary>アイコンセット</summary>
        public UniTask SetIconAsync(long mCharaId, long lv, long liberationLevel, bool fromPrizeJson = false)
        {
            return SetIconAsync(new CharacterDetailData(mCharaId, lv, liberationLevel, fromPrizeJson));
        }
        
        /// <summary>アイコンセット</summary>
        public void SetIcon(CharacterDetailData data)
        {
            SetIconAsync(data).Forget();
        }
        
        /// <summary>アイコンセット</summary>
        public void SetIcon(long mCharaId, long lv, long liberationLevel, bool fromPrizeJson = false)
        {
            SetIconAsync(mCharaId, lv, liberationLevel, fromPrizeJson).Forget();
        }
        
        /// <summary>アイコンセット</summary>
        public void SetIcon(long mCharaId)
        {
            SetIconAsync(mCharaId, 1, 0).Forget();
        }

        /// <summary>エフェクト付きでアイコンセット</summary>
        public async UniTask SetIconTextureWithEffectAsync(long mCharaId)
        {
            await UniTask.WhenAll(
                SetIconIdAsync(mCharaId),
                SetEffectAsync(mCharaId)
            );
        }
        
        /// <summary>Lvの表示</summary>
        private void SetLv(long lv)
        {
            SetActiveLv(true);
            lvText.text = string.Format(StringValueAssetLoader.Instance["character.status.lv_value"], lv);
        }
        
        public void SetImage(long mCharId)
        {
            SetTexture(mCharId);
            // ex
            extraRoot.SetActive( CharacterUtility.IsExtraCharacter(mCharId));
        }
        
        /// <summary>レアリティの表示</summary>
        private async UniTask SetRarityAsync(long value)
        {
            SetActiveRarity(true);
            rarity?.gameObject.SetActive(false);
            
            if(supportCharacterRarityImage != null)
            {
                supportCharacterRarityImage.gameObject.SetActive(true);
                await supportCharacterRarityImage.SetTextureAsync(value);
            }
        }
        
        private async UniTask SetEffectAsync(long mCharaId)
        {
            // エフェクト
            if (iconImageEffect != null)
            {
                long effectId = MasterManager.Instance.charaMaster.FindData(mCharaId)?.imageEffectId ?? 0;
                
                // エフェクトを持っているか
                bool hasEffect = effectId != 0;
                iconImageEffect.gameObject.SetActive(hasEffect);
                
                if (hasEffect)
                {
                    await iconImageEffect.LoadEffect(effectId);
                }
            }
        }

        /// <summary>レアリティの表示</summary>
        private void SetRarity(long currentRarity, long baseRarity)
        {
            SetActiveRarity(true);
            
            supportCharacterRarityImage?.gameObject.SetActive(false);
            if(rarity != null)
            {
                rarity.gameObject.SetActive(true);
                rarity.SetRarity(currentRarity, baseRarity);
            }
        }
        
        /// <summary>タイプアイコンの表示</summary>
        private UniTask SetCharacterTypeIconAsync(CharacterType characterType)
        {
            SetActiveCharacterTypeIcon(true);
            return characterTypeIcon.SetTextureAsync(characterType);
        }
        
        /// <summary>タイプアイコンのアクティブ</summary>
        public void SetActiveCharacterTypeIcon(bool value)
        {
            characterTypeIcon.gameObject.SetActive(value);
        }
        
        /// <summary>レアリティのアクティブ</summary>
        public void SetActiveRarity(bool value)
        {
            rarityRoot.SetActive(value);
        }
        
        /// <summary>Lvのアクティブ</summary>
        public void SetActiveLv(bool value)
        {
            lvRoot.SetActive(value);
            if (countRoot != null && value) countRoot.SetActive(false);
        }
        
        private string GetTitleStringKey()
        {
            switch(baseCharacterType)
            {
                case BaseCharacterType.Character:
                    return "character.base_chara_detail.title";
                case BaseCharacterType.SpecialSupportCard:
                    return "character.detail_modal.special_support_info";
                case BaseCharacterType.TrainingCharacter:
                    return "training.character_info";
                case BaseCharacterType.SupportCharacter:
                    return "character.detail_modal.support_character_info";
                case BaseCharacterType.SupportEquipment:
                    return "common.support_equipment";
                case BaseCharacterType.Adviser:
                    return "character.detail_modal.adviser.title";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Func<long> GetTrainingScenarioId;
        public SwipeableParams<CharacterDetailData> SwipeableParams = new();
        public void OnLongTap()
        {
            if(!openDetailModal) return;
            bool clearSwipeableParams = false;
            SwipeableParams ??= new SwipeableParams<CharacterDetailData>();
            if (SwipeableParams?.DetailOrderList == null || SwipeableParams.DetailOrderList.Count == 0 || SwipeableParams.StartIndex == -1)
            {
                if(iconData == null)    return;
                SwipeableParams.StartIndex = 0;
                SwipeableParams.DetailOrderList = new List<CharacterDetailData>() { iconData };
                clearSwipeableParams = true;
            }

            long trainingScenarioId = GetTrainingScenarioId?.Invoke() ?? -1;
            
            if(mChara.cardType == CardType.Character)
            {
                BaseCharaDetailModalParams modalParam = new BaseCharaDetailModalParams(SwipeableParams, canOpenCharacterEncyclopediaPage, canLiberation, GetTitleStringKey(), trainingScenarioId, canGrowth);
                CharacterDetailModalBase.Open(ModalType.BaseCharacterDetail, modalParam);
            }
            else if(mChara.cardType == CardType.SpecialSupportCharacter)
            {
                CharacterDetailModalBase.Open(ModalType.SpecialSupportCardDetail,
                    new BaseCharaDetailModalParams(SwipeableParams, false, false, GetTitleStringKey(), trainingScenarioId, canGrowth));
            }
            else if(mChara.cardType == CardType.SupportEquipment)
            {
                SupportEquipmentDetailData detailData = new SupportEquipmentDetailData( iconData.UCharId, iconData.MCharaId, iconData.Lv, statusIdList);
                SwipeableParams<SupportEquipmentDetailData> swipeableParams = new SwipeableParams<SupportEquipmentDetailData>();
                swipeableParams.DetailOrderList = new List<SupportEquipmentDetailData>() { detailData };
                SupportEquipmentDetailModalParams modalParam = new SupportEquipmentDetailModalParams(swipeableParams, false, false, iconData.FromPrizeJson, null);
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SupportEquipmentDetail, modalParam);
            }
            else if(mChara.cardType == CardType.Adviser)
            {
                BaseCharaDetailModalParams modalParam = new BaseCharaDetailModalParams(SwipeableParams, canOpenCharacterEncyclopediaPage, canLiberation, GetTitleStringKey(), trainingScenarioId, canGrowth);
                CharacterDetailModalBase.Open(ModalType.AdviserDetail, modalParam);
            }
            
            if (clearSwipeableParams) SwipeableParams = null;
        }
        
        public void SetCount(long value)
        {
            SetActiveCount(true);
            countText.text = string.Format(StringValueAssetLoader.Instance["item.count_1"], value); 
        }
        
        /// <summary>Countのアクティブ</summary>
        public void SetActiveCount(bool value)
        {
            if (countRoot != null) countRoot.SetActive(value);
            if (value) lvRoot.SetActive(false);
        }
        
        public void SetCountTextColor(Color color)
        {
            countText.color = color;
        }

        public void EmptyIcon()
        {
            SetActiveImage(false);
            SetActiveLv(false);
            SetActiveRarity(false);
            SetActiveCharacterTypeIcon(false);
            SetActiveCount(false);
        }
    }
}