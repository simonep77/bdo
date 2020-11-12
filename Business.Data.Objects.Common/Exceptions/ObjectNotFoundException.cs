using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common.Exceptions
{
    /// <summary>
    /// Eccezioni generata se l'oggetto richiesto non è stato trovato
    /// </summary>
    public class ObjectNotFoundException: ObjectException  
    {
        public ObjectNotFoundException(string msgFormat, params object[] args)
            : base(msgFormat, args)
        {
        }
    }
}
