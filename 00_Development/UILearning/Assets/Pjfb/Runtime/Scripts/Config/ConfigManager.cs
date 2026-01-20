using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb
{
    public class ConfigManager : CruFramework.Utils.Singleton<ConfigManager>
    {
        /// <summary>課金購入等で得られる通貨のID</summary>
        public long mPointIdGem {get;private set;}
        /// <summary>エール送信上限数</summary>
        public long yellLimit {get;private set;}
        /// <summary>フォロー上限数</summary>
        public long followingMaxCount {get;private set;}
        /// <summary>ギルドの最大人数</summary>
        public long maxGuildMemberCount {get;private set;} = 0;
        /// <summary>フォロー上限数</summary>
        public long uCharaVariableCountMax {get;private set;}
        /// <summary>フォロー上限数</summary>
        public long uCharaVariableTrainerCountMax {get;private set;}

        public long mainMCharaId1{get;private set;} = 0; // 男主人公キャラのID
		public long mainMCharaId2{get;private set;} = 0; // 女主人公キャラのID
        /// <summary>名前変更に使うアイテムのID</summary>
        public long changeUserNamePointId { get; private set; }
        /// <summary>名前変更に使うアイテムの使用量</summary>
        public long changeUserNamePointValue { get; private set; }

        public ConfColosseum colosseum{get;private set;}
        
        // 育成すみキャラ最大持ち数
        private const int MaxSuccessCharacterCount = 300;
        
        public ConfGuildSearchParticipationPriorityData[] guildSearchParticipationPriorityTypeList { get; private set; }
        
        /// <summary>ガチャ演出をスキップ可能になるまでの時間</summary>
        public float gachaEffectTimeUntilSkippable { get; private set; }
        
        /// <summary>更新</summary>
        public void UpdateByResponseData(NativeApiConf config)
        {
            mPointIdGem = config.mPointIdGem;
            yellLimit = config.yellLimit;
            followingMaxCount = config.followingMaxCount;
            maxGuildMemberCount = config.maxGuildMemberCount;
            mainMCharaId1 = config.mainMCharaId1;
            mainMCharaId2 = config.mainMCharaId2;
            colosseum = config.colosseum;
            uCharaVariableCountMax = config.uCharaVariableCountMax > 0 ? config.uCharaVariableCountMax : MaxSuccessCharacterCount;
            uCharaVariableTrainerCountMax = config.uCharaVariableTrainerCountMax > 0 ? config.uCharaVariableTrainerCountMax : MaxSuccessCharacterCount;
            changeUserNamePointId = config.changeUserNameMPointId;
            changeUserNamePointValue = config.changeUserNameMPointValue;
            guildSearchParticipationPriorityTypeList = config.guildSearchParticipationPriorityTypeList;
            gachaEffectTimeUntilSkippable = config.gachaEffectTimeUntilSkippable;

            Master.MasterManager.Instance.charaMaster.RemoveMainCharaId(mainMCharaId1, mainMCharaId2);
            
        } 
    }
}
