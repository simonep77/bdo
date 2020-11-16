using Business.Data.Objects.Common.Utils;
using System;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Indica che verranno rimossi i caratteri accentati
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RemoveAccents : BaseStringModifierAttribute
    {

        public override object Modify(object value)
        {
            return StringHelper.RemoveAccents((string)value);
        }

    }
}
