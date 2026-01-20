
using System.Collections.Generic;

namespace Pjfb.Master {

    public enum CharaRankMasterStatusType
    {
        Status         = 1,
        CharacterTotal = 2,
        PartyTotal     = 3,
        
    }
    
    public partial class CharaRankMasterContainer : MasterContainerBase<CharaRankMasterObject> {
        long GetDefaultKey(CharaRankMasterObject masterObject){
            return masterObject.id;
        }
        
        // タイプごとにキャッシュしておく
        private Dictionary<CharaRankMasterStatusType, CharaRankMasterObject[]> cacheData = new Dictionary<CharaRankMasterStatusType, CharaRankMasterObject[]>();

        public CharaRankMasterObject FindDataByTypeAndPower(CharaRankMasterStatusType type, BigValue totalCombatPower)
        {
            var cache = FindDatas(type);
            for (var i = cache.Length - 1; i >= 0; i--) if (totalCombatPower >= new BigValue(cache[i].minValue)) return cache[i];
            return default;
        }
        
        public CharaRankMasterObject[] FindDatas(CharaRankMasterStatusType type)
        {
            // キャッシュがある場合はキャッシュから返す
            if(cacheData.TryGetValue(type, out CharaRankMasterObject[] cache))
            {
                return cache;
            }
            
            // データを収集
            List<CharaRankMasterObject> list = new List<CharaRankMasterObject>();
            foreach(CharaRankMasterObject value in values)
            {
                if(value.type == (int)type)
                {
                    list.Add(value);
                }
            }
            
            // ランク順にソート
            list.Sort((v1, v2)=>v1.rankNumber.CompareTo(v2.rankNumber));
            // 配列化
            CharaRankMasterObject[] result = list.ToArray();
            // キャッシュに追加
            cacheData.Add(type, result);
            return result;
        }
    }
}
