using System;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Indica il trim del campo
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Trim : BaseStringModifierAttribute
    {
        /// <summary>
        /// Esegue modifica
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object Modify(object value)
        {
            return ((string)value).Trim();
        }

    }
}
