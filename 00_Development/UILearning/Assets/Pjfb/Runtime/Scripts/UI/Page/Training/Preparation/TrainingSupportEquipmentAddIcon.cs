namespace Pjfb.Training
{
    public class TrainingSupportEquipmentAddIcon : CancellableImage
    {
        private const string BasePath = "Images/UI/Common/Images/common_button_add_supportequipment{0}.png";
        private const string LimitedPath = "_limited";
        
        public static string GetPath(bool isLimited)
        {
            return string.Format(BasePath, isLimited ? LimitedPath : string.Empty);
        }
    }
}