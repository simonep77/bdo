using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Indica che una proprieta' e' un campo che viene aggiornato sempre con lo username impostato nello slot
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Username : BaseAttribute 
    {
    }
}
