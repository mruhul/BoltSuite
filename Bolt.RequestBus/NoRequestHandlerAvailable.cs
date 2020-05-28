using System;

namespace Bolt.RequestBus
{
    public class RequestBusException : ApplicationException
    {
        public RequestBusException(string msg) : base(msg)
        {
        }

        public RequestBusException(string msg, Exception innerException) : base(msg, innerException)
        {
        }
    }
    
    public sealed class NoRequestHandlerAvailable : RequestBusException
    {
        public NoRequestHandlerAvailable(Type type) : base(type.FullName)
        {
        }
    }
}