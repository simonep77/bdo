using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using Bdo.Objects.Base;

namespace Bdo.Objects
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
                throw new ObjectException("{0} - L'oggetto fornito in input risulta nullo.", this.GetType().Name);

            //Imposta oggetto
            this.mObject = obj;
            this.SetSlot(obj.GetSlot());
        }


        #region STATIC

  

        #endregion

    }
}
