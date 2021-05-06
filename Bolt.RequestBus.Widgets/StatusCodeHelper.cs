namespace Bolt.RequestBus.Widgets
{
    internal static class StatusCodeHelper
    {
        public static bool IsSuccessful(int? statusCode)
        {
            return statusCode == null
                || statusCode == 0
                || (statusCode.Value >= 200 && statusCode.Value <= 299);
        }
    }
}