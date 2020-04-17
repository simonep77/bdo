using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Bdo.Common;
using Bdo.Database;
using Bdo.Schema;
using Bdo.Schema.Definition;
using Bdo.ObjFactory;
using Bdo.Utils;
using System.ComponentModel;

namespace Bdo.Objects.Base
{
    /// <summary>
    /// Classe base per definizione liste
    /// </summary>
    public abstract class DataListBase : SlotAwareObject
    {

        internal ClassSchema mObjSchema;
        internal InnerDataList mInnerList = new InnerDataList();
        internal protected OrderBy mOrderBy = new OrderBy();
        internal object mSyncRoot = new object();
        internal protected bool mIsSearch;
        internal protected bool mCacheResult;
        internal protected bool mLoadFullObjects = true;

        #region PROPERTY

        /// <summary>
        /// Indica se la lista e' paginata
        /// </summary>
        public bool IsPaged
        {
            get
            {
                return (this.Pager != null);
            }
        }

        /// <summary>
        /// Paginatore
        /// </summary>
        public DataPager Pager {get; set;}


        /// <summary>
        /// Numero elementi
        /// </summary>
        public int Count
        {
            get
            {
                return this.mInnerList.Count;
            }
        }


        #endregion


        #region EVENTS

        public event ListChangedEventHandler ListChanged;

        /// <summary>
        /// Lancia evento se definito
        /// </summary>
        /// <param name="eventtype"></param>
        /// <param name="index"></param>
        internal protected void fireListChanged(ListChangedType eventtype, int index)
        {
            if (this.ListChanged != null)
                this.ListChanged(this, new ListChangedEventArgs(eventtype, index));

        }


        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Riempie la lista interna da reader
        /// </summary>
        /// <param name="reader"></param>
        internal protected void fillListFromReader(DbDataReader reader)
        {
            int iFieldCount = (this.IsPaged) ? reader.FieldCount - 1 : reader.FieldCount;

            while (reader.Read())
            {
                //Imposta totale records da ultima colonna della query. Se non oertinente allora imposta -1
                if (this.IsPaged && this.Pager.TotRecords == 0)
                {
                    var oTotRecs = reader[reader.FieldCount - 1];
                    if (oTotRecs is Int32)
                        this.Pager.TotRecords = (Int32)oTotRecs;
                    else
                        this.Pager.TotRecords = -1;
                }

                InnerDataListItem oItem = new InnerDataListItem();

                oItem.PkValues = new object[this.mObjSchema.PrimaryKey.Properties.Count];

                for (int i = 0; i < this.mObjSchema.PrimaryKey.Properties.Count; i++)
                {
                    if (reader.IsDBNull(i))
                        oItem.PkValues[i] = null;
                    else
                        oItem.PkValues[i] = reader.GetValue(i);
                }

                //Se presenti valori oltre la pk li salva (se non prefetch)
                if (!this.mIsSearch && iFieldCount > this.mObjSchema.PrimaryKey.Properties.Count)
                {
                    oItem.ExtraData = new Dictionary<string, object>();
                    for (int i = this.mObjSchema.PrimaryKey.Properties.Count; i < iFieldCount; i++)
                    {
                        oItem.ExtraData.Add(reader.GetName(i), reader[i]);
                    }
                }

                //Se attivo LoadObjects allora prova a caricare il singolo oggetto dal reader e lo imposta nell'Item
                if (this.mLoadFullObjects && this.mIsSearch)
                {
                    //Cerca in LT
                    if(this.Slot.LiveTrackingEnabled)
                    {
                        oItem.PkHashCode = ObjectHelper.GetObjectHashString(this.Slot, this.mObjSchema, oItem.PkValues);
                        oItem.Object = this.Slot.liveTrackingGet(oItem.PkHashCode);
                    }

                    //Se non trovato
                    if (oItem.Object == null)
                    {
                        //Crea oggetto vuoto
                        oItem.Object = this.Slot.CreateObjectByType(this.mObjSchema.OriginalType);
                        //Carica dati
                        oItem.Object.FillObjectFromReader(reader, false);
                        //oItem.PkValues = oItem.Object.mClassSchema.PrimaryKey.GetValues(oItem.Object);
                        //Prova ad inserire nelle cache
                        this.Slot.CacheSetAny(oItem.Object);
                    }

                }

                //Aggiunge a lista
                this.mInnerList.Add(oItem);
            }
            //Se presente un resultset aggiuntivo allora assume che sia il numero di record
            if (reader.NextResult())
            {
                if (reader.Read())
                    this.Pager.TotRecords = reader.GetInt32(0);
            }
        }


