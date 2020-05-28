namespace Bolt.RequestBus
{
    public interface IError
    {
        string Code { get; }
        string Message { get; }
        string PropertyName { get; }
    }
}