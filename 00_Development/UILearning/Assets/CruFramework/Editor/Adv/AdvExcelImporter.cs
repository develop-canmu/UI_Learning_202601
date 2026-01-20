using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Reflection;


namespace CruFramework.Editor.Adv
{

    public static class AdvExcelImporter
    {
        
        private const float NodePositionX = 500.0f;
        private const float NodePositionY = 400.0f;
        
        [System.Serializable]
        public abstract class ArrayParseData
        {
            public abstract object GetValues();
        }
        
        [System.Serializable]
        public class ArrayParseData<T> : ArrayParseData
        {
            [SerializeField]
            public T values;

            public override object GetValues()
            {
                return values;
            }
        }
        
        private static T CreateNode<T>(AdvGraphView graphView, ulong connectNode, ref Vector2 pos) where  T : AdvCommandNode
        {
            // ノードを返す
            return (T)CreateNode(graphView, typeof(T), connectNode, ref pos);
        }
        
        private static object CreateNode(AdvGraphView graphView, Type nodeType, ulong connectNode, ref Vector2 pos)
        {
            // ノードの生成
            ulong nodeId = graphView.CreateNode(nodeType, pos);
            // 座標設定
            pos.x += NodePositionX;
            // ノード取得
            AdvCommandNode node = graphView.GetNode(nodeId);
            // 接続がある場合
            if(connectNode > 0)
            {
                graphView.ConnectPort(connectNode, node.NodeId);
            }
            // ノードを返す
            return node;
        }
        
        private static FieldInfo GetFieldInfo(Type type, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            // フィールド
            FieldInfo fieldInfo = null;

            while(true)
            {
                if(type == null || type == typeof(object))break;
                // フィールドを取得
                fieldInfo = type.GetField(name, flags);
                if(fieldInfo != null)break;
                type = type.BaseType;
            }
            
            return fieldInfo;
        }
        
        private static void SetArgument(AdvCommandNode node, AdvConfig config, string name, string value)
        {
            Type nodeType = node.GetType();
            
            object parent = node;
            FieldInfo fieldInfo = GetFieldInfo(nodeType, name);
            
            // ない場合は
            if(fieldInfo == null)
            {
                FieldInfo commandField = GetFieldInfo(nodeType, "command");
                if(commandField != null)
                {
                    parent = commandField.GetValue(node);
                    fieldInfo = GetFieldInfo(parent.GetType(), name);
                }
            }
            // フィールドがない
            if(fieldInfo == null)
            {
                Debug.LogError(nodeType.Name + "に" + name + "は存在しません");
                return;
            }
            
            Type fieldType = fieldInfo.FieldType;
            
            if(fieldType.IsArray)
            {
                Type parseType = typeof(ArrayParseData<>).MakeGenericType(fieldType);
                string json = $"{{\"values\":{value}}}";
                ArrayParseData dataValue = (ArrayParseData)JsonUtility.FromJson(json, parseType);
                fieldInfo.SetValue(parent, dataValue.GetValues());
            }
            else if(
                fieldType == typeof(int) ||
                fieldType == typeof(uint) ||
                fieldType == typeof(long) ||
                fieldType == typeof(ulong) ||
                fieldType == typeof(float) ||
                fieldType == typeof(double) ||
                fieldType == typeof(short) ||
                fieldType == typeof(ushort) ||
                fieldType == typeof(byte) ||
                fieldType == typeof(bool) ||
                fieldType == typeof(string)
            )
            {
                fieldInfo.SetValue(parent, System.Convert.ChangeType( value, fieldType ));
            }
            else if(fieldType.IsEnum)
            {
                fieldInfo.SetValue(parent, System.Enum.Parse(fieldType, value) );
            }
            else if(fieldType == typeof(Color))
            {
                if(ColorUtility.TryParseHtmlString(value, out Color color))
                {
                    fieldInfo.SetValue(parent, color);
                }
            }
        }
        
