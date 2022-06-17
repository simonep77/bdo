using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Base
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
        public string ToJSON() => Utils.JSONWriter.ToJson(this);

    }
}
