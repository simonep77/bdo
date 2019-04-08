using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Common
{
    /// <summary>
    /// Costanti applicazione
    /// </summary>
    internal sealed class Constants
    {
    
        /// <summary>
        /// Nome libreria
        /// </summary>
        public const string STR_LIB_NAME = @"Business Data Objects (Free Library)";

        public const string STR_LIB_AUTHOR = @"Simone Pelaia";

        public const string STR_LIB_EMAIL = @"simone.pelaia@gmail.com";

        /// <summary>
        /// Stringa che identifica il contesto Database di default
        /// </summary>
        public const string STR_DB_DEFAULT = @"DEFAULT";


        /// <summary>
        /// Array di caratteri da utilizzare come split per stringhe (, e ;)
        /// </summary>
        public static readonly char[] ARR_DEFAULT_SPLIT_CHARS = { ',', ';' };

        /// <summary>
        /// Separatore LOG
        /// </summary>
        public static readonly string LOG_SEPARATOR = string.Empty.PadRight(210, '=');

    }
}
