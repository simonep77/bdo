using System;
using Bdo.Objects;
using Bdo.Objects.Base;

namespace Bdo.Attributes
{
    /// <summary>
    /// Consente di definire la dipendenza di una proprieta' rispetta ad una o piu'
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class ListMap: BaseAttribute 
    {
        private string[] mNames;

        /// <summary>
        /// Indica i nomi delle proprieta' mappate
        /// </summary>
        public string[] Names
        {
            get { return this.mNames; }
        }


        /// <summary>
        /// Crea istanza di PropertyMap specificando Nome ed ordine
        /// </summary>
        /// <param name="propertyNames"></param>
        public ListMap(params string[] propertyNames)
        {
            this.mNames = propertyNames;
        }

    }
}
