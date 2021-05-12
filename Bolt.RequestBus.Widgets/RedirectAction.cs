namespace Bolt.RequestBus.Widgets
{
    public sealed class RedirectAction
    {
        public string Url { get; set; }
        public bool IsPermanent { get; set; }
    }
}