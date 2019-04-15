using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Bdo.Objects;
using Bdo.Objects.Base;
using Bdo.Schema.Definition;
using System.Collections;

namespace Bdo.Objects
{
    /// <summary>
    /// Classe astratta di Filter
    /// </summary>
    public abstract class FilterBase: IFilter 
    {
        #region PRIVATE

        private string mName;
        private EOperator mOperator;
        private object mValue;
        private bool mIsValueChecked = false;

        private List<ChainItem> mChain;

        #endregion

        #region PRIVATE

        /// <summary>
        /// Controllo array
        /// </summary>
        /// <param name="filterName"></param>
        /// <param name="values"></param>
        /// <param name="minLen"></param>
        /// <param name="maxLen"></param>
        private void checkValueArray(string filterName, object[] values, int minLen, int maxLen)
        {
            if (values == null)
                throw new FilterException("Valore non fornito (null) per il filtro {0}", filterName);

            if (minLen > 0 && values.Length < minLen)
                throw new FilterException("Il filtro {0} necessita di almeno {1} valori", filterName, minLen);

            if (maxLen > 0 && values.Length > maxLen)
                throw new FilterException("Il filtro {0} non puo' avere piu' di {1} valori", filterName, maxLen);
        }

        /// <summary>
        /// Verifica parametro NULL
        /// </summary>
        /// <param name="filterName"></param>
        /// <param name="value"></param>
        private void checkValueNull(string filterName, object value)
        {
            if (value != null)
                throw new FilterException("Il filtro {0} non necessita un valore", filterName);
        }

        /// <summary>
        /// Verifica valore
        /// </summary>
        /// <param name="filterName"></param>
        /// <param name="value"></param>
        private void checkValueNotNull(string filterName, object value)
        {
            if (value == null)
                throw new FilterException("Il filtro {0} necessita un valore", filterName);
        }

        #endregion

        #region PUBLIC

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        public FilterBase(string name, EOperator op, object value) 
        {
            this.mName = name;
            this.mOperator = op;
            this.mValue = value;

            switch (this.mOperator)
            {
                case EOperator.In:
                    this.checkValueArray("IN", value as object[], 1, 9999);
                    break;
                case EOperator.Between:
                    this.checkValueArray("BETWEEN", value as object[], 2, 2);
                    break;
                case EOperator.IsNull:
                    this.checkValueNull("ISNULL", value);
                    break;
                case EOperator.IsNotNull:
                    this.checkValueNull("ISNOTNULL", value);
                    break;
                case EOperator.Equal:
                case EOperator.Differs:
                    break;
                default:
                    this.checkValueNotNull(this.mOperator.ToString().ToUpper(), value);
                    break;
            }
        }

        /// <summary>
        /// Filter field name
        /// </summary>
        public string  Name
        {
	        get { return this.mName; }
        }

        /// <summary>
        /// Filter oPerator
        /// </summary>
        public EOperator Operator
        {
	        get { return this.mOperator; }
        }

        /// <summary>
        /// Valore del filtro
        /// </summary>
        public object Value
        {
            get { return this.mValue; }
        }


        /// <summary>
        /// Filter AND
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public IFilter And(IFilter f)
        {
            if (this.mChain == null)
                this.mChain = new List<ChainItem>();

            this.mChain.Add(new ChainItem() { IsAnd = true, Filter = f });

            return this;
        }

        /// <summary>
        /// Filter AND con parametri espliciti
        /// </summary>
        /// <param name="name"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IFilter And(string name, EOperator op, object value)
        {
            return this.And(new Filter(name, op, value));
        }

        


        /// <summary>
        /// Filter OR
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public IFilter Or(IFilter f)
        {
            if (this.mChain == null)
                this.mChain = new List<ChainItem>();

            this.mChain.Add(new ChainItem() { IsAnd = false, Filter = f });

            return this;
        }

