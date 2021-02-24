using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
    
namespace Boids.Core
{
    public interface IEnumCacheProvider
    {
        IDictionary<string, int> GetNamedValues<TEnum>() where TEnum : Enum;
    }
    
    public class EnumCacheProvider : IEnumCacheProvider
    {	
        private readonly Dictionary<Type, IDictionary<string, int>> _namedValuesByEnum = new Dictionary<Type, IDictionary<string, int>>();

        public IDictionary<string, int> GetNamedValues<TEnum>() where TEnum : Enum
        {
            if (_namedValuesByEnum.ContainsKey(typeof(TEnum)))
            {
                Console.WriteLine($"Named values of type {typeof(TEnum)} already cached");
			
                return _namedValuesByEnum[typeof(TEnum)];
            }

            Console.WriteLine($"Cache does not contain named values of type {typeof(TEnum)}");
		
            Console.WriteLine($"Caching named values of type {typeof(TEnum)}:");
		
            var namedValues = new Dictionary<string, int>();

            var values = Enum.GetValues(typeof(TEnum));
		
            foreach (int value in values)
            {
                var name = Enum.GetName(typeof(TEnum), value);
                
                if (name == null)
                    throw new InvalidOperationException($"Could not get name for value {value}");
                
                namedValues.Add(name, value);
			
                Console.WriteLine($"\t'{name}' = {value}");
            }

            var readonlyNamedValues = new ReadOnlyDictionary<string, int>(namedValues);
            _namedValuesByEnum.Add(typeof(TEnum), readonlyNamedValues);

            return readonlyNamedValues;
        }
    }

}