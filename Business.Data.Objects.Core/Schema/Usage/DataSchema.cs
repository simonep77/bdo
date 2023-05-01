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

    internal class PropValue
    {
        public bool Loaded { get; set; }
        public bool Changed { get; set; }
        public object Value { get; set; }

        public void CopyFrom(PropValue other, bool includeDal)
        {
            this.Loaded = other.Loaded;
            this.Changed = other.Changed;

            if (other.Value != null)
            {
                if (!(other.Value is Array))
                {
                    if (!(other.Value is DataObjectBase))
                    {
                        this.Value = other.Value;
                    }
                    else
                    {
                        if (includeDal)
                            this.Value = ((SlotAwareObject)other.Value).GetSlot().CloneObject((DataObjectBase)other.Value);
                        else
                            this.Loaded = false;
                    }
                }
                else
                {
                    this.Value = ((Array)other.Value).Clone();
                }

            }
            this.Value = other.Value;
        }
    }

    /// <summary>
    /// Classe contenente i dati di un oggetto business
    /// </summary>
    internal class DataSchema
    {
        internal PropValue[] PropValues;


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
            this.PropValues.Initialize();
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
        internal DataSchema Clone(bool includeObjects, bool includeKeyHash)
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
                
                other.PropValues[i].CopyFrom(this.PropValues[i], includeObjects);
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
