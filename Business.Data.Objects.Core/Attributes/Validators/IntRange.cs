using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Core.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Schema.Definition;
using System;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Range di valori per proprietà integer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class IntRange: BaseValidatorAttribute  
    {
        public readonly int From;
        public readonly int To;

        internal override bool CanApplyToProperty(Property propIn)
        {
            return propIn.Type.IsIntegerType();
        }

        public IntRange(int from, int to, string customMessage)
            :base(customMessage)
        {
            this.From = from;
            this.To = to;
        }

        public IntRange(int from, int to)
            : this(from, to, ObjectMessages.Validate_IntRange)
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
