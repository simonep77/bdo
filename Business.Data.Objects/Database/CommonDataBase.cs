/*
 * Creato da SharpDevelop.
 * Utente: spelaia
 * Data: 26/04/2007
 * Ora: 13.00
 * 
 */

using System;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.IO;
using Bdo.Logging;
using Bdo.Utils;

namespace Bdo.Database 
{
	/// <summary>
	/// Classe Astratta da cui derivano quelle specializzate: gran parte delle propriet�
	/// e dei Metodi risiedono qui
	/// </summary>
	public abstract class CommonDataBase: IDataBase 
	{
		
		
		/// <summary>
		/// Contiene la lista di tutti gli assembli Ado caricati nell'Applicazione (Web o Client) corrente
		/// </summary>
		private static Dictionary<string, DbProviderFactory > AssFactoryDictionary = new Dictionary<string, DbProviderFactory>();

        protected const string TABLE_NAME = @"Table1";
		private object _GlobalLock = new object(); //Utilizzato per condividere una unica connection
		private DbProviderFactory _FactoryCorrente;
		private DbConnection _dbconn;
		private DbCommand	_command;
        private Stack _tranQ = new Stack(1);
        private string _connStr = string.Empty;
        private int _TotRecordQueryPaginata;
        private bool _AutoCloseConnection;
        private DBStats _Stats = new DBStats();
        private bool _TraceOnlyErrors;
        private LoggerBase _TraceLog;
        private UInt32 _HashCode;
        private Dictionary<string, object> _LockAcquired; //Istanziato al primo utilizzo
        private IsolationLevel _PendingTransactionLevel = IsolationLevel.Unspecified;

		#region "PROPERTY"

        /// <summary>
        /// Ritorna il nome della funzione per la cattura dell'ultimo Id inserito automaticamente
        /// </summary>
        public virtual string LastAutoIdFunction
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Statistiche di utilizzo
        /// </summary>
        public DBStats Stats
        {
            get {
                return this._Stats;
            }
        }

        /// <summary>
        /// Ritorna tipo database
        /// </summary>
        public string DbType
        {
            get { return this.GetType().Name; }
        }
		
        /// <summary>
        /// Ritorna connection string utilizzata
        /// </summary>
		public string ConnectionString {
			get { return _connStr; }
		}


        /// <summary>
        /// Ritorna o imposta il timeout di esecuzione query (secondi)
        /// </summary>
        public int ExecutionTimeout
        {
            get { return this._command.CommandTimeout; }
            set { this._command.CommandTimeout = value; }
        }

        /// <summary>
        /// Ottiene o imposta la tipologia di comando che si vuole eseguire:
        /// - Sql standard
        /// - Stored Procedure
        /// - Tabella diretta
        /// </summary>
        public CommandType CommandType
        {
            get { return this._command.CommandType; }
            set { this._command.CommandType = value; }
        }

        /// <summary>
        /// ottiene o imposta il comportamento della connessione
        /// </summary>
        public bool AutoCloseConnection
        {
            get {
                return this._AutoCloseConnection;
            }
            set {
                this._AutoCloseConnection = value;
            }
        }
		
		/// <summary>
		/// SQL da eseguire (o eseguito)
		/// </summary>
		public virtual string SQL
		{
			get{
                lock (this._GlobalLock)
                {
                    return this._command.CommandText;
                }
			}
			set{
                lock (this._GlobalLock)
                {
                    this._command.CommandText = value;
                }
			}
		}
		
		
		/// <summary>
		/// Numero Totale Records calcolati dopo l'ultima OpenQuery paginata
		/// </summary>
		public int TotRecordQueryPaginata {
			get 
			{
                lock (this._GlobalLock)
                {
                    return _TotRecordQueryPaginata;
                }
			}
		}

        /// <summary>
        /// Abilita o disabilita la registrazione dei dati di esecuzione
        /// </summary>
        public bool TraceON
        {
            get
            {
                return (this._TraceLog != null);
            }
        }

        /// <summary>
        /// Se TraceON abilita il trace dei soli
        /// errori
        /// </summary>
        public bool TraceOnlyErrors
        {
            get
            {
                return this._TraceOnlyErrors;
            }
        }


        /// <summary>
        /// Indica se si e' in un contesto transazionale
        /// </summary>
        public bool IsInTransaction
        {
            get 
            {
                return (this._command.Transaction != null || this.IsPendingTransaction);
            }
        }


        /// <summary>
        /// Indica se presenti lock in atto
        /// </summary>
        public bool HasAcquiredLocks
        {
            get
            {
                return (this._LockAcquired != null && this._LockAcquired.Count > 0);
            }
        }

        /// <summary>
        /// Indica se e' stato richiesto avvio transazione ma effettivamente ancora non e' stata aperta in quanto non eseguita la connessione
        /// </summary>
        internal bool IsPendingTransaction
        {
            get
            {
                return (this._PendingTransactionLevel != IsolationLevel.Unspecified);
            }
        }

