using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Master;
using TMPro;
using UnityEngine;

namespace Pjfb
{ 
    public class TrainerCardDisplayCharacterListScrollData
    {
        /// <summary>mCharaParent.id</summary>
        public long CharaParentId { get; }
        /// <summary>新規所持があるか</summary>
        public bool IsNew { get; set; }
        /// <summary>所持数</summary>
        public int HaveCount { get; }
        /// <summary>所持可能数</summary>
        public int MaxCount { get; }
        
        public TrainerCardDisplayCharacterListScrollData(long charaParentId, bool isNew, int haveCount, int maxCount)
        {
            CharaParentId = charaParentId;
            IsNew = isNew;
            HaveCount = haveCount;
            MaxCount = maxCount;
        }
    }
    
    public class TrainerCardDisplayCharacterListScrollItem : ScrollGridItem
    {
        [SerializeField]
        private CharacterParentIcon characterParentIcon;
        [SerializeField]
        private TextMeshProUGUI characterParentNameText;
        [SerializeField]
        private TextMeshProUGUI characterParentEnglishNameText;
        [SerializeField]
        private TextMeshProUGUI possessionCountText;
        [SerializeField]
        private GameObject cover;
        [SerializeField]
        private UIBadgeNotification newBadge;
        
        private TrainerCardDisplayCharacterListScrollData scrollData;
        
        protected override void OnSetView(object value)
        {
            scrollData = (TrainerCardDisplayCharacterListScrollData)value;
            CharaParentMasterObject parentMaster = MasterManager.Instance.charaParentMaster.FindData(scrollData.CharaParentId);
            
            // アイコン設定
            characterParentIcon.SetIconId(parentMaster.parentMCharaId);
            CharaParentMasterObject master = MasterManager.Instance.charaParentMaster.FindData(scrollData.CharaParentId);
            
            // 名前設定
            characterParentNameText.text = master.name;
            characterParentEnglishNameText.text = master.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.EnglishName, string.Empty);
            
            // 所持数と総数設定
            possessionCountText.text = string.Format(StringValueAssetLoader.Instance["common.ratio_value"], scrollData.HaveCount, scrollData.MaxCount);
            
            // 総数が0の場合はカバーを表示
            cover.SetActive(scrollData.HaveCount == 0);
            
            // 新規バッジの表示
            newBadge.SetActive(scrollData.IsNew);
        }

        /// <summary>UGUI</summary>
        public void OnClick()
        {
            TriggerEvent(scrollData.CharaParentId);
        }
    }
}