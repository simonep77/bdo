/*--------------------------------------

  Autore: Simone Pelaia (c)
  Data  : Data: $(DATE) Time: $(TIME)
 --------------------------------------*/

using System;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Business.Data.Objects.Core.Database.Resources;

namespace Business.Data.Objects.Database
{
    /// <summary>
    /// Description of MYSQLDataBase.
    /// 
    /// La connection string deve essere del tipo: DataSource=server;database=db;user=xxx;password=xxx
    /// </summary>
    public class MYSQLDataBase : CommonDataBase
    {
        protected override string ProviderAssembly => @"MySql.Data";
        protected override string ProviderFactoryClass => @"MySql.Data.MySqlClient.MySqlClientFactory";


        private static Regex _PAGED_REGEX = new Regex(@"[\s]*(SELECT)[\s]+(?:(DISTINCT)[\s]+)?", System.Text.RegularExpressions.RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region "PROPERTY"

        protected override bool PagedReaderLastRow
        {
            get
            {
                return false;
            }
        }
        public override string LastAutoIdFunction { get; } = @"LAST_INSERT_ID()";

        /// <summary>
        /// Isolamento di default della transazione
        /// </summary>
        public override IsolationLevel TransactionDefaultIsolation { get; set; } = IsolationLevel.RepeatableRead;


        #endregion

        public MYSQLDataBase(string connString)
            : base(connString)
        {
        }


        public MYSQLDataBase(DbConnection conn, DbTransaction tran)
            : base(conn, tran)
        {
        }


        #region "PUBLIC"


        /// <summary>
        /// Ritorna l'Ultimo ID Autoincrement/Identity inserito
        /// </summary>
        /// <returns></returns>
        public override long GetLastAutoId()
        {
            this.SQL = @"SELECT LAST_INSERT_ID()";
            return Convert.ToInt64(this.ExecScalar());
        }


        /// <summary>
        /// Ottiene lock su risorsa
        /// </summary>
        /// <param name="lockName"></param>
        public override void GetLock(string lockName, int timeoutsec)
        {
            this.BeginThreadSafeWork();
            try
            {
                //Preregistra il lock
                this.registerLock(lockName);

                this.SQL = @"SELECT COALESCE(GET_LOCK(@LOCKNAME, @LOCKTM), 0)";
                this.AddParameter("@LOCKNAME", lockName);
                this.AddParameter("@LOCKTM", timeoutsec);
                int iRet = Convert.ToInt32(this.ExecScalar());

                if (iRet == 0)
                {
                    this.TraceLog(DatabaseMessages.Cannot_Get_lock, lockName);
                    throw new DataBaseException(DatabaseMessages.Cannot_Get_lock, lockName);
                }
            }
            finally
            {
                this.EndThreadSafeWork();
            }

        }


        /// <summary>
        /// Rilascia lock su risorsa
        /// </summary>
        /// <param name="lockName"></param>
        public override void ReleaseLock(string lockName)
        {
            this.BeginThreadSafeWork();
            try
            {
                //Deregistra il lock
                this.unregisterLock(lockName);

                this.SQL = @"SELECT COALESCE(RELEASE_LOCK(@LOCKNAME), 2)";
                this.AddParameter("@LOCKNAME", lockName);
                int iRet = Convert.ToInt32(this.ExecScalar());

                //Il lock esiste ed appartiene ad altro thread
                if (iRet == 0)
                {
                    this.TraceLog(DatabaseMessages.Cannot_Release_Lock, lockName);
                    throw new DataBaseException(DatabaseMessages.Cannot_Release_Lock, lockName);
                }
            }
            finally
            {
                this.EndThreadSafeWork();
            }

        }

        private void setQueryPaged(int positionIn, int offsetIn)
        {
            this.setTotPagedRecords(0);
            //Manipola Query aggiungendo direttiva conteggio
            String sTemp = _PAGED_REGEX.Replace(this.SQL, @"$1 SQL_CALC_FOUND_ROWS $2 ", 1);

            System.Text.StringBuilder sb = new System.Text.StringBuilder(sTemp.Length + 300);
            //Manipola Query
            sb.Append(sTemp);
            //Scrive query di conteggio record
            sb.Append(@" LIMIT ");
            sb.Append(positionIn);
            sb.Append(@",");
            sb.Append(offsetIn);
            sb.Append(@";");

            sb.Append(@"SELECT FOUND_ROWS() ");

            //Imposta query
            this.SQL = sb.ToString();
        }


        /// <summary>
        /// Esegue una query e ritorna parte del risultato su dataset come da specifiche
        /// </summary>
        /// <param name="positionIn"></param>
        /// <param name="offsetIn"></param>
        /// <returns></returns>
        public override DataTable Select(int positionIn, int offsetIn)
        {
            this.BeginThreadSafeWork();
            try
            {

                this.setQueryPaged(positionIn, offsetIn);

                //Crea tab
                var ds = this.SelectM();

                //Imposta dati paginazione
                this.setTotPagedRecords(Convert.ToInt32(ds.Tables[1].Rows[0][0]));

                //Rimuove tabella paginazione
                ds.Tables.RemoveAt(1);

                //Ritorna
                return ds.Tables[0];
            }
            finally
            {
                this.EndThreadSafeWork();
            }
        }


        /// <summary>
        /// Esegue query paginata ritornando un DataReader
        /// </summary>
        /// <param name="positionIn"></param>
        /// <param name="offsetIn"></param>
        /// <returns></returns>
        public override DbDataReader ExecReaderPaged(int positionIn, int offsetIn)
        {
            this.BeginThreadSafeWork();
            try
            {
                this.setQueryPaged(positionIn, offsetIn);

                //Ritorna
                return this.ExecReader();
            }
            finally
            {
                this.EndThreadSafeWork();
            }
        }

        #endregion

        
    }



}