        /// <summary>
        /// Indica se la connessione e' attiva
        /// </summary>
        public bool IsConnectionOpen
        {
            get
            {
                return (this._dbconn != null && this._dbconn.State == ConnectionState.Open);
            }
        }

        /// <summary>
        /// Indica se possibile chiudere la connessione
        /// </summary>
        public bool CanAutoCloseConnection
        {
            get {
                //Se autoclose non attivo non chiude
                if (!this._AutoCloseConnection)
                    return false;

                //Se transazione aperta non chiude
                if (this.IsInTransaction)
                    return false;

                //Se ci sono lock non chiude
                if (this.HasAcquiredLocks)
                    return false;

                //puo' chiudere
                return true;
            }
        }


        /// <summary>
        /// Codice hash della connessione
        /// </summary>
        public UInt32 HashCode
        { get { return this._HashCode; } }

		
		#endregion
		
		#region "PUBLIC"
		
        /// <summary>
        /// Costruttore pubblico
        /// </summary>
        /// <param name="connString"></param>
		public CommonDataBase(string connString)
		{
			this._connStr = connString;
            this._HashCode = BdoHash.Instance.Hash(connString);
		}


        /// <summary>
        /// Clona una connessione DB
        /// </summary>
        /// <returns></returns>
        public IDataBase Clone() {
            IDataBase dbOut = DataBaseFactory.CreaDataBase(this.GetType().Name, this._connStr);
            dbOut.AutoCloseConnection = this.AutoCloseConnection;
            if (this.TraceON)
                dbOut.EnableTrace(this._TraceLog, this._TraceOnlyErrors);

            return dbOut;
        }

        /// <summary>
        /// Abilita logging fornendo un filelogger specifico
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="onlyErrors"></param>
        public void EnableTrace(LoggerBase logger, bool onlyErrors)
        {
            //Crea nuovo log e apre
            this._TraceLog = logger;
            this._TraceOnlyErrors = onlyErrors;
        }

        /// <summary>
        /// Abilita il trace degli statement SQL su file
        /// </summary>
        /// <param name="traceFilePath"></param>
        /// <param name="onlyErrors"></param>
        public void EnableTrace(string traceFilePath, bool onlyErrors)
        {
            //Crea nuovo log e apre
            this._TraceLog = new FileStreamLogger(traceFilePath);
            this._TraceOnlyErrors = onlyErrors;
        }

        /// <summary>
        /// Disabilita trace degli statement SQl
        /// </summary>
        public void DisableTrace()
        {
            if (this._TraceLog != null)
                this._TraceLog.Dispose();

            this._TraceLog = null;
        }
		
		/// <summary>
		/// Inizia nuova transazione
		/// </summary>
		public virtual void BeginTransaction()
		{
            this.BeginTransaction(IsolationLevel.ReadCommitted);
		}

        /// <summary>
        /// Inizia nuova transazione con specifica dell'isolation level
        /// </summary>
        /// <param name="level"></param>
        public void BeginTransaction(IsolationLevel level)
        {
            Exception exLog = null;
             
            lock (this._GlobalLock)
            {
                //Al di fuori del log imposta eventuale pending della transazione
                if (!this.IsConnectionOpen)
                {
                    this.pendingTransSet(level);
                }
                else
                {
                    try
                    {
                        this.OpenConnection();

                        //Crea la nuova transazione (se il db non supporta annidate lancia eccezione)
                        DbTransaction newTran = this.createNewTransaction(level);

                        //Ok esegue accodamento
                        if (this._command.Transaction != null)
                            this._tranQ.Push(this._command.Transaction);

                        //Imposta nuova transazione
                        this._command.Transaction = newTran;

                        //Aggiorna stats
                        this._Stats.Increment(DBStats.EStatement.Begin);
                    }
                    catch (Exception ex)
                    {
                        exLog = ex;
                        throw;
                    }
                    finally
                    {
                        this.TraceStatement("BeginTransaction " + level.ToString(), exLog);
                    }
                }
            }

        }
		
		/// <summary>
		/// Esegue Commit
		/// </summary>
		public void CommitTransaction()
		{
            Exception exLog = null;

            lock (this._GlobalLock)
            {
                //Se transazione non realmente aperta
                if (this.IsPendingTransaction)
                {
                    this.pendingTransReset();
                    return;
                }

                //Verifica esistenza
                if (this._command.Transaction == null)
                    throw new DataBaseException(Resources.DatabaseMessages.Transaction_Not_Open);

                try
                {
                    //Esegue Commit transazione corrente
                    this._command.Transaction.Commit();

                    //Aggiorna stats
                    this._Stats.Increment(DBStats.EStatement.Commit);
                }
                catch (Exception ex)
                {
                    exLog = ex;
                    throw;
                }
                finally
                {
                    this.TraceStatement("CommitTransaction", exLog);
                    //Imposta Transazione successiva
                    this._command.Transaction = this.getNextTransaction();
                    //Autoclose ?
                    this.checkAutoCloseConnection();
                }
            }
	
		}
		
		
		/// <summary>
		/// Esegue rollback
		/// </summary>
		public void RollbackTransaction()
		{
            Exception exLog = null;

            lock (this._GlobalLock)
            {
                //Se transazione non realmente aperta
                if (this.IsPendingTransaction)
                {
                    this.pendingTransReset();
                    return;
                }

                //Verifica esistenza
                if (this._command.Transaction == null)
                    throw new DataBaseException(Resources.DatabaseMessages.Transaction_Not_Open);

                try
                {
                    //Esegue Commit transazione corrente
                    this._command.Transaction.Rollback();

                    //Aggiorna stats
                    this._Stats.Increment(DBStats.EStatement.Rollback);

                }
                catch (Exception ex)
                {
                    exLog = ex;
                    throw;
                }
                finally
                {
                    this.TraceStatement("RollbackTransaction", exLog);
                    //Imposta Transazione successiva
                    this._command.Transaction = this.getNextTransaction();
                    //Autoclose ?
                    this.checkAutoCloseConnection();
                }
                
            }
		}


