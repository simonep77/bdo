using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Indica se una proprieta' deve essere esclusa da operazione di UPDATE (sul db)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcludeFromUpdate : BaseAttribute
    {
    }
}