using System;

namespace GotServerLibrary
{
    public class GotServerSendDataException : GotServerException
    {
        public GotServerSendDataException(Exception innerException)
            : base(ExceptionType.SendData, innerException)
        { }
    }

    public class GotServerRecieveDataException : GotServerException
    {
        public GotServerRecieveDataException(Exception innerException)
            : base(ExceptionType.RecieveData, innerException)
        { }
    }

    public class GotServerInitializationException : GotServerException
    {
        public GotServerInitializationException(Exception innerException)
            : base(ExceptionType.Initialization, innerException) { }
    }

    public class GotServerException : Exception
    {
        protected enum ExceptionType
        {
            Initialization,
            SendData,
            RecieveData
        }

        protected ExceptionType Type { get; }

        public override string Message
        {
            get
            {
                return string.Format("{0} Error: {1} : {2}", Type, InnerException.GetType(),
                    InnerException.Message);
            }
        }

        protected GotServerException(ExceptionType type, Exception innerException)
            : base("", innerException)
        {
            Type = type;
        }
    }
}