        /// <summary>
        /// Ritorna l'Ultimo ID Autoincrement/Identity inserito
        /// </summary>
        /// <returns></returns>
        public virtual long GetLastAutoId()
        {
            throw new NotImplementedException(string.Format(Resources.DatabaseMessages.Not_Implemented, this.GetType().Name));
        }


        /// <summary>
        /// Ritorna un nuovo id ottenuto attraverso un generatore
        /// </summary>
        /// <param name="generatorName"></param>
        /// <returns></returns>
        public virtual long GetNewGeneratorId(string generatorName)
        {
            throw new NotImplementedException(string.Format(Resources.DatabaseMessages.Not_Implemented, this.GetType().Name));
        }
		
		
		/// <summary>
        /// esegue una query non di selezione (INSERT, UPDATE, DELETE, DDL, ..)
		/// </summary>
		/// <returns></returns>
		public int ExecQuery()
		{
            Exception exLog = null;

            lock (this._GlobalLock)
            {
                try
                {
                    //Controlla presenza query
                    if (string.IsNullOrEmpty(this.SQL))
                        throw new DataBaseException(Resources.DatabaseMessages.Query_Empty);

                    this.OpenConnection();

                    int ret = this._command.ExecuteNonQuery();

                    //Aggiorna Stats
                    this.updateStatsFromSQL(this._command.CommandText);

                    //Ritorna
                    return ret;

                }
                catch (Exception ex)
                {
                    exLog = ex;
                    throw;
                }
                finally
                {
                    this.TraceStatement("ExecQuery", exLog);
                    //Resetta comando
                    this.clearCommand();
                    //Autoclose ?
                    this.checkAutoCloseConnection();
                }
            }
		}
		
		
		/// <summary>
		/// esegue una query selezione tornando solo la prima colonna della prima riga (oppure null)
		/// </summary>
		/// <returns></returns>
        public object ExecScalar()
		{
            Exception exLog = null;

            lock (this._GlobalLock)
            {
                try
                {
                    //Controlla presenza query
                    if (string.IsNullOrEmpty(this.SQL))
                        throw new DataBaseException(Resources.DatabaseMessages.Query_Empty);

                    this.OpenConnection();
                    //Esegue
                    object ret =  this._command.ExecuteScalar();

                    //Aggiorna Stats
                    this._Stats.Increment(DBStats.EStatement.Select);

                    //Ritorna
                    return ret;
                }
                catch (Exception ex)
                {
                    exLog = ex;
                    throw;
                }
                finally
                {
                    this.TraceStatement("ExecScalar", exLog);
                    //Resetta comando
                    this.clearCommand();
                    //Autoclose ?
                    this.checkAutoCloseConnection();
                }
            }
		}
		
		
		/// <summary>
		/// esegue una query selezione tornando un DbDataReader
		/// </summary>
		/// <returns>
		/// DataReader Associato ai dati
		/// </returns>
        public DbDataReader ExecReader()
		{
            Exception exLog = null;

            lock (this._GlobalLock)
            {
                try
                {
                    //Controlla presenza query
                    if (string.IsNullOrEmpty(this.SQL))
                        throw new DataBaseException(Resources.DatabaseMessages.Query_Empty);

                    this.OpenConnection();

                    //Esegue
                    DbDataReader dr = this._command.ExecuteReader(this.CanAutoCloseConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default);

                    //Aggiorna Stats
                    this.updateStatsFromSQL(this._command.CommandText);

                    //Ritorna
                    return dr;

                }
                catch (Exception ex)
                {
                    exLog = ex;
                    throw;
                }
                finally
                {
                    this.TraceStatement("ExecReader", exLog);
                    //Resetta comando
                    this.clearCommand();
                    //Non fa autoclose poiche' impostato su reader
                }
            }
		}


        /// <summary>
        /// Esegue query paginata con output datareader
        /// </summary>
        /// <param name="positionIn"></param>
        /// <param name="offsetIn"></param>
        /// <returns></returns>
        internal virtual DbDataReader ExecReaderPaged(int positionIn, int offsetIn)
        {
            throw new NotImplementedException(string.Concat("ExecReaderPaged non implementato per il tipo database ", this.GetType().Name));
        }

