using Business.Data.Objects.Common;
using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Core.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Attributes;
using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace Business.Data.Objects.Core.Schema.Definition
{
    /// <summary>
    /// Definizione di proprietà
    /// </summary>
    class PropertySimple : Property
    {

        #region PROPERTIES
        private object mDefaultValue;
        private string XmlFormatString;
        private Encrypted mEncAttr;

        //Liste dinamiche
        public List<Attributes.BaseModifierAttribute> AttrModifiers { get; set; }
        public List<Attributes.BaseValidatorAttribute> AttrValidators { get; set; }

        public override object DefaultValue
        {
            get
            {
                return this.mDefaultValue;
            }
        }

        /// <summary>
        /// Indica se presenti validatori
        /// </summary>
        public bool HasValidators
        {
            get
            {
                return (this.AttrValidators != null && this.AttrValidators.Count > 0);
            }
        }

        /// <summary>
        /// Indica se presenti modificatori
        /// </summary>
        public bool HasModifiers
        {
            get
            {
                return (this.AttrModifiers != null && this.AttrModifiers.Count > 0);
            }
        }

        /// <summary>
        /// Indica se esclusa dal caricamento standard (query load)
        /// </summary>
        public override bool IsSqlSelectExcluded { 
            get {
                return this.LoadOnAccess;
            } 
        }


        #endregion

        #region CONSTRUCTORS

        public PropertySimple(string name, Type type)
            :base(name,type)
        {
            this.mDefaultValue = PropertyHelper.GetDefaultValue(type);

        }

        #endregion

        #region PUBLIC

        public override void ValidateDefinition()
        {
            //Consentito solo byte array
            if (this.Type.IsArray && !TypeHelper.IsByteArray(this.Type))
                throw new SchemaReaderException(this, SchemaMessages.Prop_NoArrayBytes);

            //Imposta tipo default se non presente
            if (this.Column.DbType == null)
                this.Column.DbType = this.Type;

        }


        public override bool FillFromAttribute(BaseAttribute attr)
        {
            if (base.FillFromAttribute(attr))
                return true;

            //Check campi automatici
            if (attr is AutomaticField)
            {
                if (this.IsAutomatic)
                    throw new SchemaReaderException(this, SchemaMessages.Prop_NoMultipleAutomatic);

                this.IsAutomatic = true;
                this.Schema.AutoProperties.Add(this);
                this.IsReadonly = true;

                //AUTOINCREMENT
                if (attr is AutoIncrement)
                {
                    this.Schema.AutoIncPk = true;
                    this.ExcludeInsert = true;
                    this.ExcludeUpdate = true;

                    //Deve essere numerico e almeno Int32
                    if (!TypeHelper.IsIntegerType(this.Type) || TypeHelper.IntegerSize(this.Type) < 4)
                        throw new SchemaReaderException(this, SchemaMessages.Prop_AutoInc_32_Bit);
                }
                //AUTO INSERT TIMESTAMP
                else if (attr is AutoInsertTimestamp)
                {
                    this.ExcludeUpdate = true;
                }
                //AUTO UPDATE TIMESTAMP
                else if (attr is AutoUpdateTimestamp)
                {
                    //nulla
                }
            }
            //DEFAULT VALUE
            else if (attr is Attributes.DefaultValue)
            {
                this.mDefaultValue = ((Attributes.DefaultValue)attr).ConvertTo(this.Type);
            }
            //CARICA SUL PRIMO ACCESSO
            else if (attr is LoadOnAccess)
            {
                this.LoadOnAccess = true;
            }
            //MODIFICATORI
            else if (attr is BaseModifierAttribute)
            {
                BaseModifierAttribute bm = (BaseModifierAttribute)attr;

                if (bm.CanApplyToProperty(this))
                {
                    if (!this.HasModifiers)
                        this.AttrModifiers = new List<BaseModifierAttribute>();

                    this.AttrModifiers.Add(bm);
                }
            }
            //VALIDATORI
            else if (attr is BaseValidatorAttribute)
            {
                BaseValidatorAttribute bv = (BaseValidatorAttribute)attr;

                if (bv.CanApplyToProperty(this))
                {
                    if (!this.HasValidators)
                        this.AttrValidators = new List<BaseValidatorAttribute>();

                    this.AttrValidators.Add(bv);
                }

            }
            //Username automatico in salvataggio
            else if (attr is UserInfo) 
            {
                this.Schema.UserInfo = this;
            }
            //Cancellazione logica
            else if (attr is LogicalDelete) 
            {
                this.Schema.LogicalDeletes.Add(this);
            }
            //Proprieta' criptata su DB
            else if (attr is Encrypted)
            {
                if (!TypeHelper.IsString(this.Type))
                    throw new SchemaReaderException(this, SchemaMessages.Prop_Encryped_Only_String);

                this.mEncAttr = (Encrypted)attr;
            }
            //XmlFormatString
            else if (attr is XmlFormatString)
            {
                this.XmlFormatString = ((XmlFormatString)attr).Format;
            }
            else
            {
                throw new NotImplementedException();
            }

            return true;
        }


        /// <summary>
        /// Ritorna il valore della proprieta'
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override object GetValue(DataObjectBase obj)
        {
            //Se null ritorna valore default
            object oRet = obj.mDataSchema.Values[this.PropertyIndex];

            if (oRet == null)
            {
                //Se oggetto nuovo o già caricato da db ritorna default
                if (obj.mDataSchema.ObjectState == EObjectState.New || obj.mDataSchema.GetFlagsAll(this.PropertyIndex, DataFlags.Loaded))
                    //Ritorna default
                    return this.DefaultValue;

                //Deve Caricare property
                obj.LoadPropertyFromDB(this);

                //Reimposta il valore
                oRet = obj.mDataSchema.Values[this.PropertyIndex];
            }

            return oRet;
        }


        /// <summary>
        /// Ritorna il dato compatibile db
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override object GetValueForDb(DataObjectBase obj)
        {
            //Ottiene valore
            var input = this.GetValue(obj);

            //Null esce subito
            if (input == null)
                return input;

            //Se il tipo di dato db e' differente dalla property va convertito
            if (!object.Equals(this.Type,this.Column.DbType))
                return Convert.ChangeType(input, this.Column.DbType);

            //Se valore da criptare allora esegue
            if (this.mEncAttr != null)
            {
                try
                {
                    input = this.mEncAttr.Encrypt(obj.GetSlot(), this, input.ToString());
                }
                catch (Exception ex)
                {
                    throw new ObjectException(ObjectMessages.Enc_Encrypt_Failure, obj.mClassSchema.ClassName, this.Name, ex.Message);
                }
            }

            //Ritorna valore
            return this.IsNull(input) ? null : input;
            
        }


        /// <summary>
        /// Metodo privato di impostazione proprietà
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public override void SetValue(DataObjectBase obj, object value)
        {
            //Se stringa applica regole stringhe
            if (this.HasModifiers && value != null)
            {
                foreach (Attributes.BaseModifierAttribute attr in this.AttrModifiers)
                {
                    value = attr.Modify(value);
                }
            }

            //Di base imposta modifica a true
            bool bChanged = true;

            //Se attivato il real change tracking 
            if (obj.GetSlot().Conf.ChangeTrackingEnabled)
            {
                var oCurrVal = obj.mDataSchema.Values[this.PropertyIndex];

                if (this.AcceptNull && this.IsNull(oCurrVal) && this.IsNull(value))
                    bChanged = false;
                else
                    bChanged = !object.Equals(oCurrVal, value);
            }


            //Se valore modificato (realmente o sempre)
            if (bChanged)
            {
                //Imposta
                obj.mDataSchema.Values[this.PropertyIndex] = value;

                //Property Map
                if (this.HasPropertyMaps)
                {
                    for (int i = 0; i < this.PropertyMap.Count; i++)
                    {
                        //Azzera tutti i mappings dipendenti
                        obj.mDataSchema.Objects[((PropertyObject)this.PropertyMap[i]).ObjectIndex] = null;
                        obj.mDataSchema.SetFlags(this.PropertyMap[i].PropertyIndex, DataFlags.ObjLoaded, false);
                    }
                }

                //Imposta flag modifica solo se true
                obj.mDataSchema.SetFlags(this.PropertyIndex, DataFlags.Changed, true);

                //Lancia evento bindings
                obj.firePropertyChanged(this);
            }

        }

        /// <summary>
        /// Esegue validazione
        /// </summary>
        /// <param name="value"></param>
        public override void PerformValidation(object value)
        {
            if (this.HasValidators)
            {
                foreach (Attributes.BaseValidatorAttribute attr in this.AttrValidators)
                {
                    attr.Validate(this, value);
                }
            }
        }

        /// <summary>
        /// Scrive valore per Xml
        /// </summary>
        /// <param name="xw"></param>
        /// <param name="obj"></param>
        /// <param name="depth"></param>
        public override void WriteXml(XmlWrite xw, DataObjectBase obj, int depth)
        {
            var oTemp = this.GetValue(obj);
            string sValue = string.Empty;
            string sFormat = null;

            //Scrive base 64
            if (this.Type.IsArray)
            {
                sValue = Convert.ToBase64String(oTemp as byte[]);
            }
            else
            {
                if (TypeHelper.IsDecimalType(this.Type))
                    sFormat = string.Intern(obj.GetSlot().Conf.XmlDefaultDecimalFormat);
                else if (TypeHelper.IsDate(this.Type))
                    sFormat = string.Intern(obj.GetSlot().Conf.XmlDefaultDateFormat);

                if (string.IsNullOrEmpty(sFormat))
                    sValue = oTemp.ToString();
                else
                    sValue = (oTemp as IFormattable).ToString(sFormat, System.Globalization.CultureInfo.CurrentCulture);
            }

            xw.WriteElementString(this.Name, sValue);

        }

        /// <summary>
        ///  Scrive DTO
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="obj"></param>
        /// <param name="depth"></param>
        public override void WriteDTO(Dictionary<string, object> dto, DataObjectBase obj, int depth)
        {
            dto.Add(this.Name, this.GetValue(obj));
        }

        /// <summary>
        /// Legge DTO
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="obj"></param>
        public override void ReadDTO(Dictionary<string, object> dto, DataObjectBase obj)
        {
            object o = null;

            if (!dto.TryGetValue(this.Name, out o))
                return;

            this.SetValue(obj, o);

        }


        /// <summary>
        /// Carica valori proprieta' da datareader
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dr"></param>
        public override void SetValueFromReader(DataObjectBase obj, IDataReader dr)
        {
            object oTemp = dr[this.Column.Name];

            if (DBNull.Value.Equals(oTemp) || oTemp == null)
                oTemp = null;
            else
            {
                //VALORE: converte al tipo della proprietà se diverso
                if (this.Type != oTemp.GetType())
                    oTemp = Convert.ChangeType(oTemp, this.Type);
            }

            //imposta Flag caricato
            obj.mDataSchema.SetFlags(this.PropertyIndex, DataFlags.Loaded, true);

            //Se proprieta' e' criptata allora esegue decrypt
            if (oTemp != null && this.mEncAttr != null)
            {
                oTemp = this.mEncAttr.Decrypt(obj.GetSlot(), this, oTemp.ToString());
            }

            //Imposta comunque dato semplice
            obj.mDataSchema.Values[this.PropertyIndex] = oTemp;
        }

        #endregion

    }
}
