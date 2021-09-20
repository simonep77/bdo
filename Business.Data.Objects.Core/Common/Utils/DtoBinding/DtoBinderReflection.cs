using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Business.Data.Objects.Core.Common.Utils
{



    internal class DtoBinderReflection
    {

        internal class TypeMapper : Dictionary<string, PropertyInfo>
        { }

        private static ConcurrentDictionary<string, TypeMapper> _TypeMapperCache = new ConcurrentDictionary<string, TypeMapper>();


        public TypeMapper GetTypeMapper<T>()
        {
            var t = typeof(T);
            //Crea la classe di lettura info
            return _TypeMapperCache.GetOrAdd(t.FullName, (k) => {

                var diz = new TypeMapper();
                foreach (var item in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.DeclaredOnly))
                {
                    diz.Add(item.Name.ToUpper(), item);
                }
                return diz;
            });
        }

        public void MapSingle<T>(TypeMapper mapper, T obj, DbDataReader rd)
        {
            for (int i = 0; i < rd.FieldCount; i++)
            {
                PropertyInfo act;

                if (rd.IsDBNull(i))
                    continue;

                if (!mapper.TryGetValue(rd.GetName(i).ToUpper(), out act))
                    continue;

                act.SetValue(obj, Convert.ChangeType(rd[i], act.PropertyType));
            }
        }


        //private Action<object> CreateAction(MethodInfo miCommand)
        //{
        //    // Create the parameter object for the expression, and get
        //    // the type needed for it
        //    ParameterExpression tParam = Expression.Parameter(typeof(T));
        //    Type parameterType = miCommand.GetParameters()[0].ParameterType;

        //    // Create an expression to cast the parameter to the correct type
        //    // for the call
        //    Expression castToType = Expression.Convert(tParam, parameterType, null);

        //    // Create the delegate itself: compile a lambda expression where
        //    // the lambda calls the method miCommand using the instance b and
        //    // passing the result of the cast expression as the argument.
        //    return (Action<T>)Expression.Lambda(
        //            Expression.Call(
        //                Expression.Constant(b, b.GetType()),
        //                miCommand, castToType),
        //            tParam).Compile();
        //}


    }
}
