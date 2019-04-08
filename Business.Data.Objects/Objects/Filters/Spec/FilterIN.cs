using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Bdo.Schema.Definition;
using Bdo.Objects.Base;

namespace Bdo.Objects
{
    /// <summary>
    /// Filtro IN
    /// </summary>
    public class FilterIN: FilterBase 
    {

        /// <summary>
        /// Crea un filtro in a partire da un qualunque tipo di array definito
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        public FilterIN(string fieldName, Array values)
        : base(fieldName, EOperator.In, Utils.ObjectHelper.ToObjectArray(values) )
            {
            if (values == null || values.Length == 0)
                throw new ArgumentException("Il filtro IN deve contenere almeno un valore");
        }



        /// <summary>
        /// Crea filtro in a partire da qualunque enumerabile
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        public FilterIN(string fieldName, IEnumerable values)
        : base(fieldName, EOperator.In, Utils.ObjectHelper.ToObjectArray(values) )
        {
            if (values == null)
                throw new ArgumentException("Il filtro IN deve contenere almeno un valore");

        }


        /// <summary>
        /// FiltroIN con 1 valore
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="val1"></param>
        public FilterIN(string fieldName, object val1)
            : base(fieldName, EOperator.In, new object[] { val1 })
        {
        }


        /// <summary>
        /// FiltroIN con 2 valori
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        public FilterIN(string fieldName, object val1, object val2)
            : base(fieldName, EOperator.In, new object[] { val1, val2 })
        {
        }

        /// <summary>
        /// FiltroIN con 3 valori
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="val3"></param>
        public FilterIN(string fieldName, object val1, object val2, object val3)
            : base(fieldName, EOperator.In, new object[] { val1, val2, val3 })
        {
        }

        /// <summary>
        /// FiltroIN con 4 valori
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="val3"></param>
        /// <param name="val4"></param>
        public FilterIN(string fieldName, object val1, object val2, object val3, object val4)
            : base(fieldName, EOperator.In, new object[] { val1, val2, val3, val4 })
        {
        }


        /// <summary>
        /// FiltroIN con 5 valori
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="val3"></param>
        /// <param name="val4"></param>
        /// <param name="val5"></param>
        public FilterIN(string fieldName, object val1, object val2, object val3, object val4, object val5)
    : base(fieldName, EOperator.In, new object[] { val1, val2, val3, val4, val5 })
        {
        }


        /// <summary>
        /// FiltroIN con 6 valori
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="val3"></param>
        /// <param name="val4"></param>
        /// <param name="val5"></param>
        /// <param name="val6"></param>
        public FilterIN(string fieldName, object val1, object val2, object val3, object val4, object val5, object val6)
: base(fieldName, EOperator.In, new object[] { val1, val2, val3, val4, val5, val6 })
        {
        }


        /// <summary>
        ///  FiltroIN con 7 valori
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="val3"></param>
        /// <param name="val4"></param>
        /// <param name="val5"></param>
        /// <param name="val6"></param>
        /// <param name="val7"></param>
        public FilterIN(string fieldName, object val1, object val2, object val3, object val4, object val5, object val6, object val7)
: base(fieldName, EOperator.In, new object[] { val1, val2, val3, val4, val5, val6, val7 })
        {
        }


        /// <summary>
        /// FiltroIN con 8 valori
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="val3"></param>
        /// <param name="val4"></param>
        /// <param name="val5"></param>
        /// <param name="val6"></param>
        /// <param name="val7"></param>
        /// <param name="val8"></param>
        public FilterIN(string fieldName, object val1, object val2, object val3, object val4, object val5, object val6, object val7, object val8)
: base(fieldName, EOperator.In, new object[] { val1, val2, val3, val4, val5, val6, val7, val8 })
        {
        }

        /// <summary>
        /// FiltroIN con 9 valori
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="val3"></param>
        /// <param name="val4"></param>
        /// <param name="val5"></param>
        /// <param name="val6"></param>
        /// <param name="val7"></param>
        /// <param name="val8"></param>
        /// <param name="val9"></param>
        public FilterIN(string fieldName, object val1, object val2, object val3, object val4, object val5, object val6, object val7, object val8, object val9)
: base(fieldName, EOperator.In, new object[] { val1, val2, val3, val4, val5, val6, val7, val8, val9 })
        {
        }


        /// <summary>
        /// FiltroIN con 10 valori
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="val3"></param>
        /// <param name="val4"></param>
        /// <param name="val5"></param>
        /// <param name="val6"></param>
        /// <param name="val7"></param>
        /// <param name="val8"></param>
        /// <param name="val9"></param>
        /// <param name="val10"></param>
        public FilterIN(string fieldName, object val1, object val2, object val3, object val4, object val5, object val6, object val7, object val8, object val9, object val10)
: base(fieldName, EOperator.In, new object[] { val1, val2, val3, val4, val5, val6, val7, val8, val9, val10 })
        {
        }

        #region UTILITY


        #endregion

    }
}
