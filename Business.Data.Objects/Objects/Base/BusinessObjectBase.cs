using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects.Base
{
    /// <summary>
    /// Classe base per oggetti Business
    /// </summary>
    public abstract class BusinessObjectBase: SlotAwareObject
    {

        /// <summary>
        /// Ritorna rappresentazione JSON
        /// </summary>
        /// <returns></returns>
        public string ToJSON()
        {
            return Utils.JSONWriter.ToJson(this);
        }

    }
}
