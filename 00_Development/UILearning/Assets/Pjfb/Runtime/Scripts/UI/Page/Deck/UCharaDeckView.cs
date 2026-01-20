using Pjfb.Training;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Deck
{
    public abstract class UCharaDeckView : MonoBehaviour
    {
        [SerializeField]
        private DeckLockCharacterView[] characterIcon;

        public void InitView(CharacterDetailData[] characterDetailData)
        {
            for(int i = 0; i < characterIcon.Length; i++)
            {
                // characterDataのnullチェック
                long uCharaId = characterDetailData[i] == null ? DeckUtility.EmptyDeckSlotId : characterDetailData[i].UCharId;
                
                // 各アイコンのセット
                int systemNumber = GetLockNum(i);
                characterIcon[i].SetUserCharacterId(uCharaId);
                characterIcon[i].SetLockNumber(systemNumber);
                characterIcon[i].SetLockState(UserDataManager.Instance.IsUnlockSystem(systemNumber) == false);
                
                characterIcon[i].ClearLabel();
            }
        }
        
        // デッキスロットのロック番号を取得
        protected abstract int GetLockNum(int slotNum);

    }
}