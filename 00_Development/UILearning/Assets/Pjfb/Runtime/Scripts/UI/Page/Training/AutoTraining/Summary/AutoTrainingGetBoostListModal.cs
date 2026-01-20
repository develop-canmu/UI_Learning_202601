using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Training;

namespace Pjfb
{
    public class AutoTrainingGetBoostListModal : ModalWindow
    {
        public class Arguments
        {
            public TrainingMainArguments TrainingArguments { get; }
            public TrainingAutoResultStatus ResultStatus { get; }

            public Arguments(TrainingMainArguments arguments, TrainingAutoResultStatus resultStatus)
            {
                TrainingArguments = arguments;
                ResultStatus = resultStatus;
            }
        }
        
        private struct EffectType
        {
            public long EffectId { get; }
            public long MCharaId { get; }

            public EffectType(long effectId, long mCharaId = -1)
            {
                EffectId = effectId;
                MCharaId = mCharaId;
            }
        }
        
        [SerializeField]
        private ScrollGrid boostScrollGrid = null;

        // 個数用のDictionary。キャラを区別するため、effectIdとmCharaIdが主キー
        private Dictionary<EffectType, int> boostCountDic = new ();

        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments arguments = (Arguments)args;
            
            // 初期化
            boostCountDic.Clear();
            
            
            // 無印ブースト効果の個数計算
            foreach (long effectId in arguments.ResultStatus.mTrainingPointStatusEffectIdList)
            {
                EffectType type = new EffectType(effectId);
                if (!boostCountDic.TryAdd(type, 1))
                {
                    // 既にある場合は加算
                    boostCountDic[type]++;
                }
            }
            
            // スペシャルブースト効果の個数計算
            foreach (TrainingPointEffectCharaData trainingPointEffectCharaData in arguments.ResultStatus.trainingPointStatusEffectCharaList)
            {
                long mCharaId = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(trainingPointEffectCharaData.id).mCharaId;
                foreach (long effectId in trainingPointEffectCharaData.mTrainingPointStatusEffectIdList)
                {
                    EffectType type = new EffectType(effectId, mCharaId);
                    if (!boostCountDic.TryAdd(type, 1))
                    {
                        // 既にある場合は加算
                        boostCountDic[type]++;
                    }
                }
            }
            
            // スクロールデータ
            List<AutoTrainingGetBoostListScrollData> scrollData = new ();

            // スクロールデータ作成
            foreach (EffectType effectType in boostCountDic.Keys)
            {
                int count = boostCountDic[effectType];

                AutoTrainingGetBoostListScrollData data = null;
                // スペシャルブースト由来の時
                if (effectType.MCharaId > 0)
                {
                    // トレーニング中のキャラデータを取得
                    TrainingCharacterData characterData = arguments.TrainingArguments.GetTrainingCharacterData(effectType.MCharaId);
                    data = new AutoTrainingGetBoostListScrollData(effectType.EffectId, count, characterData);
                }
                else
                {
                    data = new AutoTrainingGetBoostListScrollData(effectType.EffectId, count);
                }
                
                scrollData.Add(data);
            }
            
            // グレード順で昇順に表示
            scrollData = scrollData.OrderByDescending(v => MasterManager.Instance.trainingPointStatusEffectMaster.FindData(v.EffectId).imageId)
                                   .ThenByDescending(v => v.EffectId).ToList();
            
            boostScrollGrid.SetItems(scrollData);
            
            return base.OnPreOpen(args, token);
        }
    }
}