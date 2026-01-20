#if PJFB_DEV

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Storage;
using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using System.Threading;



namespace Pjfb.Networking.API {
    public static class APILogger {
        readonly static string logFileName = "apiLog";
        readonly static string fullLogFileName = "fullApiLog";

        static public void DeleteLog() {
            IO.DeleteTemporaryFile(logFileName);
            IO.DeleteTemporaryFile(fullLogFileName);
        }
        
        static public void WriteLog(IAPIRequest request) {
            try{
                var post = request.GetCacheRawPostData();
                var json = System.Text.Encoding.UTF8.GetString(post);
                var now = AppTime.Now;
                var apiName = request.apiName;
                var log = now + " : " + apiName;
                WriteLog( logFileName, log );
                WriteLog( fullLogFileName, log + " : " + json);
            } catch( System.Exception e){
                //Debug機能なのでエラーが発生した場合は出力だけでExceptionはにぎりつぶす
                CruFramework.Logger.LogError(e.Message);
            }
        }

        

        static public string ReadLog() {
            var filePath = IO.AppendTemporaryPath(logFileName);
            var str = IO.ReadText(filePath);
            return str;
        }
        static public string ReadFullLog() {
            var filePath = IO.AppendTemporaryPath(fullLogFileName);
            var str = IO.ReadText(filePath);
            return str;
        }

        static void WriteLog(string fileName , string str) {
            var filePath = IO.AppendTemporaryPath(fileName);
            var encoding = Encoding.UTF8;
            using ( StreamWriter writer = new StreamWriter(filePath, true, encoding) ) {
                writer.WriteLine(str);
                writer.Close();
            }
        }
        
    }
}

#endif