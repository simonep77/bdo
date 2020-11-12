using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Indica che una proprieta' e' un campo con gestione automatica
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BaseAutomaticAttribute: BaseAttribute 
    {
    }
}
