using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common.Exceptions
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
