using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Database;
using Bdo.Objects.Base;
using Bdo.Objects;
using Bdo.Attributes;

namespace Bdo.Schema.Definition
{
    /// <summary>
    /// Definizione di chiave
    /// </summary>
    internal class Key 
    {

        #region FIELDS

        internal string Name { get; set; }
        internal PropertyList Properties { get; set; }
        internal UInt32 HashCode { get; set; }
        internal short KeyIndex { get; set; }
        internal string SQL_Where_Clause { get; set; }

        #endregion

        #region CONSTRUCTORS

        public Key(string name)
        {
            this.Name = name;
            this.Properties = new PropertyList(1);
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Aggiunge proprietà a chiave
        /// </summary>
        /// <param name="oPropIn"></param>
        public void AddProperty(Property oPropIn)
        {
            this.Properties.Add(oPropIn);
        }


        /// <summary>
        /// Imposta i parametri da utilizzare nella where utilizzando quanto gia' presente nell'oggetto di input
        /// </summary>
        /// <param name="dbIn"></param>
        /// <param name="objIn"></param>
        /// <returns>L'array contente i valori della key utilizzati</returns>
        public object[] FillKeyQueryWhereParams(IDataBase dbIn, DataObjectBase objIn)
        {
            return this.FillKeyQueryWhereParams(dbIn, this.GetValuesForDb(objIn));
        }


        /// <summary>
        /// Imposta i parametri da utilizzare nella where utilizzando l'array di input per i valori della key
        /// </summary>
        /// <param name="dbIn"></param>
        /// <param name="keyValuesIn"></param>
        /// <returns>L'array contente i valori della key utilizzati</returns>
        public object[] FillKeyQueryWhereParams(IDataBase dbIn, object[] keyValuesIn)
        {
            for (int i = 0; i < this.Properties.Count; i++)
            {
                var value = keyValuesIn[i];

                //Se oggetto mappato prende il valore dalla PK
                if (value is DataObjectBase)
                {
                    var bdoValue = (DataObjectBase)value;
                    value = ((DataObjectBase)value).mClassSchema.PrimaryKey.GetValuesForDb(bdoValue)[0];
                }
                     
                dbIn.AddParameter(this.Properties[i].Column.GetKeyParamName(), value);
            }

            return keyValuesIn;
        }


        /// <summary>
        /// Ritorna array con dati
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="forDbIn"></param>
        /// <returns></returns>
        public object[] GetValues(DataObjectBase obj)
        {
            object[] oRetArr = new object[this.Properties.Count];

            for (int i = 0; i < this.Properties.Count; i++)
            {
                oRetArr[i] = this.Properties[i].GetValue(obj);
            }

            return oRetArr;
        }

        /// <summary>
        /// Ritorna array con dati per db
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object[] GetValuesForDb(DataObjectBase obj)
        {
            object[] oRetArr = new object[this.Properties.Count];

            for (int i = 0; i < this.Properties.Count; i++)
            {
                oRetArr[i] = this.Properties[i].GetValueForDb(obj);
            }

            return oRetArr;
        }


        #endregion

    }
}
