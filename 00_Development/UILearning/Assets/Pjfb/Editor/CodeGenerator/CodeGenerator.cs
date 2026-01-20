using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Pjfb.Editor {
    public class CodeGenerator {
        protected static readonly string apiDescriptionKey = "description";
        protected static readonly string postDataRootKey = "request";
        protected static readonly string responseDataRootKey = "response";

        protected static readonly string typeKey = "type";
        protected static readonly string descriptionKey = "description";
        protected static readonly string fieldKey = "field";

        /// <summary> メッセージパックを利用するか </summary>
        protected virtual bool useMessagePack => false;

        protected class FieldInfo {
            public string name{get;private set;}
            public string type{get;private set;}
            public string description{get;private set;}
            public FieldInfo(KeyValuePair<string, object> pair ,CodeGenerator generator){
                name = pair.Key;
                bool isList = false;
                Dictionary<string, object> info = null;
                if( pair.Value is List<object> ) {
                    var valList = pair.Value as List<object>;
                    while( valList[0] is List<object> ){
                        valList = valList[0] as List<object>;
                    }
                    info = valList[0] as Dictionary<string, object>;
                    isList = true;
                    
                } else {
                    info = pair.Value as Dictionary<string, object>;
                    isList = false;
                }
                type = (info[typeKey] as string) + (isList ? "[]" : "");
                type = generator.ShorteningClassName(type);
                type = type.Replace("mixed", "object");
                type = type.Replace("array", "object[]");
                description = info[descriptionKey] as string;
            }

            public FieldInfo( string name, string type, string description){
                this.name = name;
                this.type = type;
                this.description = description;
            }
        }


        protected string ShorteningClassName( string val ){
            var splits = val.Split("\\");
            if( splits.Length <= 2 ) {
                return val;
            }

            var result = "";
            for( int i=splits.Length-2; i<splits.Length; ++i ){
                result += splits[i];
            }
            return result;
        }
        
        protected string CreateVOPropertyString( string commonModelPath, Dictionary<string, object> field ) {
            var str = "";
            if( field == null ) {
                return str;
            }
            int index = 0;
            foreach( var itr in field ){
                try{
                    if( IsFieldInfo( itr ) ) {
                        var fieldInfo = new FieldInfo(itr, this);
                        if( !string.IsNullOrEmpty(fieldInfo.type) ) {
                            str += CreatePropertyString(fieldInfo, index) + "\n";
                        } else {
                            Debug.LogWarning("fieldInfo.type is empty: " + fieldInfo.name);
                        }
                        
                    } else {
                        CreateCommonModelFile( commonModelPath, "MasterCommonModelTemplate", itr, 0 );
                        var fieldInfo = new FieldInfo(itr, this);
                        if( !string.IsNullOrEmpty(fieldInfo.type) ) {
                            str += CreatePropertyString(fieldInfo, index) + "\n";
                        } else {
                            Debug.LogWarning("fieldInfo.type is empty: " + fieldInfo.name);
                        }
                    }
                    index++;
                    
                } catch( System.Exception e ){
                    Debug.LogError("analysis json error");
                    throw e;
                }
            }
            return str;
        }

        protected string CreateMasterFieldsString( Dictionary<string, object> field ) {
            var str = "";
            if( field == null ) {
                return str;
            }
            foreach( var itr in field ){
                try{
                    var fieldInfo = new FieldInfo(itr, this);
                    if( !string.IsNullOrEmpty(fieldInfo.type) ) {
                        str += CreateMasterFieldString(fieldInfo) + "\n";
                    } else {
                        Debug.LogWarning("fieldInfo.type is empty: " + fieldInfo.name);
                    }
                    
                } catch( System.Exception e ){
                    Debug.LogError("analysis json error");
                    throw e;
                }
            }
            return str;
        }

        protected string CreateFieldsString( Dictionary<string, object> field ) {
            var str = "";
            if( field == null ) {
                return str;
            }
            foreach( var itr in field ){
                try{
                    var fieldInfo = new FieldInfo(itr, this);
                    if( !string.IsNullOrEmpty(fieldInfo.type) ) {
                        str += CreateFieldString(fieldInfo) + "\n";
                    } else {
                        Debug.LogWarning("fieldInfo.type is empty: " + fieldInfo.name);
                    }
                } catch( System.Exception e ){
                    Debug.LogError("analysis json error");
                    throw e;
                }
            }
            return str;
        }

        protected void CreateCommonModelFile( string commonModelDir, string templateFile, KeyValuePair<string, object> pair, int depth, string[] createOnlyTheseFile = null , bool addGetSetProperty = false){
            if( depth >= 100 ) {
                Debug.LogError("depth over");
                return;
            }

            Dictionary<string, object> valDic = null;
            if( pair.Value is List<object> ) {
                var valList = pair.Value as List<object>;
                valDic = valList[0] as Dictionary<string, object>;
            } else {
                valDic = pair.Value as Dictionary<string, object>;
            }
            var fieldDic = valDic[fieldKey] as Dictionary<string, object>;
            var className = valDic[typeKey] as string; 
            className = ShorteningClassName(className);

            var str = "";
            int index = 0;
            foreach( var itr in fieldDic ){
                try{
                    if( IsFieldInfo( itr ) ) {
                        var fieldInfo = new FieldInfo(itr, this);
                        if( !string.IsNullOrEmpty(fieldInfo.type) ) {
                            if (useMessagePack)
                            {
                                str += $"\t\t[MessagePack.Key({index})]\n";
                            }
                            str += CreateFieldString(fieldInfo, addGetSetProperty) + "\n";
                        } else {
                            Debug.LogWarning("fieldInfo.type is empty: " + fieldInfo.name);
                        }
                    } else {
                        CreateCommonModelFile( commonModelDir, templateFile, itr, ++depth , createOnlyTheseFile , addGetSetProperty);
                        var fieldInfo = new FieldInfo(itr, this);
                        if( !string.IsNullOrEmpty(fieldInfo.type) ) {
                            if (useMessagePack)
                            {
                                str += $"\t\t[MessagePack.Key({index})]\n";
                            }
                            str += CreateFieldString(fieldInfo, addGetSetProperty) + "\n";
                        } else {
                            Debug.LogWarning("fieldInfo.type is empty: " + fieldInfo.name);
                        }
                    }
                    index++;
                } catch( System.Exception e ){
                    Debug.LogError("analysis json error by CreateCommonModelFile: " + e.Message);
                    Debug.LogError("StackTrace : " + e.StackTrace);
                    throw e;
                }
            }

            if (createOnlyTheseFile != null && !createOnlyTheseFile.Contains(className))
            {
                return;
            }
            
            var commonModelFilePath = System.IO.Path.Combine( commonModelDir ,  className + ".cs");
            if( System.IO.File.Exists(commonModelFilePath) ) {
                System.IO.File.Delete(commonModelFilePath);
            }
            var template = Resources.Load<TextAsset>(templateFile);
            var outputStr = template.text;
            outputStr = outputStr.Replace("%CLASS_NAME%", className);
            outputStr = outputStr.Replace("%FIELD%", str);
            System.IO.File.WriteAllText(commonModelFilePath, outputStr);
        }

        protected bool IsFieldInfo( KeyValuePair<string, object> pair )
        {
            if( !(pair.Value is Dictionary<string, object>) && !(pair.Value is List<object>) ) {
                return false;
            }

            Dictionary<string, object> checkValue = null;
            if( (pair.Value is List<object>) ) {
                var valList = pair.Value as List<object>;
                while( valList[0] is List<object> ){
                    valList = valList[0] as List<object>;
                }
                checkValue = valList[0] as Dictionary<string, object>;
            } else {
                checkValue = pair.Value as Dictionary<string, object>;
            }

            if( checkValue.Count != 2 ) {
                return false;
            }

            if( !checkValue.ContainsKey(typeKey) || !checkValue.ContainsKey(descriptionKey) ) {
                return false;
            }

            if( checkValue.ContainsKey(fieldKey) ) {
                return false;
            }
            return true;
        }

        protected string CreateObjectPropertyString( Dictionary<string, object> field ) {
            var str = "";
            if( field == null ) {
                return str;
            }
            foreach( var itr in field ){
                try{
                    var fieldInfo = new FieldInfo(itr, this);
                    if( !string.IsNullOrEmpty(fieldInfo.type) ) {
                        str += CreateObjectPropertyString(fieldInfo) + "\n";
                    } else {
                        Debug.LogWarning("fieldInfo.type is empty: " + fieldInfo.name);
                    }
                    
                } catch( System.Exception e ){
                    Debug.LogError("analysis json error");
                    throw e;
                }
            }
            return str;
        }

        protected virtual string OverrideType( string type ) {
            return type;
        }
            

        protected string CreatePropertyString( FieldInfo info, int index ) {
            var type = OverrideType(info.type);
            var initVal = CreateInitVal(type, info.name);
            string str = string.Empty;
            if (useMessagePack)
            {
                str = $"\t\t[MessagePack.Key({index})]\n";
            }
            str += "\t\tpublic " + type + " _" + info.name + " {get{ return " + info.name + ";} set{ this." + info.name + " = value;}}";
            return str;
        }

        protected string CreateMasterFieldString( FieldInfo info ) {
            var type = OverrideType(info.type);
            var initVal = CreateInitVal(type, info.name);
            var str = "\t\t[UnityEngine.SerializeField] " + type + " " + info.name + " = " + initVal + ";";
            if( !string.IsNullOrEmpty(info.description) ) {
                str += " // " + info.description.Replace("\n", "");    
            }
            return str;
        }


        protected string CreateFieldString( FieldInfo info , bool addGetSetProperty = false) {
            var type = OverrideType(info.type);
            var initVal = CreateInitVal(type, info.name);
            var str = string.Empty;
            if (addGetSetProperty)
            {
                str = "\t\tpublic " + type + " " + info.name + " { get; set; }" +  " = " + initVal + ";";
            }
            else
            {
                str = "\t\tpublic " + type + " " + info.name + " = " + initVal + ";";
            }

            if( !string.IsNullOrEmpty(info.description) ) {
                str += " // " + info.description.Replace("\n", "");    
            }
            return str;
        }

        
        protected string CreateObjectPropertyString( FieldInfo info ) {
            var type = OverrideType(info.type);
            var initVal = CreateInitVal(type, info.name);
            var str = "\t\tpublic virtual " + type + " " + info.name + " => _rawData._" + info.name + ";";
            return str;
        }

        protected string CreateInitVal( string type, string name ){
            var initVal = "";
            //typeで初期化
            switch( name ){
                //pointPaidだけは-1で初期化するようにする
                //他で使用されている場合でも問題はないはずだけど今後使用箇所が増えたら対応を考える必要がある
                case "pointPaid":
                    initVal = "-1";
                    return initVal; 

            }
            //typeで初期化
            switch( type ){
                case "string":
                    initVal = "\"\"";
                    break;
                case "int":
                case "long":
                    initVal = "0";
                    break;
                case "float":
                    initVal = "0.0f";
                    break;
                case "bool":
                    initVal = "false";
                    break;
                case "double":
                    initVal = "0.0";
                    break;
                default:
                    initVal = "null";
                    break;
            }
            return initVal;
        }

    }
}