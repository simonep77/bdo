using System;
using System.Globalization;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Consente di definire il valore utilizzato come default.
    /// Note: 
    /// in caso di data il formato e' dd/mm/yyyy ed opzionalmente hh:mm:ss.
    /// in caso di numero decimale/double, ecc deve essere utilizzato il punto come separatore decimale. (es. 13.74)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class DefaultValue: BaseAttribute 
    {
        private static CultureInfo _CultUniv;
        private string mValue;
        
        /// <summary>
        /// Valore impostato
        /// </summary>
        public string Value
        {
            get
            {
                return this.mValue;
            }
        }

        static DefaultValue()
        {
            _CultUniv = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            _CultUniv.DateTimeFormat.ShortDatePattern = @"dd/MM/yyyy";
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="value"></param>
        public DefaultValue(string value)
        {
            this.mValue = value;
        }

        /// <summary>
        /// Converte il valore da stringa al tipo desiderato
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal object ConvertTo(Type type)
        {
            return Convert.ChangeType(this.mValue, type, _CultUniv);
        }

    }
}
