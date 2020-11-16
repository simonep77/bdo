using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Common.Resources;
using Business.Data.Objects.Common.Utils;
using System;

namespace Business.Data.Objects.Core.Attributes
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
            : this(from, to, ObjectMessages.Validate_IntRange)
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
