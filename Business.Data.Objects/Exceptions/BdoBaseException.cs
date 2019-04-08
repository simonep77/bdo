using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Common;

namespace Bdo.Objects
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
            Runtime.Instance.TraceBdoException(this);
        }

        public BdoBaseException()
        {
            Runtime.Instance.TraceBdoException(this);
        }

    }
}
