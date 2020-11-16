using System;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Il campo viene impostato con caratteri minuscoli
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LowerCase: BaseStringModifierAttribute
    {
        /// <summary>
        /// Esegue modifica
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object Modify(object value)
        {
            return ((string)value).ToLower();
        }
    }
}
