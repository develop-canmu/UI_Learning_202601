using CruFramework.Page;

namespace Pjfb.Character
{
    public enum AdviserDetailTabSheetType
    {
        //練習能力
        TrainingPracticeAbility,
        //練習メニューカード
        TrainingCard,
        // エールスキル
        YellSkill,
        // サポートスキル
        SupportSkill,
        //育成イベント
        TrainingEvent,
    }

    
    /// <summary> アドバイザー詳細モーダルのシートマネジャー </summary>
    public class AdviserDetailTabSheetManager : SheetManager<AdviserDetailTabSheetType>
    {
        
    }
}