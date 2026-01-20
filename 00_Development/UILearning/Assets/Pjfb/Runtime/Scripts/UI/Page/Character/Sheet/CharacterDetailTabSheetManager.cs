using CruFramework.Page;

namespace Pjfb.Character
{
    
    public enum CharacterDetailTabSheetType
    {
        CharaTrainingStatus,    //練習能力
        TrainingCard,           //練習メニューカード
        TrainingEvent,          //育成イベント
        SupportEvent,           //サポートイベント
    }
    
    public class CharacterDetailTabSheetManager : SheetManager<CharacterDetailTabSheetType>
    {
        
    }
}