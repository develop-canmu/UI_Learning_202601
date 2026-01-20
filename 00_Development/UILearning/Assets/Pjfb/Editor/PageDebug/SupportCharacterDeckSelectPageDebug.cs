using Pjfb.Training;

namespace Pjfb.Editor
{
    public abstract class TrainingSupportDeckSelectPageDebugBase<T> : DebugPage<T> where T : CruFramework.Page.Page
    {
        
    }

    public class TrainingSupportDeckSelectPageDebug : TrainingSupportDeckSelectPageDebugBase<TrainingSupportDeckSelectPage>
    {
        [PageDebug]
        private void TapFormationRestrictionsButton()
        {
            // 必要ならマスタ追加
            PageObject.OnClickRarityLimitation();
            // 追加したら忘れずマスタ削除
        }
    }

}