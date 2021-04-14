using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Indica che una proprieta' e' un campo che viene aggiornato in caso di cancellazione dell'oggetto
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class LogicalDelete : BaseAttribute 
    {


    }
}
