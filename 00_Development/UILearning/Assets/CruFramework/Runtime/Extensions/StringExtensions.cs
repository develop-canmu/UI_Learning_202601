using System.Text.RegularExpressions;

namespace CruFramework.Extensions {
    public static class StringExtensions 
    {
        public static string CamelToSnake( this string self ){
            var result = Regex.Replace(self, "([A-Z])", "_$0");
            if( result[0] == '_' ) {
                result = result.Substring(1);
            }
            return result;
        }

        public static string SnakeToCamel( this string self )
        {
            var strArrayData = self.Split('_');
            var resValue = "";

            foreach (var str in strArrayData)
            {
                if (str.Length <= 0)
                    continue;

                resValue += char.ToUpper(str[0]) + str.Substring(1);
            }

            return string.IsNullOrEmpty(resValue) ? self : resValue;
        }

        public static string RemoveNewLine(this string self)
        {
            return string.IsNullOrEmpty(self) ? string.Empty : self.Replace("\r", "").Replace("\n", "");
        }

        public static string PrefixToUpper( this string val ){
            var upper = val.ToUpper();
            return upper[0] + val.Substring(1);
        }

        public static string PrefixToLower( this string val ){
            var upper = val.ToLower();
            return upper[0] + val.Substring(1);
        }

    }

}
