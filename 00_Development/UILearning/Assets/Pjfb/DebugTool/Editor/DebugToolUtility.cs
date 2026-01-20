using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using System.Text;

namespace Pjfb.Editor
{
    public static class DebugToolUtility
    {
        
        public static string AlignmentJson(string json)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder indent = new StringBuilder();
            
            bool isString = false;
            
            for(int i=0;i<json.Length;i++)
            {
                char c = json[i];
                char next = i + 1 >= json.Length ? ' ' : json[i + 1];
                
                // 文字列ないの場合はそのまま出力
                if(isString)
                {
                    // 文字列終わりチェック
                    if(c == '"' && i > 0 && json[i-1] != '\\')
                    {
                        isString = false;
                    }
                    sb.Append(c);
                    continue;
                }
                
                // 文字ごとの処理
                switch(c)
                {
                    case '\n':
                        break;
                    case '\t':
                        break;
                    case '"':
                    {
                        sb.Append(c);
                        // 文字列判定
                        if(i > 0 && json[i-1] == '\\')
                        {
                            break;
                        }
                        isString = !isString;
                        break;
                    }
                    
                    case '{':
                    {
                        // 改行
                        if(i > 0)sb.AppendLine();
                        // インデントを追加
                        sb.Append(indent);
                        // 文字追加
                        sb.Append(c);
                        // 改行
                        sb.AppendLine();
                        
                        // インデント
                        indent.Append("\t");
                        // インデントを追加
                        sb.Append(indent);
                        
                        break;
                    }
                    case '}':
                    {
                        indent.Remove(0, 1);
                        // 改行
                        sb.AppendLine();
                        // インデントを追加
                        sb.Append(indent);
                        // 文字追加
                        sb.Append(c);
                        
                        if(json.Length < i + 1 && json[i + 1] != ',')
                        {
                            // 改行
                            sb.AppendLine();
                            // インデントを追加
                            sb.Append(indent);
                        }
                        break;
                    }
                    
                    case ',':
                    {
                        // 文字追加
                        sb.Append(c);
                        // 改行
                        sb.AppendLine();
                        // インデントを追加
                        sb.Append(indent);
                        break;
                    }
                    
                    default:
                    {
                        sb.Append(c);
                        break;
                    }
                }
            }
            
            
            return sb.ToString();
        }
    }
}