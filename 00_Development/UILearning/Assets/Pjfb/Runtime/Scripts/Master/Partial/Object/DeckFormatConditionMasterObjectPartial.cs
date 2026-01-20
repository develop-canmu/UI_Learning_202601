
using System;
using System.Linq;

namespace Pjfb.Master {
	
	
	public enum DeckConditionType
	{
		LE = 1,
		GE = 2,
	}

	public enum DeckOperatorType
	{
		EQ,
		NE,
		GE,
		GT,
		LE,
		LT,
		BETWEEN,
		IN,
		NOT_IN,
		MAX_DUPLICATE,
		Undefined
	}

	
	
	public partial class DeckFormatConditionMasterObject : DeckFormatConditionMasterObjectBase, IMasterObject {

		public new DeckConditionType conditionType => (DeckConditionType)base.conditionType;

		public new DeckOperatorType operatorType
		{
			get
			{
				switch (base.operatorType)
				{
					case "EQ":
						return DeckOperatorType.EQ;
					case "NE":
						return DeckOperatorType.NE;
					case "GE":
						return DeckOperatorType.GE;
					case "GT":
						return DeckOperatorType.GT;
					case "LE":
						return DeckOperatorType.LE;
					case "LT":
						return DeckOperatorType.LT;
					case "BETWEEN":
						return DeckOperatorType.BETWEEN;
					case "IN":
						return DeckOperatorType.IN;
					case "NOT_IN":
						return DeckOperatorType.NOT_IN;
					case "MAX_DUPLICATE":
						return DeckOperatorType.MAX_DUPLICATE;
					default:
						return DeckOperatorType.Undefined;
				}
			}
		}
        
        // compareValueArrayがnullの場合もあるので初期化のフラグ
        private bool isCompareValueInitialized = false;
        
        private long[] compareValueArray = null;
        /// <summary>CompareValueのlong[]</summary>
        public long[] CompareValueArray
        {
            get
            {
                // 初期化されていない場合は初期化する
                if (isCompareValueInitialized == false)
                {
                    isCompareValueInitialized = true;
                    if (!string.IsNullOrEmpty(compareValue))
                    {
                        // 配列であれば配列で返す
                        if (compareValue.Contains("["))
                        {
                            compareValueArray = compareValue.Trim('[', ']').Split(',').Select(x => Convert.ToInt64(x)).ToArray();
                            return compareValueArray;
                        }

                        // 数値であっても配列にして返す
                        if (long.TryParse(compareValue, out long value))
                        {
                            compareValueArray = new[] { value };
                            return compareValueArray;
                        }
                    }
                }

                // それ以外はnullを返す
                return compareValueArray;
            }
        }
	}

}
