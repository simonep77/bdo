using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Business.Data.Objects.Core.Schema.Definition;
using Business.Data.Objects.Core.Base;
using System.Collections;

namespace Business.Data.Objects.Core
{
    /// <summary>
    /// Classe base da utilizzare per applicare/verificare filtri
    /// sia a livello di oggetto (Proprieta') che direttamente su query
    /// </summary>
    public class Filter: FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public Filter(string fieldName, EOperator op, object fieldValue)
            :base(fieldName, op, fieldValue)
        {
        }

        #region COSTRUTTORI STATICI SEMPLIFICATI

        /// <summary>
        /// Crea un filtro EQUAL
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IFilter Eq(string fieldName, object value)
        {
            return new FilterEQUAL(fieldName, value);
        }

        /// <summary>
        /// Crea un filtro LessThen
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IFilter Lt(string fieldName, object value)
        {
            return new FilterLESS(fieldName, value);
        }

        /// <summary>
        /// Crea un filtro LessThenEqual
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IFilter Lte(string fieldName, object value)
        {
            return new FilterLESSEQ(fieldName, value);
        }

        /// <summary>
        /// Crea filtro differs
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IFilter Neq(string fieldName, object value)
        {
            return new FilterDIFFERS(fieldName, value);
        }

        /// <summary>
        /// Crea filtro Greater
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IFilter Gt(string fieldName, object value)
        {
            return new FilterGREATER(fieldName, value);
        }

        /// <summary>
        /// Crea filtro GreaterEQ
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IFilter Gte(string fieldName, object value)
        {
            return new FilterGREATEREQ(fieldName, value);
        }

        /// <summary>
        /// Crea filtro Between
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="valueA"></param>
        /// <param name="valueB"></param>
        /// <returns></returns>
        public static IFilter Betw(string fieldName, object valueA, object valueB)
        {
            return new FilterBETWEEN(fieldName, valueA, valueB);
        }

        /// <summary>
        /// Crea Filtro IsNull
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IFilter IsNull(string fieldName)
        {
            return new FilterISNULL(fieldName);
        }

        /// <summary>
        /// Crea filtro IsNotNull
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IFilter NotNull(string fieldName)
        {
            return new FilterISNOTNULL(fieldName);
        }

        /// <summary>
        /// Crea filtro Like
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IFilter Like(string fieldName, object value)
        {
            return new FilterLIKE(fieldName, value);
        }

        /// <summary>
        /// Crea filtro NotLike
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IFilter NotLike(string fieldName, object value)
        {
            return new FilterNOTLIKE(fieldName, value);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, object value)
        {
            return new FilterIN(fieldName, value);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, object value1, object value2)
        {
            return new FilterIN(fieldName, value1, value2);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, object value1, object value2, object value3)
        {
            return new FilterIN(fieldName, value1, value2, value3);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, object value1, object value2, object value3, object value4)
        {
            return new FilterIN(fieldName, value1, value2, value3, value4);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="value5"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, object value1, object value2, object value3, object value4, object value5)
        {
            return new FilterIN(fieldName, value1, value2, value3, value4, value5);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="value5"></param>
        /// <param name="value6"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, object value1, object value2, object value3, object value4, object value5, object value6)
        {
            return new FilterIN(fieldName, value1, value2, value3, value4, value5, value6);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="value5"></param>
        /// <param name="value6"></param>
        /// <param name="value7"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, object value1, object value2, object value3, object value4, object value5, object value6, object value7)
        {
            return new FilterIN(fieldName, value1, value2, value3, value4, value5, value6, value7);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="value5"></param>
        /// <param name="value6"></param>
        /// <param name="value7"></param>
        /// <param name="value8"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, object value1, object value2, object value3, object value4, object value5, object value6, object value7, object value8)
        {
            return new FilterIN(fieldName, value1, value2, value3, value4, value5, value6, value7, value8);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="value5"></param>
        /// <param name="value6"></param>
        /// <param name="value7"></param>
        /// <param name="value8"></param>
        /// <param name="value9"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, object value1, object value2, object value3, object value4, object value5, object value6, object value7, object value8, object value9)
        {
            return new FilterIN(fieldName, value1, value2, value3, value4, value5, value6, value7, value8, value9);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="value5"></param>
        /// <param name="value6"></param>
        /// <param name="value7"></param>
        /// <param name="value8"></param>
        /// <param name="value9"></param>
        /// <param name="value10"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, object value1, object value2, object value3, object value4, object value5, object value6, object value7, object value8, object value9, object value10)
        {
            return new FilterIN(fieldName, value1, value2, value3, value4, value5, value6, value7, value8, value9, value10);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, Array values)
        {
            return new FilterIN(fieldName, values);
        }

        /// <summary>
        /// Crea filtro IN
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IFilter In(string fieldName, IEnumerable values)
        {
            return new FilterIN(fieldName, values);
        }

        #endregion

    }
}
