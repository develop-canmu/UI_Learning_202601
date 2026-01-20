namespace PolyQA
{
    public static class Const
    {
        public static int NetworkPort = 8818;
        public static int SearchPort = 8819;
        public static string Version = "0.13.0";
        public static int ProtocolVersion = 7;
    }

    public static class RPC
    {
        public const string StartSession = "StartSession";
        public const string EndSession = "EndSession";
        public const string Heartbeat = "Heartbeat";
        public const string GameObjectUpdate = "GameObjectUpdate";
        public const string GetInteractableGameObjects = "GetInteractableGameObjects";
        public const string FindGameObject = "FindGameObject";
        public const string FindGameObjects = "FindGameObjects";
        public const string GetGameObjectPath = "GetGameObjectPath";
        public const string GetGameObjectActive = "GetGameObjectActive";
        public const string GetGameObjectText = "GetGameObjectText";
        public const string GetGameObjectImageName = "GetGameObjectImageName";
        public const string GetGameObjectCheckboxValue = "GetGameObjectCheckboxValue";
        public const string GetGameObjectScreenPosition = "GetGameObjectScreenPosition";
        public const string GetGameObjectScreenRect = "GetGameObjectScreenRect";
        public const string ReleaseGameObjectReference = "ReleaseGameObjectReference";
        public const string ReleaseObjectReference = "ReleaseObjectReference";
        public const string GetComponent = "GetComponent";
        public const string IsEnabledComponent = "IsEnabledComponent";
        public const string GetComponentValue = "GetComponentValue";
        public const string RegisterInputCommand = "RegisterInputCommand";
        public const string UnRegisterInputCommand = "UnRegisterInputCommand";
        public const string InvokeCommand = "InvokeCommand";
        public const string InputAction = "InputAction";
        public const string StartInputRecording = "StartInputRecording";
        public const string StopInputRecording = "StopInputRecording";
        public const string InputRecordUpdate = "InputRecordUpdate";
        public const string StartScreenRecording = "StartScreenRecording";
        public const string ScreenshotRequest = "ScreenshotRequest";

        public const string DataSendString = "DataSendString";
        public const string DataSendInt = "DataSendInt";
        public const string DataSendFloat = "DataSendFloat";
        public const string DataSendBool = "DataSendBool";
        public const string DataSendVector2 = "DataSendVector2";
        public const string DataSendVector3 = "DataSendVector3";
        public const string DataSendVector4 = "DataSendVector4";
        public const string DataSendQuaternion = "DataSendQuaternion";
        public const string DataSendColor = "DataSendColor";
        public const string DataSendByteArray = "DataSendByteArray";
    }
}