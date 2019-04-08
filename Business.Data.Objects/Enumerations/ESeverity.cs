using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects
{
    /// <summary>
    /// Definisce il livello di severità di un messaggio
    /// </summary>
    public enum ESeverity
    {
        /// <summary>
        /// Solo a scopo di debug
        /// </summary>
        Debug = 0,

        /// <summary>
        /// Informazione
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warning
        /// </summary>
        Warn = 2,

        /// <summary>
        /// Errore
        /// </summary>
        Error = 3
    }
   
}
