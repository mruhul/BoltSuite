namespace Bolt.RequestBus
{
    public interface IError
    {
        string Code { get; }
        string Message { get; }
        string PropertyName { get; }
    }

    public sealed class Error : IError
    {
        private Error()
        {
        }
        
        public string Code { get; internal set; }
        public string Message { get; internal set; }
        public string PropertyName { get; internal set; }
        
        public static IError Create(string msg, string propertyName, string code) => new Error
        {
            Code = code,
            Message = msg,
            PropertyName = propertyName
        };
        
        public static IError Create(string msg, string propertyName) => new Error
        {
            Message = msg,
            PropertyName = propertyName
        };
        
        public static IError Create(string msg) => new Error
        {
            Message = msg
        };
    }
}