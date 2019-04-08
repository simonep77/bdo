using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Database
{
    /// <summary>
    /// Classe lista database
    /// </summary>
    class DatabaseList: Dictionary<string, IDataBase>
    {
        public DatabaseList()
            :base(2)
        { }

        public DatabaseList(int capacity)
            : base(capacity)
        { }

        #region PUBLIC METHODS

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
        /// Apre tutte le transazioni con un dato isolation level
        /// </summary>
        /// <param name="level"></param>
        public void BeginTransAll(System.Data.IsolationLevel level)
        {
            foreach (var db in this.Values)
            {
                db.BeginTransaction(level);
            }
        }


        /// <summary>
        /// Apre transazione su tutti i db
        /// </summary>
        public void BeginTransAll()
        {
            foreach (var db in this.Values)
            {
                db.BeginTransaction();
            }
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
