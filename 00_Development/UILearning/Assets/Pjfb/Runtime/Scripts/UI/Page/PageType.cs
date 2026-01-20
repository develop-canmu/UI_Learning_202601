namespace Pjfb
{
    public enum PageType
    {
        Title,
        Home,
        Gacha,
        Story,
        Training,
        TrainingPreparation,
        Shop,
        Community,
        Character,
        Club,
        ClubMatch,
        Rivalry,
        TeamConfirm,
        LoginBonus,
        Colosseum,
        Tips,
        NewInGame,
        Deck,
        Encyclopedia,
        MatchResult,
        TeamConfirmTrainingMatch,
        Event,
        ClubDeck,
        LeagueMatch,
        LeagueMatchDeck,
        LeagueMatchDeckEntry,
        LeagueMatchDeckEntryTop,
        LeagueMatchTournament,
        RecommendChara,
        Ranking,
        ClubRoyal,
        ClubRoyalDeck,
        ClubRoyalInGame,
        AdviserDeck,
        
        TutorialHome = 1000,
        TutorialAdv,
        TutorialTraining,
        TutorialTrainingPreparation,
        TutorialNewInGame,
        TutorialCharacter,
        TutorialRivalry,
        TutorialTeamConfirm,
        TutorialMatchResult,
        TutorialDefault,
        

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT
        Debug = 9000,
#endif
        
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
        AdvDebug = 9001,
        HomeDebug = 9002,
#endif
    }
}