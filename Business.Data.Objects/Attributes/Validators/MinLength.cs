using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Objects;
using Bdo.Utils;

namespace Bdo.Attributes
{
    /// <summary>
    /// Lunghezza minima consentita per il campo
    /// (Verrà preconvertito a stringa)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class MinLength: BaseValidatorAttribute
    {
        public readonly int Value;

        internal override bool CanApplyToProperty(Schema.Definition.Property propIn)
        {
            return TypeHelper.IsString(propIn.Type);
        }

        public MinLength(int value)
            : this(value, Resources.ObjectMessages.Validate_MinLength)
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
