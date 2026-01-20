using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class DiffFileCopyTool : EditorWindow
{
    private string branchA = "develop";
    private string branchB = "";
    private string outputDirPath = "/DiffFileCopyTool/";
    private string gitLog = "<color=red>gitが使用できません:(</color>";
    private bool gitExists = false;
    private SerializedObject serializedObject;
    
    [SerializeField]
    private List<string> targetExtendList = new List<string>(){".prefab"};
    
    /// <Summary>
    /// ウィンドウを表示します。
    /// </Summary>
    [MenuItem("Tools/DiffFileCopyTool")]
    static void Open()
    {
        var window = GetWindow<DiffFileCopyTool>();
        window.titleContent = new GUIContent("DiffFileCopyTool");
    }

    void Awake()
    {
        //gitのパスが通っているか確認
        gitLog = GetStandardOutputFromProcess("git", "--version").Replace("\n","");
        if (gitLog.Contains("git version"))
        {
            gitLog += " <color=green>git導入済み:)</color>";
            gitExists = true;
        }

        //Listをウィンドウに表示する用
        serializedObject = new SerializedObject(this);
        
        //DiffFileCopyTool.csのパス取得
        var mono = MonoScript.FromScriptableObject(this);
        var path = AssetDatabase.GetAssetPath(mono);
        Regex regex = new Regex("^Assets/");
        outputDirPath = regex.Replace(Path.GetDirectoryName(path), "", 1) + outputDirPath;
    }

    /// <Summary>
    /// ウィンドウのパーツを表示します。
    /// </Summary>
    void OnGUI()
    {
        var style = new GUIStyle(EditorStyles.label);
        style.richText = true;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("<size=24>差分ファイルコピーツール</size>", style, GUILayout.Height(24f));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("「branchA」と「branchB」の差分を取得します。", style);
        EditorGUILayout.LabelField($"「targetExtendList」で指定した拡張子かつ差分があるファイルを「{outputDirPath}」にコピーします。", style);
        EditorGUILayout.LabelField($"「実行」ボタンで処理を開始します、「一括削除ボタン」で「{outputDirPath}」ディレクトリを削除します。", style);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(gitLog, style);
        EditorGUILayout.Space();
        
        //gitが使えない場合は入力フォーム周り非表示
        if(!gitExists)return;
        
        //対応前のブランチ名入力フォーム
        branchA = EditorGUILayout.TextField("branchA", branchA);
        if (string.IsNullOrEmpty(branchA))
        {
            branchA = "";
        }
        
        //対応後のブランチ名入力フォーム
        branchB = EditorGUILayout.TextField("branchB", branchB);
        if (string.IsNullOrEmpty(branchB))
        {
            branchB = "";
        }

        //対象拡張子入力フォーム
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetExtendList"), true);
        serializedObject.ApplyModifiedProperties();

        GUILayout.BeginHorizontal();
        
        //実行ボタン
        if (GUILayout.Button("実行", GUILayout.MaxWidth(80f), GUILayout.Height(24f)))
        {
            //現在のブランチ名を取得する
            string currentBranch = GetCurrentBranch();
            
            //空の「DiffFileCopyTool」ディレクトリを作成する
            string outputDirFullPath = Application.dataPath + "/" + outputDirPath;
            CreateOutputDir(outputDirFullPath);

            //ブランチ名でディレクトリを作成
            Regex r = new Regex(
                    "[\\x00-\\x1f<>:\"/\\\\|?*]" +
                    "|^(CON|PRN|AUX|NUL|COM[0-9]|LPT[0-9]|CLOCK\\$)(\\.|$)" + 
                    "|[\\. ]$", RegexOptions.IgnoreCase);
            string oldOutputDirPath = outputDirFullPath + "/" + r.Replace(branchA, "-");
            string newOutputDirPath = outputDirFullPath + "/" + r.Replace(branchB, "-");
            Directory.CreateDirectory(oldOutputDirPath);
            Directory.CreateDirectory(newOutputDirPath);
            
            //ローカルに2つのブランチを持ってくる
            if(!CheckOutBranch(branchB))return;
            if(!CheckOutBranch(branchA))return;

            //ブランチ間の差分ファイル抽出、移動した場合の対応で2回差分を取る
            string[] diff1 = GetStandardOutputFromProcess("git", $"--no-pager diff {branchB}..{branchA} --name-only").Split('\n');
            string[] diff2 = GetStandardOutputFromProcess("git", $"--no-pager diff {branchA}..{branchB} --name-only").Split('\n');
            string[] resultAll = diff1.Union(diff2).ToArray();
            
            //metaファイルは削除
            resultAll = resultAll.Where(result => !result.Contains(".meta")).ToArray();
            
            //指定した拡張子のみ取得
            List<string> results = new List<string>();
            foreach (var result in resultAll)
            {
                foreach (var targetExtend in targetExtendList)
                {
                    if (result.Contains(targetExtend))
                    {
                        results.Add(result);
                        break;
                    }
                }
            }

            //フルパス変換
            for (int i = 0; i < results.Count; i++)
            {
                results[i] = GetAbsolutePath(results[i]);
            }
            
            //「branchA」移動
            if(!CheckOutBranch(branchA))return;
            
            //コピー
            foreach (var result in results)
            {
                if(File.Exists(result)) File.Copy(result,$"{oldOutputDirPath}/{Path.GetFileName(result)}");
            }
            
            //「branchB」移動
            if(!CheckOutBranch(branchB))return;

            //コピー
            foreach (var result in results)
            {
                if(File.Exists(result)) File.Copy(result,$"{newOutputDirPath}/{Path.GetFileName(result)}");
            }
            
            Debug.Log($"{branchA}と{branchB}間の差分ファイルをコピーしました。");

            //元のブランチに戻る
            CheckOutBranch(currentBranch);
            
            AssetDatabase.Refresh();
        }
        
        //一括削除ボタン
        if (GUILayout.Button("一括削除", GUILayout.MaxWidth(80f), GUILayout.Height(24f)))
        {
            //「DiffFileCopyTool」ディレクトリを削除する
            string outputDirFullPath = Application.dataPath + "/" + outputDirPath;
            DeleteOutputDir(outputDirFullPath);
            AssetDatabase.Refresh();
        }
        
        GUILayout.EndHorizontal();
    }

    private string GetAbsolutePath(string relativePath)
    {
        Regex regex = new Regex(".*?/Assets");
        relativePath = regex.Replace(relativePath, "", 1);
        regex = new Regex("^Assets");
        return Application.dataPath + regex.Replace(relativePath, "", 1);
    }

    private void CreateOutputDir(string dirPath)
    {
        DeleteOutputDir(dirPath);
        Directory.CreateDirectory(dirPath);
    }

    private void DeleteOutputDir(string dirPath)
    {
        if (Directory.Exists(dirPath))
        {
            Directory.Delete(dirPath, true);
            //metaファイルを消さないと復活するため削除
            string metaFile = $"{dirPath.Remove(dirPath.Length - 1)}.meta";
            if (File.Exists(metaFile))
            {
                File.Delete(metaFile);
            }
        }
    }
    
    private bool CheckOutBranch(string branchName)
    {
        //ブランチ移動
        GetStandardOutputFromProcess("git", $"checkout {branchName}");
        //移動できたか確認
        if (!IsSuccessCheckOut(branchName))
        {
            Debug.LogError($"{branchName}へのcheckoutに失敗しました。");
            return false;
        }
        
        return true;
    }
    
    private bool IsSuccessCheckOut(string branch)
    {
        return GetCurrentBranch().Contains(branch);
    }

    private string GetCurrentBranch()
    {
        return GetStandardOutputFromProcess("git", $"rev-parse --abbrev-ref HEAD");
    }

    private string GetStandardOutputFromProcess(string exePath, string arguments)
    {
        // プロセスの起動条件を設定する。
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = exePath,
            Arguments = arguments,
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };

        // プロセスを起動する。
        using (Process process = Process.Start(startInfo))
        {
            // 標準出力を取得する。
            string output = process.StandardOutput.ReadToEnd();

            // プロセスが終了するまで待つ。
            process.WaitForExit();

            return output;
        }
    }
}