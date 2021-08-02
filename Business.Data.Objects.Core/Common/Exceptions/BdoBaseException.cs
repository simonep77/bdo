using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common.Exceptions
{
    /// <summary>
    /// Eccezione base BDO
    /// </summary>
    [Serializable]
    public class BdoBaseException: ApplicationException 
    {
        public BdoBaseException(string msgFormat, params object[] args)
            : base(string.Format(msgFormat, args))
        {
        }

        public BdoBaseException()
        {
        }

    }
}
