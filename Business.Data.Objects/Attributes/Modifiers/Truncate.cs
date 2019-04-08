using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Se la lunghezza supera quella impostata il dato viene troncato
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Truncate : BaseStringModifierAttribute
    {
        private int mLength;
        public Truncate(int length)
        {
            if (length <= 0)
                throw new ArgumentException("Truncate - La lunghezza deve essere maggiore di zero");

            this.mLength = length;
        }
        public override object Modify(object value)
        {
            string s = (string)value;
            if (!string.IsNullOrEmpty(s) && s.Length > this.mLength)
                s = s.Substring(0, this.mLength);

            return s;
        }
    }
}
