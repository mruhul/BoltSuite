namespace Bolt.RequestBus
{
    public record Error
    {
        public string Code { get; init; }
        public string Message { get; init; }
        public string PropertyName { get; init; }

        public static Error Create(string message, string propertyName = null, string code = null)
            => new()
            {
                Message = message,
                Code = code,
                PropertyName = propertyName
            };
    }
}