        /// <summary>
        /// Filtro OR con parametri espliciti
        /// </summary>
        /// <param name="name"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IFilter Or(string name, EOperator op, object value)
        {
            return this.Or(new Filter(name, op, value));
        }


        /// <summary>
        /// Esegue test di confronto su oggetto
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool PropertyTest(DataObjectBase obj)
        {
            bool bTest = false;
            Property oProp = obj.mClassSchema.Properties.GetPropertyByName(this.mName);
            object[] values;

            //Gestione dati interni (solo al primo ciclo di test)
            if (!this.mIsValueChecked)
            {

                if (oProp is PropertySimple)
                {
                    //Converte il valore (se trattasi di valore standard)
                    if (this.mValue != null && !this.mValue.GetType().IsArray)
                        this.mValue = Convert.ChangeType(this.mValue, oProp.Type);
                }
                else if (oProp is PropertyObject)
                {
                    if (this.mValue != null)
                    {
                        if (this.mValue is DataObjectBase)
                        {
                            DataObjectBase o = (DataObjectBase)this.mValue;

                            if (!o.mClassSchema.OriginalType.Equals(oProp.Type))
                                throw new Bdo.Objects.ObjectException("{0} - Errore nella verifica del filtro: la proprieta' e' di tipo {1}, il valore del filtro e' di tipo {2}", oProp.Fullname, oProp.Type.Name, o.mClassSchema.OriginalType.Name);
                        }
                        else
                        { //NON E' UN DATAOBJECT: lo creo solo se non trattasi di una IN
                            if (this.mOperator != EOperator.In)
                            {
                                //Se pk e' array allora esegue il cast e lo passa cosi
                                object[] pk = this.mValue as object[];

                                if (pk == null)
                                    pk = new object[] { this.mValue }; //Crea array PK

                                //Passato valore PK: crea relativo dataobject
                                this.mValue = obj.GetSlot().LoadObjectInternalByKEY(ClassSchema.PRIMARY_KEY, oProp.Type, true, pk);
                            }
                        }
                    }
                
                }

                this.mIsValueChecked = true;
            }

            //Prende il valore della property
            object oValue = oProp.GetValue(obj);

            switch (this.mOperator)
            {
                case EOperator.Equal:
                    bTest = object.Equals(this.mValue, oValue);
                    break;
                case EOperator.Differs:
                    bTest = !object.Equals(this.mValue, oValue);
                    break;
                case EOperator.GreaterThan: //Attenzione gli operatori di confronto sono invertiti poiche' il termine che esegue il confronto e' quello che dobbiamo verificare
                    bTest = (((IComparable)this.mValue).CompareTo(oValue) < 0);
                    break;
                case EOperator.GreaterEquals:
                    bTest = (((IComparable)this.mValue).CompareTo(oValue) <= 0);
                    break;
                case EOperator.LessThan:
                    bTest = (((IComparable)this.mValue).CompareTo(oValue) > 0);
                    break;
                case EOperator.LessEquals:
                    bTest = (((IComparable)this.mValue).CompareTo(oValue) >= 0);
                    break;
                case EOperator.Like:
                    bTest = System.Text.RegularExpressions.Regex.IsMatch(this.mValue.ToString(), oValue.ToString());
                    break;
                case EOperator.NotLike:
                    bTest = !System.Text.RegularExpressions.Regex.IsMatch(this.mValue.ToString(), oValue.ToString());
                    break;
                case EOperator.Between:
                    values = this.mValue as object[];
                    
                    bTest = (((IComparable)values[0]).CompareTo(oValue) <= 0)
                        && (((IComparable)values[1]).CompareTo(oValue) >= 0);

                    break;
                case EOperator.IsNull:
                    bTest = (oValue == null);
                    break;
                case EOperator.IsNotNull:
                    bTest = (oValue != null);
                    break;
                case EOperator.In:
                    values = this.mValue as object[];
                    for (int i = 0; i < values.Length; i++)
                    {
                        //Esegue la conversione di ciascun tipo
                        var oFilt = new FilterEQUAL(this.Name, values[i]);
                        if (oFilt.PropertyTest(obj))
                        {
                            bTest = true;
                            break;
                        }
                    }
                    
                    break;
                default:
                    throw new ArgumentException(Resources.ObjectMessages.Base_OperatorUnknown);
            }

            //Verifica chain
            if (this.mChain != null)
            {
                for (int i = 0; i < this.mChain.Count; i++)
                {
                    if (this.mChain[i].IsAnd)
                        bTest &= this.mChain[i].Filter.PropertyTest(obj);
                    else 
                        bTest |= this.mChain[i].Filter.PropertyTest(obj);
                }
            }

            //Ritorna esito
            return bTest;
        }


