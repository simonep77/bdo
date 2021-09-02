using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Core.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Attributes;
using Business.Data.Objects.Core.Base;
using System;
using System.Collections.Generic;
using System.Data;

namespace Business.Data.Objects.Core.Schema.Definition
{
    /// <summary>
    /// Definizione di proprietà
    /// </summary>
    class PropertyObject : Property
    {

        #region PROPERTIES
        public byte ObjectIndex { get; set; }

        public override object DefaultValue
        {
            get { return null; }
        }

        public override bool ExcludeSelect { get; } = true;


        #endregion

        #region CONSTRUCTORS

        public PropertyObject(string name, Type type, byte objIndex)
            : base(name, type)
        {
            this.ObjectIndex = objIndex;
        }

        #endregion

        #region PUBLIC

        public override void ValidateDefinition()
        {
            //Oggetto mappato: Verifica propertyMap
            if (!this.HasPropertyMaps)
                throw new SchemaReaderException(this, $"E' necessario impostare l'attributo {nameof(PropertyMap)} con la/le proprietà da cui caricare l'oggetto dipendente");

            //Oggetto mappato: Verifica propertyMap
            if (!this.IsReadonly || this.Column != null)
                throw new SchemaReaderException(this, SchemaMessages.Prop_PropertyMapReadonly);

        }


        public override bool FillFromAttribute(Attributes.BaseAttribute attr)
        {
            if (base.FillFromAttribute(attr))
                return true;

            //Property Map
            if (attr is PropertyMap)
            {
                //Aggiunge il mapping
                try
                {
                    PropertyMap oAttrMap = (PropertyMap)attr;

                    //Se non ci sono nomi errore
                    if (oAttrMap.Names == null || oAttrMap.Names.Length == 0)
                        throw new SchemaReaderException(this, SchemaMessages.Prop_PropertyMapMissingNames);

                    //Viene modificato il tipo di proprieta'
                    this.ExcludeInsert = true;
                    this.ExcludeUpdate = true;

                    //Se non ha map lo crea
                    if (!this.HasPropertyMaps)
                        this.PropertyMap = new PropertyList(2);

                    //Aggiunge a che mappa il riferimento della mappata
                    foreach (var s in oAttrMap.Names)
                    {
                        Property oPropToMap = this.Schema.Properties.GetPropertyByName(s);

                        //Se non esiste map lo crea
                        if (!oPropToMap.HasPropertyMaps)
                            oPropToMap.PropertyMap = new PropertyList(1);

                        //Check
                        if (!(oPropToMap is PropertySimple))
                            throw new SchemaReaderException(this, SchemaMessages.Prop_PropertyMapToSimple);

                        //Aggiunge alla corrente il riferimento alla mappata
                        this.PropertyMap.Add(oPropToMap);

                        //Aggiunge alla puntata il riferimento di chi la punta
                        oPropToMap.PropertyMap.Add(this);
                    }

                }
                catch (ObjectException ex)
                {
                    throw new SchemaReaderException(this, SchemaMessages.Prop_PropertyMapMustBeFirst, ex.Message);
                }

            }
            else
            {
                throw new NotImplementedException();
            }

            return true;
        }

        /// <summary>
        /// Ritorna il dato compatibile db
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override object GetValueForDb(DataObjectBase obj)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Ritorna il valore della proprieta'
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override object GetValue(DataObjectBase obj)
        {
            DataObjectBase oRet = obj.mDataSchema.Objects[this.ObjectIndex];

            if (oRet == null && !obj.mDataSchema.GetFlagsAll(this.PropertyIndex, DataFlags.ObjLoaded))
            {
                //Se property 0 e' nulla esce
                if (this.PropertyMap[0].IsNull(this.PropertyMap[0].GetValue(obj)))
                    return null;

                //Crea pk come composizione delle proprieta' mappate
                var arrPk = new object[this.PropertyMap.Count];
                for (int i = 0; i < this.PropertyMap.Count; i++)
                {
                    arrPk[i] = this.PropertyMap[i].GetValueForDb(obj);
                }

                //Se array OK definito ed il primo valore non nullo imposta oggetto
                if (arrPk != null && arrPk[0] != null)
                    oRet = obj.GetSlot().LoadObjectInternalByKEY(ClassSchema.PRIMARY_KEY, this.Type, true, arrPk);

                //Carica oggetto e imposta come caricato
                obj.mDataSchema.Objects[this.ObjectIndex] = (DataObjectBase)oRet;
                //In qualunque caso imposta come caricato
                obj.mDataSchema.SetFlags(this.PropertyIndex, DataFlags.ObjLoaded, true);
            }

            //Caso Base
            return oRet;
        }


        /// <summary>
        /// Metodo privato di impostazione proprietà
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public override void SetValue(DataObjectBase obj, object value)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///  Crea rappresentazione Xml
        /// </summary>
        /// <param name="xw"></param>
        /// <param name="obj"></param>
        /// <param name="depth"></param>
        public override void WriteXml(XmlWrite xw, DataObjectBase obj, int depth)
        {
            //Include oggetto mappato se profondita' prevista e campio non null
            if (depth <= 0)
                return;

            var oTemp = this.GetValue(obj);
            if (oTemp == null)
                return;

            //Include oggetto
            xw.WriteStartElement(this.Name);
            try
            {
                xw.WriteRaw(((DataObjectBase)oTemp).ToXml(depth - 1));
            }
            finally
            {
                xw.WriteEndElement();
            }

        }



        /// <summary>
        /// Carica valori proprieta' da datareader
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dr"></param>
        public override void SetValueFromReader(DataObjectBase obj, IDataReader dr)
        {
            return;
        }


        #endregion

    }
}
