using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Indica che una proprieta' fa parte di una chiave identificata dal nome fornito
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SearchKey: BaseAttribute 
    {
        private string mKeyName;

        /// <summary>
        /// Nome della chiave
        /// </summary>
        public string KeyName
        {
            get { return this.mKeyName; }
        }

        /// <summary>
        /// Imposta chiave di ricerca con nome fornito e operatore di default (=)
        /// </summary>
        /// <param name="keyName"></param>
        public SearchKey(string keyName)
        {
            this.mKeyName = keyName;
        }

    }
}
