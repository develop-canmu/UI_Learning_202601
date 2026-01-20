namespace Pjfb
{
    public class ColosseumRoomImage : CancellableRawImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetColosseumRoomImagePath(id);
        }
    }
}