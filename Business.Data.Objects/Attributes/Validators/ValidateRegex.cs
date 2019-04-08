using System;
using System.Collections.Generic;
using System.Text;
using Bdo.Objects;
using Bdo.Utils;

namespace Bdo.Attributes
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
        internal override bool CanApplyToProperty(Schema.Definition.Property propIn)
        {
            return TypeHelper.IsString(propIn.Type);
        }

        public ValidateRegex(string regex, string customMessage)
            :base(customMessage)
        {
            this.Value = regex;
        }

        public ValidateRegex(string regex)
            : this(regex, Resources.ObjectMessages.Validate_Regex)
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
