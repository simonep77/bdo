using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common.Exceptions
{
    /// <summary>
    /// Eccezione da errore slot
    /// </summary>
    public class BusinessSlotException: BdoBaseException 
    {
        /// <summary>
        /// Costruttore base
        /// </summary>
        /// <param name="className"></param>
        /// <param name="msgFormat"></param>
        /// <param name="args"></param>
        public BusinessSlotException(string msgFormat, params object[] args)
            : base(string.Format(msgFormat, args))
        {
        }
    }
}
