namespace Bolt.RequestBus.Widgets
{
    public sealed record RedirectAction
    {
        public string Url { get; init; }
        public bool IsPermanent { get; init; }
    }
}