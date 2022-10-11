using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Attributes;
using Business.Data.Objects.Core.Base;
using System;
using System.Collections.Generic;
using System.Data;

namespace Business.Data.Objects.Core.Schema.Definition
{

    /// <summary>
    /// Definizione di propriet�
    /// </summary>
    abstract class Property 
    {

        #region FIELDS

        //Schema comuni
        public ClassSchema Schema { get; set; }
        public short PropertyIndex { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public Column Column { get; set; }
        public PropertyList PropertyMap { get; set; }

        public bool IsReadonly;
        public bool IsAutomatic;
        public bool AcceptNull;
        public bool LoadOnAccess;
        public bool ExcludeInsert;
        public bool ExcludeUpdate;

        public DbType? CustomDbType { get; set; }

        //public bool Encrypted;


        #endregion

        #region PROPERTIES

        public abstract object DefaultValue { get; protected set; }

        /// <summary>
        /// Indica se presenti property map
        /// </summary>
        public bool HasPropertyMaps => this.PropertyMap != null && this.PropertyMap.Count > 0;

        /// <summary>
        /// Indica se la proprieta' va inclusa nella query di selezione standard
        /// </summary>
        public abstract bool ExcludeSelect { get; }

        /// <summary>
        /// Ritorna il nomeclasse.nomeproprieta'
        /// </summary>
        public string Fullname => string.Concat(this.Schema.ClassName, @".", this.Name);


        #endregion

        #region CONSTRUCTORS

        public Property(string name, Type type)
        {
            this.Name = string.Intern(name);
            this.Type = type;
        }


        #endregion

        #region PUBLIC


        public override string ToString() => string.Concat(this.Name, @" (type: ", this.GetType().Name, @")");


        /// <summary>
        /// Esegue validazione formale di tutti i valori immessi
        /// </summary>
        public abstract void ValidateDefinition();


        /// <summary>
        /// Esegue la validazione secondo le regole impostate
        /// </summary>
        /// <param name="value"></param>
        public virtual void PerformValidation(object value)
        { }

        /// <summary>
        /// Imposta valori a partire da attributo
        /// </summary>
        /// <param name="attr"></param>
        public virtual bool FillFromAttribute(BaseAttribute attr)
        {
            bool bRet = true;
            //Colonne
            if (attr is Column)
            {
                //Aggiunge un campo verificando l'ordinamento
                this.Column = (Column)attr;
            }
            //ACCETTA NULL
            else if (attr is AcceptNull)
            {
                this.AcceptNull = true;
            }
            //DB Provider Type
            else if (attr is CustomDbType)
            {
                this.CustomDbType = ((CustomDbType)attr).Value;
            }
            //ESCLUDE DA INSERT
            else if (attr is ExcludeFromInsert)
            {
                this.ExcludeInsert = true;
            }
            //ESCLUDE DA UPDATE
            else if (attr is ExcludeFromUpdate)
            {
                this.ExcludeUpdate = true;
            }
            else
            {
                bRet = false;
            }

            return bRet;
        }

        public abstract object GetValue(DataObjectBase obj);

        public virtual object GetValueForDb(DataObjectBase obj)
        {
            throw new NotImplementedException(string.Format(@"{0} - GetValueForDb non supportata per il tipo di proprieta'", this.Fullname));
        }

        /// <summary>
        /// Indica se il valore della proprieta' e' considerabile Null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsNull(object value)
        {
            //Se null e' nulla
            if (value == null)
                return true;

            //Se accetta null e valore uguale default lo dobbiamo considerare null 
            if (this.AcceptNull && object.Equals(value, this.DefaultValue))
                return true;

            //Tutto il resto e' false
            return false;
        }

        public abstract void SetValue(DataObjectBase obj, object value);

        public abstract void SetValueFromReader(DataObjectBase obj, IDataReader dr);



        #endregion

    }
}
