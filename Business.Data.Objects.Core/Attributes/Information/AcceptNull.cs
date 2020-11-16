using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Indica che il campo accetta valore NULL (gestito come defaultvalue)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AcceptNull : BaseAttribute
    {
    }
}
