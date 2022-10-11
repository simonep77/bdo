using Business.Data.Objects.Core.Database.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Business.Data.Objects.Database
{
    /// <summary>
    /// Classe lista database
    /// </summary>
    public class DatabaseList: Dictionary<string, IDataBase>
    {

        private bool IsInTransaction;
        private IsolationLevel TransactionIsolation = IsolationLevel.Unspecified;

        public DatabaseList()
            :base(2)
        { }

        public DatabaseList(int capacity)
            : base(capacity)
        { }

        #region PUBLIC METHODS


        private void transactionSetStart(IsolationLevel level)
        {
            this.IsInTransaction = true;
            this.TransactionIsolation = level;
        }

        private void transactionSetStop()
        {
            this.IsInTransaction = false;
            this.TransactionIsolation = IsolationLevel.Unspecified;
        }


        public new void Add(string key, IDataBase db)
        {
            base.Add(key, db);

            if (this.IsInTransaction)
            {
                if (this.TransactionIsolation == IsolationLevel.Unspecified)
                    db.BeginTransaction();
                else
                    db.BeginTransaction(this.TransactionIsolation);
            }

        }


        /// <summary>
        /// Apre tutte le connessioni (se non aperte)
        /// </summary>
        public void OpenAll()
        {
            foreach (var db in this.Values)
            {
                db.OpenConnection();
            }
        }

        /// <summary>
        /// Chiude tutte le connessioni
        /// </summary>
        public void CloseAll(bool rollbackUncommitted)
        {
            foreach (var db in this.Values)
            {
                db.CloseConnection(rollbackUncommitted);
            }
        }


        /// <summary>
        /// Apre tutte le transazioni con un dato isolation level.
        /// Se fornito "Unspecified" viene utilizzato quello di default per ciascuna tipologia di db
        /// </summary>
        /// <param name="level"></param>
        public void BeginTransAll(System.Data.IsolationLevel level)
        {
            if (this.IsInTransaction)
                throw new DataBaseException(DatabaseMessages.Transaction_DbList_OnlyOne);

            this.TransactionIsolation = level;

            foreach (var db in this.Values)
            {
                if (level == IsolationLevel.Unspecified)
                    db.BeginTransaction();
                else
                    db.BeginTransaction(level);

            }

            this.transactionSetStart(level);
        }


        /// <summary>
        /// Apre transazione su tutti i db
        /// </summary>
        public void BeginTransAll()
        {
            this.BeginTransAll(IsolationLevel.Unspecified);
        }


        /// <summary>
        /// Esegue Commit su tutti i database
        /// </summary>
        public void CommitAll()
        {
            foreach (var db in this.Values)
            {
                while (db.IsInTransaction)
                {
                    db.CommitTransaction();
                }
            }

            this.transactionSetStop();
        }


        /// <summary>
        /// Esegue il rollback su tutti i database
        /// </summary>
        public void RollbackAll()
        {
            foreach (var db in this.Values)
            {
                while (db.IsInTransaction)
                {
                    //E' necessario provare ad eseguire TUTTI i rollback a prescindere dalle eccezioni
                    try
                    {
                        db.RollbackTransaction();
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            this.transactionSetStop();

        }

        /// <summary>
        /// Statistiche unificate
        /// </summary>
        /// <returns></returns>
        public DBStats GetAllStats()
        {
            DBStats oNewStat = new DBStats();

            foreach (var db in this.Values)
            {
                oNewStat.Sum(db.Stats);
            }

            return oNewStat;
        }

        #endregion
    }
}
