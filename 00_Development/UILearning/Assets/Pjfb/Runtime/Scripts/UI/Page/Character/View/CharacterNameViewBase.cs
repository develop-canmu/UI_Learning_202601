using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Character
{
    public abstract class CharacterNameViewBase : MonoBehaviour
    {
        //Todo:CharacterIconの様なアイコンprefabで管理するようであれば削除と追加対応が必要
        [SerializeField] private TextMeshProUGUI nickNameText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private CharacterCardImage characterCardImage;

        protected virtual void InitializeUIByMChara(CharaMasterObject mChara)
        {
            InitializeUIByMCharaAsync(mChara).Forget();   
        }
        
        protected virtual async UniTask InitializeUIByMCharaAsync(CharaMasterObject mChara)
        {
            if (nickNameText is not null)
            {
                if (string.IsNullOrEmpty(mChara.nickname))
                {
                    nickNameText.gameObject.SetActive(false);
                }
                else
                {
                    nickNameText.gameObject.SetActive(true);
                    nickNameText.text = mChara.nickname;    
                }    
            }
            
            
            nameText.text = mChara.name;

            if (characterCardImage != null)
            {
                await characterCardImage.SetTextureAsync(mChara.id);
            }
        }
    }

}
