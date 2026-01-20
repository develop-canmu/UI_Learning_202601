namespace Pjfb
{
    public class ColosseumRankImage : CancellableRawImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetColosseumRankImagePath(id);
        }
    }
}
