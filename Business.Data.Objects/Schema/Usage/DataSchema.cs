using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Bdo.Objects;
using Bdo.Objects.Base;
using Bdo.Schema.Definition;
using Bdo.Utils;

namespace Bdo.Schema.Usage
{
    /// <summary>
    /// Classe contenente i dati di un oggetto business
    /// </summary>
    internal class DataSchema
    {
        internal object[] Values;

        private DataFlags[] PropFlags;

        internal DataObjectBase[] Objects;

        internal string PkHash;

        internal EObjectState ObjectState = EObjectState.New;

        internal EObjectSource ObjectSource = EObjectSource.None;

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
        internal DataSchema(int PropCount, int ObjCount)
        {
            this.Values = new object[PropCount];
            this.PropFlags = new DataFlags[PropCount];
            this.Objects = new DataObjectBase[ObjCount];
        }


        /// <summary>
        /// Ottien valore di uno o gruppo di flag
        /// </summary>
        /// <param name="index"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        internal bool GetFlagsAll(int index, DataFlags flags)
        {
            return ((this.PropFlags[index] & flags) == flags);
        }

        /// <summary>
        /// Imposta valore flag
        /// </summary>
        /// <param name="index"></param>
        /// <param name="flag"></param>
        /// <param name="value"></param>
        internal void SetFlags(int index, DataFlags flags, bool value)
        {
            if (value)
                //Imposta
                this.PropFlags[index] |= flags;
            else
                //Deimposta
                this.PropFlags[index] &= ~flags;
        }



        /// <summary>
        /// Genera clone del dataschema (per valore)
        /// </summary>
        /// <param name="oSlotIn"></param>
        /// <returns></returns>
        internal DataSchema Clone(bool includeObjects, bool includeKeyHash)
        {
            DataSchema other = new DataSchema(this.Values.Length, this.Objects.Length);

            //Imposta stato
            other.ObjectState = this.ObjectState;
            other.ObjectSource = this.ObjectSource;

            //Se inclusione chiavi
            if (includeKeyHash)
                other.PkHash = this.PkHash;

            //Copia valori
            for (int i = 0; i < this.Values.Length; i++)
            {
                //Copia flags
                other.PropFlags[i] = this.PropFlags[i];

                //Se non dobbiamo includere gli oggetti allora impostiamo come non caricati
                if (!includeObjects)
                    other.SetFlags(i, DataFlags.ObjLoaded, false);

                if (this.Values[i] == null) //NULL
                {
                    continue;
                }
                else if (!(this.Values[i] is Array)) //VALORE
                {
                    //valore UNBOXED
                    other.Values[i] = Convert.ChangeType(this.Values[i], this.Values[i].GetType());
                }
                else //ARRAY
                {
                    //Byte array
                    if (this.Values[i] is byte[])
                    {
                        byte[] arrInput = (byte[])this.Values[i];
                        byte[] arrOutput = new byte[arrInput.Length];
                        Array.Copy(arrInput, arrOutput, arrInput.Length);
                        other.Values[i] = arrOutput;
                    }
                    else
                    {
                        //Errore altri
                        throw new ObjectException("DataSchema: Array di tipo {0} non ammesso!", this.Values[i].GetType().Name);
                    }
                }
            }

            if (includeObjects)
            {
                //Copia oggetti
                for (int i = 0; i < this.Objects.Length; i++)
                {
                    if (this.Objects[i] != null)
                        other.Objects[i] = this.Objects[i].GetSlot().CloneObject(this.Objects[i]);
                }
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
            return ObjectHelper.ObjectArrayToStringRecursive(this.Values);
        }


        #endregion

        #region PRIVATE METHODS



        #endregion

        #region INTERNAL STATIC

        /// <summary>
        /// Serializzazione dataschema
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static byte[] BinSerialize(DataSchema ds)
        {
            //Se schema in input e' null ritorna array vuoto
            if (ds == null)
                return new byte[] { };
            //ATTENZIONE!! L'ordine e' fondamentale!!
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(512))
            {
                SerializationWriter sw = new SerializationWriter(ms);
                sw.WriteObject((byte)ds.ObjectState);
                sw.WriteObject((byte)ds.ObjectSource);
                sw.WriteObject(ds.PropFlags);
                sw.WriteObject(ds.Values);
                sw.WriteObject(ds.PkHash);
                //Gli oggetti non si scrivono per evitare di serializzare grafi troppo profondi
                sw.WriteObject(new DataObjectBase[ds.Objects.Length]);
                //In fase di deserialize si imposta il flag ad off

                return ms.ToArray();
            }

        }

        /// <summary>
        /// Deserializza dataschema
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static DataSchema BinDeserialize(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;

            //ATTENZIONE!! L'ordine e' fondamentale!!
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
            {
                ms.Position = 0;
                SerializationReader sr = new SerializationReader(ms);

                //Crea dataschema
                DataSchema ds = new DataSchema();
                //Legge altri dati
                ds.ObjectState = (EObjectState)sr.ReadObject();
                ds.ObjectSource = (EObjectSource)sr.ReadObject();
                ds.PropFlags = (DataFlags[])sr.ReadObject();
                ds.Values = (object[])sr.ReadObject(); ;
                ds.PkHash = (string)sr.ReadObject();
                ds.Objects = (DataObjectBase[])sr.ReadObject();
                //Imposta flag oggetto a non caricato
                for (int i = 0; i < ds.Values.Length; i++)
                {
                    ds.SetFlags(i, DataFlags.ObjLoaded, false);
                }

                return ds;
            }
        }

        #endregion


    }

}