        /// <summary>
        /// Esegue una select e ritorna la datatable con i dati
        /// </summary>
        /// <returns></returns>
        public virtual DataTable Select()
        {
            Exception exLog = null;

            lock (this._GlobalLock)
            {
                try
                {
                    //Controlla presenza query
                    if (string.IsNullOrEmpty(this.SQL))
                        throw new DataBaseException(Resources.DatabaseMessages.Query_Empty);

                    //Controlla se connessione aperta
                    this.OpenConnection();
                    //Crea tab
                    DataTable oRetTab = new DataTable(TABLE_NAME);

                    using (DbDataAdapter dA = this._FactoryCorrente.CreateDataAdapter())
                    {
                        //imposta adapter
                        dA.SelectCommand = this._command;

                        //esegue query e riempie
                        dA.Fill(oRetTab);
                    }
                    //Aggiorna Stats
                    this._Stats.Increment(DBStats.EStatement.Select);

                    //Ritorna
                    return oRetTab;
                }
                catch (Exception ex)
                {
                    exLog = ex;
                    throw;
                }
                finally
                {
                    this.TraceStatement("Select", exLog);
                    //Resetta comando
                    this.clearCommand();
                    //Autoclose ?
                    this.checkAutoCloseConnection();
                }
            }       
        }


        /// <summary>
        /// Esgue query di selezione e ritorna un dataset. Utile per query concatenate che ritornano set di dati multipli 
        /// </summary>
        /// <returns></returns>
        public virtual DataSet SelectM()
        {
            Exception exLog = null;

            lock (this._GlobalLock)
            {
                try
                {
                    //Controlla presenza query
                    if (string.IsNullOrEmpty(this.SQL))
                        throw new DataBaseException(Resources.DatabaseMessages.Query_Empty);

                    //Controlla se connessione aperta
                    this.OpenConnection();
                    //Crea tab
                    var ds = new DataSet();

                    using (DbDataAdapter dA = this._FactoryCorrente.CreateDataAdapter())
                    {
                        //imposta adapter
                        dA.SelectCommand = this._command;

                        //esegue query e riempie
                        dA.Fill(ds);
                    }
                    //Aggiorna Stats
                    this._Stats.Increment(DBStats.EStatement.Select);

                    //Ritorna
                    return ds;
                }
                catch (Exception ex)
                {
                    exLog = ex;
                    throw;
                }
                finally
                {
                    this.TraceStatement("Select", exLog);
                    //Resetta comando
                    this.clearCommand();
                    //Autoclose ?
                    this.checkAutoCloseConnection();
                }
            }
        }



        /// <summary>
        /// Esegue una query e ritorna parte del risultato su dataset come da specifiche
        /// E' possibile eseguire l'override nelle classi specializzate per utilizzare
        /// costrutti propri del db (vedi Mysql)
        /// </summary>
        /// <param name="positionIn"></param>
        /// <param name="offsetIn"></param>
        /// <returns></returns>
        public virtual DataTable Select(int positionIn, int offsetIn)
        {
            Exception exLog = null;

            lock (this._GlobalLock)
            {
                try
                {
                    //Controlla presenza query
                    if (string.IsNullOrEmpty(this.SQL))
                        throw new DataBaseException(Resources.DatabaseMessages.Query_Empty);

                    //Azzera contatore record
                    this._TotRecordQueryPaginata = 0;
                   
                    //Controlla se connessione aperta
                    this.OpenConnection();

                    DataTable oRetTab = new DataTable(TABLE_NAME);
                    using (DbDataAdapter dA = this._FactoryCorrente.CreateDataAdapter())
                    {
                        //imposta adapter
                        dA.SelectCommand = this._command;
                        //esegue query e riempie
                        dA.Fill(positionIn, offsetIn, oRetTab);
                    }

                    //Aggiorna Stats 1o statement
                    this._Stats.Increment(DBStats.EStatement.Select);
                    //Trace lo statement
                    this.TraceStatement("SelectPag", null);
                    //quindi esegue query per numero record:
                    // -- cerca ultima clausola ORDER BY
                    int orderPos = this._command.CommandText.ToUpper().LastIndexOf(" ORDER ");
                    // -- esegue parsing
                    this._command.CommandText = string.Format("SELECT COUNT(*) FROM ( {0} ) AS CNTTAB", this._command.CommandText.Substring(0, (orderPos != -1) ? orderPos : this._command.CommandText.Length));
                    // -- esegue query
                    this._TotRecordQueryPaginata = Convert.ToInt32(this._command.ExecuteScalar());
                    //Aggiorna Stats
                    this._Stats.Increment(DBStats.EStatement.Select);

                    //Ritorna
                    return oRetTab;
                }
                catch (Exception ex)
                {
                    exLog = ex;
                    throw;
                }
                finally
                {
                    this.TraceStatement("SelectPag", exLog);
                    //Resetta comando
                    this.clearCommand();
                    //Autoclose ?
                    this.checkAutoCloseConnection();
                }
            }
        }


