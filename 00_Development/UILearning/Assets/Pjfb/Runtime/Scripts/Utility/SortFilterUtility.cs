using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pjfb;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UserData;

public enum OrderType
{
    Ascending,
    Descending,
}

public enum PriorityType
{
    // Character
    Lv = 0,
    LiberationLv,
    CombatPower,
    Rank,
    Rarity,
    BaseRarity,
    Stamina,
    Speed,
    Physical,
    Technique,
    Intelligence,
    Kick,
    
    
    // Support Equipment
    SupportEquipmentRarity = 10000,
    SupportEquipmentAcquisitionOrder = 10001,
    SupportEquipmentSubAbilityCount = 10002,
    
    
    // CombinationSkill
    // 組み合わせID順（デフォルト）
    SortNumber = 20000,
    // スキルのレアリティ順
    SkillRarity = 20001,
    // 発動スキル数順
    SkillCount = 20002,
}

public enum SelectFilterType
{
    // 絞り込みOFF
    Off = 0,
    // 選択したいずれかを含む（OR検索）
    Any = 1,
    // 選択した全てを含む（AND検索）
    All = 2,
}


[Serializable]
public class SortDataBase
{
    public SortDataBase()
    {
        orderType = OrderType.Descending;
    }
    public OrderType orderType;
    public PriorityType priorityType;
}

[Serializable]
public class FilterDataBase
{
    
}

/// <summary> UChara用フィルターインターフェース </summary>
public interface IFilterUChara
{
    /// <summary> フィルターに一致するか </summary>
    public bool IsMatchFilter(UserDataChara uChara);
}

/// <summary>
/// 強化選手の並び替え/絞り込みをまとめたデータ
/// </summary>
[Serializable]
public class BaseCharacterSortFilterData
{
    public BaseCharacterSortData sortData = new();
    public BaseCharacterFilterData filterData = new();
}


/// <summary>
/// 育成キャララクター並び替えデータ
/// </summary>
[Serializable]
public class BaseCharacterSortData : SortDataBase
{
    public BaseCharacterSortData()
    {
        priorityType = PriorityType.Rarity;
    }
}

/// <summary>
/// 育成キャララクター絞り込みデータ
/// </summary>
[Serializable]
public class BaseCharacterFilterData : FilterDataBase, IFilterUChara
{
    public List<long> rarityList = new();
    public List<CharacterType> typeList = new();

    public bool IsMatchFilter(UserDataChara uChara)
    {
        // レアリティフィルターチェック（設定されている場合のみ）
        bool isRarityMatch = rarityList.Count == 0 || rarityList.Contains(RarityUtility.GetRarity(uChara.charaId, uChara.newLiberationLevel));
    
        // タイプフィルターチェック（設定されている場合のみ）
        bool isTypeMatch = typeList.Count == 0 || typeList.Contains(uChara.MChara.charaType);
    
        // 設定されているすべてのフィルターに一致する場合のみtrue
        return isRarityMatch && isTypeMatch;
    }
}

/// <summary>
/// 強化選手の並び替え/絞り込みをまとめたデータ
/// </summary>
[Serializable]
public class SuccessCharacterSortFilterData
{
    public SuccessCharacterSortData sortData = new();
    public SuccessCharacterFilterData filterData = new();
}

/// <summary>
/// サクセスキャラクター並び替えデータ
/// </summary>
[Serializable]
public class SuccessCharacterSortData : SortDataBase
{
    public SuccessCharacterSortData()
    {
        priorityType = PriorityType.CombatPower;
    }
}

/// <summary>
/// サクセスキャラクター絞り込みデータ
/// </summary>
[Serializable]
public class SuccessCharacterFilterData : FilterDataBase
{
    public enum FavoriteType
    {
        Favorite,
        All,
    }
    
    public List<long> rankList = new List<long>();
    public FavoriteType favoriteType = FavoriteType.All;
}

/// <summary>
/// 強化選手の並び替え/絞り込みをまとめたデータ
/// </summary>
[Serializable]
public class SpecialSupportCardSortFilterData
{
    public SpecialSupportCardSortData sortData = new();
    public SpecialSupportCardFilterData filterData = new();
}

/// <summary>
/// スペシャルサポートカード並び替えデータ
/// </summary>
[Serializable]
public class SpecialSupportCardSortData : SortDataBase
{
    public SpecialSupportCardSortData()
    {
        priorityType = PriorityType.Rarity;
    }
}

/// <summary>
/// スペシャルサポートカード絞り込みデータ
/// </summary>
[Serializable]
public class SpecialSupportCardFilterData : FilterDataBase, IFilterUChara
{
    public List<long> rarityList = new();
    public List<CharacterType> typeList = new();
    public List<CharacterExtraType> extraList = new();

    public bool IsMatchFilter(UserDataChara uChara)
    {
        // レアリティフィルターチェック（設定されている場合のみ）
        bool isRarityMatch = rarityList.Count == 0 || rarityList.Contains(RarityUtility.GetRarity(uChara.charaId, uChara.newLiberationLevel));
    
        // タイプフィルターチェック（設定されている場合のみ）
        bool isTypeMatch = typeList.Count == 0 || typeList.Contains(uChara.MChara.charaType);
    
        // エクストラタイプフィルターチェック（設定されている場合のみ）
        bool isExtraTypeMatch = extraList.Count == 0 || extraList.Contains(uChara.MChara.isExtraSupport ? CharacterExtraType.Extra : CharacterExtraType.Normal);
        
        // 設定されているすべてのフィルターに一致する場合のみtrue
        return isRarityMatch && isTypeMatch && isExtraTypeMatch;
    }
}

/// <summary>
/// 強化選手の並び替え/絞り込みをまとめたデータ
/// </summary>
[Serializable]
public class SupportEquipmentSortFilterData
{
    public SupportEquipmentSortData sortData = new();
    public SupportEquipmentFilterData filterData = new();
}

/// <summary>
/// サクセスキャラクター並び替えデータ
/// </summary>
[Serializable]
public class SupportEquipmentSortData : SortDataBase
{
    public SupportEquipmentSortData()
    {
        priorityType = PriorityType.SupportEquipmentRarity;
    }
}

/// <summary>
/// サクセスキャラクター絞り込みデータ
/// </summary>
[Serializable]
public class SupportEquipmentFilterData : FilterDataBase
{
    public List<long> iconTypeList = new List<long>();
    public List<long> rarityList = new List<long>();
    public List<long> typeList = new List<long>();
    public List<long> practiceSkillList = new List<long>();
}

/// <summary>
/// ユーザーアイコンの並び替え/絞り込みをまとめたデータ
/// </summary>
[Serializable]
public class UserIconSortFilterData
{
    public UserIconSortData sortData = new();
    public UserIconFilterData filterData = new();
}

/// <summary>
/// ユーザーアイコン並び替えデータ
/// </summary>
[Serializable]
public class UserIconSortData : SortDataBase
{
    public UserIconSortData()
    {
        orderType = OrderType.Descending;
        priorityType = PriorityType.Rarity;
    }
}

/// <summary>
/// ユーザーアイコン絞り込みデータ
/// </summary>
[Serializable]
public class UserIconFilterData : FilterDataBase
{
    public List<long> rarityList = new List<long>();
}

[Serializable]
public class UserTitleSortFilterData
{
    public UserTitleSortData sortData = new();
    public UserTitleFilterData filterData = new();
}

[Serializable]
public class UserTitleSortData : SortDataBase
{
    public UserTitleSortData()
    {
        orderType = OrderType.Descending;
        priorityType = PriorityType.Rarity;
    }
}

[Serializable]
public class UserTitleFilterData : FilterDataBase
{
    public List<long> rarityList = new List<long>();
}

[Serializable]
public class MyBadgeSortFilterData
{
    public MyBadgeSortData sortData = new();
    public MyBadgeFilterData filterData = new();
}

[Serializable]
public class MyBadgeSortData : SortDataBase
{
    public MyBadgeSortData()
    {
        orderType = OrderType.Descending;
        priorityType = PriorityType.Rarity;
    }
}

[Serializable]
public class MyBadgeFilterData : FilterDataBase
{
    public List<long> rarityList = new List<long>();
}

/// <summary>
/// アドバイザーの並び替え/絞り込みをまとめたデータ
/// </summary>
[Serializable]
public class AdviserSortFilterData
{
    public AdviserSortData sortData = new();
    public AdviserFilterData filterData = new();
}


/// <summary>
/// アドバイザー並び替えデータ
/// </summary>
[Serializable]
public class AdviserSortData : SortDataBase
{
    public AdviserSortData()
    {
        orderType = OrderType.Descending;
        priorityType = PriorityType.Rarity;
    }
}

/// <summary>
/// アドバイザー絞り込みデータ
/// </summary>
[Serializable]
public class AdviserFilterData : FilterDataBase, IFilterUChara
{
    // レアリティ
    public List<long> rarityList = new();
    // タイプ
    public List<CharacterType> typeList = new();

    public bool IsMatchFilter(UserDataChara uChara)
    {
        // レアリティフィルターチェック（設定されている場合のみ）
        bool isRarityMatch = rarityList.Count == 0 || rarityList.Contains(RarityUtility.GetRarity(uChara.charaId, uChara.newLiberationLevel));
        
        // タイプフィルターチェック（設定されている場合のみ）
        bool isTypeMatch = typeList.Count == 0 || typeList.Contains(uChara.MChara.charaType);

        // 設定されているすべてのフィルターに一致する場合のみtrue
        return isRarityMatch && isTypeMatch;
    }
}

/// <summary>
///  スキルコネクトの並び替え/絞り込みをまとめたデータ
/// </summary>
[Serializable]
public class CombinationSkillSortFilterData
{
    public CombinationSkillSortData sortData = new();
    public CombinationSkillFilterData filterData = new();
}

/// <summary>
///  スキルコネクトの並び替えデータ
/// </summary>
[Serializable]
public class CombinationSkillSortData : SortDataBase
{
    public CombinationSkillSortData()
    {
        orderType = OrderType.Ascending;
        priorityType = PriorityType.SortNumber;
    }
}

/// <summary>
///  スキルコネクトの絞り込みデータ
/// </summary>
[Serializable]
public class CombinationSkillFilterData : FilterDataBase
{
    public SelectFilterType filterType = SelectFilterType.Off;
    public List<long> selectedCharaIdList = new List<long>();
}

/// <summary>
/// スキルコネクト絞り込み画面のキャラアイコンリストの並び替え/絞り込みをまとめたデータ
/// </summary>
[Serializable]
public class CombinationSkillCharaIconSortFilterData
{
    public CombinationSkillCharaIconSortData sortData = new();
    public CombinationSkillCharaIconFilterData filterData = new();
}

/// <summary>
/// スキルコネクト絞り込み画面のキャラアイコンリストの並び替えデータ
/// </summary>
[Serializable]
public class CombinationSkillCharaIconSortData : SortDataBase
{
    public CombinationSkillCharaIconSortData()
    {
        orderType = OrderType.Descending;
        priorityType = PriorityType.BaseRarity;
    }
}

/// <summary>
/// スキルコネクト絞り込み画面のキャラアイコンリストの絞り込みデータ
/// </summary>
[Serializable]
public class CombinationSkillCharaIconFilterData : FilterDataBase
{
    public List<long> selectedRarityList = new();
}


public static class SortFilterUtility
{
    private static readonly string SortAscendingStringValueKey = "common.ascending";
    private static readonly string SortDescendingStringValueKey = "common.descending";
    
    private static readonly string SortLvStringValueKey = "character.status.lv";
    private static readonly string SortLiberationLvStringValueKey = "character.status.ability_lv";
    private static readonly string SortCombatPowerStringValueKey = "character.status.total_power";
    private static readonly string SortRankStringValueKey = "common.rank";
    private static readonly string SortRarityStringValueKey = "common.rarity";
    private static readonly string SortBaseRarityStringValueKey = "common.base_rarity";
    private static readonly string SortStaminaStringValueKey = "character.status.stamina";
    private static readonly string SortSpeedStringValueKey = "character.status.speed";
    private static readonly string SortPhysicalStringValueKey = "character.status.physical";
    private static readonly string SortTechnicStringValueKey = "character.status.technique";
    private static readonly string SortIntelligenceStringValueKey = "character.status.intelligence";
    private static readonly string SortKickStringValueKey = "character.status.kick";
    private static readonly string SortSupportEquipmentRarityStringValueKey = "common.rarity";
    private static readonly string SortSupportEquipmentAcquisitionOrderStringValueKey = "common.acquisition_order";
    private static readonly string SortSupportEquipmentSubAbilityStringValueKey = "character.support_equipment.sub_ability_count";
    private static readonly string SortSortNumberStringValueKey = "combination.sort.sort_number";
    private static readonly string SortSkillRarityStringValueKey = "combination.sort.skill_rarity";
    private static readonly string SortSkillCountStringValueKey = "combination.sort.skill_count";
    private static readonly string FilterOnStringValueKey = "character.scroll.filter_on";
    private static readonly string FilterOffStringValueKey = "character.scroll.filter_off";
    
