using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Indica che una proprieta' e' un campo con generazione automatica di ID
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutoIncrement : AutomaticField 
    {
    }
}