        /// <summary>
        /// Ritorna nome parametro compatibile a partire da una stringa
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        public virtual string CreateParamName(string baseName)
        {
            if (baseName.StartsWith("@"))
                //Non � necessario prependere nulla
                return baseName;
            
            //Prepende @
            return string.Concat("@", baseName);
        }

		
		/// <summary>
		/// Aggiunge parametro con nome e valore
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
        public virtual void AddParameter(string name, object value)
		{
            this.AddParameter(this.CreateParameter(name, value));
		}

        /// <summary>
        /// Aggiunge parametro con nome e valore e tipo
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public virtual void AddParameter(string name, object value, Type type)
        {
            this.AddParameter(this.CreateParameter(name, value, type));
        }


        /// <summary>
        /// Aggiunge parametro
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void AddParameter(DbParameter parameter)
        {
            lock (this._GlobalLock)
            {
                this._command.Parameters.Add(parameter);
            }
        }


        /// <summary>
        /// Aggiunge elenco parametri
        /// </summary>
        /// <param name="paramlist"></param>
        public virtual void AddParameters(IList<DbParameter> paramlist)
        {
            if (paramlist == null)
                return;

            for (int i = 0; i < paramlist.Count; i++)
            {
                this.AddParameter(paramlist[i]);
            }
        }


        /// <summary>
        /// Elimina tutti i parametri
        /// </summary>
        public void ClearParameters()
        {
            this._command.Parameters.Clear();
        }


        /// <summary>
        /// Crea parametro per nome, valore e tipo
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual DbParameter CreateParameter(string name, object value, Type type)
        {
            DbParameter param = this._FactoryCorrente.CreateParameter();
            param.ParameterName = this.CreateParamName(name);
            param.DbType = this.TypeToDbType(type);
            //Se fornito null imposta DBNULL
            param.Value = (value != null) ? value : DBNull.Value;
            return param;
        }


        /// <summary>
        /// Crea parametro per nome e valore 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual DbParameter CreateParameter(string name, object value)
        {
            DbParameter param = this._FactoryCorrente.CreateParameter();
            param.ParameterName = this.CreateParamName(name);
            //Se fornito null imposta DBNULL
            param.Value = (value != null) ? value : DBNull.Value;
            return param;
        }


        
		
		
        /// <summary>
        /// Chiude la connessione (se aperta)
        /// </summary>
        /// <param name="rollbackUnCommitted"></param>
		public void CloseConnection(bool rollbackUnCommitted)
		{
            lock (this._GlobalLock)
            {
                //Se siamo in pending significa che la connessione e' chiusa
                if (this.IsPendingTransaction)
                {
                    this.pendingTransReset();
                    return;
                }
                    

                //Chiude la connessione se aperta
                if (this.IsConnectionOpen)
                {

                    //se la transazione � aperta esegue rollback e la elimina
                    while (this.IsInTransaction)
                    {
                            if (rollbackUnCommitted)
                                //RollBAck tutte le transazioni
                                this.RollbackTransaction();
                            else
                                //Commit tutte le transazioni
                                this.CommitTransaction();
                    }

                    //Quindi Chiude Connessione
                    this._dbconn.Close();
             
                }
            }
		}
		
		
		/// <summary>
		/// Inizia una sessione Atomica di lavoro
		/// </summary>
        public void BeginThreadSafeWork()
		{
            Monitor.Enter(this._GlobalLock);
		}
		
		
		/// <summary>
		/// Termina una sessione atomica di lavoro
		/// </summary>
        public void EndThreadSafeWork()
		{
            Monitor.Exit(this._GlobalLock);
		}


        public bool HasLock(string lockName)
        {
            //Lock non inizializzato
            if (this._LockAcquired == null)
                return false;

            //Verifica
            return  this._LockAcquired.ContainsKey(lockName);
        }


        /// <summary>
        /// Ottiene un lock globale a livello di applicazione
        /// per una risorsa identificata da un nome
        /// </summary>
        /// <param name="lockName"></param>
        /// <param name="timeoutsec"></param>
        public virtual void GetLock(string lockName, int timeoutsec)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rilascia un lock globale precedentemente
        /// acquisito
        /// </summary>
        /// <param name="lockName"></param>
        public virtual void ReleaseLock(string lockName)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Resetta contesto di esecuzione db
        /// </summary>
        public void Reset()
        {
            this.clearCommand();
        }


        /// <summary>
        /// Ritorna un codice hash rappresentativo della query che si sta per eseguire
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public string GetCurrentQueryHashString(int pos, int offset)
        {
            var sb = new System.Text.StringBuilder(@"QRY|");
            sb.Append(this.HashCode); //Connection
            sb.Append(@"|"); //Sep
            sb.Append(this._command.CommandText); //SQL
            sb.Append(@"|"); //Sep
            sb.Append(pos);
            sb.Append(@"|"); //Sep
            sb.Append(offset);

            for (int i = 0; i < this._command.Parameters.Count; i++)
            {
                sb.Append(@"|"); //Sep
                sb.Append(this._command.Parameters[i].ParameterName);
                sb.Append(@"|"); //Sep
                sb.Append(this._command.Parameters[i].Value);
            }

            return sb.ToString();
        }



