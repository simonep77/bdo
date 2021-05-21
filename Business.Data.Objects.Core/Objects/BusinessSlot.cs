using Business.Data.Objects.Common;
using Business.Data.Objects.Common.Cache;
using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Common.Logging;
using Business.Data.Objects.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Core.ObjFactory;
using Business.Data.Objects.Core.Schema.Definition;
using Business.Data.Objects.Core.Schema.Usage;
using Business.Data.Objects.Core.Utils;
using Business.Data.Objects.Core.Utils.BdoOnly;
using Business.Data.Objects.Database;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Data.Objects.Core
{

    /// <summary>
    /// Oggetto base che gestisce la vita degli altri oggetti di business
    /// consentendo in primo luogo l'accesso ai dati
    /// </summary>
    public class BusinessSlot:IComparable<BusinessSlot>, IEquatable<BusinessSlot>, IDisposable
    {

        #region PRIVATE FIELDS

        //Private vars
        private string mSlotId = Guid.NewGuid().ToString();
        private DateTime mStartDate = DateTime.Now;
        private EProtectionLevel mProtectionLevel = EProtectionLevel.Normal;
        private bool mTerminated;
        private IDataBase mDB;
        private DatabaseList mDbList = new DatabaseList();
        private string mUserName = string.Empty;
        private string mUserType = string.Empty;
        private bool mIsAuthenticated;
        private Dictionary<string, object> mProperties = new Dictionary<string, object>();
        private DbPrefixDictionary mDbPrefixKeys;

        private MessageList mMessageList = new MessageList();
        private System.Diagnostics.Stopwatch mStopWatch;

        //Base config
        public SlotConfig Conf { get; internal set; }

        //Object reference management
        internal ReferenceManager<string, DataObjectBase> mLiveTrackingStore;


        //Event management
        internal SlotEventManager mEventManager;

        //Static Data
        private static ICache<string, DataSchema> _GlobalCache;
        internal static CacheTimed<string, DataTable> _ListCache;
        private static LoggerBase _SharedLog;
        private static LoggerBase _DatabaseLog;
        private static SlotConfig _StaticConf;
        private static int _StaticConfCount = 0;


        #endregion


        #region EVENTS

        /// <summary>
        /// Delegato per la cattura del debug log
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public delegate void LogDebugHandler(BusinessSlot slot, DebugLevel level, string message);

        
        /// <summary>
        /// Evento scatenato dalle chiamate al metodo LogDebug() dello slot
        /// </summary>
        public event LogDebugHandler OnLogDebugSent;

        /// <summary>
        /// Funzione che ritorna una connessione db a partire da un nome
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public delegate IDataBase DbConnectionRequestHandler(string dbName);

        /// <summary>
        /// Evento scatenato dalla richiesta di un db non presente all'interno dello slot
        /// </summary>
        public event DbConnectionRequestHandler OnDbConnectionRequired;

        /// <summary>
        /// Delegato per ritornare il dato dell'utente da utilizzare con l'attributo "UserInfo"
        /// </summary>
        /// <returns></returns>
        public delegate object UserInfoRequestHandler(BusinessSlot slot);


        /// <summary>
        /// Evento da agganciare per specificare il dato dell'utente da salvare nel campo identificato dall'attributo "Username"
        /// </summary>
        public event UserInfoRequestHandler OnUserInfoRequired;

        /// <summary>
        /// Ritorna le informazioni sull'utente (id o username o altro) dall'evento relativo o, se nullo, dal campo username dello slot
        /// </summary>
        /// <returns></returns>
        internal object GetUserInfo()
        {
            return this.OnUserInfoRequired?.Invoke(this) ?? this.UserName;
        }


        #endregion


        #region PUBLIC PROPERTIES

        /// <summary>
        /// ID Univoco Sessione
        /// </summary>
        public string SlotId
        {
            get
            {
                return this.mSlotId;
            }
        }


        /// <summary>
        /// Data/Ora inizio della sessione corrente
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return this.mStartDate;
            }
        }


        /// <summary>
        /// Livello di protezione applicato allo slot
        /// </summary>
        public EProtectionLevel ProtectionLevel
        {
            get { return this.mProtectionLevel; }
            set { this.mProtectionLevel = value; }
        }


        /// <summary>
        /// Se impostato disattiva procedure di manipolazione dati
        /// degli oggetti: Save (Insert, update), Delete.
        /// </summary>
        public bool Simulate
        {
            get
            {
                return this.Conf.SimulateEnabled;
            }
            set
            {
                this.Conf.SimulateEnabled = value;
            }
        }


        /// <summary>
        /// Attiva/Disattiva tracciamento eventi su oggetto (che vanno comunque registrati attraverso le opportune funzioni di Register)
        /// </summary>
        public bool EventManagerEnabled
        {
            get
            {
                return this.Conf.EventManagerEnabled;
            }
            set
            {
                this.Conf.EventManagerEnabled = value;

                //Crea o elimina event manager
                if (!value)
                {
                    if (this.mEventManager != null)
                    {
                        this.mEventManager.Dispose();
                        this.mEventManager = null;
                    }
                }
                else
                {
                    if (this.mEventManager == null)
                    {
                        this.mEventManager = new SlotEventManager();
                    }
                }

            }
        }


        /// <summary>
        /// Gestore eventi associati allo slot.
        /// Attenzione: se non impostato EventManagementEnabled=true l'oggetto risulta null
        /// </summary>
        public SlotEventManager EventManager
        {
            get
            {
                return this.mEventManager;
            }
        }


        /// <summary>
        /// Attiva/Disattiva verifica modifiche reali su proprieta' di oggetti
        /// </summary>
        public bool ChangeTrackingEnabled
        {
            get
            {
                return this.Conf.ChangeTrackingEnabled;
            }
            set
            {
                this.Conf.ChangeTrackingEnabled = value;
            }
        }


        /// <summary>
        /// Indica se utilizzare il tracciamento degli oggetti: se un oggetto e' stato gi� caricato nella sessione di vita dello slot
        /// ad ogni nuova richiesta dello stesso viene ritornata sempre la stessa istanza. Ci� implica che una modifica ad un oggetto
        /// � IMMEDIATAMENTE ATTIVA su tutti gli oggetti che referenziano l'oggetto con la medesima chiave
        /// </summary>
        public bool LiveTrackingEnabled
        {
            get
            {
                return this.Conf.LiveTrackingEnabled;
            }
            set
            {
                this.liveTrackingActivation(value);
            }
        }


        /// <summary>
        /// Ritorna la dimensione del LiveTracking (numero di entry attualmente tracciate in memoria).
        /// Attenzione: non � detto che questo sia il numero effettivo di oggetti in memoria in quanto i riferimenti
        /// possono venir eliminati in qualsiasi momento dal GC (una entry potrebbe essere in lista ma con un riferimento ad oggetto nullo).
        /// </summary>
        public int LiveTrackingSize
        {
            get
            {
                return this.Conf.LiveTrackingEnabled ? this.mLiveTrackingStore.Count : 0;
            }
        }


        /// <summary>
        /// Ritorna numero degli oggetti in memoria
        /// </summary>
        public int LiveTrackingCurrentSize
        {
            get
            {
                return this.LiveTrackingEnabled ? this.mLiveTrackingStore.Count : -1;
            }
        }



        /// <summary>
        /// Indica che lo slot � stato terminato (Disposed())
        /// </summary>
        public bool Terminated
        {
            get
            {
                return this.mTerminated;
            }
        }


        /// <summary>
        /// Istanza di database associata alla sessione
        /// </summary>
        public IDataBase DB
        {
            get
            {
                return this.mDB;
            }
        }


        /// <summary>
        /// Indica l'utente associato alla sessione corrente
        /// Solo ad uso applicativo.
        /// </summary>
        public string UserName
        {
            get
            {
                return this.mUserName;
            }
            set
            {
                this.mUserName = value;
            }
        }


        /// <summary>
        /// Indica il tipo di utente associato alla sessione.
        /// Ad esclusivo uso applicativo.
        /// </summary>
        public string UserType
        {
            get
            {
                return this.mUserType;
            }
            set
            {
                this.mUserType = value;
            }
        }


        /// <summary>
        /// Indica se la sessione � stata esplicitamente Autenticata.
        /// Solo ad uso utente.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return this.mIsAuthenticated; }
            set { this.mIsAuthenticated = value; }
        }


        /// <summary>
        /// Indica o imposta l'utilizzo o meno del caching
        /// </summary>
        public bool CachingEnabled
        {
            get { return this.Conf.CachingEnabled; }
            set { this.Conf.CachingEnabled = value; }
        }


        /// <summary>
        /// Lista Messaggi pubblica
        /// </summary>
        public MessageList MessageList
        {
            get
            {
                return this.mMessageList;
            }
        }


        /// <summary>
        /// Il log utilizzabile per default
        /// </summary>
        public static LoggerBase SharedLog
        {
            get { return BusinessSlot._SharedLog; }
        }


        /// <summary>
        /// Indica il numero di istanze database definite
        /// </summary>
        public int DBCount
        {
            get
            { return this.mDbList.Count; }
        }

        /// <summary>
        /// Ottiene/Imposta i minuti di timeout da utilizzare nella cache delle liste. La modifica impatta solo i nuovi oggetti
        /// </summary>
        public int ListCacheTimeoutMinuti
        {
            get
            {
                return _ListCache.DefaultTimeoutMinuti;
            }
            set
            {
                _ListCache.DefaultTimeoutMinuti = (value > 0) ? value : 20;
            }
        }


        #endregion


        #region DELEGATES AND EVENTS


        /// <summary>
        /// Indica l'evento che si vuole gestire
        /// </summary>
        public enum EObjectEvent
        {
            /// <summary>
            /// Caricamento
            /// </summary>
            Load,

            /// <summary>
            /// Inserimento istanza
            /// </summary>
            Insert,

            /// <summary>
            /// Aggiornamento istanza
            /// </summary>
            Update,

            /// <summary>
            /// Eliminazione
            /// </summary>
            Delete
        }


        public delegate void BDEventPreHandler(DataObjectBase value, ref bool cancel);
        public delegate void BDEventPostHandler(DataObjectBase value);

        #endregion


        #region PUBLIC METHODS

        #region MULTI-THREAD LOOP

        public delegate void WorkListSlice<TL>(BusinessSlot slot, TL slice);

        /// <summary>
        /// Esegue un loop multithreading con attesa di completamento. Se si verificano errori all'interno dei thread lancia eccezione al completamento
        /// </summary>
        /// <typeparam name="TL"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="numThreads"></param>
        /// <param name="func"></param>
        public void LoopMT<TL, T>(TL list, int numThreads, WorkListSlice<TL> func)
            where TL : IEnumerable<T>
        {
            bool isSlotAware = list is SlotAwareObject;
            var pager = new DataPager();
            pager.Offset = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(list.Count()) / Convert.ToDecimal(numThreads)));
            pager.TotRecords = list.Count();

            var thl = new List<SlotAsyncWorkItem<T>>();
            //valuta esito
            var errorSlices = new ConcurrentBag<SlotAsyncWorkItem<T>>();

            //Crea un semaforo sul numero massimo di threads ed esegue
            for (int i = 0; i < pager.TotPages; i++)
            {
                pager.Page = i + 1;

                var slice = list.Skip(pager.Position).Take(pager.Offset).ToList();
                var slotCloned = this.Clone();
                //Se trattasi di oggetto con slot allora assegna allo slot clonato
                if (isSlotAware)
                    slice.Cast<SlotAwareObject>().ToList().ForEach(el => el.SwitchToSlot(slotCloned));

                //Aggancia eventuale log dei clonati sull'originale
                slotCloned.OnLogDebugSent = this.OnLogDebugSent;

                //Crea struttura dati dati per esecuzione
                var arg = new SlotAsyncWorkItem<T>()
                {
                    Page = i + 1,
                    Offset = pager.Offset,
                    Slot = slotCloned,
                    Slice = slice,
                    Func = func,
                    Exception = null
                };


                //Aggiunge a lista thread
                thl.Add(arg);

                //Crea ed esegue il task
                arg.Runtask = Task.Factory.StartNew((a) =>
                {
                    var innerArg = a as SlotAsyncWorkItem<T>;
                    try
                    {
                        innerArg.Func.DynamicInvoke(innerArg.Slot, innerArg.Slice);
                    }
                    catch(Exception e)
                    {
                        innerArg.Exception = e;
                        //terminato errore
                        errorSlices.Add(innerArg);
                    }
                    finally
                    {
                        //Disattiva debug slot clonato
                        innerArg.Slot.OnLogDebugSent = null;
                        //Switch sul vecchio slot
                        if (isSlotAware)
                            innerArg.Slice.Cast<SlotAwareObject>().ToList().ForEach(el => el.SwitchToSlot(this));

                        innerArg.Slot.Dispose();
                    }
                }, arg);
            }

            //Attende completamento
            Task.WaitAll(thl.Select(a => a.Runtask).ToArray());


            //Gestione cumulata errori threads
            if (errorSlices.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Si sono verificati i seguenti errori nell'esecuzione del loop: ");
                foreach (var item in errorSlices)
                {
                    sb.AppendFormat($"Thd {item.Runtask.Id}, porzione {item.Page} di {pager.TotPages}: {item.Exception.Message}");
                    sb.AppendLine();
                    sb.AppendLine(item.Exception.StackTrace);
                }

                //Lancia eccezione unica
                throw new BusinessSlotException(sb.ToString());
            }

        }


        #endregion


        #region SLOT PROPERTIES

        /// <summary>
        /// Elimina tutte le proprieta'
        /// </summary>
        public void PropertyClear()
        {
            this.mProperties.Clear();
        }

        /// <summary>
        /// Elimina proprieta'
        /// </summary>
        /// <param name="key"></param>
        public void PropertyRemove(string key)
        {
            this.mProperties.Remove(key);
        }

        /// <summary>
        /// Imposta proprieta'. Se gia' presente sostituisce il valore
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void PropertySet(string key, object value)
        {
            this.mProperties[key] = value;
        }

        /// <summary>
        /// Verifica esistenza proprieta'
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool PropertyExist(string key)
        {
            return this.mProperties.ContainsKey(key);
        }

        #region PROPERTYGET SIMPLE


        /// <summary>
        /// Ritorna valore proprieta' con priorita' al settings. Se non presente ritorna eccezione
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object PropertyGet(string key)
        {
            return this.PropertyGet(key, false);
        }


        /// <summary>
        /// Ritorna proprieta' impostandone la priorita' di ricerca
        /// </summary>
        /// <param name="key"></param>
        /// <param name="settingFirst"></param>
        /// <returns></returns>
        public object PropertyGet(string key, bool settingFirst)
        {
            object oRet = null;

            //Quindi lo cerca nella lista interna
            this.mProperties.TryGetValue(key, out oRet);

            if (oRet == null)
                throw new BusinessSlotException($"PropertyGet - La proprieta' {key} non risulta impostata");

            return oRet;
        }


        /// <summary>
        /// Ritorna valore di proprieta' e, se inesistente, il valore di default
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public object PropertyGetWithDefault(string key, object defaultValue)
        {
            object oRet = null;

            //Quindi lo cerca nella lista interna
            this.mProperties.TryGetValue(key, out oRet);

            if (oRet == null)
                oRet = defaultValue;

            return oRet;
        }

        #endregion

        #region PROPERTYGET GENERICS

        /// <summary>
        /// Ritorna valore proprieta' con priorita' al settings. Se non presente ritorna eccezione
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T PropertyGetT<T>(string key)
        {
            return this.PropertyGetT<T>(key, false);
        }

        /// <summary>
        /// Ritorna proprieta' impostandone la priorita' di ricerca
        /// </summary>
        /// <param name="key"></param>
        /// <param name="settingFirst"></param>
        /// <returns></returns>
        public T PropertyGetT<T>(string key, bool settingFirst)
        {
            return (T)Convert.ChangeType(this.PropertyGet(key, settingFirst), typeof(T));
        }

        /// <summary>
        /// Ritorna valore di proprieta' e, se inesistente, il valore di default
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T PropertyGetWithDefaultT<T>(string key, T defaultValue)
        {
            return (T)Convert.ChangeType(this.PropertyGetWithDefault(key, defaultValue), typeof(T));
        }

        #endregion


        /// <summary>
        /// Ritorna numero di proprieta' definite + numero di chiavi ConfigurationManager.AppSettings
        /// </summary>
        /// <returns></returns>
        public int PropertyCount()
        {
            return this.mProperties.Count;
        }

        /// <summary>
        /// Ritorna tutte le chiavi di proprieta' + chiavi ConfigurationManager.AppSettings definite
        /// </summary>
        /// <returns></returns>
        public List<PropertyIdentifier> PropertyAllKeys()
        {
            List<PropertyIdentifier> outList = new List<PropertyIdentifier>(this.PropertyCount());

            //Propereties
            foreach (var item in this.mProperties.Keys)
            {
                outList.Add(new PropertyIdentifier { Key = item });
            }

            return outList;
        }

        #endregion


        #region CACHING

        /// <summary>
        /// Svuota cache globale delle liste
        /// </summary>
        public void ResetListGlobal()
        {
            _ListCache.Reset();
        }

        /// <summary>
        /// Pulisce il contenuto della cache globale
        /// </summary>
        public void ResetCacheGlobal()
        {
            _GlobalCache.Reset();
        }


        /// <summary>
        /// Indica se un oggetto e' memorizzabile in una cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsCacheable<T>() where T : DataObject<T>
        {
            ClassSchema sc = ProxyAssemblyCache.Instance.GetClassSchema(typeof(T));
            return this.IsCacheable(sc);
        }

        #endregion


        #region DATABASE

        #region NEW

        /// <summary>
        /// Ritorna elenco nomi database registrati
        /// </summary>
        public string[] DbGetNames()
        {
            string[] names = new string[this.mDbList.Count];
            int iIndex = 0;

            foreach (var opair in this.mDbList)
            {
                names[iIndex] = opair.Key;
                iIndex++;
            }

            return names;
        }


        /// <summary>
        /// Ottiene istanza db associata allo schema
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        internal IDataBase DbGet(ClassSchema schema)
        {
            return schema.IsDefaultDb ? this.mDB : this.DbGet(schema.DbConnDef.Name);
        }

        /// <summary>
        /// Ritorna il database identificato dal nome 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDataBase DbGet(string name)
        {
            //Se non default
            IDataBase db = null;

            if (!this.mDbList.TryGetValue(name, out db))
            {
                //Se impostata esegue la richiesta della connessione specificata
                db = this.OnDbConnectionRequired?.Invoke(name);

                if (db == null)
                    throw new ObjectException(SessionMessages.DB_Not_Exist, name);

                this.DbAdd(name, db);
            }

            return db;
        }


        /// <summary>
        /// Ritorna istanza database specifica per il tipo di oggetto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IDataBase DbGet<T>() where T : DataObjectBase
        {
            ClassSchema sc = ProxyAssemblyCache.Instance.GetClassSchema(typeof(T));
            return this.DbGet(sc);
        }

        /// <summary>
        /// Aggiunge db a lista
        /// </summary>
        /// <param name="name"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public IDataBase DbAdd(string name, IDataBase db)
        {
            //Imposta parametri logging database
            if (this.Conf.LogDatabaseActivity)
                db.EnableTrace(_DatabaseLog, this.Conf.LogDatabaseOnlyErrors); //Imposta logger comune

            //Aggiunge
            this.mDbList.Add(name, db);

            //Ritorna se stesso
            return db;
        }


        /// <summary>
        /// Aggiunge allo slot un'altra istanza database identificata da un nome,
        /// di tipo specificato e con connection string fornita
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public IDataBase DbAdd(string name, string dbType, string connectionString)
        {
            return this.DbAdd(name, DataBaseFactory.CreaDataBase(dbType, connectionString));
        }


        /// <summary>
        /// Rimuove database da elenco specificando se eventualmente eseguire rollback di transazioni appese
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rollbackUnCommited"></param>
        public void DbRemove(string name, bool rollbackUnCommited)
        {
            IDataBase db = this.DbGet(name);

            if (name.Equals(Constants.STR_DB_DEFAULT))
                throw new ArgumentException(SessionMessages.Cannot_Delete_Default_DB);

            //Rimuove da elenco
            this.mDbList.Remove(name);

            //Chiude connessione
            db.CloseConnection(rollbackUnCommited);
        }


        /// <summary>
        /// Ritorna una statistica compelssiva di tutte le attivit� database
        /// </summary>
        /// <returns></returns>
        public DBStats DbGetStatsAll()
        {
            return this.mDbList.GetAllStats();
        }


        /// <summary>
        /// Apre transazione su tutti i database collegati.
        /// Se fornito "Unspecified" viene utilizzato quello di default per ciascuna tipologia di db
        /// </summary>
        public void DbBeginTransAll(IsolationLevel level = IsolationLevel.Unspecified)
        {
            this.mDbList.BeginTransAll(level);
        }

        /// <summary>
        /// Esegue il commit su tutti i database collegati
        /// </summary>
        public void DbCommitAll()
        {
            this.mDbList.CommitAll();

        }

        /// <summary>
        /// Esegue il rollback su tutti i database collegati
        /// </summary>
        public void DbRollBackAll()
        {
            this.mDbList.RollbackAll();
        }


        #endregion


    
        #region DBPREFIX

        /// <summary>
        /// Dizionario contenente i nomi database (identificati da una chiave case sensitive) che verranno utilizzati nelle calssi che lo necessitano
        /// </summary>
        public DbPrefixDictionary DbPrefixKeys
        {
            get
            {   //Auto-istanzia
                if (this.mDbPrefixKeys == null)
                    this.mDbPrefixKeys = new DbPrefixDictionary();

                return this.mDbPrefixKeys;
            }
        }

        /// <summary>
        /// Risolve il nome della tabella con eventuale aggiunta della componente DB
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        internal string DbPrefixGetTableName(Attributes.Table table)
        {
            return table.IsSimpleTableName
                ? table.Name
                : string.Concat(this.DbPrefixKeys[table.DbPrefixKey], table.Name);
        }


        /// <summary>
        /// Dato un dataobject (tipo) ritorna il nome completo di tabella risolto eventualmente della componente DB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string DbPrefixGetTableName<T>()
            where T : DataObject<T>
        {
            var schema = ProxyAssemblyCache.Instance.GetClassSchema(typeof(T));

            try
            {
                return this.DbPrefixGetTableName(schema.TableDef);
            }
            catch (Exception)
            {
                throw new ObjectException(ObjectMessages.Base_NoDbPrefixDefined, schema.ClassName, schema.TableDef.DbPrefixKey);
            }
        }

        /// <summary>
        /// Data una classe ed un nome di proprieta' ritorna il nome del campo DB. 
        /// Utile quando differenti rispetto alla nomenclatura della classe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string DbPrefixGetColumn<T>(string propertyName)
            where T : DataObject<T>
        {
            return DbPrefixGetColumn(typeof(T), propertyName);
        }


        /// <summary>
        /// Data una classe ed un nome di proprieta' ritorna il nome del campo DB. 
        /// Utile quando differenti rispetto alla nomenclatura della classe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal string DbPrefixGetColumn(Type type, string propertyName)
        {
            return ProxyAssemblyCache.Instance.GetClassSchema(type).Properties.GetPropertyByName(propertyName).Column.Name;
        }


        /// <summary>
        /// Ritorna il nome completo di tabella a partire da DbPrefixKey e nome tabella
        /// </summary>
        /// <param name="prefixKey"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string DbPrefixGetTableName(string prefixKey, string tableName)
        {
            try
            {
                return string.Concat(this.DbPrefixKeys[prefixKey], tableName);
            }
            catch (Exception)
            {
                throw new ObjectException(@"Chiave '{0}' non valorizzata nel DbPrefixKeys dello slot", prefixKey);
            }
        }



        #endregion

        #endregion


        #region LOGGING

        /// <summary>
        /// Scrive LogDebug. per utilizzarlo � necessario agganciare l'evento OnLogDebugSent
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msgFmt"></param>
        /// <param name="args"></param>
        public void LogDebug(DebugLevel level, string msgFmt, params object[] args)
        {
            if (this.OnLogDebugSent == null)
                return;

            this.OnLogDebugSent(this, level, string.Format(msgFmt, args));
        }


        /// <summary>
        /// Scrive LogDebug con livello default User_1
        /// </summary>
        /// <param name="msgFmt"></param>
        /// <param name="args"></param>
        public void LogDebug(string msgFmt, params object[] args)
        {
            this.LogDebug(DebugLevel.User_1, msgFmt, args);
        }


        /// <summary>
        /// Scrive LogDebugException
        /// </summary>
        /// <param name="level"></param>
        /// <param name="e"></param>
        public void LogDebugException(DebugLevel level, Exception e)
        {
            if (this.OnLogDebugSent == null)
                return;

            //Scrive
            Exception oException = e;
            int iIndentEx = 0;
            int iInnerCount = 0;
     

            this.LogDebug(level, Constants.LOG_SEPARATOR);

            while (oException != null)
            {
                string sIndent = string.Empty.PadRight(iIndentEx);

                this.LogDebug(level, @"{0}ECCEZIONE! Livello {1}", sIndent, iInnerCount.ToString());
                this.LogDebug(level, @"{0}  + Tipo     : {1}", sIndent, oException.GetType().Name);
                this.LogDebug(level, @"{0}  + Messaggio: {1}", sIndent, oException.Message);
                //Dati variabili
                if (!string.IsNullOrEmpty(oException.Source))
                    this.LogDebug(level, @"{0}  + Source   : {1}", sIndent, oException.Source);

                if (oException.TargetSite != null)
                {
                    this.LogDebug(level, @"{0}  + Classe   : {1}", sIndent, oException.TargetSite.DeclaringType.Name);
                    this.LogDebug(level, @"{0}  + Metodo   : {1}", sIndent, oException.TargetSite.Name);
                    this.LogDebug(level, @"{0}  + Namespace: {1}", sIndent, oException.TargetSite.DeclaringType.Namespace);
                }

                if (oException.StackTrace != null)
                {
                    this.LogDebug(level, @"{0}  + Stack    :", sIndent);

                    using (System.IO.StringReader reader = new System.IO.StringReader(oException.StackTrace))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            this.LogDebug(level, @"{0}             > {1}", sIndent, line);
                        }
                    }
                }

                //Successiva
                oException = oException.InnerException;
                iInnerCount++;
                iIndentEx += 4;
            }
        }


        /// <summary>
        /// Scrive LogDebugException con livello default Error_1
        /// </summary>
        /// <param name="e"></param>
        public void LogDebugException(Exception e)
        {
            this.LogDebugException(DebugLevel.Error_1, e);
        }


        #endregion


        #region OBJECT MANAGEMENT

        #region LOADING OBJECTS

        /// <summary>
        /// Carica oggetto a partire da un filtro fornito
        /// </summary>
        /// <param name="origType"></param>
        /// <param name="raiseNotFound"></param>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        internal DataObjectBase LoadObjectInternalByFILTER(Type origType, bool raiseNotFound, IFilter filter, OrderBy order)
        {

            //Verifica dati passati
            if (filter == null )
                throw new BusinessSlotException(SessionMessages.LoadObj_Filter_Null);

            //Crea oggetto vuoto
            DataObjectBase oNewObj = (DataObjectBase)ProxyAssemblyCache.Instance.CreateDaoObj(origType, true);

            //Imposta slot
            oNewObj.SetSlot(this);

            //carica
            oNewObj.LoadByFilter(filter, order);

            //Se lo stato risulta non caricato gestisce casistica
            if (oNewObj.ObjectState != EObjectState.Loaded)
            {
                if (raiseNotFound)
                    //Richiesta eccezione
                    throw new ObjectNotFoundException(ObjectMessages.Base_Record_Filter_NotFound, oNewObj.mClassSchema.ClassName);
                else
                    //Richiesto valore null
                    return null;
            }

            //Imposta source
            oNewObj.mDataSchema.ObjectSource = EObjectSource.Database;

            //Prova ad inserire nelle cache
            this.CacheSetAny(oNewObj);

            //Gestione evento Load
            if (this.EventManagerEnabled)
                this.EventManager.RunPostEventHandlerQueue(EObjectEvent.Load, oNewObj);

            //Ritorna oggetto creato
            return oNewObj;
        }


        /// <summary>
        /// Routine di caricamento oggetto con 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="origType"></param>
        /// <param name="raiseNotFound"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal DataObjectBase LoadObjectInternalByKEY(string keyName, Type origType, bool raiseNotFound, object[] values)
        {
            //Verifica dati passati
            if (values == null || values.Length == 0)
                throw new ObjectException(ObjectMessages.Base_NoValueForKey, origType.Name, keyName);

            //Carica schema
            var schema = ProxyAssemblyCache.Instance.GetClassSchema(origType);
            var oKey = schema.Keys[keyName];
            var bIsPk = (oKey.KeyIndex == schema.PrimaryKey.KeyIndex);
            string uPkHash = null;

            //Verifico numero parametri rispetto alla chiave specificata
            if (values.Length < oKey.Properties.Count)
                throw new ObjectException(ObjectMessages.Base_KeyValuesLessThanFields, origType.Name, keyName, oKey.Properties.Count);


            //Se pk
            if (bIsPk)
            {
                //Calcola hash pk
                uPkHash = ObjectHelper.GetObjectHashString(this, schema, values);

                //Verifica subito tracking
                if (this.LiveTrackingEnabled)
                {
                    //Calcola hash chiave
                    DataObjectBase obj = this.liveTrackingGet(uPkHash);

                    //Trovato
                    if (obj != null)
                        return obj;
                }
            }


            DataObjectBase oNewObj = (DataObjectBase)ProxyAssemblyCache.Instance.CreateDaoObj(origType, false);

            //Imposta slot
            oNewObj.SetSlot(this);

            //Cerca in cache se previsto
            if (bIsPk)
                oNewObj.mDataSchema = this.cacheGetPipeline(uPkHash, oNewObj.mClassSchema);

            //Deve caricare oggetto
            if (oNewObj.mDataSchema == null)
            {
                //Crea dataschema 
                oNewObj.mDataSchema = new DataSchema(oNewObj.mClassSchema.Properties.Count, oNewObj.mClassSchema.ObjCount);

                //carica
                oNewObj.LoadBySchemaKey(oKey, values);

                //Se lo stato risulta non caricato gestisce casistica
                if (oNewObj.ObjectState != EObjectState.Loaded)
                {
                    if (raiseNotFound)
                        //Richiesta eccezione
                        throw new ObjectNotFoundException(ObjectMessages.Base_RecordKeyNotFound, oNewObj.mClassSchema.ClassName, oKey.Name, ObjectHelper.ObjectEnumerableToString(values));
                    else
                        //Richiesto valore null
                        return null;
                }
                //Imposta source
                oNewObj.mDataSchema.ObjectSource = EObjectSource.Database;

                //Se PL impostiamo hash gia' calcolato (evitiamo un calcolo inutile)
                if(bIsPk)
                    oNewObj.mDataSchema.PkHash = string.Intern(uPkHash);

                //Salva in cache se previsto solo per oggetti caricati dal db
                this.cacheSetPipeline(oNewObj);
            }
            else
            {
                //trovato in cache
            }

            //Deve salvare tracking (anche per oggetti cached)
            if (this.LiveTrackingEnabled)
                this.liveTrackingSet(oNewObj);


            //Gestione evento Load
            if (this.EventManagerEnabled)
                this.EventManager.RunPostEventHandlerQueue(EObjectEvent.Load, oNewObj);

            //Ritorna oggetto creato
            return oNewObj;
        }


        /// <summary>
        /// Carica oggetto da chiave definita e se non esiste ritorna nuovo oggetto precaricato con i valori utilizzando il type specificato
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="origType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal DataObjectBase LoadObjOrNewInternalByKEY(string keyName, Type origType, params object[] values)
        {
            var obj = this.LoadObjectInternalByKEY(keyName, origType, false, values);

            if (obj == null)
            {
                //Crea istanza
                obj = this.CreateObjectByType(origType);

                Key oKey = obj.mClassSchema.Keys[keyName];

                for (int i = 0; i < oKey.Properties.Count; i++)
                {
                    Property oProp = oKey.Properties[i];

                    //Controlla se possibile impostare la proprieta'
                    if (!oProp.IsAutomatic)
                    {
                        oProp.SetValue(obj, values[i]);
                    }
                }
            }

            //Ritorna
            return obj;
        }


        internal DataObjectBase LoadObjOrNewInternalByFILTER(Type origType, IFilter filter, OrderBy order)
        {
            var obj = this.LoadObjectInternalByFILTER(origType, false, filter, order);

            if (obj == null)
            {
                //Crea istanza
                obj = this.CreateObjectByType(origType);


                foreach (var filt in filter)
                {
                    if (filt.Operator != EOperator.Equal)
                        continue;

                    foreach (var prop in obj.mClassSchema.Properties)
                    {
                        if (!prop.IsAutomatic &&
                           filt.Name.ToUpper() == prop.Name.ToUpper())

                        {
                            //Imposta proprieta'
                            prop.SetValue(obj, filt.Value);
                            break; //Esce da ciclo proprieta'
                        }
                    }
                }

            }

            //Ritorna
            return obj;
        }


        /// <summary>
        /// Carica Oggetto Da Chiave Primaria
        /// Se oggetto non trovato viene lanciata eccezione
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public T LoadObjByPK<T>(params object[] values) where T : DataObjectBase
        {
            return (T)this.LoadObjectInternalByKEY(ClassSchema.PRIMARY_KEY, typeof(T), true, values);
        }


        /// <summary>
        /// Carica oggetto da chiave secondaria
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public T LoadObjByKEY<T>(string keyName, params object[] values) where T : DataObjectBase
        {
            return (T)this.LoadObjectInternalByKEY(keyName, typeof(T), true, values);
        }

        /// <summary>
        /// Carica oggetto da filtro custom.
        /// Se non trovato lancia eccezione.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T LoadObjByFILTER<T>(IFilter filter) where T : DataObjectBase
        {
            return (T)this.LoadObjectInternalByFILTER(typeof(T), true, filter, null);
        }

        /// <summary>
        /// Carica oggetto da filtro custom.
        /// Se non trovato lancia eccezione.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public T LoadObjByFILTER<T>(IFilter filter, OrderBy order) where T : DataObjectBase
        {
            return (T)this.LoadObjectInternalByFILTER(typeof(T), true, filter, order);
        }

        /// <summary>
        /// Carica oggetto da PK e se non esiste ritorna null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public T LoadObjNullByPK<T>(params object[] values) where T : DataObjectBase
        {
            return (T)this.LoadObjectInternalByKEY(ClassSchema.PRIMARY_KEY, typeof(T), false, values);
        }


        /// <summary>
        /// Carica oggetto da chiave definita
        /// </summary>
        /// <param name="keyName">
        /// Nome della chiave definita sull'oggetto
        /// </param>
        /// <param name="values"></param>
        /// <returns></returns>
        public T LoadObjNullByKEY<T>(string keyName, params object[] values) where T : DataObjectBase
        {
            return (T)this.LoadObjectInternalByKEY(keyName, typeof(T), false, values);
        }

        /// <summary>
        /// Carica oggetto da filtro custom.
        /// Se non trovato ritorn NULL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T LoadObjNullByFILTER<T>(IFilter filter) where T : DataObjectBase
        {
            return (T)this.LoadObjectInternalByFILTER(typeof(T), false, filter, null);
        }


        /// <summary>
        /// Carica oggetto da filtro custom.
        /// Se non trovato ritorn NULL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T LoadObjNullByFILTER<T>(IFilter filter, OrderBy order) where T : DataObjectBase
        {
            return (T)this.LoadObjectInternalByFILTER(typeof(T), false, filter, order);
        }

        /// <summary>
        /// Carica oggetto da PK e se non esiste ritorna nuovo oggetto precaricato con i valori
        /// richiesti
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public T LoadObjOrNewByPK<T>(params object[] values) where T : DataObjectBase
        {
            return (T)this.LoadObjOrNewInternalByKEY(ClassSchema.PRIMARY_KEY, typeof(T), values);
        }


        /// <summary>
        /// Carica oggetto da chiave definita e se non esiste ritorna nuovo oggetto precaricato con i valori
        /// richiesti
        /// </summary>
        /// <param name="keyName">
        /// Nome della chiave definita sull'oggetto
        /// </param>
        /// <param name="values"></param>
        /// <returns></returns>
        public T LoadObjOrNewByKEY<T>(string keyName, params object[] values) where T : DataObjectBase
        {
            return (T)this.LoadObjOrNewInternalByKEY(keyName, typeof(T), values);
        }

        /// <summary>
        ///  Carica oggetto da filtro custom e se non esiste ritorna nuovo oggetto precaricato con i valori dei soli filtri EQUAL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T LoadObjOrNewByFILTER<T>(IFilter filter) where T : DataObjectBase
        {
            return (T)this.LoadObjOrNewInternalByFILTER(typeof(T), filter, null);
        }

        /// <summary>
        /// Carica oggetto da filtro custom e se non esiste ritorna nuovo oggetto precaricato con i valori dei soli filtri EQUAL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public T LoadObjOrNewByFILTER<T>(IFilter filter, OrderBy order) where T : DataObjectBase
        {
            return (T)this.LoadObjOrNewInternalByFILTER(typeof(T), filter, order);
        }


        #endregion


        #region CREATING OBJECTS

        /// <summary>
        /// Crea nuova istanza di oggetto non tipizzato
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal DataObjectBase CreateObjectByType(Type dalTypeIn)
        {
            DataObjectBase o = (DataObjectBase)ProxyAssemblyCache.Instance.CreateDaoObj(dalTypeIn, true);
            o.SetSlot(this);

            //Ritorna oggetto appena creato
            return o;
        }

        /// <summary>
        /// Crea nuova istanza di oggetto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateObject<T>() where T : DataObjectBase
        {
            T o = (T)ProxyAssemblyCache.Instance.CreateDaoObj(typeof(T), true);
            o.SetSlot(this);

            //Ritorna oggetto appena creato
            return o;
        }

        /// <summary>
        /// Crea una lista non paginata
        /// </summary>
        /// <returns></returns>
        public TL CreateList<TL>() where TL : DataListBase
        {
            TL oList = (TL)ProxyAssemblyCache.Instance.CreateDaoNoSchemaObj(typeof(TL));
            oList.SetSlot(this);

            return oList;
        }


        /// <summary>
        /// Crea una lista paginata
        /// </summary>
        /// <param name="page"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public TL CreatePagedList<TL>(int page, int offset) where TL : DataListBase
        {
            TL oList = this.CreateList<TL>();
            oList.Pager = new DataPager();
            oList.Pager.Page = page;
            oList.Pager.Offset = offset;

            return oList;
        }

        #endregion


        #region SAVING

        /// <summary>
        /// Salva l'oggetto in input utilizzando questa sessione
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void SaveObject<T>(T obj) where T : DataObjectBase
        {
            //Se null errore
            if (obj == null)
                throw new ObjectException(ObjectMessages.Base_Save_Null, typeof(T).Name);

            //Richiama salvataggio oggetto
            obj.SetSlot(this);

            bool bCancel = false;

            //Richiama evento pre salvataggio
            if (this.EventManagerEnabled)
                this.EventManager.RunPreEventHandlerQueue((obj.ObjectState == EObjectState.New) ? EObjectEvent.Insert : EObjectEvent.Update, obj, ref bCancel);


            //Se simulazione non esegue
            if (bCancel || this.Simulate)
                return;

            //Salva oggetto
            bool isChanged = (obj.DoSave() == ESaveResult.SaveDone);

            //Se non e' avvenuta una modifica esce e non esegue il resto
            if (!isChanged)
                return;

            //Se tracking lo salva
            if (this.LiveTrackingEnabled)
                this.liveTrackingSet(obj);

            //Richiama evento post salvataggio
            if (this.EventManagerEnabled)
                this.EventManager.RunPostEventHandlerQueue((obj.ObjectSource == EObjectSource.None) ? EObjectEvent.Insert : EObjectEvent.Update, obj);
        }

        /// <summary>
        /// Elimina oggetto utilizzando questa sessione
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void DeleteObject<T>(T obj, bool bypassLogical = false) where T : DataObjectBase
        {
            //Se null errore
            if (obj == null)
                throw new ObjectException(ObjectMessages.Base_Delete_Null, typeof(T).Name);

            //Richiama salvataggio oggetto
            obj.SetSlot(this);

            bool bCancel = false;
            //Richiama evento pre cancellazione
            if (this.EventManagerEnabled)
                this.EventManager.RunPreEventHandlerQueue(EObjectEvent.Delete, obj, ref bCancel);

            //Se simulazione non esegue
            if (bCancel || this.Simulate)
                return;

            //Infine esegue cancellazione db
            //Se cancellazione logica allora imposta campo
            if (obj.mClassSchema.LogicalDeletes.Count == 0 || bypassLogical)
            {
                //Elimina fisicamente
                obj.DoDelete();

            }
            else
            {
                //Imposta valore a seconda del tipo
                foreach (var ldProp in obj.mClassSchema.LogicalDeletes)
                {
                    if (ldProp.DefaultValue is DateTime)
                        ldProp.SetValue(obj, DateTime.Now);
                    else
                        ldProp.SetValue(obj, 1);
                }

                //Esegue aggiornamento
                this.SaveObject(obj);

                //Forziamo indicazione di oggetto eliminato??
                //Imposta stato eliminato
                obj.mDataSchema.ObjectState = EObjectState.Deleted;
            }

            //Se tracking lo salva
            if (this.LiveTrackingEnabled)
                this.liveTrackingRemove(obj);

            //Richiama evento se presente
            if (this.EventManagerEnabled)
                this.EventManager.RunPostEventHandlerQueue(EObjectEvent.Delete, obj);
        }


        #endregion


        #region DELETING

        /// <summary>
        /// Salva tutti gli oggetti della lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TL"></typeparam>
        /// <param name="list"></param>
        public void SaveAll<TL>(TL list)
            where TL : DataListBase
        {
            //Se null errore
            if (list == null)
                throw new ObjectException(ObjectMessages.Base_Save_Null, typeof(TL).Name);

            //Imposta se stesso come slot
            list.SetSlot(this);

            var lstCasted = list as IList;

            //Salva
            for (int i = 0; i < lstCasted.Count; i++)
            {
                var item = lstCasted[i] as DataObjectBase;
                this.SaveObject(item);
                list.mInnerList[i].PkHashCode = item.mDataSchema.PkHash;
            }

        }


        /// <summary>
        /// Cancella tutti gli elementi di una lista. Al termine la lista risulta vuota
        /// </summary>
        /// <param name="list"></param>
        public void DeleteAll<TL>(TL list, bool bypassLogical = false)
            where TL : DataListBase
        {
            //Se null errore
            if (list == null)
            {
                throw new ObjectException(ObjectMessages.Base_Save_Null, typeof(TL).Name);
            }

            //Imposta se stesso come slot
            list.SetSlot(this);

            //Salva
            foreach (DataObjectBase item in (IEnumerable)list)
            {
                this.DeleteObject(item, bypassLogical);
            }

            //Svuota lista
            list.Clear();
        }


        #endregion


        #region CLONING

        /// <summary>
        /// Ritorna copia esatta dell'oggetto su nuova istanza
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public T CloneObject<T>(T other) where T : DataObjectBase
        {
            if (other == null)
                throw new ObjectException(ObjectMessages.Base_Null_Input, typeof(T).Name);

            //Se presente tracking ritorna la medesima istanza
            if (this.LiveTrackingEnabled)
                throw new BusinessSlotException(SessionMessages.Clone_LiveTrack_Not_Allowed);


            T o = (T)ProxyAssemblyCache.Instance.CreateDaoObj(other.mClassSchema.OriginalType);
            o.mDataSchema = other.mDataSchema.Clone(true, true);
            o.SetSlot(this);
            return o;
        }


        /// <summary>
        /// Ritorna una lista identica a quella in input
        /// </summary>
        /// <typeparam name="TL"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public DataList<TL, T> CloneList<TL, T>(DataList<TL, T> list)
            where T : DataObject<T>
            where TL : DataList<TL, T>
        {
            if (list == null)
                throw new ObjectException(ObjectMessages.Base_Null_Input, typeof(TL).Name);

            return list.Clone();

        }

        /// <summary>
        /// Ritorna copia dell'oggetto su nuova istanza azzerando la chiave primaria
        /// e impostando lo stato a nuovo
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public T CloneObjectForNew<T>(T other) where T : DataObjectBase
        {
            if (other == null)
                throw new ObjectException(ObjectMessages.Base_Null_Input, typeof(T).Name);

            T o = (T)ProxyAssemblyCache.Instance.CreateDaoObj(other.mClassSchema.OriginalType);
            o.mDataSchema = other.mDataSchema.Clone(false, false);
            //Imposta Sorgente e Stato a nuovo
            o.mDataSchema.ObjectSource = EObjectSource.None; 
            o.mDataSchema.ObjectState = EObjectState.New;
            //Imposto slot
            o.SetSlot(this);

            //Azzera Chiavi univoche
            foreach (var key in o.mClassSchema.Keys.Values)
            {
                foreach (var prop in key.Properties)
                {
                    prop.SetValue(o, prop.DefaultValue);
                }
            }

            return o;
        }


        #endregion


        #region MISC

        /// <summary>
        /// Esegue nuovamente il caricamento dalla sorgente (cache, db)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void RefreshObject<T>(T obj, bool fromDB) where T : DataObjectBase
        {
            Key oKey = obj.mClassSchema.PrimaryKey;
            DataSchema oNewData = null;
            object[] values = oKey.GetValues(obj);

            //Imposta slot
            obj.SetSlot(this);

            //Verifica caching
            bool bCacheable = this.IsCacheable(obj);

            //Cerca in cache se previsto
            if (bCacheable && !fromDB)
            {
                //Cerca
                oNewData = this.cacheGetPipeline(obj.GetHashBaseString(), obj.mClassSchema);
            }

            //Deve caricare oggetto
            if (oNewData == null)
            {
                //carica
                obj.LoadBySchemaKey(oKey, values);

                //Imposta source
                obj.mDataSchema.ObjectSource = EObjectSource.Database;

                //Salva in cache se previsto solo per oggetti caricati dal db
                this.cacheSetPipeline(obj);
            }
            else
            {
                obj.mDataSchema = oNewData;
            }
        }


        #endregion

        #endregion


        #region OBJECT EXTRA DATA

        /// <summary>
        /// Verifica se presenti dati aggiuntivi su oggetto (singolo o lista) individuati per chiave
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ExtraDataExist(SlotAwareObject obj, string key)
        {
            //Verifica
            return obj.ExtraDataExist(key);

        }

        /// <summary>
        /// Ritorna dati aggiuntivi memorizzati a livello di oggetto (singolo o lista)
        /// individuati per chiave. Se non trovata la chiave ritorna il valore defult fornito
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T ExtraDataGet<T>(SlotAwareObject obj, string key, T defaultValue)
        {
            return (T)obj.ExtraDataGet(key, defaultValue);
        }


        /// <summary>
        /// Imposta dati aggiuntivi su oggetto (singolo o lista) individuati per chiave
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ExtraDataSet<T>(SlotAwareObject obj, string key, T value)
        {
            //Extradata non valorizzato: lo crea
            obj.ExtraDataSet(key, value);
        }


        /// <summary>
        /// Rimuove dati aggiuntivi su oggetto (singolo o lista) individuati per chiave
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        public void ExtraDataRemove(SlotAwareObject obj, string key)
        {
            //Rimuove
            obj.ExtraDataRemove(key);
        }


        /// <summary>
        /// Elimina tutti i dati extra
        /// </summary>
        /// <param name="obj"></param>
        public void ExtraDataClear(SlotAwareObject obj)
        {
            obj.ExtraDataClear();
        }


        /// <summary>
        /// Ritorna tutte le chiavi registrate per l'oggetto
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string[] ExtraDataGetKeys(SlotAwareObject obj)
        {
            return obj.ExtraDataGetKeys();
        }


        /// <summary>
        /// Ritorna tutte le chiavi registrate per l'oggetto
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int ExtraDataCount(SlotAwareObject obj)
        {
            return obj.ExtraDataCount();
        }

        #endregion


        #region DEBUGGING

        /// <summary>
        /// Esegue il dump stampabile di un oggetto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string DebugObjectDump<T>(T obj) where T : DataObjectBase
        {
            StringBuilder sbDump = new StringBuilder(10000);

            sbDump.AppendLine("-- Dati Interni --");

            sbDump.Append("ObjectRefId".PadRight(30, ' '));
            sbDump.Append(@" : ");
            sbDump.Append(obj.ObjectRefId);
            sbDump.AppendLine();
            sbDump.Append("ObjectSource".PadRight(30, ' '));
            sbDump.Append(@" : ");
            sbDump.Append(obj.ObjectSource);
            sbDump.AppendLine();
            sbDump.Append("ObjectState".PadRight(30, ' '));
            sbDump.Append(@" : ");
            sbDump.Append(obj.ObjectState);
            sbDump.AppendLine();
            sbDump.Append("ObjectPkHash".PadRight(30, ' '));
            sbDump.Append(@" : ");
            sbDump.Append(obj.GetHashBaseString());
            sbDump.AppendLine();

            sbDump.AppendLine("-- Proprieta' --");
            for (int i = 0; i < obj.mClassSchema.Properties.Count; i++)
            {
                sbDump.Append(obj.mClassSchema.Properties[i].Name.PadRight(30, ' '));
                sbDump.Append(@" : ");
                sbDump.Append(obj.mClassSchema.Properties[i].GetValue(obj));
                sbDump.AppendLine();
            }

            string[] keysExtraData = this.ExtraDataGetKeys(obj);
            if (keysExtraData.Length > 0)
            {
                sbDump.AppendLine();
                sbDump.AppendLine("-- Dati Extra --");

                foreach (var item in this.ExtraDataGetKeys(obj))
                {
                    sbDump.Append(item.PadRight(30, ' '));
                    sbDump.Append(@" : ");
                    sbDump.Append(this.ExtraDataGet<object>(obj, item, null));
                    sbDump.AppendLine();
                }
            }

            return sbDump.ToString();
        }



        /// <summary>
        /// Ritorna misurazione di quanto trascorso dalla creazione dello slot
        /// Utile per calcolare performance/altro
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentElapsed()
        {
            return this.mStopWatch.Elapsed;
        }

        /// <summary>
        /// Stampa Informazioni Relative
        /// </summary>
        /// <returns></returns>
        public string PrintInfo()
        {
            StringBuilder sb = new StringBuilder(1000);
            sb.AppendLine("** SLOT INFO **");
            sb.Append("SlotID: ");
            sb.Append(this.mSlotId);
            sb.AppendLine();
            sb.Append("Start Time: ");
            sb.Append(this.mStartDate.ToString("dd/MM/yyyy HH:mm:ss"));
            sb.AppendLine();
            sb.Append("Elapsed Msec: ");
            sb.Append(this.GetCurrentElapsed().ToString());
            sb.AppendLine();
            sb.Append("Elapsed Ticks: ");
            sb.Append(this.GetCurrentElapsed().Ticks.ToString());
            sb.AppendLine();
            sb.Append("Authenticated: ");
            sb.Append(this.mIsAuthenticated);
            sb.AppendLine();
            sb.Append("User: ");
            sb.Append(this.mUserName);
            sb.AppendLine();
            sb.Append("User Type: ");
            sb.Append(this.mUserType);
            sb.AppendLine();
            sb.Append("Protection Level: ");
            sb.AppendLine(this.mProtectionLevel.ToString());
            sb.Append("Live Tracking Enabled: ");
            sb.AppendLine(this.LiveTrackingEnabled.ToString());
            sb.Append("Change Tracking Enabled: ");
            sb.AppendLine(this.ChangeTrackingEnabled.ToString());
            sb.Append("Event Manager Enabled: ");
            sb.AppendLine(this.EventManagerEnabled.ToString());
            sb.Append("Shared Log: ");
            sb.AppendLine(BusinessSlot._SharedLog.LogPath);
            sb.Append("ObjeRefIdCounter: ");
            sb.AppendLine(System.Threading.Interlocked.Read(ref ProxyAssemblyCache.Instance.ObjeRefIdCounter).ToString());
             

            sb.AppendLine("** PROPERTIES **");
            foreach (var item in this.PropertyAllKeys())
            {
                sb.AppendFormat("{0} ({1})", item.Key, (item.IsAppSetting ? "S" : "P"));
                sb.Append(": ");
                sb.AppendFormat("{0}", this.PropertyGet(item.Key, item.IsAppSetting));
                sb.AppendLine();
            }

            //Scrive info cache locale e globale
            sb.AppendLine();
            sb.AppendLine("** CACHE INFO **");
            sb.Append("Global Object Cache: ");
            sb.Append(BusinessSlot._GlobalCache.CurrentSize.ToString());
            sb.Append("/");
            sb.AppendLine(BusinessSlot._GlobalCache.MaxSize.ToString());


            sb.Append("Global List Cache: ");
            sb.Append(BusinessSlot._ListCache.CurrentSize.ToString());
            sb.Append("/");
            sb.AppendLine(BusinessSlot._ListCache.MaxSize.ToString());

            if (this.LiveTrackingEnabled)
            {
                sb.Append("Live Objects: ");
                sb.AppendLine(this.mLiveTrackingStore.Count.ToString());
            }


            sb.AppendLine();
            sb.AppendLine("** DATABASE INFO **");
            //Per ogni database
            foreach (KeyValuePair<string, IDataBase> opair in this.mDbList)
            {
                sb.Append("#Connessione ");
                sb.AppendLine(opair.Key);
                sb.Append(" - Type: ");
                sb.AppendLine(opair.Value.GetType().Name);
                sb.Append(opair.Value.Stats.ToString());
                sb.AppendLine();
            }


            //sb.AppendLine("** SETTINGS INFO **");
            //foreach (System.Configuration.SettingsPropertyValue oProp in Settings.Default.PropertyValues)
            //{
            //    sb.Append(oProp.Name);
            //    sb.Append(": ");
            //    sb.AppendLine(oProp.PropertyValue.ToString());
            //}


            if (this.EventManagerEnabled)
            {
                sb.AppendLine();
                sb.AppendLine("** EVENTS INFO **");
                sb.Append(this.EventManager.PrintInfo());
            }

            return sb.ToString();
        }


        /// <summary>
        /// Ritorna testo con dump della cache
        /// </summary>
        /// <returns></returns>
        public string PrintCacheDebug()
        {
            StringBuilder sb = new StringBuilder(1000);

            sb.AppendLine("**** GLOBAL OBJECT CACHE ****");
            sb.AppendLine();
            sb.AppendLine(BusinessSlot._GlobalCache.Print());


            sb.AppendLine("**** GLOBAL LIST CACHE ****");
            sb.AppendLine();
            sb.AppendLine(BusinessSlot._ListCache.Print());



            return sb.ToString();
        }


        /// <summary>
        /// Ritorna testo con dump del live tracking
        /// </summary>
        /// <returns></returns>
        public string PrintLiveTrackingDebug()
        {
            StringBuilder sb = new StringBuilder(10000);

            sb.AppendLine("**** LIVE TRACKING ****");
            sb.AppendLine();
            sb.AppendLine(this.mLiveTrackingStore.PrintDebug());


            return sb.ToString();
        }


        #endregion


        #region LIVE TRACKING

        /// <summary>
        /// Svuota la cache interna di tracking
        /// </summary>
        public void LiveTrackingClear()
        {
            if (!this.LiveTrackingEnabled)
                return;

            //Svuota il tracking store
            this.mLiveTrackingStore.Clear();
        }



        /// <summary>
        /// Se attivo object live tracking allora rimuove tutte le entry i cui oggetti risultano morti (gc-ed)
        /// </summary>
        /// <param name="issync"></param>
        public void LiveTrackingDeadScan(bool issync)
        {
            if (this.Conf.LiveTrackingEnabled)
                this.mLiveTrackingStore.CleanDeadEntries(issync);
        }

        #endregion


        #region SERIALIZATION


        /// <summary>
        /// Esegue serializzazione binaria oggetto. Per deserializzare utilizzare BinDeserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] BinSerialize<T>(T obj) where T : DataObject<T>
        {
            return DataSchema.BinSerialize(obj.mDataSchema);
        }

        /// <summary>
        /// Deserializza oggetto serializzato con BinSerialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public T BinDeserialize<T>(byte[] data) where T : DataObject<T>
        {
            T o = (T)ProxyAssemblyCache.Instance.CreateDaoObj(typeof(T));
            o.SetSlot(this);
            o.mDataSchema = DataSchema.BinDeserialize(data);
            return o;
        }

        #endregion

        #region BIZCREATORS

        /// <summary>
        /// Crea un business object con un oggetto dal vuoto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T BizNewWithCreateObj<T>()
            where T : BusinessObjectBase
        {
            return (T)ProxyAssemblyCache.Instance.CreateBizObj(typeof(T), this, false, null, null);
        }


        /// <summary>
        /// Crea una biz con LoadByKEY
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public T BizNewWithLoadByPK<T>(params object[] args)
            where T : BusinessObjectBase
        {
            return (T)ProxyAssemblyCache.Instance.CreateBizObj(typeof(T), this, false, ClassSchema.PRIMARY_KEY, args);
        }


        /// <summary>
        /// Crea una biz con LoadByKEY
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T BizNewWithLoadByKEY<T>(string keyName, params object[] args)
            where T : BusinessObjectBase
        {
            return (T)ProxyAssemblyCache.Instance.CreateBizObj(typeof(T), this, false, keyName, args);
        }



        /// <summary>
        /// Crea una biz con LoadOrNewByPK
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public T BizNewWithLoadOrNewByPK<T>(params object[] args)
            where T : BusinessObjectBase
        {
            return (T)ProxyAssemblyCache.Instance.CreateBizObj(typeof(T), this, true, ClassSchema.PRIMARY_KEY, args);
        }


        /// <summary>
        /// Crea una biz con LoadOrNewByKEY
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T BizNewWithLoadOrNewByKEY<T>(string keyName, params object[] args)
            where T : BusinessObjectBase
        {
            return (T)ProxyAssemblyCache.Instance.CreateBizObj(typeof(T), this, true, keyName, args);
        }



        #endregion

        #region MISC

        internal DataObjectBase FromDTO_AsNewByType(Type type,  Dictionary<string, object> dto)
        {
            var o = this.CreateObjectByType(type);

            for (int i = 0; i < o.mClassSchema.Properties.Count; i++)
            {
                o.mClassSchema.Properties[i].ReadDTO(dto, o);
            }

            return o;
        }

        internal DataObjectBase FromDTO_AsLoadedByType(Type type, Dictionary<string, object> dto)
        {
            var o = this.FromDTO_AsNewByType(type, dto);
            o.mDataSchema.ObjectSource = EObjectSource.DTO;
            o.mDataSchema.ObjectState = EObjectState.Loaded;

            return o;
        }

        /// <summary>
        /// Carica un oggetto da DTO by tipo e dto e lo rappresenta come nuovo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        public T FromDTO_AsNew<T>(Dictionary<string, object> dto) 
            where T : DataObjectBase
        {
            return (T)this.FromDTO_AsNewByType(typeof(T), dto);
        }

        /// <summary>
        ///  Carica un oggetto da DTO by tipo e dto e lo rappresenta come caricato
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        public T FromDTO_AsLoaded<T>(Dictionary<string, object> dto)
            where T : DataObjectBase
        {
            return (T)this.FromDTO_AsLoadedByType(typeof(T), dto);
        }




        /// <summary>
        /// Ritorna rappresentazione in stringa
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat("Slot ", this.mSlotId);
        }


        /// <summary>
        /// Dato uno slot crea una copia conforme:
        /// Viene generato un nuovo id ed una nuova connessione db
        /// </summary>
        /// <returns></returns>
        public BusinessSlot Clone()
        {
            //Crea slot con stessi parametri db
            BusinessSlot oCloned = new BusinessSlot(this.DB.Clone());

            //Imposta istanze database
            foreach (var item in this.mDbList)
            {
                if (!item.Key.Equals(Constants.STR_DB_DEFAULT))
                {
                    oCloned.DbAdd(item.Key, item.Value.Clone());
                }
            }

            //Imposta tutti gli attributi
            oCloned.mUserName = this.mUserName;
            oCloned.mUserType = this.mUserType;
            oCloned.mIsAuthenticated = this.mIsAuthenticated;
            oCloned.mProtectionLevel = this.mProtectionLevel;
            oCloned.mTerminated = this.mTerminated;
            oCloned.ChangeTrackingEnabled = this.ChangeTrackingEnabled;
            oCloned.LiveTrackingEnabled = this.LiveTrackingEnabled;
            oCloned.Simulate = this.Simulate;
            oCloned.CachingEnabled = this.CachingEnabled;

            //Imposta props
            foreach (var item in this.mProperties)
            {
                oCloned.mProperties.Add(item.Key, item.Value);
            }

            //Imposta i db names
            foreach (var item in this.DbPrefixKeys)
            {
                oCloned.DbPrefixKeys.Add(item.Key, item.Value);
            }

            //Imposta eventi
            oCloned.OnLogDebugSent = this.OnLogDebugSent;
            oCloned.OnDbConnectionRequired = this.OnDbConnectionRequired;
            oCloned.OnUserInfoRequired = this.OnUserInfoRequired;

            return oCloned;

        }

        #endregion


        #region XML


        #endregion

        #endregion


        #region PRIVATE METHODS

        /// <summary>
        /// Crea istanza a partire da una classe database
        /// </summary>
        /// <param name="db"></param>
        public BusinessSlot(IDataBase db)
        {
            //Crea istanza database associato
            this.InitSlot(db);
        }

        /// <summary>
        /// Crea istanza a partire da oggetti ADO
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        public BusinessSlot(DbConnection conn, DbTransaction tran)
        {
            //Crea istanza database associato
            this.InitSlot(DataBaseFactory.CreaDataBaseFromADO(conn, tran));
        }

        /// <summary>
        /// Crea nuovo slot fornendo il Tipo Database e la connectionstring
        /// </summary>
        /// <param name="dbType">
        /// Tipo Database utilizzabile: MSSQLDataBase, MYSQLDataBase,
        /// FBDataBase, ACCESSDataBase, SQLITEDataBase
        /// </param>
        /// <param name="connectionString">
        /// La stringa di connessione da utilizzare
        /// </param>
        /// <returns></returns>
        public BusinessSlot(string dbType, string connectionString)
        {
            //Inizializza slot con istanza db creata
            this.InitSlot(DataBaseFactory.CreaDataBase(dbType, connectionString));
        }


        /// <summary>
        /// Inizializza dati vari slot
        /// </summary>
        private void InitSlot(IDataBase db)
        {

            //Imposta una nuova istanza di configurazione specifica per questo slot
            this.Conf = _StaticConf.Clone();

            //Avvia stopwatch
            this.mStopWatch = System.Diagnostics.Stopwatch.StartNew();

            //Imposta db primario in lista
            this.mDB = this.DbAdd(Constants.STR_DB_DEFAULT, db);

            //Imposta tracking
            this.liveTrackingActivation(this.Conf.LiveTrackingEnabled);

        }


        /// <summary>
        /// dato un oggetto prova ad inserirlo nelle varie cache previste
        /// </summary>
        /// <param name="obj"></param>
        internal void CacheSetAny(DataObjectBase obj)
        {
            this.cacheSetPipeline(obj);

            if (this.LiveTrackingEnabled)
                this.liveTrackingSet(obj);
        }


        /// <summary>
        /// Indica se possibile inserire/leggere un oggetto in cache
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        internal bool IsCacheable(ClassSchema sc)
        {
            //Verifica se caching abilitato in generale
            if (!this.Conf.CachingEnabled)
                return false;

            //Se cache globale OK
            if (sc.GlobalCache)
                return true;

            ////Sola lettura e vogliamo scrivere
            //if (sc.IsReadOnly)
            //    return true;

            ////Db in transazione
            //if (this.DbGet(sc).IsInTransaction)
            //    return false;

            //Default
            return false;
        }

        /// <summary>
        /// indica se oggetto inseribile in cache
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool IsCacheable(DataObjectBase obj)
        {
            //Default
            return this.IsCacheable(obj.mClassSchema);
        }

        /// <summary>
        /// Ottiene schema da cache in cascata
        /// </summary>
        /// <param name="uniqueKey"></param>
        /// <param name="sc"></param>
        /// <returns></returns>
        private DataSchema cacheGetPipeline(string uniqueKey, ClassSchema sc)
        {
            //Se non salvabile in cache ritorna null
            if (!this.IsCacheable(sc))
                return null;

            DataSchema oData = null;

            //1) Cerca in cache locale (Global o Slot)
            if (sc.GlobalCache)
            {
                //Cerca in cache
                oData = _GlobalCache.GetObject(uniqueKey);

                //se non nullo utilizza un clone ed esce
                if (oData != null)
                {
                    var newData = oData.Clone(false, true);
                    newData.ObjectSource = EObjectSource.GlobalCache;

                    //Restituisce un clone per sicurezza
                    return newData;
                }
            }

            //Non trovato
            return oData;
        }


        /// <summary>
        /// Se possibile salva oggetto in cache
        /// </summary>
        /// <param name="obj"></param>
        private void cacheSetPipeline(DataObjectBase obj)
        {
            //Se non possibile esegure cache esce
            if (!this.IsCacheable(obj))
                return;

            //1) Salva in cache locale
            if (obj.mClassSchema.GlobalCache)
            {
                lock (_GlobalCache)
                {
                    //Se qualcun altro lo ha creato prima esce
                    if (_GlobalCache.ContainsObject(obj.GetHashBaseString()))
                        return;

                    //Crea una versione clonata senza oggetti da salvare in cache
                    var oClonedData = obj.mDataSchema.Clone(false, true);

                    //Salva lo schema in cache per chiave
                    _GlobalCache.SetObject(obj.GetHashBaseString(), oClonedData);

                }
            }

        }

        #region LIVE TRACKING

        /// <summary>
        /// Attiva o disattiva tracking
        /// </summary>
        /// <param name="active"></param>
        private void liveTrackingActivation(bool active)
        {
            //Se gia' impostato esce
            if (this.Conf.LiveTrackingEnabled == active && this.mLiveTrackingStore != null)
                return;

            //Ok imposta
            this.Conf.LiveTrackingEnabled = active;

            if (active)
            {
                this.mLiveTrackingStore = new ReferenceManager<string, DataObjectBase>(1024);
            }
            else
            {
                if (this.mLiveTrackingStore != null)
                {
                    this.mLiveTrackingStore.Dispose();
                    this.mLiveTrackingStore = null;
                }
            }
        }


        /// <summary>
        /// Imposta oggetto nello store del tracking
        /// </summary>
        /// <param name="obj"></param>
        internal void liveTrackingSet(DataObjectBase obj)
        {
            //Salva tutte le chiavi con unico riferimento
            this.mLiveTrackingStore.Set(obj.GetHashBaseString(), obj);

        }

        /// <summary>
        /// Rimuove oggetto dallo store del tracking
        /// </summary>
        /// <param name="obj"></param>
        internal void liveTrackingRemove(DataObjectBase obj)
        {
            this.mLiveTrackingStore.Remove(obj.GetHashBaseString());
        }

        /// <summary>
        /// Ritorna oggetto dallo store del tracking
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal DataObjectBase liveTrackingGet(string key)
        {
            return this.mLiveTrackingStore.Get(key);
        }

        #endregion

        #endregion


        #region STATIC MEMBERS

        /// <summary>
        /// Costruttore statico che esegue configurazione base
        /// </summary>
        static BusinessSlot()
        {
            InitConfigure(new SlotConfig());
        }

        /// <summary>
        /// Consente di impostare la configurazione statica base dello slot.
        /// E' ammessa una sola chiamata per ciclo di vita dell'applicazione.
        /// </summary>
        /// <param name="conf"></param>
        public static void StaticConfigure(SlotConfig conf)
        {
            //Test configurazione null
            if (conf == null)
                throw new ArgumentException("Configurazione nulla");
            //E' consentita una unica configurazione statica
            var iCount = Interlocked.Increment(ref _StaticConfCount);
            if (iCount > 1)
                throw new BusinessSlotException("Non e' possibile configurare staticamente lo slot piu' di una volta per applicazione");

            //Riconfigura lo slot
            InitConfigure(conf);
        }

        /// <summary>
        /// Configura uno slot a partire da un'oggetto configurazione
        /// </summary>
        /// <param name="conf"></param>
        private static void InitConfigure(SlotConfig conf)
        {
            try
            {
                //Imposta configurazione default
                _StaticConf = conf;

                //Inizializza 
                BusinessSlot._GlobalCache = new CacheSimple<string, DataSchema>(conf.CacheGlobalSize);
                BusinessSlot._ListCache = new CacheTimed<string, DataTable>(conf.CacheGlobalSize / 2);
               
                //Se impostata directory di log
                if (string.IsNullOrEmpty(_StaticConf.LogBaseDirectory))
                {
                    //Utilizza cartella di default dell'assembly
                    Uri uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                    _StaticConf.LogBaseDirectory = System.IO.Path.GetDirectoryName(uri.LocalPath);
                }

                //Si assicura dell'esistenza della directory
                System.IO.Directory.CreateDirectory(_StaticConf.LogBaseDirectory);

                //Chiude shared logger
                if (_SharedLog != null)
                    _SharedLog.Dispose();
                //Apre shared logger
                _SharedLog = new FileStreamLogger(System.IO.Path.Combine(_StaticConf.LogBaseDirectory, string.Concat("SlotMainLog_", DateTime.Now.ToString("yyyy_MM_dd"), ".log")))
                {
                    WriteThreadId = true
                };

                //Chiude log database se aperto
                if (_DatabaseLog != null)
                    _DatabaseLog.Dispose();

                //Crea logger per DB
                if (_StaticConf.LogDatabaseActivity)
                {
                    //Imposta trace su file specifico
                    _DatabaseLog = new FileStreamLogger(System.IO.Path.Combine(_StaticConf.LogBaseDirectory, string.Concat("TraceSQL_", DateTime.Now.ToString("yyyy_MM_dd"), ".log")))
                    {
                        WriteThreadId = true
                    };
                }

            }
            catch (Exception ex)
            {
                throw new BusinessSlotException(SessionMessages.Initialization_Error, ex.Message);
            }
        }

        #endregion


        #region IComparable<BDSession> Membri di

        public int CompareTo(BusinessSlot other)
        {
            return this.mSlotId.CompareTo(other.mSlotId);
        }

        #endregion


        #region IEquatable<BusinessSlot> Membri di

        public override int GetHashCode()
        {
            return this.mSlotId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            BusinessSlot other = (obj as BusinessSlot);

            return this.Equals(other);
        }

        public bool Equals(BusinessSlot other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            return (string.Equals(this.mSlotId, other.mSlotId));
        }

        #endregion


        #region IDisposable Membri di

        /// <summary>
        /// Libera risorse associate
        /// </summary>
        public void Dispose()
        {
            try
            {
                //Disattiva gestione eventi
                this.EventManagerEnabled = false;

                //Disabilita Tracking
                this.liveTrackingActivation(false);

                //Termina tutte le istanze db definite
                foreach (var db in this.mDbList.Values)
                {
                    db.Dispose();
                }

                //Rimuove eventuali eventi rimasti attaccati
                if (this.OnLogDebugSent != null)
                {
                    foreach (var item in this.OnLogDebugSent.GetInvocationList())
                    {
                        this.OnLogDebugSent -= (LogDebugHandler)item;
                    }
                }

                if (this.OnDbConnectionRequired != null)
                {
                    foreach (var item in this.OnDbConnectionRequired.GetInvocationList())
                    {
                        this.OnDbConnectionRequired -= (DbConnectionRequestHandler)item;
                    }
                }

                if (this.OnUserInfoRequired != null)
                {
                    foreach (var item in this.OnUserInfoRequired.GetInvocationList())
                    {
                        this.OnUserInfoRequired -= (UserInfoRequestHandler)item;
                    }
                }


                _SharedLog.Dispose();
            }
            finally
            {
                //Imposta flag di fine sessione
                this.mTerminated = true;
            }

        }

        #endregion

    }

}
