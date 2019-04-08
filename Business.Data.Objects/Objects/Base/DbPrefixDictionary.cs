using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Objects.Base
{
    /// <summary>
    /// Dizionario specifico per i nomi di db. I metodi add e this.set normalizzano il nome db aggiungendo il carattere . terminale
    /// </summary>
    public class DbPrefixDictionary: Dictionary<string, string>
    {
        private const char C_POINT = '.';

        /// <summary>
        /// Get or set il dbname in base alla key specificata
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new string this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                base[key] = this.dbNameNormalize(value);
            }
        }

        /// <summary>
        /// Aggiunge Dbname identificato da chiave
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(string key, string value)
        {
            base.Add(key, this.dbNameNormalize(value));
        }

        /// <summary>
        /// Normalizza il nome DB
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string dbNameNormalize(string name)
        {
            return string.Concat(name.TrimEnd(C_POINT), C_POINT);
        }

    }
}
