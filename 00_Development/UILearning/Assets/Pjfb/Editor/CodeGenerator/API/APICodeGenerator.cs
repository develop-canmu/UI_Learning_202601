using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.HTTP;
using Pjfb.Networking.API;

namespace Pjfb.Editor.API {
    public class APICodeGenerator : CodeGenerator {

        static readonly string GenerateDirectory = "Assets/Pjfb/Runtime/Scripts/Networking/App";
        static readonly string GenerateLambdaDirectory = "Assets/Pjfb/Runtime/Scripts/Networking/Lambda/Editor";

        private static readonly string APICommonModelTemplateName = "APICommonModelTemplate";
        private static readonly string APICommonLambdaModelTemplateName = "APICommonLambdaModelTemplate";

        private static readonly string[] LambdaTargetModelFileNames =
        {
            "BattleV2Ability",
            "BattleV2AbilityEffect",
            "BattleV2BattleFbKeeper",
            "BattleV2Chara",
            "BattleV2ClientData",
            "BattleV2Group",
            "BattleV2Player",
            "BattleV2GvgItem",
            "DeckTirednessConditionDebug",
            "WrapperIntList",
            "LambdaResult",
            "LambdaSummary",
            "SpotComboAbilitySpotComboAbilityInfo",
            "BattleV2BattleSetting",
            "PlayerGameliftOptionSetting",
            "PlayerGameliftOptionDeckInfo",
            "PlayerGameliftOptionTeamPlacement",
            "SpotComboAbilityComboAbility",
            "SpotComboAbilityComboAbilityElement",
            
            // 以下はpjfbで使用していないクラス(エラー回避のため)
            "BattleV2BattleWarField",
            "BattleV2BattleConquestField",
            "BattleV2BattleConquestFieldSpot",
            "BattleV2Tactics",
            "BattleV2TacticsDetail",
            "BattleV2UnitRole",
            "BattleV2UnitRoleOperation",
            "BattleV2UnitRoleAction",
            "NpcGroupOptionOption",
            "NpcGroupOptionDefenseTeam",
            "NpcGroupOptionUnit",
            "SpotTacticsAbilitySpotTacticsAbilityInfo",
            "SpotTacticsAbilityTacticsAbility"
        };
        
        class DirectoryInfo {
            public string requestPath{get;private set;}
            public string commonModelPath{get;private set;}
            public string partialPath{get;private set;}
            public DirectoryInfo(string requestPath, string commonModelPath, string partialPath){
                this.requestPath = requestPath;
                this.commonModelPath = commonModelPath;
                this.partialPath = partialPath;
            }
        }

        IAPISetting _setting = new CodeGenerateAPISetting();

        public void Generate() {
            Generate(GenerateDirectory).Forget();
        }

        public void GenerateLambda() {
            GenerateLambda(GenerateLambdaDirectory).Forget();
        }
        
        public async UniTask Generate(string outputPath)
        {
            try{
                EditorUtility.DisplayProgressBar("Get API List","ConnectListAPI", 1.0f );
                var directoryInfo = CreateDirectory(outputPath);
                var apiList = await ConnectListAPI();
                EditorUtility.ClearProgressBar();

                EditorUtility.DisplayProgressBar("GenerateCode","", 0.0f );
                var count = 0;
                
                CreateBaseSourceFile( directoryInfo );

                foreach( var api in apiList ){
                    EditorUtility.DisplayProgressBar("GenerateCode","Generating api code : "  + api, (float)count / (float)apiList.Length );
                    await GenerateCode(directoryInfo, api);
                    ++count;
                }

                EditorUtility.ClearProgressBar();

                AssetDatabase.Refresh();
            } finally{
                EditorUtility.ClearProgressBar();
            }
        }
        
        public async UniTask GenerateLambda(string outputPath)
        {
            try{
                EditorUtility.DisplayProgressBar("Get API List","ConnectListAPI", 1.0f );
                var directoryInfo = CreateDirectory(outputPath);
                var apiList = await ConnectListAPI();
                EditorUtility.ClearProgressBar();

                EditorUtility.DisplayProgressBar("GenerateCode","", 0.0f );
                var count = 0;
                
                foreach( var api in apiList ){
                    EditorUtility.DisplayProgressBar("GenerateCode","Generating api code : "  + api, (float)count / (float)apiList.Length );
                    await GenerateLambdaCode(directoryInfo, api);
                    ++count;
                }

                EditorUtility.ClearProgressBar();

                AssetDatabase.Refresh();
            } finally{
                EditorUtility.ClearProgressBar();
            }
        }

        protected override string OverrideType( string type ) {
            if( type == "int" ) {
                type = "long";
            } else if( type == "int[]" ) {
                type = "long[]";
            }
            return type;
        }
            

        async UniTask<string[]> ConnectListAPI()
        {
            try{
                var request = new SpecificationGetApiNameListAPIRequest();
                await ConnectAPI(request);
                var response = request.GetResponseData();
                var apiNameList = response.apiNameList.Where(itr => itr == "native-api/specification/forResponseDataCommonParamSchema" || !itr.Contains("/specification/")).ToArray();
                return apiNameList;
            } catch( System.Exception e ){
                Debug.LogError("ConnectListAPI Error : " + e.Message );
                throw e;
            }
        }

