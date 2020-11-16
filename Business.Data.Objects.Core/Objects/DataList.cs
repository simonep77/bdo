using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Core.ObjFactory;
using Business.Data.Objects.Core.Schema.Definition;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Delegato per definizione routine di test oggetto
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg"></param>
    /// <returns></returns>
    public delegate bool ObjectTest<T>(T arg);
    

    /// <summary>
    /// Classe astratta da utilizzare per gestione Liste
    /// </summary>
    /// <typeparam name="TL"></typeparam>
    /// <typeparam name="T"></typeparam>
    public abstract class DataList<TL, T> : DataListBase, IEnumerable<T>, IList<T>, IBindingList, IDisposable
        where T : DataObject<T>
        where TL : DataList<TL, T>
    {




        #region PROPERTY



        /// <summary>
        /// Ritorna oggetto alla posizione specificata
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                //Ritorna
                return  (T)this.getItem(index);
            }
            set
            {
                //Imposta
                this.setItem(index, value);
            }
        }

        #endregion

        #region PUBLIC DELEGATES

        /// <summary>
        /// Delegato per la gestione dell' Xml del singolo oggetto della lista
        /// </summary>
        /// <param name="value"></param>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        public delegate void XmlFunction(T value, XmlWrite writer, params object[] args);

        #endregion

        #region PUBLIC

        /// <summary>
        /// Costruttore
        /// </summary>
        public DataList()
        {
            //Crea tabella di default con schema
            this.mObjSchema = ProxyAssemblyCache.Instance.GetClassSchema(typeof(T));
        }

        /// <summary>
        /// Cerca per chiave primaria tra gli elementi della lista.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public T FindByPK(params object[] values)
        {
            int iFound = this.IndexOfByPK(values);

            //Non Trovato
            if (iFound < 0)
            {
                return null;
            }

            //Trovato
            return this[iFound];
        }


        /// <summary>
        /// La ricerca effettuata dopo questa istruzione verifica la presenza di una eventuale risultato precedente
        /// </summary>
        /// <returns></returns>
        public TL CacheResult()
        {
            this.mCacheResult = this.Slot.IsCacheable(this.mObjSchema);

            return (TL)this;
        }

        /// <summary>
        /// Indica che gli oggetti vanno precaricati attraverso il risultato della query di lista
        /// Attenzione! Al momento integrato direttamente solo sulle SearcBDO. Le query custom non possono eseguire il LoadFullObjects
        /// in quanto andrebbero manipolate senza garanzia del risultato
        /// </summary>
        /// <returns></returns>
        public TL LoadFullObjects()
        {
            this.mLoadFullObjects = true;

            return (TL)this;
        }



        /// <summary>
        /// Resetta campi orderBy
        /// </summary>
        public TL OrderByReset()
        {
            this.mOrderBy.Clear();

            return (TL)this;
        }


        /// <summary>
        /// Imposta il campo di sort e la direzione di sort
        /// Da utilizzare subito prima dell'esecuzione della ricerca.
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="versus"></param>
        /// <returns></returns>
        public TL OrderBy(string sortField, OrderVersus versus)
        {
            return (TL)this.orderBy(sortField, versus);
        }

        /// <summary>
        /// Order By Field ascending
        /// </summary>
        /// <param name="sortField"></param>
        /// <returns></returns>
        public TL OrderBy(string sortField)
        {
            return (TL)this.orderBy(sortField, OrderVersus.Asc);
        }

        /// <summary>
        /// Imposta Order By passando direttamente tutto l'oggetto orderby
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public TL OrderBy(OrderBy order)
        {
            return (TL)this.orderBy(order);
        }

        /// <summary>
        /// Esegue ricerca semplice di tutti gli oggetti
        /// </summary>
        /// <returns></returns>
        public TL SearchAllObjects()
        {
            return (TL)this.searchByColumn(null);
        }

        /// <summary>
        /// Esegue ricerca oggetti a partire da un valore di colonna applicando l'operatore impostato
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="op">Se [IsNull, IsNotNull] il valore non viene considerato</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TL SearchByColumn(string columnName, EOperator op, object value)
        {
            return (TL)this.searchByColumn(new Filter(columnName, op, value));
        }


        /// <summary>
        /// Ricerca attraverso un filtro di colonna
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TL SearchByColumn(IFilter filter)
        {
            return (TL)this.searchByColumn(filter);
        }


        /// <summary>
        /// Ritorna array con oggetti caricati
        /// Vengono copiati solo i riferimenti
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            //Istanzio tutti gli oggetti
            int iLen = this.Count;
            T[] oItems = new T[iLen];

            for (int i = 0; i < iLen; i++)
            {
                oItems[i] = this[i];
            }

            return oItems;
        }


        /// <summary>
        /// Ritorna primo elemento della lista 
        /// oppure null se vuota
        /// </summary>
        /// <returns></returns>
        public T GetFirst() {
            if (this.Count == 0)
                return default(T);

            return this[0];
        }


        /// <summary>
        /// Ritorna ultimo elemento della lista 
        /// oppure null se vuota
        /// </summary>
        /// <returns></returns>
        public T GetLast()
        {
            if (this.Count == 0)
                return default(T);

            return this[this.Count - 1];
        }





        /// <summary>
        /// Ritorna l'oggetto che ha il valore massimo della proprieta'
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public T Max(string propertyName)
        {
            return this.getMinMax(propertyName, null, false);
        }


        /// <summary>
        /// Ritorna l'oggetto che ha il valore massimo della proprieta' applicando un ulteriore filtro
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T Max(string propertyName, IFilter filter)
        {
            return this.getMinMax(propertyName, filter, false);
        }

        /// <summary>
        /// Ritorna oggetto con valore minimo della proprieta' fornita
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public T Min(string propertyName)
        {
            return this.getMinMax(propertyName, null, true);
        }


        /// <summary>
        ///  Ritorna oggetto con valore minimo della proprieta' fornita applicando ulteriore filtro
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T Min(string propertyName, IFilter filter)
        {
            return this.getMinMax(propertyName, filter, true);
        }


        /// <summary>
        /// Metodo privato unico per calcolo min e max
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="filter"></param>
        /// <param name="isMin"></param>
        /// <returns></returns>
        private T getMinMax(string propertyName, IFilter filter, bool isMin)
        {
            //Se vuota esce
            if (this.Count <= 0)
                return null;

            //Se prop non comparabile
            Property oProp = this.mObjSchema.Properties.GetPropertyByName(propertyName);

            if (!typeof(IComparable).IsAssignableFrom(oProp.Type))
                return null;

            //Ok, calcola
            T oRet = null;
            IComparable oTemp = null;

            for (int i = 0; i < this.Count; i++)
            {
                //Verifica filtro
                if (filter != null && !filter.PropertyTest(this[i]))
                    continue;

                IComparable oTemp2 = oProp.GetValue(this[i]) as IComparable;
                //Preimposta comparazione con elemento precedente
                var bCompare = (oTemp == null);
                
                //Necessario procedere a comparazione
                if (!bCompare)
                    bCompare = isMin ? (oTemp2.CompareTo(oTemp) <= 0) : (oTemp2.CompareTo(oTemp) >= 0);

                //Comparazione verificata
                if (bCompare)
                {
                    oRet = this[i];
                    oTemp = oTemp2;
                }
            }

            return oRet;
        }




        /// <summary>
        /// Ritorna Xml con dati oggetti. E' possibile specificare un delegato per poter manipolare
        /// l'xml di ogni oggetto con altri dati. Utilizzando rewriteAll viene soppresso l'output Xml standard 
        /// dell'oggetto
        /// </summary>
        /// <param name="function">
        /// Delegato ad una funzione (e.s. AggiornaXmlUtente(Utente ut, Xmlwrite xw)) per la manipolazione dell'xml
        /// </param>
        /// <param name="rewriteAll">
        /// Impostato a true disabilita l'output xml standard dell'oggetto
        /// </param>
        /// <param name="args">
        /// Dati esterni da inviare alla funzione di scrittura xml
        /// </param>
        /// <returns></returns>
        public string ToXml(XmlFunction function, bool rewriteAll, params object[] args)
        {
            DataObjectBase o;
            using (XmlWrite xw = new XmlWrite())
            {
                for (int i = 0; i < this.Count; i++)
                {
                    o = this[i];

                    xw.WriteStartElement(o.mClassSchema.ClassName);
                    try
                    {
                        if (!rewriteAll)
                            xw.WriteRaw(o.ToXml());

                        //Se fornita funzione allora la richiama
                        if (function != null)
                        {
                            function((T)o, xw, args);
                        }
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
        /// Ritorna nuova lista come unione di elementi con la lista in input.
        /// Oggetti con la medesima PK vengono riportati una sola volta.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public TL Union(TL other)
        {
            TL oListRet = this.Slot.CreateList<TL>();

            //Lista originale
            oListRet.mInnerList.AddRange(this.mInnerList);

            //Lista input
            if (other != null)
            {
                for (int i = 0; i < other.Count; i++)
                {
                    //Aggiunge in lista se non gia' presente
                    if (oListRet.getIndexOfByPK(other.mInnerList[i].PkValues) == -1)
                    {
                        oListRet.Add(other[i]);
                    }
                }
            }

            return oListRet;
        }


        /// <summary>
        /// Ritorna un clone della lista corrente
        /// </summary>
        /// <returns></returns>
        public TL Clone()
        {
            TL oListRet = this.Slot.CreateList<TL>();

            for (int i = 0; i < this.Count; i++)
            {
                InnerDataListItem item = new InnerDataListItem();
                item.PkValues = this.mInnerList[i].PkValues;
                item.PkHashCode = this.mInnerList[i].PkHashCode;
                item.ExtraData = this.mInnerList[i].ExtraData;

                oListRet.mInnerList.Add(item);
            }

            return oListRet;
        }


        /// <summary>
        /// Ritorna nuova lista come unione di tutti gli elementi con la lista in input.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public TL UnionAll(TL other)
        {
            TL oListRet = this.Slot.CreateList<TL>();

            //Merge delle sole tabelle interne
            oListRet.mInnerList.AddRange(this.mInnerList);
            
            if (other != null)
                oListRet.mInnerList.AddRange(other.mInnerList);

            return oListRet;
        }

        /// <summary>
        /// Ritorna tutti gli elementi presenti nella lista che non sono presenti nella lista in input.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public TL Diff(TL other)
        {
            TL oListRet = this.Slot.CreateList<TL>();

            //Lista originale
            for (int i = 0; i < this.Count; i++)
            {
                //Aggiunge in lista se non gia' presente (confronto con la sola PK su datatable)
                if (other == null || other.getIndexOfByPK(this.mInnerList[i].PkValues) == -1)
                {
                    //Crea nuova row da tab return e copia i valori di quella vecchia
                    oListRet.mInnerList.Add(this.mInnerList[i]);
                }
            }

            return oListRet;
        }


        /// <summary>
        /// Ritorna dizionario di valori raggruppati con associata la lista di elementi
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public GroupByResult<TL> GroupByProperty(string propertyName)
        {
            GroupByResult<TL> ret = new GroupByResult<TL>();

            Dictionary<object, TL> dicOutput = new Dictionary<object, TL>();
            Property oProp = this.mObjSchema.Properties.GetPropertyByName(propertyName);
            object oTemp;
            TL oTmplist = null;

            //Lista originale
            for (int i = 0; i < this.Count; i++)
            {
                oTemp = oProp.GetValue(this[i]);

                //Se non esiste
                if (!ret.TryGetValue(oTemp, out oTmplist))
                {
                    oTmplist = this.Slot.CreateList<TL>();
                    ret.Add(oTemp, oTmplist);
                }
                oTmplist.mInnerList.Add(this.mInnerList[i]);
            }

            //Ritorna la lista
            return ret;
        }




        /// <summary>
        /// Data una lista ritorna una sottolista paginata
        /// </summary>
        /// <param name="page"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public TL ToPagedList(int page, int offset)
        {
            TL newList = this.Slot.CreatePagedList<TL>(page, offset);
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

        #region PROTETTI


        protected TL DoSearch()
        {
            //Ritorna se stesso per semplificare
            return (TL)base.doSearch();
        }


        #endregion

        #region PRIVATI





        #endregion

        #region IEnumerable Membri di

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.mInnerList.Count; i++)
            {
                yield return this[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        #endregion

        #region "STATIC"

        /// <summary>
        /// Crea una lista non paginata
        /// </summary>
        /// <param name="slotIn"></param>
        /// <returns></returns>
        //public static TL Create(BusinessSlot slotIn)
        //{
        //    return slotIn.CreateList<TL>();
        //}


        /// <summary>
        ///  Crea una lista paginata
        /// </summary>
        /// <param name="slotIn"></param>
        /// <param name="page"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        //public static TL CreatePaged(BusinessSlot slotIn, int pageIn, int offsetIn)
        //{
        //    return slotIn.CreatePagedList<TL>(pageIn, offsetIn);
        //}

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            this.resetList();
        }

        #endregion

        #region ICollection<T> Membri di

        /// <summary>
        /// Aggiunge un set di elementi alla lista
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }


        /// <summary>
        /// Aggiunge un elemento alla lista
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            this.Insert(this.Count, item);
        }


        /// <summary>
        /// Aggiunge un elemento alla lista se non gia' presente
        /// </summary>
        /// <param name="item"></param>
        /// <returns> true se ha effettuato l'aggiunta, false se l'aggiornamento </returns>
        public bool AddOrUpdate(T item)
        {
            var idx = this.IndexOf(item);

            if (idx == -1)
                this.Add(item);
            else
                this[idx] = item;

            return (idx == -1);
        }





        /// <summary>
        /// Verifica se la lista contiene l'elemento fornito (by PK)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return (this.IndexOf(item) >= 0);
        }

        /// <summary>
        /// Copia gli elementi su array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            int arrLen = array.Length;
            int newIndex = 0;

            for (int i = arrayIndex; i < arrLen; i++)
            {
                array[newIndex] = this[i];
                newIndex++;
            }
        }


        public bool IsReadOnly
        {
            get { return false; }
        }


        /// <summary>
        /// rimuove un set di elementi alla lista
        /// </summary>
        /// <param name="items"></param>
        public void RemoveRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                this.Remove(item);
            }
        }

        /// <summary>
        /// Rimuove oggetto
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            if (item == null)
                return false;              

            int iFound = this.IndexOfByPK(item.mClassSchema.PrimaryKey.GetValues(item));
            //Non trovato
            if (iFound < 0)
                return false;

            //Trovato
            this.RemoveAt(iFound);
            return true;
        }


        /// <summary>
        /// Ritorna un valore accessorio ripreso dalla query di caricamento lista.
        /// Utile per ricerche che ritornano dati aggiuntivi rispetto agli oggetti stessi
        /// </summary>
        /// <param name="item"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public object GetExtraValue(T item, string columnName)
        {

            return this.GetExtraValue(this.IndexOf(item), columnName);
        }

        #endregion

        #region IList<T> Membri di

        /// <summary>
        /// Ricerca Indice Oggetto
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            if (item == null)
                return -1;

            return this.IndexOfByPK(item.mClassSchema.PrimaryKey.GetValues(item));
        }


        /// <summary>
        /// Ritorna il primo indice di oggetto per PK
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public int IndexOfByPK(params object[] values)
        {
            if (values == null || values.Length == 0)
                return -1;

            return this.getIndexOfByPK(values);
        }



        /// <summary>
        /// Inserisce elemento nella posizione indicata
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            //Prevnt NULLs
            if (item == null)
                throw new ArgumentException("Non e' possibile inserire nella lista un oggetto NULL!");

            //Setta item
            this.mInnerList.Insert(index, new InnerDataListItem() { 
                Object = item, 
                PkHashCode = item.GetHashBaseString(),
                PkValues = this.mObjSchema.PrimaryKey.GetValues(item)
            });

            this.fireListChanged(ListChangedType.ItemAdded, index);


        }

        /// <summary>
        /// Rimuove elemento alla posizione indicata
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            //Verifica indice
            if (index < 0 || index >= this.Count)
                throw new ObjectException(ObjectMessages.Base_IndexOutOfBounds, this.GetType().Name, index);

            //Elimina da tabella
            this.mInnerList.RemoveAt(index);

            //Invia notifica
            this.fireListChanged(ListChangedType.ItemDeleted, index);
        }

        #endregion

        #region IBindingList Membri di

        public void AddIndex(PropertyDescriptor property)
        {
            throw new Exception("La lista non consente la manipolazione del numero di elementi.");
        }

        public object AddNew()
        {
            throw new ObjectException("{0} - La lista non consente l'aggiunta di oggetti non salvati.", this.GetType().Name);
        }

        public bool AllowEdit
        {
            get { return false; }
        }

        public bool AllowNew
        {
            get { return false; }
        }

        public bool AllowRemove
        {
            get { return true; }
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Find(PropertyDescriptor property, object key)
        {
            T obj = this.FindFirstByPropertyFilter(new Filter(property.Name, EOperator.Equal, key));
            return this.IndexOf(obj);
        }

        public bool IsSorted
        {
            get { return false; }
        }


        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException("Metodo non supportato");
        }

        public void RemoveSort()
        {
            throw new NotImplementedException("Metodo non supportato");
        }

        public ListSortDirection SortDirection
        {
            get { return ListSortDirection.Ascending; }
        }

        public PropertyDescriptor SortProperty
        {
            get { return null; }
        }

        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        public bool SupportsSearching
        {
            get { return false; }
        }

        public bool SupportsSorting
        {
            get { return false; }
        }

  
        /// <summary>
        /// Ritorna lista ordinata
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public TL SortByProperty(string propertyName, bool ascending)
        {
            int iLen = this.Count;
            int iMultiplier = ascending ? 1 : -1;

            Property oProp = this.mObjSchema.Properties.GetPropertyByName(propertyName);

            TL oListRet = this.Slot.CreateList<TL>();

            for (int i = 0; i < iLen; i++)
            {
                int iInsertPos = 0;

                for (int k = 0; k < oListRet.Count; k++)
			    {
                    //Esegue comparazione
                    int iComp = ((IComparable)oProp.GetValue(this[i])).CompareTo(oProp.GetValue(oListRet[k])) * iMultiplier;
                    if (iComp > 0)
                    {
                        iInsertPos++;
                    }
                    else 
                    {
                        break;
                    }
			    }
                //Inserisce in nuova lista
                oListRet.mInnerList.Insert(iInsertPos, this.mInnerList[i]);
            }

            //Ritorna Lista
            return oListRet;
        }

      
        /// <summary>
        /// Ritorna una lista di oggetti che rispondono true al delegato passato in input
        /// </summary>
        /// <param name="testFunc"></param>
        /// <returns></returns>
        public TL FindAllByDelegate(ObjectTest<T> testFunc)
        {
            //Verifica delegato
            if (testFunc == null)
                throw new ObjectException(ObjectMessages.List_NullDelegate, typeof(T).Name);

            TL oListRet = this.Slot.CreateList<TL>();

            int iLen = this.Count;

            for (int i = 0; i < iLen; i++)
            {
                if (testFunc(this[i]))
                {
                    oListRet.Add(this.mInnerList[i]);
                }
            }

            //Ritorna Lista
            return oListRet;
        }

        /// <summary>
        /// Ricerca primo oggetto che ha la proprietà fornita con il valore specificato
        /// </summary>
        /// <param name="propertyName">Nome della proprietà per cui si vuole cercare (case sensitive)</param>
        /// <param name="value">valore da testare</param>
        /// <returns></returns>
        public T FindFirstByPropertyFilter(IFilter filter)
        {
            T o = null;
            int iLen = this.Count;

            if (iLen > 0)
            {
                for (int i = 0; i < iLen; i++)
                {
                    if (filter.PropertyTest(this[i]))
                    {
                        o = this[i];
                        break;
                    }
                }
            }

            //Ritorna Lista
            return o;
        }


        /// <summary>
        /// Ricerca ultimo oggetto che ha la proprietà fornita con il valore specificato
        /// </summary>
        /// <param name="propertyName">Nome della proprietà per cui si vuole cercare (case sensitive)</param>
        /// <param name="value">valore da testare</param>
        /// <returns></returns>
        public T FindLastByPropertyFilter(IFilter filter)
        {
            T o = null;
            int iLen = this.Count;

            if (iLen > 0)
            {
                for (int i = iLen-1; i > -1; i--)
                {
                    if (filter.PropertyTest(this[i]))
                    {
                        o = this[i];
                        break;
                    }
                }
            }

            //Ritorna Lista
            return o;
        }


        /// <summary>
        /// Ricerca tutti gli oggetti che hanno la proprietà fornita con il valore che rientra nel filtro
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TL FindAllByPropertyFilter(IFilter filter)
        {
            TL oListRet = this.Slot.CreateList<TL>();

            int iLen = this.Count;

            for (int i = 0; i < iLen; i++)
            {
                if (filter.PropertyTest(this[i]))
                {
                    oListRet.mInnerList.Add(this.mInnerList[i]);
                }
            }

            //Ritorna Lista
            return oListRet;
        }


        /// <summary>
        /// Esegue aggiornamento di più proprietà contemporaneamente
        /// </summary>
        /// <param name="propertyValues"></param>
        public void SetPropertyMassive(IDictionary<string, object> propertyValues)
        {
            //Crea la struttura di property da aggiornare
            IDictionary<int, KeyValuePair<Property, object>> oInnerDic = new Dictionary<int, KeyValuePair<Property, object>>(propertyValues.Count);
            foreach (var pair in propertyValues)
            {
                    Property oProp = this.mObjSchema.Properties.GetPropertyByName(pair.Key);
                
                    //Se la proprieta' e' readonly ritorna errore
                    if (oProp.IsReadonly)
                        throw new ObjectException("La proprieta' {0} e' in sola lettura", oProp.Name);
 
                    KeyValuePair<Property, object> oNewpair = new KeyValuePair<Property, object>(oProp, pair.Value);

                    //Imposta
                    oInnerDic.Add(oProp.PropertyIndex, oNewpair);
            }

            //Esegue aggiornamento
            for (int i = 0; i < this.Count; i++)
            {
                foreach (var pair in oInnerDic)
                {
                    //Imposta
                    pair.Value.Key.SetValue(this[i], pair.Value.Value);
                }
            }
        }


        /// <summary>
        /// Imposta una proprieta' in maniera massiva su tutti gli oggetti
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetPropertyMassive(string propertyName, object value) 
        {
            Property oProp = this.mObjSchema.Properties.GetPropertyByName(propertyName);

            //Se la proprieta' e' readonly ritorna errore
            if (oProp.IsReadonly)
                throw new ObjectException("La proprieta' {0} e' in sola lettura", oProp.Name);

            for (int i = 0; i < this.Count; i++)
            {
                oProp.SetValue(this[i], value);
            }
        }

        /// <summary>
        /// Imposta massivamente le proprietà di oggetti che corrispondono al filtro impostato.
        /// RItorna il numero di oggetti modificati
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="filter"></param>
        public int SetPropertyMassiveByFilter(string propertyName, object value, IFilter filter)
        {
            if (filter == null)
                throw new ArgumentException("Il filtro di ricerca non puo' essere nullo");

            Property oProp = this.mObjSchema.Properties.GetPropertyByName(propertyName);
            int iLen = this.Count;
            int iRet = 0;

            for (int i = 0; i < iLen; i++)
            {
                if (filter.PropertyTest(this[i]))
                {
                    oProp.SetValue(this[i], value);
                    iRet++;
                }
            }

            return iRet;
        }


        #endregion

        #region IList Membri di

        object System.Collections.IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (value as T);
            }
        }

        public int Add(object value)
        {
            this.Add(value as T);
            return this.Count;
        }

        public bool Contains(object value)
        {
            return this.Contains(value as T);
        }

        public int IndexOf(object value)
        {
            return this.IndexOf(value as T);
        }

        public void Insert(int index, object value)
        {
            this.Insert(index, value as T);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            this.Remove(value as T);
        }


        #endregion

        #region ICollection Membri di

        public void CopyTo(Array array, int index)
        {
            this.CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this.mSyncRoot; }
        }

        #endregion

    }
}
