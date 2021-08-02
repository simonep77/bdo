using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Core.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Schema.Definition;
using System;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Lunghezza massima consentita per il campo
    /// (Verrà preconvertito a stringa)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class MaxLength: BaseValidatorAttribute
    {
        public readonly int Value;

        internal override bool CanApplyToProperty(Property propIn)
        {
            return TypeHelper.IsString(propIn.Type);
        }

        public MaxLength(int value)
            : this(value, ObjectMessages.Validate_MaxLength)
        { }

        public MaxLength(int value, string customMessage)
            :base(customMessage)
        {
            this.Value = value;
        }

        internal override void Validate(Schema.Definition.Property propIn, object value)
        {
            string sVal = value as string;

            if (sVal.Length > this.Value)
                throw new ObjectException(this.CustomMessage, propIn.Schema.ClassName, propIn.Name, this.Value);
        }
    }
}
