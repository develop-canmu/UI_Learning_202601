using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CruFramework.Adv;
using Pjfb.Editor.Adv;
using UnityEngine;
using UnityEditor;

namespace CruFramework.Editor.Adv.Convert
{
    public class AdvEngineCommandConvert
    {
        [System.Serializable]
        private struct AdvEditorSaveData
        {
            [SerializeField]
            public ulong allocateNodeId;
            [SerializeField]
            public string advDataFileGuid;
            [SerializeReference]
            public List<AdvCommandNode> commandNodes;
        }
        
        [System.Serializable]
        private class AdvSelectNodeConvertData
        {
            [SerializeField]
            public int id = 0;
            [SerializeField]
            public int nodeId = 0;
            [SerializeField]
            public string text = string.Empty;
            
        }
        
        private class AdvChangeStateCommand : IAdvCommand
        {
            public int nextNodeId = 0;
            
            void IAdvCommand.Execute(AdvManager m)
            {
                
            }
        }
        
        private class AdvGraphNodeChangeState : AdvGraphNodeDefault<AdvChangeStateCommand>
        {
            
        }
        
        private class CommandData
        {
            public AdvCommandNode node = null;
            public int rid = 0;
            public int nextNodeId = -1;
            public bool isComplete = false;
        }
        
        private class CommandConvertData
        {
            public string className = string.Empty;
            public string assemblyName = "CruEngine.Runtime";
            public Dictionary<string, object> values = new Dictionary<string, object>();
        }
        
