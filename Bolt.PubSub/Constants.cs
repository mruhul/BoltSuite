namespace Bolt.PubSub
{
    public static class HeaderNames
    {
        public const string MsgId = "msg-id";
        public const string MessageType = "msg-type";
        public const string Version = "msg-version";
        public const string CreatedAt = "created-at";
        public const string PublishedAt = "published-at";
        public const string Cid = "cid";
        public const string AppId = "app-id";
        public const string DeliveryCount = "delivery-count";
    }

    public static class ContentTypeNames
    {
        public const string Json = "application/json";
    }
}
