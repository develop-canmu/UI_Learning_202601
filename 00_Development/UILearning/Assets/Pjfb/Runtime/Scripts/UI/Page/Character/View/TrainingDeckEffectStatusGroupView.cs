using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb
{
    public class TrainingDeckEffectStatusGroupView : MonoBehaviour
    {
        // ステータス表示オブジェクト
        [SerializeField] private GameObject statusGroup;
        // ステータスのバフカテゴリタイトル
        [SerializeField] private TMP_Text statusTitleText;
        // ステータス表示スクリプト
        [SerializeField] private TrainingDeckEffectStatusView statusViewPrefab;
        [SerializeField] private RectTransform statusViewRoot;
        
        // スキル内容表示オブジェクト
        [SerializeField] private GameObject skillGroup;
        // スキルのバフカテゴリタイトル
        [SerializeField] private TMP_Text skillGroupTitle;
        // スキルのバフ効果量の表示
        [SerializeField] private TMP_Text skillEffectValue;
        
        // 生成したStatusView
        private List<TrainingDeckEffectStatusView> statusViewList = new List<TrainingDeckEffectStatusView>();
        
        public void SetEnhanceData(BuffCategoryData categoryData)
        {
            // アイコンId順でソート
            List<BuffIconData> iconDatalist = categoryData.BuffIconDataList.OrderBy(x => x.BuffIconId).ToList();
            
            // バフアイコンがステータスかどうか
            bool isStatus = categoryData.IsStatus;
            statusGroup.SetActive(isStatus);
            skillGroup.SetActive(isStatus == false);
            
            // ステータス表示なら
            if (isStatus)
            {
                statusTitleText.text = categoryData.GetBuffCategoryName();

                // 作成するオブジェクト数とキャッシュしてるオブジェクト数を引いた数(足りていない個数)
                int shortageValue = iconDatalist.Count - statusViewList.Count;
                // 足りていない分作成する
                for (int i = 0; i < shortageValue; i++)
                {
                    TrainingDeckEffectStatusView statusView = Instantiate(statusViewPrefab, statusViewRoot);
                    statusViewList.Add(statusView);   
                }

                // いったん全部非表示
                foreach (TrainingDeckEffectStatusView statusView in statusViewList)
                {
                    statusView.gameObject.SetActive(false);
                }
                
                for(int i = 0; i < iconDatalist.Count; i++)
                {
                    // 色を決定する
                    Color realColor = GetColor(iconDatalist[i].IsNewAcquisitionReal);
                    Color percentColor = GetColor(iconDatalist[i].IsNewAcquisitionPercent);
                    statusViewList[i].gameObject.SetActive(true);
                    statusViewList[i].SetStatus(iconDatalist[i], realColor, percentColor);
                }
            }
            // ステータス表示でない場合
            else
            {
                // 表示は1つだけ
                BuffIconData iconData = categoryData.BuffIconDataList.First();
                skillGroupTitle.text = categoryData.GetBuffCategoryName();

                bool isPercentValue = iconData.RealValue == 0;
                // 実数は桁を区切る
                string enhanceValue = isPercentValue ? iconData.PercentValueString : string.Format("{0:#,0}",iconData.RealValue);
                // 強調表示するか
                bool isPositiveColor = isPercentValue ? iconData.IsNewAcquisitionPercent : iconData.IsNewAcquisitionReal;
                skillEffectValue.text = string.Format(StringValueAssetLoader.Instance["training.deckEnhance.buffStatus"], enhanceValue);
                skillEffectValue.color = GetColor(isPositiveColor);
            }
        }

        // 表示するためのテキストの色を取得する
        private Color GetColor(bool isPositive)
        {
           return isPositive ? ColorValueAssetLoader.Instance[TrainingDeckEnhanceUtility.PositiveColorId] : ColorValueAssetLoader.Instance[TrainingDeckEnhanceUtility.NormalColorId];
        }
    }
}