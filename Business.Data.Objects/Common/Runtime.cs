using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Objects;
using Business.Data.Objects.Common.Exceptions;

namespace Bdo.Common
{
    /// <summary>
    /// Informazioni di runtime BDO
    /// </summary>
    public class Runtime
    {

        public static readonly Runtime Instance = new Runtime();

        public delegate void BdoExceptionHandler(BdoBaseException e);
        public delegate void BdoSlotEvent(BusinessSlot b);

        /// <summary>
        /// Evento per la cattura (in new) di tutte le eccezioni BDO
        /// </summary>
        public event BdoExceptionHandler OnBdoException;

        internal void TraceBdoException(BdoBaseException e)
        {
            if (this.OnBdoException != null)
                this.OnBdoException(e);
        }
    }
}
