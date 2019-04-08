using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Bdo.Schema.Definition;
using Bdo.Objects.Base;

namespace Bdo.Objects
{
    /// <summary>
    /// Filtro GREATER EQUAL
    /// </summary>
    public class FilterGREATEREQ : FilterBase 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="op"></param>
        /// <param name="propValue"></param>
        public FilterGREATEREQ(string fieldName, object fieldValue)
            :base(fieldName, EOperator.GreaterEquals, fieldValue)
        {
        }


    }
}
