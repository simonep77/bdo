using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects
{
    /// <summary>
    /// Enumerazione provenienza oggetto
    /// </summary>
    public enum EObjectSource: byte 
    {
        /// <summary>
        /// Oggetto nuovo
        /// </summary>
        None = 0,

        /// <summary>
        /// Caricato da database
        /// </summary>
        Database = 1,

        /// <summary>
        /// Caricato da cache globale
        /// </summary>
        GlobalCache = 2,

        /// <summary>
        /// Caricato da cache esterna
        /// </summary>
        ExternalCache = 3,

        /// <summary>
        /// Caricato da DTO
        /// </summary>
        DTO = 4,

    }
}
