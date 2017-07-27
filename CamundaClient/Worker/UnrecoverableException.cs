using System;

namespace CamundaClient.Worker
{
    public class UnrecoverableException : Exception
    {
        public string BusinessErrorCode { get; set; }
        
        public UnrecoverableException(string businessErrorCode, string message)
        : base(message)
        {
            BusinessErrorCode = businessErrorCode;
        }

    }
}
