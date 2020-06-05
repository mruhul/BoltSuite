using System;
using System.Collections.Generic;

namespace Bolt.Tenancy.Impl
{
    internal sealed class CaseInsensitiveDictionary<T> : Dictionary<string, T>
    {
        public CaseInsensitiveDictionary() : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}