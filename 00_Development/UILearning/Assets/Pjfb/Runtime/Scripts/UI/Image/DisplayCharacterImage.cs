using Pjfb.Master;

namespace Pjfb
{
    public class DisplayCharacterImage : CharacterCardImageBase
    {
        protected override string GetKey(long id)
        {
            ProfileCharaMasterObject master = MasterManager.Instance.profileCharaMaster.FindData(id);
            // キャラのリソースがあればキャラの画像を表示
            if (master.displayCharaImageId != 0)
            {
                return PageResourceLoadUtility.GetCharacterCardImagePath(master.displayCharaImageId);
            }
            else
            {
                return PageResourceLoadUtility.GetDisplayCharacterImagePath(master.id);
            }
        }

        protected override string GetEffectKey(long id)
        {
            ProfileCharaMasterObject master = MasterManager.Instance.profileCharaMaster.FindData(id);
            // キャラのリソースがあればキャラのエフェクトを表示
            if (master.displayCharaImageId != 0)
            {
                return PageResourceLoadUtility.GetCharacterCardEffectPath(master.displayCharaImageId);
            }
            else
            {
                return PageResourceLoadUtility.GetDisplayCharacterEffectPath(master.id);
            }
        }
    }
}