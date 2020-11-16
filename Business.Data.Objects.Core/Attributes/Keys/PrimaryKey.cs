using Business.Data.Objects.Core.Schema.Definition;
using System;

namespace Business.Data.Objects.Core.Attributes
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
