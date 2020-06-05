using System.Collections.Generic;
using System.Linq;
using Bolt.App.Core;

namespace Bolt.Tenancy.Impl
{
    internal interface IConfigAggregator<T>
    {
        CaseInsensitiveDictionary<T> Get();
    }
    
    internal sealed class ConfigAggregator<T> : IConfigAggregator<T>
    {
        private readonly IEnumerable<ITenantConfigSetup<T>> _setups;
        private readonly ConfigSectionReader _configReader;
        private static volatile CaseInsensitiveDictionary<T> _data;
        private static readonly object _lock = new object();

        public ConfigAggregator(IEnumerable<ITenantConfigSetup<T>> setups, ConfigSectionReader configReader)
        {
            _setups = setups;
            _configReader = configReader;
        }

        public CaseInsensitiveDictionary<T> Get()
        {
            if (_data != null) return _data;

            lock (_lock)
            {
                if (_data != null) return _data;
                
                var result = new CaseInsensitiveDictionary<T>();

                if (_setups == null || !_setups.Any())
                {
                    result = _configReader.Read<T>()?.Settings ?? new CaseInsensitiveDictionary<T>();
                }
                else
                {
                    foreach (var setup in _setups)
                    {
                        var value = setup.Get();

                        if (value != null)
                        {
                            foreach (var item in value)
                            {
                                if (item.Value != null)
                                {
                                    result[item.Key] = item.Value;
                                }
                            }
                        }
                    }
                }

                _data = result;
            }

            return _data;
        }
    }
}