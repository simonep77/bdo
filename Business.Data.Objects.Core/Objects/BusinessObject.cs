using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Core.Base;
using System.Collections.Generic;

namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Classe astratta per la definizione dei metodi di
    /// business per una generica classe
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BusinessObject<T> : BusinessObjectBase where T : DataObject<T>
    {

        #region PROPRIETA'

        private T mObject;
        /// <summary>
        /// Oggetto dati associato
        /// </summary>
        public T DataObj
        {
            get { return this.mObject; }
        }


        #endregion

        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="obj"></param>
        public BusinessObject(T obj)
        {
            //Verifica che l'oggetto in input non sia nullo
            if (obj == null)
                throw new ObjectException($"{this.GetType().Name} - L'oggetto fornito in input risulta nullo.");

            //Imposta oggetto
            this.mObject = obj;
            this.SetSlot(obj.GetSlot());
        }


        #region LAZY LOAD MANAGEMENT


        private Dictionary<string, object> mDic = new Dictionary<string, object>();

        /// <summary>
        /// Funzione di caricamento oggetto lazy
        /// </summary>
        /// <returns></returns>
        protected delegate object LazyLoadFunc();

        /// <summary>
        /// Ritorna oggetto precedentemente caricato oppure lo carica tramite la funzione in input e lo memorizza per accessi successivi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        protected T GetLazy<T>(string propertyName, LazyLoadFunc fn)
        {
            object obj;

            if (!mDic.TryGetValue(propertyName, out obj))
            {
                obj = fn();
                mDic.Add(propertyName, obj);
            }

            return (T)obj;
        }


        #endregion

    }
}
