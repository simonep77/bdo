using System;
using System.Collections.Generic;

namespace Bdo.Schema.Definition
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
