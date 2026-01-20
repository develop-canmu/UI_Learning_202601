using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine.UI;
using System.Linq;

namespace Pjfb.Training
{
    public class TrainingGetInspirationCardView : MonoBehaviour
    {
        [SerializeField]
        private TrainingGetInspirationView inspirationPrefab = null;
        [SerializeField]
        private Transform itemRoot = null;
        
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        
        [SerializeField]
        private GameObject selectedBadge = null;
        [SerializeField]
        private GameObject probabilityUpBadge = null;
        [SerializeField]
        private GameObject occurrenceBadge = null;
        
        [SerializeField]
        private Animator aniamtor = null;
        
        [SerializeField]
        private Image backgroundImage = null;
        
        [SerializeField]
        private TrainingCardView trainingCardView = null;
        
        private List<TrainingGetInspirationView> inspirationViews = new List<TrainingGetInspirationView>();
        
        /// <summary>開くアニメーション</summary>
        public UniTask PlayOpenAnimation()
        {
            return AnimatorUtility.WaitStateAsync(aniamtor, "Open");
        }
        
        /// <summary>アイドルアニメーション</summary>
        public UniTask PlayIdleAnimation()
        {
            return AnimatorUtility.WaitStateAsync(aniamtor, "Idle");
        }
        
        /// <summary>データ登録</summary>
        public void SetData(TrainingInspirationCardList data)
        {
            // 練習メニューカードと所持者のキャラアイコン表示
            trainingCardView.SetCardAndCharacter(data.MTrainingCardId, data.MTrainingCardCharaId, data.MCharId, data.DisplayEnhanceUIFlags);
            // 獲得インスピレーション表示
            SetInspirations(data.Inspirations);
        }
        
        /// <summary>インスピレーション</summary>
        public void SetInspirations(TrainingInspirationCardList.InspirationData[] inspirations)
        {
            // 生成済みのViewを削除
            foreach(TrainingGetInspirationView view in inspirationViews)
            {
                GameObject.Destroy(view.gameObject);
            }
            inspirationViews.Clear();

            // ソート
            TrainingInspirationCardList.InspirationData[] orderedInspirations = inspirations
                .OrderByDescending(v=>v.IsNew)
                .ThenByDescending(v=> MasterManager.Instance.trainingCardInspireMaster.FindData(v.Id).grade)
                .ThenByDescending(v=> MasterManager.Instance.trainingCardInspireMaster.FindData(v.Id).priority)
                .ThenByDescending(v=> v.Id)
                .ToArray();
        
            if(scrollGrid != null)
            {
                TrainingGetInspirationScrollItem.Argument[] args = new TrainingGetInspirationScrollItem.Argument[orderedInspirations.Length];
                for(int i=0;i<args.Length;i++)
                {
                    args[i] = new TrainingGetInspirationScrollItem.Argument(orderedInspirations[i].Id, orderedInspirations[i].IsNew);
                }
                scrollGrid.SetItems(args);
            }
            
            if(inspirationPrefab != null && itemRoot != null)
            {
                for(int i=0;i<orderedInspirations.Length;i++)
                {
                    // プレハブを生成
                    TrainingGetInspirationView inspirationView = GameObject.Instantiate<TrainingGetInspirationView>(inspirationPrefab, itemRoot);
                    // リストに追加
                    inspirationViews.Add(inspirationView);
                    // 表示を更新
                    inspirationView.SetInspiration(orderedInspirations[i].Id, false);
                    // アクティブ
                    inspirationView.gameObject.SetActive(true);
                }
            }
        }
        
        /// <summary>背景の色</summary>
        public void SetBackgroundColor(Color color)
        {
            backgroundImage.color = color;
        }
        
        /// <summary>選択中表示</summary>
        public void SetSelectedBadge(bool active)
        {
            selectedBadge.SetActive(active);
        }
        
        /// <summary>発生確率アップ表示</summary>
        public void SetProbabilityUpBadge(bool active)
        {
            probabilityUpBadge.SetActive(active);
        }
        
        /// <summary>発生中表示</summary>
        public void SetOccurrenceBadge(bool active)
        {
            occurrenceBadge.SetActive(active);
        }
    }
}