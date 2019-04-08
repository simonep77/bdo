using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Bdo.Schema.Definition;
using Bdo.Objects.Base;

namespace Bdo.Objects
{
    /// <summary>
    /// Filtro ISNOTNULL
    /// </summary>
    public class FilterISNOTNULL : FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterISNOTNULL(string fieldName)
            :base(fieldName, EOperator.IsNotNull, null)
        {
        }


    }
}
