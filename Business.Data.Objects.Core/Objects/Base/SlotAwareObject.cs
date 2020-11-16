using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Common.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Base
{
    /// <summary>
    /// Classe base per oggetti dipendenti da slot
    /// </summary>
    public abstract class SlotAwareObject
    {
        private BusinessSlot mSlot;
        private Dictionary<string, object> mExtraData;

        #region SLOT HANDLING

        /// <summary>
        /// Slot associato all'oggetto (interno)
        /// </summary>
        protected BusinessSlot Slot
        {
            get
            {
                return this.mSlot;
            }
        }

        /// <summary>
        /// Imposta lo slot sull'oggetto
        /// </summary>
        /// <param name="slot"></param>
        internal virtual void SetSlot(BusinessSlot slot)
        {
            this.mSlot = slot;
        }

        /// <summary>
        /// Slot associato all'oggetto
        /// </summary>
        public BusinessSlot GetSlot()
        {
            return this.mSlot;
        }

        #endregion


        #region PUBLIC

        #region EXTRA DATA

        /// <summary>
        /// Se non presente extra data viene creato
        /// </summary>
        private void extraDataTouch()
        {
            //Extradata non valorizzato: lo crea
            if (this.mExtraData == null)
                this.mExtraData = new Dictionary<string, object>();
        }

        /// <summary>
        /// Verifica se presenti dati aggiuntivi su oggetto (singolo o lista) individuati per chiave
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal bool ExtraDataExist(string key)
        {
            //Assicura extradata
            this.extraDataTouch();

            //Verifica
            return this.mExtraData.ContainsKey(key);

        }

        /// <summary>
        /// Ritorna dati aggiuntivi memorizzati a livello di oggetto (singolo o lista)
        /// individuati per chiave. Se non trovata la chiave ritorna il valore defult fornito
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal object ExtraDataGet(string key, object defaultValue)
        {
            //Assicura extradata
            this.extraDataTouch();

            object oRet;

            //Cerca valore
            if (!this.mExtraData.TryGetValue(key, out oRet))
                return defaultValue;

            return oRet;
        }


        /// <summary>
        /// Imposta dati aggiuntivi su oggetto (singolo o lista) individuati per chiave
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ExtraDataSet(string key, object value)
        {
            //Assicura extradata
            this.extraDataTouch();

            //Imposta valore
            this.mExtraData[key] = value;
        }


        /// <summary>
        /// Rimuove dati aggiuntivi su oggetto (singolo o lista) individuati per chiave
        /// </summary>
        /// <param name="key"></param>
        public void ExtraDataRemove(string key)
        {
            //Assicura extradata
            this.extraDataTouch();

            //Rimuove
            this.mExtraData.Remove(key);
        }


        /// <summary>
        /// Ritorna il numero di elementi Extra
        /// </summary>
        /// <returns></returns>
        public int ExtraDataCount()
        {
            //Assicura extradata
            this.extraDataTouch();

            //Ritorna
            return this.mExtraData.Count;
        }


        /// <summary>
        /// Elimina tutti i dati extra
        /// </summary>
        public void ExtraDataClear()
        {
            //Assicura extradata
            this.extraDataTouch();

            //Rimuove
            this.mExtraData.Clear();
        }


        /// <summary>
        /// Ritorna tutte le chiavi registrate per l'oggetto
        /// </summary>
        /// <returns></returns>
        public string[] ExtraDataGetKeys()
        {
            //Extradata non valorizzato: ritorna array vuoto
            var keys = this.ExtraDataKeys();

            //Rimuove
            string[] retArr = new string[keys.Count];
            keys.CopyTo(retArr, 0);
            return retArr;
        }

        /// <summary>
        /// Ritorna collection di key extra data
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object>.KeyCollection ExtraDataKeys()
        {
            //Assicura extradata
            this.extraDataTouch();

            return this.mExtraData.Keys;
        }

        /// <summary>
        /// Ritorna collection di valori extra data
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object>.ValueCollection ExtraDataValues()
        {
            //Assicura extradata
            this.extraDataTouch();

            return this.mExtraData.Values;
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
            this.SetSlot(slotIn);
        }

        #endregion

    }
}
