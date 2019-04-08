using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Indica che il valore della proprieta' (DateTime) viene generato in fase di inserimento ed aggiornato ad ogni update.
    /// E' possibile utilizzarlo su proprieta' semplici
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutoUpdateTimestamp : BaseAutomaticAttribute
    {
    }
}
