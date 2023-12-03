using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace Boids.Core
{
    public interface IEnumCacheProvider
    {
        IDictionary<string, int> GetNamedValues<TEnum>() where TEnum : Enum;
    }
    
    public class EnumCacheProvider : IEnumCacheProvider
    {	
        private readonly Dictionary<Type, IDictionary<string, int>> _namedValuesByEnum = new Dictionary<Type, IDictionary<string, int>>();
        private readonly ILogger<EnumCacheProvider> _logger;

        public EnumCacheProvider(ILogger<EnumCacheProvider> logger)
        {
            _logger = logger;
        }

        public IDictionary<string, int> GetNamedValues<TEnum>() where TEnum : Enum
        {
            if (_namedValuesByEnum.ContainsKey(typeof(TEnum)))
            {
                _logger.LogInformation("Named values of type {Type} already cached", typeof(TEnum));
			
                return _namedValuesByEnum[typeof(TEnum)];
            }

            _logger.LogInformation("Cache does not contain named values of type {Type}", typeof(TEnum));
		
            _logger.LogInformation("Caching named values of type {Type}:", typeof(TEnum));
		
            var namedValues = new Dictionary<string, int>();

            var values = Enum.GetValues(typeof(TEnum));
		
            foreach (int value in values)
            {
                var name = Enum.GetName(typeof(TEnum), value);
                
                if (name == null)
                    throw new InvalidOperationException($"Could not get name for value {value}");
                
                namedValues.Add(name, value);
			
                _logger.LogInformation("\t{Name} = {Value}", name, value);
            }

            var readonlyNamedValues = new ReadOnlyDictionary<string, int>(namedValues);
            _namedValuesByEnum.Add(typeof(TEnum), readonlyNamedValues);

            return readonlyNamedValues;
        }
    }

}