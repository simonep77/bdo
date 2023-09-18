/*--------------------------------------

  Autore: Simone Pelaia (c)
  Data  : Data: $(DATE) Time: $(TIME)
 --------------------------------------*/

using System;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Business.Data.Objects.Database
{
    /// <summary>
    /// Accesso dati sql server con driver Microsoft.Data
    /// </summary>
    public class MSSQLMDDataBase : MSSQL2005DataBase
    {
        protected override string ProviderAssembly => @"Microsoft.Data.SqlClient";
        protected override string ProviderFactoryClass => @"Microsoft.Data.SqlClient.SqlClientFactory";

        /// <summary>
        /// Costruttore base 
        /// </summary>
        /// <param name="connString"></param>
        public MSSQLMDDataBase(string connString)
            : base(connString)
        {
        }

        /// <summary>
        /// Costruttore specifico
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        public MSSQLMDDataBase(DbConnection conn, DbTransaction tran)
            : base(conn, tran)
        {
        }

    }
}
