using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
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
