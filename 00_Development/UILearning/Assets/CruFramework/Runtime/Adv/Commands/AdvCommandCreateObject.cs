using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandCreateObject : IAdvCommand, IAdvResource, IAdvCommandSaveNodeId
    {
        [SerializeField][HideInInspector]
        private ulong nodeId = 0;

        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.ObjectDatas))]
        [AdvDocument("生成するオブジェクトId。Idの設定はヘッダーのConfigで設定できます。")]
        public int id = 0;
        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.ObjectLayers))]
        [AdvDocument("生成するレイヤーId。")]
        public int layerId = 0;
        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.Positions))]
        [AdvDocument("生成する座標Id。")]
        public int positionId = 0;

        [SerializeField]
        [AdvDocument("生成する座標のOffset。")]
        public Vector3 offset = Vector3.zero;

        void IAdvCommandSaveNodeId.SetNodeId(ulong nodeId)
        {
            this.nodeId = nodeId;
        }
        
        async UniTask IAdvResource.PreLoadResources(AdvManager manager)
        {
            AdvObjectDataId objectId = manager.Config.ObjectDatas[id];
            AdvObjectCategoryId categoryId = manager.Config.ObjectCategories[objectId.Category];
            await manager.PreLoadResourceAsync(categoryId.ResourceKey, objectId.ResourceId);
        }
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            manager.CreateObject(nodeId, id, layerId, positionId, offset);
        }
    }
}