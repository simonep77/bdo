using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Core.Common.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Data.Objects.Core.Base
{
    /// <summary>
    /// Classe base per oggetti dipendenti da slot
    /// </summary>
    public abstract class SlotAwareObject
    {
        private Dictionary<string, object> mExtraData;

        /// <summary>
        /// Espone i dati extra
        /// </summary>
        private Dictionary<string, object> ExtraData
        {
            get
            {
                if (this.mExtraData == null)
                    this.mExtraData = new Dictionary<string, object>();
                return this.mExtraData;
            }
        }

        #region SLOT HANDLING

        /// <summary>
        /// Slot associato all'oggetto (interno)
        /// </summary>
        public BusinessSlot Slot { get; internal set; }


        /// <summary>
        /// Slot associato all'oggetto
        /// </summary>
        [Obsolete("Il metodo verrà eliminato in quanto è esposta la proprietà Slot")]
        public BusinessSlot GetSlot() => this.Slot;

        #endregion


        #region PUBLIC

        #region EXTRA DATA

        /// <summary>
        /// Verifica se presenti dati aggiuntivi su oggetto (singolo o lista) individuati per chiave
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal bool ExtraDataExist(string key)
        {
            //Verifica
            return this.ExtraData.ContainsKey(key);

        }

        /// <summary>
        /// Ritorna dati aggiuntivi memorizzati a livello di oggetto (singolo o lista)
        /// individuati per chiave. Se non trovata la chiave ritorna il valore defult fornito
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public object ExtraDataGet(string key, object defaultValue)
        {
            //Cerca valore
            if (!this.ExtraData.TryGetValue(key, out object oRet))
                return defaultValue;

            return oRet;
        }


        /// <summary>
        /// Ritorna dati aggiuntivi tipizzati
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T ExtraDataGet<T>(string key, T defaultValue)
        {
            //Cerca valore
            if (!this.ExtraData.TryGetValue(key, out object oRet))
                return defaultValue;

            return (T)Convert.ChangeType(oRet, typeof(T));
        }


        /// <summary>
        /// Imposta dati aggiuntivi su oggetto (singolo o lista) individuati per chiave
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ExtraDataSet(string key, object value)
        {
            //Imposta valore
            this.ExtraData[key] = value;
        }


        /// <summary>
        /// Rimuove dati aggiuntivi su oggetto (singolo o lista) individuati per chiave
        /// </summary>
        /// <param name="key"></param>
        public void ExtraDataRemove(string key)
        {
            //Rimuove
            this.ExtraData.Remove(key);
        }


        /// <summary>
        /// Ritorna il numero di elementi Extra
        /// </summary>
        /// <returns></returns>
        public int ExtraDataCount()
        {
            //Ritorna
            return this.ExtraData.Count;
        }


        /// <summary>
        /// Elimina tutti i dati extra
        /// </summary>
        public void ExtraDataClear()
        {
            //Rimuove
            this.ExtraData.Clear();
        }

        /// <summary>
        /// Ritorna collection di key extra data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ExtraDataKeys()
        {
            return this.ExtraData.Keys.Select(x => x);
        }

        /// <summary>
        /// Ritorna collection di valori extra data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> ExtraDataValues()
        {
            return this.mExtraData.Values.Select(x => x);
        }

        #endregion

        /// <summary>
        /// Passa oggetto su Slot Fornito
        /// </summary>
        /// <param name="slotIn"></param>
        public void SwitchToSlot(BusinessSlot slotIn)
        {
            if (slotIn == null || slotIn.Terminated)
                throw new ObjectException(ObjectMessages.Base_SwithToNullSession);

            //Imposta sessione
            this.Slot = slotIn;
        }

        #endregion

    }
}