        /// <summary>
        /// Appende la clausola generata (SQL + Parametri)
        /// </summary>
        /// <param name="db"></param>
        public void AppendFilterSql(Bdo.Database.IDataBase db, StringBuilder sql, int paramIndex)
        {
            object[] arrValues;
            DbParameter oParam;
            bool bComplex = (this.mChain != null);

            if (bComplex)
                sql.Append(@"(");//Apre statement principale

            //Aertura filtro
            sql.Append(@"(");

            sql.Append(this.mName);
            sql.Append(Utils.ObjectHelper.OperatorToString(this.mOperator));

            switch (this.mOperator)
            {
                case EOperator.IsNull:
                case EOperator.IsNotNull:
                    break;
                case EOperator.Between:
                    arrValues = this.mValue as object[];
                    oParam = db.CreateParameter(string.Concat(this.mName, paramIndex.ToString(), "_1"), arrValues[0]);
                    db.AddParameter(oParam);
                    sql.Append(oParam.ParameterName);
                    sql.Append(@" AND ");

                    oParam = db.CreateParameter(string.Concat(this.mName, paramIndex.ToString(), "_2"), arrValues[1]);
                    db.AddParameter(oParam);
                    sql.Append(oParam.ParameterName);

                    break;
                case EOperator.In:
                    sql.Append(@"(");
                    arrValues = this.mValue as object[];
                    for (int i = 0; i < arrValues.Length; i++)
                    {
                        oParam = db.CreateParameter(string.Concat(this.mName, paramIndex.ToString(), "_", i.ToString()), arrValues[i]);
                        db.AddParameter(oParam);
                        sql.Append(oParam.ParameterName);
                        sql.Append(@", ");
                    }
                    sql.Remove(sql.Length - 2, 2);
                    sql.Append(@")");

                    break;
                default:
                    oParam = db.CreateParameter(string.Concat(this.mName, paramIndex.ToString()), this.mValue);
                    db.AddParameter(oParam);
                    sql.Append(oParam.ParameterName);
                    break;
            }

            //Aggiunge AND
            if (bComplex)
            {
                for (int i = 0; i < this.mChain.Count; i++)
                {
                    sql.Append(this.mChain[i].ToSqlOperator());

                    this.mChain[i].Filter.AppendFilterSql(db, sql, ++paramIndex);
                }
            }


            //Chiusura filtro
            sql.Append(@")");


            //Chiude statement principale
            if (bComplex)
                sql.Append(@")");
        }

        public IEnumerator<IFilter> GetEnumerator()
        {
            yield return this;

            if (this.mChain != null)
            {
                for (int i = 0; i < this.mChain.Count; i++)
                {
                    foreach (var item in this.mChain[i].Filter)
                    {
                        yield return item;
                    }

                }
            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        

        #endregion
    }




    /// <summary>
    /// Classe per la gestione della catena di filtri
    /// </summary>
    public class ChainItem
    {
        public bool IsAnd;
        public IFilter Filter;

        public string ToSqlOperator()
        {
            return (this.IsAnd ? @" AND " : @" OR ");
        }
    }
}
