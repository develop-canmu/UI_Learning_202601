using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb.Character
{
    public class CharacterStatusTypeView : MonoBehaviour
    {
        [SerializeField] private CharacterCharacterTypeImage typeImage = null;
        [SerializeField] private TMPro.TMP_Text typeText = null;

        /// <summary>キャラクタを表示</summary>
        public async UniTask SetCharacter(long charaId)
        {
            var mChar = MasterManager.Instance.charaMaster.FindData(charaId);
            
            await typeImage.SetTextureAsync(mChar.charaType);
            typeText.text = mChar.GetCharacterTypeName();
        }
    }
}

