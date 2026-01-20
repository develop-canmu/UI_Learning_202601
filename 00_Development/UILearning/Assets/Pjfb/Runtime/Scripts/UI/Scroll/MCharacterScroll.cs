using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CruFramework.UI;
using Pjfb.Encyclopedia;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
    
    public class MCharacterScroll : ItemIconScroll<MCharacterScrollData>
    {
        private long parentMCharaId;
        private HashSet<long> mCharaPossessionHashSet;

        
        
        public void InitializeScroll(long id, HashSet<long> possessionHashSet)
        {
            parentMCharaId = id;
            mCharaPossessionHashSet = possessionHashSet;
            Refresh();
        }
        
        protected override List<MCharacterScrollData> GetItemList()
        {
            var mCharaList = MasterManager.Instance.charaMaster.values.Where(x => x.parentMCharaId == parentMCharaId).OrderBy(x => x.id);
            HashSet<long> availableCharaIdSet = MasterManager.Instance.charaParentMaster
                .FindDataByParentMCharaId(parentMCharaId)?.mCharaIdList.ToHashSet() ?? new HashSet<long>();
            
            
            var result = new List<MCharacterScrollData>();
            foreach (var mChara in mCharaList)
            {
                if (!availableCharaIdSet.Contains(mChara.id)) continue;
                result.Add(new MCharacterScrollData(mChara.id, mCharaPossessionHashSet?.Contains(mChara.id) ?? false));
            }

            return result;
        }

        public void RefreshItemView()
        {
            scrollGrid.RefreshItemView();
        }

        public MCharacterScrollData GetFirstPossessionMCharacterScrollData()
        {
            return scrollGrid.GetItems().Cast<MCharacterScrollData>().FirstOrDefault(x => x.HasCharacter);
        }
    }
}