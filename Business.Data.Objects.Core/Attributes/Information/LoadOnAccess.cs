using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Indica che il valore del campo va caricato dal db solo al primo accesso
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LoadOnAccess : BaseAttribute
    {
    }
}
