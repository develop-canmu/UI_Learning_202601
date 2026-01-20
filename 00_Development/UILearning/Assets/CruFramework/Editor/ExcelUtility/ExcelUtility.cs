using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.IO;

using ExcelDataReader;

namespace CruFramework.Editor
{
    public static class ExcelUtility
    {
        public static Dictionary<string, string[,]> ToCSV(string excelFilePath)
        {
            Dictionary<string, string[,]> result = new Dictionary<string, string[,]>();

            try
            {
                //ファイルの読み取り開始
                //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using (FileStream stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                    {
                        //デフォルトのエンコードは西ヨーロッパ言語の為、日本語が文字化けする
                        //オプション設定でエンコードをシフトJISに変更する
                        FallbackEncoding = Encoding.GetEncoding("Shift_JIS")
                    });
                    
                    for(int i=0;i<reader.ResultsCount;i++)
                    {
                        int maxFieldCount = 0;
                        List<string[]> values = new List<string[]>();
                        
                        while(reader.Read())
                        {
                            // 最大フィールド数
                            maxFieldCount = Mathf.Max(maxFieldCount, reader.FieldCount);
                            // セルのデータ
                            string[] cell = new string[reader.FieldCount];
                            //1行毎に情報を取得
                            for(int col=0;col<reader.FieldCount;col++)
                            {
                                //セルの入力文字を読み取り
                                object value = reader.GetValue(col);
                                cell[col] = value == null ? string.Empty : value.ToString();
                            }
                            // リストに追加
                            values.Add(cell);
                        }
                        
                        // 整理して結果に詰める
                        string[,] csv = new string[values.Count, maxFieldCount];
                        // 初期化
                        for(int n=0;n<values.Count;n++)
                        {
                            for(int k=0;k<maxFieldCount;k++)
                            {
                                csv[n, k] = string.Empty;
                            }
                        }
                        
                        for(int n=0;n<values.Count;n++)
                        {
                            for(int k=0;k<values[n].Length;k++)
                            {
                                csv[n, k] = values[n][k];
                            }
                        }
                        
                        // 結果に詰める
                        result.Add(reader.Name, csv);                        
                        //次のシートへ移動
                        reader.NextResult();
                    }
                }
            }
            catch(SystemException e)
            {
                Debug.LogError(e);
                return null;
            }
            
            return result;
        }
    }
}
