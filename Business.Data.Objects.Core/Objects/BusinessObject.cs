using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Core.Base;

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


        #region STATIC

  

        #endregion

    }
}
