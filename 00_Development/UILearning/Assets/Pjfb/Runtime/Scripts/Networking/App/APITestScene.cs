using UnityEngine;
using Pjfb.Networking.API;
using Pjfb.Networking.HTTP;
using Pjfb.Networking.App;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Cysharp.Threading.Tasks;
using System.IO.Compression;
using System.IO;
public class APITestScene : MonoBehaviour
{
    
    public void OnTapVersion(){
        ConnectVersion().Forget();
    }

    public void OnTapMaster(){
        ConnectMaster().Forget();
    }

    public void OnTapUserCreate(){
        ConnectUserCreate().Forget();
    }

    public void OnTapUserLogin(){
        ConnectUserLogin().Forget();
    }

    public void OnTapDataGet(){
        ConnectDataGet().Forget();
    }

    public void OnTapTest(){
        ConnectTest().Forget();
    }

    public async UniTask ConnectVersion(){
        
        Debug.Log("connect VersionGetAppVerAPIRequest");
        var request = new VersionGetAppVerAPIRequest();
        
        await APIManager.Instance.Connect(request);
        var response = request.GetResponseData();

        Debug.Log("Finished VersionGetAppVerAPIRequest");
    }


    public async UniTask ConnectMaster(){
        
        Debug.Log("connect Master");

        // var request = new MasterDownloadCheckFileAPIRequest();
        // var post = new MasterDownloadCheckFileAPIPost();
        // post.currentMasterVer = 0;
        // request.SetPostData(post);
        // await APIManager.Instance.Connect(request);
        // var response = request.GetResponseData();
        // foreach( var file in response.fileList ){
        //     var url = response.cdnPath + file;
        //     var httpConnector = new HTTPConnector(url, HTTPMethod.Get);
        //     httpConnector.timeoutSecond = APIManager.Instance.setting.timeoutSecond;
        //     var httpResponse = await httpConnector.Connect();
        //     Debug.Log(httpResponse.statusCode);
        //     var data = Pjfb.Storage.IO.DecompressGZip(httpResponse.data);
        //     var json = System.Text.Encoding.UTF8.GetString(data);
            
        //     var path = Pjfb.Storage.IO.AppendPersistentPath("testJson.txt");
        //     Pjfb.Storage.IO.WriteText(path, json);
        //     Debug.Log("path = " + path);
        // }


        Debug.Log("pre-----------");

        foreach( var data in  Pjfb.Master.MasterManager.Instance.abilityMaster.values){
            Debug.Log("data = " + data.name);
        }
        
        await Pjfb.Master.MasterManager.Instance.DownloadMaster();
        Debug.Log("post-----------");
        foreach( var data in  Pjfb.Master.MasterManager.Instance.abilityMaster.values){
            Debug.Log("data = " + data.name);
        }
        Debug.Log("Finished Master");
    }


    public async UniTask ConnectUserCreate(){
        Debug.Log("connect ConnectCreateUser");
        var request = new UserCreateAPIRequest();
        var post = new UserCreateAPIPost();
        post.name = "testUser";
        post.gender = 1;
        post.deviceInfo = Pjfb.Networking.App.APIUtility.CreateDeviceInfo();
        request.SetPostData(post);
        
        await APIManager.Instance.Connect(request);
        var response = request.GetResponseData();
        Debug.Log("Finished ConnectCreateUser");
    }

    public async UniTask ConnectUserLogin(){

        Debug.Log("connect ConnectUserLogin");
        var request = new UserLoginAPIRequest();
        var post = new UserLoginAPIPost();
        post.appToken = LocalSaveManager.immutableData.appToken;
        request.SetPostData(post);
        
        await APIManager.Instance.Connect(request);
        var response = request.GetResponseData();
        Debug.Log("Finished ConnectUserLogin");
    }

    public async UniTask ConnectDataGet(){

        Debug.Log("connect ConnectDataGet");
        var request = new UserGetDataAPIRequest();
        var post = new UserGetDataAPIPost();
        request.SetPostData(post);
        
        await APIManager.Instance.Connect(request);
        var response = request.GetResponseData();

        foreach( var charData in Pjfb.UserData.UserDataManager.Instance.chara.data ) {
            Debug.Log("charData.id = " + charData.Value.id);
            Debug.Log("charData.masterId = " + charData.Value.masterId);
            Debug.Log("charData.charaId = " + charData.Value.charaId);
        }
        Debug.Log("Finished ConnectDataGet");
    }


    public async UniTask ConnectTest(){
        Debug.Log("connect Test");
        var request = new DebugTestAPIRequest();
        await APIManager.Instance.Connect(request);
        var response = request.GetResponseData();
        Debug.Log("Finished Test");
    }

    // Start is called before the first frame update
    void Start()
    {
        
        APIManager.Instance.SetSetting( new PjfbAPISetting() );
        APIManager.Instance.SetErrorHandler( new PjfbAPIErrorHandler() );
        APIManager.Instance.setting.baseURL = APIManager.Instance.setting.versionDownloadURL;

        // var saveData = new ImmutableSaveData();
        // saveData.appToken = "62beb310b2b73_5af9ba59fe8c80cc60d1238727c35033_51fdd49e66477d33e21a1ce0bef89941";
        // LocalSaveManager.Instance.SaveImmutableData(saveData);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
