using Pjfb.Networking.API;


namespace Pjfb.Networking.App {
    
    public class PjfbLocalAPIConnecter : LocalAPIConnecter{
        
        protected override int connectionVirtualMilliseconds{
            get {
                return 1000 * 3;
            } 
        }
        
        public override string CreateDummyJsonData(IAPIRequest request) {
            return @"{ ""code"": 0,""data"": {""data1"":""aaa"", ""data2"":100,""data3"":false}}";
        }
    }
}