        /// <summary>
        /// Resetta le liste interne
        /// </summary>
        internal protected void resetList()
        {
            //Azzera contatore record
            if (this.IsPaged)
                this.Pager.TotRecords = 0;

            //Azzera lista
            this.mInnerList.Clear();

            this.fireListChanged(ListChangedType.Reset, -1);
        }



        /// <summary>
        /// Ritorna item dalla lista (caricandolo se necessario)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="dalType"></param>
        /// <returns></returns>
        internal protected DataObjectBase getItem(int index)
        {
            var oInnerItem = this.mInnerList[index];

            if (oInnerItem.Object == null)
                oInnerItem.Object = this.Slot.LoadObjectInternalByKEY(ClassSchema.PRIMARY_KEY, this.mObjSchema.OriginalType, true, oInnerItem.PkValues);

            //Ritorna
            return oInnerItem.Object;

        }


        internal protected void setItem(int index, DataObjectBase value)
        {
            var oOldInnerList = this.mInnerList[index];

            //Se trattasi dello stesso oggetto non fa nulla
            if (object.ReferenceEquals(oOldInnerList.Object, value))
                return;

            //Imposta nuovo elemento per evitare che tutte le eventuali sottoliste ereditino la modifica (sara' giusto???)
            var oNewInnerItem = new InnerDataListItem()
            {
                Object = value,
                PkValues = this.mObjSchema.PrimaryKey.GetValues(value),
                PkHashCode = value.GetHashBaseString()
            };

            //Lo imposta
            this.mInnerList[index] = oNewInnerItem;

            this.fireListChanged(ListChangedType.ItemChanged, index);
        }

        /// <summary>
        /// Esegue la ricerca
        /// </summary>
        /// <returns></returns>
        internal protected DataListBase doSearch()
        {
            try
            {
                //Resetta liste
                this.resetList();

                //Appoggia sessione
                IDataBase db = this.Slot.DbGet(this.mObjSchema);

                //Appende ORDER BY a query
                db.SQL = string.Concat(db.SQL, this.mOrderBy.ToString());

                //Se presente cache esterna verifica se presente un precedente risultato
                string sQueryHash = null;
                DbDataReader rd = null;

                if (this.mCacheResult)
                {
                    //Crea hash della query
                    if (this.IsPaged)
                        sQueryHash = db.GetCurrentQueryHashString(this.Pager.Position, this.Pager.Offset);
                    else
                        sQueryHash = db.GetCurrentQueryHashString(0, 0);

                    //Cerca in cache, se trovata datatable allora crea reader associato
                    var dt = BusinessSlot._ListCache.GetObject(sQueryHash);
                    if (dt != null)
                    {
                        //Resetta il db per evitare di lasciare i parametri allocati
                        db.Reset();
                        //Crea datareader da datatable
                        rd = dt.CreateDataReader();
                    }
                }

                //Se non caricato da cache procede all'esecuzione della query
                if (rd == null)
                {
                    //Carica
                    if (this.IsPaged)
                        //Query paginata
                        rd = ((CommonDataBase)db).ExecReaderPaged(this.Pager.Position, this.Pager.Offset);
                    else
                        //Query standard
                        rd = db.ExecReader();

                    
                    //Qui salva in cache se necessario
                    if (this.mCacheResult)
                    {
                        //Crea datatable a partire dal reader
                        var dt = new System.Data.DataTable();
                        dt.Load(rd);
                        //Salva in cache
                        BusinessSlot._ListCache.SetObject(sQueryHash, dt);
                        //Ricrea il reader dalla tabella
                        rd = dt.CreateDataReader();
                    }
                }
                    
                //Carica dati
                using (rd)
                {
                    this.fillListFromReader(rd);
                }
                
            }
            finally
            {
                //Resetta comunque le variabili di esecuzione contesto
                this.mCacheResult = false;
                this.mLoadFullObjects = true;
                this.mIsSearch = false;
            }

            

            this.fireListChanged(ListChangedType.Reset, -1);

            //Ritorna se stesso per semplificare
            return this;
        }



        #region INDEX MANAGEMENT