        // 誤爆しない様に一旦メニューに表示されない様に
        //[MenuItem("CruFramework/Adv/EngineConvert/Command")]
        public static void Convert()
        {
            //Convert("Assets/Pjfb/Editor/Adv/GraphViewFiles/300010110.adv");
            //Convert("Assets/Pjfb/Editor/Adv/GraphViewFiles/0.adv");
            //Convert("Assets/Pjfb/Editor/Adv/GraphViewFiles/99001.adv");
            
            if(Directory.Exists("AdvConvert") == false)
            {
                Directory.CreateDirectory("AdvConvert");
            }
            
            // ファイルを取得
            string[] files = Directory.GetFiles("Assets/Pjfb/Editor/Adv/GraphViewFiles", "*.adv");
            // 各ファイルを変換
            for(int i=0;i<files.Length;i++)
            {
                try
                {
                    if(Convert(files[i]) == false)break;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
               
                // プログレスバー
                EditorUtility.DisplayProgressBar( Path.GetFileName(files[i]), string.Empty, (float)i / (float)(files.Length - 1) );
            }
            // プログレスバーの削除
            EditorUtility.ClearProgressBar();
            
            
        }
        
        private static bool Convert(string path)
        {
            // ファイル読み込み
            string json = File.ReadAllText(path);
            AdvEditorSaveData data = JsonUtility.FromJson<AdvEditorSaveData>(json);

            // Dic
            Dictionary<ulong, CommandData> nodeDic = new Dictionary<ulong, CommandData>();
            
            // コマンド
            List<CommandData> commandDatas = new List<CommandData>();
            
            // Json構築用
            AdvEngineConvertJson convertJson = new AdvEngineConvertJson();
            
            // ルート
            convertJson.AddObject("graphView");
            { 
                // Entry
                ulong entryNodeId = 0;
                int rid = 1000;
                // Dicを生成
                for(int i=0;i<data.commandNodes.Count;i++)
                {
                    AdvCommandNode node = data.commandNodes[i];
                    // コマンドデータ取得
                    CommandData[] commands = GetCommandDatas(node, ref data.allocateNodeId);
                    // コマンド
                    for(int n=0;n<commands.Length;n++)
                    {
                        commandDatas.Add(commands[n]);
                        commands[n].rid = rid++;
                        
                        nodeDic.Add(commands[n].node.NodeId, commands[n]);
                    }
                    
                    // Entry
                    if(node is AdvGraphNodeEntryPoint)
                    {
                        entryNodeId = node.NodeId;
                    }
                }
                
                // CaseEndに繋がってるノードをStateChangeに変える
                int commandCount = commandDatas.Count;
                for(int i=0;i<commandCount;i++)
                {
                    if(commandDatas[i].node.OutputNodes.Count == 1)
                    {
                        ulong nextNode = commandDatas[i].node.OutputNodes[0];
                        if(nodeDic.ContainsKey(nextNode) && nodeDic[nextNode].node is AdvGraphNodeCaseEnd caseEndNode)
                        {
                            CommandData endCommand = new CommandData();
                            endCommand.node = new AdvGraphNodeEnd();
                            SetValue(endCommand.node, "nodeId", ++data.allocateNodeId);
                            endCommand.rid = rid++;
                            
                            CommandData c = new CommandData();
                            AdvGraphNodeChangeState changeStateNode = new AdvGraphNodeChangeState();
                            changeStateNode.Command.nextNodeId = (int)nextNode;
                            c.node = changeStateNode;
                            SetValue(c.node, "nodeId", ++data.allocateNodeId);
                            c.node.OutputNodes.Add(endCommand.node.NodeId);
                            c.rid = rid++;
                            
                            commandDatas[i].node.OutputNodes.Clear();
                            commandDatas[i].node.OutputNodes.Add(c.node.NodeId);
                            
                            commandDatas.Add(c);
                            nodeDic.Add(c.node.NodeId, c);
                            
                            commandDatas.Add(endCommand);
                            nodeDic.Add(endCommand.node.NodeId, endCommand);

                        }
                    }
                }
                
                // AllocateId
                convertJson.AddValue("allocateNodeId", data.allocateNodeId);
                // Scroll
                convertJson.AddObject("scrollValue");
                {
                    convertJson.AddValue("x", 0);
                    convertJson.AddValue("y", 0);
                }
                convertJson.EndObject();
                
                // NodeData
                convertJson.AddArrayObject("nodeDatas");
                {
                    // Entryから順に変換
                    Vector2Int p = Vector2Int.zero;
                    ConvertGridPosition(convertJson, nodeDic[entryNodeId], ref p, nodeDic, null);
                }
                convertJson.EndArrayObject();
            }
            convertJson.EndObject();
            
            // 参照
            convertJson.AddObject("references");
            {
                convertJson.AddValue("version", 2);
                convertJson.AddArrayObject("RefIds");
                {
                    for(int i=0;i<commandDatas.Count;i++)
                    {
                        convertJson.BeginElement();
                        
                        CommandData node = commandDatas[i];
                        // コマンド変換
                        CommandConvertData convertData = ConvertCommand(node.node, path, nodeDic);
                        
                        if(convertData == null)return false;
                        
                        convertJson.AddValue("rid", node.rid);
                        convertJson.AddObject("type");
                        {
                            convertJson.AddValue("class", convertData.className); 
                            convertJson.AddValue("ns", "CruEngine"); 
                            convertJson.AddValue("asm", convertData.assemblyName); 
                        }
                        convertJson.EndObject();
                        
                        convertJson.AddObject("data");
                        {
                            // 共通
                            convertJson.AddValue("id", node.node.NodeId);
                            convertJson.AddValue("nextNodeId", node.nextNodeId);
                            
                            foreach(KeyValuePair<string, object> value in convertData.values)
                            {
                                convertJson.AddValue(value.Key, value.Value);
                            }
                        }
                        convertJson.EndObject();
                        
                        convertJson.EndElement();
                    }
                }
                convertJson.EndArrayObject();
                
            }
            convertJson.EndObject();
            
            //GUIUtility.systemCopyBuffer = convertJson.ToString();
            //Debug.LogError( convertJson);
            System.IO.File.WriteAllText($"AdvConvert/{Path.GetFileNameWithoutExtension(path)}.json", convertJson.ToString());
            
            return true;
        }
        
        private static void ConvertGridPosition(AdvEngineConvertJson json, CommandData command, ref Vector2Int position, Dictionary<ulong, CommandData> nodeDic, List<AdvCommandNode> changeStateList)
        {
            // すでに変換済み
            if(command.isComplete)return;
            
            if(changeStateList != null)
            {
                if(command.node is AdvGraphNodeCaseEnd caseEnd)
                {
                    changeStateList.Add(caseEnd);
                    return;
                }
            }
            
            // 変換済みに
            command.isComplete = true;
            
            json.BeginElement();
            json.AddObject("node");
            json.AddValue("rid", command.rid);
            json.EndObject();
            
            json.AddObject("gridPosition");
            json.AddValue("x", position.x);
            json.AddValue("y", position.y);
            json.EndObject();
            json.EndElement();
            
            // 接続先が１つの場合
            if(command.node.OutputNodes.Count == 1)
            {
                position.x++;
                command.nextNodeId = (int)command.node.OutputNodes[0];
                if(nodeDic.TryGetValue(command.node.OutputNodes[0], out CommandData nextNode))
                {
                    ConvertGridPosition(json, nextNode, ref position, nodeDic, changeStateList);
                }
            }
            // 接続先が複数の場合
            else if(command.node.OutputNodes.Count >= 2)
            {
                List<AdvCommandNode> list = new List<AdvCommandNode>();
                
                foreach(ulong nextNodeId in command.node.OutputNodes)
                {
                    position = new Vector2Int(0, position.y + 1);
                    if(nodeDic.TryGetValue(nextNodeId, out CommandData nextNode))
                    {
                        ConvertGridPosition(json, nextNode, ref position, nodeDic, list);
                    }
                }
                
                foreach(AdvCommandNode node in list)
                {
                    position = new Vector2Int(0, position.y + 1);
                    
                    if(nodeDic.TryGetValue(node.NodeId, out CommandData nextNode))
                    {
                        ConvertGridPosition(json, nextNode, ref position, nodeDic, changeStateList);
                    }
                }
            }

            if(command.node is AdvGraphNodeChangeState changeState)
            {
                position = new Vector2Int(0, position.y + 1);
                
                if(nodeDic.TryGetValue((ulong)changeState.Command.nextNodeId, out CommandData nextNode))
                {
                    ConvertGridPosition(json, nextNode, ref position, nodeDic, changeStateList);
                }
            }
        }
        
        
        private static CommandData[] GetCommandDatas(AdvCommandNode command, ref ulong allocateId)
        {
            List<CommandData> result = new List<CommandData>();
            
            switch(command)
            {
                default:
                {
                    CommandData c = new CommandData();
                    c.node = command;
                    result.Add(c);
                    break;
                }
                
                case AdvGraphNodeCreateCharacter c:
                {
                    // データ取得
                    object[] datas = GetValue<object[]>(c.Command, "datas");
                    // 一つもない場合はそのまま
                    if(datas.Length <= 0)
                    {
                        CommandData commandData = new CommandData();
                        commandData.node = command;
                        commandData.node.OutputNodes.AddRange(command.OutputNodes);
                        result.Add(commandData);
                    }
                    else
                    {
                        for(int i=0;i<datas.Length;i++)
                        {
                            // 新しくコマンドを発行
                            CommandData commandData = new CommandData();
                            result.Add(commandData);
                            
                            AdvGraphNodeCreateCharacter addCommand = new AdvGraphNodeCreateCharacter();
                            // コマンド
                            commandData.node = addCommand;
                            commandData.node.OutputNodes.AddRange(command.OutputNodes);
                            // Id
                            SetValue(addCommand, "nodeId", i == 0 ? command.NodeId : ++allocateId);
                            // data
                            Array array = Array.CreateInstance(datas[i].GetType(), 1);
                            array.SetValue(datas[i], 0);
                            SetValue(addCommand.Command, "datas", array);
                        }
                    }
                    break;
                }
                
                case AdvGraphNodeActiveCharacter c:
                {
                    // データ取得
                    object[] datas = GetValue<object[]>(c.Command, "activeDatas");
                    // 一つもない場合はそのまま
                    if(datas.Length <= 0)
                    {
                        CommandData commandData = new CommandData();
                        commandData.node = command;
                        commandData.node.OutputNodes.AddRange(command.OutputNodes);
                        result.Add(commandData);
                    }
                    else
                    {
                        for(int i=0;i<datas.Length;i++)
                        {
                            // 新しくコマンドを発行
                            CommandData commandData = new CommandData();
                            result.Add(commandData);
                            
                            AdvGraphNodeActiveCharacter addCommand = new AdvGraphNodeActiveCharacter();
                            // コマンド
                            commandData.node = addCommand;
                            commandData.node.OutputNodes.AddRange(command.OutputNodes);
                            // Id
                            SetValue(addCommand, "nodeId", i == 0 ? command.NodeId : ++allocateId);
                            // data
                            Array array = Array.CreateInstance(datas[i].GetType(), 1);
                            array.SetValue(datas[i], 0);
                            SetValue(addCommand.Command, "activeDatas", array);
                        }
                    }
                    break;
                }
                
                case AdvGraphNodeDeleteCharacter c:
                {
                    // データ取得
                    int[] datas = GetValue<int[]>(c.Command, "ids");
                    // 一つもない場合はそのまま
                    if(datas.Length <= 0)
                    {
                        CommandData commandData = new CommandData();
                        commandData.node = command;
                        commandData.node.OutputNodes.AddRange(command.OutputNodes);
                        result.Add(commandData);
                    }
                    else
                    {
                        for(int i=0;i<datas.Length;i++)
                        {
                            // 新しくコマンドを発行
                            CommandData commandData = new CommandData();
                            result.Add(commandData);
                            
                            AdvGraphNodeDeleteCharacter addCommand = new AdvGraphNodeDeleteCharacter();
                            // コマンド
                            commandData.node = addCommand;
                            commandData.node.OutputNodes.AddRange(command.OutputNodes);
                            // Id
                            SetValue(addCommand, "nodeId", i == 0 ? command.NodeId : ++allocateId);
                            // data
                            Array array = Array.CreateInstance(datas[i].GetType(), 1);
                            array.SetValue(datas[i], 0);
                            SetValue(addCommand.Command, "ids", array);
                        }
                    }
                    break;
                }
                
                case AdvGraphNodeSpineSetFace c:
                {
                    // データ取得
                    object[] datas = GetValue<object[]>(c.Command, "faceDatas");
                    // 一つもない場合はそのまま
                    if(datas.Length <= 0)
                    {
                        CommandData commandData = new CommandData();
                        commandData.node = command;
                        commandData.node.OutputNodes.AddRange(command.OutputNodes);
                        result.Add(commandData);
                    }
                    else
                    {
                        for(int i=0;i<datas.Length;i++)
                        {
                            // 新しくコマンドを発行
                            CommandData commandData = new CommandData();
                            result.Add(commandData);
                            
                            AdvGraphNodeSpineSetFace addCommand = new AdvGraphNodeSpineSetFace();
                            // コマンド
                            commandData.node = addCommand;
                            commandData.node.OutputNodes.AddRange(command.OutputNodes);
                            // Id
                            SetValue(addCommand, "nodeId", i == 0 ? command.NodeId : ++allocateId);
                            // data
                            Array array = Array.CreateInstance(datas[i].GetType(), 1);
                            array.SetValue(datas[i], 0);
                            SetValue(addCommand.Command, "faceDatas", array);
                        }
                    }
                    break;
                }
                
                case AdvGraphNodeDeleteObject c:
                {
                    // データ取得
                    ulong[] datas = GetValue<ulong[]>(c.Command, "ids");
                    // 一つもない場合はそのまま
                    if(datas.Length <= 0)
                    {
                        CommandData commandData = new CommandData();
                        commandData.node = command;
                        commandData.node.OutputNodes.AddRange(command.OutputNodes);
                        result.Add(commandData);
                    }
                    else
                    {
                        for(int i=0;i<datas.Length;i++)
                        {
                            // 新しくコマンドを発行
                            CommandData commandData = new CommandData();
                            result.Add(commandData);
                            
                            AdvGraphNodeDeleteObject addCommand = new AdvGraphNodeDeleteObject();
                            // コマンド
                            commandData.node = addCommand;
                            commandData.node.OutputNodes.AddRange(command.OutputNodes);
                            // Id
                            SetValue(addCommand, "nodeId", i == 0 ? command.NodeId : ++allocateId);
                            // data
                            Array array = Array.CreateInstance(datas[i].GetType(), 1);
                            array.SetValue(datas[i], 0);
                            SetValue(addCommand.Command, "ids", array);
                        }
                    }
                    break;
                }
                
                case AdvGraphNodeActiveMessageWindow c:
                {
                    // データ取得
                    object[] datas = GetValue<object[]>(c.Command, "activeDatas");
                    // 一つもない場合はそのまま
                    if(datas.Length <= 0)
                    {
                        CommandData commandData = new CommandData();
                        commandData.node = command;
                        commandData.node.OutputNodes.AddRange(command.OutputNodes);
                        result.Add(commandData);
                    }
                    else
                    {
                        for(int i=0;i<datas.Length;i++)
                        {
                            // 新しくコマンドを発行
                            CommandData commandData = new CommandData();
                            result.Add(commandData);
                            
                            AdvGraphNodeActiveMessageWindow addCommand = new AdvGraphNodeActiveMessageWindow();
                            // コマンド
                            commandData.node = addCommand;
                            commandData.node.OutputNodes.AddRange(command.OutputNodes);
                            // Id
                            SetValue(addCommand, "nodeId", i == 0 ? command.NodeId : ++allocateId);
                            // data
                            Array array = Array.CreateInstance(datas[i].GetType(), 1);
                            array.SetValue(datas[i], 0);
                            SetValue(addCommand.Command, "activeDatas", array);
                        }
                    }
                    break;
                }
                
                case AdvGraphNodeHighlightCharacter c:
                {
                    // データ取得
                    int[] datas = GetValue<int[]>(c.Command, "ids");
                    // 一つもない場合はそのまま
                    if(datas.Length <= 0)
                    {
                        CommandData commandData = new CommandData();
                        commandData.node = command;
                        commandData.node.OutputNodes.AddRange(command.OutputNodes);
                        result.Add(commandData);
                    }
                    else
                    {
                        for(int i=0;i<datas.Length;i++)
                        {
                            // 新しくコマンドを発行
                            CommandData commandData = new CommandData();
                            result.Add(commandData);
                            
                            AdvGraphNodeHighlightCharacter addCommand = new AdvGraphNodeHighlightCharacter();
                            // コマンド
                            commandData.node = addCommand;
                            commandData.node.OutputNodes.AddRange(command.OutputNodes);
                            // Id
                            SetValue(addCommand, "nodeId", i == 0 ? command.NodeId : ++allocateId);
                            // data
                            Array array = Array.CreateInstance(datas[i].GetType(), 1);
                            array.SetValue(datas[i], 0);
                            SetValue(addCommand.Command, "ids", array);
                        }
                    }
                    break;
                }
                
                case AdvGraphNodeFrontCharacter c:
                {
                    // データ取得
                    int[] datas = GetValue<int[]>(c.Command, "ids");
                    // 一つもない場合はそのまま
                    if(datas.Length <= 0)
                    {
                        CommandData commandData = new CommandData();
                        commandData.node = command;
                        commandData.node.OutputNodes.AddRange(command.OutputNodes);
                        result.Add(commandData);
                    }
                    else
                    {
                        for(int i=0;i<datas.Length;i++)
                        {
                            // 新しくコマンドを発行
                            CommandData commandData = new CommandData();
                            result.Add(commandData);
                            
                            AdvGraphNodeFrontCharacter addCommand = new AdvGraphNodeFrontCharacter();
                            // コマンド
                            commandData.node = addCommand;
                            commandData.node.OutputNodes.AddRange(command.OutputNodes);
                            // Id
                            SetValue(addCommand, "nodeId", i == 0 ? command.NodeId : ++allocateId);
                            // data
                            Array array = Array.CreateInstance(datas[i].GetType(), 1);
                            array.SetValue(datas[i], 0);
                            SetValue(addCommand.Command, "ids", array);
                        }
                    }
                    break;
                }
            }
            
            // 接続先の修正
            if(result.Count > 1)
            {
                // 元の接続先
                List<ulong> src = new List<ulong>(result[0].node.OutputNodes);
                for(int i=0;i<result.Count-1;i++)
                {
                    result[i].node.OutputNodes.Clear();
                    result[i].node.OutputNodes.Add(result[i+1].node.NodeId);
                }
                
                result[result.Count-1].node.OutputNodes.Clear();
                result[result.Count-1].node.OutputNodes.AddRange(src);
                

            }
            
            return result.ToArray();
        }
        
        private static T GetValue<T>(object value, string name)
        {
            Type type = value.GetType();
            while(true)
            {
                if(type == null || type == typeof(object))return default;
                
                FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if(field != null)
                {
                    return (T)field.GetValue(value);
                }
                
                type = type.BaseType;
            }
            
        }
        
        private static void SetValue(object parent, string name, object value)
        {
            Type type = parent.GetType();
            while(true)
            {
                if(type == null || type == typeof(object))return;

                FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if(field != null)
                {
                    field.SetValue(parent, value);
                }
                
                type = type.BaseType;
            }
        }
        
        
        private static CommandConvertData ConvertCommand(AdvCommandNode command, string path, Dictionary<ulong, CommandData> commands)
        {
            CommandConvertData result = new CommandConvertData();

            switch(command)
            {
                
                case AdvGraphNodeEntryPoint:
                {
                    result.className = "CruStateMachineEntryNode";
                    break;
                }
                
                
                case AdvGraphNodeCaseEnd:
                {
                    result.className = "CruStateMachineStateEntryNode";
                    break;
                }
                
                case AdvGraphNodeSelectConditions:
                {
                    result.className = "CruStateMachineStateEntryNode";
                    break;
                }
                
                                
                case AdvGraphNodeTrainingChoiceCondition:
                {
                    result.className = "CruStateMachineStateEntryNode";
                    break;
                }
                
                case AdvGraphNodeSwitch:
                {
                    result.className = "CruStateMachineEndNode";
                    break;
                }
                
                case AdvGraphNodeEnd:
                {
                    result.className = "CruStateMachineEndNode";
                    break;
                }
                
                case AdvGraphNodeChangeState c:
                {
                    result.className = "CruStateMachineStateSwitchNode";
                    result.values.Add("nodeId", c.Command.nextNodeId);
                    break;
                }
                
                case AdvGraphNodeCreateCharacter c:
                {
                    result.className = "CruAdvCreateCharacterNode";
                    
                    object[] datas = GetValue<object[]>(c.Command, "datas");
                    result.values.Add("characterId", GetValue<int>(datas[0], "id") );
                    result.values.Add("positionId", GetValue<int>(datas[0], "positionId") );
                    result.values.Add("isActive", GetValue<bool>(datas[0], "isActive") );
                    break;
                }
                
                case AdvGraphNodeDeleteCharacter c:
                {
                    result.className = "CruAdvDeleteCharacterNode";
                    
                    int[] datas = GetValue<int[]>(c.Command, "ids");
                    result.values.Add("characterId", datas[0] );
                    break;
                }
                
                case AdvGraphNodeDeleteObject c:
                {
                    result.className = "CruAdvDeleteObjectNode";
                    
                    ulong[] datas = GetValue<ulong[]>(c.Command, "ids");
                    result.values.Add("objectId", (int)datas[0] );
                    break;
                }
                
                case AdvGraphNodeMoveCharacter c:
                {
                    result.className = "CruAdvMoveCharacterNode";
                    
                    result.values.Add("characterId", GetValue<int>(c.Command, "characterId") );
                    result.values.Add("positionId", GetValue<int>(c.Command, "positionId") );
                    result.values.Add("offset", GetValue<Vector3>(c.Command, "offset") );
                    result.values.Add("duration", GetValue<float>(c.Command, "duration") );
                    result.values.Add("isWait", GetValue<bool>(c.Command, "isWait") );
                    break;
                }
                
                case AdvGraphNodeSpineSetFace c:
                {
                    result.className = "CruAdvSetCharacterFaceNode";
                    result.assemblyName = "Cru.AdvSpine.Runtime";
                    
                    object[] datas = GetValue<object[]>(c.Command, "faceDatas");
                    if(datas.Length > 0 )
                    {
                        result.values.Add("characterId", GetValue<int>(datas[0], "characterId") );
                        result.values.Add("faceId", GetValue<int>(datas[0], "faceId") );
                    }
                    else
                    {
                        Debug.LogError("AdvGraphNodeSpineSetFace Array 0 : " + Path.GetFileName(path));
                    }
                    break;
                }
                
                case AdvGraphSetTexture c:
                {
                    result.className = "CruAdvSetTextureNode";
                    
                    result.values.Add("textureId", GetValue<int>(c.Command, "textureId") );
                    result.values.Add("resourceId", GetValue<int>(c.Command, "id") );
                    break;
                }
                
                case AdvGraphNodeActiveCharacter c:
                {
                    result.className = "CruAdvActiveCharacterNode";
                    
                    object[] datas = GetValue<object[]>(c.Command, "activeDatas");
                    
                    if(datas.Length > 0)
                    {
                        result.values.Add("characterId", GetValue<int>(datas[0], "id") );
                        result.values.Add("isActive", GetValue<bool>(datas[0], "isActive") );
                    }
                    else
                    {
                        Debug.LogError("AdvGraphNodeActiveCharacter Array 0 : " + Path.GetFileName(path));
                    }
                    

                    break;
                }
                
                case AdvGraphNodeActiveMessageWindow c:
                {
                    result.className = "CruAdvActiveMessageWindowNode";
                    
                    object[] datas = GetValue<object[]>(c.Command, "activeDatas");
                    result.values.Add("windowId", GetValue<int>(datas[0], "id") );
                    result.values.Add("isActive", GetValue<bool>(datas[0], "isActive") );
                    break;
                }
                
                case AdvGraphNodeHighlightCharacter c:
                {
                    result.className = "CruAdvHighlightCharacterNode";
                    
                    int[] datas = GetValue<int[]>(c.Command, "ids");
                    result.values.Add("characterId", datas[0] );
                    break;
                }
                
                case AdvGraphNodeFrontCharacter c:
                {
                    result.className = "CruAdvFrontCharacterNode";
                    
                    int[] datas = GetValue<int[]>(c.Command, "ids");
                    result.values.Add("characterId", datas[0]);

                    break;
                }
                
                case AdvGraphNodeMessage c:
                {
                    result.className = "CruAdvMessageNode";
                    
                    result.values.Add("windowId", GetValue<int>(c.Command, "windowId") );
                    result.values.Add("speakerId", GetValue<int>(c.Command, "speaker") );
                    result.values.Add("isWait", GetValue<bool>(c.Command, "isWait") );
                    result.values.Add("message", GetValue<string>(c.Command, "message") );
                    result.values.Add("voicePath", GetValue<string>(c.Command, "voicePath") );
                    result.values.Add("front", GetValue<bool>(c.Command, "front") );
                    break;
                }
                
                case AdvGraphNodeTransition c:
                {
                    result.className = "CruAdvTransitionNode";
                    
                    result.values.Add("fadeId", GetValue<int>(c.Command, "type") + 3 );
                    result.values.Add("isWait", GetValue<bool>(c.Command, "isWait") );
                    break;
                }
                
                case AdvGraphNodeSelect c:
                {
                    result.className = "CruAdvSelectNode";
                    
                    ulong[] nextNodes = null;
                    // 接続先
                    if(c.OutputNodes.Count > 0 && commands.TryGetValue(c.OutputNodes[0], out CommandData nextNode))
                    {
                        nextNodes = nextNode.node.OutputNodes.ToArray();
                    }
                    else
                    {
                        nextNodes = new ulong[0];
                    }
                    
                    result.values.Add("skipMode", GetValue<int>(c.Command, "skipMode") );
                    
                    string[] messages = GetValue<string[]>(c.Command, "messages");
                    
                    AdvSelectNodeConvertData[] datas = new AdvSelectNodeConvertData[messages.Length];
                    
                    for(int i=0;i<messages.Length;i++)
                    {
                        datas[i] = new AdvSelectNodeConvertData();
                        datas[i].text = messages[i];
                        datas[i].nodeId = (int)nextNodes[i];
                    }
                    
                    result.values.Add("messages", datas );
                    
                    break;
                }
                
                case AdvGraphNodeTrainingChoice c:
                {
                    result.className = "CruAdvTrainingChoiceNode";
                    
                    ulong[] nextNodes = null;
                    // 接続先
                    if(c.OutputNodes.Count > 0 && commands.TryGetValue(c.OutputNodes[0], out CommandData nextNode))
                    {
                        nextNodes = nextNode.node.OutputNodes.ToArray();
                    }
                    else
                    {
                        nextNodes = new ulong[0];
                    }
                    
                    result.values.Add("skipMode", GetValue<int>(c.Command, "skipMode") );
                    
                    object[] messages = GetValue<object[]>(c.Command, "textDatas");
                    
                    AdvSelectNodeConvertData[] datas = new AdvSelectNodeConvertData[messages.Length];
                    
                    for(int i=0;i<messages.Length;i++)
                    {
                        datas[i] = new AdvSelectNodeConvertData();
                        datas[i].id = GetValue<int>(messages[i], "id");
                        datas[i].text = GetValue<string>(messages[i], "text");
                        datas[i].nodeId = (int)nextNodes[i];
                    }
                    
                    result.values.Add("messages", datas );
                    
                    break;
                }
                
                case AdvGraphNodePlayBgm c:
                {
                    result.className = "CruAdvPlayBgmNode";
                    
                    result.values.Add("bgmPath", GetValue<string>(c.Command, "id"));
                    result.values.Add("volume", GetValue<float>(c.Command, "volume") );
                    break;
                }
                
                case AdvGraphNodeStopBgm c:
                {
                    result.className = "CruAdvStopBgmNode";
                    
                    break;
                }
                
                case AdvGraphNodeSound c:
                {
                    result.className = "CruAdvPlayBgmNode";
                    
                    result.values.Add("bgmPath", "1");
                    result.values.Add("volume", GetValue<float>(c.Command, "volume") );
                    break;
                }
                
                case AdvGraphNodePlaySe c:
                {
                    result.className = "CruAdvPlaySeNode";
                    
                    result.values.Add("sePath", GetValue<string>(c.Command, "id"));
                    result.values.Add("volume", GetValue<float>(c.Command, "volume") );
                    break;
                }
                
                case AdvGraphNodeStopSe c:
                {
                    result.className = "CruAdvStopSeNode";
                    
                    result.values.Add("sePath", GetValue<string>(c.Command, "id"));
                    break;
                }
                
                case AdvGraphNodeTrainingVoice c:
                {
                    result.className = "CruAdvPlayTrainingVoiceNode";
                    
                    result.values.Add("voiceType", GetValue<int>(c.Command, "voiceType"));
                    break;
                }
                
                case AdvGraphNodeShakeCamera c:
                {
                    result.className = "CruAdvShakeCameraNode";
                    
                    result.values.Add("duration", GetValue<float>(c.Command, "duration"));
                    result.values.Add("stlength", GetValue<float>(c.Command, "stlength"));
                    result.values.Add("vibrato", GetValue<int>(c.Command, "vibrato"));
                    result.values.Add("isWait", GetValue<bool>(c.Command, "isWait"));
                    break;
                }
                
                case AdvGraphNodeShakeCharacter c:
                {
                    result.className = "CruAdvShakeCharacterNode";
                    
                    result.values.Add("characterId", GetValue<int>(c.Command, "id"));
                    result.values.Add("duration", GetValue<float>(c.Command, "duration"));
                    result.values.Add("stlength", GetValue<float>(c.Command, "stlength"));
                    result.values.Add("vibrato", GetValue<int>(c.Command, "vibrato"));
                    result.values.Add("isWait", GetValue<bool>(c.Command, "isWait"));
                    break;
                }
                
                case AdvGraphNodeCreateObject c:
                {
                    result.className = "CruAdvCreateObjectNode";
                    
                    result.values.Add("objectId", GetValue<int>(c.Command, "id"));
                    result.values.Add("layerId", GetValue<int>(c.Command, "layerId"));
                    result.values.Add("positionId", GetValue<int>(c.Command, "positionId"));
                    result.values.Add("offset", GetValue<Vector3>(c.Command, "offset"));
                    break;
                }
                
                case AdvGraphNodeSetCharacterParent c:
                {
                    result.className = "CruAdvSetCreateCharacterParentNode";
                    
                    result.values.Add("characterId", GetValue<int>(c.Command, "characterId"));
                    result.values.Add("parentId", (int)GetValue<ulong>(c.Command, "parentId"));
                    break;
                }
                
                case AdvGraphNodeWaitClick c:
                {
                    result.className = "CruAdvWaitClickNode";
                    result.values.Add("isEnable", GetValue<bool>(c.Command, "isEnable"));
                    break;
                }
                
                case AdvGraphNodeTrainingAwakeningCutin c:
                {
                    result.className = "CruAdvTrainingAwakeningCutin";
                    break;
                }
                
                case AdvGraphNodeOpenTrainingGoal c:
                {
                    result.className = "CruAdvOpenTrainingGoal";
                    result.values.Add("hideMessageWindow", GetValue<bool>(c.Command, "hideMessageWindow"));
                    break;
                }
                
                case AdvGraphNodeColorFade c:
                {
                    result.className = "CruAdvColorFadeNode";
                    
                    result.values.Add("fadeId", GetValue<int>(c.Command, "type") + 1);
                    result.values.Add("duration", GetValue<float>(c.Command, "duration"));
                    result.values.Add("isWait", GetValue<bool>(c.Command, "isWait"));
                    result.values.Add("color", GetValue<Color>(c.Command, "color"));
                    break;
                }
                
                case AdvGraphNodeTrainingStatusUp c:
                {
                    result.className = "CruAdvTrainingStatusUpNode";
                    
                    result.values.Add("windowId", GetValue<int>(c.Command, "windowId"));
                    result.values.Add("messageLineCount", GetValue<int>(c.Command, "messageLineCount"));
                    break;
                }

                case AdvGraphNodeCamera c:
                {
                    result.className = "CruAdvUICamera";
                    
                    result.values.Add("position", GetValue<Vector2>(c.Command, "position"));
                    result.values.Add("positionType", GetValue<int>(c.Command, "positionType"));
                    result.values.Add("zoom", GetValue<float>(c.Command, "zoom"));
                    result.values.Add("duration", GetValue<float>(c.Command, "duration"));
                    result.values.Add("isWait", GetValue<bool>(c.Command, "isWait"));
                    break;
                }
                
                case AdvGraphNodeSetText c:
                {
                    result.className = "CruAdvSetTextNode";
                    
                    result.values.Add("textObjectId", GetValue<int>(c.Command, "textObjectId"));
                    result.values.Add("stringId", GetValue<int>(c.Command, "stringId"));
                    result.values.Add("color", GetValue<Color>(c.Command, "color"));
                    break;
                }

                case AdvGraphNodeTutorial c:
                {
                    result.className = "CruAdvTutorialNode";
                    
                    result.values.Add("type", GetValue<int>(c.Command, "type"));
                    break;
                }
                
                case AdvGraphNodeOverrideSpeaker c:
                {
                    result.className = "CruAdvSetSpeakerNode";
                    
                    result.values.Add("speaker", GetValue<string>(c.Command, "speaker"));
                    result.values.Add("windowId", GetValue<int>(c.Command, "windowId"));
                    break;
                }

                default:
                {
                    Debug.LogError("未対応のコマンド:" + Path.GetFileName(path) + " : " + command.GetType());
                    result.className = "CruStateMachineStateEntryNode";
                    result.values.Add("name", command.GetType().Name.Replace("AdvGraphNode", string.Empty) );
                    return null;
                    break;
                }
            }

            return result;
        }
    }
}