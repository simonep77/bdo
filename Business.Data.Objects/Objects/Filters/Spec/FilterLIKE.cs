using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Bdo.Schema.Definition;
using Bdo.Objects.Base;

namespace Bdo.Objects
{
    /// <summary>
    /// Filtro LIKE
    /// </summary>
    public class FilterLIKE: FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterLIKE(string fieldName, object fieldValue)
            :base(fieldName, EOperator.Like, fieldValue)
        {
        }
        
    }
}
