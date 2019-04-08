using System;
using System.Collections.Generic;
using System.Text;

namespace Bdo.Attributes
{
    /// <summary>
    /// Se la lunghezza supera quella impostata il dato viene troncato
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PadRight : BaseStringModifierAttribute
    {
        private int mLength;
        private char mPadChar= ' ';
        public PadRight(int length, char padChar)
        {
            if (length <= 0)
                throw new ArgumentException("PadRight - La lunghezza deve essere maggiore di zero");

            this.mLength = length;
            this.mPadChar = padChar;
        }
        public override object Modify(object value)
        {
            string s = (string)value;

            if (string.IsNullOrEmpty(s))
                return s;

            return s.PadRight(this.mLength, this.mPadChar);
        }
    }
}
