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
                //    diz.Add(item.Name.ToUpper(), this.Convert(CreateAction<T>(item)));
                    diz.Add(item.Name.ToUpper(), this.CreateAction<T>(item));
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


        //private Action<T, object> CreateAction<T>(PropertyInfo property)
        //{
        //    //var type = property.DeclaringType;

        //    var instanceParam = Expression.Parameter(typeof(T));
        //    var argumentParam = Expression.Parameter(typeof(Object));

        //    //return Expression.Lambda<Action<T, Object>>(
        //    //               Expression.Call(instanceParam, property.SetMethod, Expression.Convert(argumentParam, property.PropertyType)),
        //    //               instanceParam, argumentParam
        //    //             ).Compile();

        //    return         //    return Expression.Lambda<Action<T, Object>>(
        //       Expression.Call(instanceParam, property.SetMethod, Expression.Convert(argumentParam, property.PropertyType)),
        //       instanceParam, argumentParam
        //     ).Compile();
        //}

        private Action<object, object> CreateAction<T>(PropertyInfo property)
        {
            var instanceParam = Expression.Parameter(typeof(T));
            var argumentParam = Expression.Parameter(typeof(Object));

            var act =  Expression.Lambda<Action<T, Object>>(
                           Expression.Call(instanceParam, property.SetMethod, Expression.Convert(argumentParam, property.PropertyType)),
                           instanceParam, argumentParam
                         ).Compile();

            return new Action<object, object>((o, o1) => { var castObj = (T)Convert.ChangeType(o, typeof(T)); act(castObj, Convert.ChangeType(o1, property.PropertyType)); });
        }

        //public Action<object,object> Convert<T>(Action<T, object> myActionT)
        //{
        //    if (myActionT == null) return null;

        //    var t = typeof(T);
        //    return new Action<object, object>((o, o1) => myActionT((T)o, o1));
        //}

        //var _actions = new ConcurrentDictionary<Type, Action<object>>();
        //Action<string> actionStr = s => Console.WriteLine(s);
        //var actionObj = new Action<object>(obj => { var castObj = (V)Convert.ChangeType(obj, typeof(V)); actionStr(castObj); });
        //_actions.TryAdd(typeof(string), actionObj);


    }
}
