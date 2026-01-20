using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using TMPro;
using System.Linq;
using Pjfb.Networking.App.Request;

namespace Pjfb.Training
{
    public class TrainingHeaderBoostButtonView : MonoBehaviour
    {
        
        [SerializeField]
        private TrainingBoostEffectView effectView = null;
        [SerializeField]
        private TMP_Text countText = null;
        
        
        private TrainingMainArguments args = null;
        private List<TrainingGetBoostListScrollItem.TrainingGetBoostListScrollData> sortDataList = new ();
        
        public void SetData(TrainingMainArguments args)
        {
            // 初期化
            sortDataList.Clear();
            
            this.args = args;
            
            // 効果なし
            if(args.PointStatus.mTrainingPointStatusEffectIdList.Length <= 0 && args.PointStatus.trainingPointStatusEffectCharaList.Length <= 0)
            {
                return;
            }
            
            foreach (long id in args.PointStatus.mTrainingPointStatusEffectIdList)
            {
                sortDataList.Add(new TrainingGetBoostListScrollItem.TrainingGetBoostListScrollData(id));
            }

            foreach (TrainingPointEffectCharaData trainingPointEffectCharaData in args.PointStatus.trainingPointStatusEffectCharaList)
            {
                long mCharaId = MasterManager.Instance.trainingPointStatusEffectCharaMaster.FindData(trainingPointEffectCharaData.id).mCharaId;
                TrainingCharacterData characterData = args.GetTrainingCharacterData(mCharaId);
                foreach (long effectId in trainingPointEffectCharaData.mTrainingPointStatusEffectIdList)
                {
                    sortDataList.Add(new TrainingGetBoostListScrollItem.TrainingGetBoostListScrollData(effectId, characterData));
                }
            }

            // ソート
            sortDataList = sortDataList
                // 表示優先度でソート
                .OrderByDescending(data => MasterManager.Instance.trainingPointStatusEffectMaster.FindData(data.EffectId).displayPriority)
                // ID降順でソート
                .ThenByDescending(data => data.EffectId)
                // 確定
                .ToList();
            
            // 0番目のデータを表示用に取得
            TrainingGetBoostListScrollItem.TrainingGetBoostListScrollData firstData = sortDataList.First();
            
            // 表示
            effectView.Set(firstData.EffectId, firstData.CharacterData, false);
            // 発動数
            countText.text = string.Format(StringValueAssetLoader.Instance["common.amount"], sortDataList.Count);
        }
        
        
        public void OnClick()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal( ModalType.TrainingGetBoostList, new TrainingGetBoostListModal.Data(sortDataList) );
        }
    }
}