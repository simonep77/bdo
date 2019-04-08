using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects
{
    /// <summary>
    /// Classe che identifica una differenza di campo db tra
    /// due oggetti
    /// </summary>
    public class DataDiff
    {
        private string mFieldName;
        private object mSourceValue;
        private object mOtherValue;

        #region PROPERTY

        /// <summary>
        /// Nome Campo Modificato
        /// </summary>
        public string FieldName
        {
            get
            {
                return this.mFieldName;
            }
        }

        /// <summary>
        /// Valore dell'oggetto di cui si richiede la verifica
        /// </summary>
        public object SourceValue
        {
            get
            {
                return this.mSourceValue;
            }
        }

        /// <summary>
        /// Valore dell'oggetto di raffronto
        /// </summary>
        public object OtherValue
        {
            get
            {
                return this.mOtherValue;
            }
        }

        #endregion

        #region CONSTRUCTOR

        public DataDiff( string fieldName, ref object srcValue, ref object otherValue )
        {
            this.mFieldName = fieldName;
            this.mSourceValue = srcValue;
            this.mOtherValue = otherValue;
        }

        #endregion
    }
}