        /// <summary>
        /// Ritorna un codice hash rappresentativo della query che si sta per eseguire
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public UInt32 GetCurrentQueryHashCode(int pos, int offset)
        {
           return BdoHash.Instance.Hash(this.GetCurrentQueryHashString(pos,offset));
        }

        /// <summary>
        /// Ritorna connessione ADO sottostante
        /// </summary>
        /// <returns></returns>
        public DbConnection GetAdoConnection()
        {
            //Se esiste una transazione pending e' necessario forzare l'apertura della connessione per garantire l'avvio della transazione
            if (this.IsPendingTransaction)
                this.OpenConnection();

            return this._dbconn;
        }


        /// <summary>
        /// Ritorna transazione ADO sottostante
        /// </summary>
        /// <returns></returns>
        public DbTransaction GetAdoTransaction()
        {
            //Se esiste una transazione pending e' necessario forzare l'apertura della connessione per garantire l'avvio della transazione
            if (this.IsPendingTransaction)
                this.OpenConnection();

            return this._command.Transaction;
        }


        #region LOCKED TRANSACTIONS

        /// <summary>
        /// Richiede lock ed avvia transazione
        /// </summary>
        /// <param name="lockName"></param>
        /// <param name="timeOutSec"></param>
        public void LockedTransactionBegin(string lockName, int timeOutSec)
        {
            this.GetLock(lockName, timeOutSec);
            this.BeginTransaction();
        }

        /// <summary>
        /// Committa e rilascia il lock. In caso di eccezione della transazione rilascia comunque il lock e propaga l'eccezione
        /// </summary>
        /// <param name="lockName"></param>
        public void LockedTransactionCommit(string lockName)
        {
            if (!this.HasLock(lockName))
                throw new DataBaseException(Resources.DatabaseMessages.Lock_Not_Acquired);

            try
            {
                this.CommitTransaction();
            }
            finally
            {
                this.ReleaseLock(lockName);
            }
        }

        /// <summary>
        /// Rollback e rilascia il lock. In caso di eccezione della transazione rilascia comunque il lock e propaga l'eccezione
        /// </summary>
        /// <param name="lockName"></param>
        public void LockedTransactionRollback(string lockName)
        {
            if (!this.HasLock(lockName))
                throw new DataBaseException(Resources.DatabaseMessages.Lock_Not_Acquired);

            try
            {
                this.RollbackTransaction();
            }
            finally
            {
                this.ReleaseLock(lockName);
            }
        }

        #endregion



        /// <summary>
        /// Rilascio Finale delle risorse con rollback eventuali transazioni non committed
        /// </summary>
        public void Dispose()
        {
            //Chiude db e libera risorse
            using (this._dbconn)
            {
                using (this._command)
                {
                    this.CloseConnection(true);
                }
            }

            //Chiude file di trace
            this.DisableTrace();

        }


		#endregion
		
		#region "PROTECTED"

        /// <summary>
        /// Registra il lock nella classe database in modo da tenerne traccia
        /// </summary>
        /// <param name="lockName"></param>
        protected void registerLock(string lockName)
        {
            //Verifica presenza elenco e lo istanzia
            if (this._LockAcquired == null)
                _LockAcquired = new Dictionary<string, object>();

            //registra lock
            this._LockAcquired.Add(lockName, null);
        }

        /// <summary>
        /// Deregistra lock
        /// </summary>
        /// <param name="lockName"></param>
        protected void unregisterLock(string lockName)
        {
            //Verifica presenza
            if (this._LockAcquired == null)
                throw new DataBaseException(Resources.DatabaseMessages.Lock_Reference_Null);

            //deregistra lock
            this._LockAcquired.Remove(lockName);
        }

        /// <summary>
        /// Data una stringa sql aggiorna le statistiche in base al tipo
        /// </summary>
        /// <param name="sqlIn"></param>
        protected void updateStatsFromSQL(string sqlIn)
        {
            for (int i = 0; i < sqlIn.Length; i++)
            {
                //Aggiorna Stats
                switch (sqlIn[i])
                {

                    case 'u':
                    case 'U':
                        this._Stats.Increment(DBStats.EStatement.Update);
                        return;
                    case 'i':
                    case 'I':
                        this._Stats.Increment(DBStats.EStatement.Insert);
                        return;
                    case 'd':
                    case 'D':
                        if (sqlIn.Length > 1 && char.ToUpper(sqlIn[i + 1]) != 'E')
                            break;
                        this._Stats.Increment(DBStats.EStatement.Delete);
                        return;
                    case 's':
                    case 'S':
                        this._Stats.Increment(DBStats.EStatement.Select);
                        return;
                    case ' ':
                        break;
                    default:
                        this._Stats.Increment(DBStats.EStatement.Other);
                        return;
                }  
            }
        }
		
		
		/// <summary>
		/// Dato un nome Assembly ed il nome della classe Factory se non � gi� caricata
		/// esegue il caricamento, esegue caching del risultato e carica tutti i campi
		/// Attenzione!! Da utilizzare solo nel costruttore delle classi derivate
		/// </summary>
		/// <param name="nomeAssembly"></param>
		/// <param name="classeFactory"></param>
		protected void LoadAssemblyAndInitByFactory(string nomeAssembly, string classeFactory)
		{
			//Carica...
			DbProviderFactory oFactory = this.doGetAssemblyDbFactory(nomeAssembly, classeFactory);

			//Imposta i Dati specifici per il tipo database definito
            this.InitByFactory(oFactory);
		}


