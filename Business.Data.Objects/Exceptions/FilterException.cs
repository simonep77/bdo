using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects
{
    /// <summary>
    /// Eccezione da errore Filter
    /// </summary>
    public class FilterException: BdoBaseException 
    {
        public FilterException(string msgFormat, params object[] args)
            : base(string.Format(msgFormat, args))
        {
        }
    }
}
