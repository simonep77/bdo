using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Indica che verranno rimossi i caratteri accentati
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RemoveAccents : BaseStringModifierAttribute
    {

        public override object Modify(object value)
        {
            return Bdo.Utils.StringHelper.RemoveAccents((string)value);
        }

    }
}
