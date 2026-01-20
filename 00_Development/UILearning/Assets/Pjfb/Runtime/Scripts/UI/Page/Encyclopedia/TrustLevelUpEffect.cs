using System.Linq;
using CruFramework.UI;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Master;

namespace Pjfb.Encyclopedia
{
    public class TrustLevelUpEffect : MonoBehaviour
    {
        private const string LevelKey = "character.trust_level";
        

        [SerializeField] private Animator animator;
        [SerializeField] private ScrollGrid rewardScroll;
        [SerializeField] private TextMeshProUGUI fromLevelText;
        [SerializeField] private TextMeshProUGUI toLevelText;
        [SerializeField] private CharacterCardImage characterCardImage;
        
        
        #region PrivateMethods
        public async UniTask InitializeAndOpen(long parentMCharaId, long levelRewardId, long fromLevel, long toLevel)
        {
            AppManager.Instance.UIManager.Footer.Hide();
            AppManager.Instance.UIManager.Header.Hide();
            long minRarityMCharaId = RarityUtility.GetMinRarityMCharaId(parentMCharaId);
            await characterCardImage.SetTextureAsync(minRarityMCharaId);
            
            fromLevelText.text = string.Format(StringValueAssetLoader.Instance[LevelKey], fromLevel);
            toLevelText.text = string.Format(StringValueAssetLoader.Instance[LevelKey], toLevel);
            
            
            var rewardTable =
                MasterManager.Instance.levelRewardPrizeMaster.values.Where(x => x.mLevelRewardId == levelRewardId);
            
            var prizeJsonList =
                rewardTable.Where(x => x.level <= toLevel && x.level > fromLevel)
                    .SelectMany(x => x.prizeJson).Select(x => new PrizeJsonGridItem.Data(x)).ToList();
            
            rewardScroll.SetItems(prizeJsonList);
    
            animator.gameObject.SetActive(true);
            animator.SetTrigger("Open");
        }
        #endregion

        public void OnClickClose()
        {
            animator.SetTrigger("Close");
            AppManager.Instance.UIManager.Footer.Show();
            AppManager.Instance.UIManager.Header.Show();
        }

        
    }
}
