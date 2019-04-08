using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Indica se una proprieta' deve essere esclusa da operazione di INSERT (sul db)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcludeFromInsert : BaseAttribute 
    {
    }
}
