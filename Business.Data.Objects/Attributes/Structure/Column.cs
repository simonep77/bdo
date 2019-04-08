using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Consente di definire più specificamente una colonna in tabella
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class Column: BaseAttribute 
    {
        private string mName;
        private string mParamName;

        /// <summary>
        /// Tipo DB
        /// </summary>
        public Type DbType { get; set; }

        /// <summary>
        /// Nome della colonna
        /// </summary>
        public string Name
        {
            get { return this.mName; }
        }

        /// <summary>
        /// Ritorna nome parametro
        /// </summary>
        public string ParamName
        {
            get 
            { 
                return this.mParamName; 
            }
        }

        /// <summary>
        /// Costruttore semplice con nome colonna
        /// </summary>
        /// <param name="columnName"></param>
        public Column(string columnName)
            : this(columnName, null, null)
        {
        }

        /// <summary>
        /// Costruttore con nome colonna e tipo db
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="dbType"></param>
        public Column(string columnName, Type dbType)
            : this(columnName, dbType, null)
        {
        }

        /// <summary>
        /// Costruttore completo con nome colonna, tipo db e nome parametro
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="dbType"></param>
        /// <param name="paramName"></param>
        public Column(string columnName, Type dbType, string paramName)
        {
            this.mName = columnName;
            this.DbType = dbType;
            this.mParamName = string.Intern(string.Concat(@"@", paramName ?? columnName));
        }

        /// <summary>
        /// Ritorna il nome del parametro associato
        /// </summary>
        /// <returns></returns>
        public string GetKeyParamName()
        {
            return string.Concat(this.ParamName, "PK");
        }

    }
}