        /// <summary>
        /// Carica impostazioni da factory corrente
        /// </summary>
        protected void InitByFactory(DbProviderFactory factory)
        {
            //Imposta i Dati specifici per il tipo database definito
            this._FactoryCorrente = factory;
            this._dbconn = factory.CreateConnection();
            this._dbconn.ConnectionString = this._connStr;
            this._command = this._dbconn.CreateCommand();
        }


        /// <summary>
        /// Inizializza attraverso oggetti ADO
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <param name="factory"></param>
        protected void InitByADO(DbConnection conn, DbTransaction tran, DbProviderFactory factory)
        {
            //Imposta i Dati specifici per il tipo database definito
            this._FactoryCorrente = factory;
            this._dbconn = conn;
            this._command = this._dbconn.CreateCommand();

            if (tran != null)
                this._tranQ.Push(tran);
        }

		
		
		/// <summary>
		/// Apre la connessione al database
		/// </summary>
		public void OpenConnection()
		{
			//apre connessione
			if (!this.IsConnectionOpen){
				this._command.Transaction = null;
				this._dbconn.Open();

                if(this.IsPendingTransaction)
                {
                    //Recupera il livello richiesto
                    var level = this._PendingTransactionLevel;
                    //Annulla pending
                    this.pendingTransReset();
                    //Avvia transazione
                    this.BeginTransaction(level);
                }
			}
		}


        /// <summary>
        /// Se impostata autochiusura e possibile chiudere allora esegue chiusura
        /// </summary>
        protected virtual void checkAutoCloseConnection()
        {
            //Se autoclose non attivo non chiude
            if (!this.CanAutoCloseConnection)
                return;

            //OK chiude
            this.CloseConnection(true);
        }
		
        /// <summary>
        /// Pulizia comando
        /// </summary>
		protected virtual void clearCommand()
		{
            //Pulisce
            this._command.CommandText = "";
            this._command.CommandType = CommandType.Text;
            this._command.Parameters.Clear();
		}


        /// <summary>
        /// Imposta numero record query paginata
        /// </summary>
        /// <param name="numRecords"></param>
        protected void setTotPagedRecords(int numRecords)
        {
            this._TotRecordQueryPaginata = numRecords;
        }


        /// <summary>
        /// Crea nuova transazione
        /// </summary>
        /// <returns></returns>
        protected DbTransaction createNewTransaction(IsolationLevel level)
        {
            return this._dbconn.BeginTransaction(level);
        }


        /// <summary>
        /// Ritorna la successiva Transazione altrimenti null
        /// </summary>
        /// <returns></returns>
        protected DbTransaction getNextTransaction()
        {
            return (this._tranQ.Count > 0) ? (DbTransaction)this._tranQ.Pop() : null;
        }


        /// <summary>
        /// Scrive Riga Su File di Trace
        /// </summary>
        protected void TraceLog( string messageFmt, params object[] args )
        {
            if (this._TraceLog == null)
                return;

            this._TraceLog.LogMessage(messageFmt, args);
        }

        /// <summary>
        /// Esegue il trace di uno statement
        /// </summary>
        /// <param name="position"></param>
        /// <param name="ex"></param>
        protected void TraceStatement(string position, Exception ex)
        {
            //Se non e' abilitato trace non scrive
            if (this._TraceLog == null || (this._TraceOnlyErrors && ex == null))
                return;
            
            //Scrive con lock comune
            this._TraceLog.BeginSafeWrite();
            try
            {
                string sRow = string.Concat(this.GetType().Name, "  ");
                this._TraceLog.LogMessage("==============================================================================================");
                this._TraceLog.LogMessage(string.Concat(sRow, position));
                this._TraceLog.LogMessage(string.Concat(sRow, "SQL: ", this._command.CommandText));
                for (int i = 0; i < this._command.Parameters.Count; i++)
                {
                    this._TraceLog.LogMessage(string.Concat(sRow, "PARAM: ", this._command.Parameters[i].ParameterName, " = ", this._command.Parameters[i].Value.ToString()));
                }

                //Scrive esito
                if (ex == null)
                {
                    this._TraceLog.LogMessage(Resources.DatabaseMessages.TraceLog_Return_OK);
                }
                else
                {
                    this._TraceLog.LogMessage(Resources.DatabaseMessages.TraceLog_Return_ERR);
                    this._TraceLog.LogMessage(string.Concat("Exception: ", ex.Message));
                }
            }
            finally
            {
                this._TraceLog.EndSafeWrite(); //Chiude Thread-safe
            }
        }


