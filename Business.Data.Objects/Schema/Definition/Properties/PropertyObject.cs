using System;
using System.Collections.Generic;
using System.Data;
using Bdo.Utils;
using Bdo.Attributes;
using Bdo.Objects;
using Bdo.Objects.Base;

namespace Bdo.Schema.Definition
{
    /// <summary>
    /// Definizione di proprietà
    /// </summary>
    class PropertyObject : Property
    {

        #region PROPERTIES
        public bool IsMapped;
        public byte ObjectIndex { get; set; }

        public override object DefaultValue
        {
            get { return null; }
        }
        
        public override bool IsSqlSelectExcluded
        {
            get
            {
                return this.HasPropertyMaps;
            }
        }


        #endregion

        #region CONSTRUCTORS

        public PropertyObject(string name, Type type, byte objIndex)
            :base(name,type)
        {
            this.ObjectIndex = objIndex;
        }

        #endregion

        #region PUBLIC

        public override void ValidateDefinition()
        {
            if (this.IsMapped)
            {
                //Oggetto mappato: Verifica propertyMap
                if (this.HasPropertyMaps && (!this.IsReadonly || this.Column != null))
                    throw new SchemaReaderException(this, Resources.SchemaMessages.Prop_PropertyMapReadonly);
            }
            else
            {
                //Oggetto diretto
                if (this.Column == null)
                    throw new SchemaReaderException(this, Resources.SchemaMessages.Prop_ObjectSpecifyColumns);

                //Se prop oggetto verifica che sia presente anche il tipo della colonna
                //Verifica tipo definito
                if (this.Column.DbType == null)
                    throw new SchemaReaderException(this, Resources.SchemaMessages.Prop_ObjectSpecifySubType);
            }
                    

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
                        throw new SchemaReaderException(this, Resources.SchemaMessages.Prop_PropertyMapMissingNames);
                    
                    //Viene modificato il tipo di proprieta'
                    this.IsMapped = true;
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
                            throw new SchemaReaderException(this, Resources.SchemaMessages.Prop_PropertyMapToSimple);

                        //Aggiunge alla corrente il riferimento alla mappata
                        this.PropertyMap.Add(oPropToMap);

                        //Aggiunge alla puntata il riferimento di chi la punta
                        oPropToMap.PropertyMap.Add(this);
                    }

                }
                catch (Bdo.Objects.ObjectException ex)
                {
                    throw new SchemaReaderException(this, Resources.SchemaMessages.Prop_PropertyMapMustBeFirst, ex.Message);
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
        public override object GetValueForDb(Objects.Base.DataObjectBase obj)
        {
            //Ottiene valore
            var input = obj.mDataSchema.Values[this.PropertyIndex];

            //Null esce subito
            if (input == null)
                return input;

            //Converte
            return Convert.ChangeType(input, this.Column.DbType);
        }


        /// <summary>
        /// Ritorna il valore della proprieta'
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override object GetValue(Objects.Base.DataObjectBase obj)
        {
            DataObjectBase oRet = obj.mDataSchema.Objects[this.ObjectIndex];

            if (oRet == null && !obj.mDataSchema.GetFlagsAll(this.PropertyIndex, DataFlags.ObjLoaded))
            {
                object[] arrPk;

                if(this.IsMapped)
                {
                    //Se property 0 e' nulla esce
                    if (this.PropertyMap[0].IsNull(this.PropertyMap[0].GetValue(obj)))
                        return null;

                    //Crea pk come composizione delle proprieta' mappate
                    arrPk = new object[this.PropertyMap.Count];
                    for (int i = 0; i < this.PropertyMap.Count; i++)
                    {
                        arrPk[i] = this.PropertyMap[i].GetValueForDb(obj);
                    }
                }
                else
                {
                    //Carica PK con valori base proprieta'
                    arrPk = new object[] { this.GetValueForDb(obj) };
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
            //Di base imposta modifica a true
            bool bChanged = true;

            //Se attivato il real change tracking 
            if (obj.GetSlot().Conf.ChangeTrackingEnabled)
                //Se il valore e' variato imposta flag
                bChanged = !object.Equals(value, this.GetValue(obj));

            //Se valore modificato (realmente o sempre)
            if (!bChanged)
                return;

            if (this.IsMapped)
                throw new NotImplementedException();

            //Se null caso semplice
            if (value == null)
            {
                obj.mDataSchema.Objects[this.ObjectIndex] = null;
                obj.mDataSchema.Values[this.PropertyIndex] = null;
                obj.mDataSchema.SetFlags(this.PropertyIndex, DataFlags.ObjLoaded, false);
            }
            else
            {
                var oBdoObj = value as DataObjectBase;

                if (oBdoObj == null)
                {
                    obj.mDataSchema.Objects[this.ObjectIndex] = null;
                    obj.mDataSchema.SetFlags(this.PropertyIndex, DataFlags.ObjLoaded, false);
                    obj.mDataSchema.Values[this.PropertyIndex] = value;
                }
                else
                {
                    obj.mDataSchema.Objects[this.ObjectIndex] = oBdoObj;
                    obj.mDataSchema.SetFlags(this.PropertyIndex, DataFlags.ObjLoaded, true);
                    obj.mDataSchema.Values[this.PropertyIndex] = oBdoObj.mClassSchema.PrimaryKey.Properties[0].GetValue(oBdoObj);
                }
            }

            //Imposta flag modifica e oggetto caricato
            obj.mDataSchema.SetFlags(this.PropertyIndex, DataFlags.Changed, true);

            //Lancia evento bindings
            obj.firePropertyChanged(this);

        }


        /// <summary>
        ///  Crea rappresentazione Xml
        /// </summary>
        /// <param name="xw"></param>
        /// <param name="obj"></param>
        /// <param name="depth"></param>
        public override void WriteXml(XmlWrite xw, DataObjectBase obj, int depth)
        {
            //Scrive proprietà base
            object oValue = this.GetValueForDb(obj);

            if (!this.IsMapped)
            {
                if (oValue != null)
                    xw.WriteElementString(this.Column.Name, oValue.ToString());
                else
                    xw.WriteElementString(this.Column.Name, string.Empty);
            }

            //Include oggetto mappato se profondita' prevista e campio non null
            if (depth > 0 && (this.IsMapped || oValue != null))
            {
                var oTemp = this.GetValue(obj);
                if (oTemp != null)
                {
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
            }

        }

        /// <summary>
        /// Scrive property nel DTO
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="obj"></param>
        public override void WriteDTO(Dictionary<string, object> dto, DataObjectBase obj, int depth)
        {
            if (depth <= 0)
                return;

            DataObjectBase oTemp = (DataObjectBase)this.GetValue(obj);
            dto.Add(this.Name, (oTemp == null) ? null : oTemp.ToDTO(--depth));
        }


        /// <summary>
        /// Legge DTO
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="obj"></param>
        public override void ReadDTO(Dictionary<string, object> dto, DataObjectBase obj)
        {
            object o = null;

            //Se non trovato esce
            if (!dto.TryGetValue(this.Name, out o))
                return;

            //Se oggetto non diretto esce
            if (this.IsMapped)
                return;

            //Verifica DTO interno
            var dtoInner = o as Dictionary<string, object>;
            if (dtoInner == null && o != null)
                throw new ObjectException("Il valore della chiave {0} del DTO non e' una rappresentazione di oggetto BDO valida");

            //Salva
            this.SetValue(obj, obj.GetSlot().FromDTO_AsLoadedByType(this.Type, dtoInner));

        }


        /// <summary>
        /// Carica valori proprieta' da datareader
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dr"></param>
        public override void SetValueFromReader(DataObjectBase obj, IDataReader dr)
        {
            object oTemp;

            //Se oggetto mappato esce
            if (this.IsMapped)
                return;

            //PROP OGGETTO
            oTemp = dr[this.Column.Name];

            if (DBNull.Value.Equals(oTemp) || oTemp == null)
            {
                oTemp = null;
                //Imposta oggetto come caricato
                obj.mDataSchema.SetFlags(this.PropertyIndex, DataFlags.ObjLoaded, true);
            }
            else
            {
                //VALORE: converte al tipo della proprietà se diverso
                if (oTemp.GetType() != this.Column.DbType)
                    oTemp = Convert.ChangeType(oTemp, this.Column.DbType);
            }

            //imposta Flag caricato
            obj.mDataSchema.SetFlags(this.PropertyIndex, DataFlags.Loaded, true);

            //Imposta comunque dato semplice
            obj.mDataSchema.Values[this.PropertyIndex] = oTemp;
        }


        #endregion

    }
}
