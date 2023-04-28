using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Attributes.Structure
{
    /// <summary>
    /// Indica che la classe � soggetta a storicizzazione
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class History : BaseAttribute
    {
    }
}
