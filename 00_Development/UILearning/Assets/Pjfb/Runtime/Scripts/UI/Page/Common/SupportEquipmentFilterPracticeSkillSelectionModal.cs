using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine.Serialization;

namespace Pjfb
{
    public class SupportEquipmentFilterPracticeSkillSelectionModal : ModalWindow
    {
        // ConfirmModalData
        /// <summary>タイトル</summary>
        private const string SelectLimitTitleKey = "training.select_limit_title";
        /// <summary>説明文</summary>
        private const string SelectLimitDescriptionKey = "training.select_limit_description";
        /// <summary>閉じる</summary>
        private const string CloseKey = "common.close";
        /// <summary>3つまで選択可能</summary>
        private const int SelectableCount = 3;

        /// <summary>ScrollGrid</summary>
        [SerializeField]
        private ScrollGrid scrollGrid;

        /// <summary>判定用</summary>
        private List<SupportEquipmentFilterPracticeSkillScrollData> scrollDatas = new List<SupportEquipmentFilterPracticeSkillScrollData>();

        /// <summary>最初に表示するデータ</summary>
        private List<SupportEquipmentSortFilterModal.FilterPracticeSkillInfo> data;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            scrollDatas.Clear();
            data = (List<SupportEquipmentSortFilterModal.FilterPracticeSkillInfo>)args;
            
            var trainingStatusTypeDetailMasters = MasterManager.Instance.trainingStatusTypeDetailMaster.values;
            foreach (var trainingStatusTypeDetailMaster in trainingStatusTypeDetailMasters)
            {
                if(!trainingStatusTypeDetailMaster.isUsedAsTrainerStatus) continue;
                // データの追加
                scrollDatas.Add(new SupportEquipmentFilterPracticeSkillScrollData(trainingStatusTypeDetailMaster, data.Any(practiceSkillInfo => practiceSkillInfo.PracticeSkillId == trainingStatusTypeDetailMaster.id)));
            }
            scrollGrid.SetItems(scrollDatas);

            return base.OnPreOpen(args, token);
        }

        /// <summary>trueに変更できるかの確認</summary>
        public bool CanChangeToggle()
        {
            if (scrollDatas.Count(flag => flag.Toggle) + 1 > SelectableCount)
            {
                OpenConfirm();
                return false;
            }

            return true;
        }

        /// <summary>警告モーダル表示</summary>
        private void OpenConfirm()
        {
            ConfirmModalWindow.Open(new ConfirmModalData(StringValueAssetLoader.Instance[SelectLimitTitleKey],
                StringValueAssetLoader.Instance[SelectLimitDescriptionKey], null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance[CloseKey], w => w.Close())));
        }

        /// <summary>トグルのリセット処理</summary>
        public void OnClickResetButton()
        {
            foreach (var scrollData in scrollDatas)
            {
                scrollData.Toggle = false;
            }
            scrollGrid.RefreshItemView();
        }

        /// <summary>キャンセル</summary>
        public void OnClickCancel()
        {
            List<long> result = data.Select(x => x.PracticeSkillId).ToList();
            SetCloseParameter(result);
            CloseAsync().Forget();
        }

        /// <summary>決定</summary>
        public void OnClickSubmit()
        {
            List<long> result = scrollDatas.Where(scrollData => scrollData.Toggle).Select(scrollData => scrollData.TrainingStatusTypeDetailMasterObject.id).ToList();
            SetCloseParameter(result);
            CloseAsync().Forget();
        }
    }
}