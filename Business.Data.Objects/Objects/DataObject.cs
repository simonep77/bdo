using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Objects.Base;
using Bdo.Schema.Definition;
using Bdo.Schema.Usage;
using Bdo.Utils;
using Business.Data.Objects.Common;
using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Common.Utils;

namespace Bdo.Objects
{
    /// <summary>
    /// Classe da cui ereditare tutti gli oggetti di business con accesso a database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataObject<T>
        : DataObjectBase where T : DataObject<T>
        
    {

        #region PUBLIC DELEGATES

        /// <summary>
        /// Delegato per la gestione dell' Xml del singolo oggetto della lista
        /// </summary>
        /// <param name="value"></param>
        /// <param name="writer"></param>
        /// <param name="args"></param>
        public delegate void XmlFunction(T value, XmlWrite writer, params object[] args);

        #endregion

        #region PUBLIC METHODS


        /// <summary>
        /// Verifica se due oggetti sono uguali in tutte le proprietà mappate su DB
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool EqualsDeep(T other)
        {
            DataDiffList oDiffList = this.Diff(other);

            return (oDiffList.Count == 0);
        }


        /// <summary>
        /// Ritorna l'elenco di modifiche tra la versione corrente (gia' salvata)
        /// ed il corrispondente oggetto caricato dal sorgente (cache, database)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public DataDiffList DiffSource()
        {
            if (this.ObjectState != EObjectState.Loaded)
            {
                throw new ObjectException(Resources.ObjectMessages.Base_DiffSourceNotLoaded, this.mClassSchema.ClassName);
            }

            //Forza skip del tracking se attivo
            if (this.Slot.LiveTrackingEnabled)
                this.Slot.mLiveTrackingStore.IsActive = false;
            try
            {
                //Esegue il diff
                return this.Diff(this.Slot.LoadObjByPK<T>(this.mClassSchema.PrimaryKey.GetValues(this)));
            }
            finally
            {
                //riattiva tracking
                if (this.Slot.LiveTrackingEnabled)
                    this.Slot.mLiveTrackingStore.IsActive = true;
            }
        }


        /// <summary>
        /// Dati due oggetti ritorna elenco differenze
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public DataDiffList Diff(T other)
        {
            Property oProp;
            object oSource, oOther;

            //Se other null eccezione
            if (other == null)
            {
                throw new ObjectException(Resources.ObjectMessages.Diff_Null, this.mClassSchema.ClassName);
            }

            //Se other non e' dello stesso tipo
            if (this.mClassSchema.InternalID != other.mClassSchema.InternalID)
            {
                throw new ObjectException(Resources.ObjectMessages.Diff_WrongType, this.mClassSchema.ClassName, other.mClassSchema.ClassName);
            }

            DataDiffList oDiffList = new DataDiffList();
            for (int iPropIndex = 0; iPropIndex < this.mClassSchema.Properties.Count; iPropIndex++)
            {
                oProp = this.mClassSchema.Properties[iPropIndex];
                oSource = this.GetProperty(iPropIndex);
                oOther = other.GetProperty(iPropIndex);

                //Proprietà normale
                if (!object.Equals(oSource, oOther))
                {
                    DataDiff oDiff = new DataDiff(oProp.Name, ref oSource, ref oOther);
                    oDiffList.Add(oDiff);
                }
            }

            return oDiffList;
        }

        /// <summary>
        /// Scrive Xml con possibilita' di integrazione dati attraverso la specifica di una funzione
        /// di manipolazione
        /// </summary>
        /// <param name="function"></param>
        /// <param name="rewriteAll">
        /// Se true allora non emette l'Xml standard dell'oggetto
        /// </param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string ToXml(XmlFunction function, bool rewriteAll, params object[] args)
        {
            using (XmlWrite xw = new XmlWrite())
            {
                //Se deve riscrivere
                if (!rewriteAll)
                {
                    xw.WriteRaw(this.ToXml(1));
                }

                //Se fornita funzione allora la richiama
                if (function != null)
                {
                    function((T)this, xw, args);
                }

                return xw.ToString();
            }
        }

        /// <summary>
        /// Ritorna una lista contenente l'elemento selezionato
        /// </summary>
        /// <typeparam name="TL"></typeparam>
        /// <returns></returns>
        public TL ToList<TL>()
            where TL : DataList<TL, T>
        {
            var list = this.Slot.CreateList<TL>();
            list.Add(this);
            return list;
        }

        /// <summary>
        /// Ritorna una lista contenente l'elemento selezionato e gli elementi presenti nell'enumerabile in input
        /// </summary>
        /// <typeparam name="TL"></typeparam>
        /// <param name="others"></param>
        /// <returns></returns>
        public TL ToListFromEnumerable<TL>(IEnumerable<T> others)
            where TL : DataList<TL, T>
        {

            var list = this.ToList<TL>();
            if (others != null)
                list.AddRange(others);
            return list;
        }


        /// <summary>
        /// Ritorna una lista contenente l'elemento selezionato e gli elementi presenti nell'array in input
        /// </summary>
        /// <typeparam name="TL"></typeparam>
        /// <param name="others"></param>
        /// <returns></returns>
        public TL ToListFromArray<TL>(params T[] others)
            where TL : DataList<TL, T>
        {
            return this.ToListFromEnumerable<TL>(others);
        }


        /// <summary>
        /// Dato un DataObject ritorna il corrispondente business object
        /// </summary>
        /// <typeparam name="TL"></typeparam>
        /// <returns></returns>
        public TL ToBizObject<TL>()
            where TL: BusinessObject<T>
        {
            return (TL)Bdo.ObjFactory.ProxyAssemblyCache.Instance.CreateBizObj(Bdo.ObjFactory.ProxyAssemblyCache.Instance.GetBizEntry(typeof(TL)), this);
        }


        #endregion

    }
}
