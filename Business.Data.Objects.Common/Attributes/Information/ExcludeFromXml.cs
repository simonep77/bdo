using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Indica che il campo non deve essere incluso nell'output XML
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcludeFromXml : BaseAttribute
    {
    }
}
