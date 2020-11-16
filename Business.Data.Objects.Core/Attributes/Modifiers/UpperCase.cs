using System;

namespace Business.Data.Objects.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UpperCase : BaseStringModifierAttribute
    {
        public override object Modify(object value)
        {
            return ((string)value).ToUpper();
        }
    }
}
