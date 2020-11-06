using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using Bdo.Properties;
using Bdo.Database;
using Bdo.Common;
using Bdo.Schema.Definition;
using Bdo.Schema.Usage;
using Bdo.Utils;


namespace Bdo.Objects.Base
{
    /// <summary>
    /// Classe base per la gestione degli oggetti persistenti
    /// </summary>
    public abstract class DataObjectBase : SlotAwareObject, IDisposable, INotifyPropertyChanged
    {

        //Variabili interne
        internal ClassSchema mClassSchema;
        internal DataSchema mDataSchema;
        internal Int64 mObjectRefId;

        #region PUBLIC PROPERTIES

        /// <summary>
        /// Riferimento univoco assegnato all'istanza dell'oggetto
        /// </summary>
        public Int64 ObjectRefId
        {
            get
            {
                return this.mObjectRefId;
            }
            internal set
            {
                this.mObjectRefId = value;
            }
        }

        /// <summary>
        /// Indica la provenienza dell'oggetto
        /// </summary>
        public EObjectSource ObjectSource
        {
            get
            {
                return this.mDataSchema.ObjectSource;
            }
        }

        /// <summary>
        /// Indica lo stato interno dell'oggetto
        /// </summary>
        public EObjectState ObjectState
        {
            get
            {
                return this.mDataSchema.ObjectState;
            }
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Esegue l'evento di property change del databindings
        /// </summary>
        /// <param name="propIn"></param>
        internal void firePropertyChanged(Property propIn)
        { 
            //Notifica per DataBindings
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propIn.Name));        
        }

        /// <summary>
        ///  Funzione per impostare il valore di proprietà
        /// </summary>
        /// <param name="propertyIndex"></param>
        /// <param name="value"></param>
        public void SetProperty(int propertyIndex, object value)
        {
            //Ottiene property associata
            Property oProp = this.GetPropertyDefinition(propertyIndex);

            //Imposta valore
            oProp.SetValue(this, value);
        }

        /// <summary>
        /// Funzione interna per ottenere il valore di proprietà
        /// </summary>
        /// <param name="propertyIndex"></param>
        /// <returns></returns>
        public object GetProperty(int propertyIndex)
        {
            //Ottiene property associata
            Property oProp = this.GetPropertyDefinition(propertyIndex);

            return oProp.GetValue(this);
        }



