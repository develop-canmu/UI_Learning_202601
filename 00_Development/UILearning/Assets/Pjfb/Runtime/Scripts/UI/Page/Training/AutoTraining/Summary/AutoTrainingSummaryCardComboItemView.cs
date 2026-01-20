using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Training;

namespace Pjfb
{
    public class AutoTrainingSummaryCardComboItemView : MonoBehaviour
    {
        // 合計コンボ発生回数
        [SerializeField] 
        private AutoTrainingSummaryStatusItemView totalCardComboView;
        
        // コンボ数ごとの表示
        [SerializeField]
        private AutoTrainingSummaryStatusItemView cardComboViewPrefab = null;

        // 生成したコンボ数表示
        private List<AutoTrainingSummaryStatusItemView> cardComboViewList = new List<AutoTrainingSummaryStatusItemView>();
        
        //// <summary> 発生したコンボデータを割り当て </summary>
        public void SetComboData(ResultIdCount[] idCountList)
        {
            // 生成したオブジェクトを削除
            foreach (AutoTrainingSummaryStatusItemView view in cardComboViewList)
            {
                Destroy(view.gameObject);
            }
            
            // 各コンボ毎の発生回数をDictionaryで保持
            Dictionary<long, long> cardComboCountDictionary = new Dictionary<long, long>();
            // 発生した合計コンボ数
            long totalComboCount = 0;
            
            // 合計コンボ発生回数は常に表示
            totalCardComboView.gameObject.SetActive(true);
            
            // コンボが発生している
            if (idCountList.Length > 0)
            {
                // 各コンボ数ごとの発生回数を求める
                foreach (ResultIdCount idCount in idCountList)
                {
                    // コンボ数を取得
                    long comboValue = MasterManager.Instance.trainingCardComboElementMaster.GetComboValue(idCount.id);

                    // 見つからないコンボはとばす
                    if (comboValue <= 0)
                    {
                       continue; 
                    }

                    // まだキーにないコンボ数なら追加する
                    if (cardComboCountDictionary.ContainsKey(comboValue) == false)
                    {
                        cardComboCountDictionary.Add(comboValue, idCount.count);
                    }
                    // すでにあるコンボ数なら発生回数を加算
                    else
                    {
                        cardComboCountDictionary[comboValue] += idCount.count;
                    }
                }

                // コンボ数ごとに表示を作成する(コンボ数を昇順で)
                foreach (KeyValuePair<long, long> pair in cardComboCountDictionary.OrderBy(x => x.Key))
                {
                    AutoTrainingSummaryStatusItemView view = Instantiate<AutoTrainingSummaryStatusItemView>(cardComboViewPrefab, this.gameObject.transform);
                    view.gameObject.SetActive(true);
                    cardComboViewList.Add(view);
                    view.SetName(string.Format(StringValueAssetLoader.Instance["auto_training.summary.each_combo_count"], pair.Key));
                    view.SetValue(pair.Value);
                    // 合計コンボ数に加算
                    totalComboCount += pair.Value;
                }
            }
            
            totalCardComboView.SetValue(totalComboCount);
        }
    }
}