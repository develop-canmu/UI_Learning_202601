using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandCreateCharacter : IAdvCommand, IAdvCharacterResource
    {
        
        [System.Serializable]
        private class CreateData
        {
            [SerializeField]
            [AdvObjectId(nameof(AdvConfig.CharacterDatas), AdvObjectIdAttribute.WindowType.SearchWindow)]
            public int id = 0;
            [SerializeField]
            [AdvObjectId(nameof(AdvConfig.Positions))]
            public int positionId = 0;
            [SerializeField]
            public bool isActive = true;
        }
        
        
        [SerializeField]
        [AdvDocument("生成するキャラクタId。複数同時に生成可能。Idの設定はヘッダーのConfigで設定できます。")]
        private CreateData[] datas = null;
        
        
        async UniTask IAdvCharacterResource.PreLoadResources(AdvManager manager, AdvCharacter characterPrefab)
        {
            for(int i=0;i<datas.Length;i++)
            {
                if(manager.Config.CharacterDatas.HasValue(datas[i].id) == false)continue;
                string resourceId = manager.Config.CharacterDatas[datas[i].id].ResourceId;
                if(string.IsNullOrEmpty(resourceId))continue;
                await characterPrefab.PreLoadResource(manager, resourceId);
            }
        }
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            for(int i=0;i<datas.Length;i++)
            {
                manager.CreateCharacter(datas[i].id, datas[i].positionId, datas[i].isActive);
            }
        }
    }
}