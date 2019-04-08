using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Bdo.Attributes;
using Bdo.Schema.Definition;

namespace Bdo.Utils
{
    /// <summary>
    /// Classe con funzioni di utilita' per Proprieta'
    /// </summary>
    public static class PropertyHelper
    {

        /// <summary>
        /// Ritorna il valore base di un tipo.
        /// Attenzione!!! Vengono valorizzari solo i tipi numerici, string e DateTime.
        /// Tutto il resto torna null
        /// </summary>
        /// <param name="aType"></param>
        /// <returns></returns>
        public static object GetDefaultValue(Type aType)
        {
            if (aType.IsValueType)
            {
                return Activator.CreateInstance(aType);
            }
            else if (aType == typeof(string))
            {
                return string.Empty;
            }
            else if (aType.IsEnum)
            {
                return Enum.GetValues(aType).GetValue(0);
            }
            else
            {
                return null;
            }
        }



    }


}
