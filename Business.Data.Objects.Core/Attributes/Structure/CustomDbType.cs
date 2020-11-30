using System;
using System.Data;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Consente di impostare un tipo di dato specifico del provider da utilizzare nelle query
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
    public class CustomDbType: BaseAttribute 
    {
        /// <summary>
        /// Tipo di dato del provider
        /// </summary>
        public DbType Value { get; }



        /// <summary>
        /// Imposta il tipo di dato forzato da utilizzare nelle query
        /// </summary>
        /// <param name="type"></param>
        public CustomDbType(DbType type)
        {
            this.Value = type;
        }


    }
}