        /// <summary>
        /// Trova un elemento fornendo valori PrimaryKey
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public int getIndexOfByPK(object[] values)
        {
            //Calcola hash dei parametri
            string uKey = ObjectHelper.GetObjectHashString(this.Slot, this.mObjSchema, values);

            //Crea indice (se non presente)
            for (int i = 0; i < this.Count; i++)
            {
                var item = this.mInnerList[i];

                //Ricalcola/imposta PKHash
                if (string.IsNullOrEmpty(item.PkHashCode))
                {
                    if (item.Object == null)
                        item.PkHashCode = ObjectHelper.GetObjectHashString(this.Slot, this.mObjSchema, item.PkValues);
                    else
                        item.PkHashCode = item.Object.GetHashBaseString();
                }
                   

                if (uKey == item.PkHashCode)
                    return i;
            }

            //Ritorna
            return -1;
        }


        #endregion


        #region QUERY MANAGEMENT

        /// <summary>
        /// Ricerca attraverso un filtro di colonna
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        internal protected DataListBase searchByColumn(IFilter filter)
        {
            //Imposta sql
            IDataBase db = this.Slot.DbGet(this.mObjSchema);
            //Inizia a preparare SQL
            StringBuilder sql = null;

            if (this.mLoadFullObjects)
                sql = new StringBuilder(string.Intern(this.mObjSchema.TableDef.SQL_Select_Item), this.mObjSchema.TableDef.SQL_Select_Item.Length + 300);
            else
                sql = new StringBuilder(string.Intern(this.mObjSchema.TableDef.SQL_Select_List), this.mObjSchema.TableDef.SQL_Select_List.Length + 300);

            sql.Append(this.Slot.DbPrefixGetTableName(this.mObjSchema.TableDef));
            //Se fornito filtro
            if (filter != null)
            {
                //Imposta SQL filtro
                sql.Append(@" WHERE ");
                filter.AppendFilterSql(db, sql, 0);

            }

            db.SQL = sql.ToString();

            //Imposta provenienza ricerca
            this.mIsSearch = true;

            //Esegue e Ritorna se stesso
            return this.doSearch();
        }


        /// <summary>
        /// Imposta il campo di sort e la direzione di sort
        /// Da utilizzare subito prima dell'esecuzione della ricerca.
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="versus"></param>
        /// <returns></returns>
        internal protected DataListBase orderBy(string sortField, OrderVersus versus)
        {
            //Imposta order by solo se definito il campo di sort
            this.mOrderBy.Add(sortField, versus);

            return this;
        }


        /// <summary>
        /// Imposta Order By passando direttamente tutto l'oggetto orderby
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        internal protected DataListBase orderBy(OrderBy order)
        {
            this.mOrderBy = order;

            return this;
        }

        #endregion


        #endregion

        #region PUBLIC METHODS


        internal override void SetSlot(BusinessSlot slot)
        {
            base.SetSlot(slot);
            this.mLoadFullObjects = this.Slot.Conf.LoadFullObjects;
        }

        /// <summary>
        /// Ritorna Xml per la gestione della paginazione
        /// </summary>
        /// <returns></returns>
        public string GetPagingXml()
        { 
            DataPager pager = this.Pager ?? new DataPager();

            using (XmlWrite xw = new XmlWrite())
            {
                //Formattazione standard
                xw.WriteStartElement("Paginazione");
                try
                {
                    xw.WriteElementString("Offset", pager.Offset.ToString());
                    xw.WriteElementString("Pagina", pager.Page.ToString());
                    xw.WriteElementString("TotRecord", pager.TotRecords.ToString());
                    xw.WriteElementString("TotPagine", pager.TotPages.ToString());

                }
                finally
                {
                    xw.WriteEndElement();
                }

                return xw.ToString();
            }

        }


        /// <summary>
        /// Imposta uno slot sulla lista corrente
        /// </summary>
        public new void SwitchToSlot(BusinessSlot slot)
        {
            base.SwitchToSlot(slot);

            //La imposta su tutti i sotto oggetti se istanziati
            for (int i = 0; i < this.mInnerList.Count; i++)
            {
                if (this.mInnerList[i].Object != null)
                    this.mInnerList[i].Object.SwitchToSlot(slot);
            }
        }



        /// <summary>
        /// Ritorna elenco oggetti in formato DTO con profondita' 0
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, object>> ToDTO()
        {
            return this.ToDTO(0);
        }

        /// <summary>
        /// Ritorna elenco oggetti in formato DTO con profondita' specificata
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, object>> ToDTO(int depth)
        {
            List<Dictionary<string, object>> oRet = new List<Dictionary<string, object>>(this.Count);

            for (int i = 0; i < this.Count; i++)
            {
                oRet.Add(this.getItem(i).ToDTO(depth));
            }

            return oRet;
        }


        /// <summary>
        /// Ritorna elenco oggetti in formato JSON
        /// </summary>
        /// <returns></returns>
        public string ToJSON()
        {
            return Utils.JSONWriter.ToJson(this);
        }


        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(this.GetType().Name, " - Count: ", this.mInnerList.Count.ToString());
        }


