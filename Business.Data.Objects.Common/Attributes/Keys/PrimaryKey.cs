using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Definizione di proprieta PrimaryKey
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class PrimaryKey: SearchKey   
    {
        /// <summary>
        /// Costruttore
        /// </summary>
        public PrimaryKey()
            : base(ClassSchema.PRIMARY_KEY)
        {  }

    }
}
