using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Bdo.Schema.Definition;
using System.Data;

namespace Bdo.Attributes
{
    /// <summary>
    /// Consente di impostare un tipo di dato specifico del provider da utilizzare nelle query
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
    public class DbDataType: BaseAttribute 
    {
        /// <summary>
        /// Tipo di dato del provider
        /// </summary>
        public DbType Value { get; }



        /// <summary>
        /// Imposta il tipo di dato forzato da utilizzare nelle query
        /// </summary>
        /// <param name="type"></param>
        public DbDataType(DbType type)
        {
            this.Value = type;
        }


    }
}