        async UniTask<APIResult> ConnectAPI( IAPIRequest request ){
            IAPIConnecter connecter = new APIConnecter();
            request.timeoutSecond = _setting.timeoutSecond;
            var header = CreateBasicAuthHeader();
            SettingRootPostParam( request.GetRootPostData() );

            request.SetHeaders(header);
            var result = await connecter.Connect( _setting.apiURL, request );
            var rawResponseJson = System.Text.Encoding.UTF8.GetString(result.responseData );
            IAPISerializer<EmptyPostData, EmptyResponseData> rootSerializer = new JsonSerializer<EmptyPostData, EmptyResponseData>();
            var rootResponse = rootSerializer.Deserialize( result.responseData );
            request.OnReceivedData( result.responseData, rootResponse.isEncrypted );
            return result;
        }


        Dictionary<string,string> CreateBasicAuthHeader() {
            var setting = new CodeGenerateAPISetting();
            string auth = setting.BCMa5vHjK7pC + ":" + setting.STBE5HfShN8w;
            auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
            auth = "Basic " + auth;
            var header = new Dictionary<string,string>();
            header.Add("AUTHORIZATION", auth);
            return header;
        }

        void SettingRootPostParam( RootPostData postData ){
            postData.reqCount = 1;
            postData.isRetry = false;
#if UNITY_IOS
            postData.osType = 1;

#elif UNITY_ANDROID
            postData.osType = 2;
#else
            postData.osType = 1;
            CruFramework.Logger.LogWarning("The platform may be Standalone. Check your platform");
#endif      
            postData.appVer = "0.0.0";
            postData.assetVer = _setting.assetVersion;
            postData.sessionId = _setting.sessionId;
            postData.loginId = _setting.loginId;
        }


        void CreateBaseSourceFile( DirectoryInfo directoryInfo ) {
            var template = Resources.Load<TextAsset>("APIRequestBaseTemplate");
            var str = template.text;
            var requestFilePath = System.IO.Path.Combine( directoryInfo.requestPath , "AppAPIRequestBase.cs");
            if( System.IO.File.Exists(requestFilePath) ) {
                System.IO.File.Delete( requestFilePath );
            }
            System.IO.File.WriteAllText(requestFilePath, str);

            requestFilePath = System.IO.Path.Combine( directoryInfo.partialPath , "AppAPIRequestBasePartial.cs");
            if( !System.IO.File.Exists(requestFilePath) ) {
                template = Resources.Load<TextAsset>("APIRequestBasePartialTemplate");
                str = template.text;
                System.IO.File.WriteAllText(requestFilePath, str);
            }

        }


        async UniTask GenerateCode( DirectoryInfo directoryInfo, string baseApiName ) {
            var apiName = baseApiName.Replace("native-api/", "");
            var jsonRoot = await GetApiData(baseApiName, apiName);
            var dataRoot = jsonRoot["data"] as Dictionary<string, object>;
            CreateSourceFile( directoryInfo, apiName, dataRoot );
        }

        async UniTask GenerateLambdaCode( DirectoryInfo directoryInfo, string baseApiName ) {
            var apiName = baseApiName.Replace("native-api/", "");
            var jsonRoot = await GetApiData(baseApiName, apiName);
            var dataRoot = jsonRoot["data"] as Dictionary<string, object>;
            CreateLambdaSourceFile( directoryInfo, apiName, dataRoot );
        }
        
        private async UniTask<Dictionary<string, object>> GetApiData(string baseApiName, string apiName)
        {
            var url = System.IO.Path.Combine(_setting.apiURL, apiName);
            var connecter = new HTTPConnector(url, HTTPMethod.Post);
            var request = new SpecificationGetApiSchemaAPIRequest();
            var postData = new SpecificationGetApiSchemaAPIPost();
            postData.apiName = baseApiName; 
            request.SetPostData(postData);
            await ConnectAPI(request);
            var json = System.Text.Encoding.UTF8.GetString(request.rawResponseData);
#if PJFB_ENABLE_API_LOG
            CruFramework.Logger.Log("[API] response : " + json);
#endif            
            return MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
        }

        DirectoryInfo CreateDirectory( string outputPath ){
            var root = System.IO.Directory.GetCurrentDirectory(); 
            var outputDir = System.IO.Path.Combine(root, outputPath);
            var requestRootDir = System.IO.Path.Combine(outputDir, "Request");
            var generatedDir = System.IO.Path.Combine(requestRootDir, "Generated");
            var commonModelDir = System.IO.Path.Combine(requestRootDir,"Generated", "CommonObject");
            var partialDir = System.IO.Path.Combine(requestRootDir, "Partial");
            
            if( System.IO.Directory.Exists(generatedDir) ) {
                System.IO.Directory.Delete(generatedDir, true);
            }
            System.IO.Directory.CreateDirectory(generatedDir);

            if( !System.IO.Directory.Exists(commonModelDir) ) {
                System.IO.Directory.CreateDirectory(commonModelDir);
            }

            if( !System.IO.Directory.Exists(partialDir) ) {
                System.IO.Directory.CreateDirectory(partialDir);
            }
            return new DirectoryInfo(generatedDir, commonModelDir, partialDir);
        }
        
