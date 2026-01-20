namespace PolyQA.Agent
{
    public class Agent
    {
        public string IPAddress { get; }
        
        public Agent(string ipAddress)
        {
            IPAddress = ipAddress;
        }
    }
}