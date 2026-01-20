using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public abstract class TrainingEnhanceDeckBaseView : MonoBehaviour
    {
        public class Param
        {
            public List<BuffTargetData> growthTargetList;
            public List<BuffTargetData> trainingList;
            public List<BuffTargetData> equipmentList;
            public List<BuffTargetData> friendList;
        }
        //// <summary> バフの対象ごとに並び替えする </summary>
        protected List<BuffTargetData> SortBuffTargetType(Param param, List<BuffTargetData> result)
        {
            // 育成対象選手のデータを追加
            result.AddRange(param.growthTargetList);
            // サポートキャラのみ取得し追加
            result.AddRange(param.trainingList.Where(x => x.DeckType == TrainingDeckSlotType.SupportCharacter));
            // フレンド枠を追加
            result.AddRange(param.friendList);
            // スペシャルサポートカードのみ取得し追加
            result.AddRange(param.trainingList.Where(x => x.DeckType == TrainingDeckSlotType.SupportCard));
            // Exスペシャルサポートカードのみ取得し追加
            result.AddRange(param.trainingList.Where(x => x.DeckType == TrainingDeckSlotType.ExSupportCard));
            // サポート器具を追加
            result.AddRange(param.equipmentList);

            return result;
        }
    }
}