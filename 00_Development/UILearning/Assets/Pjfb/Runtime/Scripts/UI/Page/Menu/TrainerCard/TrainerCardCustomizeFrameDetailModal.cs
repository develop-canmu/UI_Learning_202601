
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pjfb
{
    public class TrainerCardCustomizeFrameDetailModal : ModalWindow
    {
        public class ModalData
        {
            public long Id { get; }
            
            public ModalData(long id)
            {
                Id = id;
            }
        }
        
        [SerializeField]
        private TrainerCardCustomizeThumbnailImage customizeImage;
        
        [SerializeField]
        private TextMeshProUGUI nameText;
        
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            ModalData data = (ModalData) args;
            
            ProfileFrameMasterObject master = MasterManager.Instance.profileFrameMaster.FindData(data.Id);
            
            // 画像設定
            await customizeImage.SetTextureAsync(master.thumbnailImageId);
            
            // テキスト設定
            nameText.text = master.name;
            descriptionText.text = master.description;
            
            await base.OnPreOpen(args, token);
        }
    }
}