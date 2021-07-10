using System.Linq;

namespace Bolt.PubSub.RabbitMq
{
    internal static class StringExtensions
    {
        public static string EmptyAlternative(this string source, string alternative)
            => string.IsNullOrWhiteSpace(source) ? alternative : source;

        public static bool IsEmpty(this string source)
            => string.IsNullOrWhiteSpace(source);

        public static bool HasValue(this string source)
            => !string.IsNullOrWhiteSpace(source);

        public static bool IsSame(this string souce, string compareWith)
            => souce.Equals(compareWith, System.StringComparison.OrdinalIgnoreCase);

        public static bool IsSame(this string source, params string[] compareAnyOfThese)
            => compareAnyOfThese == null 
                ? false 
                : compareAnyOfThese.Any(x => x.IsSame(source));
    }
}
