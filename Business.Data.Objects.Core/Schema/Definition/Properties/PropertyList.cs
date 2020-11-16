using System;
using System.Collections.Generic;

namespace Business.Data.Objects.Core.Schema.Definition
{
    /// <summary>
    /// Elenco Proprietà
    /// </summary>
    internal class PropertyList : List<Property>
    {

        internal PropertyList(int capacity)
            :base(capacity)
        {
        }

     
    }
}
