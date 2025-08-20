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
        /// Slot associato all'oggetto (interno), non esporre public pr evitare problemi di serializzazione. Utilizzare il metodo GetSlot()
        /// </summary>
        protected BusinessSlot Slot { get; private set; }

        /// <summary>
        /// Imposta lo slot
        /// </summary>
        /// <param name="slot"></param>
        internal virtual void SetSlot(BusinessSlot slot) => this.Slot = slot;
 
        /// <summary>
        /// Slot associato all'oggetto. E' utilizzato un metodo per evitare serializzazione
        /// </summary>
        public BusinessSlot GetSlot() => this.Slot;

        #endregion


        #region PUBLIC

        #region EXTRA DATA

        /// <summary>
        /// Verifica se presenti dati aggiuntivi su oggetto (singolo o lista) individuati per chiave
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal bool ExtraDataExist(string key) => this.ExtraData.ContainsKey(key);

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
        public void ExtraDataSet(string key, object value) => this.ExtraData[key] = value;
   
        /// <summary>
        /// Rimuove dati aggiuntivi su oggetto (singolo o lista) individuati per chiave
        /// </summary>
        /// <param name="key"></param>
        public void ExtraDataRemove(string key) => this.ExtraData.Remove(key);

        /// <summary>
        /// Ritorna il numero di elementi Extra
        /// </summary>
        /// <returns></returns>
        public int ExtraDataCount() => this.ExtraData.Count;

        /// <summary>
        /// Elimina tutti i dati extra
        /// </summary>
        public void ExtraDataClear() => this.ExtraData.Clear();

        /// <summary>
        /// Ritorna collection di key extra data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ExtraDataKeys() => this.ExtraData.Keys.Select(x => x);

        /// <summary>
        /// Ritorna collection di valori extra data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> ExtraDataValues() => this.mExtraData.Values;

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
