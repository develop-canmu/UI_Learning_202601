namespace Pjfb
{
    public class CharacterCardHomeTextImage : CancellableRawImageWithId
    {
        protected override void OnPreLoadTexture()
        {
            
        }

        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterCardHomeTextImagePath(id);
        }
    }
}