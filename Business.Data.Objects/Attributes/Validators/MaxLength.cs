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
    /// Lunghezza massima consentita per il campo
    /// (Verrà preconvertito a stringa)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class MaxLength: BaseValidatorAttribute
    {
        public readonly int Value;

        internal override bool CanApplyToProperty(Schema.Definition.Property propIn)
        {
            return TypeHelper.IsString(propIn.Type);
        }

        public MaxLength(int value)
            : this(value, Resources.ObjectMessages.Validate_MaxLength)
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
