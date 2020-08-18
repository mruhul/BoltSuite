using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
[assembly: InternalsVisibleTo("Bolt.FluentHttpClient.Tests")]

namespace Bolt.FluentHttpClient
{
    internal static class UrlBuilder
    {
        private const char CharQs = '?';
        private const char CharEq = '=';
        private const char CharAmp = '&';
        public static string Build(string url, List<NameValueUnit> queryParams)
        {
            var isQueryStringsEmpty = queryParams == null || queryParams.Count == 0;

            if(isQueryStringsEmpty) return url;

            var qsIndex = url.IndexOf(CharQs);

            var sb = new StringBuilder(url);

            sb.Append(qsIndex == -1 ? CharQs : CharAmp);

            for(var i = 0; i < queryParams.Count; i++)
            {
                sb.Append(queryParams[i].Name).Append(CharEq).Append(queryParams[i].Value);

                if(i < queryParams.Count - 1)
                {
                    sb.Append(CharAmp);
                }
            }

            return sb.ToString();
        }
    }
}