        void CreateSourceFile( DirectoryInfo directory, string apiName, Dictionary<string, object> jsonRoot ){

            CreateAPIRequestFile( directory, apiName, jsonRoot );
            CreatePartialFile( directory, apiName );
            
        }

        void CreateLambdaSourceFile( DirectoryInfo directory, string apiName, Dictionary<string, object> jsonRoot ){

            var responseData = jsonRoot[responseDataRootKey] as Dictionary<string, object>;
            CreateFieldsString(directory, apiName, APICommonLambdaModelTemplateName, responseData, LambdaTargetModelFileNames, true);
        }
        
        void CreateAPIRequestFile( DirectoryInfo directory, string apiName,  Dictionary<string, object> jsonRoot ){
            var postDataJson = jsonRoot[postDataRootKey] as Dictionary<string, object>;
            var responseData = jsonRoot[responseDataRootKey] as Dictionary<string, object>;
            var apiDescription = jsonRoot[apiDescriptionKey] as string;
            
            var postField = CreateFieldsString(directory, apiName, APICommonModelTemplateName, postDataJson);
            var responseField = CreateFieldsString(directory, apiName, APICommonModelTemplateName, responseData);
            var className = apiNameToClassName(apiName);

            var template = Resources.Load<TextAsset>("APIRequestTemplate");
            var str = template.text;
            str = str.Replace("%API_NAME%", apiName);
            str = str.Replace("%API_DESCRIPTION%", apiDescription);
            str = str.Replace("%CLASS_NAME%", className);
            str = str.Replace("%POST_FIELD%", postField);
            str = str.Replace("%RESPONSE_FIELD%", responseField);

            var requestFilePath = System.IO.Path.Combine( directory.requestPath ,  className + "APIRequest.cs");
            System.IO.File.WriteAllText(requestFilePath, str);
        }

        void CreatePartialFile( DirectoryInfo directory, string apiName ){
            var className = apiNameToClassName(apiName);
            var partialFilePath = System.IO.Path.Combine( directory.partialPath ,  className + "APIRequestPartial.cs");
            if( !System.IO.File.Exists(partialFilePath) ) {
                var template = Resources.Load<TextAsset>("APIRequestPartialTemplate");
                var str = template.text;
                str = str.Replace("%CLASS_NAME%", className);
                System.IO.File.WriteAllText(partialFilePath, str);
            }
        }


        
        

        string CreateFieldsString( DirectoryInfo directory, string apiName, string templateName, Dictionary<string, object> rootJson , string[] createOnlyTheseFile = null, bool addGetSetProperty = false)
        {
            var str = "";
            if( rootJson == null ) {
                return str;
            }
            foreach( var itr in rootJson ){
                try{
                    if( IsFieldInfo( itr ) ) {
                        var fieldInfo = new FieldInfo(itr, this);
                        if( !string.IsNullOrEmpty(fieldInfo.type) ) {
                            str += CreateFieldString(fieldInfo, addGetSetProperty) + "\n";
                        } else {
                            Debug.LogWarning("fieldInfo.type is empty: " + fieldInfo.name);
                        }
                        
                    } else {
                        CreateCommonModelFile( directory.commonModelPath, templateName, itr, 0 , createOnlyTheseFile, addGetSetProperty);
                        var fieldInfo = new FieldInfo(itr, this);
                        //baseで定義されるものはスキップ
                        if(  fieldInfo.name == "itemContainer" || fieldInfo.name == "prizeJsonList" || fieldInfo.name == "autoSell" ) {
                            continue;
                        }

                        if( !string.IsNullOrEmpty(fieldInfo.type) ) {
                            str += CreateFieldString(fieldInfo, addGetSetProperty) + "\n";
                        } else {
                            Debug.LogWarning("fieldInfo.type is empty: " + fieldInfo.name);
                        }
                    }
                    
                } catch( System.Exception e ){
                    Debug.LogError("analysis json error : " + e.Message);
                    Debug.LogError("StackTrace : " + e.StackTrace);
                    throw e;
                }
            }
            return str;
        }


        public string apiNameToClassName( string val ){
            val = HyphenToCamel(val);
            var strArrayData = val.Split('/');
            var resValue = "";

            foreach (var str in strArrayData)
            {
                if (str.Length <= 0)
                    continue;

                resValue += char.ToUpper(str[0]) + str.Substring(1);
            }

            return string.IsNullOrEmpty(resValue) ? val : resValue;
        }

        public string HyphenToCamel( string self )
        {
            var strArrayData = self.Split('-');
            var resValue = "";

            foreach (var str in strArrayData)
            {
                if (str.Length <= 0)
                    continue;

                resValue += char.ToUpper(str[0]) + str.Substring(1);
            }

            return string.IsNullOrEmpty(resValue) ? self : resValue;
        }

    }

}