    public static string GetSortPriorityKey(PriorityType priorityType)
    {
        switch (priorityType)
        {
            case PriorityType.Lv:
                return SortLvStringValueKey;
            case PriorityType.LiberationLv:
                return SortLiberationLvStringValueKey;
            case PriorityType.CombatPower:
                return SortCombatPowerStringValueKey;
            case PriorityType.Rank:
                return SortRankStringValueKey;
            case PriorityType.Rarity:
                return SortRarityStringValueKey;
            case PriorityType.BaseRarity:
                return SortBaseRarityStringValueKey;
            case PriorityType.Stamina:
                return SortStaminaStringValueKey;
            case PriorityType.Speed:
                return SortSpeedStringValueKey;
            case PriorityType.Physical:
                return SortPhysicalStringValueKey;
            case PriorityType.Technique:
                return SortTechnicStringValueKey;
            case PriorityType.Intelligence:
                return SortIntelligenceStringValueKey;
            case PriorityType.Kick:
                return SortKickStringValueKey;
            case PriorityType.SupportEquipmentRarity:
                return SortSupportEquipmentRarityStringValueKey;
            case PriorityType.SupportEquipmentAcquisitionOrder:
                return SortSupportEquipmentAcquisitionOrderStringValueKey;
            case PriorityType.SupportEquipmentSubAbilityCount:
                return SortSupportEquipmentSubAbilityStringValueKey;
            case PriorityType.SortNumber:
                return SortSortNumberStringValueKey;
            case PriorityType.SkillRarity:
                return SortSkillRarityStringValueKey;
            case PriorityType.SkillCount:
                return SortSkillCountStringValueKey;
            default:
                // Todo : delete
                // 古いデータ
                return SortLvStringValueKey;
            //throw new ArgumentOutOfRangeException();
        }
    }
    
    public static string GetIsFilterKey(bool isFilter)
    {
        return isFilter ? FilterOnStringValueKey : FilterOffStringValueKey;
    }
    