        /// <summary>
        /// Scrive xml lista
        /// </summary>
        /// <param name="depht"></param>
        /// <returns></returns>
        public string ToXml(int depht)
        {
            using (XmlWrite xw = new XmlWrite())
            {
                for (int i = 0; i < this.mInnerList.Count; i++)
                {
                    xw.WriteStartElement(this.mObjSchema.ClassName);
                    try
                    {
                        xw.WriteRaw(this.getItem(i).ToXml(depht));
                    }
                    finally
                    {
                        xw.WriteEndElement();
                    }
                }

                return xw.ToString();
            }
        }


        /// <summary>
        /// Rappresentazione XML
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            return this.ToXml(0);
        }


        /// <summary>
        /// Esegue conteggio in base a filtro per proprieta'
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public int CountByFilter(IFilter filter)
        {

            int iLen = this.Count;

            if (filter == null)
            {
                return iLen;
            }

            int iRet = 0;

            for (int i = 0; i < iLen; i++)
            {
                if (filter.PropertyTest(this.getItem(i)))
                {
                    iRet++;
                }
            }

            return iRet;
        }


        /// <summary>
        /// Ritorna un valore accessorio ripreso dalla query di caricamento lista.
        /// Utile per ricerche che ritornano dati aggiuntivi rispetto agli oggetti stessi
        /// </summary>
        /// <param name="index"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public object GetExtraValue(int index, string columnName)
        {
            if (index < 0 || index >= this.Count)
                throw new ObjectException(Resources.ObjectMessages.Base_IndexOutOfBounds, this.GetType().Name, index);

            InnerDataListItem item = this.mInnerList[index];

            if (item.ExtraData == null)
                throw new ObjectException("ExtraValue non presenti", this.GetType().Name, index);

            object oRet = null;

            if (!item.ExtraData.TryGetValue(columnName, out oRet))
                throw new ObjectException(Resources.ObjectMessages.List_UnknownColumn, this.GetType().Name, columnName);

            return oRet;
        }


        #region OPERATIONS



        /// <summary>
        /// Somma tutti i valori delle proprieta' degli elementi della lista
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public decimal Sum(string propertyName)
        {
            return Sum(propertyName, null);
        }


        /// <summary>
        /// Somma applicando un filtro
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public decimal Sum(string propertyName, IFilter filter)
        {
            decimal dRet = decimal.Zero;

            int iLen = this.Count;

            if (iLen > 0)
            {
                Property oProp = this.mObjSchema.Properties.GetPropertyByName(propertyName);

                if (!TypeHelper.IsNumericType(oProp.Type))
                    throw new ObjectException(Resources.ObjectMessages.List_CannotAggregateNonNumeric, this.GetType().Name, oProp.Name);

                for (int i = 0; i < iLen; i++)
                {
                    if (filter != null && !filter.PropertyTest(this.getItem(i)))
                        continue;

                    dRet += Convert.ToDecimal(oProp.GetValue(this.getItem(i)));
                }
            }

            return dRet;
        }




        /// <summary>
        /// Ritorna la media dei valori delle proprieta' della lista con filtro
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public decimal Avg(string propertyName, IFilter filter)
        {
            if (this.Count == 0)
                return decimal.Zero;

            return (this.Sum(propertyName, filter) / this.Count);
        }


        /// <summary>
        /// Ritorna la media dei valori delle proprieta' della lista
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public decimal Avg(string propertyName)
        {
            return this.Avg(propertyName, null);
        }


        /// <summary>
        /// Svuota la lista
        /// </summary>
        public void Clear()
        {
            this.resetList();
        }

        #endregion


        /// <summary>
        /// Data una lista ritorna una sottolista paginata
        /// </summary>
        /// <param name="page"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal DataListBase toPagedList(int page, int offset)
        {

            var newList = (DataListBase)ProxyAssemblyCache.Instance.CreateDaoNoSchemaObj(this.GetType().BaseType);
            newList.Pager = new DataPager();
            newList.Pager.Page = page;
            newList.Pager.Offset = offset;
            newList.Pager.TotRecords = this.Count;

            int idxBegin = newList.Pager.Position;
            int idxEnd = Math.Min(idxBegin + offset, this.Count);

            for (int i = idxBegin; i < idxEnd; i++)
            {
                newList.mInnerList.Add(this.mInnerList[i]);
            }

            return newList;
        }

        #endregion

        #region ABSTRACT

        #endregion


    }
}
