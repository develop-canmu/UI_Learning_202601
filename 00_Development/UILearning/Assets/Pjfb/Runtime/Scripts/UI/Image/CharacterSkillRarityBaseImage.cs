namespace Pjfb
{
    public class CharacterSkillRarityBaseImage : CancellableImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterSkillRarityBaseImagePath(id);
        }
    }
}