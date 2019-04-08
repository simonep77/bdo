using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Attributo che indica se l'oggetto deve essere mantenuto nella cache della sessione
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
    public class GlobalCache : BaseAttribute
    {
      
    }
}
