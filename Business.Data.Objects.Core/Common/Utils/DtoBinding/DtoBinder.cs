using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Business.Data.Objects.Core.Common.Utils
{



    internal class DtoBinder
    {

        private static ConcurrentDictionary<string, TypeMapper> _TypeMapperCache = new ConcurrentDictionary<string, TypeMapper>();


        internal class TypeMapper : Dictionary<string, Action<object, object>>
        { }


        public TypeMapper GetTypeMapper<T>()
        {
            var t = typeof(T);
            //Crea la classe di lettura info
            return _TypeMapperCache.GetOrAdd(t.FullName, (k) => {

                var diz = new TypeMapper();
                foreach (var item in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.DeclaredOnly))
                {
                    diz.Add(item.Name.ToUpper(), CreateAction(item));
                }
                return diz;
            });
        }

        public void MapSingle<T>(TypeMapper mapper, T obj, DbDataReader rd)
        {
            for (int i = 0; i < rd.FieldCount - 1; i++)
            {
                Action<object, object> act;

                if (rd.IsDBNull(i))
                    continue;

                if (!mapper.TryGetValue(rd.GetName(i).ToUpper(), out act))
                    continue;

                act(obj, rd[i]);
            }
        }


        private Action<object, object> CreateAction(PropertyInfo property)
        {
            //var type = property.DeclaringType;

            var instanceParam = Expression.Parameter(typeof(object));
            var argumentParam = Expression.Parameter(typeof(Object));

            return Expression.Lambda<Action<object, Object>>(
                           Expression.Call(instanceParam, property.SetMethod, Expression.Convert(argumentParam, property.PropertyType)),
                           instanceParam, argumentParam
                         ).Compile();
        }


    }
}
