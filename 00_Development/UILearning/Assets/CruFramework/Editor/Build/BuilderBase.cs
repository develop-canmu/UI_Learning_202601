using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Editor.Build
{
    /// <summary>
    /// ビルドスクリプトの基底クラス
    /// </summary>
    public class BuilderBase
    {
        private static Dictionary<string, string> commandLineArgs = null;
        /// <summary>コマンドライン引数一覧</summary>
        protected static Dictionary<string, string> CommandLineArgs { get { return commandLineArgs; } }

        /// <summary>
        /// コマンドライン引数の初期化
        /// </summary>
        public static void CreateCommandLineArgs()
        {
            // 生成済み
            if (commandLineArgs != null) return;

            commandLineArgs = new Dictionary<string, string>();
            // コマンドライン引数取得
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.Length <= 0) continue;

                if (arg[0] == '-')
                {
                    if (i + 1 < args.Length && args[i + 1][0] != '-')
                    {
                        commandLineArgs.Add(arg.Substring(1), args[i + 1].Trim());
                    }
                    else
                    {
                        commandLineArgs.Add(arg.Substring(1), string.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// コマンドライン引数を取得
        /// </summary>
        /// <typeparam name="T">取得する引数の型</typeparam>
        /// <param name="key">キー</param>
        /// <param name="defaultValue">キーが存在しない場合の初期値</param>
        /// <returns>コマンドライン引数</returns>
        public static T GetCommandLineArg<T>(string key, T defaultValue = default(T))
        {
            // コマンドライン引数初期化
            CreateCommandLineArgs();
            // キーがない
            if (!commandLineArgs.ContainsKey(key)) return defaultValue;
            // 型変換して返す
            return (T)System.Convert.ChangeType(commandLineArgs[key], typeof(T));
        }
    }
}
