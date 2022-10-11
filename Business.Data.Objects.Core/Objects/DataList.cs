using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Core.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Core.Objects;
using Business.Data.Objects.Core.ObjFactory;
using Business.Data.Objects.Core.Schema.Definition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Business.Data.Objects.Core
{

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
                return (T)this.getItem(index);
            }
            set
            {
                //Imposta
                this.setItem(index, value);
            }
        }

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
        /// La ricerca effettuata dopo questa istruzione verifica la presenza di una eventuale risultato precedente
        /// </summary>
        /// <returns></returns>
        public TL CacheResult()
        {
            this.mCacheResult = true;
            //this.mCacheResult = this.Slot.IsCacheable(this.mObjSchema);

            return (TL)this;
        }

        /// <summary>
        /// Indica che gli oggetti vanno precaricati attraverso il risultato della query di lista
        /// Attenzione! Al momento integrato direttamente solo sulle SearcBDO. Le query custom non possono eseguire il LoadFullObjects
        /// in quanto andrebbero manipolate senza garanzia del risultato
        /// </summary>
        /// <returns></returns>
        [Obsolete("Ormai il comportamento e' sempre questo")]
        public TL LoadFullObjects()
        {
            return (TL)this;
        }

        /// <summary>
        /// Istruisce la query successiva ad includere gli oggetti eliminati logicamente
        /// </summary>
        /// <returns></returns>
        public TL IncludeDeleted()
        {
            base.mIncludeDeleted = true;

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
        [Obsolete("Utilizzare SearchByLinq")]
        public TL SearchByColumn(string columnName, EOperator op, object value)
        {
            return (TL)this.searchByColumn(new Filter(columnName, op, value));
        }


        /// <summary>
        /// Ricerca attraverso un filtro di colonna
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Obsolete("Utilizzare SearchByLinq")]
        public TL SearchByColumn(IFilter filter)
        {
            return (TL)this.searchByColumn(filter);
        }


        /// <summary>
        /// Ricerca attraverso un'espressione linq.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TL SearchByLinq(Expression<Func<T, bool>> predicate)
        {
            var t = new LinqQueryTranslator<T>(this.Slot);

            return (TL)this.searchByCustomWhere(t.Translate(predicate));
        }

        /// <summary>
        /// Esegue ordinamento tramite selezione del campo. Modalita' ASC
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TL OrderByLinq<TKey>(Expression<Func<T, TKey>> predicate)
        {
            var t = new LinqQueryTranslator<T>(this.Slot);

            var f = t.TranslateKey(predicate);

            this.orderBy(f, OrderVersus.Asc);

            return (TL)this;
        }

        /// <summary>
        /// Esegue ordinamento tramite selezione del campo. Modalita' DESC
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TL OrderByLinqDesc<TKey>(Expression<Func<T, TKey>> predicate)
        {
            var t = new LinqQueryTranslator<T>(this.Slot);

            var f = t.TranslateKey(predicate);

            this.orderBy(f, OrderVersus.Desc);

            return (TL)this;
        }


        /// <summary>
        /// Data una lista ritorna una sottolista paginata
        /// </summary>
        /// <param name="page"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public TL ToPagedList(int page, int offset)
        {
            TL newList = this.Slot.CreateList<TL>(page, offset);
            newList.Pager.TotRecords = this.Count;

            int idxBegin = newList.Pager.Position;
            int idxEnd = Math.Min(idxBegin + offset, this.Count);

            for (int i = idxBegin; i < idxEnd; i++)
            {
                newList.mInnerList.Add(this.mInnerList[i]);
            }

            return newList;
        }


        /// <summary>
        /// Ritorna una lista di BusinessObjects a partire da questa lista
        /// </summary>
        /// <typeparam name="TB"></typeparam>
        /// <returns></returns>
        public List<TB> ToBizObjectList<TB>()
            where TB : BusinessObject<T>
        {
            return this.Slot.ToBizObjectList<TB, T>(this, null);
        }

        /// <summary>
        /// Ritorna una lista di BusinessObjects a partire da questa lista
        /// con la possibilità di eseguire un'azione specifica su ciascun oggetto creato
        /// </summary>
        /// <typeparam name="TB"></typeparam>
        /// <param name="act"></param>
        /// <returns></returns>
        public List<TB> ToBizObjectList<TB>(Action<TB> act)
    where TB : BusinessObject<T>
        {
            return this.Slot.ToBizObjectList<TB, T>(this, act);
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


        public bool IsReadOnly => false;


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

            int iFound = this.IndexOf(item);
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

            return this.getIndexOfByPK(item.mClassSchema.PrimaryKey.GetValues(item));
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
            this.mInnerList.Insert(index, new InnerDataListItem()
            {
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

        public bool AllowEdit => false;

        public bool AllowNew => false;

        public bool AllowRemove => true;

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Find(PropertyDescriptor property, object key)
        {
            var prop = this.mObjSchema.Properties.GetPropertyByName(property.Name);

            for (int i = 0; i < this.mInnerList.Count; i++)
            {
                if (this[i].GetProperty(prop.PropertyIndex).Equals(key))
                    return i;
            }

            return -1;
        }

        public bool IsSorted => false;
        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException("Metodo non supportato");
        }

        public void RemoveSort()
        {
            throw new NotImplementedException("Metodo non supportato");
        }

        public ListSortDirection SortDirection => ListSortDirection.Ascending;

        public PropertyDescriptor SortProperty => null;

        public bool SupportsChangeNotification => true;

        public bool SupportsSearching => false;

        public bool SupportsSorting => false;


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

        public bool IsSynchronized => false;

        public object SyncRoot => this.mSyncRoot;


        #endregion

    }
}
