﻿/*--------------------------------------

  Autore: Simone Pelaia (c)
  Data  : Data: $(DATE) Time: $(TIME)
 --------------------------------------*/

using System;
using System.Data;
using System.Data.Common;

namespace Business.Data.Objects.Database
{
	/// <summary>
	/// Description of MSSQLDataBase.
	/// </summary>
	public class MSSQLDataBase: CommonDataBase 
	{
        protected override string ProviderAssembly => @"System.Data.SqlClient";
        protected override string ProviderFactoryClass => @"System.Data.SqlClient.SqlClientFactory";

        public MSSQLDataBase(string connString):base(connString)
		{	
		}

        /// <summary>
        /// Costruttore specifico
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        public MSSQLDataBase(DbConnection conn, DbTransaction tran)
            : base(conn, tran)
        {
        }


        public override string LastAutoIdFunction
        {
            get
            {
                return @"@@IDENTITY";
            }
        }


        /// <summary>
        /// Ritorna l'Ultimo ID Autoincrement/Identity inserito
        /// </summary>
        /// <returns></returns>
        public override long GetLastAutoId()
        {
            this.SQL = @"SELECT @@IDENTITY";
            return Convert.ToInt64(this.ExecScalar());
        }

        /// <summary>
        /// Ottiene il lock di una risorsa con nome
        /// </summary>
        /// <param name="lockName"></param>
        public override void GetLock(string lockName, int timeoutsec)
        {
            //Preregistra il lock
            this.registerLock(lockName);

            //Esegue
            this.CommandType = CommandType.StoredProcedure;
            this.SQL = "sp_getapplock";
            this.AddParameter("@Resource", lockName);
            this.AddParameter("@LockOwner", "Session");
            this.AddParameter("@LockMode", "Exclusive");
            this.AddParameter("@LockTimeout", timeoutsec * 1000); 
            int iRet = Convert.ToInt32(this.ExecScalar());
            //Controllo errori
            if (iRet < 0)
            {
                throw new DataBaseException($"Impossibile ottenere il lock '{lockName}'");
            }
            
        }


        /// <summary>
        /// Rilascia lock precedentemente acquisito
        /// </summary>
        /// <param name="lockName"></param>
        public override void ReleaseLock(string lockName)
        {
            //Preregistra il lock
            this.unregisterLock(lockName);

            //Esegue
            this.CommandType = CommandType.StoredProcedure;
            this.SQL = "sp_releaseapplock";
            this.AddParameter("@Resource", lockName);
            this.AddParameter("@LockOwner", "Session");
            int iRet = Convert.ToInt32(this.ExecScalar());
            //Controllo errori
            if (iRet < 0)
            {
                throw new DataBaseException($"Impossibile rilasciare il lock {lockName}");
            }
        }


    }
}
