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


        #region Base Methods

        /// <summary>
        /// Routine overridable per eseguire codice prima del salvataggio
        /// </summary>
        protected virtual void saveExecBefore()
        { }

        /// <summary>
        /// Routine overridable per eseguire codice dopo il salvataggio se andato a buon fine
        /// </summary>
        protected virtual void saveExecAfter()
        { }

        
        /// <summary>
        /// Esegue salvataggio del dataobject sottostante
        /// </summary>
        public void Save()
        {
            //Esegue before
            this.saveExecBefore();

            //Salva
            this.Slot.SaveObject(this.DataObj);

            //Esegue After
            this.saveExecAfter();
        }

        /// <summary>
        /// Routine overridable per eseguire codice prima della cancellazione
        /// </summary>
        protected virtual void deleteExecBefore()
        { }

        /// <summary>
        /// Routine overridable per eseguire codice dopo la cancellazione se andata a buon fine
        /// </summary>
        protected virtual void deleteExecAfter()
        { }

        /// <summary>
        /// Esegue delete del dataobject sottostante
        /// </summary>
        public void Delete()
        {
            //Esegue before
            this.deleteExecBefore();

            //Salva
            this.Slot.DeleteObject(this.DataObj);

            //Esegue After
            this.deleteExecAfter();
        }


        #endregion


        #region LAZY LOAD MANAGEMENT


        private Dictionary<string, object> mLazyDic = new Dictionary<string, object>();

        /// <summary>
        /// Funzione di caricamento oggetto lazy tipizzato
        /// </summary>
        /// <returns></returns>
        protected delegate T1 LazyLoadFunc<T1>();

        /// <summary>
        /// Ritorna oggetto precedentemente caricato oppure lo carica tramite la funzione in input e lo memorizza per accessi successivi.
        /// Utilizzare LazyGet, in futuro verrà rimosso
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="uniqueKey"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        protected T1 GetLazy<T1>(string uniqueKey, LazyLoadFunc<T1> fn)
        {
            if (!mLazyDic.TryGetValue(uniqueKey, out object obj))
            {
                obj = fn();
                mLazyDic.Add(uniqueKey, obj);
            }

            return (T1)obj;
        }

        /// <summary>
        /// Ritorna oggetto precedentemente caricato oppure lo carica tramite la funzione in input e lo memorizza per accessi successivi
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="uniqueKey"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        protected T1 LazyGet<T1>(string uniqueKey, LazyLoadFunc<T1> fn)
        {
            if (!mLazyDic.TryGetValue(uniqueKey, out object obj))
            {
                obj = fn();
                mLazyDic.Add(uniqueKey, obj);
            }

            return (T1)obj;
        }


        /// <summary>
        /// Come il Getlazy ma con possibilità di specificare una diversa azione in caso di chiamata su oggetto non ancora salvato (evita potenziali query)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="uniqueKey"></param>
        /// <param name="fnForLoaded">Funzione eseguita in caso di oggetto Caricato da DB</param>
        /// <param name="fnForNew">Funzione eseguita in caso di oggetto non ancora salvato su db (utile per forzature)</param>
        /// <returns></returns>
        protected T1 LazyGetEx<T1>(string uniqueKey, LazyLoadFunc<T1> fnForLoaded, LazyLoadFunc<T1> fnForNew)
        {
            if (!mLazyDic.TryGetValue(uniqueKey, out object obj))
            {
                if (this.DataObj.ObjectState == Data.Objects.Common.EObjectState.New)
                    obj = fnForNew();
                else
                    obj = fnForLoaded();

                mLazyDic.Add(uniqueKey, obj);
            }

            return (T1)obj;
        }


        /// <summary>
        /// Resetta dei dati eventualmente cached sull'oggetto in modo che l'accesso successivo esegua il refresh
        /// </summary>
        /// <param name="uniqueKey"></param>
        protected void LazyReset(string uniqueKey)
        {
            this.mLazyDic.Remove(uniqueKey);
        }

        /// <summary>
        /// Forza l'impostazione di un valore Lazy (per usi successivi)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="uniqueKey"></param>
        /// <param name="value"></param>
        protected void LazySet<T1>(string uniqueKey, T1 value)
        {
            this.mLazyDic[uniqueKey] = value;
        }


        #endregion

    }
}
