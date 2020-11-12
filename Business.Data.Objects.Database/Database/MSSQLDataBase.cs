/*--------------------------------------

  Autore: Simone Pelaia (c)
  Data  : Data: $(DATE) Time: $(TIME)
 --------------------------------------*/

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Business.Data.Objects.Database
{
	/// <summary>
	/// Description of MSSQLDataBase.
	/// </summary>
	public class MSSQLDataBase: CommonDataBase 
	{
		
		public MSSQLDataBase(string connString):base(connString)
		{	
			//Init
            this.InitByFactory(System.Data.SqlClient.SqlClientFactory.Instance);
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


        public override DbParameter AddParameter(string name, object value, Type type)
        {
            var p = this.CreateParameter(name, value, type);
            
            //Fix per query conversioni varchar -> navarchar
            if (p.DbType == System.Data.DbType.String && value != null)
            {
                string s = value as string;
                if (s.Length < 8000)
                    p.DbType = System.Data.DbType.AnsiString;
            }

            base.AddParameter(p);
            return p;
        }


    }
}
