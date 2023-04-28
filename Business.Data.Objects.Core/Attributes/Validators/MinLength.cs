using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Core.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Schema.Definition;
using System;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Lunghezza minima consentita per il campo
    /// (Verrà preconvertito a stringa)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class MinLength: BaseValidatorAttribute
    {
        public readonly int Value;

        internal override bool CanApplyToProperty(Property propIn)
        {
            return propIn.Type.IsString();
        }

        public MinLength(int value)
            : this(value, ObjectMessages.Validate_MinLength)
        { }

        public MinLength(int value, string customMessage)
            :base(customMessage)
        {
            this.Value = value;
        }

        internal override void Validate(Schema.Definition.Property propIn, object value)
        {
            string sVal = value as string;

            if (sVal.Length < this.Value)
                throw new ObjectException(this.CustomMessage, propIn.Schema.ClassName, propIn.Name, this.Value);
        }
    }
}
