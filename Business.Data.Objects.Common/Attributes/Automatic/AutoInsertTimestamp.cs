using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Indica che il valore della proprieta' viene generato in fase di inserimento.
    /// E' possibile utilizzarlo su proprieta' semplici
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutoInsertTimestamp : BaseAutomaticAttribute 
    {
    }
}

