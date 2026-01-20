
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.Master {

    
    public enum DeckSlotCardType
    {
        Variable = 0,
        Support = 1,
        SpecialSupport = 2,
        SupportEquipment = 3,
        Adviser = 11,
    }
    
    public partial class DeckFormatSlotMasterContainer : MasterContainerBase<DeckFormatSlotMasterObject> {
        long GetDefaultKey(DeckFormatSlotMasterObject masterObject){
            return masterObject.id;
        }
        
        /// <summary>フォーマットIdの検索結果</summary>
        private Dictionary<long, DeckFormatSlotMasterObject[]> formatFindCache = new Dictionary<long, DeckFormatSlotMasterObject[]>();
        /// <summary>キャラタイプの検索結果</summary>
        private Dictionary<long, Dictionary<DeckSlotCardType, DeckFormatSlotMasterObject[]>> characterTypeFindCache = new Dictionary<long, Dictionary<DeckSlotCardType, DeckFormatSlotMasterObject[]>>();
        
        /// <summary>フォーマットIdからスロットを取得</summary>
        public DeckFormatSlotMasterObject[] FindDatas(long formatId)
        {        
            // キャッシュがあれば
            if(formatFindCache.TryGetValue(formatId, out DeckFormatSlotMasterObject[] datas))
            {
                return datas;
            }
        
            List<DeckFormatSlotMasterObject> result = new List<DeckFormatSlotMasterObject>();
            foreach(DeckFormatSlotMasterObject value in values)
            {
                if(value.mDeckFormatId == formatId)
                {
                    result.Add(value);
                }
            }
            
            datas = result.ToArray();
            formatFindCache.Add(formatId, datas);
            return datas;
        }
        
        /// <summary>フォーマットIdから指定のキャラタイプのスロットを取得</summary>
        public DeckFormatSlotMasterObject[] FindDatas(long formatId, DeckSlotCardType characterType)
        {
            // キャッシュがあれば
            if(characterTypeFindCache.TryGetValue(formatId, out Dictionary<DeckSlotCardType, DeckFormatSlotMasterObject[]> datas) == false)
            {
                datas = new Dictionary<DeckSlotCardType, DeckFormatSlotMasterObject[]>();
                characterTypeFindCache.Add(formatId, datas);
            }
            
            if(datas.TryGetValue(characterType, out DeckFormatSlotMasterObject[] slots))
            {
                return slots;
            }
            
            List<DeckFormatSlotMasterObject> result = new List<DeckFormatSlotMasterObject>();
            foreach(DeckFormatSlotMasterObject value in values)
            {
                // Exは除く
                if(value.isExtraSupport)continue;
                
                if(value.mDeckFormatId == formatId && value.conditionCardType == (int)characterType)
                {
                    result.Add(value);
                }
            }
            
            DeckFormatSlotMasterObject[] resultArray = result.ToArray();
            datas.Add(characterType, resultArray);
            return resultArray;            
        }
        
        /// <summary>フォーマットIdからExサポートのスロットを取得</summary>
        public DeckFormatSlotMasterObject[] FindExtraSupportDatas(long formatId)
        {
            List<DeckFormatSlotMasterObject> result = new List<DeckFormatSlotMasterObject>();
            foreach(DeckFormatSlotMasterObject value in values)
            {
                if(value.mDeckFormatId == formatId && value.isExtraSupport)
                {
                    result.Add(value);
                }
            }
            
            return result.ToArray();
        } 
    }
}
