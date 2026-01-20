using System;

namespace CruFramework.Addressables
{
    public class AddressablesException : Exception
    {
        private AddressablesManager.ErrorCode errorCode = AddressablesManager.ErrorCode.Unknown;
        /// <summary>エラーコード</summary>
        public AddressablesManager.ErrorCode ErrorCode { get { return errorCode; } }
        
        public AddressablesException(AddressablesManager.ErrorCode errorCode, string message, Exception inner) : base(message, inner)
        {
            this.errorCode = errorCode;
        }
    }
}