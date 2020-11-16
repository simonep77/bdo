using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Schema.Definition;
using System;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Range di valori per proprietà integer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class DoubleRange: BaseValidatorAttribute  
    {
        public readonly double From;
        public readonly double To;

        internal override bool CanApplyToProperty(Property propIn)
        {
            return TypeHelper.IsDecimalType(propIn.Type);
        }

        public DoubleRange(double from, double to, string customMessage)
            :base(customMessage)
        {
            this.From = from;
            this.To = to;
        }

        public DoubleRange(double from, double to)
            : this(from, to, ObjectMessages.Validate_IntRange)
        {
        }

        internal override void Validate(Schema.Definition.Property propIn, object value)
        {
            var dVal = Convert.ToDouble(value);

            if (dVal < this.From || dVal > this.To)
                throw new ObjectException(this.CustomMessage, propIn.Schema.ClassName, propIn.Name, this.From, this.To);
        }

    }
}
