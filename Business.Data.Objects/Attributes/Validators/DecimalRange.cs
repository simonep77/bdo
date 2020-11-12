using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Objects;
using Bdo.Utils;
using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Common.Utils;

namespace Bdo.Attributes
{
    /// <summary>
    /// Range di valori per proprietà integer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class DecimalRange: BaseValidatorAttribute  
    {
        public readonly decimal From;
        public readonly decimal To;

        internal override bool CanApplyToProperty(Schema.Definition.Property propIn)
        {
            return TypeHelper.IsDecimalType(propIn.Type);
        }

        public DecimalRange(decimal from, decimal to, string customMessage)
            :base(customMessage)
        {
            this.From = from;
            this.To = to;
        }

        public DecimalRange(decimal from, decimal to)
            : this(from, to, Resources.ObjectMessages.Validate_IntRange)
        {
        }

        internal override void Validate(Schema.Definition.Property propIn, object value)
        {
            var dVal = Convert.ToDecimal(value);

            if (dVal < this.From || dVal > this.To)
                throw new ObjectException(this.CustomMessage, propIn.Schema.ClassName, propIn.Name, this.From, this.To);
        }

    }
}
