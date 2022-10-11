using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Common
{
    /// <summary>
    /// Definisce il livello di severità di un messaggio
    /// </summary>
    public enum ESaveResult
    {
        /// <summary>
        /// Risultato non noto
        /// </summary>
        Unset = 0,

        /// <summary>
        /// Eseguito salvataggio
        /// </summary>
        SaveDone = 1,

        /// <summary>
        /// Salvataggio non effettuato per oggetto non modificato
        /// </summary>
        UnChanged = 2
    }
   
}
