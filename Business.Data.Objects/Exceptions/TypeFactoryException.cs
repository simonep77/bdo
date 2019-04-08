using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects
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
