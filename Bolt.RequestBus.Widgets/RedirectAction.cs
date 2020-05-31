namespace Bolt.RequestBus.Widgets
{
    public interface IRedirectAction
    {
        string Url { get; }
        bool IsPermanent { get; }
    }
    
    internal sealed class RedirectAction : IRedirectAction
    {
        public string Url { get; set; }
        public bool IsPermanent { get; set; }
    }
}