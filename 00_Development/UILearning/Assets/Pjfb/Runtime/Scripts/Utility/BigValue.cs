
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Pjfb
{
    
    public static class BigValueExFunction
    {
        public static BigValue Sum<T>(this IEnumerable<T> values, Func<T, BigValue> func)
        {
            BigValue result = BigValue.Zero;
            foreach(T value in values)
            {
                result += func(value);
            }
            return result;
        }
        
        public static BigValue Sum(this IEnumerable<BigValue> values)
        {
            BigValue result = BigValue.Zero;
            foreach(BigValue value in values)
            {
                result += value;
            }
            return result;
        }
    }
    
    /// <summary>
    /// 数が大きくなりそうなところに使用する
    /// 運用が進んで桁が増えたらこのクラスを修正する
    /// </summary>
    public struct BigValue : IEquatable<BigValue>, IComparable<BigValue>
    {
        /// <summary>倍率レート</summary>
        public const long DefaultRateValue = 10000;
        
        /// <summary>0のデータを返す</summary>
        public static BigValue Zero{get{return new BigValue(0);}}
        /// <summary>倍率</summary>
        public static BigValue RateValue{get{return new BigValue(DefaultRateValue);}}
        
        /// <summary>割合最大値</summary>
        public static readonly long MaxRate = long.MaxValue / DefaultRateValue;

        /// <summary>倍率計算</summary>
        public static BigValue MulRate(BigValue bigValue, BigValue rateValue, long rate = DefaultRateValue)
        {
            return bigValue * rateValue / new BigValue(rate);
        }
        
        /// <summary>倍率計算</summary>
        public static BigValue DivRate(BigValue bigValue, BigValue rateValue, long rate = DefaultRateValue)
        {
            return bigValue * new BigValue(rate) / rateValue;
        }
        
        /// <summary>Maxのラップ</summary>
        public static BigValue Max(BigValue value1, BigValue value2)
        {
            return new BigValue( BigInteger.Max(value1.value, value2.Value ) );
        }
        
        /// <summary>四捨五入</summary>
        public static BigValue Round(double value)
        {
            return new BigValue(Math.Round(value));
        }

        /// <summary>桁数算出</summary>
        public BigValue Log10()
        {
            return new BigValue(BigInteger.Log10(value));
        }
        
        /// <summary>DevRem</summary>
        public static BigValue DivRem(BigValue dividend, BigValue divisor, out BigValue remainder)
        {
            BigValue devValue = new BigValue(BigInteger.DivRem(dividend.value, divisor.value, out BigInteger rem));
            remainder = new BigValue(rem);
            return devValue;
        }
        
        /// <summary>累乗</summary>>
        public static BigValue Pow(BigValue baseNum ,int count)
        {
            return new BigValue(BigInteger.Pow(baseNum.value, count));
        }
        
        /// <summary>文字列のパース</summary>
        public static bool TryParse(string str, out BigValue bigValue)
        {
            if(BigInteger.TryParse(str, out BigInteger value) == false)
            {
                bigValue = BigValue.Zero;
                return false;
            }
            
            bigValue = new BigValue(value);
            return true;
        }
        
        /// <summary>コンストラクタ</summary>
        public BigValue(long value)
        {
            this.value = value;
        }

        /// <summary>コンストラクタ:string</summary>
        public BigValue(string value)
        {
            this.value = string.IsNullOrEmpty(value) ? BigInteger.Zero : BigInteger.Parse(value);
        }
        
        /// <summary>コンストラクタ:BigInteger</summary>
        public BigValue(BigInteger value)
        {
            this.value = value;
        }
        
        public BigValue(double value)
        {
            this.value = new BigInteger(value);
        }
        
        private BigInteger value;
        /// <summary>値</summary>
        public BigInteger Value{get{return value;}set{this.value = value;}}
        
        /*
        /// <summary>直接代入できるように</summary>
        public static implicit operator long(BigValue value)
        {
            return value.value;
        }
        
        /// <summary>直接代入できるように</summary>
        public static implicit operator BigValue(long value)
        {
            return new BigValue(value);
        }
        */
        
        /// <summary>BigValue*doubleで切り上げ計算</summary>
        public static BigValue CalculationCeiling(BigValue bigValue, double value)
        {
            BigValue resultValue = new BigValue();
            resultValue.value = BigInteger.DivRem(bigValue.value * new BigValue(value * DefaultRateValue).value, DefaultRateValue, out BigInteger remainder);
            int adjust = remainder > 0 ? 1 : 0;
            return resultValue + adjust;
        }
        
        /// <summary>BigValue*float型の計算結果を四捨五入したいとき</summary>
        public static BigValue CalculationRound(BigValue value, float rate)
        {
            value *= DefaultRateValue;
            BigValue rateValue = new BigValue(rate * DefaultRateValue);
            BigValue surplusValue = value % rateValue;
            // 余りは割る数より大きくならないのでdoubleに変換して問題ないはず
            double surplus = (double)surplusValue.value / (double)rateValue.value;
            // 小数点以下をRoundした値をvalue*rateに足す
            return (value * rate / DefaultRateValue) + Math.Round(surplus);
        }
        
        /// <summary>
        /// Clamp関数
        /// Unityの.netのバージョンが低く、
        /// BigInteger.Clampが使用できないため自前で用意
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static BigValue Clamp(BigValue value, BigValue min, BigValue max)
        {
            if(value.value < min.value)
            {
                return min;
            }
            if(value.value > max.value)
            {
                return max;
            }
            return  value;
        }

        /// <summary>
        /// 割合計算
        ///  0 ~ MaxRateまでの範囲で返す
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static double RatioCalculation(BigValue numerator, BigValue denominator)
        {
            // 小数が使えないため分子をDefaultRateValue倍して計算
            BigValue result = numerator * DefaultRateValue / denominator; 
            result = Clamp(result, Zero, RateValue * MaxRate);
            // 計算結果を小数にして返す
            return (double)result.Value / DefaultRateValue;
        }
        
        /// <summary>演算</summary>
        public static BigValue operator+(BigValue left, BigValue right)
        {
            return new BigValue(left.value + right.value);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator+(BigValue left, long right)
        {
            return new BigValue(left.value + right);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator+(BigValue left, int right)
        {
            return new BigValue(left.value + right);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator+(BigValue left, double right)
        {
            BigValue leftValue = new BigValue(left.value) * DefaultRateValue;
            BigValue rightValue = new BigValue(right * DefaultRateValue);
            return new BigValue(leftValue.value + rightValue.value) / BigValue.DefaultRateValue;
        }
        
        
        /// <summary>演算</summary>
        public static BigValue operator-(BigValue left, BigValue right)
        {
            return new BigValue(left.value - right.value);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator-(BigValue left, long right)
        {
            return new BigValue(left.value - right);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator-(BigValue left, int right)
        {
            return new BigValue(left.value - right);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator-(BigValue left, float right)
        {
            BigValue leftValue = new BigValue(left.value) * DefaultRateValue;
            BigValue rightValue = new BigValue(right * DefaultRateValue);
            return new BigValue(leftValue.value - rightValue.value) / BigValue.DefaultRateValue;
        }
        
        /// <summary>演算</summary>
        public static int operator-(int left, BigValue right)
        {
            return left - (int)right.Value;
        }
        
        /// <summary>演算</summary>
        public static BigValue operator*(BigValue left, BigValue right)
        {
            return new BigValue(left.value * right.value);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator*(BigValue left, long right)
        {
            return new BigValue(left.value * right);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator*(BigValue left, int right)
        {
            return new BigValue(left.value * right);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator*(BigValue left, float right)
        {
            float collection = right * DefaultRateValue;
            return new BigValue(left.value * (BigInteger)collection / DefaultRateValue);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator*(float left, BigValue right)
        {
            float collection = left * DefaultRateValue;
            return new BigValue((BigInteger)collection * right.value / DefaultRateValue);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator*(BigValue left, double right)
        {
            double collection = right * DefaultRateValue;
            return new BigValue(left.value * (BigInteger)collection / DefaultRateValue);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator*(double left, BigValue right)
        {
            double collection = left * DefaultRateValue;
            return new BigValue((BigInteger)collection * right.value / DefaultRateValue);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator/(BigValue left, BigValue right)
        {
            return new BigValue(left.value / right.value);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator/(BigValue left, long right)
        {
            return new BigValue(left.value / right);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator/(BigValue left, int right)
        {
            return new BigValue(left.value / right);
        }
        
        ///<summary>演算</summary>
        public static BigValue operator/(BigValue left, float right)
        {
            BigValue leftValue = new BigValue(left.value) * DefaultRateValue;
            BigValue rightValue = new BigValue(right * DefaultRateValue);
            return new BigValue(leftValue.value / rightValue.value);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator%(BigValue left, BigValue right)
        {
            return new BigValue(left.value % right.value);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator%(BigValue left, long right)
        {
            return new BigValue(left.value % right);
        }
        
        /// <summary>演算</summary>
        public static BigValue operator%(BigValue left, int right)
        {
            return new BigValue(left.value % right);
        }

        /// <summary>比較演算</summary>
        public static bool operator==(BigValue left, BigValue right)
        {
            return left.value == right.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator==(BigValue left, long right)
        {
            return left.value == right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator==(BigValue left, int right)
        {
            return left.value == right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator!=(BigValue left, BigValue right)
        {
            return left.value != right.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator!=(BigValue left, long right)
        {
            return left.value != right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator!=(BigValue left, int right)
        {
            return left.value != right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<(BigValue left, BigValue right)
        {
            return left.value < right.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>(BigValue left, BigValue right)
        {
            return left.value > right.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<(BigValue left, long right)
        {
            return left.value < right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<(BigValue left, double right)
        {
            BigValue leftValue = left * DefaultRateValue;
            BigValue rightValue = new BigValue(right * DefaultRateValue);
            return leftValue <  rightValue;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>(BigValue left, double right)
        {
            BigValue leftValue = left * DefaultRateValue;
            BigValue rightValue = new BigValue(right * DefaultRateValue);
            return leftValue >  rightValue;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<(double left, BigValue right)
        {
            BigValue leftValue = new BigValue(left * DefaultRateValue);
            BigValue rightValue = right * DefaultRateValue;
            return leftValue <  rightValue;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>(double left, BigValue right)
        {
            BigValue leftValue = new BigValue(left * DefaultRateValue);
            BigValue rightValue = right * DefaultRateValue;
            return leftValue >  rightValue;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>(BigValue left, long right)
        {
            return left.value > right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<(BigValue left, int right)
        {
            return left.value < right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>(BigValue left, int right)
        {
            return left.value > right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<=(BigValue left, BigValue right)
        {
            return left.value <= right.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>=(BigValue left, BigValue right)
        {
            return left.value >= right.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<=(BigValue left, long right)
        {
            return left.value <= right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>=(BigValue left, long right)
        {
            return left.value >= right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<=(BigValue left, int right)
        {
            return left.value <= right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>=(BigValue left, int right)
        {
            return left.value >= right;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<=(BigValue left, double right)
        {
            BigValue value = new BigValue(right * DefaultRateValue);
            return left.value * DefaultRateValue <= value.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>=(BigValue left, double right)
        {
            BigValue value = new BigValue(right * DefaultRateValue);
            return left.value * DefaultRateValue >= value.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<=(long left, BigValue right)
        {
            return left <= right.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>=(long left, BigValue right)
        {
            return left >= right.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<=(int left, BigValue right)
        {
            return left <= right.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>=(int left, BigValue right)
        {
            return left >= right.value;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator<=(double left, BigValue right)
        {
            BigValue value = new BigValue(left * DefaultRateValue);
            return value.value <= right.value * DefaultRateValue;
        }
        
        /// <summary>比較演算</summary>
        public static bool operator>=(double left, BigValue right)
        {
            BigValue value = new BigValue(left * DefaultRateValue);
            return value.value >= right.value * DefaultRateValue;
        }
        
        /// <summary>ToString</summary>
        public override string ToString()
        {
            return value.ToString();
        }
        
        /// <summary>値が一致するか</summary>
        bool IEquatable<BigValue>.Equals(BigValue other)
        {
            return value == other.value;
        }
        
        int IComparable<BigValue>.CompareTo(BigValue other)
        {
            return value.CompareTo(other.Value);
        }

        public override bool Equals(object obj)
        {
            if(obj is BigValue v)
            {
                return this == v;
            }
            return false;
        }

        /// <summary>GetHashCode</summary>
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        

    }
}