    public static string GetSortOrderKey(OrderType orderType)
    {
        switch (orderType)
        {
            case OrderType.Ascending:
                return SortAscendingStringValueKey;
            case OrderType.Descending:
                return SortDescendingStringValueKey;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary> 指定したフィルターデータに一致するリストを取得 </summary>
    public static List<UserDataChara> GetFilterList<TFilter>(this List<UserDataChara> list, SortFilterType type, HashSet<long> excludeIdSet = null, List<long> ignoreIds = null)
        where TFilter : IFilterUChara 
    {
        excludeIdSet ??= new HashSet<long>();
        if (GetFilterDataByType(type) is not TFilter filterData) return list;
        var result = new List<UserDataChara>();
        foreach(UserDataChara uChara in list)
        {
            
            if(ignoreIds != null && ignoreIds.Contains(uChara.id))
            {
                continue;
            }
            
            if (!excludeIdSet.Contains(uChara.id))
            {
                if (filterData.IsMatchFilter(uChara) == false) continue;
            }
            result.Add(uChara);
        }
        return result;
    }
    
    #region BaseCharacter

    public static List<UserDataChara> GetSortBaseCharacterList(this List<UserDataChara> list, SortFilterType type)
    {
        if (GetSortDataByType(type) is not BaseCharacterSortData sortData) return list;
        
        if (sortData.priorityType is PriorityType.CombatPower)
            sortData.priorityType = PriorityType.Rarity;
        

        //第2ソートはレアリティ降順
        //第3ソートはレベル降順
        //第4ソートはId昇順
        switch (sortData.priorityType)
        {
            case PriorityType.Lv:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.level)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => data.level)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.LiberationLv:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.newLiberationLevel)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => data.newLiberationLevel)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Rarity:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.BaseRarity:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => RarityUtility.GetBaseRarity(data.charaId))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => RarityUtility.GetBaseRarity(data.charaId))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Stamina:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Stamina)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Stamina)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Speed:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Speed)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Speed)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Physical:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Physical)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Physical)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Technique:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Technique)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Technique)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList();
                break;

            case PriorityType.Kick:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Kick)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Kick)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Intelligence:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Intelligence)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.charaId, data.level, data.newLiberationLevel).Intelligence)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.charaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.charaId).ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return list;
    }

    public static List<UserDataChara> GetFilterBaseCharacterList(this List<UserDataChara> list, SortFilterType type, HashSet<long> excludeIdSet = null, List<long> ignoreIds = null)
    {
        return GetFilterList<BaseCharacterFilterData>(list, type, excludeIdSet, ignoreIds);
    }
    
    // Todo:共通化を行う
    public static List<CharaV2FriendLend> GetSortBaseCharacterList(this List<CharaV2FriendLend> list, SortFilterType type)
    {
        if (GetSortDataByType(type) is not BaseCharacterSortData sortData) return list;
        
        if (sortData.priorityType is PriorityType.CombatPower)
            sortData.priorityType = PriorityType.Rarity;
        

        //第2ソートはレアリティ降順
        //第3ソートはレベル降順
        //第4ソートはId昇順
        switch (sortData.priorityType)
        {
            case PriorityType.Lv:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.level)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenBy(data => data.mCharaId).ToList()
                    : list.OrderByDescending(data => data.level)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenBy(data => data.mCharaId).ToList();
                break;
            case PriorityType.LiberationLv:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.newLiberationLevel)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList()
                    : list.OrderByDescending(data => data.newLiberationLevel)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList();
                break;
            case PriorityType.Rarity:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList()
                    : list.OrderByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList();
                break;
            case PriorityType.BaseRarity:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => RarityUtility.GetBaseRarity(data.mCharaId))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList()
                    : list.OrderByDescending(data => RarityUtility.GetBaseRarity(data.mCharaId))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList();
                break;
            case PriorityType.Stamina:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Stamina)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Stamina)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList();
                break;
            case PriorityType.Speed:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Speed)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Speed)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList();
                break;
            case PriorityType.Physical:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Physical)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Physical)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList();
                break;
            case PriorityType.Technique:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Technique)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Technique)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList();
                break;

            case PriorityType.Kick:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Kick)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Kick)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList();
                break;
            case PriorityType.Intelligence:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Intelligence)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.CalcCharacterStatus(data.mCharaId, data.level, data.newLiberationLevel).Intelligence)
                        .ThenByDescending(data => RarityUtility.GetRarity(data.mCharaId, data.newLiberationLevel))
                        .ThenByDescending(data => data.level)
                        .ThenBy(data => data.mCharaId).ToList();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return list;
    }

    // Todo:共通化を行う
    public static List<CharaV2FriendLend> GetFilterBaseCharacterList(this List<CharaV2FriendLend> list, SortFilterType type, List<long> excludeIdSet = null, List<long> ignoreIds = null)
    {
        excludeIdSet ??= new List<long>();
        if (GetFilterDataByType(type) is not BaseCharacterFilterData filterData) return list;
        var result = new List<CharaV2FriendLend>();
        foreach(CharaV2FriendLend friendChara in list)
        {            
            if(ignoreIds != null && ignoreIds.Contains(friendChara.id))
            {
                continue;
            }
            
            if (!excludeIdSet.Contains(friendChara.id))
            {
                var mChara = MasterManager.Instance.charaMaster.FindData(friendChara.mCharaId);
                if (filterData.rarityList.Count > 0 && !filterData.rarityList.Contains(RarityUtility.GetRarity(friendChara.mCharaId, friendChara.newLiberationLevel))) continue;
                if (filterData.typeList.Count > 0 && mChara != null && !filterData.typeList.Contains(mChara.charaType)) continue;
            }
            result.Add(friendChara);
        }
        return result;
    }

    #endregion

    #region SuccessCharacter

    public static List<UserDataCharaVariable> GetSortSuccessCharacterList(this List<UserDataCharaVariable> list, SortFilterType type)
    {
        if (GetSortDataByType(type) is not SuccessCharacterSortData sortData) return list;

        if (sortData.priorityType is PriorityType.Lv or PriorityType.LiberationLv)
            sortData.priorityType = PriorityType.CombatPower;

        if (sortData.priorityType is PriorityType.Rarity or PriorityType.BaseRarity)
            sortData.priorityType = PriorityType.Rank;
        
        //第2ソートは評価点降順
        //第3ソートはId昇順
        // spd		    スピード（万分率）
        // tec		    テクニック（万分率）
        // param1(sta)	スタミナ（万分率）
        // param2(phy)	フィジカル（万分率）
        // param3(sig)	視野（万分率）
        // param4(kic)	キック（万分率）
        // param5(wis)	賢さ（万分率）
        switch (sortData.priorityType)
        {
            case PriorityType.CombatPower:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.combatPower)
                        .ThenByDescending(data => data.charaId).ToList()
                    : list.OrderByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Rank:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => StatusUtility.GetCharacterRank(new BigValue(data.combatPower)))
                        .ThenByDescending(data => data.combatPower)
                        .ThenByDescending(data => data.charaId).ToList()
                    : list.OrderByDescending(data => StatusUtility.GetCharacterRank(new BigValue(data.combatPower)))
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Stamina:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.param1)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => data.param1)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Speed:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.spd)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => data.spd)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Physical:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.param2)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => data.param2)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Technique:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.tec)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => data.tec)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList();
                break;

            case PriorityType.Intelligence:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.param5)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => data.param5)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Kick:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.param4)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => data.param4)
                        .ThenByDescending(data => data.combatPower)
                        .ThenBy(data => data.charaId).ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return list;
    }
    
    public static List<UserDataCharaVariable> GetFilterSuccessCharacterList(this List<UserDataCharaVariable> list, SortFilterType type , HashSet<long> excludeIdSet = null)
    {
        excludeIdSet ??= new HashSet<long>();

        if (GetFilterDataByType(type) is not SuccessCharacterFilterData filterData) return list;
        var result = new List<UserDataCharaVariable>();
        foreach(var uCharaVariable in list)
        {
            if (!excludeIdSet.Contains(uCharaVariable.id))
            {
                if (filterData.rankList.Count > 0 && !filterData.rankList.Contains(StatusUtility.GetCharacterRank( new BigValue(uCharaVariable.combatPower) ))) continue;
                if (filterData.favoriteType == SuccessCharacterFilterData.FavoriteType.Favorite && !uCharaVariable.isLocked) continue;
            }
           
            result.Add(uCharaVariable);
        }
        return result;
    }
    
    #endregion

    #region SpecialSupportCard

    public static List<UserDataChara> GetSortSpecialSupportCardList(this List<UserDataChara> list, SortFilterType type)
    {
        if (GetSortDataByType(type) is not SpecialSupportCardSortData sortData) return list;
        
        if (sortData.priorityType is PriorityType.CombatPower)
            sortData.priorityType = PriorityType.Rarity;
        
        switch (sortData.priorityType)
        {
            case PriorityType.Lv:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.level)
                        .ThenByDescending(data => MasterManager.Instance.rarityMaster.FindData(data.MChara.mRarityId).value)
                        .ThenByDescending(data => MasterManager.Instance.charaMaster.FindData(data.charaId).isExtraSupport )
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => data.level)
                        .ThenByDescending(data => MasterManager.Instance.rarityMaster.FindData(data.MChara.mRarityId).value)
                        .ThenByDescending(data => MasterManager.Instance.charaMaster.FindData(data.charaId).isExtraSupport )
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.LiberationLv:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.newLiberationLevel)
                        .ThenByDescending(data => MasterManager.Instance.rarityMaster.FindData(data.MChara.mRarityId).value)
                        .ThenByDescending(data => data.level)
                        .ThenByDescending(data => MasterManager.Instance.charaMaster.FindData(data.charaId).isExtraSupport )
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data => data.newLiberationLevel)
                        .ThenByDescending(data => MasterManager.Instance.rarityMaster.FindData(data.MChara.mRarityId).value)
                        .ThenByDescending(data => data.level)
                        .ThenByDescending(data => MasterManager.Instance.charaMaster.FindData(data.charaId).isExtraSupport )
                        .ThenBy(data => data.charaId).ToList();
                break;
            case PriorityType.Rarity:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => MasterManager.Instance.rarityMaster.FindData(data.MChara.mRarityId).value)
                        .ThenByDescending(data => data.level)
                        .ThenByDescending(data => MasterManager.Instance.charaMaster.FindData(data.charaId).isExtraSupport )
                        .ThenBy(data => data.charaId).ToList()
                    : list.OrderByDescending(data =>MasterManager.Instance.rarityMaster.FindData(data.MChara.mRarityId).value)
                        .ThenByDescending(data => data.level)
                        .ThenByDescending(data => MasterManager.Instance.charaMaster.FindData(data.charaId).isExtraSupport )
                        .ThenBy(data => data.charaId).ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return list;
    }

    public static List<UserDataChara> GetFilterSpecialSupportCardList(this List<UserDataChara> list, SortFilterType type, HashSet<long> excludeIdSet = null, List<long> ignoreIds = null)
    {
        return GetFilterList<SpecialSupportCardFilterData>(list, type, excludeIdSet, ignoreIds);
    }

    #endregion

    
     #region Support Equipment

     public static List<UserDataSupportEquipment> GetSortSupportEquipmentList(this List<UserDataSupportEquipment> list, SortFilterType type) 
     {
        if (GetSortDataByType(type) is not SupportEquipmentSortData sortData) return list;
        
        if (sortData.priorityType is PriorityType.CombatPower)
            sortData.priorityType = PriorityType.SupportEquipmentRarity;
        

        //第2ソートは 種類 降順
        //第3ソートは レアリティ 降順
        //第4ソートは 入手順 (レアリティだけ) 降順
        //第5ソートは mCharaId 昇順
        //第6ソートは Id 昇順
        switch (sortData.priorityType)
        {
            case PriorityType.SupportEquipmentRarity:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.MChara.Rarity)
                        .ThenBy(data => data.Type)
                        .ThenByDescending(data => data.createAt)
                        .ThenBy(data => data.charaId)
                        .ThenBy(data => data.id).ToList()
                    : list.OrderByDescending(data => data.MChara.Rarity)
                        .ThenBy(data => data.Type)
                        .ThenByDescending(data => data.createAt)
                        .ThenBy(data => data.charaId)
                        .ThenBy(data => data.id).ToList();
                break;
            case PriorityType.SupportEquipmentAcquisitionOrder:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.createAt)
                        .ThenBy(data => data.Type)
                        .ThenByDescending(data => data.MChara.Rarity)
                        .ThenBy(data => data.charaId)
                        .ThenBy(data => data.id).ToList()
                    : list.OrderByDescending(data => data.createAt)
                        .ThenBy(data => data.Type)
                        .ThenByDescending(data => data.MChara.Rarity)
                        .ThenBy(data => data.charaId)
                        .ThenBy(data => data.id).ToList();
                break;
            case PriorityType.SupportEquipmentSubAbilityCount:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderBy(data => data.SubPracticeSkillDataList.Count)
                        .ThenBy(data => data.Type)
                        .ThenByDescending(data => data.MChara.Rarity)
                        .ThenBy(data => data.charaId)
                        .ThenBy(data => data.id).ToList()
                    : list.OrderByDescending(data => data.SubPracticeSkillDataList.Count)
                        .ThenBy(data => data.Type)
                        .ThenByDescending(data => data.MChara.Rarity)
                        .ThenBy(data => data.charaId)
                        .ThenBy(data => data.id).ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return list;
     }

     public static List<UserDataSupportEquipment> GetFilterSupportEquipmentList(this List<UserDataSupportEquipment> list, SortFilterType type, HashSet<long> excludeIdSet = null)
     {
         excludeIdSet ??= new HashSet<long>();
         if (GetFilterDataByType(type) is not SupportEquipmentFilterData filterData) return list;
         var result = new List<UserDataSupportEquipment>();
         foreach (UserDataSupportEquipment uSupportEquipment in list)
         {
             if (!excludeIdSet.Contains(uSupportEquipment.id))
             {
                 CharaTrainerLotteryMasterObject charaTrainerLottery = MasterManager.Instance.charaTrainerLotteryMaster.FindData(uSupportEquipment.MChara.mCharaTrainerLotteryId);
                 if(filterData.iconTypeList.Count > 0 && 
                    !filterData.iconTypeList.Contains(charaTrainerLottery.iconType)) continue;
                 if (filterData.rarityList.Count > 0 &&
                     !filterData.rarityList.Contains(uSupportEquipment.MChara.Rarity)) continue;
                 if (filterData.typeList.Count > 0 && !filterData.typeList.Contains(uSupportEquipment.Type)) continue;
                 if (filterData.practiceSkillList.Count > 0 && !CheckHavePracticeSkill(filterData.practiceSkillList, uSupportEquipment)) continue; 
             }

             result.Add(uSupportEquipment);
         }

         return result;
     }

     private static bool CheckHavePracticeSkill(List<long> practiceSkillList,
         UserDataSupportEquipment userDataSupportEquipment)
     {
         var mainSkillDataList =
             PracticeSkillUtility.GetCharacterPracticeSkill(userDataSupportEquipment.charaId, userDataSupportEquipment.level);
         foreach (var practiceSkillId in practiceSkillList)
         {
             if (mainSkillDataList.Any(data => data.TrainingStatusTypeDetailId == practiceSkillId))
             {
                 return true;
             }

             if (userDataSupportEquipment.SubPracticeSkillDataList.Any(data =>
                     data.TrainingStatusTypeDetailId == practiceSkillId))
             {
                 return true;
             }
         }
         
         return false;
     }

     public static List<UserDataSupportEquipment> GetAllSellFilterSupportEquipmentList(this List<UserDataSupportEquipment> list, SortFilterType type, HashSet<long> excludeIdSet = null)
     {
         //フィルターが何も設定されていない場合、空のリストを返す
         if (GetFilterDataByType(type) is not SupportEquipmentFilterData filterData) return new List<UserDataSupportEquipment>();
         if (filterData.iconTypeList.Count == 0 && filterData.rarityList.Count == 0 && filterData.typeList.Count == 0 && filterData.practiceSkillList.Count == 0)
             return new List<UserDataSupportEquipment>();
         
         list = list.GetFilterSupportEquipmentList(type, excludeIdSet);
         return list.GetSortSupportEquipmentList(type);
     }
    
     public static List<SupportEquipmentScrollData> SortSupportEquipmentListSortOrder(this List<SupportEquipmentScrollData> list, SupportEquipmentSortData sortData, List<SupportEquipmentScrollData> nonSortList = null) 
     {
        //第2ソートは 種類 降順
        //第3ソートは レアリティ 降順
        //第4ソートは 入手順 (レアリティだけ) 降順
        //第5ソートは mCharaId 昇順
        switch (sortData.priorityType)
        {
            case PriorityType.SupportEquipmentRarity:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderByDescending(data=> data.Options & SupportEquipmentScrollDataOptions.Formatting)
                        .ThenBy(data => data.MChara.Rarity)
                        .ThenBy(data => data.USupportEquipment.Type)
                        .ThenByDescending(data => data.USupportEquipment.createAt)
                        .ThenBy(data => data.MCharaId).ToList()
                    : list.OrderByDescending(data=> data.Options & SupportEquipmentScrollDataOptions.Formatting)
                        .ThenByDescending(data => data.MChara.Rarity)
                        .ThenBy(data => data.USupportEquipment.Type)
                        .ThenByDescending(data => data.USupportEquipment.createAt)
                        .ThenBy(data => data.MCharaId).ToList();
                break;
            case PriorityType.SupportEquipmentAcquisitionOrder:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderByDescending(data=> data.Options & SupportEquipmentScrollDataOptions.Formatting)
                        .ThenBy(data => data.USupportEquipment.createAt)
                        .ThenBy(data => data.USupportEquipment.Type)
                        .ThenByDescending(data => data.MChara.Rarity)
                        .ThenBy(data => data.MCharaId).ToList()
                    : list.OrderByDescending(data=> data.Options & SupportEquipmentScrollDataOptions.Formatting)
                        .ThenByDescending(data => data.USupportEquipment.createAt)
                        .ThenBy(data => data.USupportEquipment.Type)
                        .ThenByDescending(data => data.MChara.Rarity)
                        .ThenBy(data => data.MCharaId).ToList();
                break;
            case PriorityType.SupportEquipmentSubAbilityCount:
                list = sortData.orderType == OrderType.Ascending
                    ? list.OrderByDescending(data=> data.Options & SupportEquipmentScrollDataOptions.Formatting)
                        .ThenBy(data => data.USupportEquipment.SubPracticeSkillDataList.Count)
                        .ThenBy(data => data.USupportEquipment.Type)
                        .ThenByDescending(data => data.MChara.Rarity)
                        .ThenBy(data => data.MCharaId).ToList()
                    : list.OrderByDescending(data=> data.Options & SupportEquipmentScrollDataOptions.Formatting)
                        .ThenByDescending(data => data.USupportEquipment.SubPracticeSkillDataList.Count)
                        .ThenBy(data => data.USupportEquipment.Type)
                        .ThenByDescending(data => data.MChara.Rarity)
                        .ThenBy(data => data.MCharaId).ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (nonSortList != null)
        {
            nonSortList.AddRange(list);
            return nonSortList;
        }
        else
        {
            return list;
        }
     }

    #endregion

    #region Adviser

    /// <summary> アドバイザーフィルター適用リスト取得 </summary>
    public static List<UserDataChara> GetFilterAdviserList(this List<UserDataChara> list, SortFilterType type, HashSet<long> excludeIdSet = null, List<long> ignoreIds = null)
    {
        return GetFilterList<AdviserFilterData>(list, type, excludeIdSet, ignoreIds);
    }

    /// <summary> アドバイザーソート適用リスト取得 </summary>
    public static List<UserDataChara> GetSortAdviserList(this List<UserDataChara> list, SortFilterType type)
    {
        AdviserSortData sortData = (AdviserSortData)GetSortDataByType(type);
        IOrderedEnumerable<UserDataChara> sort = null;
        switch (sortData.priorityType)
        {
            // レベル
            case PriorityType.Lv:
            {
                // 昇順、降順ごとに並び替え
                sort = sortData.orderType == OrderType.Ascending 
                    ? list.OrderBy(x => x.level)
                    : list.OrderByDescending(x => x.level);

                // レアリティ降順、キャラId昇順
                return sort.ThenByDescending(x => RarityUtility.GetRarity(x.charaId, x.newLiberationLevel)).ThenBy(x => x.charaId).ToList();
            }
            // 解放レベル
            case PriorityType.LiberationLv:
            {
                // 昇順、降順ごとに並び替え
                sort = sortData.orderType == OrderType.Ascending 
                    ? list.OrderBy(x => x.newLiberationLevel)
                    : list.OrderByDescending(x => x.newLiberationLevel);

                // レアリティ降順
                return sort.ThenByDescending(x => RarityUtility.GetRarity(x.charaId, x.newLiberationLevel))
                    // レベル降順
                    .ThenByDescending(x => x.level)
                    //キャラId昇順
                    .ThenBy(x => x.charaId).ToList();
            }
            // レアリティ
            case PriorityType.Rarity:
            {
                // 昇順、降順ごとに並び替え
                sort = sortData.orderType == OrderType.Ascending 
                    ? list.OrderBy(x => RarityUtility.GetRarity(x.charaId, x.newLiberationLevel))
                    : list.OrderByDescending(x => RarityUtility.GetRarity(x.charaId, x.newLiberationLevel));

                // レベル降順
                return sort.ThenByDescending(x => x.level)
                    //キャラId昇順
                    .ThenBy(x => x.charaId).ToList();
            }
            // ベースレアリティ
            case PriorityType.BaseRarity:
            {
                // 昇順、降順ごとに並び替え
                sort = sortData.orderType == OrderType.Ascending 
                    ? list.OrderBy(x => RarityUtility.GetBaseRarity(x.charaId))
                    : list.OrderByDescending(x => RarityUtility.GetBaseRarity(x.charaId));

                // レベル降順
                return sort.ThenByDescending(x => x.level)
                    //キャラId昇順
                    .ThenBy(x => x.charaId).ToList();
            }
        }
        
        return list;
    }
    
    #endregion
    
    #region CombinationSkill
    
    /// <summary> スキルコネクト内の共通フィルター処理 </summary>
    private static List<T> GetFilterCombinationSkillList<T>(this List<T> list, SortFilterType type, List<long> selectCharaIdSet = null) where T : ICombinationFilterable
    {
        if (GetFilterDataByType(type) is not CombinationSkillFilterData filterData) return list;
        // 選択が空なら即return
        if (selectCharaIdSet == null || selectCharaIdSet.Count <= 0) return list;

        var filteredList = new List<T>();

        switch (filterData.filterType)
        {
            // 絞り込みOFF
            case SelectFilterType.Off:
                filteredList = list;
                break;
            // 選択したいずれかを含む（OR検索）
            case SelectFilterType.Any:
                foreach (var combinationSkill in list)
                {
                    var mCharaIdSet = combinationSkill.CharaIdHashSet;

                    foreach (var charaId in selectCharaIdSet)
                    {
                        // いずれかが含まれていたら、そのCombinationSkillをリストに追加
                        if (mCharaIdSet.Contains(charaId))
                        {
                            filteredList.Add(combinationSkill);
                            break;
                        }
                    }
                }
                break;
            // 選択した全てを含む（AND検索）
            case SelectFilterType.All:
                foreach (var combinationSkill in list)
                {
                    var mCharaIdSet = combinationSkill.CharaIdHashSet;
                    
                    // 全て含んでいるか
                    bool isAllInclude = true;
                    foreach (long charaId in selectCharaIdSet)
                    {
                        // 含まれてない
                        if (mCharaIdSet.Contains(charaId) == false)
                        {
                            isAllInclude = false;
                            break;
                        }
                    }
                    
                    // すべて含まれていたら、そのCombinationTrainingをリストに追加
                    if (isAllInclude)
                    {
                        filteredList.Add(combinationSkill);
                    }
                }
                break;
        }
        
        return filteredList;
    }
    /// <summary> スキルコネクト内の共通ソート処理 </summary>
    private static List<T> GetSortCombinationSkillList<T>(this List<T> list, SortFilterType type) where T : ICombinationSortable
    {
        if (GetSortDataByType(type) is not CombinationSkillSortData sortData) return list;
        
        switch (sortData.priorityType)
        {
            // SortNumber順
            case PriorityType.SortNumber:
                list = sortData.orderType == OrderType.Ascending
                    // 昇順で追加
                    ? list.OrderBy(t => t.SortNumber)
                        // 第二ソートはID順
                        .ThenBy(t => t.Id).ToList()
                    // 降順で追加
                    : list.OrderByDescending(t => t.SortNumber)
                        // 第二ソートはID順
                        .ThenBy(t => t.Id).ToList();
                break;
            // 発動スキル数順
            case PriorityType.SkillCount:
                list = sortData.orderType == OrderType.Ascending
                    // 昇順で追加
                    ? list.OrderBy(t => t.SkillCount)
                        // 第二ソートはSortNumber順
                        .ThenBy(t => t.SortNumber)
                        // 第三ソートはID順
                        .ThenBy(t => t.Id).ToList()
                    // 降順で追加
                    : list.OrderByDescending(t => t.SkillCount)
                        // 第二ソートはSortNumber順
                        .ThenBy(t => t.SortNumber)
                        // 第三ソートはID順
                        .ThenBy(t => t.Id).ToList();
                break;
            // スキルレアリティ順
            case PriorityType.SkillRarity:
                list = sortData.orderType == OrderType.Ascending
                    // 昇順で追加
                    ? list.OrderBy(t => t.SkillRarity)
                        // 第二ソートはSortNumber順
                        .ThenBy(t => t.SortNumber)
                        // 第三ソートはID順
                        .ThenBy(t => t.Id).ToList()
                    // 降順で追加
                    : list.OrderByDescending(t => t.SkillRarity)
                        // 第二ソートはSortNumber順
                        .ThenBy(t => t.SortNumber)
                        // 第三ソートはID順
                        .ThenBy(t => t.Id).ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return list;
    }
    
    /// <summary> マッチスキルフィルター適用リスト取得 </summary>
    public static List<CombinationManager.CombinationMatch> GetFilterCombinationMatchList(this List<CombinationManager.CombinationMatch> list, SortFilterType type, List<long> selectCharaIdSet = null)
    {
        return list.GetFilterCombinationSkillList(type, selectCharaIdSet);
    }
    /// <summary> マッチスキルソート適用リスト取得 </summary>
    public static List<CombinationManager.CombinationMatch> GetSortCombinationMatchList(this List<CombinationManager.CombinationMatch> list, SortFilterType type)
    {
        return list.GetSortCombinationSkillList(type);
    }
    
    /// <summary> トレーニングスキルフィルター適用リスト取得 </summary>
    public static List<CombinationManager.CombinationTraining> GetFilterCombinationTrainingList(this List<CombinationManager.CombinationTraining> list, SortFilterType type, List<long> selectCharaIdSet = null)
    {
        return list.GetFilterCombinationSkillList(type, selectCharaIdSet);
    }
    /// <summary> トレーニングスキルソート適用リスト取得 </summary>
    public static List<CombinationManager.CombinationTraining> GetSortCombinationTrainingList(this List<CombinationManager.CombinationTraining> list, SortFilterType type)
    {
        return list.GetSortCombinationSkillList(type);
    }
    
    /// <summary> コレクションスキルフィルター適用リスト取得 </summary>
    public static List<CombinationManager.CombinationCollection> GetFilterCombinationCollectionList(this List<CombinationManager.CombinationCollection> list, SortFilterType type, List<long> selectCharaIdSet = null)
    {
        return list.GetFilterCombinationSkillList(type, selectCharaIdSet);
    }
    /// <summary> コレクションスキルソート適用リスト取得 </summary>
    public static List<CombinationManager.CombinationCollection> GetSortCombinationCollectionList(this List<CombinationManager.CombinationCollection> list, SortFilterType type)
    {
        return list.GetSortCombinationSkillList(type);
    }

    /// <summary> スキルコネクト絞り込み画面のキャラアイコンリストのフィルター適用リスト取得 </summary>
    public static List<UserDataChara> GetFilterCombinationSkillCharaIconList(this List<UserDataChara> charaList, SortFilterType type, List<long> selectedRarityList = null)
    {
        if (GetFilterDataByType(type) is not CombinationSkillCharaIconFilterData) return charaList;
        // 選択が空なら即return
        if (selectedRarityList == null || selectedRarityList.Count <= 0) return charaList;

        var filteredList = new List<UserDataChara>();
        
        foreach (var charaData in charaList)
        {
            // IDと能力開放レベルからレアリティを取得
            var charaRarity = RarityUtility.GetRarity(charaData.charaId, charaData.newLiberationLevel);
            
            foreach (var selectedRarity in selectedRarityList)
            {
                // 選択したレアリティのキャラをリストに追加
                if (charaRarity == selectedRarity)
                {
                    filteredList.Add(charaData);
                    break;
                }
            }
        }
        
        return filteredList;
    }
    /// <summary> スキルコネクト絞り込み画面のキャラアイコンリストのソート適用リスト取得 </summary>
    public static List<UserDataChara> GetSortCombinationSkillCharaIconList(this List<UserDataChara> charaList, SortFilterType type)
    {
        if (GetSortDataByType(type) is not CombinationSkillCharaIconSortData sortData) return charaList;
        
        switch (sortData.priorityType)
        {
            // 初期レアリティ順
            case PriorityType.BaseRarity:
                charaList = sortData.orderType == OrderType.Ascending
                    // 昇順で追加
                    ? charaList.OrderBy(x => RarityUtility.GetBaseRarity(x.charaId))
                        // 第二ソートはID昇順
                        .ThenBy(x => x.charaId).ToList()
                    // 降順で追加
                    : charaList.OrderByDescending(x => RarityUtility.GetBaseRarity(x.charaId))
                        // 第二ソートはID昇順
                        .ThenBy(x => x.charaId).ToList();
                break;
            // レアリティ順
            case PriorityType.Rarity:
                charaList = sortData.orderType == OrderType.Ascending
                    // 昇順で追加
                    ? charaList.OrderBy(x => RarityUtility.GetRarity(x.charaId, x.newLiberationLevel))
                        // 第二ソートはID昇順
                        .ThenBy(x => x.charaId).ToList()
                    // 降順で追加
                    : charaList.OrderByDescending(x => RarityUtility.GetRarity(x.charaId, x.newLiberationLevel))
                        // 第二ソートはID昇順
                        .ThenBy(x => x.charaId).ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return charaList;
    }
    
    #endregion
    
    #region UserTitle
    /// <summary>ソートしたタイトルのId取得</summary>
    public static List<long> GetSortUserTitle(List<long> titleIds, long currentId)
    {
        UserTitleSortData sortData = (UserTitleSortData)GetSortDataByType(SortFilterType.UserTitle);
        
        Dictionary<long, TitleMasterObject> titleMasterDic = new ();
        foreach (long id in titleIds)
        {
            titleMasterDic[id] = MasterManager.Instance.titleMaster.FindData(id);
        }
        switch (sortData.orderType)
        {
            case OrderType.Ascending:
                // レアリティ昇順、優先度降順、ID昇順
                return titleIds.OrderByDescending(id => id == currentId).ThenBy(id => MasterManager.Instance.rarityMaster.FindData(titleMasterDic[id].mRarityId).value)
                                    .ThenByDescending(id => titleMasterDic[id].priority)
                                        .ThenBy(id => id).ToList();
            case OrderType.Descending:
                return titleIds.OrderByDescending(id => id == currentId).ThenByDescending(id => MasterManager.Instance.rarityMaster.FindData(titleMasterDic[id].mRarityId).value)
                                    .ThenByDescending(id => titleMasterDic[id].priority)
                                        .ThenBy(id => id).ToList();
        }

        return titleIds;
    }
    
    /// <summary>フィルターしたタイトルのId取得</summary>
    public static List<long> GetFilterUserTitle(List<long> titleIds, long currentId)
    {
        UserTitleFilterData filterData = (UserTitleFilterData)GetFilterDataByType(SortFilterType.UserTitle);
        if (filterData.rarityList.Count > 0)
        {
            List<long> result = new List<long>();
            foreach (long titleId in titleIds)
            {
                TitleMasterObject titleMaster = MasterManager.Instance.titleMaster.FindData(titleId);
                if (filterData.rarityList.Contains(titleMaster.mRarityId) || titleId == currentId)
                {
                    result.Add(titleId);
                }
            }

            return result;
        }

        return titleIds;
    }

    #endregion

    #region UserIcon

    /// <summary>ユーザーアイコンのソート</summary>
    public static List<long> GetSortUserIcon(List<long> iconIds, long currentId)
    {
        UserIconSortData sortData = (UserIconSortData)GetSortDataByType(SortFilterType.UserIcon);
        Dictionary<long, IconMasterObject> iconMasterDic = new ();
        foreach (long id in iconIds)
        {
            iconMasterDic[id] = MasterManager.Instance.iconMaster.FindData(id);
        }
        
        switch (sortData.orderType)
        {
            case OrderType.Ascending:
                // レアリティ昇順、優先度降順、ID昇順
                return iconIds.OrderByDescending(id => id == currentId).ThenBy(id => MasterManager.Instance.rarityMaster.FindData(iconMasterDic[id].mRarityId).value)
                                  .ThenByDescending(id => iconMasterDic[id].priority)
                                        .ThenBy(id => id).ToList();
            case OrderType.Descending:
                return iconIds.OrderByDescending(id => id == currentId).ThenByDescending(id => MasterManager.Instance.rarityMaster.FindData(iconMasterDic[id].mRarityId).value)
                                  .ThenByDescending(id => iconMasterDic[id].priority)
                                        .ThenBy(id => id).ToList();
        }

        return iconIds;
    }
    
    /// <summary>ユーザーアイコンのフィルタ</summary>
    public static List<long> GetFilterUserIcon(List<long> iconIds, long currentId)
    {
        List<long> result = new List<long>();
        UserIconFilterData filterData = (UserIconFilterData)GetFilterDataByType(SortFilterType.UserIcon);
        if (filterData.rarityList.Count > 0)
        {
            foreach (long iconId in iconIds)
            {
                IconMasterObject iconMaster = MasterManager.Instance.iconMaster.FindData(iconId);
                if (filterData.rarityList.Contains(iconMaster.mRarityId) || iconId == currentId)
                {
                    result.Add(iconId);
                }
            }

            return result;
        }

        return iconIds;
    }

    #endregion
    
    #region MyBadge
    
    /// <summary>マイバッジのソート</summary>
    public static long[] GetSortMyBadge(long[] badgeIds, long[] settingBadgeIds)
    {
        MyBadgeSortData sortData = (MyBadgeSortData)GetSortDataByType(SortFilterType.MyBadge);
        Dictionary<long, EmblemMasterObject> badgeMasterDic = new ();
        foreach (long id in badgeIds)
        {
            badgeMasterDic[id] = MasterManager.Instance.emblemMaster.FindData(id);
        }
        
        switch (sortData.orderType)
        {
            case OrderType.Ascending:
                // レアリティ昇順、優先度降順、ID昇順
                return badgeIds.OrderByDescending(settingBadgeIds.Contains).ThenBy(id => MasterManager.Instance.rarityMaster.FindData(badgeMasterDic[id].mRarityId).value)
                                  .ThenByDescending(id => badgeMasterDic[id].priority)
                                        .ThenBy(id => id).ToArray();
            case OrderType.Descending:
                return badgeIds.OrderByDescending(settingBadgeIds.Contains).ThenByDescending(id =>MasterManager.Instance.rarityMaster.FindData(badgeMasterDic[id].mRarityId).value)
                                  .ThenByDescending(id => badgeMasterDic[id].priority)
                                        .ThenBy(id => id).ToArray();
        }

        return badgeIds;
    }
    
    /// <summary>マイバッジのフィルタ</summary>
    public static long[] GetFilterMyBadge(long[] badgeIds, long[] settingBadgeIds)
    {
        List<long> result = new List<long>();
        MyBadgeFilterData filterData = (MyBadgeFilterData)GetFilterDataByType(SortFilterType.MyBadge);
        // レアリティが設定されている場合フィルタ
        if (filterData.rarityList.Count > 0)
        {
            foreach (long badgeId in badgeIds)
            {
                EmblemMasterObject badgeMaster = MasterManager.Instance.emblemMaster.FindData(badgeId);
                if (filterData.rarityList.Contains(badgeMaster.mRarityId) || settingBadgeIds.Contains(badgeId))
                {
                    result.Add(badgeId);
                }
            }

            return result.ToArray();
        }

        return badgeIds;
    }
    
    #endregion

    public enum SortFilterType
    {
        None = -1,
        TrainingSelectBaseCharacter, // 強化選手 育成選手選択画面
        TrainingSupportSelectBaseCharacter, // 強化選手 サポート選手選択画面
        TrainingFriendSelectBaseCharacter, // 強化選手 フレンド選択画面
        GrowthLiberationListBaseCharacter, // 強化選手 選手強化/能力開放画面
        ListBaseCharacter, // 強化選手 選手一覧/解放画面
        FriendBorrowingBaseCharacter, // 強化選手 フレンド貸出設定画面
        ListSuccessCharacter, // 育成済み選手 選手一覧画面
        SellSuccessCharacter, // 育成済み選手 選手移籍画面
        FavoriteSuccessCharacter, // 育成済み選手 選手お気に入り画面
        DeckCharacterSelectSuccessCharacter, // 育成済み選手 デッキ編成（選手選択）画面
        ListSpecialSupportCard, // スペシャルサポートカード 一覧画面
        TrainingSelectSpecialSupportCard, // スペシャルサポートカード スペシャルサポートカード選択画面
        TrainingSelectExtraSupportCard, // エクストラサポートカード エクストラサポートカード選択画面
        ListSupportEquipment, // サポート器具 選手一覧画面
        SellSupportEquipment, // サポート器具 選手移籍画面
        FavoriteSupportEquipment, // サポート器具 選手お気に入り画面
        TrainingSelectSupportEquipment, // サポート器具 サポート器具選択画面
        AllSellSupportEquipment, // サポート器具 サポート器具一括売却画面
        UserIcon, // ユーザーアイコン
        UserTitle, // ユーザータイトル
        MyBadge, // マイバッジ
        AdviserGrowthLiberationList, // アドバイザー 強化、解放画面
        AdviserList, // アドバイザー 一覧画面
        TrainingAdviserList, // アドバイザー サポート選択画面
        AdviserDeckSelect, // アドバイザー デッキ編成（クラブロワイヤル）画面
        ListCombinationMatch, // マッチスキル一覧画面
        ListCombinationTraining, // トレーニングスキル一覧画面
        ListCombinationCollection, // コレクションスキル一覧画面
        CombinationMatchCharaIcon, // マッチスキル絞り込みの選手アイコン画面
        CombinationMatchAdviserIcon, // マッチスキル絞り込みのアドバイザーアイコン画面
        CombinationTrainingCharaIcon, // トレーニングスキル絞り込みの選手アイコン画面
        CombinationTrainingAdviserIcon, // トレーニングスキル絞り込みのアドバイザーアイコン画面
        CombinationTrainingSpecialSupportIcon, // トレーニングスキル絞り込みのスペシャルサポートカードアイコン画面
        CombinationCollectionCharaIcon, // コレクションスキル絞り込みの選手アイコン画面
        CombinationCollectionAdviserIcon, // コレクションスキル絞り込みのアドバイザーアイコン画面
        CombinationCollectionSpecialSupportIcon, // コレクションスキル絞り込みのスペシャルサポートカードアイコン画面
        PlayerCombinationMatch, // プレイヤーチームの発動マッチスキル一覧画面
        EnemyCombinationMatch, // 相手チームの発動マッチスキル一覧画面
        PlayerCombinationMatchCharaIcon, // プレイヤーチームの発動マッチスキル絞り込みの選手アイコン画面
        EnemyCombinationMatchCharaIcon, // 相手チームの発動マッチスキル絞り込みの選手アイコン画面
    }

    public static SortDataBase GetSortDataByType(SortFilterType type) => type switch
    {
        SortFilterType.None => throw new NotImplementedException(),
        SortFilterType.TrainingSelectBaseCharacter => LocalSaveManager.saveData.trainingSelectBaseCharacterSortFilterData.sortData,
        SortFilterType.TrainingSupportSelectBaseCharacter => LocalSaveManager.saveData.trainingSupportSelectBaseCharacterSortFilterData.sortData,
        SortFilterType.TrainingFriendSelectBaseCharacter => LocalSaveManager.saveData.trainingFriendSelectBaseCharacterSortFilterData.sortData,
        SortFilterType.GrowthLiberationListBaseCharacter => LocalSaveManager.saveData.growthLiberationListBaseCharacterSortFilterData.sortData,
        SortFilterType.ListBaseCharacter => LocalSaveManager.saveData.listBaseCharacterSortFilterData.sortData,
        SortFilterType.FriendBorrowingBaseCharacter => LocalSaveManager.saveData.friendBorrowingBaseCharacterSortFilterData.sortData,
        SortFilterType.ListSuccessCharacter => LocalSaveManager.saveData.listSuccessCharacterSortFilterData.sortData,
        SortFilterType.SellSuccessCharacter => LocalSaveManager.saveData.sellSuccessCharacterSortFilterData.sortData,
        SortFilterType.FavoriteSuccessCharacter => LocalSaveManager.saveData.favoriteSuccessCharacterSortFilterData.sortData,
        SortFilterType.DeckCharacterSelectSuccessCharacter => LocalSaveManager.saveData.deckCharacterSelectSuccessCharacterSortFilterData.sortData,
        SortFilterType.ListSpecialSupportCard => LocalSaveManager.saveData.listSpecialSupportCardSortFilterData.sortData,
        SortFilterType.TrainingSelectSpecialSupportCard => LocalSaveManager.saveData.trainingSelectSpecialSupportCardSortFilterData.sortData,
        SortFilterType.TrainingSelectExtraSupportCard => LocalSaveManager.saveData.trainingSelectExtraSupportCardSortFilterData.sortData,
        SortFilterType.ListSupportEquipment => LocalSaveManager.saveData.listSupportEquipmentSortFilterData.sortData,
        SortFilterType.SellSupportEquipment => LocalSaveManager.saveData.sellSupportEquipmentSortFilterData.sortData,
        SortFilterType.FavoriteSupportEquipment => LocalSaveManager.saveData.favoriteSupportEquipmentSortFilterData.sortData,
        SortFilterType.TrainingSelectSupportEquipment => LocalSaveManager.saveData.trainingSelectSupportEquipmentSortFilterData.sortData,
        SortFilterType.AllSellSupportEquipment => LocalSaveManager.saveData.AllSellSupportEquipmentSortFilterData.sortData,
        SortFilterType.UserIcon => LocalSaveManager.saveData.userIconSortFilterData.sortData,
        SortFilterType.UserTitle => LocalSaveManager.saveData.userTitleSortFilterData.sortData,
        SortFilterType.MyBadge => LocalSaveManager.saveData.myBadgeSortFilterData.sortData,
        SortFilterType.AdviserGrowthLiberationList => LocalSaveManager.saveData.adviserGrowthLiberationSortFilterData.sortData,
        SortFilterType.AdviserList => LocalSaveManager.saveData.adviserListSortFilterData.sortData,
        SortFilterType.TrainingAdviserList => LocalSaveManager.saveData.trainingAdviserListSortFilterData.sortData,
        SortFilterType.AdviserDeckSelect => LocalSaveManager.saveData.adviserDeckListSortFilterData.sortData,
        SortFilterType.ListCombinationMatch => LocalSaveManager.saveData.listCombinationMatchSortFilterData.sortData,
        SortFilterType.ListCombinationTraining => LocalSaveManager.saveData.listCombinationTrainingSortFilterData.sortData,
        SortFilterType.ListCombinationCollection => LocalSaveManager.saveData.listCombinationCollectionSortFilterData.sortData,
        SortFilterType.CombinationMatchCharaIcon => LocalSaveManager.saveData.combinationMatchCharaIconSortFilterData.sortData,
        SortFilterType.CombinationMatchAdviserIcon => LocalSaveManager.saveData.combinationMatchAdviserIconSortFilterData.sortData,
        SortFilterType.CombinationTrainingCharaIcon => LocalSaveManager.saveData.combinationTrainingCharaIconSortFilterData.sortData,
        SortFilterType.CombinationTrainingAdviserIcon => LocalSaveManager.saveData.combinationTrainingAdviserIconSortFilterData.sortData,
        SortFilterType.CombinationTrainingSpecialSupportIcon => LocalSaveManager.saveData.combinationTrainingSpecialSupportIconSortFilterData.sortData,
        SortFilterType.CombinationCollectionCharaIcon => LocalSaveManager.saveData.combinationCollectionCharaIconSortFilterData.sortData,
        SortFilterType.CombinationCollectionAdviserIcon => LocalSaveManager.saveData.combinationCollectionAdviserIconSortFilterData.sortData,
        SortFilterType.CombinationCollectionSpecialSupportIcon => LocalSaveManager.saveData.combinationCollectionSpecialSupportIconSortFilterData.sortData,
        SortFilterType.PlayerCombinationMatch => LocalSaveManager.saveData.playerCombinationMatchSortFilterData.sortData,
        SortFilterType.EnemyCombinationMatch => LocalSaveManager.saveData.enemyCombinationMatchSortFilterData.sortData,
        SortFilterType.PlayerCombinationMatchCharaIcon => LocalSaveManager.saveData.playerCombinationMatchCharaIconSortFilterData.sortData,
        SortFilterType.EnemyCombinationMatchCharaIcon => LocalSaveManager.saveData.enemyCombinationMatchCharaIconSortFilterData.sortData,
        
        _ => throw new NotImplementedException()
    };
    
    public static FilterDataBase GetFilterDataByType(SortFilterType type) => type switch
    {
        SortFilterType.None => throw new NotImplementedException(),
        SortFilterType.TrainingSelectBaseCharacter => LocalSaveManager.saveData.trainingSelectBaseCharacterSortFilterData.filterData,
        SortFilterType.TrainingSupportSelectBaseCharacter => LocalSaveManager.saveData.trainingSupportSelectBaseCharacterSortFilterData.filterData,
        SortFilterType.TrainingFriendSelectBaseCharacter => LocalSaveManager.saveData.trainingFriendSelectBaseCharacterSortFilterData.filterData,
        SortFilterType.GrowthLiberationListBaseCharacter => LocalSaveManager.saveData.growthLiberationListBaseCharacterSortFilterData.filterData,
        SortFilterType.ListBaseCharacter => LocalSaveManager.saveData.listBaseCharacterSortFilterData.filterData,
        SortFilterType.FriendBorrowingBaseCharacter => LocalSaveManager.saveData.friendBorrowingBaseCharacterSortFilterData.filterData,
        SortFilterType.ListSuccessCharacter => LocalSaveManager.saveData.listSuccessCharacterSortFilterData.filterData,
        SortFilterType.SellSuccessCharacter => LocalSaveManager.saveData.sellSuccessCharacterSortFilterData.filterData,
        SortFilterType.FavoriteSuccessCharacter => LocalSaveManager.saveData.favoriteSuccessCharacterSortFilterData.filterData,
        SortFilterType.DeckCharacterSelectSuccessCharacter => LocalSaveManager.saveData.deckCharacterSelectSuccessCharacterSortFilterData.filterData,
        SortFilterType.ListSpecialSupportCard => LocalSaveManager.saveData.listSpecialSupportCardSortFilterData.filterData,
        SortFilterType.TrainingSelectSpecialSupportCard => LocalSaveManager.saveData.trainingSelectSpecialSupportCardSortFilterData.filterData,
        SortFilterType.TrainingSelectExtraSupportCard => LocalSaveManager.saveData.trainingSelectExtraSupportCardSortFilterData.filterData,
        SortFilterType.ListSupportEquipment => LocalSaveManager.saveData.listSupportEquipmentSortFilterData.filterData,
        SortFilterType.SellSupportEquipment => LocalSaveManager.saveData.sellSupportEquipmentSortFilterData.filterData,
        SortFilterType.FavoriteSupportEquipment => LocalSaveManager.saveData.favoriteSupportEquipmentSortFilterData.filterData,
        SortFilterType.TrainingSelectSupportEquipment => LocalSaveManager.saveData.trainingSelectSupportEquipmentSortFilterData.filterData,
        SortFilterType.AllSellSupportEquipment => LocalSaveManager.saveData.AllSellSupportEquipmentSortFilterData.filterData,
        SortFilterType.UserIcon => LocalSaveManager.saveData.userIconSortFilterData.filterData,
        SortFilterType.UserTitle => LocalSaveManager.saveData.userTitleSortFilterData.filterData,
        SortFilterType.MyBadge => LocalSaveManager.saveData.myBadgeSortFilterData.filterData,
        SortFilterType.AdviserGrowthLiberationList => LocalSaveManager.saveData.adviserGrowthLiberationSortFilterData.filterData,
        SortFilterType.AdviserList => LocalSaveManager.saveData.adviserListSortFilterData.filterData,
        SortFilterType.TrainingAdviserList => LocalSaveManager.saveData.trainingAdviserListSortFilterData.filterData,
        SortFilterType.AdviserDeckSelect => LocalSaveManager.saveData.adviserDeckListSortFilterData.filterData,
        SortFilterType.ListCombinationMatch => LocalSaveManager.saveData.listCombinationMatchSortFilterData.filterData,
        SortFilterType.ListCombinationTraining => LocalSaveManager.saveData.listCombinationTrainingSortFilterData.filterData,
        SortFilterType.ListCombinationCollection => LocalSaveManager.saveData.listCombinationCollectionSortFilterData.filterData,
        SortFilterType.CombinationMatchCharaIcon => LocalSaveManager.saveData.combinationMatchCharaIconSortFilterData.filterData,
        SortFilterType.CombinationMatchAdviserIcon => LocalSaveManager.saveData.combinationMatchAdviserIconSortFilterData.filterData,
        SortFilterType.CombinationTrainingCharaIcon => LocalSaveManager.saveData.combinationTrainingCharaIconSortFilterData.filterData,
        SortFilterType.CombinationTrainingAdviserIcon => LocalSaveManager.saveData.combinationTrainingAdviserIconSortFilterData.filterData,
        SortFilterType.CombinationTrainingSpecialSupportIcon => LocalSaveManager.saveData.combinationTrainingSpecialSupportIconSortFilterData.filterData,
        SortFilterType.CombinationCollectionCharaIcon => LocalSaveManager.saveData.combinationCollectionCharaIconSortFilterData.filterData,
        SortFilterType.CombinationCollectionAdviserIcon => LocalSaveManager.saveData.combinationCollectionAdviserIconSortFilterData.filterData,
        SortFilterType.CombinationCollectionSpecialSupportIcon => LocalSaveManager.saveData.combinationCollectionSpecialSupportIconSortFilterData.filterData,
        SortFilterType.PlayerCombinationMatch => LocalSaveManager.saveData.playerCombinationMatchSortFilterData.filterData,
        SortFilterType.EnemyCombinationMatch => LocalSaveManager.saveData.enemyCombinationMatchSortFilterData.filterData,
        SortFilterType.PlayerCombinationMatchCharaIcon => LocalSaveManager.saveData.playerCombinationMatchCharaIconSortFilterData.filterData,
        SortFilterType.EnemyCombinationMatchCharaIcon => LocalSaveManager.saveData.enemyCombinationMatchCharaIconSortFilterData.filterData,
        
        _ => throw new NotImplementedException()
    };

    public static void SaveSortData(SortFilterType type, SortDataBase sortData)
    {
        switch (type)
        {
            case SortFilterType.None:
                break;
            case SortFilterType.TrainingSelectBaseCharacter:
                LocalSaveManager.saveData.trainingSelectBaseCharacterSortFilterData.sortData = sortData as BaseCharacterSortData;
                break;
            case SortFilterType.TrainingSupportSelectBaseCharacter:
                LocalSaveManager.saveData.trainingSupportSelectBaseCharacterSortFilterData.sortData = sortData as BaseCharacterSortData;
                break;
            case SortFilterType.TrainingFriendSelectBaseCharacter:
                LocalSaveManager.saveData.trainingFriendSelectBaseCharacterSortFilterData.sortData = sortData as BaseCharacterSortData;
                break;
            case SortFilterType.GrowthLiberationListBaseCharacter:
                LocalSaveManager.saveData.growthLiberationListBaseCharacterSortFilterData.sortData = sortData as BaseCharacterSortData;
                break;
            case SortFilterType.ListBaseCharacter:
                LocalSaveManager.saveData.listBaseCharacterSortFilterData.sortData = sortData as BaseCharacterSortData;
                break;
            case SortFilterType.FriendBorrowingBaseCharacter:
                LocalSaveManager.saveData.friendBorrowingBaseCharacterSortFilterData.sortData = sortData as BaseCharacterSortData;
                break;
            case SortFilterType.ListSuccessCharacter:
                LocalSaveManager.saveData.listSuccessCharacterSortFilterData.sortData = sortData as SuccessCharacterSortData;
                break;
            case SortFilterType.SellSuccessCharacter:
                LocalSaveManager.saveData.sellSuccessCharacterSortFilterData.sortData = sortData as SuccessCharacterSortData;
                break;
            case SortFilterType.FavoriteSuccessCharacter:
                LocalSaveManager.saveData.favoriteSuccessCharacterSortFilterData.sortData = sortData as SuccessCharacterSortData;
                break;
            case SortFilterType.DeckCharacterSelectSuccessCharacter:
                LocalSaveManager.saveData.deckCharacterSelectSuccessCharacterSortFilterData.sortData = sortData as SuccessCharacterSortData;
                break;
            case SortFilterType.ListSpecialSupportCard:
                LocalSaveManager.saveData.listSpecialSupportCardSortFilterData.sortData = sortData as SpecialSupportCardSortData;
                break;
            case SortFilterType.TrainingSelectSpecialSupportCard:
                LocalSaveManager.saveData.trainingSelectSpecialSupportCardSortFilterData.sortData = sortData as SpecialSupportCardSortData;
                break;
            case SortFilterType.TrainingSelectExtraSupportCard:
                LocalSaveManager.saveData.trainingSelectExtraSupportCardSortFilterData.sortData = sortData as SpecialSupportCardSortData;
                break;
            case SortFilterType.ListSupportEquipment:
                LocalSaveManager.saveData.listSupportEquipmentSortFilterData.sortData = sortData as SupportEquipmentSortData;
                break;
            case SortFilterType.SellSupportEquipment:
                LocalSaveManager.saveData.sellSupportEquipmentSortFilterData.sortData = sortData as SupportEquipmentSortData;
                break;
            case SortFilterType.FavoriteSupportEquipment:
                LocalSaveManager.saveData.favoriteSupportEquipmentSortFilterData.sortData = sortData as SupportEquipmentSortData;
                break;
            case SortFilterType.TrainingSelectSupportEquipment:
                LocalSaveManager.saveData.trainingSelectSupportEquipmentSortFilterData.sortData = sortData as SupportEquipmentSortData;
                break;
            case SortFilterType.UserIcon:
                LocalSaveManager.saveData.userIconSortFilterData.sortData = sortData as UserIconSortData;
                break;
            case SortFilterType.UserTitle:
                LocalSaveManager.saveData.userTitleSortFilterData.sortData = sortData as UserTitleSortData;
                break;
            case SortFilterType.MyBadge:
                LocalSaveManager.saveData.myBadgeSortFilterData.sortData = sortData as MyBadgeSortData;
                break;
            case SortFilterType.AdviserGrowthLiberationList:
                LocalSaveManager.saveData.adviserGrowthLiberationSortFilterData.sortData = sortData as AdviserSortData;
                break;
            case SortFilterType.AdviserList:
                LocalSaveManager.saveData.adviserListSortFilterData.sortData = sortData as AdviserSortData;
                break;
            case SortFilterType.TrainingAdviserList:
                LocalSaveManager.saveData.trainingAdviserListSortFilterData.sortData = sortData as AdviserSortData;
                break;
            case SortFilterType.AdviserDeckSelect:
                LocalSaveManager.saveData.adviserDeckListSortFilterData.sortData = sortData as AdviserSortData;
                break;
            case SortFilterType.ListCombinationMatch:
                LocalSaveManager.saveData.listCombinationMatchSortFilterData.sortData = sortData as CombinationSkillSortData;
                break;
            case SortFilterType.ListCombinationTraining:
                LocalSaveManager.saveData.listCombinationTrainingSortFilterData.sortData = sortData as CombinationSkillSortData;
                break;
            case SortFilterType.ListCombinationCollection:
                LocalSaveManager.saveData.listCombinationCollectionSortFilterData.sortData = sortData as CombinationSkillSortData;
                break;
            case SortFilterType.CombinationMatchCharaIcon:
                LocalSaveManager.saveData.combinationMatchCharaIconSortFilterData.sortData = sortData as CombinationSkillCharaIconSortData;
                break;
            case SortFilterType.CombinationMatchAdviserIcon:
                LocalSaveManager.saveData.combinationMatchAdviserIconSortFilterData.sortData = sortData as CombinationSkillCharaIconSortData;
                break;
            case SortFilterType.CombinationTrainingCharaIcon:
                LocalSaveManager.saveData.combinationTrainingCharaIconSortFilterData.sortData = sortData as CombinationSkillCharaIconSortData;
                break;
            case SortFilterType.CombinationTrainingAdviserIcon:
                LocalSaveManager.saveData.combinationTrainingAdviserIconSortFilterData.sortData = sortData as CombinationSkillCharaIconSortData;
                break;
            case SortFilterType.CombinationTrainingSpecialSupportIcon:
                LocalSaveManager.saveData.combinationTrainingSpecialSupportIconSortFilterData.sortData = sortData as CombinationSkillCharaIconSortData;
                break;
            case SortFilterType.CombinationCollectionCharaIcon:
                LocalSaveManager.saveData.combinationCollectionCharaIconSortFilterData.sortData = sortData as CombinationSkillCharaIconSortData;
                break;
            case SortFilterType.CombinationCollectionAdviserIcon:
                LocalSaveManager.saveData.combinationCollectionAdviserIconSortFilterData.sortData = sortData as CombinationSkillCharaIconSortData;
                break;
            case SortFilterType.CombinationCollectionSpecialSupportIcon:
                LocalSaveManager.saveData.combinationCollectionSpecialSupportIconSortFilterData.sortData = sortData as CombinationSkillCharaIconSortData;
                break;
            case SortFilterType.PlayerCombinationMatch:
                LocalSaveManager.saveData.playerCombinationMatchSortFilterData.sortData = sortData as CombinationSkillSortData;
                break;
            case SortFilterType.EnemyCombinationMatch:
                LocalSaveManager.saveData.enemyCombinationMatchSortFilterData.sortData = sortData as CombinationSkillSortData;
                break;
            case SortFilterType.PlayerCombinationMatchCharaIcon:
                LocalSaveManager.saveData.playerCombinationMatchCharaIconSortFilterData.sortData = sortData as CombinationSkillCharaIconSortData;
                break;
            case SortFilterType.EnemyCombinationMatchCharaIcon:
                LocalSaveManager.saveData.enemyCombinationMatchCharaIconSortFilterData.sortData = sortData as CombinationSkillCharaIconSortData;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    
    
    public static void SaveFilterData(SortFilterType type, FilterDataBase filterData)
    {
        switch (type)
        {
            case SortFilterType.None:
                break;
            case SortFilterType.TrainingSelectBaseCharacter:
                LocalSaveManager.saveData.trainingSelectBaseCharacterSortFilterData.filterData = filterData as BaseCharacterFilterData;
                break;
            case SortFilterType.TrainingSupportSelectBaseCharacter:
                LocalSaveManager.saveData.trainingSupportSelectBaseCharacterSortFilterData.filterData = filterData as BaseCharacterFilterData;
                break;
            case SortFilterType.TrainingFriendSelectBaseCharacter:
                LocalSaveManager.saveData.trainingFriendSelectBaseCharacterSortFilterData.filterData = filterData as BaseCharacterFilterData;
                break;
            case SortFilterType.GrowthLiberationListBaseCharacter:
                LocalSaveManager.saveData.growthLiberationListBaseCharacterSortFilterData.filterData = filterData as BaseCharacterFilterData;
                break;
            case SortFilterType.ListBaseCharacter:
                LocalSaveManager.saveData.listBaseCharacterSortFilterData.filterData = filterData as BaseCharacterFilterData;
                break;
            case SortFilterType.FriendBorrowingBaseCharacter:
                LocalSaveManager.saveData.friendBorrowingBaseCharacterSortFilterData.filterData = filterData as BaseCharacterFilterData;
                break;
            case SortFilterType.ListSuccessCharacter:
                LocalSaveManager.saveData.listSuccessCharacterSortFilterData.filterData = filterData as SuccessCharacterFilterData;
                break;
            case SortFilterType.SellSuccessCharacter:
                LocalSaveManager.saveData.sellSuccessCharacterSortFilterData.filterData = filterData as SuccessCharacterFilterData;
                break;
            case SortFilterType.FavoriteSuccessCharacter:
                LocalSaveManager.saveData.favoriteSuccessCharacterSortFilterData.filterData = filterData as SuccessCharacterFilterData;
                break;
            case SortFilterType.DeckCharacterSelectSuccessCharacter:
                LocalSaveManager.saveData.deckCharacterSelectSuccessCharacterSortFilterData.filterData = filterData as SuccessCharacterFilterData;
                break;
            case SortFilterType.ListSpecialSupportCard:
                LocalSaveManager.saveData.listSpecialSupportCardSortFilterData.filterData = filterData as SpecialSupportCardFilterData;
                break;
            case SortFilterType.TrainingSelectSpecialSupportCard:
                LocalSaveManager.saveData.trainingSelectSpecialSupportCardSortFilterData.filterData = filterData as SpecialSupportCardFilterData;
                break;
            case SortFilterType.TrainingSelectExtraSupportCard:
                LocalSaveManager.saveData.trainingSelectExtraSupportCardSortFilterData.filterData = filterData as SpecialSupportCardFilterData;
                break;
            case SortFilterType.ListSupportEquipment:
                LocalSaveManager.saveData.listSupportEquipmentSortFilterData.filterData = filterData as SupportEquipmentFilterData;
                break;
            case SortFilterType.SellSupportEquipment:
                LocalSaveManager.saveData.sellSupportEquipmentSortFilterData.filterData = filterData as SupportEquipmentFilterData;
                break;
            case SortFilterType.FavoriteSupportEquipment:
                LocalSaveManager.saveData.favoriteSupportEquipmentSortFilterData.filterData = filterData as SupportEquipmentFilterData;
                break;
            case SortFilterType.TrainingSelectSupportEquipment:
                LocalSaveManager.saveData.trainingSelectSupportEquipmentSortFilterData.filterData = filterData as SupportEquipmentFilterData;
                break;
            case SortFilterType.AllSellSupportEquipment:
                LocalSaveManager.saveData.AllSellSupportEquipmentSortFilterData.filterData = filterData as SupportEquipmentFilterData;
                break;
            case SortFilterType.UserIcon:
                LocalSaveManager.saveData.userIconSortFilterData.filterData = filterData as UserIconFilterData;
                break;
            case SortFilterType.UserTitle:
                LocalSaveManager.saveData.userTitleSortFilterData.filterData = filterData as UserTitleFilterData;
                break;
            case SortFilterType.MyBadge:
                LocalSaveManager.saveData.myBadgeSortFilterData.filterData = filterData as MyBadgeFilterData;
                break;
            case SortFilterType.AdviserGrowthLiberationList:
                LocalSaveManager.saveData.adviserGrowthLiberationSortFilterData.filterData = filterData as AdviserFilterData;
                break;
            case SortFilterType.AdviserList:
                LocalSaveManager.saveData.adviserListSortFilterData.filterData = filterData as AdviserFilterData;
                break;
            case SortFilterType.TrainingAdviserList:
                LocalSaveManager.saveData.trainingAdviserListSortFilterData.filterData = filterData as AdviserFilterData;
                break;
            case SortFilterType.AdviserDeckSelect:
                LocalSaveManager.saveData.adviserDeckListSortFilterData.filterData = filterData as AdviserFilterData;
                break;
            case SortFilterType.ListCombinationMatch:
                LocalSaveManager.saveData.listCombinationMatchSortFilterData.filterData = filterData as CombinationSkillFilterData;
                break;
            case SortFilterType.ListCombinationTraining:
                LocalSaveManager.saveData.listCombinationTrainingSortFilterData.filterData = filterData as CombinationSkillFilterData;
                break;
            case SortFilterType.ListCombinationCollection:
                LocalSaveManager.saveData.listCombinationCollectionSortFilterData.filterData = filterData as CombinationSkillFilterData;
                break;
            case SortFilterType.CombinationMatchCharaIcon:
                LocalSaveManager.saveData.combinationMatchCharaIconSortFilterData.filterData = filterData as CombinationSkillCharaIconFilterData;
                break;
            case SortFilterType.CombinationMatchAdviserIcon:
                LocalSaveManager.saveData.combinationMatchAdviserIconSortFilterData.filterData = filterData as CombinationSkillCharaIconFilterData;
                break;
            case SortFilterType.CombinationTrainingCharaIcon:
                LocalSaveManager.saveData.combinationTrainingCharaIconSortFilterData.filterData = filterData as CombinationSkillCharaIconFilterData;
                break;
            case SortFilterType.CombinationTrainingAdviserIcon:
                LocalSaveManager.saveData.combinationTrainingAdviserIconSortFilterData.filterData = filterData as CombinationSkillCharaIconFilterData;
                break;
            case SortFilterType.CombinationTrainingSpecialSupportIcon:
                LocalSaveManager.saveData.combinationTrainingSpecialSupportIconSortFilterData.filterData = filterData as CombinationSkillCharaIconFilterData;
                break;
            case SortFilterType.CombinationCollectionCharaIcon:
                LocalSaveManager.saveData.combinationCollectionCharaIconSortFilterData.filterData = filterData as CombinationSkillCharaIconFilterData;
                break;
            case SortFilterType.CombinationCollectionAdviserIcon:
                LocalSaveManager.saveData.combinationCollectionAdviserIconSortFilterData.filterData = filterData as CombinationSkillCharaIconFilterData;
                break;
            case SortFilterType.CombinationCollectionSpecialSupportIcon:
                LocalSaveManager.saveData.combinationCollectionSpecialSupportIconSortFilterData.filterData = filterData as CombinationSkillCharaIconFilterData;
                break;
            case SortFilterType.PlayerCombinationMatch:
                LocalSaveManager.saveData.playerCombinationMatchSortFilterData.filterData = filterData as CombinationSkillFilterData;
                break;
            case SortFilterType.EnemyCombinationMatch:
                LocalSaveManager.saveData.enemyCombinationMatchSortFilterData.filterData = filterData as CombinationSkillFilterData;
                break;
            case SortFilterType.PlayerCombinationMatchCharaIcon:
                LocalSaveManager.saveData.playerCombinationMatchCharaIconSortFilterData.filterData = filterData as CombinationSkillCharaIconFilterData;
                break;
            case SortFilterType.EnemyCombinationMatchCharaIcon:
                LocalSaveManager.saveData.enemyCombinationMatchCharaIconSortFilterData.filterData = filterData as CombinationSkillCharaIconFilterData;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    
    public static bool IsFilter(SortFilterType type)
    {
        switch (type)
        {
            case SortFilterType.None:
                return false;
            case SortFilterType.TrainingSelectBaseCharacter:
                var trainingSelectBaseCharacterFilterData = LocalSaveManager.saveData.trainingSelectBaseCharacterSortFilterData.filterData;
                return trainingSelectBaseCharacterFilterData != null && (trainingSelectBaseCharacterFilterData.rarityList.Count > 0 || trainingSelectBaseCharacterFilterData.typeList.Count > 0);
            case SortFilterType.TrainingSupportSelectBaseCharacter:
                var trainingSupportSelectBaseCharacterFilterData = LocalSaveManager.saveData.trainingSupportSelectBaseCharacterSortFilterData.filterData;
                return trainingSupportSelectBaseCharacterFilterData != null && (trainingSupportSelectBaseCharacterFilterData.rarityList.Count > 0 || trainingSupportSelectBaseCharacterFilterData.typeList.Count > 0);
            case SortFilterType.TrainingFriendSelectBaseCharacter:
                var trainingFriendSelectBaseCharacterFilterData = LocalSaveManager.saveData.trainingFriendSelectBaseCharacterSortFilterData.filterData;
                return trainingFriendSelectBaseCharacterFilterData != null && (trainingFriendSelectBaseCharacterFilterData.rarityList.Count > 0 || trainingFriendSelectBaseCharacterFilterData.typeList.Count > 0);
            case SortFilterType.GrowthLiberationListBaseCharacter:
                var growthLiberationListBaseCharacterFilterData = LocalSaveManager.saveData.growthLiberationListBaseCharacterSortFilterData.filterData;
                return growthLiberationListBaseCharacterFilterData != null && (growthLiberationListBaseCharacterFilterData.rarityList.Count > 0 || growthLiberationListBaseCharacterFilterData.typeList.Count > 0);
            case SortFilterType.ListBaseCharacter:
                var listBaseCharacterFilterData = LocalSaveManager.saveData.listBaseCharacterSortFilterData.filterData;
                return listBaseCharacterFilterData != null && (listBaseCharacterFilterData.rarityList.Count > 0 || listBaseCharacterFilterData.typeList.Count > 0);
            case SortFilterType.FriendBorrowingBaseCharacter:
                var friendBorrowingBaseCharacterFilterData = LocalSaveManager.saveData.friendBorrowingBaseCharacterSortFilterData.filterData;
                return friendBorrowingBaseCharacterFilterData != null && (friendBorrowingBaseCharacterFilterData.rarityList.Count > 0 || friendBorrowingBaseCharacterFilterData.typeList.Count > 0);
            case SortFilterType.ListSuccessCharacter:
                var listSuccessCharacterFilterData = LocalSaveManager.saveData.listSuccessCharacterSortFilterData.filterData;
                return listSuccessCharacterFilterData != null && (listSuccessCharacterFilterData.rankList.Count > 0 || listSuccessCharacterFilterData.favoriteType == SuccessCharacterFilterData.FavoriteType.Favorite);
            case SortFilterType.SellSuccessCharacter:
                var sellSuccessCharacterFilterData = LocalSaveManager.saveData.sellSuccessCharacterSortFilterData.filterData;
                return sellSuccessCharacterFilterData != null && (sellSuccessCharacterFilterData.rankList.Count > 0 || sellSuccessCharacterFilterData.favoriteType == SuccessCharacterFilterData.FavoriteType.Favorite);
            case SortFilterType.FavoriteSuccessCharacter:
                var favoriteSuccessCharacterFilterData = LocalSaveManager.saveData.favoriteSuccessCharacterSortFilterData.filterData;
                return favoriteSuccessCharacterFilterData != null && (favoriteSuccessCharacterFilterData.rankList.Count > 0 || favoriteSuccessCharacterFilterData.favoriteType == SuccessCharacterFilterData.FavoriteType.Favorite);
            case SortFilterType.DeckCharacterSelectSuccessCharacter:
                var deckCharacterSelectSuccessCharacterFilterData = LocalSaveManager.saveData.deckCharacterSelectSuccessCharacterSortFilterData.filterData;
                return deckCharacterSelectSuccessCharacterFilterData != null && (deckCharacterSelectSuccessCharacterFilterData.rankList.Count > 0 || deckCharacterSelectSuccessCharacterFilterData.favoriteType == SuccessCharacterFilterData.FavoriteType.Favorite);
            case SortFilterType.ListSpecialSupportCard:
                var listSpecialSupportCardFilterData = LocalSaveManager.saveData.listSpecialSupportCardSortFilterData.filterData;
                return listSpecialSupportCardFilterData != null && (listSpecialSupportCardFilterData.rarityList.Count > 0 || listSpecialSupportCardFilterData.typeList.Count > 0 || listSpecialSupportCardFilterData.extraList.Count > 0);
            case SortFilterType.TrainingSelectSpecialSupportCard:
                var trainingSelectSpecialSupportCardFilterData = LocalSaveManager.saveData.trainingSelectSpecialSupportCardSortFilterData.filterData;
                return trainingSelectSpecialSupportCardFilterData != null && (trainingSelectSpecialSupportCardFilterData.rarityList.Count > 0 || trainingSelectSpecialSupportCardFilterData.typeList.Count > 0);
            case SortFilterType.TrainingSelectExtraSupportCard:
                var trainingSelectExtraSupportCardFilterData = LocalSaveManager.saveData.trainingSelectExtraSupportCardSortFilterData.filterData;
                return trainingSelectExtraSupportCardFilterData != null && (trainingSelectExtraSupportCardFilterData.rarityList.Count > 0 || trainingSelectExtraSupportCardFilterData.typeList.Count > 0);
            case SortFilterType.ListSupportEquipment:
                var listSupportEquipmentFilterData = LocalSaveManager.saveData.listSupportEquipmentSortFilterData.filterData;
                return listSupportEquipmentFilterData != null && (listSupportEquipmentFilterData.iconTypeList.Count > 0 || listSupportEquipmentFilterData.rarityList.Count > 0 || listSupportEquipmentFilterData.typeList.Count > 0 || listSupportEquipmentFilterData.practiceSkillList.Count > 0);
            case SortFilterType.SellSupportEquipment:
                var sellSupportEquipmentFilterData = LocalSaveManager.saveData.sellSupportEquipmentSortFilterData.filterData;
                return sellSupportEquipmentFilterData != null && (sellSupportEquipmentFilterData.iconTypeList.Count > 0 || sellSupportEquipmentFilterData.rarityList.Count > 0 || sellSupportEquipmentFilterData.typeList.Count > 0 || sellSupportEquipmentFilterData.practiceSkillList.Count > 0);
            case SortFilterType.FavoriteSupportEquipment:
                var favoriteSupportEquipmentFilterData = LocalSaveManager.saveData.favoriteSupportEquipmentSortFilterData.filterData;
                return favoriteSupportEquipmentFilterData != null && (favoriteSupportEquipmentFilterData.iconTypeList.Count > 0 || favoriteSupportEquipmentFilterData.rarityList.Count > 0 || favoriteSupportEquipmentFilterData.typeList.Count > 0 || favoriteSupportEquipmentFilterData.practiceSkillList.Count > 0);
            case SortFilterType.TrainingSelectSupportEquipment:
                var trainingSelectSupportEquipmentFilterData = LocalSaveManager.saveData.trainingSelectSupportEquipmentSortFilterData.filterData;
                return trainingSelectSupportEquipmentFilterData != null && (trainingSelectSupportEquipmentFilterData.iconTypeList.Count > 0 || trainingSelectSupportEquipmentFilterData.rarityList.Count > 0 || trainingSelectSupportEquipmentFilterData.typeList.Count > 0 || trainingSelectSupportEquipmentFilterData.practiceSkillList.Count > 0);
            case SortFilterType.UserIcon:
                UserIconFilterData userIconFilterData = LocalSaveManager.saveData.userIconSortFilterData.filterData;
                return userIconFilterData != null && userIconFilterData.rarityList.Count > 0;
            case SortFilterType.UserTitle:
                UserTitleFilterData userTitleFilterData = LocalSaveManager.saveData.userTitleSortFilterData.filterData;
                return userTitleFilterData != null && userTitleFilterData.rarityList.Count > 0;
            case SortFilterType.MyBadge:
                MyBadgeFilterData myBadgeFilterData = LocalSaveManager.saveData.myBadgeSortFilterData.filterData;
                return myBadgeFilterData != null && myBadgeFilterData.rarityList.Count > 0;
            case SortFilterType.AdviserGrowthLiberationList:
                AdviserFilterData adviserGrowthFilterData  = LocalSaveManager.saveData.adviserGrowthLiberationSortFilterData.filterData;
                return adviserGrowthFilterData != null && (adviserGrowthFilterData.rarityList.Count > 0 || adviserGrowthFilterData.typeList.Count > 0);
            case SortFilterType.AdviserList:
                AdviserFilterData adviserListFilterData  = LocalSaveManager.saveData.adviserListSortFilterData.filterData;
                return adviserListFilterData != null && (adviserListFilterData.rarityList.Count > 0 || adviserListFilterData.typeList.Count > 0);
            case SortFilterType.TrainingAdviserList:
                AdviserFilterData trainingAdviserListFilterData  = LocalSaveManager.saveData.trainingAdviserListSortFilterData.filterData;
                return trainingAdviserListFilterData != null && (trainingAdviserListFilterData.rarityList.Count > 0 || trainingAdviserListFilterData.typeList.Count > 0);
            case SortFilterType.AdviserDeckSelect:
                AdviserFilterData adviserDeckSelectFilterData  = LocalSaveManager.saveData.adviserDeckListSortFilterData.filterData;
                return adviserDeckSelectFilterData != null && (adviserDeckSelectFilterData.rarityList.Count > 0 || adviserDeckSelectFilterData.typeList.Count > 0);
            case SortFilterType.ListCombinationMatch:
                CombinationSkillFilterData combinationMatchFilterData = LocalSaveManager.saveData.listCombinationMatchSortFilterData.filterData;
                return combinationMatchFilterData != null && combinationMatchFilterData.filterType != SelectFilterType.Off && combinationMatchFilterData.selectedCharaIdList.Count > 0;
            case SortFilterType.ListCombinationTraining:
                CombinationSkillFilterData combinationTrainingFilterData = LocalSaveManager.saveData.listCombinationTrainingSortFilterData.filterData;
                return combinationTrainingFilterData != null && combinationTrainingFilterData.filterType != SelectFilterType.Off && combinationTrainingFilterData.selectedCharaIdList.Count > 0;
            case SortFilterType.ListCombinationCollection:
                CombinationSkillFilterData combinationCollectionFilterData = LocalSaveManager.saveData.listCombinationCollectionSortFilterData.filterData;
                return combinationCollectionFilterData != null && combinationCollectionFilterData.filterType != SelectFilterType.Off && combinationCollectionFilterData.selectedCharaIdList.Count > 0;
            case SortFilterType.CombinationMatchCharaIcon:
                CombinationSkillCharaIconFilterData combinationMatchCharaIconFilterData = LocalSaveManager.saveData.combinationMatchCharaIconSortFilterData.filterData;
                return combinationMatchCharaIconFilterData != null && combinationMatchCharaIconFilterData.selectedRarityList.Count > 0;
            case SortFilterType.CombinationMatchAdviserIcon:
                CombinationSkillCharaIconFilterData combinationMatchAdviserIconFilterData = LocalSaveManager.saveData.combinationMatchAdviserIconSortFilterData.filterData;
                return combinationMatchAdviserIconFilterData != null && combinationMatchAdviserIconFilterData.selectedRarityList.Count > 0;
            case SortFilterType.CombinationTrainingCharaIcon:
                CombinationSkillCharaIconFilterData combinationTrainingCharaIconFilterData = LocalSaveManager.saveData.combinationTrainingCharaIconSortFilterData.filterData;
                return combinationTrainingCharaIconFilterData != null && combinationTrainingCharaIconFilterData.selectedRarityList.Count > 0;
            case SortFilterType.CombinationTrainingAdviserIcon:
                CombinationSkillCharaIconFilterData combinationTrainingAdviserIconFilterData = LocalSaveManager.saveData.combinationTrainingAdviserIconSortFilterData.filterData;
                return combinationTrainingAdviserIconFilterData != null && combinationTrainingAdviserIconFilterData.selectedRarityList.Count > 0;
            case SortFilterType.CombinationTrainingSpecialSupportIcon:
                CombinationSkillCharaIconFilterData combinationTrainingSpecialSupportIconFilterData = LocalSaveManager.saveData.combinationTrainingSpecialSupportIconSortFilterData.filterData;
                return combinationTrainingSpecialSupportIconFilterData != null && combinationTrainingSpecialSupportIconFilterData.selectedRarityList.Count > 0;
            case SortFilterType.CombinationCollectionCharaIcon:
                CombinationSkillCharaIconFilterData combinationCollectionCharaIconFilterData = LocalSaveManager.saveData.combinationCollectionCharaIconSortFilterData.filterData;
                return combinationCollectionCharaIconFilterData != null && combinationCollectionCharaIconFilterData.selectedRarityList.Count > 0;
            case SortFilterType.CombinationCollectionAdviserIcon:
                CombinationSkillCharaIconFilterData combinationCollectionAdviserIconFilterData = LocalSaveManager.saveData.combinationCollectionAdviserIconSortFilterData.filterData;
                return combinationCollectionAdviserIconFilterData != null && combinationCollectionAdviserIconFilterData.selectedRarityList.Count > 0;
            case SortFilterType.CombinationCollectionSpecialSupportIcon:
                CombinationSkillCharaIconFilterData combinationCollectionSpecialSupportIconFilterData = LocalSaveManager.saveData.combinationCollectionSpecialSupportIconSortFilterData.filterData;
                return combinationCollectionSpecialSupportIconFilterData != null && combinationCollectionSpecialSupportIconFilterData.selectedRarityList.Count > 0;
            case SortFilterType.PlayerCombinationMatch:
                CombinationSkillFilterData playerCombinationMatchFilterData = LocalSaveManager.saveData.playerCombinationMatchSortFilterData.filterData;
                return playerCombinationMatchFilterData != null && playerCombinationMatchFilterData.filterType != SelectFilterType.Off && playerCombinationMatchFilterData.selectedCharaIdList.Count > 0;
            case SortFilterType.EnemyCombinationMatch:
                CombinationSkillFilterData enemyCombinationMatchFilterData = LocalSaveManager.saveData.enemyCombinationMatchSortFilterData.filterData;
                return enemyCombinationMatchFilterData != null && enemyCombinationMatchFilterData.filterType != SelectFilterType.Off && enemyCombinationMatchFilterData.selectedCharaIdList.Count > 0;
            case SortFilterType.PlayerCombinationMatchCharaIcon:
                CombinationSkillCharaIconFilterData playerCombinationMatchCharaIconFilterData = LocalSaveManager.saveData.playerCombinationMatchCharaIconSortFilterData.filterData;
                return playerCombinationMatchCharaIconFilterData != null && playerCombinationMatchCharaIconFilterData.selectedRarityList.Count > 0;
            case SortFilterType.EnemyCombinationMatchCharaIcon:
                CombinationSkillCharaIconFilterData enemyCombinationMatchCharaIconFilterData = LocalSaveManager.saveData.enemyCombinationMatchCharaIconSortFilterData.filterData;
                return enemyCombinationMatchCharaIconFilterData != null && enemyCombinationMatchCharaIconFilterData.selectedRarityList.Count > 0;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public static OrderType GetReversalOrderType(OrderType type)
    {
        switch (type)
        {
            case OrderType.Ascending:
                return OrderType.Descending;
            case OrderType.Descending:
                return OrderType.Ascending;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
