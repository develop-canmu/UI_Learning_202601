using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using Unity.VisualScripting;

namespace Pjfb.Encyclopedia
{
    public class CharaParentData
    {
        public CharaParentData(CharaParentBase charaParentBase, int possessionCount, int maxCount)
        {
            CharaParentBase = charaParentBase;
            PossessionCount = possessionCount;
            MaxCount = maxCount;
            MCharaParent =  MasterManager.Instance.charaParentMaster.values.FirstOrDefault(x =>
                x.parentMCharaId == CharaParentBase.parentMCharaId);
        }

        public readonly CharaParentMasterObject MCharaParent;
        public readonly int PossessionCount;
        public readonly int MaxCount;
        public readonly CharaParentBase CharaParentBase;
    }
    
    public static class CharacterEncyclopediaUtility
    {
        public static async UniTask<Dictionary<long, CharaParentData>> GetCharaParentDataDictionary()
        {
            CharaLibraryGetCharaParentListAPIRequest request = new CharaLibraryGetCharaParentListAPIRequest();
            await APIManager.Instance.Connect(request);

            var charaParentList = request.GetResponseData().charaParentList;

            Dictionary<long, int> parentCharaPossessionCountDictionary = new();
            // ユーザーの所持キャラリスト(名鑑表示対象はキャラクターとアドバイザー)
            List<UserDataChara> userCharaList = new List<UserDataChara>();
            userCharaList.AddRange(UserDataManager.Instance.GetUserDataCharaListByType(CardType.Character));
            userCharaList.AddRange(UserDataManager.Instance.GetUserDataCharaListByType(CardType.Adviser));
            
            foreach (var mChara in userCharaList
                         .DistinctBy(x => x.charaId))
            {
                long key = mChara.MChara.parentMCharaId;
                if (parentCharaPossessionCountDictionary.ContainsKey(key))
                {
                    parentCharaPossessionCountDictionary[key] += 1;
                }
                else
                {
                    parentCharaPossessionCountDictionary.Add(key, 1);
                }
            }

            Dictionary<long, CharaParentData> charaParentDataDictionary = new();
            foreach (var mCharaParent in MasterManager.Instance.charaParentMaster.values)
            {
                if(!mCharaParent.isPlayable || mCharaParent.IsSupportEquipment) continue;
                long parentMCharaId = mCharaParent.parentMCharaId;
                CharaParentBase charaParentBase =
                    charaParentList.FirstOrDefault(x => x.parentMCharaId == parentMCharaId);
                if (charaParentBase is null)
                {
                    charaParentBase = new CharaParentBase()
                    {
                        parentMCharaId = parentMCharaId,
                        trustLevel = 0,
                        trustLevelRead = 0,
                        hasTrustPrize = false,
                        trustExp = 0,
                    };
                }

                int possessionCount = parentCharaPossessionCountDictionary.GetValueOrDefault(parentMCharaId, 0);

                charaParentDataDictionary.Add(parentMCharaId, new CharaParentData(charaParentBase, possessionCount, mCharaParent.mCharaIdList.Length));
            }
            return charaParentDataDictionary;
        }

        /// <summary> 選手名鑑に存在するキャラか </summary>
        public static bool HasEncyclopedia(long mCharaId)
        {
            // 親キャラId
            long parentMCharaId = MasterManager.Instance.charaMaster.FindData(mCharaId).parentMCharaId;
            
            foreach (CharaLibraryProfileMasterObject master in MasterManager.Instance.charaLibraryProfileMaster.values)
            {
                if (master.masterId == parentMCharaId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary> 選手名鑑ボタンを表示できるか </summary>
        public static bool ShowEncyclopediaButton(long mCharaId)
        {
            // キャラを持ってないなら非表示
            if (UserDataManager.Instance.chara.HaveCharacterWithMasterCharaId(mCharaId) == false)
            {
                return false;
            }

            // 選手名鑑に存在するキャラなら表示
            return HasEncyclopedia(mCharaId);
        }
    }
}