using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Objects;
using Bdo.Utils;

namespace Bdo.Attributes
{
    /// <summary>
    /// Range di valori per proprietà integer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class IntRange: BaseValidatorAttribute  
    {
        public readonly int From;
        public readonly int To;

        internal override bool CanApplyToProperty(Schema.Definition.Property propIn)
        {
            return TypeHelper.IsIntegerType(propIn.Type);
        }

        public IntRange(int from, int to, string customMessage)
            :base(customMessage)
        {
            this.From = from;
            this.To = to;
        }

        public IntRange(int from, int to)
            : this(from, to, Resources.ObjectMessages.Validate_IntRange)
        {
        }

        internal override void Validate(Schema.Definition.Property propIn, object value)
        {
            int iVal = Convert.ToInt32(value);

            if (iVal < this.From || iVal > this.To)
                throw new ObjectException(this.CustomMessage, propIn.Schema.ClassName, propIn.Name, this.From, this.To);
        }

    }
}
