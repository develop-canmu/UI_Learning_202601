using CruFramework.UI;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingCardUnionInformationScrollDynamicItemSelector : ScrollDynamicItemSelector
    {
        /// <summary>
        /// カードユニオン情報のスクロールアイテムの共通インターフェース
        /// </summary>
        public interface ICardUnionScrollItem { }

        // ヘッダーデータ用アイテム
        [SerializeField]
        private TrainingCardUnionInformationHeaderScrollItem headerItem = null;
        
        // カードデータ用アイテム
        [SerializeField] 
        private TrainingCardUnionInformationScrollItem cardUnionInfoItem = null;
        
        public override ScrollDynamicItem GetItem(object item)
        {
            // ヘッダーデータの場合
            if (item is TrainingCardUnionInformationHeaderScrollItem.Argument)
            {
                return headerItem;
            }
            // カードデータの場合
            else if (item is TrainingCardUnionInformationScrollItem.Argument)
            {
                return cardUnionInfoItem;
            }
            
            // 引数の型が想定外の場合はエラーを出す
            string errorMessage = $"Unsupported data type: {item.GetType().Name}";
            CruFramework.Logger.LogError(errorMessage);
            throw new System.ArgumentException(errorMessage, nameof(item));
        }
    }
}

