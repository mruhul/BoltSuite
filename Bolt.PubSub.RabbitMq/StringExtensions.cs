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

        private const string UtcFormat = "o";
        public static string ToUtcFormat(this System.DateTime source) 
            => source.Kind == System.DateTimeKind.Utc 
                ? source.ToString(UtcFormat)
                : source.ToUniversalTime().ToString(UtcFormat);

        /// <summary>
        /// Convert a datetime to string using DateTimeStyles.RoundtripKind ("o"). When the date is not UTC kind 
        /// the function first convert it to universal time before format to string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToUtcFormat(this System.DateTime? source)
            => source == null 
                ? null 
                : source.Value.ToUtcFormat();

        /// <summary>
        /// Try Parse a string assuming the datetime has been converted to string using format DateTimeStyles.RoundtripKind ("o")
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static System.DateTime? TryParseUtcFormat(this string date)
            => System.DateTime.TryParse(date, null, System.Globalization.DateTimeStyles.RoundtripKind, out var result) 
                ? result 
                : null;

        /// <summary>
        /// Parse a string assuming the datetime has been converted to string using format DateTimeStyles.RoundtripKind ("o")
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static System.DateTime ParseUtcFormat(this string date)
            => System.DateTime.Parse(date, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }
}
