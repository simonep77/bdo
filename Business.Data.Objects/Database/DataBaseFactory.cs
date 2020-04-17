using System;
using System.Configuration;
using System.Reflection;
using System.Data.Common;

namespace Bdo.Database 
{
	/// <summary>
	/// Description of DataBaseFactory.
	/// </summary>
	public static class DataBaseFactory
	{

		/// <summary>
		/// Crea Un IDataBase partendo dalla ConnectionString con name "DefaultConnection"
		/// </summary>
		/// L'Oggetto DataBase di Tipo Specificato nella ConnectionString dalla direttiva ProviderName
		/// E.S. ProviderName="BusinessLayer.Database.MSSQLDataBase"
		/// </returns>
		public static IDataBase CreaDataBase()
		{
			return DataBaseFactory.CreaDataBase("DefaultConnection");
		}
		
		
		/// <summary>
		/// Crea Un IDataBase partendo dalla ConnectionString con name fornito
		/// </summary>
		/// <returns>
		/// L'Oggetto DataBase di Tipo Specificato nella ConnectionString dalla direttiva ProviderName
		/// E.S. ProviderName="BusinessLayer.Database.MSSQLDataBase"
		/// </returns>
		public static IDataBase CreaDataBase(string connStringKey)
		{
			ConnectionStringSettings connSS = ConfigurationManager.ConnectionStrings[connStringKey] ;
			//Verifica che la connectionstring esiste
			if (connSS == null)
				throw new DataBaseException(string.Format(Resources.DatabaseMessages.ConnKey_NotFound, connStringKey));
			
			//Cerca il Tipo Di DataBase Impostato
			if (!string.IsNullOrEmpty(connSS.ProviderName))
			{
				//Cerca Di Instanziare il DataBase Specificato
				return DataBaseFactory.CreaDataBase(connSS.ProviderName, connSS.ConnectionString);
			}
			
			//Ritorna lo standard MSSQL se non è impostato il ProviderName
			return new MSSQLDataBase(connSS.ConnectionString);
		}
		
		
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
                    //case @"PGSQLDATABASE":
                    //    return new PGSQLDataBase(connectionString);
                    //case @"FBDATABASE":
                    //    return new FBDataBase(connectionString);
                    //case @"ACCESSDATABASE":
                    //    return new ACCESSDataBase(connectionString);
                    default:
                        throw new DataBaseException(Resources.DatabaseMessages.Provider_Unknown, tipoDataBase);
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
                throw new DataBaseException(Resources.DatabaseMessages.Provider_Unknown, tipoDataBase);
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
