using System.Collections.Generic;
using System.Linq;

namespace Bolt.FluentHttpClient.Fluent
{
    internal static class NameValueUnitCollectionMapper
    {
        public static NameValueUnit[] FromDto<T>(T data)
        {
            var properties = data.GetType().GetProperties().Where(x => x.CanRead).ToArray();

            var result = new NameValueUnit[properties.Length];

            for (var i = 0; i < properties.Length; i++)
            {
                var prop = properties[i];

                var value = prop.GetValue(data)?.ToString();

                result[i] = new NameValueUnit
                {
                    Name = prop.Name,
                    Value = value
                };
            }

            return result;
        }

        public static NameValueUnit[] FromDictionary(Dictionary<string,string> data)
        {
            var result = new NameValueUnit[data.Count];

            var index = 0;

            foreach (var item in data)
            {
                result[index] = new NameValueUnit
                {
                    Name = item.Key,
                    Value = item.Value
                };

                index++;
            }

            return result;
        }

        public static NameValueUnit[] FromDictionary(IDictionary<string, string> data)
        {
            var result = new NameValueUnit[data.Count];

            var index = 0;

            foreach (var item in data)
            {
                result[index] = new NameValueUnit
                {
                    Name = item.Key,
                    Value = item.Value
                };

                index++;
            }

            return result;
        }
    }
}
