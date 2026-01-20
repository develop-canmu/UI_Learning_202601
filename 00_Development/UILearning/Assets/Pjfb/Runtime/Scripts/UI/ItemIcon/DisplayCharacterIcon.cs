using Pjfb.Master;

namespace Pjfb
{
    public class DisplayCharacterIcon : CancellableRawImageWithId
    {
        protected override string GetKey(long id)
        {
            ProfileCharaMasterObject master = MasterManager.Instance.profileCharaMaster.FindData(id);
            // キャラのリソースがあればキャラの画像を表示
            if (master.displayCharaImageId != 0)
            {
                return PageResourceLoadUtility.GetCharacterIconImagePath(master.displayCharaImageId);
            }
            else
            {
                return PageResourceLoadUtility.GetDisplayCharacterIconPath(master.id);
            }
        }
    }
}