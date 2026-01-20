using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using JetBrains.Annotations;
using Pjfb.InGame;
using Pjfb.Networking.App.Request;

namespace Pjfb
{

    [CreateAssetMenu]
    public class TutorialSettings : ScriptableObject
    {
        public enum Step
        {
            None = 0,
            SkipTutorial = 1,
            Adv1 = 100,
            Adv2 = 200,
            Digest = 300,
            Adv3 = 400,
            Adv4 = 500,
            TrainingDeck = 600,
            TrainingPractice = 700,
            TrainingInGame = 800,
            TrainingResult = 900,
            RivalryBattleMatching = 1000,
            RivalryBattleInGame = 1100,
            RivalryBattleResult = 1200,
            Adv5 = 1300,
            Strengthen = 1400,
            Adv6 = 1500,
            SkippedTutorial = 9998,
            ExitTutorial = 9999,
        }
        
        [Serializable]
        public enum ActionType
        {
            PlayAdv,
            Modal,
            Focus,
            Image,
            FocusCharacterIcon,
            FocusRoot,
            Skip,
            Message,
            ExitHome,
        }

        [Serializable]
        public enum DigestTriggerType
        {
            None,
            In,
            Out,
            Message,
            MatchUpActivated,
            AceMatchUpActivated,
        }

        [Serializable]
        public class InGameMessageDetail
        {
            public bool isPlayer;
            public int charaId;
            public int charaVoiceLibraryId;
        }

        [Serializable]
        public class InGameMessageContainer
        {
            public List<InGameMessageDetail> messageList;
        }

        [Serializable]
        public class HuntFinishContainer
        {
            public long mvpCharaId;
            public HuntFinishAPIResponse huntFinishApiResponse;
        }
        
        [Serializable]
        public class HuntChoicePrizeContainer
        {
            public long choiceCharaId;
            public HuntPrizeSet[] prizeSet;
        }

        [Serializable]
        public class ActionData
        {
            public bool autoNext;
            public int delay;
            public bool deleteButtonEvent;
            public bool invokeBaseButtonEvent;
            public bool disableTouchGuard;
            public ActionType actionType;
            public BattleConst.DigestType digestTypeCondition;
            public DigestTriggerType digestTriggerType;
            public List<int> intParams;
            public List<string> stringParams; 
        }
    
        [Serializable]
        public class Detail
        {
            public string adminName;
            public Step stepId;
            public int startProgressIndex;
            public bool scenarioInGameMode;
            public bool skipUpdateStep;
            public PageType TutorialPageType;
            public List<ActionData> actionDataList;
        }

        [Serializable]
        public class ModalDetail
        {
            public string bodyKey;
            public string titleKey;            
        }

        [Serializable]
        public class TrainingProgressData
        {
            public string adminName;
            public long code;
            public TrainingEventReward eventReward;
            public TrainingTrainingEvent trainingEvent;
            public TrainingPending pending;
            public TrainingCharaVariable charaVariable;
            public TrainingBattlePending battlePending;
        }

        [Serializable]
        public class SelectRoundMemberData
        {
            public string adminName;
            public int ownerId;
            public List<int> offenceCharacterList;
            public List<int> defenceCharacterList;
        }

        [Serializable]
        public class MarkData
        {
            public int offenceCharacterId;
            public List<int> defenceCharacterIdList;
        }
        
        [Serializable]
        public class SetMarkTargetStateData
        {
            public string adminName;
            public List<MarkData> markDataList;
        }

        [Serializable]
        public class JustRunActionStateData
        {
            public string adminName;
            public bool normalDigestSpeed;
            public TutorialBattleMatchUpResult matchUpResult;
        }

        [Serializable]
        public class SelectMatchUpActionStateData
        {
            public string adminName;
            public TutorialBattleMatchUpResult matchUpResult;
        }

        [Serializable]
        public class TutorialBattleMatchUpResult : BattleMatchUpResult
        {
            public bool AddEndLog;
            public List<BattleConst.DigestTiming> replaceDigestTimings;
            public List<int> replaceCharacterIds;
            public List<long> replaceAbilityIds;
            public List<int> insertCharacterIds;
            public List<long> insertAbilityIds;
            public List<int> preResultCharacterIds;
            public List<long> preResultAbilityIds;
        }
        
        [Serializable]
        public class InGameSettingData
        {
            public bool scenarioInGameMode;
            public List<SelectRoundMemberData> selectRoundMemberDataList;
            public List<SetMarkTargetStateData> setMarkTargetStateDataList;
            public List<JustRunActionStateData> justRunActionStateDataList;
            public List<SelectMatchUpActionStateData> selectMatchUpActionStateDataList;
        }

        [Serializable]
        public class HuntGetTimetableContainer
        {
            public HuntGetTimetableDetailAPIResponse huntGetTimetableDetail;
        }
        
        [Serializable]
        public class HomeGetDataContainer
        {
            public HomeGetDataAPIResponse getData;
        }

        public CharaV2FriendLend tutorialFriendDate;
        public List<Detail> detailList;
        public List<ModalDetail> modalDetailList;
        public int tutorialTrainingScenarioId;
        public int tutorialTrainingSupportDeckNumber;
        public int tutorialReleaseCharaId;
        public string tutorialPushBannerName;
        public List<TrainingProgressData> trainingProgressDataList;
        public List<BattleV2ClientData> battleV2ClientDataList;
        public List<InGameSettingData> inGameSettingDataList;
        public List<InGameMessageContainer> inGameMessageContainerList;
        public List<HuntFinishContainer> huntFinishContainerList;
        public List<HuntChoicePrizeContainer> huntChoicePrizeContainerList;
        public List<int> preloadAdvIdList;
        public HuntGetTimetableContainer huntGetTimetable;
        public HomeGetDataContainer homeData;
            
        /// <summary>
        /// 演出のウォームアップ用オブジェクトアドレス一覧
        /// </summary>
        public List<string> warmUpDigestObjectAddressList;
            
        public ModalDetail GetModalDetail(int index)
        {
            if (modalDetailList == null ||
                index >= modalDetailList.Count)
            {
                return null;
            }
            return modalDetailList[index];
        }
    }
}