        /// <summary>
        /// Ritorna la rappresentazione in stringa dell'oggetto
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(this.mClassSchema.ClassName,
                @" (",
                ObjectHelper.ObjectEnumerableToString(this.mClassSchema.PrimaryKey.GetValues(this)),
                @")");
        }


        /// <summary>
        /// Ritorna una rappresentazione XML dell'oggetto senza navigazione nei sotto oggetti
        /// </summary>
        /// <returns></returns>
        public virtual string ToXml()
        {
            return this.ToXml(0);
        }


        /// <summary>
        /// Ritorna rappresentazione Xml dell oggetto fornendo il livello di profondita' (navigazione sotto-oggetti)
        /// 0 indica non navigare nei sotto-oggetti
        /// Prestare attenzione ad eventuali riferimenti circolari
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public virtual string ToXml(int depth)
        {
            Property oProp;

            using (XmlWrite xw = new XmlWrite())
            {
                for (int k = 0; k < this.mClassSchema.Properties.Count; k++)
                {
                    //Verifica proprietà una per una in base al tipo
                    oProp = this.mClassSchema.Properties[k];

                    //Verifica se esclusa da Xml
                    if (oProp.ExcludeXml)
                        continue;

                    oProp.WriteXml(xw, this, depth);
                   
                }

                return xw.ToString();
            }

        }

        /// <summary>
        /// Ritorna rappresentazione in forma di coppie property=valore con
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ToDTO()
        {
            return this.ToDTO(0);
        }

        /// <summary>
        /// Ritorna rappresentazione in forma di coppie property=valore con specifica di profondita'
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ToDTO(int depth)
        {
            Dictionary<string, object> oRet = new Dictionary<string, object>(this.mClassSchema.Properties.Count + 10);

            for (int k = 0; k < this.mClassSchema.Properties.Count; k++)
            {
                //Verifica proprietà una per una in base al tipo
                this.mClassSchema.Properties[k].WriteDTO(oRet, this, depth);

            }

            return oRet;
        }


        /// <summary>
        /// Ritorna rappresentazione  JSON dell'oggetto
        /// </summary>
        /// <returns></returns>
        public virtual string ToJSON()
        {
            return JSONWriter.ToJson(this);
        }


        /// <summary>
        /// Ritorna Xml contenente i valori delle proprieta' in input
        /// </summary>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public string ToXml(params string[] propertyNames)
        {
            //Controllo array
            if (propertyNames == null || propertyNames.Length == 0)
                throw new ObjectException("{0} - L'elenco di proprieta' fornite e' vuoto");

            using (XmlWrite xw = new XmlWrite())
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    //Verifica proprietà una per una in base al tipo
                    this.mClassSchema.Properties.GetPropertyByName(propertyNames[i]).WriteXml(xw, this, 0);
                }

                return xw.ToString();
            }
            
        }


        /// <summary>
        /// Verifica se property nulla (Interna)
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        internal bool IsDefaultValue(Property prop)
        {
            return object.Equals(prop.GetValue(this), prop.DefaultValue);
        }


        /// <summary>
        /// Verifica se una proprietà ha valore considerato null
        /// </summary>
        /// <param name="propName"></param>
        public bool IsNull(string propName)
        {
            var oProp = this.mClassSchema.Properties.GetPropertyByName(propName);
            return oProp.IsNull(oProp.GetValue(this));
        }



        /// <summary>
        /// Indica se la proprieta' specificata e' stata modificata 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsChanged(string propertyName)
        {
            return this.mDataSchema.GetFlagsAll(this.mClassSchema.Properties.GetPropertyByName(propertyName).PropertyIndex, DataFlags.Changed);
        }


        /// <summary>
        /// Ritorna elenco di proprieta' modificate
        /// Attenzione! Dopo il salvataggio le proprieta' risultano non modificate!
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurrentChanges()
        {
            List<string> oListRet = new List<string>();

            for (int i = 0; i < this.mClassSchema.Properties.Count; i++)
            {
                if (this.mDataSchema.GetFlagsAll(this.mClassSchema.Properties[i].PropertyIndex, DataFlags.Changed))
                    oListRet.Add(this.mClassSchema.Properties[i].Name);
            }

            return oListRet;
        }

        /// <summary>
        /// Da implementare per eseguire la validazione
        /// </summary>
        public virtual void Validate() { }

        /// <summary>
        /// Salva l'oggetto nel database
        /// </summary>
        internal ESaveResult DoSave()
        {
            //Gia' eliminato
            if (this.mDataSchema.ObjectState == EObjectState.Deleted)
                throw new ObjectException(Resources.ObjectMessages.Deleted_NoAction, this.GetType().Name);

            //Esegue validazione di tutte le property se non disabilitata da config
            this.validateProperties((this.mDataSchema.ObjectState == EObjectState.New));

            //Esegue validazione utente (applicativa)
            this.Validate();

            //Verifica abilitazione
            if (this.mClassSchema.IsReadOnly)
                throw new ObjectException(Resources.ObjectMessages.OperationFail_SaveOrDelete, this.GetType().Name);

            //Esegue
            var eSaveRes = ESaveResult.Unset;

            switch (this.mDataSchema.ObjectState)
            {
                case EObjectState.New:
                    //esegue
                    eSaveRes = this.performDbInsert();

                    //Se effettuato salvataggio deve ricalcolare hash della PK
                    if (eSaveRes == ESaveResult.SaveDone)
                        this.mDataSchema.PkHash = ObjectHelper.GetObjectHashString(this);

                    break;
                case EObjectState.Loaded:
                    //esegue
                    eSaveRes =  this.performDbUpdate();
                    break;
            }


            return eSaveRes;
        }


        /// <summary>
        /// Elimina l'oggetto dal database
        /// </summary>
        internal void DoDelete()
        {
            //No Nuovo
            if (this.mDataSchema.ObjectState == EObjectState.New)
                throw new ObjectException(Resources.ObjectMessages.New_CannotDelete, this.GetType().Name);
            //No Eliminato
            else if (this.mDataSchema.ObjectState == EObjectState.Deleted)
                throw new ObjectException(Resources.ObjectMessages.Deleted_NoAction, this.GetType().Name);

            //Verifica abilitazione
            if (this.mClassSchema.IsReadOnly)
                throw new ObjectException(Resources.ObjectMessages.OperationFail_SaveOrDelete, this.GetType().Name);

            //Elimina da db
            this.performDbDelete();

        }


        /// <summary>
        /// Ritorna una stringa per calcolo di hash
        /// </summary>
        /// <returns></returns>
        public string GetHashBaseString()
        {
            if (string.IsNullOrEmpty(this.mDataSchema.PkHash))
                this.mDataSchema.PkHash = ObjectHelper.GetObjectHashString(this);

            return this.mDataSchema.PkHash;
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Esegue caricamento singola property da DB
        /// </summary>
        /// <param name="prop"></param>
        internal void LoadPropertyFromDB(PropertySimple prop)
        {
            //Imposta db
            IDataBase db = this.Slot.DbGet(this.mClassSchema);

            try
            {
                //Imposta SQL (preleva quello standard dalla PK e sostituisce il contenuto della select con il solo nome campo)
                db.SQL = string.Concat(@"SELECT ", prop.Column.Name, " FROM " , this.Slot.DbPrefixGetTableName(this.mClassSchema.TableDef), this.mClassSchema.PrimaryKey.SQL_Where_Clause);

                //Imposta PK
                var oKeyValues = this.mClassSchema.PrimaryKey.FillKeyQueryWhereParams(db, this);

                //Esegue query su datareader
                using (DbDataReader dr = db.ExecReader())
                {
                    //Controlla presenza risultato e si dispone sul record
                    if (!dr.Read())
                        throw new ObjectException(Resources.ObjectMessages.Base_RecordKeyNotFound, this.GetType().Name, this.mClassSchema.PrimaryKey.Name, oKeyValues);

                    //OK Imposta oggetto
                    prop.SetValueFromReader(this, dr);
                }
            }
            finally
            {
                db.Reset();
            }



        }

        /// <summary>
        /// Esegue la query impostata ed carica dati su oggetto
        /// </summary>
        /// <param name="db"></param>
        private void ExecQueryAndLoadObj(IDataBase db)
        {
            //Imposta dati dopo query
            using (DbDataReader dr = db.ExecReader())
            {
                //Controlla presenza risultato e si dispone sul record
                if (!dr.Read())
                    return;
                else
                {
                    //OK Imposta oggetto
                    this.FillObjectFromReader(dr, false);

                }
            }
        }

        /// <summary>
        /// Carica oggetto a partire da un filtro custom
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        internal void LoadByFilter(IFilter filter, OrderBy order)
        {
            IDataBase db = this.Slot.DbGet(this.mClassSchema);

            //SQL DIRETTO
            var sb = new StringBuilder(this.mClassSchema.TableDef.SQL_Select_Item);
            sb.Append(this.Slot.DbPrefixGetTableName(this.mClassSchema.TableDef));
            sb.Append(@" WHERE ");

            //Imposta parametri WHERE
            filter.AppendFilterSql(db, sb, 0);

            //Se valorizzato include l'order by
            if (order != null)
                sb.Append(order.ToString());

            //imposta query
            db.SQL = sb.ToString();

            //Imposta dati dopo query
            this.ExecQueryAndLoadObj(db);
        }

        /// <summary>
        /// Carica oggetto da chiave 
        /// </summary>
        /// <param name="keyIn"></param>
        /// <param name="values"></param>
        internal void LoadBySchemaKey(Key keyIn, params object[] values)
        {

            IDataBase db = this.Slot.DbGet(this.mClassSchema);
            
            //SQL DIRETTO
            db.SQL = string.Concat(string.Intern(this.mClassSchema.TableDef.SQL_Select_Item),  this.Slot.DbPrefixGetTableName(this.mClassSchema.TableDef), 
                string.Intern(keyIn.SQL_Where_Clause));

            //Imposta parametri WHERE
            keyIn.FillKeyQueryWhereParams(db, values);

            //Imposta dati dopo query
            this.ExecQueryAndLoadObj(db);
        }


        /// <summary>
        /// Carica le proprietà standard dell'oggetto
        /// </summary>
        /// <param name="dr"></param>
        internal void FillObjectFromReader(IDataReader dr, bool includeAll)
        {
            Property oProp;

            //Loop tra proprietà
            for (int k = 0; k < this.mClassSchema.Properties.Count; k++)
            {
                oProp = this.mClassSchema.Properties[k];

                //Salta le proprietà non incluse nel caricamento
                if (oProp.IsSqlSelectExcluded && !includeAll)
                    continue;

                //Richiama il caricamento della singola proprietà
                oProp.SetValueFromReader(this, dr);
            }


            //Imposta stato oggetto caricato
            this.mDataSchema.ObjectState = EObjectState.Loaded;
        }

   
        /// <summary>
        /// Carica definizione di proprietà con controllo
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private Property GetPropertyDefinition(int propertyIndex)
        {
            //Ritorna proprietà
            return this.mClassSchema.Properties[propertyIndex];
        }


        /// <summary>
        /// Base properties validation (based on class definition)
        /// </summary>
        private void validateProperties(bool isInsert)
        {
            bool bIsValid = true;
            Property oProp;
            object oValue;
            bool bChanged;

            if (!isInsert)
            {
                //Verifica modifica alla PK in modifica
                foreach (var oPropKey in this.mClassSchema.PrimaryKey.Properties)
                {
                    //Check changed flag
                    bChanged = this.mDataSchema.GetFlagsAll(oPropKey.PropertyIndex, DataFlags.Changed);

                    //La primary key non può essere modificata in aggiornamento
                    if (this.mDataSchema.GetFlagsAll(oPropKey.PropertyIndex, DataFlags.Changed))
                        throw new ObjectException(Resources.ObjectMessages.Validate_PrimaryKeyModified, this.mClassSchema.ClassName, oPropKey.Name);

                }
            }

            //In generale
            for (int iPropIndex = 0; iPropIndex < this.mClassSchema.Properties.Count; iPropIndex++)
            {
                //Get property definition
                oProp = this.mClassSchema.Properties[iPropIndex];
                try
                {
                    //Check changed flag
                    bChanged = this.mDataSchema.GetFlagsAll(oProp.PropertyIndex, DataFlags.Changed);
                    

                    //skip object not new and not changed
                    if (this.mDataSchema.ObjectState == EObjectState.Loaded && !bChanged)
                        continue;

                    //Verifica se necessario skip di alcune casistiche
                    bool flagToCheck = oProp.IsReadonly || oProp.IsAutomatic;

                    if (isInsert)
                        flagToCheck |= oProp.ExcludeInsert;
                    else
                        flagToCheck |= oProp.ExcludeUpdate;

                    //Skip automatic properties
                    if (flagToCheck)
                        continue;

                    //HACK: Get value (this forces loading of dataobject properties and loadonaccess)
                    oValue = oProp.GetValue(this);

                    //If property is not nullable (db) then cannot be null

                    if (!oProp.AcceptNull && oProp.IsNull(oValue))
                        throw new ObjectException(Resources.ObjectMessages.New_NullNotAllowed, this.mClassSchema.ClassName, oProp.Name);

                    //Esegue la validazione prevista
                    oProp.PerformValidation(oValue);

                }
                catch (Exception ex)
                {
                    //Validation failed flag
                    bIsValid = false;

                    //Message list enabled
                    if (this.Slot.Conf.ObjectValidationUseMessageList)
                        //Add to list
                        this.Slot.MessageList.Add(new Message(1, ex.Message, ESeverity.Error, oProp.Name));
                    else
                        //Non si utilizza la listamessaggi, ci si ferma comunque al primo errore lanciando eccezione
                        //di validazione
                        throw new ObjectValidationException(ex.Message);

                }

            }

            //if validation KO throws exception
            if (!bIsValid)
                throw new ObjectValidationException(Resources.ObjectMessages.ValidateProperties_Failure, this.mClassSchema.ClassName);

        }

 
        /// <summary>
        /// Esegue la insert sul DB
        /// </summary>
        private ESaveResult performDbInsert()
        {
            //Appoggio
            IDataBase db = this.Slot.DbGet(this.mClassSchema);
            List<DbParameter> oDbParams = new List<DbParameter>(this.mClassSchema.Properties.Count);
            Property oProp;
            var sTableFullName = this.Slot.DbPrefixGetTableName(this.mClassSchema.TableDef);

            try
            {
                //Inserisce campi
                for (int iPropIndex = 0; iPropIndex < this.mClassSchema.Properties.Count; iPropIndex++)
                {
                    oProp = this.mClassSchema.Properties[iPropIndex];

                    //In generale e' necessario impostare tutti i campi come non modificati
                    this.mDataSchema.SetFlags(oProp.PropertyIndex, DataFlags.Changed, false);

                    //Esclusione esplicita
                    if (oProp.ExcludeInsert || oProp.IsAutomatic)
                        continue;

                    //Imposta campi
                    object oValue = oProp.GetValueForDb(this);
               
                    //Imposta valore
                    oDbParams.Add(db.CreateParameter(oProp.Column.ParamName, oValue, oProp.Column.DbType));

                    //Aggiorna
                    this.mDataSchema.SetFlags(oProp.PropertyIndex, DataFlags.Loaded, true);
                }

                //Imposta SQL base
                var sbSql = new StringBuilder(@"INSERT INTO ", this.mClassSchema.TableDef.SQL_Insert.Length + 300);

                sbSql.Append(sTableFullName);
                sbSql.Append(@" ");
                sbSql.Append(string.Intern(this.mClassSchema.TableDef.SQL_Insert));

                //In base al tipo di esecuzione
                if (this.mClassSchema.MustReload)
                {
                    sbSql.Append(@";");
                    sbSql.Append(this.mClassSchema.TableDef.SQL_Select_Reload);
                    sbSql.Append(sTableFullName);
                    //Se autoinc imposta solo il nome della funzione
                    if (this.mClassSchema.AutoIncPk)
                    {
                        sbSql.Append(@" WHERE ");
                        sbSql.Append(this.mClassSchema.PrimaryKey.Properties[0].Column.Name);
                        sbSql.Append(@"=");
                        sbSql.Append(db.LastAutoIdFunction);
                    }
                    else 
                    {
                        sbSql.Append(this.mClassSchema.PrimaryKey.SQL_Where_Clause);

                        for (int i = 0; i < this.mClassSchema.PrimaryKey.Properties.Count; i++)
                        {
                            oDbParams.Add(db.CreateParameter(this.mClassSchema.PrimaryKey.Properties[i].Column.GetKeyParamName(), 
                                this.mClassSchema.PrimaryKey.Properties[i].GetValueForDb(this), 
                                this.mClassSchema.PrimaryKey.Properties[i].Type));
                        }
                    }

                    //Risolve
                    db.SQL = sbSql.ToString();
                    db.AddParameters(oDbParams);

                    using (DbDataReader dr = db.ExecReader())
                    {
                        //Controllo
                        if (!dr.Read())
                            throw new ObjectException(Resources.ObjectMessages.New_NoRecord, this.GetType().Name);

                        //Aggiorna proprieta'
                        for (int i = 0; i < this.mClassSchema.AutoProperties.Count; i++)
                        {
                            this.mClassSchema.AutoProperties[i].SetValueFromReader(this, dr);
                        }
                    }
                }
                else
                {
                    //Imposta db ed esegue
                    db.SQL = sbSql.ToString();
                    db.AddParameters(oDbParams);

                    //Controlla esito per verificare situazioni anomale
                    if (db.ExecQuery() == 0)
                        //Nessun Inserimento
                        throw new ObjectException(Resources.ObjectMessages.New_NoRecord, this.GetType().Name);
                }

                //Imposta stato caricato comunque
                this.mDataSchema.ObjectState = EObjectState.Loaded;

            }
            finally
            {
                //Resetta db per evitare eventuali parametri impostati
                //e mai utilizzati
                db.Reset();
            }

            return ESaveResult.SaveDone;
        }


        /// <summary>
        /// Esegue update sul DB
        /// </summary>
        private ESaveResult performDbUpdate()
        {
            //Appoggio
            IDataBase db = this.Slot.DbGet(this.mClassSchema);
            List<DbParameter> oDbParams = new List<DbParameter>(this.mClassSchema.Properties.Count);

            int iNumChangedProps = 0;
            Property oProp;

            StringBuilder sbSQL = new StringBuilder(@"UPDATE ", 500);
            sbSQL.Append(this.Slot.DbPrefixGetTableName(this.mClassSchema.TableDef));
            sbSQL.Append(@" SET ");

            try
            {
                //Inserisce campi
                for (int iPropIndex = 0; iPropIndex < this.mClassSchema.Properties.Count; iPropIndex++)
                {
                    //Imposta proprieta'
                    oProp = this.mClassSchema.Properties[iPropIndex];

                    //Se da escludere passa a successiva
                    if (oProp.ExcludeUpdate)
                        continue;

                    //Imposta controllo concorrenza
                    if (oProp.IsAutomatic)
                    {
                        //Scrive Campo
                        sbSQL.Append(oProp.Column.Name);
                        sbSQL.Append(@"=CURRENT_TIMESTAMP, ");
                        continue;
                    }

                    //PROPRIETA' 
                    if (!this.mDataSchema.GetFlagsAll(oProp.PropertyIndex, DataFlags.Changed))
                        continue;

                    var oValue = oProp.GetValueForDb(this);

                    //Scrive Campo
                    sbSQL.Append(oProp.Column.Name);
                    sbSQL.Append(@"=");
                    sbSQL.Append(oProp.Column.ParamName);
                    sbSQL.Append(@", ");

                    //Imposta valore
                    oDbParams.Add(db.CreateParameter(oProp.Column.ParamName, oValue, oProp.Column.DbType));

                    //Aggiorna
                    iNumChangedProps++;
                    //TODO: Attenzione impostando qui il flag, in caso di errore rimane l'oggetto sporco!!!!
                    this.mDataSchema.SetFlags(oProp.PropertyIndex, DataFlags.Changed, false);
                }

                //NON C'È NULLA DA MODIFICARE ESCE
                if (iNumChangedProps == 0)
                    return ESaveResult.UnChanged;

                //Rimuove ,
                sbSQL.Remove(sbSQL.Length - 2, 2);

                //Aggiunge parte PK + reload (se impostata)
                sbSQL.Append(this.mClassSchema.PrimaryKey.SQL_Where_Clause);
                sbSQL.Append(@"; ");
                if (!string.IsNullOrEmpty(this.mClassSchema.TableDef.SQL_Select_Reload))
                {
                    sbSQL.Append(this.mClassSchema.TableDef.SQL_Select_Reload);
                    sbSQL.Append(this.Slot.DbPrefixGetTableName(this.mClassSchema.TableDef));
                    sbSQL.Append(this.mClassSchema.PrimaryKey.SQL_Where_Clause);

                }

                //Genera la WHERE
                var oKeyValues = this.mClassSchema.PrimaryKey.FillKeyQueryWhereParams(db, this);

                //Esegue
                db.SQL = sbSQL.ToString();
                db.AddParameters(oDbParams);

                //A seconda
                //Se non presenti prop da aggiornare
                if (!this.mClassSchema.MustReload)
                {
                    //Esegue
                    int ret = db.ExecQuery();

                    //Controlla esito per verificare situazioni anomale
                    if (ret == 0)
                    {
                        //Nessuna Modifica
                        throw new ObjectException(Resources.ObjectMessages.Edit_NoRecord, this.GetType().Name, this.mClassSchema.PrimaryKey.Name, ObjectHelper.ObjectEnumerableToString(oKeyValues));
                    }
                    else if (ret > 1)
                    {
                        //Modifiche Multiple
                        throw new ObjectException(Resources.ObjectMessages.Edit_MultipleRecords, this.GetType().Name, this.mClassSchema.PrimaryKey.Name, ObjectHelper.ObjectEnumerableToString(oKeyValues));
                    }
                }
                else
                {//Presenti
                    using (DbDataReader dr = db.ExecReader())
                    {
                        //Controllo
                        if (!dr.Read())
                        {
                            //Nessuna Modifica
                            throw new ObjectException(Resources.ObjectMessages.Edit_NoRecord, this.GetType().Name, this.mClassSchema.PrimaryKey.Name, ObjectHelper.ObjectEnumerableToString(oKeyValues));
                        }

                        //Aggiorna proprieta'
                        for (int i = 0; i < this.mClassSchema.AutoProperties.Count; i++)
                        {
                            this.mClassSchema.AutoProperties[i].SetValueFromReader(this, dr);
                        }
                    }
                }

            }
            finally
            {
                //Resetta db per evitare eventuali parametri impostati
                //e mai utilizzati
                db.Reset();
            }

            return ESaveResult.SaveDone;
        }

        /// <summary>
        /// Perform database delete
        /// </summary>
        private void performDbDelete()
        {
            //Appoggio
            IDataBase db = this.Slot.DbGet(this.mClassSchema);

            try
            {
                //Imposta QUERY
                db.SQL = string.Concat(@"DELETE FROM ", this.Slot.DbPrefixGetTableName(this.mClassSchema.TableDef), this.mClassSchema.PrimaryKey.SQL_Where_Clause);
                var oKeyValues = this.mClassSchema.PrimaryKey.FillKeyQueryWhereParams(db, this);

                //Esegue
                int ret = db.ExecQuery();

                //Controlla esito per verificare situazioni anomale
                if (ret == 0)
                {
                    //Nessuna Cancellazione
                    throw new ObjectException(Resources.ObjectMessages.Deleted_NoRecord, this.GetType().Name, this.mClassSchema.PrimaryKey.Name, ObjectHelper.ObjectEnumerableToString(oKeyValues));
                }
                else if (ret > 1)
                {
                    //Cancellazioni Multiple
                    throw new ObjectException(Resources.ObjectMessages.Deleted_MultipleRecords, this.GetType().Name, this.mClassSchema.PrimaryKey.Name, ObjectHelper.ObjectEnumerableToString(oKeyValues));
                }

                //Imposta stato eliminato
                this.mDataSchema.ObjectState = EObjectState.Deleted;
            }
            finally
            {
                //Resetta db per evitare eventuali parametri impostati
                //e mai utilizzati
                db.Reset();
            }

        }


        #endregion

        #region STATIC METHODS


        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            //Sgancia sessione
            this.SetSlot(null);

            //Libera risorse
            //this.mSchema.Dispose();
            this.mDataSchema = null;
        }

        #endregion

        #region IEquatable<BDBaseObject> Membri di


        public override int GetHashCode()
        {
            return this.GetHashBaseString().GetHashCode();
        }


        /// <summary>
        /// Confronto tra due oggetti (viene confrontata Primary Key)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            DataObjectBase oOther = other as DataObjectBase;

            //Se null non può essere uguale
            if (oOther == null)
                return false;

            //Se sono la stessa istanza torna true
            if (object.ReferenceEquals(this, oOther))
                return true;


            //Schema differenti
            if (!object.Equals(this.mClassSchema.InternalID, oOther.mClassSchema.InternalID))
                return false;

            Key oPriKey = this.mClassSchema.PrimaryKey;

            //Verifica uguaglianza Primary Key
            for (int i = 0; i < this.mClassSchema.PrimaryKey.Properties.Count; i++)
            {
                if (!object.Equals(this.mClassSchema.PrimaryKey.Properties[i].GetValue(this), this.mClassSchema.PrimaryKey.Properties[i].GetValue(oOther)))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region INotifyPropertyChanged Membri di

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }
}