        /// <summary>
        /// Converte tipo .Net in DbType
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        protected System.Data.DbType TypeToDbType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.String:
                case TypeCode.Char:
                    return System.Data.DbType.String;
                case TypeCode.Decimal:
                    return System.Data.DbType.Decimal;
                case TypeCode.Double:
                    return System.Data.DbType.Double;
                case TypeCode.Int32:
                    return System.Data.DbType.Int32;
                case TypeCode.Int64:
                    return System.Data.DbType.Int64;
                case TypeCode.Int16:
                    return System.Data.DbType.Int16;
                case TypeCode.DateTime:
                    return System.Data.DbType.DateTime;
                case TypeCode.Boolean:
                    return System.Data.DbType.Boolean;
                case TypeCode.Single:
                    return System.Data.DbType.Single;
                case TypeCode.UInt16:
                    return System.Data.DbType.UInt16;
                case TypeCode.UInt32:
                    return System.Data.DbType.UInt32;
                case TypeCode.UInt64:
                    return System.Data.DbType.UInt64;
                case TypeCode.Byte:
                    return System.Data.DbType.Byte;
                case TypeCode.SByte:
                    return System.Data.DbType.SByte;
                default:
                    if (t.IsArray && t.Equals(typeof(byte[])))
                        return System.Data.DbType.Binary;

                    return System.Data.DbType.Object;
            }
        }



        #endregion

        #region "PRIVATE"

        /// <summary>
        /// Resetta l'indicatore di transazione pendente
        /// </summary>
        private void pendingTransReset()
        {
            this._PendingTransactionLevel = IsolationLevel.Unspecified;
        }

        /// <summary>
        /// Imposta indicatore transazione pendente
        /// </summary>
        /// <param name="level"></param>
        private void pendingTransSet(IsolationLevel level)
        {
            this._PendingTransactionLevel = level;
        }


        /// <summary>
        /// Esegue il solo caricamento dell'assembly/factory e lo inserisce nella lista (se necessario)
        /// </summary>
        /// <param name="nomeAssembly"></param>
        /// <param name="classeFactory"></param>
        private DbProviderFactory doGetAssemblyDbFactory(string nomeAssembly, string classeFactory)
        {
            DbProviderFactory oFactory = null;
            //Se esiste lo imposta ed esce senza complicazioni
            if (CommonDataBase.AssFactoryDictionary.TryGetValue(nomeAssembly, out oFactory))
                return oFactory;

            //Cerca assembly gi� caricato
            lock (CommonDataBase.AssFactoryDictionary)
            {
                //Se esiste lo imposta ed esce (questa ripetizione serve ad evitare un secondo caricamento in caso di concorrenza)
                if (CommonDataBase.AssFactoryDictionary.TryGetValue(nomeAssembly, out oFactory))
                    return oFactory;

                //Non trovato, carica assembly
                //TODO Forse meglio mettere il nome del file??
                Assembly assThis = Assembly.GetExecutingAssembly();
                Assembly ass = null;
                string sAssemblyDll = string.Concat(nomeAssembly, @".dll");
                string sAsspath;
             
                //Carica dll a partire dal percorso di origine di BDO
                if (Uri.IsWellFormedUriString(assThis.EscapedCodeBase, UriKind.Absolute))
                {
                    Uri uri = new Uri(assThis.EscapedCodeBase);

                    if (uri.IsFile || uri.IsUnc)
                        sAsspath = Path.Combine(Path.GetDirectoryName(uri.LocalPath), sAssemblyDll);
                    else
                    {
                        //Se ad es. http lascia uri
                        string sNomeBdo = Path.GetFileName(uri.AbsoluteUri);
                        sAsspath = uri.AbsoluteUri.Replace(sNomeBdo, "") + sAssemblyDll;
                    }
                }
                else
                {
                    sAsspath = Path.Combine(assThis.CodeBase, sAssemblyDll);
                }

                //Carica
                if (File.Exists(sAsspath))
                    ass = Assembly.LoadFrom(sAsspath);
                else
                    throw new ApplicationException(string.Format(Resources.DatabaseMessages.Assembly_Not_Found, nomeAssembly));

                //Crea Nuovo Factory
                try
                {
                    oFactory = (DbProviderFactory)Activator.CreateInstance(ass.GetType(classeFactory, true, true));
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Format(Resources.DatabaseMessages.Assembly_Load_Error, ass.FullName, ex.Message));
                }

                //Aggiunge Factory a lista
                CommonDataBase.AssFactoryDictionary.Add(nomeAssembly, oFactory);
            }

            return oFactory;
        }

		#endregion
	}
}
