using System;

namespace Pjfb.Utility
{
    /// <summary> 比較演算子タイプ </summary>
    public enum CompareOperationType
    {
        // ==
        EQ,
        // !=
        NE,
        // >=
        GE,
        // >
        GT,
        // <=
        LE,
        // <
        LT,
    }

    /// <summary> 比較演算子操作用クラス </summary>
    public static class CompareOperationUtility
    {
        public static CompareOperationType OperationType(string operationType)
        {
            CompareOperationType result = CompareOperationType.EQ;
            bool isSuccess = Enum.TryParse(operationType, out result);

            // 失敗時のエラー
            if (isSuccess == false)
            {
                CruFramework.Logger.LogError($"OperationType : {operationType}のパースに失敗しました");
                result = CompareOperationType.EQ;
            }

            return result;
        }
        
        public static bool IsCompare(CompareOperationType operationType, int x1, int x2)
        {
            switch (operationType)
            {
                case CompareOperationType.EQ:
                {
                    return x1 == x2;
                }
                case CompareOperationType.NE:
                {
                    return x1 != x2;
                }
                case CompareOperationType.GE:
                {
                    return x1 >= x2;
                }
                case CompareOperationType.GT:
                {
                    return x1 > x2;
                }
                case CompareOperationType.LE:
                {
                    return x1 <= x2;
                }
                case CompareOperationType.LT:
                {
                    return x1 < x2;
                }
                default:
                {
                    CruFramework.Logger.LogError($"Not Define {operationType} in CompareOperationType");
                    return false;
                }
            }
        }
        
        public static bool IsCompare(CompareOperationType operationType, long x1, long x2)
        {
            switch (operationType)
            {
                case CompareOperationType.EQ:
                {
                    return x1 == x2;
                }
                case CompareOperationType.NE:
                {
                    return x1 != x2;
                }
                case CompareOperationType.GE:
                {
                    return x1 >= x2;
                }
                case CompareOperationType.GT:
                {
                    return x1 > x2;
                }
                case CompareOperationType.LE:
                {
                    return x1 <= x2;
                }
                case CompareOperationType.LT:
                {
                    return x1 < x2;
                }
                default:
                {
                    CruFramework.Logger.LogError($"Not Define {operationType} in CompareOperationType");
                    return false;
                }
            }
        }
    }
}