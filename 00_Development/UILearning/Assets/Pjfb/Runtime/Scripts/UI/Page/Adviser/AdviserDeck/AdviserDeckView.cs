using Pjfb.Master;
using Pjfb.SystemUnlock;
using Pjfb.UserData;

namespace Pjfb.Deck
{
    public class AdviserDeckView : UCharaDeckView
    {
        protected override int GetLockNum(int slotNum)
        {
            return (int)SystemUnlockDataManager.SystemUnlockNumber.ClubRoyalAdviser + slotNum;
        }
    }
}