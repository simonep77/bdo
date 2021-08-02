using Business.Data.Objects.Common.Exceptions;
using Business.Data.Objects.Core.Common.Resources;
using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Schema.Definition;
using System;

namespace Business.Data.Objects.Core.Attributes
{
    /// <summary>
    /// Imposta un'espressione regolare per la validazione del campo
    /// (Verrà preconvertito a stringa)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class ValidateRegex : BaseValidatorAttribute
    {
        
        public readonly string Value;

        /// <summary>
        /// Indica se applicabile alla proprieta'
        /// </summary>
        /// <param name="propIn"></param>
        /// <returns></returns>
        internal override bool CanApplyToProperty(Property propIn)
        {
            return TypeHelper.IsString(propIn.Type);
        }

        public ValidateRegex(string regex, string customMessage)
            :base(customMessage)
        {
            this.Value = regex;
        }

        public ValidateRegex(string regex)
            : this(regex, ObjectMessages.Validate_Regex)
        {
        }

        internal override void Validate(Schema.Definition.Property propIn, object value)
        {
            string sVal = value as string;

            if (!System.Text.RegularExpressions.Regex.IsMatch(sVal, this.Value))
                throw new ObjectException(this.CustomMessage, propIn.Schema.ClassName, propIn.Name, this.Value);

        }

    }
}
