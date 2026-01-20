using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandActiveTexture : IAdvCommand
    {
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.Textures))]
        [AdvDocument("アクティブを切り替えるテクスチャ。")]
        private int id = 0;

        [SerializeField]
        [AdvDocument("フラグ。")]
        private bool active = false;

        void IAdvCommand.Execute(AdvManager manager)
        {
	        AdvTextureObject texture = manager.GetTextureObject(id);
            if(texture != null)
            {
                texture.gameObject.SetActive(active);
            }
        }
    }
}