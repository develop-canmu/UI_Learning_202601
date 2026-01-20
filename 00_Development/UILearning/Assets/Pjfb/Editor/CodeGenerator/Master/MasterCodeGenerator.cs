using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.HTTP;
using Pjfb.Networking.API;
using CruFramework.Extensions;

namespace Pjfb.Editor.Master
{
    public class MasterCodeGenerator : CodeGenerator
    {

        static readonly string MasterDirectory = "Assets/Pjfb/Runtime/Scripts/Master/";
        // static readonly string GenerateDirectory = "Assets/Pjfb/Runtime/Scripts/Master/Generated";

        /// <summary> メッセージパックを利用するか </summary>
        protected override bool useMessagePack => true;

        class DirectoryInfo
        {
            public string generatedPath { get; private set; }
            public string containerPath { get; private set; }
            public string objectPath { get; private set; }
            public string partialContainerPath { get; private set; }
            public string partialObjectPath { get; private set; }
            public string commonModelPath { get; private set; }

            public DirectoryInfo(string generatedPath, string containerPath, string objectPath,
            string partialContainerPath, string partialObjectPath, string commonModelPath)
            {
                this.generatedPath = generatedPath;
                this.containerPath = containerPath;
                this.objectPath = objectPath;
                this.partialContainerPath = partialContainerPath;
                this.partialObjectPath = partialObjectPath;
                this.commonModelPath = commonModelPath;
            }
        }

        IAPISetting _setting = new CodeGenerateAPISetting();

        public void Generate()
        {
            Generate(MasterDirectory).Forget();
        }

