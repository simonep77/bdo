using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Bdo.Schema.Definition;

namespace Bdo.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
    public class Table: BaseAttribute 
    {
        public string DbConnectionKey { get; set; }
        public string DbPrefixKey { get; set; }
        public string Name { get; set; }
        internal string SQL_Select_Item { get; set; }
        internal string SQL_Select_List { get; set; }
        internal string SQL_Insert { get; set; }
        internal string SQL_Select_Reload { get; set; }

        /// <summary>
        /// Indica se il nome tabella e' utilizzabile da solo e non necessita di aggiunte di nomi db
        /// </summary>
        public bool IsSimpleTableName
        {
            get
            {
                return (this.DbPrefixKey == null);
            }
        }

        /// <summary>
        /// Imposta nome standard di tabella
        /// </summary>
        /// <param name="tableName"></param>
        public Table(string tableName)
            : this(tableName, null, null)
        {
        }


        /// <summary>
        /// Imposta il nome della tabella ed il nome della chiave del prefisso db che andra' inserito nello slot a runtime
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbPrefixKey"></param>
        public Table(string tableName, string dbPrefixKey)
            : this(tableName, dbPrefixKey, null)
        {
        }


        /// <summary>
        /// Imposta nome tabella, chiave prefisso db e chiave connessione
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbPrefixKey"></param>
        /// <param name="dbConnKey"></param>
        public Table(string tableName, string dbPrefixKey, string dbConnKey)
        {
            this.Name = tableName;
            this.DbPrefixKey = dbPrefixKey;
            this.DbConnectionKey = dbConnKey;
        }


    }
}
