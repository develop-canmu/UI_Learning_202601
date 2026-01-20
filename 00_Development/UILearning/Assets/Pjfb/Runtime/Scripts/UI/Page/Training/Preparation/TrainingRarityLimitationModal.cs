using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingRarityLimitationModal : ModalWindow
    {
        [SerializeField] private ScrollGrid scroller;

        public class ModalArgs
        {
            public long ScenarioId { get; }

            public ModalArgs(long scenarioId)
            {
                ScenarioId = scenarioId;
            }
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            ModalArgs modalArgs = (ModalArgs)args;
            
            IEnumerable<DeckFormatConditionMasterObject> conditionList = FindDeckConditionList(modalArgs.ScenarioId);
            List<RarityLimitationScrollItem.Data> scrollItemList = CreateScrollItemList(conditionList);

            scroller.SetItems(scrollItemList);

            await base.OnPreOpen(args, token);
        }
        
        /// <summary>
        /// 以下条件の「編成制限」情報を表示優先度順で取得
        /// ・デッキフォーマットが「指定されたシナリオIDに紐づく」もの
        /// ・デッキフォーマットが「サポート器具」のもの
        /// </summary>
        private IEnumerable<DeckFormatConditionMasterObject> FindDeckConditionList(long scenarioId)
        {
            TrainingScenarioMasterObject trainingScenario = MasterManager.Instance.trainingScenarioMaster.FindData(scenarioId);
            
            return MasterManager.Instance.deckFormatConditionMaster.values
                .Where(x =>
                    x.mDeckFormatId == trainingScenario.mDeckFormatId ||
                    x.mDeckFormatId == (long)DeckFormatIdType.SupportEquipment)
                .OrderByDescending(x => x.displayPriority);
        }

        private List<RarityLimitationScrollItem.Data> CreateScrollItemList(IEnumerable<DeckFormatConditionMasterObject> conditionList)
        {
            List<RarityLimitationScrollItem.Data> result = new();
            foreach (DeckFormatConditionMasterObject condition in conditionList)
            {
                // descriptionが空であることを想定した仕様があるので、仕様に合わせて非表示にするスキップ処理
                if (string.IsNullOrEmpty(condition.description)) { continue; }

                result.Add(new RarityLimitationScrollItem.Data(
                    displayName: condition.name,
                    description: condition.description)
                );
            }
            return result;
        }
    }
}