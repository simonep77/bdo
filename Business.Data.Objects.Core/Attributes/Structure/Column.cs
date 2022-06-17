using System;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Consente di definire più specificamente una colonna in tabella
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class Column: BaseAttribute 
    {

        private readonly static char[] _NormalizeChars = new char[] { '[', ']', '`' };

        /// <summary>
        /// Tipo DB
        /// </summary>
        public Type DbType { get; set; }

        /// <summary>
        /// Nome della colonna
        /// </summary>
        public string Name { get;}

        /// <summary>
        /// Ritorna nome parametro
        /// </summary>
        public string ParamName { get; }

        /// <summary>
        /// Nome del campo normalizzata per usi interni
        /// </summary>
        public string NormalizedName { get; }

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
            this.Name = columnName;
            this.DbType = dbType;
            this.ParamName = string.Intern(string.Concat(@"@", paramName ?? columnName));
            this.NormalizedName = this.Name.Trim(_NormalizeChars);
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
