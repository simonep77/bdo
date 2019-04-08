using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects
{
    /// <summary>
    /// Eccezioni generate dall'oggetto
    /// </summary>
    [Serializable]
    public class ObjectException: BdoBaseException 
    {
        public ObjectException(string msgFormat, params object[] args)
            : base(string.Format(msgFormat, args))
        {
        }
    }
}