        public static void Import(string[,] csv, AdvGraphView graphView, AdvConfig config)
        {
            Vector2 nodePos = Vector2.zero;
            // エントリーポイントの作成
            AdvGraphNodeEntryPoint entryPoint = CreateNode<AdvGraphNodeEntryPoint>(graphView, 0, ref nodePos);
            // 接続するノードId
            ulong nodeId = entryPoint.NodeId;
            // 分岐ノード
            ulong switchNodeId = 0;
            int caseCount = 0;
            int selectCount = 0;
            int caseEndCount = 0;
            List<ulong> caseEndNodes = new List<ulong>();
                // キャラId
            int[] characterIds = config.CharacterDatas.GetIds();
            // キャラ名リスト
            Dictionary<string, int> characterList = new Dictionary<string, int>();
            foreach(int id in characterIds)
            {
                string name = config.CharacterDatas.GetValue(id).Name;
                if(characterList.ContainsKey(name) == false)
                {
                    characterList.Add(config.CharacterDatas.GetValue(id).Name, id);
                }
            }
            
            // データの位置を探す
            int dataStart = 0;
            int dataIndex = -1;
            int commandIndex = -1;
            for(int i=0;i<csv.GetLength(0);i++)
            {
                for(int n=0;n<csv.GetLength(1);n++)
                {
                    string v = csv[i, n];

                    if(v == "NO.")
                    {
                        dataIndex = n;
                        dataStart = i + 1;
                    }
                    
                    if(v == "Command")
                    {
                        commandIndex = n;
                    }
                }
            }
            
            if(dataIndex == -1)return;
            
            // コマンドタイプ取得用
            Dictionary<string, Type> commandTypes = new Dictionary<string, Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(Assembly a in assemblies)
            {
                Type[] types = a.GetTypes();
                foreach(Type type in types)
                {
                    if(type.IsSubclassOf(typeof(AdvCommandNode)))
                    {
                        string name = type.Name;
                        if(name.StartsWith("AdvGraphNode"))
                        {
                            name = name.Substring("AdvGraphNode".Length);
                        }
                        commandTypes.Add(name, type);
                    }
                }
            }

            
            // データを取得
            for(int i=dataStart;i<csv.GetLength(0);i++)
            {
                // コマンド番号
                if(int.TryParse( csv[i, dataIndex], out int commandNo ) == false)
                {
                    continue;
                }
                
                // 話者
                string speaker = csv[i, dataIndex + 1];
                // メッセージ
                string message1 = csv[i, dataIndex + 2];
                string message2 = csv[i + 1, dataIndex + 2];
                // メッセージがない場合は処理しない
                if(string.IsNullOrEmpty(message1) == false || string.IsNullOrEmpty(message2) == false)
                {
                    // メッセージコマンド
                    AdvGraphNodeMessage msg = CreateNode<AdvGraphNodeMessage>(graphView, nodeId, ref nodePos);
                    nodeId = msg.NodeId;
                    // メッセージ追加
                    msg.Command.Message = message1 + "\n" + message2;
                    // 話者の設定
                    if(characterList.TryGetValue(speaker, out int speakerId))
                    {
                        msg.Command.Speaker = speakerId;
                    }
                    else
                    {
                        Debug.LogWarning($"No {commandNo} {speaker} が設定ファイルにありません");
                    }
                }
                
                // コマンド
                if(commandIndex != -1)
                {
                    // コマンド
                    string commandName = csv[i, commandIndex];
                    // コマンドなし
                    if(string.IsNullOrEmpty(commandName))continue;

                    if(commandTypes.TryGetValue(commandName, out Type commandType) == false)
                    {
                        Debug.LogError(commandName + "は定義されていません");
                    }
                    else
                    {
                        // CaseEndのノードは後で作るのでIdだけ保持しておく
                        if(commandType == typeof(AdvGraphNodeCaseEnd))
                        {
                            caseEndCount++;
                            caseEndNodes.Add(nodeId);
                            // すべてのCaseEndが出た場合は
                            if(caseEndCount >= selectCount)
                            {
                                // 座標
                                float y = 0;
                                float x = float.MinValue;
                                foreach(ulong id in caseEndNodes)
                                {
                                    Vector2 pos = graphView.GetNode(id).NodePosition;
                                    x = Mathf.Max(x, pos.x);
                                    y += pos.y;
                                }
                                
                                x += NodePositionX;
                                y /= caseEndNodes.Count;
                                
                                nodePos = new Vector2(x, y);

                                ulong connectedNodeId = nodeId;
                                AdvCommandNode commandNode = (AdvCommandNode)CreateNode(graphView, commandType, nodeId, ref nodePos);
                                nodeId = commandNode.NodeId;
                                
                                foreach(ulong id in caseEndNodes)
                                {
                                    if(id == connectedNodeId)continue;
                                    graphView.ConnectPort(id, nodeId);
                                }
                                
                            }
                        }
                        else
                        {
                            // 接続するノード
                            ulong connectNodeId = nodeId;
                        
                            // 分岐だった場合はSwitchに接続
                            if(commandType.IsSubclassOf( typeof(AdvGraphNodeCase ) ))
                            {
                                connectNodeId = switchNodeId;
                                
                                nodePos = graphView.GetNode(switchNodeId).NodePosition;
                                nodePos.x += NodePositionX;
                                nodePos.y += NodePositionY * (caseCount - selectCount / 2);
                                caseCount++;
                            }
                        
                            // コマンド
                            AdvCommandNode commandNode = (AdvCommandNode)CreateNode(graphView, commandType, connectNodeId, ref nodePos);
                            nodeId = commandNode.NodeId;

                            // 引数
                            if(csv[i, commandIndex + 1].Trim().Length > 0)
                            {
                                string[] commandArguments = csv[i, commandIndex + 1].Split(";");
                                
                                for(int n=0;n<commandArguments.Length;n++)
                                {
                                    string[] args = commandArguments[n].Split('=');
                                    if(args.Length != 2)
                                    {
                                        Debug.LogError("引数エラー " + commandName + " " + commandArguments[n]);
                                        continue;
                                    }
                                    // 引数名
                                    string argName = args[0].Trim();
                                    // 値
                                    string value = args[1].Trim();
                                    // 引数をセット
                                    SetArgument(commandNode, config, argName, value);
                                }
                            }
                            
                            // Selectコマンド
                            if(commandNode is IAdvCommandSelect s)
                            {
                                selectCount = s.GetSelectCount();
                            }
                            
                            // 分岐点を記録
                            if(commandNode is AdvGraphNodeSwitch)
                            {
                                switchNodeId = nodeId;
                                caseCount = 0;
                                caseEndCount = 0;
                                caseEndNodes.Clear();
                            }
                            
                        }
                    }
                }
            }
        }
    }
}
