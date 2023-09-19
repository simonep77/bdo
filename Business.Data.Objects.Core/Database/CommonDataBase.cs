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
using System.Linq;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Common.Logging;
using Business.Data.Objects.Core.Database.Resources;
using Business.Data.Objects.Common;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Business.Data.Objects.Core.Common.Utils;
using System.Diagnostics;

namespace Business.Data.Objects.Database
{
    /// <summary>
    /// Classe Astratta da cui derivano quelle specializzate: gran parte delle proprietà
    /// e dei Metodi risiedono qui
    /// </summary>
    public abstract class CommonDataBase : IDataBase
    {
        /// <summary>
        /// Assembly di riferimento del provider
        /// </summary>
        protected abstract string ProviderAssembly { get; }

        /// <summary>
        /// Classe del factory provider
        /// </summary>
        protected abstract string ProviderFactoryClass { get; }

        /// <summary>
        /// Istanza del factory corrente
        /// </summary>
        protected DbProviderFactory ProviderFactory { get; private set; }

        /// <summary>
        /// Mappatore tipi .NET -> tipi DB
        /// </summary>
        public virtual DbTypeMapper TypeMapper { get; } = new DbTypeMapper();

        /// <summary>
        /// Timer interno per il tracciamento dei tempi di esecuzione di ogni statetment
        /// </summary>
        private Stopwatch Timer { get; set; } = new Stopwatch();

        /// <summary>
        /// Tempi esecuzione ultimo statement 
        /// </summary>
        public TimeSpan LastExecutionElaps => this.Timer.Elapsed;

        /// <summary>
        /// Contiene la lista di tutti gli assembli Ado caricati nell'Applicazione (Web o Client) corrente
        /// </summary>
        private static Dictionary<string, DbProviderFactory> AssFactoryDictionary = new Dictionary<string, DbProviderFactory>(3);

        protected const string TABLE_NAME = @"Table1";
        private DbConnection _dbconn;
        private DbCommand _command;
        private Stack _tranQ = new Stack(1);
        private LoggerBase _TraceLog;
        private Dictionary<string, object> _LockAcquired; //Istanziato al primo utilizzo
        private IsolationLevel _PendingTransactionLevel = IsolationLevel.Unspecified;

        #region "PROPERTY"


        protected virtual bool PagedReaderLastRow => true;


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
        public DBStats Stats { get; } = new DBStats();

        /// <summary>
        /// Ritorna tipo database
        /// </summary>
        public string DbType => this.GetType().Name;

        /// <summary>
        /// Ritorna connection string utilizzata
        /// </summary>
        public string ConnectionString { get; private set; }


        /// <summary>
        /// Ritorna o imposta il timeout di esecuzione query (secondi)
        /// </summary>
        public int ExecutionTimeout
        {
            get => this._command.CommandTimeout;
            set => this._command.CommandTimeout = value;
        }

        /// <summary>
        /// Ottiene o imposta la tipologia di comando che si vuole eseguire:
        /// - Sql standard
        /// - Stored Procedure
        /// - Tabella diretta
        /// </summary>
        public CommandType CommandType
        {
            get => this._command.CommandType;
            set => this._command.CommandType = value;
        }

        /// <summary>
        /// ottiene o imposta il comportamento della connessione
        /// </summary>
        public bool AutoCloseConnection { get; set; }


        /// <summary>
        /// Lista dei parametri attualmente presenti
        /// </summary>
        public DbParameterCollection Parameters
        {
            get => this._command.Parameters;
        }

        /// <summary>
        /// SQL da eseguire (o eseguito)
        /// </summary>
        public virtual string SQL
        {
            get => this._command.CommandText;
            set => this._command.CommandText = value;
        }


        /// <summary>
        /// Numero Totale Records calcolati dopo l'ultima OpenQuery paginata
        /// </summary>
        public int TotRecordQueryPaginata { get; private set; }


        /// <summary>
        /// Abilita o disabilita la registrazione dei dati di esecuzione
        /// </summary>
        public bool TraceON => this._TraceLog != null;


        /// <summary>
        /// Se TraceON abilita il trace dei soli
        /// errori
        /// </summary>
        public bool TraceOnlyErrors { get; set; }


