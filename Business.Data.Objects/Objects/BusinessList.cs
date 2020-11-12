using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Objects.Base;
using Bdo.ObjFactory;
using Business.Data.Objects.Common.Exceptions;

namespace Bdo.Objects
{
    /// <summary>
    /// Classe astratta per la definizione dei metodi di
    /// business per una generica classe lista
    /// </summary>
    /// <typeparam name="TL"></typeparam>
    public abstract class BusinessList<TL, T, TB> : SlotAwareObject, IEnumerable<TB>, IList<TB>
        where TL : DataList<TL, T>
        where T : DataObject<T>
        where TB : BusinessObject<T>
    {

        #region PROPRIETA'

        private List<TB> mBizList;
        private TL mDataList;
        private ProxyEntryBiz mBizCreator;

        /// <summary>
        /// Oggetto dati associato
        /// </summary>
        public TL DataList
        {
            get { return this.mDataList; }
        }

        public TB this[int index]
        {
            get
            {
                //Costruttore del BusinessObject
                if (this.mBizList[index] == null)
                    this.mBizList[index] = (TB)ProxyAssemblyCache.Instance.CreateBizObj(this.mBizCreator, this.mDataList[index]);

                return this.mBizList[index];
            }
            set
            {
                if (value == null)
                    throw new ArgumentException("L'oggetto da impostare non puo' essere nullo");

                this.mBizList[index] = value;
                this.DataList[index] = value.DataObj;
            }
        }

        #endregion

        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="list"></param>
        protected BusinessList(TL list)
        {
            //Verifica che l'oggetto in input non sia nullo
            if (list == null)
                throw new ObjectException("{0} - La lista fornita in input risulta nulla.", this.GetType().Name);

            //Imposta oggetto
            this.mDataList = list;
            this.mBizList = new List<TB>(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                this.mBizList.Add(null);
            }

            this.SetSlot(list.GetSlot());
            this.mBizCreator = ProxyAssemblyCache.Instance.GetBizEntry(typeof(TB));
        }




        #region Interfaccia Ienumerable
        

        IEnumerator<TB> IEnumerable<TB>.GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        #endregion


        #region Interfaccia ILIST

        public int IndexOf(TB item)
        {
            return this.mDataList.IndexOf(item.DataObj);
        }

        public void Insert(int index, TB item)
        {
            this.mBizList.Insert(index, item);
            this.mDataList.Insert(index, item.DataObj);
        }

        public void RemoveAt(int index)
        {
            this.mBizList.RemoveAt(index);
            this.mDataList.RemoveAt(index);
        }


        public void Add(TB item)
        {
            this.mBizList.Add(item);
            this.mDataList.Add(item.DataObj);
        }

        /// <summary>
        /// Aggiunge l'oggetto o aggiorna l'istanza gia' presente con l'istanza fornita
        /// </summary>
        /// <param name="item"></param>
        public void AddOrUpdate(TB item)
        {
            var idx = this.IndexOf(item);

            if (idx == -1)
            {
                this.Add(item);
            }
            else
            {
                if (!object.ReferenceEquals(item, this.mBizList[idx]))
                {
                    this[idx] = item;
                }
            }
        }


        public void Clear()
        {
            this.mBizList.Clear();
            this.mDataList.Clear();
        }

        public bool Contains(TB item)
        {
            return (this.mDataList.IndexOf(item.DataObj) != -1);
        }

        public void CopyTo(TB[] array, int arrayIndex)
        {
            for (int i = 0; i < this.Count; i++)
            {
                array[arrayIndex++] = this[i];
            }
        }

        public int Count
        {
            get
            {
                return this.mDataList.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TB item)
        {
            this.mBizList.Remove(item);
            return this.mDataList.Remove(item.DataObj);
        }

        #endregion



        #region Interfaccia ILIST
        
        int IList<TB>.IndexOf(TB item)
        {
            return this.mDataList.IndexOf(item.DataObj);
        }

        void IList<TB>.Insert(int index, TB item)
        {
            this.Insert(index, item);
        }

        void IList<TB>.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        TB IList<TB>.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value;
            }
        }

        void ICollection<TB>.Add(TB item)
        {
            this.Add(item);
        }

        void ICollection<TB>.Clear()
        {
            this.Clear(); ;
        }

        bool ICollection<TB>.Contains(TB item)
        {
            return this.Contains(item);
        }

        void ICollection<TB>.CopyTo(TB[] array, int arrayIndex)
        {
            this.CopyTo(array, arrayIndex);
        }

        int ICollection<TB>.Count
        {
            get { return this.Count; ; }
        }

        bool ICollection<TB>.IsReadOnly
        {
            get { return this.IsReadOnly; }
        }

        bool ICollection<TB>.Remove(TB item)
        {
            return this.Remove(item);
        }

        #endregion


    }


}
