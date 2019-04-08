using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects
{
    /// <summary>
    /// Eccezioni generata se la validazione non è corretta
    /// </summary>
    public class ObjectValidationException: ObjectException  
    {
        public ObjectValidationException(string msgFormat, params object[] args)
            : base(msgFormat, args)
        {
        }
    }
}
