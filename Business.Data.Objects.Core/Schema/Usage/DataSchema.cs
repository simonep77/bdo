using Business.Data.Objects.Common;
using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Core.Schema.Definition;
using Business.Data.Objects.Core.Utils;
using System;
using System.Linq;

namespace Business.Data.Objects.Core.Schema.Usage
{
    /// <summary>
    /// Valore di una proprietà BDO
    /// </summary>
    internal class PropValue
    {
        public bool Loaded { get; set; }
        public bool Changed { get; set; }
        public object Value { get; set; }

        public void CopyFrom(PropValue other)
        {
            this.Loaded = other.Loaded;
            this.Changed = other.Changed;

            if (other.Value != null)
            {
                var t = other.GetType();

                if (t.IsValueType || t.IsString())
                    this.Value = other.Value;
                else if (t.IsBdoDalType())
                    this.Loaded = false;
                else if (t.IsByteArray())
                    this.Value = ((Array)other.Value).Clone();
            }
            this.Value = other.Value;
        }
    }

    /// <summary>
    /// Classe contenente i dati di un oggetto business
    /// </summary>
    internal class DataSchema
    {
        private PropValue[] PropValues;

        internal string PkHash;

        internal EObjectState ObjectState = EObjectState.New;

        internal EObjectSource ObjectSource = EObjectSource.None;

        internal ESaveResult SaveResult = ESaveResult.Unset;

        #region PUBLIC METHODS

        /// <summary>
        /// Costruttore privato per gestire in autonomia la deserializzazione
        /// </summary>
        private DataSchema()
        { }



        /// <summary>
        /// Crea dataschema a partire da class schema
        /// </summary>
        /// <param name="schema"></param>
        internal DataSchema(int PropCount)
        {
            this.PropValues = new PropValue[PropCount];
            
            for (int i = 0; i < PropCount; i++)
            {
                this.PropValues[i] = new PropValue();
            }
        }

        /// <summary>
        /// Ritorna propertyvalue by property
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        internal PropValue GetByProperty(Property prop) => this.PropValues[prop.PropertyIndex];

        /// <summary>
        /// Genera clone del dataschema (per valore)
        /// </summary>
        /// <param name="oSlotIn"></param>
        /// <returns></returns>
        internal DataSchema Clone(bool includeKeyHash)
        {
            DataSchema other = new DataSchema(this.PropValues.Length);

            //Imposta stato
            other.ObjectState = this.ObjectState;
            other.ObjectSource = this.ObjectSource;
            other.SaveResult = this.SaveResult;

            //Se inclusione chiavi
            if (includeKeyHash)
                other.PkHash = this.PkHash;

            //Copia valori
            for (int i = 0; i < this.PropValues.Length; i++)
            {
                other.PropValues[i].CopyFrom(this.PropValues[i]);
            }

            //ritorna
            return other;
        }


        /// <summary>
        /// Ritorna la rappresentazione in stringa
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ObjectHelper.ObjectArrayToStringRecursive(this.PropValues.Select(x => x.Value).ToArray());
        }


        #endregion



    }

}