        public async UniTask Generate(string outputPath)
        {
            try
            {
                EditorUtility.DisplayProgressBar("Get API List", "ConnectGetMaster", 0.0f);
                var directoryInfo = CreateDirectory(outputPath);
                var root = await ConnectGetMasterAPI();
                var rootRoot = root["data"] as Dictionary<string, object>;
                var schemaList = rootRoot["schemaList"] as List<object>;
                EditorUtility.ClearProgressBar();

                EditorUtility.DisplayProgressBar("GenerateCode", "", 0.0f);
                var masterList = new List<string>();
                var count = 0;
                foreach (var schema in schemaList)
                {
                    EditorUtility.DisplayProgressBar("GenerateCode", "Generating master code", (float)count / (float)schemaList.Count);
                    var masterName = GenerateCode(directoryInfo, schema as Dictionary<string, object>);
                    masterList.Add(masterName);
                    ++count;
                }

                CreateManagerPartialFile(directoryInfo, masterList);
                CreateDeserializeFile(directoryInfo, masterList);

                EditorUtility.ClearProgressBar();

                AssetDatabase.Refresh();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        protected override string OverrideType(string type)
        {
            if (type == "int")
            {
                type = "long";
            }
            else if (type == "int[]")
            {
                type = "long[]";
            }
            return type;
        }

        async UniTask<Dictionary<string, object>> ConnectGetMasterAPI()
        {
            try
            {
                var request = new SpecificationGetMasterClassSchemaListAPIRequest();
                await ConnectAPI(request);
                var json = System.Text.Encoding.UTF8.GetString(request.rawResponseData);
#if PJFB_ENABLE_API_LOG
                CruFramework.Logger.Log("[API] response : " + json);
#endif
                var root = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;

                return root;
            }
            catch (System.Exception e)
            {
                Debug.LogError("GetMasterAPI Error : " + e.Message);
                throw e;
            }
        }

        async UniTask<APIResult> ConnectAPI(IAPIRequest request)
        {
            IAPIConnecter connecter = new APIConnecter();
            request.timeoutSecond = _setting.timeoutSecond;
            var header = CreateBasicAuthHeader();
            SettingRootPostParam(request.GetRootPostData());

            request.SetHeaders(header);
            var result = await connecter.Connect(_setting.apiURL, request);
            IAPISerializer<EmptyPostData, EmptyResponseData> rootSerializer = new JsonSerializer<EmptyPostData, EmptyResponseData>();
            var rootResponse = rootSerializer.Deserialize(result.responseData);
            request.OnReceivedData(result.responseData, rootResponse.isEncrypted);
            return result;
        }


        Dictionary<string, string> CreateBasicAuthHeader()
        {
            var setting = new CodeGenerateAPISetting();
            string auth = setting.BCMa5vHjK7pC + ":" + setting.STBE5HfShN8w;
            auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
            auth = "Basic " + auth;
            var header = new Dictionary<string, string>();
            header.Add("AUTHORIZATION", auth);
            return header;
        }

        void SettingRootPostParam(RootPostData postData)
        {
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
            postData.appVer = "0.0,0";
            postData.assetVer = _setting.assetVersion;
            postData.sessionId = _setting.sessionId;
            postData.loginId = _setting.loginId;
        }


        string GenerateCode(DirectoryInfo directoryInfo, Dictionary<string, object> schema)
        {
            var masterName = CreateMasterName(schema);
            CruFramework.Logger.Log("create : " + masterName);
            var field = schema["field"] as Dictionary<string, object>;
            CreateSourceFile(directoryInfo, masterName, field);
            return masterName;
        }

        DirectoryInfo CreateDirectory(string outputPath)
        {
            var root = System.IO.Directory.GetCurrentDirectory();
            var generatedDir = System.IO.Path.Combine(root, outputPath, "Generated");
            var containerModelDir = System.IO.Path.Combine(generatedDir, "Container");
            var objectDir = System.IO.Path.Combine(generatedDir, "Object");
            var partialDir = System.IO.Path.Combine(root, outputPath, "Partial");
            var partialContainerDir = System.IO.Path.Combine(partialDir, "Container");
            var partialObjectDir = System.IO.Path.Combine(partialDir, "Object");
            var commonModelDir = System.IO.Path.Combine(generatedDir, "CommonObject");

            if (System.IO.Directory.Exists(generatedDir))
            {
                System.IO.Directory.Delete(generatedDir, true);
            }

            if (!System.IO.Directory.Exists(partialDir))
            {
                System.IO.Directory.CreateDirectory(partialDir);
            }

            if (!System.IO.Directory.Exists(partialContainerDir))
            {
                System.IO.Directory.CreateDirectory(partialContainerDir);
            }

            if (!System.IO.Directory.Exists(partialObjectDir))
            {
                System.IO.Directory.CreateDirectory(partialObjectDir);
            }




            System.IO.Directory.CreateDirectory(generatedDir);
            System.IO.Directory.CreateDirectory(containerModelDir);
            System.IO.Directory.CreateDirectory(objectDir);
            System.IO.Directory.CreateDirectory(commonModelDir);

            return new DirectoryInfo(generatedDir, containerModelDir, objectDir, partialContainerDir, partialObjectDir, commonModelDir);
        }

        string CreateMasterName(Dictionary<string, object> schema)
        {
            var rawName = schema["name"] as string;
            var splits = rawName.Split("\\");
            var masterName = splits[splits.Length - 1];
            return masterName.Substring(1).PrefixToUpper();
        }

        void CreateSourceFile(DirectoryInfo directory, string masterName, Dictionary<string, object> field)
        {
            CreateMasterObjectFile(directory, masterName, field);
            CreateContainerFile(directory, masterName);
            CreatePartialFile(directory, masterName);
        }

        void CreateMasterObjectFile(DirectoryInfo directory, string masterName, Dictionary<string, object> field)
        {

            var voPropertyStr = CreateVOPropertyString(directory.commonModelPath, field);
            var fieldStr = CreateMasterFieldsString(field);
            var objPropertyStr = CreateObjectPropertyString(field);
            var template = Resources.Load<TextAsset>("MasterObjectTemplate");
            var str = template.text;
            str = str.Replace("%MASTER_NAME%", masterName);
            str = str.Replace("%VO_PROPERTY%", voPropertyStr);
            str = str.Replace("%VO_FIELD%", fieldStr);
            str = str.Replace("%OBJ_PROPERTY%", objPropertyStr);
            str = str.Replace("%MASTER_NAME_SNAKE%", masterName.CamelToSnake());

            var filePath = System.IO.Path.Combine(directory.objectPath, masterName + "MasterObject.cs");
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            System.IO.File.WriteAllText(filePath, str);
        }


        void CreateContainerFile(DirectoryInfo directory, string masterName)
        {
            var template = Resources.Load<TextAsset>("MasterContainerTemplate");
            var str = template.text;
            str = str.Replace("%MASTER_NAME%", masterName);
            var filePath = System.IO.Path.Combine(directory.containerPath, masterName + "MasterContainer.cs");
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            System.IO.File.WriteAllText(filePath, str);
        }

        void CreatePartialFile(DirectoryInfo directory, string masterName)
        {

            var filePath = System.IO.Path.Combine(directory.partialContainerPath, masterName + "MasterContainerPartial.cs");
            if (!System.IO.File.Exists(filePath))
            {
                var template = Resources.Load<TextAsset>("MasterContainerPartialTemplate");
                var str = template.text;
                str = str.Replace("%MASTER_NAME%", masterName);
                System.IO.File.WriteAllText(filePath, str);
            }


            filePath = System.IO.Path.Combine(directory.partialObjectPath, masterName + "MasterObjectPartial.cs");
            if (!System.IO.File.Exists(filePath))
            {
                var template = Resources.Load<TextAsset>("MasterObjectPartialTemplate");
                var str = template.text;
                str = str.Replace("%MASTER_NAME%", masterName);
                System.IO.File.WriteAllText(filePath, str);
            }
        }

        void CreateManagerPartialFile(DirectoryInfo directory, List<string> masterNameList)
        {
            var propertyFieldStr = CreateManagerPropertyFieldString(masterNameList);
            var methodStr = CreateManagerMethodString(masterNameList);
            var clearStr = CreateManagerClearMethodString(masterNameList);
            var deserializeRootStr = CreateDeserializeRootString(masterNameList);
            var deserializeDataListStr = CreateDeserializeDataListString(masterNameList);

            var template = Resources.Load<TextAsset>("MasterManagerPartialTemplate");
            var str = template.text;
            str = str.Replace("%PROPERTY_FIELD%", propertyFieldStr);
            str = str.Replace("%LOAD_METHOD%", methodStr);
            str = str.Replace("%CLEAR_METHOD%", clearStr);

            str = str.Replace("%DESERIALIZE_FIELD%", deserializeRootStr);
            str = str.Replace("%DESERIALIZE_DATA_LIST%", deserializeDataListStr);

            var filePath = System.IO.Path.Combine(directory.generatedPath, "MasterManagerPartial.cs");
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            System.IO.File.WriteAllText(filePath, str);
        }

        void CreateDeserializeFile(DirectoryInfo directory, List<string> masterNameList)
        {
            var deserializeRootStr = CreateDeserializeRootString(masterNameList);
            var deserializeDataListStr = CreateDeserializeDataListString(masterNameList);

            var template = Resources.Load<TextAsset>("MasterDeserializeTemplate");
            var str = template.text;
            str = str.Replace("%DESERIALIZE_FIELD%", deserializeRootStr);
            str = str.Replace("%DESERIALIZE_DATA_LIST%", deserializeDataListStr);

            var filePath = System.IO.Path.Combine(directory.generatedPath, "MasterDeserializeRoot.cs");
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            System.IO.File.WriteAllText(filePath, str);
        }



        string CreateManagerPropertyFieldString(List<string> masterNameList)
        {
            var str = "";
            foreach (var masterName in masterNameList)
            {
                str += string.Format("\t\tpublic {0}MasterContainer {1}Master{{get;private set;}}", masterName.PrefixToUpper(), masterName.PrefixToLower()) + "\n";
            }
            return str;
        }


        string CreateManagerMethodString(List<string> masterNameList)
        {
            var str = "";
            foreach (var masterName in masterNameList)
            {
                str += string.Format("\t\t\t{0}Master = LoadAndAddMaster<{1}MasterContainer>();", masterName.PrefixToLower(), masterName.PrefixToUpper()) + "\n";
            }
            return str;
        }

        string CreateManagerClearMethodString(List<string> masterNameList)
        {
            var str = "";
            foreach (var masterName in masterNameList)
            {
                str += string.Format("\t\t\t{0}Master = null;", masterName.PrefixToLower()) + "\n";
            }
            return str;
        }

        string CreateDeserializeRootString(List<string> masterNameList)
        {
            var str = "";
            int index = 0;
            foreach (var masterName in masterNameList)
            {
                str += $"\t\t[MessagePack.Key({index})]\n";
                str += string.Format("\t\t\tpublic {0}MasterValueObject[] m_{1} = null;", masterName, masterName.CamelToSnake().ToLower()) + "\n";
                index++;
            }
            return str;
        }

        string CreateDeserializeDataListString(List<string> masterNameList)
        {
            var str = "";
            foreach (var masterName in masterNameList)
            {
                var lowerMasterName = masterName.CamelToSnake().ToLower();
                str += "\t\t\t\t{" + string.Format("\"{0}\",m_{1}", masterName, lowerMasterName) + "}," + "\n";
            }
            return str;
        }



    }
}
