using System;
using System.Collections.Generic;
using System.Data;
using Bdo.Utils;
using Bdo.Attributes;
using Bdo.Objects;
using Bdo.Objects.Base;
using Bdo.ObjFactory;

namespace Bdo.Schema.Definition
{
    /// <summary>
    /// Definizione di proprietà
    /// </summary>
    class PropertyDataList : Property
    {

        #region PROPERTIES
        private ClassSchema mDalSchema;
        private Type mDalTypeOrig;
        private ListMap mAttr;

        public override object DefaultValue
        {
            get { return null; }
        }
        
        public override bool IsSqlSelectExcluded
        {
            get
            {
                return true;
            }
        }

        public ClassSchema DalSchema {
            get
            {
                if (this.mDalSchema == null)
                    this.mDalSchema = ProxyAssemblyCache.Instance.GetClassSchema(this.mDalTypeOrig);

                return mDalSchema;
            }
        }


        #endregion

        #region CONSTRUCTORS

        public PropertyDataList(string name, Type type)
            :base(name,type)
        {
        }

        #endregion

        #region PUBLIC

        public override void ValidateDefinition()
        {
            //Check che la classe puntata sia una lista
            if(!this.Type.IsSubclassOf(typeof(DataListBase)))
                throw new SchemaReaderException(this, Resources.SchemaMessages.Prop_Must_Be_List);

            //Check sola lettura
            if (this.HasPropertyMaps && (!this.IsReadonly))
                throw new SchemaReaderException(this, Resources.SchemaMessages.Prop_PropertyMapReadonly);

            ////Check esistenza proprieta'
            //foreach (var item in this.PropertyMap)
            //{
            //    this.mDalTypeOrig.GetProperty(item.Name)
            //    if (!this.DalSchema.Properties.ContainsProperty(item.Name))
            //        throw new SchemaReaderException(@"{0}.{1} - La proprieta' {2} specificata nel mapping deve essere presente nella classe target {3}",
            //            this.Schema.ClassName, this.Name, item.Name, this.mDalTypeOrig.Name);
            //}
        }


        public override bool FillFromAttribute(BaseAttribute attr)
        {
            if (base.FillFromAttribute(attr))
                return true;

            //Property Map
            if (attr is ListMap)
            {
                //Aggiunge il mapping
                var oAttrMap = (ListMap)attr;

                //Se non ci sono nomi errore
                if (oAttrMap.Names == null || oAttrMap.Names.Length == 0)
                    throw new SchemaReaderException(this, Resources.SchemaMessages.Prop_PropertyMapMissingNames);

                //Check classe dal puntata
                var listType = this.Type.BaseType;
                while (!listType.IsGenericType)
                    listType = listType.BaseType;

                this.mDalTypeOrig = listType.GetGenericArguments()[1];
                //Viene modificato il tipo di proprieta'
                this.ExcludeInsert = true;
                this.ExcludeUpdate = true;

                //Salva i nomi delle property target
                this.mAttr = oAttrMap;

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
            //Verifica se gia' impostato a livello di extra data
            var list = obj.GetSlot().ExtraDataGet<DataListBase>(obj, this.Name, null);
            if (list != null)
                return list; //TROVATO

            //NON TROVATO
            list = (DataListBase)ProxyAssemblyCache.Instance.CreateDaoNoSchemaObj(this.Type);
            list.SetSlot(obj.GetSlot());

            IFilter flt = null;

            //Cerca il tipo mappato
            var targetSchema = list.mObjSchema;

            for (int i = 0; i < this.mAttr.Names.Length; i++)
            {
                var targetProp = targetSchema.Properties.GetPropertyByName(this.mAttr.Names[i]);
                var pkVal = obj.mClassSchema.PrimaryKey.Properties[i].GetValue(obj);

                if (flt == null)
                    flt = new FilterEQUAL(targetProp.Column.Name, pkVal);
                else
                    flt.And(new FilterEQUAL(targetProp.Column.Name, pkVal));
            }

            //Invoca ricerca
            list.searchByColumn(flt);

            //Salva risultato
            obj.GetSlot().ExtraDataSet(obj, this.Name, list);

            //Ritorna
            return list;
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
            var oTemp = (DataListBase)this.GetValue(obj);
            xw.WriteStartElement(this.Name);
            try
            {
                xw.WriteRaw((oTemp == null) ? string.Empty : oTemp.ToXml(depth));
            }
            finally
            {
                xw.WriteEndElement();
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

            var oTemp = this.GetValue(obj) as DataListBase;
            dto.Add(this.Name, (oTemp == null) ? null : oTemp.ToDTO(--depth));
        }


        /// <summary>
        /// Legge DTO
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="obj"></param>
        public override void ReadDTO(Dictionary<string, object> dto, DataObjectBase obj)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Carica valori proprieta' da datareader
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dr"></param>
        public override void SetValueFromReader(DataObjectBase obj, IDataReader dr)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
