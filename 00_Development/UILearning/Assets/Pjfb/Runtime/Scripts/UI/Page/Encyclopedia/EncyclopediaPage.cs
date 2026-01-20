using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Storage;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Encyclopedia
{
    public enum EncyclopediaPageType
    {
        EncyclopediaList,
        EncyclopediaDetail,
    }


    [Serializable]
    public class VoiceBadgeReadData
    {
        public VoiceBadgeReadData(long parentMCharaId)
        {
            this.parentMCharaId = parentMCharaId;
            this.gameVoiceReadLevel = 0;
            this.othersVoiceReadLevel = 0;
        }
        
        public VoiceBadgeReadData(long parentMCharaId, long gameVoiceReadLevel, long othersVoiceReadLevel)
        {
            this.parentMCharaId = parentMCharaId;
            this.gameVoiceReadLevel = gameVoiceReadLevel;
            this.othersVoiceReadLevel = othersVoiceReadLevel;
        }
        public long parentMCharaId;
        public long gameVoiceReadLevel;
        public long othersVoiceReadLevel;
    }
    
    public class EncyclopediaPage : PageManager<EncyclopediaPageType>
    {
        public static Dictionary<long, VoiceBadgeReadData> VoiceBadgeReadDictionary;
        public static Dictionary<long, CharaParentData> CharaParentDataDictionary;
        public static HashSet<long> MCharaPossessionHashSet;
        private static List<long> availableIdList;
        public static IReadOnlyList<long> AvailableIdList => availableIdList.AsReadOnly();
 
        public bool IsPageOpened { get; private set; }
        
        protected override string GetAddress(EncyclopediaPageType page)
        {
            return $"Prefabs/UI/Page/Encyclopedia/{page}Page.prefab";
        }

        public static void OpenPage(bool stack ,object args)
        {
            var pageType = PageType.Encyclopedia;
            if (AppManager.Instance.TutorialManager.OpenScenarioTutorial(pageType, stack, args)) return;
            AppManager.Instance.UIManager.PageManager.OpenPage(pageType, stack, args);
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {

            IsPageOpened = false;
            ClearPageStack();

            MCharaPossessionHashSet = UserDataManager.Instance.chara.data.Values.Select(x => (long)x.charaId).ToHashSet();
            CharaParentDataDictionary = await CharacterEncyclopediaUtility.GetCharaParentDataDictionary();

            availableIdList = CharaParentDataDictionary.Where(x => x.Value.PossessionCount > 0)
                .OrderBy(x => x.Value.MCharaParent.sortNumber)
                .Select(x => x.Key).ToList();

            SetVoiceBadgeReadDictionary();
            
            if (args != null)
            {
                long id = (long)args;
                int index = GetIndexByCharaParentId(id);
                if(index >= 0)
                    await OpenPageAsync(EncyclopediaPageType.EncyclopediaDetail, true, index, token);
                else
                    await OpenPageAsync(EncyclopediaPageType.EncyclopediaList, true, null, token);
            }
            else
            {
                await OpenPageAsync(EncyclopediaPageType.EncyclopediaList, true, null, token);
            }
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            IsPageOpened = true;
        }

        public static int GetIndexByCharaParentId(long id)
        {
            return availableIdList.FindIndex(x => x == id);
        }

        public static CharaParentData GetCharaParentDataByIndex(int index)
        {
            return CharaParentDataDictionary[AvailableIdList[index]];
        }

        private static void SetVoiceBadgeReadDictionary()
        {
            bool saveData = false;
            VoiceBadgeReadDictionary = LocalSaveManager.saveData.voiceBadgeReadData.ToDictionary(x => x.parentMCharaId, x => x);
            foreach (var parentMCharaId in availableIdList)
            {
                if (VoiceBadgeReadDictionary.ContainsKey(parentMCharaId)) continue;
                var voiceReadData = new VoiceBadgeReadData(parentMCharaId);
                LocalSaveManager.saveData.voiceBadgeReadData.Add(voiceReadData);
                VoiceBadgeReadDictionary.Add(parentMCharaId, voiceReadData);
                saveData = true;
            }

            if (saveData)
            {
                LocalSaveManager.Instance.SaveData();
            }
        }
    }
}


