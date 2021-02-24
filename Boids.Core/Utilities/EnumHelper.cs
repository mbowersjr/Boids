using System;
using System.Collections.Generic;

namespace Boids.Core
{
    public class EnumHelper
    {
        private static readonly Dictionary<Type, Dictionary<int, string>> _namedValuesByEnum = new Dictionary<Type, Dictionary<int, string>>();
        
        public static Dictionary<int, string> GetNamedValues<T>() where T : Enum
        {
            if (_namedValuesByEnum.ContainsKey(typeof(T)))
            {
                return _namedValuesByEnum[typeof(T)];
            }

            var namedValues = new Dictionary<int, string>();

            var values = Enum.GetValues(typeof(T));
            foreach (int item in values)
            {
                namedValues.Add(item, Enum.GetName(typeof(T), item));
            }
            
            _namedValuesByEnum.Add(typeof(T), namedValues);

            return namedValues;
        }
    }
}