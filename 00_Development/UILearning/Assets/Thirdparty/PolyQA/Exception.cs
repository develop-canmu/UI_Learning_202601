using System;

namespace PolyQA
{
    public class ConnectException : Exception
    {
        public ConnectException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}