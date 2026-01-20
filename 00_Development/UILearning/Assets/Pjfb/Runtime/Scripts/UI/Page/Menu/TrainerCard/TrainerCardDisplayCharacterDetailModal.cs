using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;
using UnityEngine.Serialization;

namespace Pjfb
{
    public class TrainerCardDisplayCharacterDetailModal : ModalWindow
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
        private DisplayCharacterImage characterImage;
        
        [SerializeField]
        private TextMeshProUGUI nameText;
        
        [SerializeField]
        private TextMeshProUGUI descriptionText;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            ModalData data = (ModalData) args;
            
            ProfileCharaMasterObject master = MasterManager.Instance.profileCharaMaster.FindData(data.Id);
            
            await characterImage.SetTextureAsync(data.Id);
            
            nameText.text = master.name;
            descriptionText.text = master.description;
            
            await base.OnPreOpen(args, token);
        }
    }
}