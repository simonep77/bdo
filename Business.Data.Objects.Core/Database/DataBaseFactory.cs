using System;
using System.Configuration;
using System.Reflection;
using System.Data.Common;
using Business.Data.Objects.Core.Database.Resources;

namespace Business.Data.Objects.Database
{
	/// <summary>
	/// Description of DataBaseFactory.
	/// </summary>
	public static class DataBaseFactory
	{

		
		/// <summary>
		/// Istanzia un oggetto IDataBase dato il nome della classe e la relativa connection string
		/// </summary>
		/// <param name="tipoDataBase"></param>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public static IDataBase CreaDataBase(string tipoDataBase, string connectionString)
		{
            try
            {
                tipoDataBase = tipoDataBase.ToUpper();

                //Crea database per nome
                switch (tipoDataBase)
                { 
                    case @"MYSQLDATABASE":
                        return new MYSQLDataBase(connectionString);
                    case @"MSSQL2005DATABASE":
                        return new MSSQL2005DataBase(connectionString);
                    case @"MSSQL2012DATABASE":
                        return new MSSQL2012DataBase(connectionString);
                    case @"MSSQLDATABASE":
                        return new MSSQLDataBase(connectionString);
                    case @"SQLITEDATABASE":
                        return new SQLITEDataBase(connectionString);
                    default:
                        throw new DataBaseException(DatabaseMessages.Provider_Unknown, tipoDataBase);
                }
            }
            catch (TargetInvocationException ex)
            {
                throw new DataBaseException(@"DataBaseFactory.CreaDataBase - Errore in fase di creazione dell'oggetto database: {0}", ex.InnerException.Message);
            }
            catch (Exception e)
            {
                throw new DataBaseException(@"DataBaseFactory.CreaDataBase - Errore in fase di creazione dell'oggetto database: {0}", e.Message);
            }
			
		}


        /// <summary>
        /// Crea istanza db partendo da connection
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static IDataBase CreaDataBaseFromADO(DbConnection conn, DbTransaction tran)
        {
            string tipoDataBase = string.Empty;
            try
            {
                tipoDataBase = conn.GetType().ToString().ToUpper();
                CommonDataBase db = null;


                //Crea database per nome
                if (tipoDataBase.StartsWith("SYSTEM.DATA.SQLCLIENT", StringComparison.Ordinal))
                    db = new MSSQL2005DataBase(conn, tran);
                else if (tipoDataBase.StartsWith("MYSQL.", StringComparison.Ordinal))
                    db = new MYSQLDataBase(conn, tran);
                else
                    throw new NotImplementedException(string.Concat("DataBaseFactory.CreaDataBaseFromADO - La connessione specificata non e' gestita: ", tipoDataBase));

                return db;
            }
            catch (ArgumentException)
            {
                throw new DataBaseException(DatabaseMessages.Provider_Unknown, tipoDataBase);
            }
            catch (TargetInvocationException ex)
            {
                throw new DataBaseException(@"DataBaseFactory.CreaDataBase - Errore in fase di creazione dell'oggetto database: {0}", ex.InnerException.Message);
            }
            catch (Exception e)
            {
                throw new DataBaseException(@"DataBaseFactory.CreaDataBase - Errore in fase di creazione dell'oggetto database: {0}", e.Message);
            }

        }
	}
}