        /// <summary>
        /// Indica se si e' in un contesto transazionale
        /// </summary>
        public bool IsInTransaction => this._command.Transaction != null || this.IsPendingTransaction;


        /// <summary>
        /// Indica se presenti lock in atto
        /// </summary>
        public bool HasAcquiredLocks => this._LockAcquired != null && this._LockAcquired.Count > 0;

        /// <summary>
        /// Indica se e' stato richiesto avvio transazione ma effettivamente ancora non e' stata aperta in quanto non eseguita la connessione
        /// </summary>
        internal bool IsPendingTransaction => this._PendingTransactionLevel != IsolationLevel.Unspecified;

        /// <summary>
        /// Indica se la connessione e' attiva
        /// </summary>
        public bool IsConnectionOpen => this._dbconn != null && this._dbconn.State == ConnectionState.Open;


        /// <summary>
        /// Indica se possibile chiudere la connessione
        /// </summary>
        public bool CanAutoCloseConnection
        {
            get
            {
                //Se autoclose non attivo non chiude
                if (!this.AutoCloseConnection)
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
        public UInt32 HashCode { get; private set; }


        /// <summary>
        /// Isolamento di default della transazione
        /// </summary>
        public virtual System.Data.IsolationLevel TransactionDefaultIsolation { get; set; } = System.Data.IsolationLevel.ReadCommitted;


        #endregion

        #region Eventi

        /// <summary>
        /// Evento scatenato in caso di begin
        /// </summary>
        public event TransactionEventHandler OnBeginTransaction;

        /// <summary>
        /// Evento scatenato in caso di commit
        /// </summary>
        public event TransactionEventHandler OnCommitTransaction;

        /// <summary>
        /// Evento scatenato in caso di rollback
        /// </summary>
        public event TransactionEventHandler OnRollbackTransaction;

        #endregion

        #region "PUBLIC"

        /// <summary>
        /// Costruttore pubblico
        /// </summary>
        /// <param name="connString"></param>
        public CommonDataBase(string connString)
        {
            this.ProviderFactory = this.GetDbFactory();
            this.ConnectionString = connString;
            this.HashCode = BdoHash.Instance.Hash(connString);

            this.InitByFactory();
        }

        public CommonDataBase(DbConnection conn, DbTransaction tran)
            : this(conn.ConnectionString)
        {
            //Carica
            this.InitByADO(conn, tran);
        }


        /// <summary>
        /// Clona una connessione DB
        /// </summary>
        /// <returns></returns>
        public IDataBase Clone()
        {
            IDataBase dbOut = DataBaseFactory.CreaDataBase(this.GetType().Name, this.ConnectionString);
            dbOut.AutoCloseConnection = this.AutoCloseConnection;
            if (this.TraceON)
                dbOut.EnableTrace(this._TraceLog, this.TraceOnlyErrors);

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
            this.TraceOnlyErrors = onlyErrors;
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
            this.TraceOnlyErrors = onlyErrors;
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
            this.BeginTransaction(this.TransactionDefaultIsolation);
        }

        /// <summary>
        /// Inizia nuova transazione con specifica dell'isolation level
        /// </summary>
        /// <param name="level"></param>
        public void BeginTransaction(IsolationLevel level)
        {
            Exception exLog = null;

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
                    this.Stats.Increment(DBStats.EStatement.Begin);
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

                //Infine richiama evento
                this.OnBeginTransaction?.Invoke(this);
            }

        }

        /// <summary>
        /// Esegue Commit
        /// </summary>
        public void CommitTransaction()
        {
            Exception exLog = null;

            //Se transazione non realmente aperta
            if (this.IsPendingTransaction)
            {
                this.pendingTransReset();
                return;
            }

            //Verifica esistenza
            if (this._command.Transaction == null)
                throw new DataBaseException(DatabaseMessages.Transaction_Not_Open);

            try
            {
                //Esegue Commit transazione corrente
                this._command.Transaction.Commit();

                //Aggiorna stats
                this.Stats.Increment(DBStats.EStatement.Commit);
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

            //Infine richiama evento
            this.OnCommitTransaction?.Invoke(this);

        }


        /// <summary>
        /// Esegue rollback
        /// </summary>
        public void RollbackTransaction()
        {
            Exception exLog = null;

            //Se transazione non realmente aperta
            if (this.IsPendingTransaction)
            {
                this.pendingTransReset();
                return;
            }

            //Verifica esistenza
            if (this._command.Transaction == null)
                throw new DataBaseException(DatabaseMessages.Transaction_Not_Open);

            try
            {
                //Esegue Commit transazione corrente
                this._command.Transaction.Rollback();

                //Aggiorna stats
                this.Stats.Increment(DBStats.EStatement.Rollback);

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

            //Infine richiama evento
            this.OnRollbackTransaction?.Invoke(this);
        }


        /// <summary>
        /// Ritorna l'Ultimo ID Autoincrement/Identity inserito
        /// </summary>
        /// <returns></returns>
        public virtual long GetLastAutoId()
        {
            throw new NotImplementedException(string.Format(DatabaseMessages.Not_Implemented, this.GetType().Name));
        }


        /// <summary>
        /// Ritorna un nuovo id ottenuto attraverso un generatore
        /// </summary>
        /// <param name="generatorName"></param>
        /// <returns></returns>
        public virtual long GetNewGeneratorId(string generatorName)
        {
            throw new NotImplementedException(string.Format(DatabaseMessages.Not_Implemented, this.GetType().Name));
        }


        /// <summary>
        /// esegue una query non di selezione (INSERT, UPDATE, DELETE, DDL, ..)
        /// </summary>
        /// <returns></returns>
        public int ExecQuery()
        {
            Exception exLog = null;

            try
            {
                //Controlla presenza query
                if (string.IsNullOrEmpty(this.SQL))
                    throw new DataBaseException(DatabaseMessages.Query_Empty);

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


        /// <summary>
        /// esegue una query selezione tornando solo la prima colonna della prima riga (oppure null)
        /// </summary>
        /// <returns></returns>
        public object ExecScalar()
        {
            Exception exLog = null;

            try
            {
                //Controlla presenza query
                if (string.IsNullOrEmpty(this.SQL))
                    throw new DataBaseException(DatabaseMessages.Query_Empty);

                this.OpenConnection();
                //Esegue
                object ret = this._command.ExecuteScalar();

                //Aggiorna Stats
                this.Stats.Increment(DBStats.EStatement.Select);

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


        /// <summary>
        /// esegue una query selezione tornando un DbDataReader
        /// </summary>
        /// <returns>
        /// DataReader Associato ai dati
        /// </returns>
        public DbDataReader ExecReader()
        {
            Exception exLog = null;

            try
            {
                //Controlla presenza query
                if (string.IsNullOrEmpty(this.SQL))
                    throw new DataBaseException(DatabaseMessages.Query_Empty);

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


        /// <summary>
        /// Esegue query paginata con output datareader
        /// </summary>
        /// <param name="positionIn"></param>
        /// <param name="offsetIn"></param>
        /// <returns></returns>
        public virtual DbDataReader ExecReaderPaged(int positionIn, int offsetIn)
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

            try
            {
                //Controlla presenza query
                if (string.IsNullOrEmpty(this.SQL))
                    throw new DataBaseException(DatabaseMessages.Query_Empty);

                //Controlla se connessione aperta
                this.OpenConnection();
                //Crea tab
                DataTable oRetTab = new DataTable(TABLE_NAME);

                using (DbDataAdapter dA = this.ProviderFactory.CreateDataAdapter())
                {
                    //imposta adapter
                    dA.SelectCommand = this._command;

                    //esegue query e riempie
                    dA.Fill(oRetTab);
                }
                //Aggiorna Stats
                this.Stats.Increment(DBStats.EStatement.Select);

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


        /// <summary>
        /// Esegue query di selezione e ritorna un dataset. Utile per query concatenate che ritornano set di dati multipli 
        /// </summary>
        /// <returns></returns>
        public virtual DataSet SelectM()
        {
            Exception exLog = null;

            try
            {
                //Controlla presenza query
                if (string.IsNullOrEmpty(this.SQL))
                    throw new DataBaseException(DatabaseMessages.Query_Empty);

                //Controlla se connessione aperta
                this.OpenConnection();
                //Crea tab
                var ds = new DataSet();

                using (DbDataAdapter dA = this.ProviderFactory.CreateDataAdapter())
                {
                    //imposta adapter
                    dA.SelectCommand = this._command;

                    //esegue query e riempie
                    dA.Fill(ds);
                }
                //Aggiorna Stats
                this.Stats.Increment(DBStats.EStatement.Select);

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

            try
            {
                //Controlla presenza query
                if (string.IsNullOrEmpty(this.SQL))
                    throw new DataBaseException(DatabaseMessages.Query_Empty);

                //Azzera contatore record
                this.TotRecordQueryPaginata = 0;

                //Controlla se connessione aperta
                this.OpenConnection();

                DataTable oRetTab = new DataTable(TABLE_NAME);
                using (DbDataAdapter dA = this.ProviderFactory.CreateDataAdapter())
                {
                    //imposta adapter
                    dA.SelectCommand = this._command;
                    //esegue query e riempie
                    dA.Fill(positionIn, offsetIn, oRetTab);
                }

                //Aggiorna Stats 1o statement
                this.Stats.Increment(DBStats.EStatement.Select);
                //Trace lo statement
                this.TraceStatement("SelectPag", null);
                //quindi esegue query per numero record:
                // -- cerca ultima clausola ORDER BY
                int orderPos = this._command.CommandText.ToUpper().LastIndexOf(" ORDER ");
                // -- esegue parsing
                this._command.CommandText = string.Format("SELECT COUNT(*) FROM ( {0} ) AS CNTTAB", this._command.CommandText.Substring(0, (orderPos != -1) ? orderPos : this._command.CommandText.Length));
                // -- esegue query
                this.TotRecordQueryPaginata = Convert.ToInt32(this._command.ExecuteScalar());
                //Aggiorna Stats
                this.Stats.Increment(DBStats.EStatement.Select);

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

        /// <summary>
        /// Dizionario concorrente per le mappature delle query
        /// </summary>
        private static DtoBinderReflection _DtoBinder = new DtoBinderReflection();


        /// <summary>
        /// Esegue una query paginata e mappa i dati sull'oggetto di tipo T
        /// Vengono mappati i campi pubblici di istanza se il risultato non e' null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual PageableResult<T> Query<T>(int page, int offset)
           where T : new()
        {
            //Se sp allora errore
            if (this.CommandType != CommandType.Text)
                throw new ArgumentException("E' possibile paginare solo una query SQL standard!");

            //Crea la classe di lettura info
            var tInfo = _DtoBinder.GetTypeMapper<T>();

            var res = new PageableResult<T>();

            res.Pager.Page = page;
            res.Pager.Offset = offset;

            using (var rd = this.ExecReaderPaged(res.Pager.Position, res.Pager.Offset))
            {
                while (rd.Read())
                {
                    //Imposta totale records da ultima colonna della query. Se non pertinente allora imposta -1
                    if (res.Pager.TotRecords == 0 && this.PagedReaderLastRow)
                    {
                        var oTotRecs = rd[rd.FieldCount - 1];
                        if (oTotRecs is Int32)
                            res.Pager.TotRecords = (Int32)oTotRecs;
                    }

                    T obj = new T();

                    _DtoBinder.MapSingle<T>(tInfo, obj, rd);

                    res.Result.Add(obj);
                }

                //Se presente un resultset aggiuntivo allora assume che sia il numero di record
                if (!this.PagedReaderLastRow && rd.NextResult())
                {
                    if (rd.Read())
                        res.Pager.TotRecords = rd.GetInt32(0);
                }

            }

            return res;
        }

        /// <summary>
        /// Esegue una query con mappatura su oggetti
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> Query<T>()
           where T : new()
        {
            var tInfo = _DtoBinder.GetTypeMapper<T>();

            var res = new List<T>();

            using (var rd = this.ExecReader())
            {
                while (rd.Read())
                {
                    T obj = new T();

                    _DtoBinder.MapSingle<T>(tInfo, obj, rd);

                    res.Add(obj);
                }

            }

            return res;
        }

        /// <summary>
        /// Maps a SqlDataReader record to an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <param name="newObject"></param>
        public static void MapDataToObject<T>(DbDataReader dataReader, T newObject)
        {
            Type t = typeof(T);

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                PropertyInfo mi = t.GetProperty(dataReader.GetName(i), BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (mi == null || dataReader.IsDBNull(i))
                    continue;

                mi.SetValue(newObject, Convert.ChangeType(dataReader[i], mi.PropertyType));
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
                //Non è necessario prependere nulla
                return baseName;

            //Prepende @
            return string.Concat("@", baseName);
        }


        /// <summary>
        /// Aggiunge parametro con nome e valore
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public virtual DbParameter AddParameter(string name, object value)
        {
            return this.AddParameter(name, value, value?.GetType());
        }

        /// <summary>
        /// Aggiunge parametro con nome e valore e tipo
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public virtual DbParameter AddParameter(string name, object value, Type type)
        {
            var p = this.CreateParameter(name, value, type);
            this.AddParameter(p);
            return p;
        }

        public virtual DbParameter AddParameter(string name, object value, DbType dbtype)
        {
            var p = this.CreateParameter(name, value, dbtype);
            this.AddParameter(p);
            return p;
        }


        /// <summary>
        /// Aggiunge parametro
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void AddParameter(DbParameter parameter) => this._command.Parameters.Add(parameter);


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
        public void ClearParameters() => this._command.Parameters.Clear();


        /// <summary>
        /// Crea parametro per nome, valore e tipo .NET
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual DbParameter CreateParameter(string name, object value, Type type)
        {
            DbParameter param = this.ProviderFactory.CreateParameter();
            param.ParameterName = this.CreateParamName(name);

            //Se fornito un type allora lo decodifica
            if (type != null)
                param.DbType = this.TypeMapper.GetDbTypeFor(type);

            //Se fornito null imposta DBNULL
            param.Value = (value != null) ? value : DBNull.Value;
            return param;
        }

        /// <summary>
        /// Crea parametro per nome, valore e tipo del driver DB
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbtype"></param>
        /// <returns></returns>
        public virtual DbParameter CreateParameter(string name, object value, DbType dbtype)
        {
            DbParameter param = this.ProviderFactory.CreateParameter();
            param.ParameterName = this.CreateParamName(name);

            //Se fornito un type allora lo decodifica
            param.DbType = dbtype;

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
            //Se siamo in pending significa che la connessione e' chiusa
            if (this.IsPendingTransaction)
            {
                this.pendingTransReset();
                return;
            }


            //Chiude la connessione se aperta
            if (this.IsConnectionOpen)
            {

                //se la transazione è aperta esegue rollback e la elimina
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


        public bool HasLock(string lockName) => this._LockAcquired != null && this._LockAcquired.ContainsKey(lockName);


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
            return BdoHash.Instance.Hash(this.GetCurrentQueryHashString(pos, offset));
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
                throw new DataBaseException(DatabaseMessages.Lock_Not_Acquired);

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
                throw new DataBaseException(DatabaseMessages.Lock_Not_Acquired);

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

            //Sgancia eventi
            if (this.OnBeginTransaction != null)
            {
                foreach (Delegate d in OnBeginTransaction.GetInvocationList())
                {
                    this.OnBeginTransaction -= (TransactionEventHandler)d;
                }
            }

            if (this.OnCommitTransaction != null)
            {
                foreach (Delegate d in OnCommitTransaction.GetInvocationList())
                {
                    this.OnCommitTransaction -= (TransactionEventHandler)d;
                }
            }

            if (this.OnRollbackTransaction != null)
            {
                foreach (Delegate d in OnRollbackTransaction.GetInvocationList())
                {
                    this.OnRollbackTransaction -= (TransactionEventHandler)d;
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
                throw new DataBaseException(DatabaseMessages.Lock_Reference_Null);

            //deregistra lock
            this._LockAcquired.Remove(lockName);
        }

        /// <summary>
        /// Data una stringa sql aggiorna le statistiche in base al tipo
        /// </summary>
        /// <param name="sqlIn"></param>
        protected void updateStatsFromSQL(string sqlIn)
        {
            //Stoppa il timer di esecuzione
            this.Timer.Stop();
            for (int i = 0; i < sqlIn.Length; i++)
            {
                //Aggiorna Stats
                switch (sqlIn[i])
                {

                    case 'u':
                    case 'U':
                        this.Stats.Increment(DBStats.EStatement.Update);
                        return;
                    case 'i':
                    case 'I':
                        this.Stats.Increment(DBStats.EStatement.Insert);
                        return;
                    case 'd':
                    case 'D':
                        if (sqlIn.Length > 1 && char.ToUpper(sqlIn[i + 1]) != 'E')
                            break;
                        this.Stats.Increment(DBStats.EStatement.Delete);
                        return;
                    case 's':
                    case 'S':
                        this.Stats.Increment(DBStats.EStatement.Select);
                        return;
                    case ' ':
                        break;
                    default:
                        this.Stats.Increment(DBStats.EStatement.Other);
                        return;
                }
            }
        }



        /// <summary>
        /// Apre la connessione al database
        /// </summary>
        public void OpenConnection()
        {
            this.Timer.Restart();
            //apre connessione
            if (!this.IsConnectionOpen)
            {
                this._command.Transaction = null;
                this._dbconn.Open();

                if (this.IsPendingTransaction)
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
            this.TotRecordQueryPaginata = numRecords;
        }


        /// <summary>
        /// Crea nuova transazione
        /// </summary>
        /// <returns></returns>
        protected DbTransaction createNewTransaction(System.Data.IsolationLevel level)
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
        protected void TraceLog(string messageFmt, params object[] args)
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
            if (this._TraceLog == null || (this.TraceOnlyErrors && ex == null))
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
                    this._TraceLog.LogMessage(DatabaseMessages.TraceLog_Return_OK);
                }
                else
                {
                    this._TraceLog.LogMessage(DatabaseMessages.TraceLog_Return_ERR);
                    this._TraceLog.LogMessage(string.Concat("Exception: ", ex.Message));
                }
            }
            finally
            {
                this._TraceLog.EndSafeWrite(); //Chiude Thread-safe
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
        /// Dato un tipo di factory ritorna una istanza valorizzata
        /// </summary>
        /// <param name="facType"></param>
        /// <returns></returns>
        private DbProviderFactory GetDbFactory()
        {
            DbProviderFactory fact;

            if (!CommonDataBase.AssFactoryDictionary.TryGetValue(this.ProviderAssembly, out fact))
            {
                lock (CommonDataBase.AssFactoryDictionary)
                {
                    //Se qualcuno ha fatto prima!!
                    if (CommonDataBase.AssFactoryDictionary.TryGetValue(this.ProviderAssembly, out fact))
                        return fact;

                    //Carica assembly e tipo
                    var ass = AppDomain.CurrentDomain.Load(this.ProviderAssembly);
                    var facType = ass.GetType(this.ProviderFactoryClass);

                    //Il factory va cercato nel campo statico "Instance" del factory stesso
                    FieldInfo fieldInfo = facType.GetFields(BindingFlags.Static | BindingFlags.Public).Where(f => f.Name == @"Instance").FirstOrDefault();

                    if (fieldInfo == null)
                        throw new DataBaseException($"Non e' stato possibile recuperare l'istanza del provider {facType.FullName} dal suo campo statico 'Instance'");

                    fact = (DbProviderFactory)fieldInfo.GetValue(null);

                    CommonDataBase.AssFactoryDictionary.Add(this.ProviderAssembly, fact);

                }

            }

            return fact;

        }



        /// <summary>
        /// Carica impostazioni da factory corrente
        /// </summary>
        private void InitByFactory()
        {
            //Imposta i Dati specifici per il tipo database definito
            this._dbconn = this.ProviderFactory.CreateConnection();
            this._dbconn.ConnectionString = this.ConnectionString;
            this._command = this._dbconn.CreateCommand();
        }


        /// <summary>
        /// Inizializza attraverso oggetti ADO
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <param name="factory"></param>
        private void InitByADO(DbConnection conn, DbTransaction tran)
        {
            //Imposta i Dati specifici per il tipo database definito
            this._dbconn = conn;
            this._command = this._dbconn.CreateCommand();

            if (tran != null)
                this._tranQ.Push(tran);
        }

        #endregion
    }
}
