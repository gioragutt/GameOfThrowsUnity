using System;

namespace GotServerLibrary
{
    public enum ExceptionType
    {
        Initialization,
        SendData,
        RecieveData
    }

    public class GotServerException : Exception
    {
        public ExceptionType ExceptionType { get; }

        private static string ToExceptionMessage(ExceptionType type, string message)
        {
            return string.Format("{0} Error: {1}", type, message);
        }

        public GotServerException(ExceptionType type, Exception innerException)
            : this(type, innerException.Message, innerException)
        {
            
        }

        public GotServerException(ExceptionType type, string message, Exception innerException) 
            : base(ToExceptionMessage(type, message), innerException)
        {
            ExceptionType = type;
        }
    }
}
