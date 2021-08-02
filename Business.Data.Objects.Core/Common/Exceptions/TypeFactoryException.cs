using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common.Exceptions
{
    /// <summary>
    /// Eccezione da errore schema
    /// </summary>
    public class TypeFactoryException: BdoBaseException 
    {
        public TypeFactoryException(string msgFormat, params object[] args)
            : base(string.Format(msgFormat, args))
        {
        }
    }
}
