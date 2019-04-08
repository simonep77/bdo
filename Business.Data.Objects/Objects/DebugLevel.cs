using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects
{
    /// <summary>
    /// Indica il livello di debug dell'informazione trasmessa
    /// per convenzione i livelli più bassi indicano informazioni più di basso livello
    /// </summary>
    public enum DebugLevel: int
    {
        /// <summary>
        /// Debug di dettagli utente 1
        /// </summary>
        User_1 = 1,

        /// <summary>
        /// Debug di dettagli utente 2
        /// </summary>
        User_2 = 2,

        /// <summary>
        /// Debug di dettagli utente 3
        /// </summary>
        User_3 = 3,

        /// <summary>
        /// Debug di dettagli utente 4
        /// </summary>
        User_4 = 4,

        /// <summary>
        /// Debug di dettagli utente 5
        /// </summary>
        User_5 = 5,

        /// <summary>
        /// Debug di dettagli tecnici 1
        /// </summary>
        Technical_1 = 6,

        /// <summary>
        /// Debug di dettagli tecnici 2
        /// </summary>
        Technical_2 = 7,

        /// <summary>
        /// Debug di dettagli tecnici 3
        /// </summary>
        Technical_3 = 8,

        /// <summary>
        /// Debug di dettagli tecnici 4
        /// </summary>
        Technical_4 = 9,

        /// <summary>
        /// Debug di dettagli tecnici 5
        /// </summary>
        Technical_5 = 10,

        /// <summary>
        /// Livello debug 1
        /// </summary>
        Debug_1 = 11,

        /// <summary>
        /// Livello debug 2
        /// </summary>
        Debug_2 = 12,

        /// <summary>
        /// Livello debug 3
        /// </summary>
        Debug_3 = 13,

        /// <summary>
        /// Livello Info 1
        /// </summary>
        Info_1 = 21,

        /// <summary>
        /// Livello Info 2
        /// </summary>
        Info_2 = 22,

        /// <summary>
        /// Livello Info 3
        /// </summary>
        Info_3 = 23,

        /// <summary>
        /// Livello Warning 1
        /// </summary>
        Warn_1 = 31,

        /// <summary>
        /// Livello Warning 2
        /// </summary>
        Warn_2 = 32,

        /// <summary>
        /// Livello Warning 3
        /// </summary>
        Warn_3 = 33,

        /// <summary>
        /// Livello Error 1
        /// </summary>
        Error_1 = 41,

        /// <summary>
        /// Livello Error 2
        /// </summary>
        Error_2 = 42,

        /// <summary>
        /// Livello Error 3
        /// </summary>
        Error_3 = 43
    }
}
