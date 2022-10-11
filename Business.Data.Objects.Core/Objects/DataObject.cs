using Business.Data.Objects.Common;
using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Core.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Core.ObjFactory;
using Business.Data.Objects.Core.Schema.Definition;
using System.Collections.Generic;
using System.Dynamic;
using System;
using System.Reflection;

namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Classe da cui ereditare tutti gli oggetti di business con accesso a database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataObject<T>
        : DataObjectBase where T : DataObject<T>
    {


        #region PUBLIC METHODS

        //public DataObject()
        //{
        //    var t = typeof(T);
        //    //Se la classe e' astratta significa che e' creata tramite slot, altrimenti tramite "new"
        //    if (!t.IsAbstract)
        //    {
        //        //Crea istanza
        //        ProxyEntryDAO oTypeEntry = ProxyAssemblyCache.Instance.GetDaoEntry(t);

        //        //Imposta schema su oggetto
        //        this.mClassSchema = oTypeEntry.ClassSchema;
        //        this.ObjectRefId = ProxyAssemblyCache.Instance.NewObjeRefId();
        //        this.mDataSchema = new Schema.Usage.DataSchema(this.mClassSchema.Properties.Count, this.mClassSchema.ObjCount);
        //    }
        //}


        /// <summary>
        /// Verifica se due oggetti sono uguali in tutte le proprietà mappate su DB
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool EqualsDeep(T other)
        {
            var oDiffList = this.Diff(other);

            return (oDiffList.Count == 0);
        }


        /// <summary>
        /// Ritorna l'elenco di modifiche tra la versione corrente (gia' salvata)
        /// ed il corrispondente oggetto caricato dal sorgente (cache, database)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public List<Tuple<string, object, object>> DiffSource()
        {
            if (this.ObjectState != EObjectState.Loaded)
                throw new ObjectException(ObjectMessages.Base_DiffSourceNotLoaded, this.mClassSchema.ClassName);

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
        /// Dati due oggetti ritorna elenco differenze. Item1 e' il nome della prorieta, item2 il valore dell'oggetto origine, item3 dell'oggetto confrontato
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public List<Tuple<string, object, object>> Diff(T other)
        {
            object oSource, oOther;

            //Se other null eccezione
            if (other == null)
                throw new ObjectException(ObjectMessages.Diff_Null, this.mClassSchema.ClassName);

            //Se other non e' dello stesso tipo
            if (this.mClassSchema.InternalID != other.mClassSchema.InternalID)
                throw new ObjectException(ObjectMessages.Diff_WrongType, this.mClassSchema.ClassName, other.mClassSchema.ClassName);

            var oDiffList = new List<Tuple<string, object, object>>();


            foreach (var oProp in this.mClassSchema.Properties)
            {
                if (!(oProp is PropertySimple))
                    continue;

                oSource = oProp.GetValue(this);
                oOther = oProp.GetValue(other);

                //Proprietà normale
                if (!object.Equals(oSource, oOther))
                    oDiffList.Add(new Tuple<string, object, object>(oProp.Name, oSource, oOther));
            }

            return oDiffList;
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
        /// Dato un DataObject ritorna il corrispondente business object
        /// </summary>
        /// <typeparam name="TL"></typeparam>
        /// <returns></returns>
        public TL ToBizObject<TL>()
            where TL: BusinessObject<T>
        {
            return (TL)ProxyAssemblyCache.Instance.CreateBizObj(ProxyAssemblyCache.Instance.GetBizEntry(typeof(TL)), this);
        }

        ///// <summary>
        ///// Ritorna un oggetto dinamico copiato dall'attuale
        ///// </summary>
        ///// <returns></returns>
        //public dynamic ToDynamicObject()
        //{
        //    var o = new ExpandoObject();
        //    var od = o as IDictionary<string, object>;
        //    foreach (var prop in this.mClassSchema.Properties)
        //    {
        //        od[prop.Name] = prop.GetValue(this);
        //    }
        //    od[nameof(DataObjectBase.ObjectSource)] = this.mDataSchema.ObjectSource;
        //    od[nameof(DataObjectBase.ObjectState)] = this.mDataSchema.ObjectState;

        //    return o;
        //}

        ///// <summary>
        ///// Carica i dati di un oggetto dinamico su quello corrente
        ///// </summary>
        ///// <param name="obj"></param>
        //public void FromDynamicObject(dynamic obj)
        //{
        //    var od = obj as IDictionary<string, object>;

        //    foreach (var prop in this.mClassSchema.Properties)
        //    {
        //        prop.SetValue(this, od[prop.Name]);
        //    }

        //    this.mDataSchema.ObjectSource = (EObjectSource)od[nameof(DataObjectBase.ObjectSource)];
        //    this.mDataSchema.ObjectState = (EObjectState)od[nameof(DataObjectBase.ObjectState)];
        //}


        /// <summary>
        /// Prova a copiare i valori su un oggetto di outpu output
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void TryCopyTo(object output, bool checkNullable = true)
        {
            var outputType = output.GetType();

            foreach (var prop in this.mClassSchema.Properties)
            {
                try
                {
                    var _value = prop.GetValue(this);

                    if (_value != null)
                    {
                        PropertyInfo p = outputType.GetProperty(prop.Name);

                        if (p == null)
                            continue;

                        //Se valore puo' considerarsi null e la prprietà è nullable non la imposta
                        if (checkNullable && Nullable.GetUnderlyingType(p.PropertyType) != null && prop.IsNull(_value))
                            continue;

                        if (p.CanWrite)
                            p.SetValue(output, Convert.ChangeType(_value, prop.Type));
                    }
                }
                catch
                {

                }
            }
        }

        #endregion

    }
}
